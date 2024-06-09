using Microsoft.AspNetCore.Identity;

namespace AppIdentity.Resources;

public class AppResult<T> where T : class
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}