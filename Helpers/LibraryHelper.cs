namespace PatsyLibrary.Helpers;

public class LibraryHelper
{
    public static class Result
    {
        public static (bool IsSuccess, string Message) Success(bool isSuccess, string msg)
        {
            return (isSuccess, msg);
        }
    }
}