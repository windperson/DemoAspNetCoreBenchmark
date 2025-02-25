using Microsoft.AspNetCore.Mvc;

namespace DemoRandomStreamApi;

public static class MinimalApiApplicationHelper
{
    public static void SetupRandomNumberApi(this WebApplication app)
    {
        // Create a stream of random numbers via a GET request /random?count={total}
        app.MapGet("/random", async ([FromQuery(Name = "count")] int count, HttpContext httpContext) =>
        {
            var random = new Random();

            await httpContext.Response.WriteAsJsonAsync(Enumerable.Range(0, count).Select(_ => random.Next()).ToList());
        });
    }

    public static void SetupAsyncRandomNumberApi(this WebApplication app)
    {
        // Create async stream of random numbers via a GET request /async_random?count={total} 
        app.MapGet("/async_random",
            ([FromQuery(Name = "count")] int count) => RandomIntRangeAsync(count));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async IAsyncEnumerable<int> RandomIntRangeAsync(int count)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var random = new Random();
        for (var i = 0; i < count; i++)
        {
            yield return random.Next();
        }
    }
}