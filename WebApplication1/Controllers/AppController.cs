using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Context;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers; // Define o namespace dos controllers

[ApiController] // Indica que é um controller de API
[Route("api/[controller]")] // Define a rota como /api/App
public class AppController : ControllerBase // Controller base da API
{
    private readonly AppDbContext _context; // Contexto do banco

    public AppController(AppDbContext context) => _context = context; // Injeta o contexto no construtor

    #region LOGIN METHODS

    [HttpPost("login")] // Rota POST /api/app/login
    public IActionResult Login([FromBody] Login login) // Método de login
    {
        var student = _context.Students.FirstOrDefault(s => // Busca aluno por sobrenome e RU
            s.LastName.ToLower().Trim() == login.LastName.ToLower().Trim()
            && s.RU == login.RU);

        if (student == null) // Se não encontrar, retorna 401
            return Unauthorized();

        var claims = new[] // Cria os claims do token
        {
            new Claim(ClaimTypes.Name, student.Name), // Nome
            new Claim("StudentId", student.Id.ToString()) // ID do aluno
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A320EFBA-E00C-4344-BCDB-A54B93D571A5")); // Chave JWT
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Credenciais com HMAC-SHA256

        var token = new JwtSecurityToken( // Gera o token JWT
            claims: claims,
            expires: DateTime.Now.AddHours(1), // Expira em 1 hora
            signingCredentials: creds);

        return Ok(new // Retorna o token no corpo
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    #endregion

    #region BOOKS METHODS

    [Authorize] // Requer autenticação
    [HttpGet("books")] // Rota GET /api/app/books
    public IActionResult GetBooks()
    {
        var books = _context.Books.ToList(); // Busca todos os livros
        return Ok(books); // Retorna 200 com a lista
    }

    [Authorize]
    [HttpPost("books")] // Rota POST /api/app/books
    public IActionResult CreateBook([FromBody] Book book)
    {
        if (book == null) // Valida objeto nulo
            return BadRequest();

        _context.Books.Add(book); // Adiciona o livro
        _context.SaveChanges(); // Salva alterações
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book); // Retorna 201 com localização
    }

    [Authorize]
    [HttpGet("books/{id}")] // Rota GET /api/app/books/{id}
    public IActionResult GetBookById(int id)
    {
        var book = _context.Books.Find(id); // Busca livro por ID
        if (book == null)
            return NotFound(); // Se não achar, 404

        return Ok(book); // Retorna 200 com o livro
    }

    [Authorize]
    [HttpPut("books/{id}")] // Rota PUT /api/app/books/{id}
    public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
    {
        if (id != updatedBook.Id) // Valida ID do corpo com a URL
            return BadRequest("ID do livro não confere");

        var book = _context.Books.Find(id); // Busca livro
        if (book == null)
            return NotFound();

        // Atualiza os campos
        book.Title = updatedBook.Title;
        book.AuthorName = updatedBook.AuthorName;
        book.AuthorLastName = updatedBook.AuthorLastName;
        book.Publisher = updatedBook.Publisher;
        book.YearPublishedDate = updatedBook.YearPublishedDate;
        book.City = updatedBook.City;
        book.EtAlii = updatedBook.EtAlii;

        _context.SaveChanges(); // Salva no banco
        return NoContent(); // Retorna 204
    }

    [Authorize]
    [HttpDelete("books/{id}")] // Rota DELETE /api/app/books/{id}
    public IActionResult DeleteBook(int id)
    {
        var book = _context.Books.Find(id); // Busca livro
        if (book == null)
            return NotFound();

        _context.Books.Remove(book); // Remove do banco
        _context.SaveChanges(); // Salva alteração
        return NoContent(); // Retorna 204
    }

    #endregion

    #region STUDENTS METHODS

    [Authorize]
    [HttpGet("students")] // Rota GET /api/app/students
    public IActionResult GetStudents()
    {
        var students = _context.Students.ToList(); // Busca todos os alunos
        return Ok(students); // Retorna lista
    }

    [Authorize]
    [HttpGet("students/{id}")] // Rota GET /api/app/students/{id}
    public IActionResult GetStudentById(int id)
    {
        var student = _context.Students.Find(id); // Busca aluno
        if (student == null)
            return NotFound();

        return Ok(student); // Retorna 200 com aluno
    }

    [Authorize]
    [HttpPost("students")] // Rota POST /api/app/students
    public IActionResult CreateStudent([FromBody] Student student)
    {
        if (student == null)
            return BadRequest();

        _context.Students.Add(student); // Adiciona aluno
        _context.SaveChanges(); // Salva
        return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student); // Retorna 201
    }

    [Authorize]
    [HttpPut("students/{id}")] // Rota PUT /api/app/students/{id}
    public IActionResult UpdateStudent(int id, [FromBody] Student updatedStudent)
    {
        if (id != updatedStudent.Id) // Valida IDs
            return BadRequest("ID no corpo da requisição não corresponde ao da URL.");

        var student = _context.Students.Find(id); // Busca aluno
        if (student == null)
            return NotFound();

        // Atualiza dados
        student.Name = updatedStudent.Name;
        student.LastName = updatedStudent.LastName;
        student.RU = updatedStudent.RU;
        student.Course = updatedStudent.Course;

        _context.SaveChanges(); // Salva no banco
        return NoContent(); // Retorna 204
    }

    [Authorize]
    [HttpDelete("students/{id}")] // Rota DELETE /api/app/students/{id}
    public IActionResult DeleteStudent(int id)
    {
        var student = _context.Students.Find(id); // Busca aluno
        if (student == null)
            return NotFound();

        _context.Students.Remove(student); // Remove do banco
        _context.SaveChanges(); // Salva
        return NoContent(); // Retorna 204
    }

    #endregion
}
