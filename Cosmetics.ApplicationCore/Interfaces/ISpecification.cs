using System.Linq.Expressions;
using Cosmetics.ApplicationCore.Entities;

namespace Cosmetics.ApplicationCore.Interfaces;

/// <summary>
/// Specification Pattern - Đóng gói logic truy vấn theo Clean Architecture.
/// </summary>
public interface ISpecification<T> where T : BaseEntity
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}
