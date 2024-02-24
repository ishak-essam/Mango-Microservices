using Mango.Service.CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext ( DbContextOptions<AppDbContext> options ) : base (options)
        {
        }
        public DbSet<CartDetails> CartDetail { get; set; }
        public DbSet<CartHeader> CartHeader { get; set; }
    }
    
}
