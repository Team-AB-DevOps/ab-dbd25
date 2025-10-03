using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SqlToGraph.Services;
using DotNetEnv;

namespace SqlToGraph;

class Program
{
    static async Task Main(string[] args)
    {
        // Load environment variables from .env files
        Env.Load("../../../.env");

        Console.WriteLine("=== PostgreSQL to Neo4j Migration Tool ===");
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

            var postgresConnection =
                Environment.GetEnvironmentVariable("ConnectionString")
                ?? $"server={host};port={port};database={database};userid={username};password={password}";

            // Override connection strings with environment-based ones
            configuration["ConnectionStrings:PostgreSql"] = postgresConnection;

            Console.WriteLine($"Using PostgreSQL: {postgresConnection.Replace(password, "***")}");

            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            // Interactive mode
            await RunInteractiveMode(serviceProvider);
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

        // Add Entity Framework DbContext with proper scoping
        services.AddDbContext<api.Data.DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

        // Add our services as scoped to work properly with DbContext
        services.AddScoped<IPostgreSqlDataService, PostgreSqlDataService>();
        services.AddScoped<INeo4jMigrationService, Neo4jMigrationService>();
        services.AddScoped<IMigrationOrchestrator, MigrationOrchestrator>();
    }

    private static async Task RunInteractiveMode(IServiceProvider serviceProvider)
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
                    await TestConnectionsCommand(serviceProvider);
                    break;
                case "2":
                    await MigrateCommand(serviceProvider);
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
    private static async Task TestConnectionsCommand(IServiceProvider serviceProvider)
    {
        Console.WriteLine();
        Console.WriteLine("Testing database connections...");

        using var scope = serviceProvider.CreateScope();
        var migrationOrchestrator = scope.ServiceProvider.GetRequiredService<IMigrationOrchestrator>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Test PostgreSQL connection
            Console.Write("Testing PostgreSQL connection... ");
            await migrationOrchestrator.TestPostgreSqlConnectionAsync();
            Console.WriteLine("✅ Connected");

            // Test Neo4j connection
            Console.Write("Testing Neo4j connection... ");
            await migrationOrchestrator.TestNeo4jConnectionAsync();
            Console.WriteLine("✅ Connected");

            Console.WriteLine();
            Console.WriteLine("✅ All database connections successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed");
            Console.WriteLine();
            Console.WriteLine($"❌ Database connection tests failed: {ex.Message}");
            Console.WriteLine("Check your configuration and ensure databases are running.");
        }
    }

    private static async Task MigrateCommand(IServiceProvider serviceProvider)
    {
        Console.WriteLine();
        Console.WriteLine("⚠️  WARNING: This will migrate all data from PostgreSQL to Neo4j.");
        Console.WriteLine(
            "Make sure your Neo4j database is empty or you want to overwrite existing data."
        );
        Console.Write("Do you want to continue? (y/N): ");

        var confirm = Console.ReadLine();
        if (confirm?.ToLower() != "y" && confirm?.ToLower() != "yes")
        {
            Console.WriteLine("Migration cancelled.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Starting migration...");
        Console.WriteLine("This may take several minutes depending on data size.");

        var startTime = DateTime.Now;

        using var scope = serviceProvider.CreateScope();
        var migrationOrchestrator = scope.ServiceProvider.GetRequiredService<IMigrationOrchestrator>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            await migrationOrchestrator.RunMigrationAsync();

            var duration = DateTime.Now - startTime;
            Console.WriteLine();
            Console.WriteLine(
                $"✅ Migration completed successfully in {duration.TotalSeconds:F1} seconds!"
            );
            Console.WriteLine("You can now query your Neo4j database with the migrated data.");
            Console.WriteLine();
            Console.WriteLine("Example Cypher queries to try:");
            Console.WriteLine("  MATCH (u:User)-[:OWNS]->(p:Profile) RETURN u.firstName, p.name");
            Console.WriteLine("  MATCH (m:Media)-[:BELONGS_TO_GENRE]->(g:Genre) RETURN m.name, g.name");
            Console.WriteLine("  MATCH (u:User)-[:SUBSCRIBES_TO]->(s:Subscription)-[:GIVES_ACCESS_TO]->(g:Genre) RETURN u.firstName, s.name, collect(g.name)");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"❌ Migration failed: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Please check:");
            Console.WriteLine("1. PostgreSQL connection string and database access");
            Console.WriteLine("2. Neo4j connection details and database access");
            Console.WriteLine("3. Both databases are running and accessible");
            Console.WriteLine("4. User has proper permissions on both databases");
            
            logger.LogError(ex, "Migration failed");
        }
    }
}
