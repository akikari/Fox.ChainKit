//==================================================================================================
// Implements the fluent builder for constructing chains.
// Allows adding handlers, conditions, diagnostics, and exception handling.
//==================================================================================================
using Fox.ChainKit.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Implements the fluent builder for constructing a chain of handlers.
/// </summary>
/// <typeparam name="TContext">The type of context object the chain will process.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ChainBuilder{TContext}"/> class.
/// </remarks>
/// <param name="serviceProvider">The service provider to resolve handlers.</param>
//==================================================================================================
public sealed class ChainBuilder<TContext>(IServiceProvider serviceProvider) : IChainBuilder<TContext>
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly List<HandlerDescriptor<TContext>> handlers = [];
    private Func<Exception, TContext, Task>? exceptionHandler;
    private Action<ChainDiagnostics>? diagnosticsHandler;

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public IChainBuilder<TContext> AddHandler<THandler>() where THandler : IHandler<TContext>
    {
        handlers.Add(new HandlerDescriptor<TContext>
        {
            Factory = sp => sp.GetRequiredService<THandler>(),
            HandlerType = typeof(THandler)
        });

        return this;
    }

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public IChainBuilder<TContext> AddConditionalHandler<THandler>(Func<TContext, bool> predicate) where THandler : IHandler<TContext>
    {
        ArgumentNullException.ThrowIfNull(predicate);

        handlers.Add(new HandlerDescriptor<TContext>
        {
            Factory = sp => sp.GetRequiredService<THandler>(),
            Condition = predicate,
            HandlerType = typeof(THandler)
        });

        return this;
    }

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public IChainBuilder<TContext> UseExceptionHandler(Func<Exception, TContext, Task> exceptionHandler)
    {
        this.exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        return this;
    }

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public IChainBuilder<TContext> UseDiagnostics(Action<ChainDiagnostics> diagnosticsHandler)
    {
        this.diagnosticsHandler = diagnosticsHandler ?? throw new ArgumentNullException(nameof(diagnosticsHandler));
        return this;
    }

    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public IChain<TContext> Build()
    {
        return new Chain<TContext>(serviceProvider, handlers, exceptionHandler, diagnosticsHandler);
    }

    //==============================================================================================
    /// <summary>
    /// Adds a handler descriptor directly. Used by extension methods.
    /// </summary>
    /// <param name="descriptor">The handler descriptor to add.</param>
    //==============================================================================================
    internal void AddHandlerDescriptor(HandlerDescriptor<TContext> descriptor)
    {
        handlers.Add(descriptor ?? throw new ArgumentNullException(nameof(descriptor)));
    }
}
