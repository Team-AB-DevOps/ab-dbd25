using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using api.Data;

namespace MigrationTool.Services;

public class PostgresService
{
    private readonly DataContext _context;
    private readonly ILogger<PostgresService> _logger;

    public PostgresService(IConfiguration configuration, ILogger<PostgresService> logger)
    {
        _logger = logger;
        
        var connectionString = configuration.GetConnectionString("PostgresConnection");
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        _context = new DataContext(options);
        
        _logger.LogInformation("Connected to PostgreSQL database");
    }

    public DataContext GetContext() => _context;

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await _context.Database.CanConnectAsync();
            _logger.LogInformation("PostgreSQL connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostgreSQL connection test failed");
            return false;
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
