using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace MSS.Api.Services;

public interface IDatabaseSetupService
{
    Task<DatabaseSetupResult> InitializeAsync(CancellationToken cancellationToken = default);
}

public class DatabaseSetupService : IDatabaseSetupService
{
    private readonly string _connectionString;
    private readonly string _contentRootPath;
    private readonly ILogger<DatabaseSetupService> _logger;

    public DatabaseSetupService(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<DatabaseSetupService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _contentRootPath = environment.ContentRootPath;
        _logger = logger;
    }

    public async Task<DatabaseSetupResult> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var databaseName = GetDatabaseName();
        var masterConnectionString = BuildMasterConnectionString();

        var result = new DatabaseSetupResult();

        result.DatabaseCreated = await EnsureDatabaseExistsAsync(masterConnectionString, databaseName, cancellationToken);

        await ExecuteScriptAsync(GetScriptPath("Schema.sql"), cancellationToken);
        result.SchemaApplied = true;

        await ExecuteScriptAsync(GetScriptPath("SeedData.sql"), cancellationToken);
        result.SeedDataApplied = true;

        return result;
    }

    private string GetScriptPath(string fileName)
    {
        var outputPath = Path.Combine(AppContext.BaseDirectory, "Database", fileName);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        var projectPath = Path.Combine(_contentRootPath, "..", "Database", fileName);
        if (File.Exists(projectPath))
        {
            return projectPath;
        }

        return outputPath;
    }

    private string GetDatabaseName()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            throw new InvalidOperationException("The DefaultConnection does not specify a database name (Initial Catalog).");
        }

        return builder.InitialCatalog;
    }

    private string BuildMasterConnectionString()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        };

        return builder.ConnectionString;
    }

    private async Task<bool> EnsureDatabaseExistsAsync(string masterConnectionString, string databaseName, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            IF DB_ID(@dbName) IS NULL
            BEGIN
                EXEC('CREATE DATABASE [' + @dbName + ']');
                SELECT 1;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;
        command.Parameters.AddWithValue("@dbName", databaseName);

        var created = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
        _logger.LogInformation("Database {DatabaseName} {Action}.", databaseName, created == 1 ? "created" : "already existed");

        return created == 1;
    }

    private async Task ExecuteScriptAsync(string scriptPath, CancellationToken cancellationToken)
    {
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"SQL script not found at path '{scriptPath}'.", scriptPath);
        }

        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = trimmed;
            command.CommandTimeout = 120;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}

public class DatabaseSetupResult
{
    public bool DatabaseCreated { get; set; }
    public bool SchemaApplied { get; set; }
    public bool SeedDataApplied { get; set; }
}

