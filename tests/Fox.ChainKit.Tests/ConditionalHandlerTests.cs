//==================================================================================================
// Tests for conditional handler execution.
// Verifies that handlers execute only when conditions are met.
//==================================================================================================
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for conditional handler functionality.
/// </summary>
//==================================================================================================
public sealed class ConditionalHandlerTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that conditional handler executes when condition is true.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddConditionalHandler_should_execute_when_condition_is_true()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddConditionalHandler<TestHandlerB>(ctx => ctx.ShouldExecute)
            .Build();

        var context = new TestContext { ShouldExecute = true };
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().Contain(nameof(TestHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that conditional handler is skipped when condition is false.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddConditionalHandler_should_skip_when_condition_is_false()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddConditionalHandler<TestHandlerB>(ctx => ctx.ShouldExecute)
            .Build();

        var context = new TestContext { ShouldExecute = false };
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().NotContain(nameof(TestHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddConditionalHandler throws ArgumentNullException when predicate is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConditionalHandler_should_throw_when_predicate_is_null()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var builder = new ChainBuilder<TestContext>(serviceProvider);

        var act = () => builder.AddConditionalHandler<TestHandlerA>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
