//==================================================================================================
// Defines the chain execution interface.
// Represents a configured chain ready to process contexts.
//==================================================================================================

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Represents a chain of handlers that can process a context object.
/// </summary>
/// <typeparam name="TContext">The type of context object to process.</typeparam>
//==================================================================================================
public interface IChain<in TContext>
{
    //==============================================================================================
    /// <summary>
    /// Executes the chain of handlers for the given context.
    /// </summary>
    /// <param name="context">The context object to process.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    //==============================================================================================
    Task RunAsync(TContext context, CancellationToken cancellationToken = default);
}
