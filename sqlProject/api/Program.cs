using api.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (File.Exists(".env"))
{
    Env.Load(".env");
}

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


if (builder.Environment.EnvironmentName == "Test")
{
    //TODO: In-memory db for integration tests
    // builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("DataSource=:memory:"));
}
else
{
    // PostgreSQL
    var connectionString = builder.Configuration.GetValue<string>("ConnectionString");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("ConnectionString not found");
    }

    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseNpgsql(connectionString).EnableDetailedErrors();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
