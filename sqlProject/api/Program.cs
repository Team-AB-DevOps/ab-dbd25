using System.Text;
using api.Data;
using api.ExceptionHandlers;
using api.Interfaces;
using api.Repositories;
using api.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from root .env file
Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPrivilegesRepository, PrivilegesRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddSingleton<DatabaseInitializer>();

// Custom ExceptionHandlers
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

// JWT Authentication
builder
    .Services.AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT_KEY environment variable is not set.");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            };
        }
    );

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

if (builder.Environment.EnvironmentName == "Test")
{
    //TODO: In-memory db for integration tests
    // builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("DataSource=:memory:"));
}
else
{
    // PostgreSQL - Build connection string from environment variables
    var connectionString = builder.Configuration.GetValue<string>("ConnectionString");

    // If no hardcoded connection string, build from environment variables
    if (string.IsNullOrEmpty(connectionString))
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PW");

        if (
            string.IsNullOrEmpty(database)
            || string.IsNullOrEmpty(username)
            || string.IsNullOrEmpty(password)
        )
        {
            throw new InvalidOperationException(
                "Missing required PostgreSQL environment variables: POSTGRES_DB, POSTGRES_USER, POSTGRES_PW"
            );
        }

        connectionString =
            $"server={host};port={port};database={database};userid={username};password={password}";
    }

    // Make connection string available to other services (e.g., DatabaseInitializer)
    builder.Configuration["ConnectionString"] = connectionString;

    builder.Services.AddDbContext<DataContext>(options =>
    {
        options
            .UseNpgsql(connectionString)
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

app.UseAuthentication();
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
    var seedData = Path.Combine(app.Environment.ContentRootPath, "Sql", "data.sql");
    var storedObjects = Path.Combine(app.Environment.ContentRootPath, "Sql", "stored_objects.sql");

    initializer.InitializeDatabase(seedData);
    initializer.InitializeDatabase(storedObjects);
}

app.Run();
