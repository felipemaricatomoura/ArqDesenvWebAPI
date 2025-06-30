using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class AppController : ControllerBase
{



    [HttpGet]
    public IActionResult GetBooks()
    {

        return Ok();
    }
}
