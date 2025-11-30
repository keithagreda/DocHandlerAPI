
namespace DocumentHandlerAPI.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToHttpResult<T>(this ApiResponse<T> response)
        {
            return Results.Json(response, statusCode: response.StatusCode);
        }

        public static IResult ToHttpResult(this ApiResponse response)
        {
            return Results.Json(response, statusCode: response.StatusCode);
        }
    }
}
