using System.ComponentModel.DataAnnotations;

namespace MijnGameKast.API.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}