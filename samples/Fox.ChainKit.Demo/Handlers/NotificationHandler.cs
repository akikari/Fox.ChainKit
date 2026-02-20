//==================================================================================================
// Sends notification email to customer after processing.
// Final step in the order processing chain.
//==================================================================================================
using Fox.ChainKit.Demo.Context;
using Fox.ChainKit.Demo.Logging;

namespace Fox.ChainKit.Demo.Handlers;

//==================================================================================================
/// <summary>
/// Sends notification email to customer.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NotificationHandler"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
//==================================================================================================
internal sealed class NotificationHandler(ILogger logger) : IHandler<OrderContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.ProcessingLog.Add("Notification");
        logger.LogInformation($"Sending notification to {context.CustomerEmail}...");
        logger.LogSuccess("Notification sent!");
        return Task.FromResult(HandlerResult.Continue);
    }
}
