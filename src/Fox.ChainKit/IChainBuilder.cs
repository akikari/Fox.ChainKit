//==================================================================================================
// Defines the fluent builder interface for constructing chains.
// Provides methods to add handlers, diagnostics, and exception handling.
//==================================================================================================

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Provides a fluent interface for building a chain of handlers.
/// </summary>
/// <typeparam name="TContext">The type of context object the chain will process.</typeparam>
//==================================================================================================
public interface IChainBuilder<TContext>
{
    //==============================================================================================
    /// <summary>
    /// Adds a handler to the chain.
    /// </summary>
    /// <typeparam name="THandler">The type of handler to add.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    IChainBuilder<TContext> AddHandler<THandler>() where THandler : IHandler<TContext>;

    //==============================================================================================
    /// <summary>
    /// Adds a conditional handler that only executes when the predicate returns true.
    /// </summary>
    /// <typeparam name="THandler">The type of handler to add.</typeparam>
    /// <param name="predicate">The condition to evaluate before executing the handler.</param>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    IChainBuilder<TContext> AddConditionalHandler<THandler>(Func<TContext, bool> predicate) where THandler : IHandler<TContext>;

    //==============================================================================================
    /// <summary>
    /// Configures exception handling for the chain.
    /// </summary>
    /// <param name="exceptionHandler">The handler to invoke when an exception occurs.</param>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    IChainBuilder<TContext> UseExceptionHandler(Func<Exception, TContext, Task> exceptionHandler);

    //==============================================================================================
    /// <summary>
    /// Enables diagnostics for the chain execution.
    /// </summary>
    /// <param name="diagnosticsHandler">The handler to receive diagnostic information.</param>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    IChainBuilder<TContext> UseDiagnostics(Action<ChainDiagnostics> diagnosticsHandler);

    //==============================================================================================
    /// <summary>
    /// Builds the chain from the configured handlers.
    /// </summary>
    /// <returns>The configured chain ready for execution.</returns>
    //==============================================================================================
    IChain<TContext> Build();
}
