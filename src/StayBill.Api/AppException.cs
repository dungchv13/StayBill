namespace StayBill.Api;

public sealed class AppException : Exception
{
    public int StatusCode { get; }
    public string Error { get; }

    public AppException(int statusCode, string error, string message) : base(message)
    {
        StatusCode = statusCode;
        Error = error;
    }

    public static AppException NotFound(string error, string message) =>
        new(StatusCodes.Status404NotFound, error, message);

    public static AppException Conflict(string error, string message) =>
        new(StatusCodes.Status409Conflict, error, message);

    public static AppException BadRequest(string error, string message) =>
        new(StatusCodes.Status400BadRequest, error, message);

    public static AppException Unauthorized(string error, string message) =>
        new(StatusCodes.Status401Unauthorized, error, message);
}
