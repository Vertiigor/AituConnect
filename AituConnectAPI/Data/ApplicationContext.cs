using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AituConnectAPI.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PipelineContext> Pipelines { get; set; }

        public DbSet<Message> Messages { get; set; }

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

            builder.Entity<Post>()
                .Property(p => p.Status)
                .HasConversion(v => v.ToString(), v => (PostStatus)Enum.Parse(typeof(PostStatus), v));

            builder.Entity<PipelineContext>()
                .Property(p => p.Type)
                .HasConversion(v => v.ToString(), v => (PipelineType)Enum.Parse(typeof(PipelineType), v));

            builder.Entity<PipelineContext>()
                .Property(p => p.CurrentStep)
                .HasConversion(v => v.ToString(), v => (PipelineStepType)Enum.Parse(typeof(PipelineStepType), v));
        }
    }
}
