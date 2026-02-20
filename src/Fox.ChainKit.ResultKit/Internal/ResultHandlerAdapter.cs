//==================================================================================================
// Internal adapter that converts Result-based handlers to standard handlers.
// Automatically maps Result.Success to Continue and Result.Failure to Stop.
//==================================================================================================
using Fox.ResultKit;

namespace Fox.ChainKit.ResultKit.Internal;

//==================================================================================================
/// <summary>
/// Adapts an IResultHandler to the standard IHandler interface.
/// </summary>
/// <typeparam name="TContext">The type of context object to process.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultHandlerAdapter{TContext}"/> class.
/// </remarks>
/// <param name="resultHandler">The result handler to adapt.</param>
/// <param name="resultCallback">Optional callback to capture the result.</param>
//==================================================================================================
internal sealed class ResultHandlerAdapter<TContext>(IResultHandler<TContext> resultHandler, Action<Result>? resultCallback = null) : IHandler<TContext>
{
    private readonly IResultHandler<TContext> resultHandler = resultHandler ?? throw new ArgumentNullException(nameof(resultHandler));

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public async Task<HandlerResult> HandleAsync(TContext context, CancellationToken cancellationToken = default)
    {
        var result = await resultHandler.HandleAsync(context, cancellationToken);

        resultCallback?.Invoke(result);

        return result.IsSuccess ? HandlerResult.Continue : HandlerResult.Stop;
    }
}
