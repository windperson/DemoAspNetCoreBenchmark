using System.Runtime.CompilerServices;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;

namespace ServerAsyncStreamBenchmark;

[MemoryDiagnoser(displayGenColumns: true)]
public class ServerAsyncStreamBenchmarks
{
    private const string MinimalApiUrl = "http://localhost:5002";
    private WebApplication _sutMinimalApi = null!;
    private HttpClient _minimalApiClient = null!;

    [Params(5, 10, 100, 500, 1_000, 5_000, 10_000, 30_000)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Count { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
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
        _minimalApiClient.Dispose();
        await _sutMinimalApi.StopAsync();
    }

    [Benchmark(Baseline = true, Description = "Normal List")]
    public async Task MinimalApiRandomInt()
    {
        var response = await _minimalApiClient.GetAsync($"/random?count={Count}");
        response.EnsureSuccessStatusCode();
        var responseRawString = await response.Content.ReadAsStringAsync();
        var responseList = JsonSerializer.Deserialize<List<int>>(responseRawString);
        CheckResult(responseList);
    }


    [Benchmark(Description = "Async Stream")]
    public async Task MinimalApiAsyncRandomInt()
    {
        var response = await _minimalApiClient.GetAsync($"/async_random?count={Count}");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var responseList = await JsonSerializer.DeserializeAsync<List<int>>(stream);
        CheckResult(responseList);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void CheckResult(List<int>? responseList)
    {
        if (responseList == null)
        {
            const string message = "Response list is null";
            Console.WriteLine(message);
            throw new Exception(message);
        }

        if (responseList.Count != Count)
        {
            const string message = "Response list count is not equal to the expected count";
            Console.WriteLine(message);
            throw new Exception(message);
        }
    }
}