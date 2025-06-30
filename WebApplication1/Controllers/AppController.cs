using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
    private readonly AppDbContext _context;
    public AppController(AppDbContext context) => _context = context;

    #region LOGIN METHODS

    [HttpPost("login")]
    public IActionResult Login([FromBody] Login login)
    {
        var student = _context.Students.FirstOrDefault(s =>
            s.LastName.ToLower().Trim() == login.LastName.ToLower().Trim()
            && s.RU == login.RU);

        if (student == null)
            return Unauthorized();

        var claims = new[]
        {
        new Claim(ClaimTypes.Name, student.Name),
        new Claim("StudentId", student.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A320EFBA-E00C-4344-BCDB-A54B93D571A5"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    #endregion

    #region BOOKS METHODS

    [Authorize]
    [HttpGet("books")]
    public IActionResult GetBooks()
    {
        var books = _context.Books.ToList();
        return Ok(books);
    }

    [HttpPost("books")]
    public IActionResult CreateBook([FromBody] Book book)
    {
        if (book == null)
            return BadRequest();

        _context.Books.Add(book);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [HttpGet("books/{id}")]
    public IActionResult GetBookById(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPut("books/{id}")]
    public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
    {
        if (id != updatedBook.Id)
            return BadRequest("ID do livro não confere");

        var book = _context.Books.Find(id);
        if (book == null)
            return NotFound();

        book.Title = updatedBook.Title;
        book.AuthorName = updatedBook.AuthorName;
        book.AuthorLastName = updatedBook.AuthorLastName;
        book.Publisher = updatedBook.Publisher;
        book.YearPublishedDate = updatedBook.YearPublishedDate;
        book.City = updatedBook.City;
        book.EtAlii = updatedBook.EtAlii;

        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("books/{id}")]
    public IActionResult DeleteBook(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null)
            return NotFound();

        _context.Books.Remove(book);
        _context.SaveChanges();
        return NoContent();
    }

    #endregion

    #region STUDENTS METHODS

    [HttpGet("students")]
    public IActionResult GetStudents()
    {
        var students = _context.Students.ToList();
        return Ok(students);
    }

    [HttpGet("students/{id}")]
    public IActionResult GetStudentById(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpPost("students")]
    public IActionResult CreateStudent([FromBody] Student student)
    {
        if (student == null)
            return BadRequest();

        _context.Students.Add(student);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
    }

    [HttpPut("students/{id}")]
    public IActionResult UpdateStudent(int id, [FromBody] Student updatedStudent)
    {
        if (id != updatedStudent.Id)
            return BadRequest("ID no corpo da requisição não corresponde ao da URL.");

        var student = _context.Students.Find(id);
        if (student == null)
            return NotFound();

        student.Name = updatedStudent.Name;
        student.LastName = updatedStudent.LastName;
        student.RU = updatedStudent.RU;
        student.Course = updatedStudent.Course;

        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("students/{id}")]
    public IActionResult DeleteStudent(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null)
            return NotFound();

        _context.Students.Remove(student);
        _context.SaveChanges();
        return NoContent();
    }

    #endregion
}