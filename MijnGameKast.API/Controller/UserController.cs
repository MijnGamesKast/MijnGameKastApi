using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using MijnGameKast.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace MijnGameKast.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private static List<User> _users = new List<User>()
    {
        new User() { Id = 1, Name = "Jeroen" },
        new User() { Id = 2, Name = "John" },
        new User() { Id = 3, Name = "Maarten" }
    };

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        User? user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Shit is niet gevonden");
        }
        
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Post([FromBody] User user)
    {
        Console.WriteLine("Console test");
        Console.WriteLine(user);
        User newUser = new User() { Id = user.Id, Name = user.Name };
        _users.Add(newUser);
        return CreatedAtAction(
            nameof(GetById),
            new { id = newUser.Id },
            newUser
        );
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        // Console.WriteLine("Er is een item verwijderd");
        // Remove user from the list with id number 3 for example
        
        return Ok("User successfully deleted");
    }

    [HttpPut]
    public IActionResult Update(int? id)
    {
        Console.WriteLine("Er is een user geupdate");
        return Ok("User successfully updated");
    }
}