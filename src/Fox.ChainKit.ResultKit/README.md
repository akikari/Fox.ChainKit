# Fox.ChainKit.ResultKit

Result pattern integration for Fox.ChainKit, providing automatic failure handling and seamless Result-based chain execution.

## Features

- **Result-Based Handlers**: Work directly with `Result` objects from Fox.ResultKit
- **Automatic Failure Handling**: `Result.Failure` automatically stops the chain
- **Success Continuation**: `Result.Success` continues to the next handler
- **Result Diagnostics**: Track and monitor Result-specific execution information
- **Conditional Result Handlers**: Execute Result handlers based on conditions
- **Result Callbacks**: Capture Result objects for logging or analysis

## Installation

```bash
dotnet add package Fox.ChainKit.ResultKit
```

This package requires:
- `Fox.ChainKit` (core package)
- `Fox.ResultKit` (Result pattern implementation)

## Quick Start

### 1. Define Result Handlers

```csharp
public class ValidationHandler : IResultHandler<OrderContext>
{
    public Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.OrderId))
        {
            return Task.FromResult(Result.Failure("Order ID is required"));
        }
        
        return Task.FromResult(Result.Success());
    }
}

public class ProcessingHandler : IResultHandler<OrderContext>
{
    public async Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await ProcessOrderAsync(context, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Processing failed: {ex.Message}");
        }
    }
}
```

### 2. Build Result-Based Chain

```csharp
using Fox.ChainKit.ResultKit.Extensions;

var services = new ServiceCollection();
services.AddTransient<ValidationHandler>();
services.AddTransient<ProcessingHandler>();

var serviceProvider = services.BuildServiceProvider();

var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddResultHandler<OrderContext, ValidationHandler>()
    .AddResultHandler<OrderContext, ProcessingHandler>()
    .Build();

var context = new OrderContext { OrderId = "ORD-123", Amount = 150.00m };
await chain.RunAsync(context);
```

## Result Flow

### Success Flow

When a Result handler returns `Result.Success()`:
- The handler is marked as successful
- The chain **continues** to the next handler
- Equivalent to `HandlerResult.Continue`

```csharp
public Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken)
{
    // Validation passes
    return Task.FromResult(Result.Success());
}
```

### Failure Flow

When a Result handler returns `Result.Failure()`:
- The handler is marked as failed
- The chain **stops immediately** (early exit)
- Remaining handlers are not executed
- Equivalent to `HandlerResult.Stop`

```csharp
public Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken)
{
    // Validation fails
    return Task.FromResult(Result.Failure("Invalid order data"));
}
```

## Advanced Features

### Result Callbacks

Capture Result objects for logging, diagnostics, or error handling:

```csharp
var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddResultHandler<OrderContext, ValidationHandler>(result =>
    {
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Validation failed: {result.Error}");
        }
    })
    .AddResultHandler<OrderContext, ProcessingHandler>(result =>
    {
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Processing failed: {result.Error}");
        }
    })
    .Build();
```

### Conditional Result Handlers

Execute Result handlers only when conditions are met:

```csharp
var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddResultHandler<OrderContext, ValidationHandler>()
    .AddConditionalResultHandler<OrderContext, PremiumValidationHandler>(
        ctx => ctx.Amount > 1000)
    .AddResultHandler<OrderContext, ProcessingHandler>()
    .Build();
```

### Result Diagnostics

Monitor Result-based chain execution:

```csharp
using Fox.ChainKit.ResultKit.Extensions;

var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddResultHandler<OrderContext, ValidationHandler>()
    .AddResultHandler<OrderContext, ProcessingHandler>()
    .UseDiagnostics(diagnostics =>
    {
        Console.WriteLine(diagnostics.FormatResultDiagnostics());
        // Output: "Result handlers: 2, Failed: 0, Skipped: 0"
        
        if (diagnostics.StoppedEarly)
        {
            Console.WriteLine($"Chain stopped: {diagnostics.EarlyStopReason}");
        }
    })
    .Build();
```

## Mixing Handler Types

You can mix standard handlers and Result handlers in the same chain:

```csharp
var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<LoggingHandler>()                                  // Standard handler
    .AddResultHandler<OrderContext, ValidationHandler>()           // Result handler
    .AddResultHandler<OrderContext, ProcessingHandler>()           // Result handler
    .AddHandler<NotificationHandler>()                             // Standard handler
    .Build();
```

### Capturing Results When Mixing Handler Types

When mixing standard and Result handlers, you need to explicitly capture Result objects to access error information. There are three recommended approaches:

#### Approach 1: Result Callbacks (Recommended)

Use callbacks to capture each Result handler's output:

```csharp
var results = new List<(string HandlerName, Result Result)>();

var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<LoggingHandler>()
    .AddResultHandler<OrderContext, ValidationHandler>(result =>
    {
        results.Add(("ValidationHandler", result));
        if (!result.IsSuccess)
        {
            logger.LogError($"Validation failed: {result.Error}");
        }
    })
    .AddResultHandler<OrderContext, ProcessingHandler>(result =>
    {
        results.Add(("ProcessingHandler", result));
    })
    .AddHandler<NotificationHandler>()
    .Build();

await chain.RunAsync(context);

// Access all captured Results
foreach (var (name, result) in results)
{
    Console.WriteLine($"{name}: {(result.IsSuccess ? "Success" : $"Failed - {result.Error}")}");
}
```

#### Approach 2: Context-Based Result Storage

Add a Result collection to your context:

```csharp
public class OrderContext
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }

    // Result storage
    public List<Result> Results { get; } = [];
}

// In your Result handler:
public Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken)
{
    var result = PerformValidation(context);
    context.Results.Add(result);  // Store in context
    return Task.FromResult(result);
}

// After chain execution:
await chain.RunAsync(context);

if (context.Results.Any(r => !r.IsSuccess))
{
    var errors = context.Results.Where(r => !r.IsSuccess).Select(r => r.Error);
    Console.WriteLine($"Chain failed with errors: {string.Join(", ", errors)}");
}
```

#### Approach 3: Dedicated Result Collector Service

Create a collector service for cross-cutting Result tracking:

```csharp
public sealed class ResultCollector
{
    public List<(string HandlerName, Result Result)> Results { get; } = [];

    public void Collect(string handlerName, Result result)
    {
        Results.Add((handlerName, result));
    }

    public bool HasFailures => Results.Any(r => !r.Result.IsSuccess);

    public IEnumerable<string> GetFailureMessages() =>
        Results.Where(r => !r.Result.IsSuccess).Select(r => r.Result.Error ?? "Unknown error");
}

// Register as scoped or singleton
services.AddScoped<ResultCollector>();

// Inject into context or handlers
public class OrderContext
{
    public ResultCollector ResultCollector { get; set; } = null!;
}

// Use in Result handler
public Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken)
{
    var result = PerformValidation(context);
    context.ResultCollector.Collect("ValidationHandler", result);
    return Task.FromResult(result);
}
```

### Comparison of Approaches

| Approach | Best For | Pros | Cons |
|----------|----------|------|------|
| **Callbacks** | Simple scenarios | No context modification needed | Verbose for many handlers |
| **Context Storage** | Business logic needs Results | Results available in context | Couples context to Result pattern |
| **Collector Service** | Cross-cutting concerns | Clean separation, reusable | Additional service registration |

**Recommendation**: Use **callbacks** for simple cases, **context storage** when business logic needs Results, and **collector service** for complex audit/logging scenarios.

## Best Practices

1. **Use Result for business logic failures**: Return `Result.Failure()` for expected failures
2. **Let exceptions bubble for infrastructure issues**: Use exception handling for unexpected errors
3. **Capture Results for audit trails**: Use callbacks to log all Result objects
4. **Keep error messages meaningful**: Provide clear, actionable error messages
5. **Test failure scenarios**: Ensure failure Results properly stop the chain

## Testing

Test Result handlers independently:

```csharp
[Fact]
public async Task ValidationHandler_should_return_failure_for_empty_order_id()
{
    var handler = new ValidationHandler();
    var context = new OrderContext { OrderId = "" };
    
    var result = await handler.HandleAsync(context);
    
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be("Order ID is required");
}

[Fact]
public async Task ValidationHandler_should_return_success_for_valid_order()
{
    var handler = new ValidationHandler();
    var context = new OrderContext { OrderId = "ORD-123", Amount = 100m };
    
    var result = await handler.HandleAsync(context);
    
    result.IsSuccess.Should().BeTrue();
}
```

## When to Use Result-Based Handlers

Use `IResultHandler<TContext>` when:
- You need explicit success/failure tracking
- You want automatic chain stopping on failure
- You're already using Fox.ResultKit in your application
- You need to return error information from handlers

Use `IHandler<TContext>` when:
- You need fine-grained control over chain flow
- Failure is not binary (success/failure)
- You don't need Result pattern semantics

## Comparison: IHandler vs IResultHandler

| Feature | IHandler<TContext> | IResultHandler<TContext> |
|---------|-------------------|-------------------------|
| Return Type | `HandlerResult` | `Result` |
| Success | `HandlerResult.Continue` | `Result.Success()` |
| Failure | `HandlerResult.Stop` | `Result.Failure(error)` |
| Error Info | Not included | Included in Result |
| Use Case | General purpose | Business logic with failures |

## License

MIT License - see LICENSE file for details.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## Links

- [Fox.ChainKit Core](../Fox.ChainKit/README.md)
- [Fox.ResultKit](https://github.com/akikari/Fox.ResultKit)
- [GitHub Repository](https://github.com/akikari/Fox.ChainKit)
- [NuGet Package](https://www.nuget.org/packages/Fox.ChainKit.ResultKit)
- [Sample Application](../../samples/Fox.ChainKit.Demo)
