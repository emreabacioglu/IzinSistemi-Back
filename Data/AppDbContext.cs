using Microsoft.EntityFrameworkCore;
using IzinSistemi_Back.Models;

namespace IzinSistemi_Back.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Personel> Personels { get; set; }
        public DbSet<Leave> Leaves
        {
            get; set;
        }
    }
}