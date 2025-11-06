using Npgsql;

namespace api.Services;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionString"];
    }

    public void InitializeDatabase(string sqlFilePath)
    {
        var sqlScript = File.ReadAllText(sqlFilePath);

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(sqlScript, connection);
        command.ExecuteNonQuery();
    }

    public void InitializeUsersAndRoles(string sqlFilePath)
    {
        var sqlScript = File.ReadAllText(sqlFilePath);

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var checkCmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM pg_roles WHERE rolname = 'app_readonly'",
            connection
        );

        var exists = (long)(checkCmd.ExecuteScalar() ?? 0L) > 0;

        if (exists)
        {
            return;
        }

        sqlScript = sqlScript
            .Replace(
                "${APP_READER_PASSWORD}",
                Environment.GetEnvironmentVariable("APP_READER_PASSWORD")
                    ?? throw new InvalidOperationException("APP_READER_PASSWORD not set")
            )
            .Replace(
                "${APP_WRITER_PASSWORD}",
                Environment.GetEnvironmentVariable("APP_WRITER_PASSWORD")
                    ?? throw new InvalidOperationException("APP_WRITER_PASSWORD not set")
            )
            .Replace(
                "${ADMIN_PASSWORD}",
                Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                    ?? throw new InvalidOperationException("ADMIN_PASSWORD not set")
            );

        using var command = new NpgsqlCommand(sqlScript, connection);
        command.ExecuteNonQuery();
    }
}
