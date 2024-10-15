namespace MetuTrade.DataAccess;

using MetuTrade.Core;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public List<Bar> Bars { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Bar>().HasKey(e => new { e.Symbol, e.Interval, e.OpenTime });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Server=LAPTOP-1U2CSG0R\\SQLEXPRESS; Database=MetuTradeDb; Trusted_Connection=True; TrustServerCertificate=True;");
    }
}
