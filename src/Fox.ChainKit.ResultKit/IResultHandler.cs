//==================================================================================================
// Defines a Result-based handler in the chain of responsibility pattern.
// Handlers return Result objects for automatic failure handling.
//==================================================================================================
using Fox.ResultKit;

namespace Fox.ChainKit.ResultKit;

//==================================================================================================
/// <summary>
/// Represents a handler that processes a context object and returns a Result.
/// </summary>
/// <typeparam name="TContext">The type of context object to process.</typeparam>
//==================================================================================================
public interface IResultHandler<in TContext>
{
    //==============================================================================================
    /// <summary>
    /// Handles the context and returns a Result indicating success or failure.
    /// </summary>
    /// <param name="context">The context object to process.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the Result.</returns>
    //==============================================================================================
    Task<Result> HandleAsync(TContext context, CancellationToken cancellationToken = default);
}
