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
}