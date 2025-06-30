using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Context; // Define o namespace do contexto

public class AppDbContext : DbContext // Classe do contexto do EF Core
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // Construtor com opções
    {
    }

    public DbSet<Book> Books { get; set; } // Tabela de livros
    public DbSet<Student> Students { get; set; } // Tabela de estudantes

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Método para configurar o modelo
    {
        base.OnModelCreating(modelBuilder); // Chama configuração base
        modelBuilder.ApplyConfiguration(new BookConfiguration()); // Aplica config do Book
        modelBuilder.ApplyConfiguration(new StudentConfiguration()); // Aplica config do Student
    }
}
