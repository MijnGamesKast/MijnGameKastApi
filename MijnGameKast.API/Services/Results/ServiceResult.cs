namespace MijnGameKast.API.Services.Results;

public class ServiceResult
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public ServiceResultType Type { get; set; }
}