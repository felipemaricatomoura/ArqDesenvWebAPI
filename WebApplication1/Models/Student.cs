using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Models;

public class Student
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int RU { get; set; }
    public string? Course { get; set; }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.LastName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.RU).IsRequired();
        builder.Property(s => s.Course).HasMaxLength(150);

        // Seed data
        builder.HasData(
            new Student { 
                Id = 1, 
                Name = "Felipe Maricato", 
                LastName = "Moura", 
                RU = 2734850, 
                Course = "GRAD - TECNOLOGIA EM DESENVOLVIMENTO MOBILE - DISTÂNCIA"
            }
        );
    }
}
