using Microsoft.Data.SqlClient;
using System.Data;

namespace PropuestaTecnica.DataAccess.Utilities.ADO;

public static class SqlParam
{
    public static SqlParameter In(string name, SqlDbType type, object value) =>
        new SqlParameter($"@{name}", type) { Value = value ?? DBNull.Value };

    public static SqlParameter Out(string name, SqlDbType type, int size = 0)
    {
        var p = new SqlParameter($"@{name}", type) { Direction = ParameterDirection.Output };
        if (size > 0) p.Size = size;
        return p;
    }
}