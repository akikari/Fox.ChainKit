//==================================================================================================
// Internal descriptor for a handler in the chain.
// Contains the handler factory and optional condition predicate.
//==================================================================================================

namespace Fox.ChainKit.Internal;

//==================================================================================================
/// <summary>
/// Internal descriptor for a handler in the chain.
/// </summary>
/// <typeparam name="TContext">The type of context object.</typeparam>
//==================================================================================================
internal sealed class HandlerDescriptor<TContext>
{
    //==============================================================================================
    /// <summary>
    /// Gets the factory function to create the handler instance.
    /// </summary>
    //==============================================================================================
    public Func<IServiceProvider, IHandler<TContext>> Factory { get; init; } = null!;

    //==============================================================================================
    /// <summary>
    /// Gets the optional condition predicate that determines if the handler should execute.
    /// </summary>
    //==============================================================================================
    public Func<TContext, bool>? Condition { get; init; }

    //==============================================================================================
    /// <summary>
    /// Gets the type of the handler.
    /// </summary>
    //==============================================================================================
    public Type HandlerType { get; init; } = null!;
}
