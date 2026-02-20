//==================================================================================================
// Defines a handler in the chain of responsibility pattern.
// Each handler processes a context and returns whether the chain should continue.
//==================================================================================================

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Represents a handler that processes a context object in a chain of responsibility.
/// </summary>
/// <typeparam name="TContext">The type of context object to process.</typeparam>
//==================================================================================================
public interface IHandler<in TContext>
{
    //==============================================================================================
    /// <summary>
    /// Handles the context and returns whether the chain should continue or stop.
    /// </summary>
    /// <param name="context">The context object to process.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the handler result.</returns>
    //==============================================================================================
    Task<HandlerResult> HandleAsync(TContext context, CancellationToken cancellationToken = default);
}
