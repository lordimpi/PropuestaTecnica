using System.Data;
using System.Reflection;

namespace PropuestaTecnica.DataAccess.Utilities.ADO;

public static class DataReaderMapper
{
    public static List<T> MapToList<T>(IDataReader dr) where T : new()
    {
        var result = new List<T>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanWrite).ToArray();

        var ordinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < dr.FieldCount; i++)
            ordinals[dr.GetName(i)] = i;

        while (dr.Read())
        {
            var item = new T();
            foreach (var p in props)
            {
                if (!ordinals.TryGetValue(p.Name, out var ord)) continue;
                var val = dr.GetValue(ord);
                if (val == DBNull.Value) { p.SetValue(item, null); continue; }

                var targetType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                p.SetValue(item, Convert.ChangeType(val, targetType));
            }
            result.Add(item);
        }
        return result;
    }
}