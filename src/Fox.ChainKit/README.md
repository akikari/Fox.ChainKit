# Fox.ChainKit

A lightweight, DI-friendly, modular Chain of Responsibility framework for .NET.

## Features

- **Explicit and Type-Safe**: No reflection, no runtime discovery, no magic
- **DI Integration**: First-class support for dependency injection
- **Diagnostics**: Built-in execution time tracking and handler result monitoring
- **Conditional Handlers**: Execute handlers based on runtime conditions
- **Exception Handling**: Optional exception handling with custom handlers
- **Early Exit**: Stop chain execution when needed
- **Clean Architecture**: SOLID principles, DRY, and testable design
- **Cross-Platform**: Targets .NET 8, .NET 9, and .NET 10

## Installation

```bash
dotnet add package Fox.ChainKit
```

## Quick Start

### 1. Define Your Context

```csharp
public class OrderContext
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public bool IsValid { get; set; }
}
```

### 2. Create Handlers

```csharp
public class ValidationHandler : IHandler<OrderContext>
{
    public Task<HandlerResult> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.OrderId))
        {
            return Task.FromResult(HandlerResult.Stop);
        }
        
        context.IsValid = true;
        return Task.FromResult(HandlerResult.Continue);
    }
}

public class ProcessingHandler : IHandler<OrderContext>
{
    public async Task<HandlerResult> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        // Process the order
        await Task.Delay(100, cancellationToken);
        return HandlerResult.Continue;
    }
}
```

### 3. Build and Execute Chain

```csharp
var services = new ServiceCollection();
services.AddTransient<ValidationHandler>();
services.AddTransient<ProcessingHandler>();

var serviceProvider = services.BuildServiceProvider();

var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddHandler<ProcessingHandler>()
    .Build();

var context = new OrderContext { OrderId = "ORD-123", Amount = 150.00m };
await chain.RunAsync(context);
```

## Advanced Features

### Conditional Handlers

Execute handlers only when specific conditions are met:

```csharp
var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddConditionalHandler<PremiumProcessingHandler>(ctx => ctx.Amount > 1000)
    .AddHandler<NotificationHandler>()
    .Build();
```

### Exception Handling

Handle exceptions gracefully with custom exception handlers:

```csharp
var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddHandler<ProcessingHandler>()
    .UseExceptionHandler(async (exception, context) =>
    {
        Console.WriteLine($"Error processing order {context.OrderId}: {exception.Message}");
    })
    .Build();
```

### Diagnostics

Track execution time and monitor handler results:

```csharp
var chain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddHandler<ProcessingHandler>()
    .UseDiagnostics(diagnostics =>
    {
        Console.WriteLine($"Total time: {diagnostics.TotalExecutionTime.TotalMilliseconds}ms");
        Console.WriteLine($"Handlers executed: {diagnostics.Handlers.Count}");
        
        if (diagnostics.StoppedEarly)
        {
            Console.WriteLine($"Stopped early: {diagnostics.EarlyStopReason}");
        }
    })
    .Build();
```

### DI Integration

Register chains directly in your service collection:

```csharp
services.AddChain<OrderContext>(builder =>
{
    builder.AddHandler<ValidationHandler>()
           .AddHandler<ProcessingHandler>()
           .AddHandler<NotificationHandler>();
});

// Later, resolve and use:
var chain = serviceProvider.GetRequiredService<IChain<OrderContext>>();
await chain.RunAsync(context);
```

## Handler Result

Handlers return `HandlerResult` to control chain flow:

- **`HandlerResult.Continue`**: Continue to the next handler
- **`HandlerResult.Stop`**: Stop the chain immediately (early exit)

## Best Practices

1. **Keep handlers focused**: Each handler should have a single responsibility
2. **Use dependency injection**: Register handlers in DI container for better testability
3. **Handle cancellation**: Always respect the `CancellationToken`
4. **Use diagnostics in development**: Enable diagnostics to understand chain behavior
5. **Test handlers individually**: Unit test each handler independently
6. **Consider conditional handlers**: Use conditions instead of adding logic inside handlers

## Testing

Fox.ChainKit is designed for testability:

```csharp
[Fact]
public async Task ValidationHandler_should_stop_chain_for_invalid_order()
{
    var handler = new ValidationHandler();
    var context = new OrderContext { OrderId = "" };
    
    var result = await handler.HandleAsync(context);
    
    result.Should().Be(HandlerResult.Stop);
}
```

## Result-Based Chains

For Result pattern integration, see [Fox.ChainKit.ResultKit](../Fox.ChainKit.ResultKit/README.md).

## License

MIT License - see LICENSE file for details.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## Links

- [GitHub Repository](https://github.com/akikari/Fox.ChainKit)
- [NuGet Package](https://www.nuget.org/packages/Fox.ChainKit)
- [Sample Application](../../samples/Fox.ChainKit.Demo)
