//==================================================================================================
// Extension methods for integrating Result-based handlers into chains.
// Provides fluent API for adding ResultKit handlers with automatic conversion.
//==================================================================================================
using Fox.ChainKit.Internal;
using Fox.ChainKit.ResultKit.Internal;
using Fox.ResultKit;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.ResultKit.Extensions;

//==================================================================================================
/// <summary>
/// Extension methods for adding Result-based handlers to chains.
/// </summary>
//==================================================================================================
public static class ChainBuilderExtensions
{
    //==============================================================================================
    /// <summary>
    /// Adds a Result-based handler to the chain. Automatically converts Result.Failure to Stop.
    /// </summary>
    /// <typeparam name="TContext">The type of context object.</typeparam>
    /// <typeparam name="THandler">The type of Result handler to add.</typeparam>
    /// <param name="builder">The chain builder.</param>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    public static IChainBuilder<TContext> AddResultHandler<TContext, THandler>(this IChainBuilder<TContext> builder) where THandler : IResultHandler<TContext>
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder is not ChainBuilder<TContext> chainBuilder)
        {
            throw new InvalidOperationException("Builder must be a ChainBuilder instance to add Result handlers.");
        }

        chainBuilder.AddHandlerDescriptor(new HandlerDescriptor<TContext>
        {
            Factory = sp =>
            {
                var resultHandler = sp.GetRequiredService<THandler>();
                return new ResultHandlerAdapter<TContext>(resultHandler);
            },
            HandlerType = typeof(THandler)
        });

        return builder;
    }

    //==============================================================================================
    /// <summary>
    /// Adds a Result-based handler with diagnostics callback to the chain.
    /// </summary>
    /// <typeparam name="TContext">The type of context object.</typeparam>
    /// <typeparam name="THandler">The type of Result handler to add.</typeparam>
    /// <param name="builder">The chain builder.</param>
    /// <param name="onResult">Callback to receive the Result after handler execution.</param>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    public static IChainBuilder<TContext> AddResultHandler<TContext, THandler>(this IChainBuilder<TContext> builder, Action<Result> onResult) where THandler : IResultHandler<TContext>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(onResult);

        if (builder is not ChainBuilder<TContext> chainBuilder)
        {
            throw new InvalidOperationException("Builder must be a ChainBuilder instance to add Result handlers.");
        }

        chainBuilder.AddHandlerDescriptor(new HandlerDescriptor<TContext>
        {
            Factory = sp =>
            {
                var resultHandler = sp.GetRequiredService<THandler>();
                return new ResultHandlerAdapter<TContext>(resultHandler, onResult);
            },
            HandlerType = typeof(THandler)
        });

        return builder;
    }

    //==============================================================================================
    /// <summary>
    /// Adds a conditional Result-based handler to the chain.
    /// </summary>
    /// <typeparam name="TContext">The type of context object.</typeparam>
    /// <typeparam name="THandler">The type of Result handler to add.</typeparam>
    /// <param name="builder">The chain builder.</param>
    /// <param name="predicate">The condition to evaluate before executing the handler.</param>
    /// <returns>The builder instance for method chaining.</returns>
    //==============================================================================================
    public static IChainBuilder<TContext> AddConditionalResultHandler<TContext, THandler>(this IChainBuilder<TContext> builder, Func<TContext, bool> predicate) where THandler : IResultHandler<TContext>
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(predicate);

        if (builder is not ChainBuilder<TContext> chainBuilder)
        {
            throw new InvalidOperationException("Builder must be a ChainBuilder instance to add Result handlers.");
        }

        chainBuilder.AddHandlerDescriptor(new HandlerDescriptor<TContext>
        {
            Factory = sp =>
            {
                var resultHandler = sp.GetRequiredService<THandler>();
                return new ResultHandlerAdapter<TContext>(resultHandler);
            },
            Condition = predicate,
            HandlerType = typeof(THandler)
        });

        return builder;
    }
}
