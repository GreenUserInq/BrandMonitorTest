using BrandMonitorTest.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandMonitorTest.Data
{
    public class TaskDbContext : DbContext
    {
        public DbSet<EmptyTask> Tasks { get; set; }

        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmptyTask>()
                .Property(t => t.State)
                .HasConversion<string>();
        }
    }
}
