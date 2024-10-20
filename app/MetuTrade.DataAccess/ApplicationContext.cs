namespace MetuTrade.DataAccess;

using MetuTrade.Core.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<Bar> Bars { get; set; }
    public DbSet<Bot> Bots { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Bar>().HasKey(e => new { e.Symbol, e.Interval, e.OpenTime });
        modelBuilder.Entity<Bot>().HasKey(e => e.Id);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Server=LAPTOP-1U2CSG0R\\SQLEXPRESS; Database=MetuTradeDb; Trusted_Connection=True; TrustServerCertificate=True;");
    }
}
