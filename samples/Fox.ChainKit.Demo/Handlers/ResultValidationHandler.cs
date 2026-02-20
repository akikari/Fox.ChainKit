//==================================================================================================
// Validates order data using Result pattern.
// Returns Result.Failure if validation fails, Result.Success otherwise.
//==================================================================================================
using Fox.ChainKit.Demo.Context;
using Fox.ChainKit.Demo.Logging;
using Fox.ChainKit.ResultKit;
using Fox.ResultKit;

namespace Fox.ChainKit.Demo.Handlers;

//==================================================================================================
/// <summary>
/// Validates order data using Result pattern.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultValidationHandler"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
//==================================================================================================
internal sealed class ResultValidationHandler(ILogger logger) : IResultHandler<OrderContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.ProcessingLog.Add("ResultValidation");
        logger.LogInformation($"Result-based validation for order {context.OrderId}...");

        if (string.IsNullOrEmpty(context.OrderId) || context.Amount <= 0)
        {
            logger.LogError("Validation failed!");
            return Task.FromResult(Result.Failure("Invalid order data"));
        }

        logger.LogSuccess("Validation passed!");
        return Task.FromResult(Result.Success());
    }
}
