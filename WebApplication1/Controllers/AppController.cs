using Microsoft.AspNetCore.Mvc;
using WebApplication1.Context;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class AppController : ControllerBase
{
    private readonly AppDbContext _context;
    public AppController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public IActionResult GetBooks()
    {
        var books = _context.Books.ToList();
        return Ok(books);
    }
}
