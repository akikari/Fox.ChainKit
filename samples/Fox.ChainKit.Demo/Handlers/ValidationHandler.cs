//==================================================================================================
// Validates order data before processing.
// Returns Stop if validation fails, Continue otherwise.
//==================================================================================================
using Fox.ChainKit.Demo.Context;
using Fox.ChainKit.Demo.Logging;

namespace Fox.ChainKit.Demo.Handlers;

//==================================================================================================
/// <summary>
/// Validates order data before processing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationHandler"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
//==================================================================================================
internal sealed class ValidationHandler(ILogger logger) : IHandler<OrderContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.ProcessingLog.Add("Validation");
        logger.LogInformation($"Validating order {context.OrderId}...");

        if (string.IsNullOrEmpty(context.OrderId) || context.Amount <= 0)
        {
            context.IsValid = false;
            logger.LogError("Validation failed!");
            return Task.FromResult(HandlerResult.Stop);
        }

        logger.LogSuccess("Validation passed!");
        return Task.FromResult(HandlerResult.Continue);
    }
}
