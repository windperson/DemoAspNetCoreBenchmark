using DemoRandomStreamApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ServerAsyncStreamBenchmark;

public static class WebApplicationHelper
{
    public static WebApplication CreateSutMinimalApi(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddAuthorization();


        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.SetupRandomNumberApi();
        app.SetupAsyncRandomNumberApi();

        return app;
    }
}