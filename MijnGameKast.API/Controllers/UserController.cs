using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using MijnGameKast.API.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : CustomBaseController
{
    // private static List<User> _users = new List<User>()
    // {
    //     new User() { Id = 1, Name = "Jeroen" },
    //     new User() { Id = 2, Name = "John" },
    //     new User() { Id = 3, Name = "Maarten" }
    // };
    //
    // [HttpGet]
    // public IActionResult Get()
    // {
    //     return Ok(_users);
    // }
    //
    // [HttpGet("{id}")]
    // public IActionResult GetById(int id)
    // {
    //     User? user = _users.FirstOrDefault(u => u.Id == id);
    //     if (user == null)
    //     {
    //         return NoContent();
    //     }
    //     
    //     return Ok(user);
    // }
    //
    // [HttpPost]
    // public IActionResult Post([FromBody] User user)
    // {
    //     Console.WriteLine("Console test");
    //     Console.WriteLine(user);
    //     User newUser = new User() { Id = user.Id, Name = user.Name };
    //     _users.Add(newUser);
    //     return CreatedAtAction(
    //         nameof(GetById),
    //         new { id = newUser.Id },
    //         newUser
    //     );
    // }
    //
    // [HttpDelete]
    // public IActionResult Delete(int? id)
    // {
    //     User? user = _users.FirstOrDefault(u => u.Id == id);
    //
    //     if (user == null)
    //     {
    //         return NoContent();
    //     }
    //
    //     _users.Remove(user);
    //     
    //     return Ok($"User with id {id} successfully deleted");
    // }
    //
    // [HttpPut]
    // public IActionResult Update(int? id)
    // {
    //     Console.WriteLine("Er is een user geupdate");
    //     return Ok("User successfully updated");
    // }
}