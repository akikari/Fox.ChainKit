//==================================================================================================
// Provides extension methods for registering chains with dependency injection.
// Integrates chain builders with IServiceCollection for easy DI setup.
//==================================================================================================
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Extensions;

//==================================================================================================
/// <summary>
/// Extension methods for integrating chains with dependency injection.
/// </summary>
//==================================================================================================
public static class ServiceCollectionExtensions
{
    //==============================================================================================
    /// <summary>
    /// Registers a chain and its handlers with the dependency injection container.
    /// </summary>
    /// <typeparam name="TContext">The type of context object the chain will process.</typeparam>
    /// <param name="services">The service collection to register the chain with.</param>
    /// <param name="configure">The action to configure the chain builder.</param>
    /// <returns>The service collection for method chaining.</returns>
    //==============================================================================================
    public static IServiceCollection AddChain<TContext>(this IServiceCollection services, Action<IChainBuilder<TContext>> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddSingleton<IChain<TContext>>(sp =>
        {
            var builder = new ChainBuilder<TContext>(sp);
            configure(builder);
            return builder.Build();
        });

        return services;
    }

    //==============================================================================================
    /// <summary>
    /// Registers a chain builder factory with the dependency injection container.
    /// </summary>
    /// <typeparam name="TContext">The type of context object the chain will process.</typeparam>
    /// <param name="services">The service collection to register the chain builder with.</param>
    /// <returns>The service collection for method chaining.</returns>
    //==============================================================================================
    public static IServiceCollection AddChainBuilder<TContext>(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IChainBuilder<TContext>>(sp => new ChainBuilder<TContext>(sp));

        return services;
    }
}
