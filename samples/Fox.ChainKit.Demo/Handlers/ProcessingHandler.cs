//==================================================================================================
// Processes the order after validation.
// Simulates asynchronous order processing operation.
//==================================================================================================
using Fox.ChainKit.Demo.Context;
using Fox.ChainKit.Demo.Logging;

namespace Fox.ChainKit.Demo.Handlers;

//==================================================================================================
/// <summary>
/// Processes the order.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ProcessingHandler"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
//==================================================================================================
internal sealed class ProcessingHandler(ILogger logger) : IHandler<OrderContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public async Task<HandlerResult> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.ProcessingLog.Add("Processing");
        logger.LogInformation($"Processing order {context.OrderId}...");

        await Task.Delay(50, cancellationToken);

        logger.LogSuccess($"Order processed for ${context.Amount}");
        return HandlerResult.Continue;
    }
}
