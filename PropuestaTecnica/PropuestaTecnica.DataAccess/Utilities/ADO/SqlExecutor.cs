using Microsoft.Data.SqlClient;
using System.Data;

namespace PropuestaTecnica.DataAccess.Utilities.ADO;

public interface ISqlExecutor
{
    Task<int> NonQueryAsync(string procOrSql, CommandType type, SqlParameter[]? parameters = null,
        SqlTransaction? tx = null, int? timeout = null, CancellationToken ct = default);

    Task<object> ScalarAsync(string procOrSql, CommandType type, SqlParameter[]? parameters = null,
        SqlTransaction? tx = null, int? timeout = null, CancellationToken ct = default);

    Task<List<T>> QueryAsync<T>(string procOrSql, CommandType type, Func<IDataReader, List<T>> mapper,
        SqlParameter[]? parameters = null, SqlTransaction? tx = null, int? timeout = null, CancellationToken ct = default);
}

public sealed class SqlExecutor : ISqlExecutor
{
    private readonly ISqlConnectionFactory _factory;
    private readonly int _defaultTimeout;

    public SqlExecutor(ISqlConnectionFactory factory, int defaultTimeout = 30)
    {
        _factory = factory;
        _defaultTimeout = defaultTimeout;
    }

    public async Task<int> NonQueryAsync(string text, CommandType type, SqlParameter[]? parameters = null,
        SqlTransaction? tx = null, int? timeout = null, CancellationToken ct = default)
    {
        using var conn = tx?.Connection ?? _factory.Create();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(text, conn, tx) { CommandType = type, CommandTimeout = timeout ?? _defaultTimeout };
        if (parameters?.Length > 0) cmd.Parameters.AddRange(parameters);
        return await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<object> ScalarAsync(string text, CommandType type, SqlParameter[]? parameters = null,
        SqlTransaction? tx = null, int? timeout = null, CancellationToken ct = default)
    {
        using var conn = tx?.Connection ?? _factory.Create();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(text, conn, tx) { CommandType = type, CommandTimeout = timeout ?? _defaultTimeout };
        if (parameters?.Length > 0) cmd.Parameters.AddRange(parameters);
        return await cmd.ExecuteScalarAsync(ct);
    }

    public async Task<List<T>> QueryAsync<T>(string text, CommandType type, Func<IDataReader, List<T>> mapper,
        SqlParameter[]? parameters = null, SqlTransaction? tx = null, int? timeout = null, CancellationToken ct = default)
    {
        using var conn = tx?.Connection ?? _factory.Create();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(text, conn, tx) { CommandType = type, CommandTimeout = timeout ?? _defaultTimeout };
        if (parameters?.Length > 0) cmd.Parameters.AddRange(parameters);

        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, ct);
        return mapper(reader);
    }
}