# RetailSystem

RetailSystem is a simple **two-service** demo for a retail scenario:

- **StoreApp** – a “store” backend that owns its products and publishes changes.
- **CentralApp** – a “central” backend that aggregates products from all stores.
- **Shared** – shared DTOs, messaging & infrastructure (RabbitMQ, caching, base EF entities, etc.).

Communication between StoreApp and CentralApp is done through **RabbitMQ** using **exchanges and queues**.  
Both services use **SQL Server** and **EF Core** for persistence, and **FusionCache** for caching configuration.

---

## 1. Prerequisites

- .NET SDK 10.0 (or whatever the solution targets)
- **SQL Server** instance (local or remote)
- **RabbitMQ** (easiest via Docker)
- (Optional, for CLI migrations) `dotnet-ef` global tool:

```bash
dotnet tool install --global dotnet-ef
```

Project layout:

- `Shared/` – common code (DTOs, messaging, cache abstraction, base EF entities).
- `StoreApp/` – store worker + HTTP API (`/swagger` on port 4300).
- `CentralApp/` – central worker + HTTP API (`/swagger` on its Kestrel port).

---

## 2. Start RabbitMQ (Docker)

The easiest way to run RabbitMQ locally is with the management image:

```bash
docker run -d   --hostname retail-rabbit   --name retail-rabbit   -p 5672:5672   -p 15672:15672   rabbitmq:3-management
```

- **AMQP port**: `5672`
- **Management UI**: <http://localhost:15672>
- **Default credentials**: `guest` / `guest`

You can log into the management UI and later verify exchanges/queues:

- Exchanges: `store-sync`, `central-sync`
- Queues: configured through the config tables (see below).

---

## 3. Application configuration (appsettings)

Both **StoreApp** and **CentralApp** need:

- A **SQL Server connection string**
- A **RabbitMQ** configuration section

> The example below uses your public `somee.com` connection string.  
> In a real deployment, move secrets to user secrets or environment variables.

### 3.1 StoreApp `appsettings.json`

File: `StoreApp/appsettings.json`

```json
{
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://localhost:4300"
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "StoreConnection": ""
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

StoreApp uses:

```csharp
builder.Configuration.GetConnectionString("StoreConnection");
```

So the key **must** be `"StoreConnection"`.

---

### 3.2 CentralApp `appsettings.json`

File: `CentralApp/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "CentralConnection": ""
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

CentralApp uses:

```csharp
builder.Configuration.GetConnectionString("CentralConnection");
```

So the key **must** be `"CentralConnection"`.

---

## 4. Initialize the databases (EF Core migrations)

The solution contains migrations for both contexts:

- `StoreApp` → `StoreDbContext`
- `CentralApp` → `CentralDbContext`

From the **solution root** (`RetailSystem/`):

### 4.1 Store database

```bash
cd StoreApp
dotnet ef database update --context StoreDbContext
cd ..
```

This will create at least:

- `StoreProducts`
- `StoreConfig`

### 4.2 Central database

```bash
cd CentralApp
dotnet ef database update --context CentralDbContext
cd ..
```

This will create at least:

- `Products`
- `Stores`
- `Config`

Make sure the configured SQL Server user can create/alter the database.

---

## 5. Configure the config tables

The **config tables** drive RabbitMQ behaviour (exchanges, queues, routing) and store identity.

There are two separate tables:

- `StoreConfig` in the **Store DB** (`StoreApp`)
- `Config` in the **Central DB** (`CentralApp`)

Additionally, the **Central DB** has a `Stores` table that must contain at least one store record matching the Store’s GUID.

### 5.1 Store DB – `StoreConfig` table

Model: `StoreApp.Database.Models.Config`  
Table: `StoreConfig`

Columns include: `Id`, `Key`, `Value`, `Description`, `CreatedAt`, `UpdatedAt`, etc.

**Required keys** (from Store helpers and RabbitMQ services):

| Key              | Example Value                       | Used For                                                                 |
|------------------|-------------------------------------|-------------------------------------------------------------------------|
| `StoreId`        | `8d24d7c3-0000-0000-0000-0000000001`| Unique GUID for this store. Sent in `ProductSyncMessage.StoreGuid`.    |
| `CentralSyncKey` | `store-sync`                        | Name of the **exchange** used for Store → Central product sync.        |
| `StoreRoutingKey`| `Sofia`                             | Routing key when publishing to `store-sync`.                           |
| `StoreQueueKey`  | `store.sofia.q`                     | Name of the queue used by the **StoreApp consumer**.                   |

Typical example rows:

- `Key = 'StoreId'`, `Value = '8d24d7c3-0000-0000-0000-0000000001'`
- `Key = 'CentralSyncKey'`, `Value = 'store-sync'`
- `Key = 'StoreRoutingKey'`, `Value = 'Sofia'`
- `Key = 'StoreQueueKey'`, `Value = 'store.sofia.q'`

Insert them using SSMS / Azure Data Studio / etc.  
After changing config values, **restart StoreApp** because values are cached with FusionCache.

---

### 5.2 Central DB – `Config` table

Model: `CentralApp.Database.Models.Config`  
Table: `Config`

**Required keys** (from Central helpers and RabbitMQ services):

| Key               | Example Value   | Used For                                                                 |
|-------------------|-----------------|-------------------------------------------------------------------------|
| `CentralQueueKey` | `central.q`     | Name of the queue used by the **CentralApp consumer**.                 |
| `StoreSyncKey`    | `central-sync`  | Name of the **exchange** used for Central → Stores broadcasts.         |

Typical example rows:

- `Key = 'CentralQueueKey'`, `Value = 'central.q'`
- `Key = 'StoreSyncKey'`, `Value = 'central-sync'`

Again, insert via SQL and **restart CentralApp** after changes.

---

### 5.3 Central DB – `Stores` table

Model: `CentralApp.Database.Models.CentralStore`  
Table: `Stores`

When a `ProductSyncMessage` arrives, `CentralApp` resolves a store by:

```csharp
x => x.Id == message.StoreGuid;
```

So the **Id in `Stores` must match the `StoreId` configured in `StoreConfig`**.

Required fields:

- `Id` (GUID) – must equal the store’s `StoreId`.
- `Name` (string) – e.g. `"Sofia Store"`.
- `Code` (optional) – e.g. `"SOF"`.
- `RoutingKey` (required) – e.g. `"Sofia"` (should match `StoreRoutingKey` in `StoreConfig`).

Example row:

- `Id` = `8d24d7c3-0000-0000-0000-0000000001`
- `Name` = `Sofia Store`
- `RoutingKey` = `Sofia`

If this row is missing, the central product sync handler will fail to resolve the store.

---

## 6. RabbitMQ exchanges used by the project

Exchanges are declared at startup using `ExchangeDeclareDTO` helpers.

### 6.1 StoreApp exchanges

Declared in `StoreApp` startup:

- **Exchange:** `store-sync`
- **Type:** `direct`
- **Durable:** `true`
- **AutoDelete:** `false`

StoreApp:

- Publishes **product changes** to `store-sync` with routing key from `StoreRoutingKey`.
- Declares a queue named `StoreQueueKey` and binds it to `store-sync`.

---

### 6.2 CentralApp exchanges

Declared in `CentralApp` startup:

- **Exchange:** `central-sync`
- **Type:** `fanout`
- **Durable:** `true`
- **AutoDelete:** `false`

CentralApp:

- Publishes **updates back to stores** to `central-sync` (fanout).
- Declares a queue named `CentralQueueKey` and binds it.

> **Summary:**
>
> - `store-sync` (`direct`): Store → Central  
> - `central-sync` (`fanout`): Central → all Stores

---

## 7. Running the applications

From solution root (`RetailSystem/`):

### 7.1 Run CentralApp

```bash
dotnet run --project CentralApp/CentralApp.csproj
```

- HTTP + Swagger in Development:
  - `https://localhost:5001/swagger` or
  - `http://localhost:5000/swagger` (depending on Kestrel configuration)

CentralApp responsibilities:

- Keeps central **Products** and **Stores**.
- Consumes product sync messages from stores.
- Publishes broadcasts back to stores (if/when needed).

---

### 7.2 Run StoreApp

```bash
dotnet run --project StoreApp/StoreApp.csproj
```

- HTTP API at: `http://localhost:4300`
- Swagger UI: `http://localhost:4300/swagger`

StoreApp responsibilities:

- Owns local `StoreProducts`.
- Reads `StoreConfig` for identity and MQ configuration.
- Sends `ProductSyncMessage` to RabbitMQ when products are created/updated/deleted.
- Can receive messages from central via its configured queue.

---

## 8. Typical flow (high-level)

1. **Store configuration** is set:
   - `StoreConfig` table populated (`StoreId`, `CentralSyncKey`, `StoreRoutingKey`, `StoreQueueKey`).
2. **Central configuration** is set:
   - `Config` table populated (`CentralQueueKey`, `StoreSyncKey`).
   - `Stores` table has an entry whose `Id` equals the store’s `StoreId`.
3. **RabbitMQ** is running with exchanges:
   - `store-sync` (direct),
   - `central-sync` (fanout).
4. **Apps start**:
   - RabbitMQ connections are initialized.
   - Exchanges and queues are declared/bound based on config.
   - Config values are cached via FusionCache.
5. When a product is created/updated in **StoreApp**:
   - StoreApp reads `StoreId` from config.
   - Builds a `ProductSyncMessage`.
   - Publishes it to `store-sync` with routing key `StoreRoutingKey`.
6. **CentralApp** receives the message:
   - Resolves the store in `Stores` by `StoreGuid`.
   - Upserts the product into central `Products` table.
   - (Optionally) broadcasts changes back to all stores via `central-sync`.

---

## 9. Notes & troubleshooting

- If you change **config table values** (StoreConfig or Config), restart the corresponding app so the FusionCache-backed config is refreshed.
- If RabbitMQ is not reachable:
  - Check Docker container is running.
  - Check `RabbitMQ:HostName`, `UserName`, `Password` in both apps.
- If you see something like `ConfigMissingException`:
  - Ensure the required keys exist and are spelled exactly:
    - `StoreId`, `CentralSyncKey`, `StoreRoutingKey`, `StoreQueueKey`
    - `CentralQueueKey`, `StoreSyncKey`
- If a `Store` cannot be found in central:
  - Check the `Stores` table has an entry whose `Id` matches the `StoreId` from `StoreConfig`.

---

## 10. What this project demonstrates

- Basic **microservice-style split**: Store vs Central.
- **RabbitMQ messaging** with:
  - `direct` exchange for targeted store → central traffic.
  - `fanout` exchange for central → all stores broadcasts.
- **EF Core** with separate contexts and databases.
- **Config-driven** behaviour via DB tables + **FusionCache**.
- A clean separation of:
  - **Shared abstractions and DTOs** in `Shared`.
  - **Store-specific logic** in `StoreApp`.
  - **Central aggregation logic** in `CentralApp`.
