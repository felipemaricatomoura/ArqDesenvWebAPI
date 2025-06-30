using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Models; // Define o namespace dos modelos

public class Book // Modelo Book
{
    public int Id { get; set; } // ID do livro
    public string? Title { get; set; } // Título
    public string? AuthorName { get; set; } // Nome do autor
    public string? AuthorLastName { get; set; } // Sobrenome do autor
    public string? Publisher { get; set; } // Editora
    public int YearPublishedDate { get; set; } // Ano de publicação
    public string? City { get; set; } // Cidade
    public bool EtAlii { get; set; } = false; // Et alii (outros autores)
}

public class BookConfiguration : IEntityTypeConfiguration<Book> // Configuração da entidade Book
{
    public void Configure(EntityTypeBuilder<Book> builder) // Método de configuração
    {
        builder.HasKey(b => b.Id); // Define a chave primária
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200); // Título obrigatório com no máx. 200
        builder.Property(b => b.AuthorName).IsRequired().HasMaxLength(100); // Nome obrigatório com no máx. 100
        builder.Property(b => b.AuthorLastName).HasMaxLength(100); // Sobrenome com no máx. 100
        builder.Property(b => b.Publisher).IsRequired().HasMaxLength(150); // Editora obrigatória com máx. 150
        builder.Property(b => b.YearPublishedDate).IsRequired(); // Ano de publicação obrigatório
        builder.Property(b => b.City).HasMaxLength(100); // Cidade com no máx. 100

        builder.HasData( // Dados iniciais (seed)
            new Book
            {
                Id = 1, // ID do livro
                Title = "Programação Back End II", // Título
                AuthorName = "Cleverson Lopes", // Nome do autor
                AuthorLastName = "Ledur", // Sobrenome do autor
                Publisher = "Sagah", // Editora
                YearPublishedDate = 2019, // Ano
                City = "Porto Alegre", // Cidade
                EtAlii = false // Et alii
            },
            new Book
            {
                Id = 2,
                Title = "Programação Back End III",
                AuthorName = "Predro H. Chagas",
                AuthorLastName = "Freitas",
                Publisher = "Sagah",
                YearPublishedDate = 2021,
                City = "Porto Alegre",
                EtAlii = true
            },
            new Book
            {
                Id = 3,
                Title = "RICH Internet Application e desenvolvimento Web para programadores.",
                AuthorName = "Paul J. Ajax",
                AuthorLastName = "Deitel",
                Publisher = "Sagah",
                YearPublishedDate = 2008,
                City = "São Paulo",
                EtAlii = false
            }
        );
    }
}
