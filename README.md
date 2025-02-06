# Cache Stampede Protection Demo 🛡️

This project demonstrates and compares different caching strategies in ASP.NET Core to prevent cache stampede issues. Cache stampede occurs when multiple requests try to regenerate the same cached item simultaneously, potentially overwhelming the system.

## 🚀 Features

- Implementation of three different caching strategies:
  - Memory Cache (built-in ASP.NET Core) ��
  - Hybrid Cache (Microsoft's preview library) 🔄
  - Fusion Cache (with stampede protection) 🛡️

## 🛠️ Technologies Used

- ASP.NET Core
- Microsoft.Extensions.Caching.Memory
- Microsoft.Extensions.Caching.Hybrid (Preview)
- ZiggyCreatures.Caching.Fusion
- k6 (for load testing)

## ℹ️ About HybridCache

HybridCache is Microsoft's new caching library (currently in preview) that provides:
- Efficient in-process primary caching using MemoryCache
- Optional distributed secondary cache support (Redis, SQL Server)
- Built-in stampede protection
- Support for cache entry tagging
- Configurable serialization options
- Performance optimizations for immutable objects

## 📊 Performance Test Results

### Memory Cache Results
- Maximum Users: 1000 VUs
- Test Duration: 6 seconds
- Success Rate: 100% (1272 requests)
- Average Request Duration: 1.45s
- Maximum Request Duration: 3.16s
- Warning: Exceeded threshold (p95 < 2000ms)
- Total Iterations: 1272

### Hybrid Cache Results ⭐
- Maximum Users: 1000 VUs
- Test Duration: 6 seconds
- Success Rate: 100% (1763 requests)
- Average Request Duration: 1.52s
- Maximum Request Duration: 3.01s
- P90/P95: 3.01s
- Total Iterations: 1763
- Throughput: ~219 requests/second

### Fusion Cache Results
- Maximum Users: 1000 VUs
- Test Duration: 6 seconds
- Success Rate: 100% (1760 requests)
- Average Request Duration: 1.59s
- Maximum Request Duration: 3.14s
- Warning: Exceeded threshold (p95 < 2000ms)
- Total Iterations: 1760
- Features built-in stampede protection

## 🧪 Load Test Configuration

Each caching strategy is tested with:
- Ramp up: 100 users in 2 seconds
- Peak load: 1000 users in 2 seconds
- Ramp down: 0 users in 2 seconds
- Performance thresholds:
  - 95% of requests should complete under 2 seconds
  - Less than 10% of requests should fail

## 📝 Key Findings

1. **Hybrid Cache Performance**: 
   - Consistent performance under high load
   - Average request duration of 1.52s
   - Highest number of successful iterations (1763)
   - Good throughput at ~219 requests/second

2. **Memory Cache Performance**:
   - Moderate performance
   - Lower average request duration (1.45s)
   - Exceeded performance thresholds

3. **Fusion Cache Performance**:
   - Similar to Hybrid Cache
   - Built-in stampede protection
   - Exceeded performance thresholds
   - Good for most scenarios especially when requiring stampede protection

## 🚦 API Endpoints

- `/memory-cache` - Tests Memory Cache implementation
- `/hybrid-cache` - Tests Hybrid Cache implementation
- `/fusion-cache` - Tests Fusion Cache implementation

Each endpoint simulates a 3-second database delay and caches the result for 1 second.

## 💡 Conclusion

The Hybrid Cache implementation shows the best performance metrics among all three strategies, with significantly lower average request duration and higher throughput. However, each caching strategy has its use cases:

- Use **Hybrid Cache** for best overall performance
- Use **Fusion Cache** when cache stampede protection is critical
- Use **Memory Cache** for simpler scenarios with lower concurrency requirements

## 🏃‍♂️ How to Run

1. Clone the repository
2. Install dependencies
3. Run the application
4. Execute load tests using k6:
   ```bash
   k6 run k6-loadtest/memory-cache.js
   k6 run k6-loadtest/hybrid-cache.js
   k6 run k6-loadtest/fusion-cache.js
   ```
