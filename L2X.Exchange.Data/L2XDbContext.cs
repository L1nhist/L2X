using L2X.Exchange.Data.Entities;

namespace L2X.Exchange.Data;

public class L2XDbContext(DbContextOptions<L2XDbContext> options) : DbContext(options)
{
    #region Properties
    public DbSet<Account> Accounts { get; set; }

    public DbSet<Symbol> Symbols { get; set; }

    public DbSet<Member> Members { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Ticker> Tickers { get; set; }
    #endregion

    #region Overridens
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseSerialColumns();

        builder.Entity<Account>().HasOne(a => a.Owner).WithMany(m => m.Accounts).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Account>().HasOne(a => a.Ticker).WithMany(t => t.Accounts).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Symbol>().HasOne(d => d.BaseUnit).WithMany(t => t.BaseSymbols).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Symbol>().HasOne(d => d.QuoteUnit).WithMany(t => t.QuoteSymbols).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>().HasOne(o => o.Owner).WithMany(m => m.Orders).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Order>().HasOne(o => o.Symbol).WithMany(i => i.Orders).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ticker>().HasOne(t => t.Parent).WithMany(t => t.Children).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Account>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Symbol>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Member>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Ticker>().HasQueryFilter(x => !x.IsDeleted);
    }
    #endregion
}