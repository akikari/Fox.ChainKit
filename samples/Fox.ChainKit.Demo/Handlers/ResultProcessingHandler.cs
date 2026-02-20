//==================================================================================================
// Processes order using Result pattern.
// Simulates asynchronous order processing with Result return type.
//==================================================================================================
using Fox.ChainKit.Demo.Context;
using Fox.ChainKit.Demo.Logging;
using Fox.ChainKit.ResultKit;
using Fox.ResultKit;

namespace Fox.ChainKit.Demo.Handlers;

//==================================================================================================
/// <summary>
/// Processes order using Result pattern.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultProcessingHandler"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
//==================================================================================================
internal sealed class ResultProcessingHandler(ILogger logger) : IResultHandler<OrderContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public async Task<Result> HandleAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.ProcessingLog.Add("ResultProcessing");
        logger.LogInformation($"Result-based processing for order {context.OrderId}...");

        await Task.Delay(50, cancellationToken);

        logger.LogSuccess($"Order processed for ${context.Amount}");
        return Result.Success();
    }
}
