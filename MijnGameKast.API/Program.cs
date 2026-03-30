namespace MijnGameKast.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}


// // UserController userController = new UserController();
//
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
//
// app.UseHttpsRedirection();
//
// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };
//
// var users = new[]
// {
//     new { Id = 0, Name = "admin" },
//     new { Id = 1, Name = "Jeroen" },
//     new { Id = 2, Name = "Justen" }, 
//     new { Id = 3, Name = "Jonas" },
// };
//
// var userEndPoint = app.MapGroup("/user");
//
// app.MapGet("/weatherforecast", () =>
//     {
//         var forecast = Enumerable.Range(1, 5).Select(index =>
//                 new WeatherForecast
//                 (
//                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                     Random.Shared.Next(-20, 55),
//                     summaries[Random.Shared.Next(summaries.Length)]
//                 ))
//             .ToArray();
//         return forecast;
//     })
//     .WithName("GetWeatherForecast");
//
// // app.MapGet("/test/{id}/{test}", (int id, string test) => userController.MyMethod(id, test));
// // userEndPoint.MapGet("/{id}", (int id) => userController.MyMethod(id, "test"));
//
// app.MapGet("/users/{id}", (int id) =>
// {
//     // Get user by id
//     var user = users.FirstOrDefault(u => u.Id == id);
//     
//     // Check if user exists and if not return error message
//     if (user == null)
//     {
//         return "No user found!";
//     }
//     
//     // Return the name of the user
//     return user?.Name;
// });
//
// app.MapGet("/test", () => "get");
// app.MapPost("/test", () => "post");
// app.MapPut("/test", () => "put");
// app.MapPatch("/test", () => "patch");
// app.MapDelete("/test", () => "delete");
//
//
// app.Run();
//
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }