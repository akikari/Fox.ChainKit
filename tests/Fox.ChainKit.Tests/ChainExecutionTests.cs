//==================================================================================================
// Tests for basic chain execution.
// Verifies handler execution order and basic chain flow.
//==================================================================================================
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for basic chain execution functionality.
/// </summary>
//==================================================================================================
public sealed class ChainExecutionTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that handlers execute in the correct order.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task RunAsync_should_execute_handlers_in_order()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerB>();
        services.AddTransient<TestHandlerC>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddHandler<TestHandlerB>()
            .AddHandler<TestHandlerC>()
            .Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().ContainInOrder(
            nameof(TestHandlerA),
            nameof(TestHandlerB),
            nameof(TestHandlerC));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that the chain stops when a handler returns Stop.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task RunAsync_should_stop_when_handler_returns_stop()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerStop>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddHandler<TestHandlerStop>()
            .AddHandler<TestHandlerB>()
            .Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().ContainInOrder(
            nameof(TestHandlerA),
            nameof(TestHandlerStop));

        context.ExecutedHandlers.Should().NotContain(nameof(TestHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that an empty chain executes without error.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task RunAsync_should_handle_empty_chain()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider).Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().BeEmpty();
    }

    //==============================================================================================
    /// <summary>
    /// Tests that RunAsync throws ArgumentNullException when context is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task RunAsync_should_throw_when_context_is_null()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider).Build();

        var act = async () => await chain.RunAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
