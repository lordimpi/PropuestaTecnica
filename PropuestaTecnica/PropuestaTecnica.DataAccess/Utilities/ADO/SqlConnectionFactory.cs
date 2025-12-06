using Microsoft.Data.SqlClient;
using PropuestaTecnica.Common.Configurations;
using PropuestaTecnica.Common.IOptionPattern;

namespace PropuestaTecnica.DataAccess.Utilities.ADO;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _cs;

    public SqlConnectionFactory(IGenericOptionsService<DbOptions> genericOptionsService) => _cs = genericOptionsService.GetSnapshotOptions().DefaultConnection;

    public SqlConnection Create() => new SqlConnection(_cs);
}