using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.DataAccess.Utilities.Extensions.Queryable;

public static class CommentQueryableExtensions
{
    /// <summary>
    /// Incluye User e Incident en consultas de Comment.
    /// </summary>
    public static IQueryable<Comment> WithDetails(this IQueryable<Comment> query)
    {
        return query
            //.Include(c => c.User)
            .Include(c => c.Incident);
    }
}