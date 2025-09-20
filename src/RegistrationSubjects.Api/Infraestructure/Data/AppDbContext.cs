using Microsoft.EntityFrameworkCore;
using RegistrationSubjects.Core.Entities;

namespace RegistrationSubjects.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");
                entity.Property(s => s.First_Name).HasColumnName("first_name");
                entity.Property(s => s.Last_Name).HasColumnName("last_name");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.ToTable("professor");
                entity.Property(p => p.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject");
                entity.Property(s => s.Title).HasColumnName("title");
                entity.Property(s => s.Credits).HasColumnName("credits");

                entity.HasOne(s => s.Professor)
                      .WithMany(p => p.Subjects)
                      .HasForeignKey(s => s.Professor_Id);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("enrollment");
                entity.Property(e => e.CreatedAt).HasColumnName("createdat");

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Subject)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.SubjectId);

                entity.HasIndex(e => new { e.StudentId, e.SubjectId }).IsUnique();
            });
        }
    }
}
