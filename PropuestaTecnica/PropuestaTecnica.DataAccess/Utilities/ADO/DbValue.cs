namespace PropuestaTecnica.DataAccess.Utilities.ADO;

public static class DbValue
{
    public static object ToDb(object value, bool treatZeroAsNull = false)
    {
        if (value == null) return DBNull.Value;
        if (value is string s && string.IsNullOrWhiteSpace(s)) return DBNull.Value;
        if (treatZeroAsNull && value is IConvertible c && Convert.ToDecimal(c) == 0m) return DBNull.Value;
        return value;
    }

    public static T FromDb<T>(object db)
    {
        if (db == null || db == DBNull.Value) return default!;
        var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(db, t);
    }
}