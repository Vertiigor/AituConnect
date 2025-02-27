using AituConnectAPI.Models;
using AituConnectAPI.Models.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AituConnectAPI.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PipelineContext> Pipelines { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(v => v.ToString(), v => (Roles)Enum.Parse(typeof(Roles), v));
        }
    }
}
