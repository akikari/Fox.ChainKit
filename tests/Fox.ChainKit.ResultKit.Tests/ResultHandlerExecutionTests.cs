//==================================================================================================
// Tests for Result handler execution in chains.
// Verifies Result-based handler execution and automatic failure handling.
//==================================================================================================
using FluentAssertions;
using Fox.ChainKit.ResultKit.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.ResultKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for Result handler execution functionality.
/// </summary>
//==================================================================================================
public sealed class ResultHandlerExecutionTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that Result handlers execute successfully and continue the chain.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddResultHandler_should_execute_and_continue_on_success()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerA>();
        services.AddTransient<TestResultHandlerB>();
        services.AddTransient<TestResultHandlerC>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerA>()
            .AddResultHandler<TestContext, TestResultHandlerB>()
            .AddResultHandler<TestContext, TestResultHandlerC>()
            .Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().ContainInOrder(
            nameof(TestResultHandlerA),
            nameof(TestResultHandlerB),
            nameof(TestResultHandlerC));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that Result handler stops the chain on failure.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddResultHandler_should_stop_chain_on_failure()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerA>();
        services.AddTransient<TestResultHandlerFailure>();
        services.AddTransient<TestResultHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerA>()
            .AddResultHandler<TestContext, TestResultHandlerFailure>()
            .AddResultHandler<TestContext, TestResultHandlerB>()
            .Build();

        var context = new TestContext
        {
            ShouldFail = true,
            ErrorMessage = "Validation failed"
        };

        await chain.RunAsync(context);

        context.ExecutedHandlers.Should().ContainInOrder(
            nameof(TestResultHandlerA),
            nameof(TestResultHandlerFailure));

        context.ExecutedHandlers.Should().NotContain(nameof(TestResultHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that Result handler with callback receives the result.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task AddResultHandler_with_callback_should_receive_result()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerFailure>();
        var serviceProvider = services.BuildServiceProvider();

        Fox.ResultKit.Result? capturedResult = null;

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerFailure>(result => capturedResult = result)
            .Build();

        var context = new TestContext
        {
            ShouldFail = true,
            ErrorMessage = "Test message"
        };

        await chain.RunAsync(context);

        capturedResult.Should().NotBeNull();
        capturedResult!.IsSuccess.Should().BeFalse();
        capturedResult.Error.Should().Be("Test message");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddResultHandler throws ArgumentNullException when builder is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddResultHandler_should_throw_when_builder_is_null()
    {
        IChainBuilder<TestContext> builder = null!;

        var act = () => builder.AddResultHandler<TestContext, TestResultHandlerA>();

        act.Should().Throw<ArgumentNullException>();
    }
}
