// Common/ApiResponse.cs
public class ApiResponse<T>
{
    public bool IsSuccess { get; protected set; }
    public T? Data { get; protected set; }
    public ApiError? Error { get; protected set; }
    public int StatusCode { get; protected set; }

    protected ApiResponse() { }

    // Success responses
    public static ApiResponse<T> Success(T data, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Created(T data)
    {
        return Success(data, 201);
    }

    // Failure responses
    public static ApiResponse<T> Failure(string message, int statusCode = 400, List<string>? validationErrors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Error = new ApiError
            {
                Message = message,
                ValidationErrors = validationErrors ?? new List<string>()
            },
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return Failure(message, 404);
    }

    public static ApiResponse<T> BadRequest(string message, List<string>? validationErrors = null)
    {
        return Failure(message, 400, validationErrors);
    }

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
    {
        return Failure(message, 401);
    }

    public static ApiResponse<T> Forbidden(string message = "Forbidden")
    {
        return Failure(message, 403);
    }

    public static ApiResponse<T> InternalError(string message = "An internal error occurred")
    {
        return Failure(message, 500);
    }
}

// For operations with no return data
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(int statusCode = 200)
    {
        return new ApiResponse
        {
            IsSuccess = true,
            StatusCode = statusCode
        };
    }

    public static ApiResponse NoContent()
    {
        return Success(204);
    }

    public new static ApiResponse Failure(string message, int statusCode = 400, List<string>? validationErrors = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Error = new ApiError
            {
                Message = message,
                ValidationErrors = validationErrors ?? new List<string>()
            },
            StatusCode = statusCode
        };
    }

    public new static ApiResponse NotFound(string message = "Resource not found")
    {
        return Failure(message, 404);
    }

    public new static ApiResponse BadRequest(string message, List<string>? validationErrors = null)
    {
        return Failure(message, 400, validationErrors);
    }

    public new static ApiResponse Unauthorized(string message = "Unauthorized access")
    {
        return Failure(message, 401);
    }

    public new static ApiResponse Forbidden(string message = "Forbidden")
    {
        return Failure(message, 403);
    }

    public new static ApiResponse InternalError(string message = "An internal error occurred")
    {
        return Failure(message, 500);
    }
}

// Common/ApiError.cs
public class ApiError
{
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public static class ErrorCodes
{
    // User errors
    public const string DocumentNotFound = "DOCUMENT_NOT_FOUND";
    public const string DocumentAlreadyExists = "DOCUMENT_ALREADY_EXISTS";
}
