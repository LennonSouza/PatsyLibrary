namespace PatsyLibrary.Helpers;

public class AuthorizeHelper
{
    public static Task<bool> AuthorizeSession(HttpContext httpContext)
    {
        if (!httpContext.Session.Keys.Any())
        {
            httpContext.Session.Clear();
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }
}