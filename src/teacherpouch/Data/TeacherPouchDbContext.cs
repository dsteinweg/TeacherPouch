using Microsoft.EntityFrameworkCore;
using TeacherPouch.Models;

namespace TeacherPouch.Data
{
    public class TeacherPouchDbContext : DbContext
    {
        public TeacherPouchDbContext(DbContextOptions<TeacherPouchDbContext> options)
            : base(options)
        {

        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PhotoTag> PhotoTags { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PhotoTag>()
                .HasKey("PhotoId", "TagId");

            builder.Entity<PhotoTag>()
                .HasOne(pt => pt.Photo)
                .WithMany(p => p.PhotoTags)
                .HasForeignKey(pt => pt.PhotoId);

            builder.Entity<PhotoTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PhotoTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}
