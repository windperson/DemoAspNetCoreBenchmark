using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;

namespace WebApiBenchmarks;

[MemoryDiagnoser(displayGenColumns: true)]
public class MvcVsMinimalApiBenchmarks
{
    private const string MvcWebApiUrl = "http://localhost:5001";
    private WebApplication _sutMvcWebApi = null!;
    private HttpClient _mvcWebApiClient = null!;

    private const string MinimalApiUrl = "http://localhost:5002";
    private WebApplication _sutMinimalApi = null!;
    private HttpClient _minimalApiClient = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        _sutMvcWebApi = WebApplicationHelper.CreateSutMvcWebApi(["--urls", MvcWebApiUrl]);
        // don't use RunAsync() because it will block the thread
        await _sutMvcWebApi.StartAsync();
        _mvcWebApiClient = new HttpClient()
        {
            BaseAddress = new Uri(MvcWebApiUrl)
        };

        _sutMinimalApi = WebApplicationHelper.CreateSutMinimalApi(["--urls", MinimalApiUrl]);
        // don't use RunAsync() because it will block the thread
        await _sutMinimalApi.StartAsync();
        _minimalApiClient = new HttpClient()
        {
            BaseAddress = new Uri(MinimalApiUrl)
        };
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _mvcWebApiClient.Dispose();
        await _sutMvcWebApi.StopAsync();

        _minimalApiClient.Dispose();
        await _sutMinimalApi.StopAsync();
    }

    [Benchmark]
    public async Task<string> MvcWebApi()
    {
        var response = await _mvcWebApiClient.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    [Benchmark]
    public async Task<string> MinimalApi()
    {
        var response = await _minimalApiClient.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}