using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MigrationTool.Services;
using DotNetEnv;

namespace MigrationTool;

class Program
{
    static async Task Main(string[] args)
    {
        // Load environment variables from .env files
        Env.Load("../sqlProject/api/.env");
        Env.Load("../.env");

        Console.WriteLine("=== PostgreSQL to MongoDB Migration Tool ===");
        Console.WriteLine();

        try
        {
            // Build configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = configBuilder.Build();

            // Build connection strings from environment variables
            // PostgreSQL: Use existing connection string from .env or fallback to default
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
            var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PW");
        
        if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Missing required PostgreSQL environment variables: POSTGRES_DB, POSTGRES_USER, POSTGRES_PW");
        }
        
        var postgresConnection = Environment.GetEnvironmentVariable("ConnectionString") ?? 
                $"server={host};port={port};database={database};userid={username};password={password}";
            
            // MongoDB: Build from individual env vars or use fallback values
            var mongoUser = Environment.GetEnvironmentVariable("MONGO_APP_USER") ?? "appuser";
            var mongoPw = Environment.GetEnvironmentVariable("MONGO_APP_PW") ?? "apppassword123";
            var mongoDb = Environment.GetEnvironmentVariable("MONGO_DB") ?? "ab_database_mongo";
            var mongoConnection = $"mongodb://{mongoUser}:{mongoPw}@localhost:27017/{mongoDb}?authSource={mongoDb}";

            // Override connection strings with environment-based ones
            configuration["ConnectionStrings:PostgresConnection"] = postgresConnection;
            configuration["ConnectionStrings:MongoConnection"] = mongoConnection;
            
            Console.WriteLine($"Using PostgreSQL: {postgresConnection.Replace("password123!", "***")}");
            Console.WriteLine($"Using MongoDB: {mongoConnection.Replace(mongoPw, "***")}");

            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            // Get migration service
            var migrationService = serviceProvider.GetRequiredService<MigrationService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Parse command line arguments
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "test":
                        await TestConnectionsCommand(migrationService, logger);
                        break;
                    case "migrate":
                        await MigrateCommand(migrationService, logger);
                        break;
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {args[0]}");
                        ShowHelp();
                        break;
                }
            }
            else
            {
                // Interactive mode
                await RunInteractiveMode(migrationService, logger);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddConfiguration(configuration.GetSection("Logging"));
        });

        // Add configuration
        services.AddSingleton(configuration);

        // Add our services
        services.AddTransient<PostgresService>();
        services.AddTransient<MongoService>();
        services.AddTransient<MigrationService>();
    }

    private static async Task RunInteractiveMode(MigrationService migrationService, ILogger<Program> logger)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Test database connections");
            Console.WriteLine("2. Run full migration");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice (1-3): ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await TestConnectionsCommand(migrationService, logger);
                    break;
                case "2":
                    await MigrateCommand(migrationService, logger);
                    break;
                case "3":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                    break;
            }
        }
    }

    private static async Task TestConnectionsCommand(MigrationService migrationService, ILogger<Program> logger)
    {
        Console.WriteLine();
        Console.WriteLine("Testing database connections...");
        
        var success = await migrationService.TestConnectionsAsync();
        
        if (success)
        {
            Console.WriteLine("✅ All database connections successful!");
        }
        else
        {
            Console.WriteLine("❌ Database connection tests failed. Check your configuration and ensure databases are running.");
        }
    }

    private static async Task MigrateCommand(MigrationService migrationService, ILogger<Program> logger)
    {
        Console.WriteLine();
        Console.WriteLine("⚠️  WARNING: This will migrate all data from PostgreSQL to MongoDB.");
        Console.WriteLine("Make sure your MongoDB database is empty or you want to overwrite existing data.");
        Console.Write("Do you want to continue? (y/N): ");

        var confirm = Console.ReadLine();
        if (confirm?.ToLower() != "y" && confirm?.ToLower() != "yes")
        {
            Console.WriteLine("Migration cancelled.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Starting migration...");
        
        var startTime = DateTime.Now;
        
        try
        {
            await migrationService.RunFullMigrationAsync();
            
            var duration = DateTime.Now - startTime;
            Console.WriteLine();
            Console.WriteLine($"✅ Migration completed successfully in {duration.TotalSeconds:F1} seconds!");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"❌ Migration failed: {ex.Message}");
            logger.LogError(ex, "Migration failed");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("PostgreSQL to MongoDB Migration Tool");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  MigrationTool [command]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  test      Test database connections");
        Console.WriteLine("  migrate   Run full migration from PostgreSQL to MongoDB");
        Console.WriteLine("  help      Show this help message");
        Console.WriteLine();
        Console.WriteLine("If no command is provided, the tool runs in interactive mode.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  MigrationTool test");
        Console.WriteLine("  MigrationTool migrate");
    }
}
