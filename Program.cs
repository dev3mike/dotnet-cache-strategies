using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Memory cache
builder.Services.AddMemoryCache();

// Hybrid cache
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

// Fusion cache
builder.Services.AddFusionCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


// Summary of performance test results for Memory Cache:
// - 1 scenario with a maximum of 1000 virtual users (VUs) over 6 seconds.
// - All requests returned a status of 200 (success).
// - 100.00% of checks passed (1272 out of 1272).
// - Data received: 233 kB at an average rate of 33 kB/s.
// - Data sent: 117 kB at an average rate of 16 kB/s.
// - Average request duration: 1.45s, with a maximum of 3.16s.
// - No failed requests (0.00% failure rate).
// - Average iteration duration: 2.46s, with a maximum of 4.17s.
// - 1272 iterations completed with a maximum of 1000 VUs.
// - Warning: thresholds for 'http_req_duration' were exceeded. (p(95)<2000)
app.MapGet("/memory-cache", async () =>
    {
        var memoryCache = app.Services.GetService<IMemoryCache>();
        
        if(memoryCache == null) return Results.NotFound();
        
        var cachedValue = await memoryCache.GetOrCreateAsync("memory-cache", async (entry) =>
        {
            await Task.Delay(3000);
            Console.WriteLine("Getting data from the database");
            return "Data from the database";
        }, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromSeconds(1) });
        
        return Results.Ok(cachedValue);
    })
    .WithName("MemoryCache");


// Summary of performance test results for Hybrid Cache:
// - 1 scenario with a maximum of 1000 virtual users (VUs) over 6 seconds.
// - All requests returned a status of 200 (success).
// - 100.00% of checks passed (2125 out of 2125).
// - Data received: 389 kB at an average rate of 62 kB/s.
// - Data sent: 196 kB at an average rate of 31 kB/s.
// - Average request duration: 279.51ms, with a maximum of 3.25s.
// - No failed requests (0.00% failure rate).
// - Average iteration duration: 1.28s, with a maximum of 4.26s.
// - 2125 iterations completed with a maximum of 1000 VUs.
// - Note: Performance is better than memory cache, with lower average request duration.
app.MapGet("/hybrid-cache", async () =>
    {
        var hybridCache = app.Services.GetService<HybridCache>();
        
        if(hybridCache == null) return Results.NotFound();

        var cachedValue = await hybridCache.GetOrCreateAsync("hybrid-cache", async (entry) =>
        {
            await Task.Delay(3000);
            Console.WriteLine("Getting data from the database");
            return "Data from the database";
        }, new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromSeconds(1),
        });
        
        return Results.Ok(cachedValue);
    })
    .WithName("HybridCache");


// Summary of performance test results for Fusion Cache:
// - 1 scenario with a maximum of 1000 virtual users (VUs) over 6 seconds.
// - All requests returned a status of 200 (success).
// - 100.00% of checks passed (1760 out of 1760).
// - Data received: 322 kB at an average rate of 39 kB/s.
// - Data sent: 162 kB at an average rate of 20 kB/s.
// - Average request duration: 1.59s, with a maximum of 3.14s.
// - No failed requests (0.00% failure rate).
// - Average iteration duration: 2.59s, with a maximum of 4.14s.
// - 1760 iterations completed with a maximum of 1000 VUs.
// - Warning: thresholds for 'http_req_duration' were exceeded. (p(95)<2000)
// - Note: Cache-stampede protection should help with concurrent requests.
app.MapGet("/fusion-cache", async () =>
    {
        var fusionCache = app.Services.GetService<IFusionCache>();
        
        if(fusionCache == null) return Results.NotFound();

        var cachedValue = await fusionCache.GetOrSetAsync("fusion-cache", async (entry) =>
        {
            await Task.Delay(3000);
            Console.WriteLine("Getting data from the database");
            return "Data from the database";
        }, TimeSpan.FromSeconds(1));
        
        return Results.Ok(cachedValue);
    })
    .WithName("FusionCache");

// Conclusion: Among the three caching strategies, the Hybrid Cache shows the best performance with the lowest average request duration and no threshold warnings. The Fusion Cache and Memory Cache both exceeded the request duration threshold, indicating potential performance issues under high load.

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}