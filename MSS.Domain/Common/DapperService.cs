using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace MSS.Domain.Common;

/// <summary>
/// Common Dapper service implementation for database operations
/// </summary>
public class DapperService : IDapperService, IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public DapperService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection GetConnection()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }
        return _connection;
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        var connection = GetConnection();
        _transaction = await Task.Run(() => connection.BeginTransaction());
        return _transaction;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connection = GetConnection();
        return await connection.QueryAsync<T>(sql, param, transaction ?? _transaction, commandTimeout, commandType);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        var connection = GetConnection();
        return await connection.QueryAsync(sql, map, param, transaction ?? _transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        var connection = GetConnection();
        return await connection.QueryAsync(sql, map, param, transaction ?? _transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connection = GetConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction ?? _transaction, commandTimeout, commandType);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connection = GetConnection();
        return await connection.ExecuteAsync(sql, param, transaction ?? _transaction, commandTimeout, commandType);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connection = GetConnection();
        var result = await connection.ExecuteScalarAsync<T>(sql, param, transaction ?? _transaction, commandTimeout, commandType);
        return result ?? default!;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}

