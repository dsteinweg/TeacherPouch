using Microsoft.EntityFrameworkCore;
using TeacherPouch.Models;

namespace TeacherPouch.Data;

public class TeacherPouchDbContext(DbContextOptions<TeacherPouchDbContext> options) : DbContext(options)
{
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PhotoTag> PhotoTags => Set<PhotoTag>();
    public DbSet<Question> Questions => Set<Question>();

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
