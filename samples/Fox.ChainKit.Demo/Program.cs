//==================================================================================================
// Fox.ChainKit Demo Application
// Demonstrates basic and Result-based chain execution with diagnostics.
//==================================================================================================
using Fox.ChainKit;
using Fox.ChainKit.Demo.Context;
using Fox.ChainKit.Demo.Handlers;
using Fox.ChainKit.Demo.Logging;
using Fox.ChainKit.ResultKit.Extensions;
using Fox.ResultKit;
using Microsoft.Extensions.DependencyInjection;

//==================================================================================================
// Setup DI container
//==================================================================================================

var services = new ServiceCollection();

services.AddSingleton<ILogger, ConsoleLogger>();
services.AddTransient<ValidationHandler>();
services.AddTransient<ProcessingHandler>();
services.AddTransient<NotificationHandler>();
services.AddTransient<ResultValidationHandler>();
services.AddTransient<ResultProcessingHandler>();

var serviceProvider = services.BuildServiceProvider();

//==================================================================================================
// Example 1: Basic chain execution
//==================================================================================================

Console.WriteLine("=== Example 1: Basic Chain Execution ===\n");

var basicChain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddHandler<ProcessingHandler>()
    .AddHandler<NotificationHandler>()
    .UseDiagnostics(diagnostics =>
    {
        Console.WriteLine($"Total execution time: {diagnostics.TotalExecutionTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"Handlers executed: {diagnostics.Handlers.Count}");

        foreach (var handler in diagnostics.Handlers)
        {
            Console.WriteLine($"  - {handler.HandlerName}: {handler.ExecutionTime.TotalMilliseconds:F2}ms (Result: {handler.Result})");
        }
    })
    .Build();

var orderContext = new OrderContext
{
    OrderId = "ORD-12345",
    Amount = 150.00m,
    CustomerEmail = "customer@example.com"
};

await basicChain.RunAsync(orderContext);

Console.WriteLine($"\nProcessing log: {string.Join(" -> ", orderContext.ProcessingLog)}\n");

//==================================================================================================
// Example 2: Chain with early exit
//==================================================================================================

Console.WriteLine("=== Example 2: Chain with Early Exit ===\n");

var earlyExitChain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddHandler<ProcessingHandler>()
    .AddHandler<NotificationHandler>()
    .UseDiagnostics(diagnostics =>
    {
        if (diagnostics.StoppedEarly)
        {
            Console.WriteLine($"Chain stopped early: {diagnostics.EarlyStopReason}");
        }
    })
    .Build();

var invalidOrderContext = new OrderContext
{
    OrderId = "",
    Amount = -10.00m,
    CustomerEmail = "invalid@example.com"
};

await earlyExitChain.RunAsync(invalidOrderContext);

Console.WriteLine($"\nProcessing log: {string.Join(" -> ", invalidOrderContext.ProcessingLog)}\n");

//==================================================================================================
// Example 3: Result-based chain
//==================================================================================================

Console.WriteLine("=== Example 3: Result-based Chain ===\n");

var resultChain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddResultHandler<OrderContext, ResultValidationHandler>()
    .AddResultHandler<OrderContext, ResultProcessingHandler>()
    .UseDiagnostics(diagnostics =>
    {
        Console.WriteLine($"Total execution time: {diagnostics.TotalExecutionTime.TotalMilliseconds:F2}ms");
        Console.WriteLine(diagnostics.FormatResultDiagnostics());
    })
    .Build();

var resultOrderContext = new OrderContext
{
    OrderId = "ORD-67890",
    Amount = 250.00m,
    CustomerEmail = "result@example.com"
};

await resultChain.RunAsync(resultOrderContext);

Console.WriteLine($"\nProcessing log: {string.Join(" -> ", resultOrderContext.ProcessingLog)}\n");

//==================================================================================================
// Example 4: Conditional handlers
//==================================================================================================

Console.WriteLine("=== Example 4: Conditional Handlers ===\n");

var conditionalChain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddConditionalHandler<ProcessingHandler>(ctx => ctx.Amount > 100)
    .AddHandler<NotificationHandler>()
    .UseDiagnostics(diagnostics =>
    {
        var skipped = diagnostics.Handlers.Count(h => h.Skipped);
        Console.WriteLine($"Skipped handlers: {skipped}");
    })
    .Build();

var smallOrderContext = new OrderContext
{
    OrderId = "ORD-SMALL",
    Amount = 50.00m,
    CustomerEmail = "small@example.com"
};

await conditionalChain.RunAsync(smallOrderContext);

Console.WriteLine($"\nProcessing log: {string.Join(" -> ", smallOrderContext.ProcessingLog)}\n");

//==================================================================================================
// Example 5: Mixing handlers and capturing Results
//==================================================================================================

Console.WriteLine("=== Example 5: Mixing Handler Types with Result Capture ===\n");

var results = new List<(string HandlerName, Result Result)>();

var mixedChain = new ChainBuilder<OrderContext>(serviceProvider)
    .AddHandler<ValidationHandler>()
    .AddResultHandler<OrderContext, ResultValidationHandler>(result =>
    {
        results.Add(("ResultValidationHandler", result));
        if (!result.IsSuccess)
        {
            Console.WriteLine($"  [ERROR] Result validation failed: {result.Error}");
        }
    })
    .AddResultHandler<OrderContext, ResultProcessingHandler>(result =>
    {
        results.Add(("ResultProcessingHandler", result));
    })
    .Build();

var mixedOrderContext = new OrderContext
{
    OrderId = "ORD-MIXED",
    Amount = 300.00m,
    CustomerEmail = "mixed@example.com"
};

await mixedChain.RunAsync(mixedOrderContext);

Console.WriteLine($"\nProcessing log: {string.Join(" -> ", mixedOrderContext.ProcessingLog)}");
Console.WriteLine($"Captured {results.Count} Result objects:");

foreach (var (name, result) in results)
{
    Console.WriteLine($"  - {name}: {(result.IsSuccess ? "[OK] Success" : $"[ERROR] Failed - {result.Error}")}");
}

Console.WriteLine();
