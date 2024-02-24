using Mango.Service.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext ( DbContextOptions<AppDbContext> options ) : base (options)
        {
        }
        public DbSet<OrderDetails> orderDetails { get; set; }
        public DbSet<OrderHeader> orderHeaders { get; set; }
    }
    
}
