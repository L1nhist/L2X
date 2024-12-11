using System.Diagnostics.CodeAnalysis;

namespace L2X.Data.Entities;

public interface IEntity
{
    object[] Keys { get; }
}

public interface IEntity<T> : IEntity
{
    [Key]
    [Required]
    [NotNull]
    T Id { get; set; }
}