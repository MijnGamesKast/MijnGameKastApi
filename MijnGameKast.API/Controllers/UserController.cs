namespace MijnGameKast.API.Controllers;

public class UserController
{
    public string MyMethod(int id, string test)
    {
        return $"id: {id} test: {test}";
    }
}