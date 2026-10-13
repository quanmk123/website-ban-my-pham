namespace Cosmetics.ApplicationCore.Entities;

/// <summary>
/// Base class cho tất cả Entity trong hệ thống.
/// Cung cấp Id mặc định kiểu int.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}
