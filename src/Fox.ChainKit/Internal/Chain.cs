//==================================================================================================
// Implements the chain execution engine.
// Executes handlers sequentially with diagnostics and exception handling support.
//==================================================================================================
using System.Diagnostics;

namespace Fox.ChainKit.Internal;

//==================================================================================================
/// <summary>
/// Implements the chain of responsibility pattern for executing handlers sequentially.
/// </summary>
/// <typeparam name="TContext">The type of context object to process.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="Chain{TContext}"/> class.
/// </remarks>
/// <param name="serviceProvider">The service provider to resolve handlers.</param>
/// <param name="handlers">The list of handler descriptors.</param>
/// <param name="exceptionHandler">The optional exception handler.</param>
/// <param name="diagnosticsHandler">The optional diagnostics handler.</param>
//==================================================================================================
internal sealed class Chain<TContext>(IServiceProvider serviceProvider, List<HandlerDescriptor<TContext>> handlers, Func<Exception, TContext, Task>? exceptionHandler, Action<ChainDiagnostics>? diagnosticsHandler) : IChain<TContext>
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly List<HandlerDescriptor<TContext>> handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public async Task RunAsync(TContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var diagnostics = diagnosticsHandler != null ? new ChainDiagnostics() : null;
        var chainStopwatch = diagnostics != null ? Stopwatch.StartNew() : null;

        try
        {
            foreach (var descriptor in handlers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (descriptor.Condition != null && !descriptor.Condition(context))
                {
                    diagnostics?.Handlers.Add(new HandlerDiagnostics(HandlerName: descriptor.HandlerType.Name, Skipped: true));
                    continue;
                }

                var handler = descriptor.Factory(serviceProvider);
                var stopwatch = diagnostics != null ? Stopwatch.StartNew() : null;

                try
                {
                    var result = await handler.HandleAsync(context, cancellationToken);

                    if (diagnostics != null)
                    {
                        stopwatch!.Stop();
                        diagnostics.Handlers.Add(new HandlerDiagnostics(HandlerName: descriptor.HandlerType.Name, ExecutionTime: stopwatch.Elapsed, Result: result));
                    }

                    if (result == HandlerResult.Stop)
                    {
                        if (diagnostics != null)
                        {
                            diagnostics.StoppedEarly = true;
                            diagnostics.EarlyStopReason = $"Handler '{descriptor.HandlerType.Name}' returned Stop";
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (diagnostics != null)
                    {
                        stopwatch?.Stop();
                        diagnostics.Handlers.Add(new HandlerDiagnostics(HandlerName: descriptor.HandlerType.Name, ExecutionTime: stopwatch?.Elapsed ?? TimeSpan.Zero, HasException: true, ExceptionMessage: ex.Message));
                    }

                    if (exceptionHandler != null)
                    {
                        await exceptionHandler(ex, context);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            if (diagnostics != null && chainStopwatch != null)
            {
                chainStopwatch.Stop();
                diagnostics.TotalExecutionTime = chainStopwatch.Elapsed;
                diagnosticsHandler!(diagnostics);
            }
        }
    }
}
