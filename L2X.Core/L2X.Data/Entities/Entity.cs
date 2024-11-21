namespace L2X.Data.Entities;

public class Entity<T> : IEntity<T>
{
    object[] IEntity.Keys => [Id!];

    [Key]
    public required T Id { get; set; }
}