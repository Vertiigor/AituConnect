using MessageProducerService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessageProducerService.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(v => v.ToString(), v => (Roles)Enum.Parse(typeof(Roles), v));

            builder.Entity<Post>()
               .HasMany(p => p.Subjects)
               .WithMany()
               .UsingEntity(j => j.ToTable("PostSubjects"));

            builder.Entity<Post>()
                .Property(p => p.Status)
                .HasConversion(v => v.ToString(), v => (Status)Enum.Parse(typeof(Status), v));
        }
    }
}
