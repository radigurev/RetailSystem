using Microsoft.AspNetCore.Builder;
using StoreApp.Helpers;
using StoreApp.Mapping;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(StoreProductProfile).Assembly);
builder.AddExceptionHandlers();
builder.AddDbContext();
builder.AddServices();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();