using System.Text;
using api.Data;
using api.ExceptionHandlers;
using api.Repositories;
using api.Repositories.Interfaces;
using api.Services;
using api.Services.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from root .env file
Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, AuthRepository>();
builder.Services.AddScoped<IPrivilegesRepository, PrivilegesRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<SqlRepository>();
builder.Services.AddScoped<MongoRepository>();
builder.Services.AddScoped<Neo4jRepository>();

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
            {
                throw new InvalidOperationException("JWT_KEY environment variable is not set.");
            }

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

// MongoDB: Build from individual env vars or use fallback values
var mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "localhost";
var mongoPort = Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017";
var mongoUser = Environment.GetEnvironmentVariable("MONGO_APP_USER") ?? "appuser";
var mongoPw = Environment.GetEnvironmentVariable("MONGO_APP_PW") ?? "apppassword123";
var mongoDb = Environment.GetEnvironmentVariable("MONGO_DB") ?? "ab_database_mongo";
var mongoConnection =
    $"mongodb://{mongoUser}:{mongoPw}@{mongoHost}:{mongoPort}/{mongoDb}?authSource={mongoDb}";

builder.Configuration["MongoConnectionString"] = mongoConnection;

// Mongo Client and Database configuration
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString =
        config["MongoConnectionString"] ?? throw new ArgumentNullException("MongoConnectionString");
    var mongoUrl = MongoUrl.Create(connectionString);
    return new MongoClient(mongoUrl);
});

builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString =
        config["MongoConnectionString"] ?? throw new ArgumentNullException("MongoConnectionString");
    var mongoUrl = MongoUrl.Create(connectionString);
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoUrl.DatabaseName);
});

// Neo4j Driver configuration
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var neo4j = config.GetSection("Neo4j");
    var uri = neo4j["Uri"] ?? throw new ArgumentNullException("Neo4j:Uri");
    var user = neo4j["User"] ?? throw new ArgumentNullException("Neo4j:User");
    var password = Environment.GetEnvironmentVariable("NEO4J_PW");
    if (string.IsNullOrEmpty(password))
    {
        throw new InvalidOperationException("NEO4J_PW environment variable is not set.");
    }

    return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
