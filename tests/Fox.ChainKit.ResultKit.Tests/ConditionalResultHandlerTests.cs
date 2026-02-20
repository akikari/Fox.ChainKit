//==================================================================================================
// Tests for conditional Result handler execution.
// Verifies conditional Result handlers execute only when conditions are met.
//==================================================================================================
using FluentAssertions;
using Fox.ChainKit.ResultKit.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.ResultKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for conditional Result handler functionality.
/// </summary>
//==================================================================================================
public sealed class ConditionalResultHandlerTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that conditional Result handler executes when condition is true.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddConditionalResultHandler_should_execute_when_condition_is_true()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerA>();
        services.AddTransient<TestResultHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerA>()
            .AddConditionalResultHandler<TestContext, TestResultHandlerB>(ctx => ctx.ShouldExecute)
            .Build();

        var context = new TestContext { ShouldExecute = true };
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().Contain(nameof(TestResultHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that conditional Result handler is skipped when condition is false.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddConditionalResultHandler_should_skip_when_condition_is_false()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerA>();
        services.AddTransient<TestResultHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerA>()
            .AddConditionalResultHandler<TestContext, TestResultHandlerB>(ctx => ctx.ShouldExecute)
            .Build();

        var context = new TestContext { ShouldExecute = false };
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().NotContain(nameof(TestResultHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddConditionalResultHandler throws ArgumentNullException when predicate is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConditionalResultHandler_should_throw_when_predicate_is_null()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var builder = new ChainBuilder<TestContext>(serviceProvider);

        var act = () => builder.AddConditionalResultHandler<TestContext, TestResultHandlerA>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
