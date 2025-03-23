namespace PatsyLibrary.Services;

public class LibraryHelper
{
    public static class Result
    {
        public static (bool IsSuccess, string Message) Success(bool isSuccess, string msg)
        {
            return (isSuccess, msg);
        }

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
}