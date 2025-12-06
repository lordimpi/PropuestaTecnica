using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.DataAccess.Utilities.Extensions.Queryable;

public static class CategoryQueryableExtensions
{
    public static IQueryable<Category> WithDetails(this IQueryable<Category> query)
    {
        return query;
    }
}