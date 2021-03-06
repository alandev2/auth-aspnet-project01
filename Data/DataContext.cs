using Microsoft.EntityFrameworkCore;
using backend_aspnet_crud.Entities;

namespace backend_aspnet_crud.Data
{
    public class DataContext: DbContext {
        public DataContext(DbContextOptions<DataContext> options): base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<FileM> Files { get; set; }
        public DbSet<Post> Posts { get; set; }
    } 
}