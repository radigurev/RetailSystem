using CentralApp.Extensions;
using CentralApp.Mapping;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddDbContext();

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(CentralProductProfile).Assembly);
builder.AddServices();

builder.AddExceptionHandlers();
builder.Services.AddProblemDetails();
WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();