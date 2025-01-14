using L2X.Exchange.Data.Entities;

namespace L2X.Exchange.Data;

public class L2XDbContext(DbContextOptions<L2XDbContext> options) : DbContext(options)
{
    #region Properties
    public DbSet<Account> Accounts { get; set; }

    public DbSet<Market> Markets { get; set; }

    public DbSet<Member> Members { get; set; }

    public DbSet<Match> Matches { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<PreOrder> PreOrders { get; set; }

    public DbSet<Ticker> Tickers { get; set; }
    #endregion

    #region Overridens
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseSerialColumns();
        builder.UseIdentityColumns();

		builder.Entity<Account>().HasOne(a => a.Owner).WithMany(m => m.Accounts).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Account>().HasOne(a => a.Ticker).WithMany(t => t.Accounts).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Match>().HasOne(m => m.Maker).WithMany(m => m.MakerMatchers).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Match>().HasOne(m => m.Taker).WithMany(m => m.TakerMatchers).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Match>().HasOne(m => m.MakerOrder).WithMany(o => o.MakerMatchers).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Match>().HasOne(m => m.TakerOrder).WithMany(o => o.TakerMatchers).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Match>().HasOne(m => m.Market).WithMany(s => s.Matchers).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>().HasOne(o => o.Owner).WithMany(m => m.Orders).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Order>().HasOne(o => o.Market).WithMany(i => i.Orders).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Order>().Property(o => o.OrderNo).UseIdentityByDefaultColumn();

        builder.Entity<PreOrder>().HasOne(o => o.Owner).WithMany(m => m.PreOrders).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<PreOrder>().HasOne(o => o.Market).WithMany(i => i.PreOrders).OnDelete(DeleteBehavior.Cascade);
		builder.Entity<PreOrder>().Property(o => o.Id).UseIdentityAlwaysColumn();

		builder.Entity<Market>().HasOne(d => d.BaseUnit).WithMany(t => t.BaseSymbols).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Market>().HasOne(d => d.QuoteUnit).WithMany(t => t.QuoteSymbols).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ticker>().HasOne(t => t.Parent).WithMany(t => t.Children).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Account>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Market>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Member>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Match>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<PreOrder>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Ticker>().HasQueryFilter(x => !x.IsDeleted);
    }
    #endregion
}