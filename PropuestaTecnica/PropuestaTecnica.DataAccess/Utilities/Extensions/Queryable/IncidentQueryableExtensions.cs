using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.DataAccess.Utilities.Extensions.Queryable;

public static class IncidentQueryableExtensions
{
    /// <summary>
    /// Incluye Category y User en consultas de Incident.
    /// </summary>
    public static IQueryable<Incident> WithDetails(this IQueryable<Incident> query)
    {
        return query
            .Include(i => i.Category);
        //.Include(i => i.User);
    }
}