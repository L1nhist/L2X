using L2X.Core.Structures;
using System.Diagnostics.CodeAnalysis;

namespace L2X.Data.Entities;

public class Entity<T> : IEntity<T>
{
    object[] IEntity.Keys => [Id!];

    [Key]
    [Required]
    [NotNull]
    public T Id { get; set; }
}

public class Entity : Entity<Guid>
{
    public Entity()
        => Id = Uuid.NewCode;
}