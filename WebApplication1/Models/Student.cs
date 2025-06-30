using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Models; // Define o namespace dos modelos

public class Student // Modelo Student
{
    public int Id { get; set; } // ID do estudante
    public string? Name { get; set; } // Nome
    public string? LastName { get; set; } // Sobrenome
    public int RU { get; set; } // Registro único (RU)
    public string? Course { get; set; } // Curso
}

public class StudentConfiguration : IEntityTypeConfiguration<Student> // Configuração da entidade Student
{
    public void Configure(EntityTypeBuilder<Student> builder) // Método de configuração
    {
        builder.HasKey(s => s.Id); // Define chave primária
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100); // Nome obrigatório, máx. 100
        builder.Property(s => s.LastName).IsRequired().HasMaxLength(100); // Sobrenome obrigatório, máx. 100
        builder.Property(s => s.RU).IsRequired(); // RU obrigatório
        builder.Property(s => s.Course).HasMaxLength(150); // Curso com no máx. 150

        builder.HasData( // Dados iniciais (seed)
            new Student
            {
                Id = 1, // ID do estudante
                Name = "Felipe Maricato", // Nome
                LastName = "Moura", // Sobrenome
                RU = 2734850, // Registro único
                Course = "GRAD - TECNOLOGIA EM DESENVOLVIMENTO MOBILE - DISTÂNCIA" // Curso
            }
        );
    }
}
