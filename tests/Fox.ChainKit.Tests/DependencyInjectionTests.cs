//==================================================================================================
// Tests for DI integration extensions.
// Verifies service collection registration and chain resolution.
//==================================================================================================
using FluentAssertions;
using Fox.ChainKit.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for dependency injection integration.
/// </summary>
//==================================================================================================
public sealed class DependencyInjectionTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that AddChain registers the chain correctly.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddChain_should_register_chain_in_service_collection()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerB>();

        services.AddChain<TestContext>(builder =>
        {
            builder.AddHandler<TestHandlerA>();
            builder.AddHandler<TestHandlerB>();
        });

        var serviceProvider = services.BuildServiceProvider();
        var chain = serviceProvider.GetRequiredService<IChain<TestContext>>();

        chain.Should().NotBeNull();

        var context = new TestContext();
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().ContainInOrder(
            nameof(TestHandlerA),
            nameof(TestHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddChain throws ArgumentNullException when services is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddChain_should_throw_when_services_is_null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddChain<TestContext>(builder => { });

        act.Should().Throw<ArgumentNullException>();
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddChain throws ArgumentNullException when configure is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddChain_should_throw_when_configure_is_null()
    {
        var services = new ServiceCollection();

        var act = () => services.AddChain<TestContext>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddChainBuilder registers the builder correctly.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddChainBuilder_should_register_builder_in_service_collection()
    {
        var services = new ServiceCollection();
        services.AddChainBuilder<TestContext>();

        var serviceProvider = services.BuildServiceProvider();
        var builder = serviceProvider.GetRequiredService<IChainBuilder<TestContext>>();

        builder.Should().NotBeNull();
        builder.Should().BeOfType<ChainBuilder<TestContext>>();
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddChainBuilder throws ArgumentNullException when services is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddChainBuilder_should_throw_when_services_is_null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddChainBuilder<TestContext>();

        act.Should().Throw<ArgumentNullException>();
    }
}
