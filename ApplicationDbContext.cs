using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
    }
}