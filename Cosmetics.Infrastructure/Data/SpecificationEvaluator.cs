using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cosmetics.Infrastructure.Data;

/// <summary>
/// Specification Evaluator - Áp dụng Specification lên IQueryable.
/// </summary>
public static class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Áp dụng Includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // Áp dụng sắp xếp
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Áp dụng phân trang
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}
