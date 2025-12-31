namespace olieblind.lib.Models;

public class ErrorModel
{
    public string Message { get; set; } = string.Empty;

    public static ErrorModel From(Exception ex)
    {
        return new ErrorModel() { Message = ex.Message };
    }
}