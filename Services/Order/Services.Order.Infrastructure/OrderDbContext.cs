using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Order.Infrastructure;

public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
        optionsBuilder.UseSqlServer("Data Source=localhost,1444;Database=OrderDb;User=sa;Password=Password12*");

        return new OrderDbContext(optionsBuilder.Options);
    }
}

public class OrderDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "ordering";

 
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
	{

	}

	public DbSet<Domain.OrderAggregate.Order> Orders { get; set; }
	public DbSet<Domain.OrderAggregate.OrderItem> OrderItemss { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SCHEMA);
        modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().ToTable("OrderItems", DEFAULT_SCHEMA);
        modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().Property(p=>p.Price).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(p=>p.Address).WithOwner();


        base.OnModelCreating(modelBuilder);

    }
}
