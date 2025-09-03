using api.Data;
using api.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (File.Exists(".env"))
{
    Env.Load(".env");
}

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddSingleton<DatabaseInitializer>();

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
        options.UseNpgsql(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableDetailedErrors();
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

// Apply migrations only if not in the "Test" environment
if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();

    // Call the database initializer at startup
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var sqlFilePath = Path.Combine(app.Environment.ContentRootPath, "Sql", "data.sql");
    initializer.InitializeDatabase(sqlFilePath);
}

app.Run();
