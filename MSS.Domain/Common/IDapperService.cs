using System.Data;
using Dapper;

 namespace MSS.Domain.Common;

/// <summary>
/// Common Dapper service interface for database operations
/// </summary>
public interface IDapperService
{
    /// <summary>
    /// Execute a query and return a list of entities
    /// </summary>
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// Execute a query that returns multiple types
    /// </summary>
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// Execute a query that returns multiple types
    /// </summary>
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// Execute a query and return a single entity
    /// </summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// Execute a command and return the number of rows affected
    /// </summary>
    Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// Execute a command and return the scalar value
    /// </summary>
    Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task<IDbTransaction> BeginTransactionAsync();

    /// <summary>
    /// Get the database connection
    /// </summary>
    IDbConnection GetConnection();
}

