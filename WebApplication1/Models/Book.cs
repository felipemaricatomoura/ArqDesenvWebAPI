using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApplication1.Models;

public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorLastName { get; set; }
    public string? Publisher { get; set; }
    public int YearPublishedDate { get; set; }
    public string? City { get; set; }
    public bool EtAlii { get; set; } = false;
}

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.AuthorName).IsRequired().HasMaxLength(100);
        builder.Property(b => b.AuthorLastName).HasMaxLength(100);
        builder.Property(b => b.Publisher).IsRequired().HasMaxLength(150);
        builder.Property(b => b.YearPublishedDate).IsRequired();
        builder.Property(b => b.City).HasMaxLength(100);                

        builder.HasData(
            new Book
            {
                Id = 1,
                Title = "Programação Back End II",
                AuthorName = "Cleverson Lopes",
                AuthorLastName = "Ledur",
                Publisher = "Sagah",
                YearPublishedDate = 2019,
                City = "Porto Alegre"
            },
            new Book
            {
                Id = 2,
                Title = "Programação Back End III",
                AuthorName = "Predro H. Chgas",
                AuthorLastName = "Ledur",
                Publisher = "Sagah",
                YearPublishedDate = 2020,
                City = "Porto Alegre"
            }
            );
    }
}