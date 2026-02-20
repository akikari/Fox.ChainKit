//==================================================================================================
// Tests for ResultKit diagnostics extensions.
// Verifies Result-specific diagnostic functionality.
//==================================================================================================
using FluentAssertions;
using Fox.ChainKit.ResultKit.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.ResultKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for Result-aware diagnostics functionality.
/// </summary>
//==================================================================================================
public sealed class ResultDiagnosticsTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that diagnostics captures Result handler execution.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseDiagnostics_should_capture_result_handler_execution()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerA>();
        services.AddTransient<TestResultHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerA>()
            .AddResultHandler<TestContext, TestResultHandlerB>()
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();
        capturedDiagnostics!.Handlers.Should().HaveCount(2);
        capturedDiagnostics.Handlers[0].HandlerName.Should().Be(nameof(TestResultHandlerA));
        capturedDiagnostics.Handlers[1].HandlerName.Should().Be(nameof(TestResultHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that diagnostics captures Result handler failure.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseDiagnostics_should_capture_result_handler_failure()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerFailure>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerFailure>()
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext { ShouldFail = true };
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();
        capturedDiagnostics!.StoppedEarly.Should().BeTrue();
        capturedDiagnostics.Handlers[0].Result.Should().Be(HandlerResult.Stop);
    }

    //==============================================================================================
    /// <summary>
    /// Tests that FormatResultDiagnostics returns correct summary.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task FormatResultDiagnostics_should_return_correct_summary()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestResultHandlerA>();
        services.AddTransient<TestResultHandlerFailure>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddResultHandler<TestContext, TestResultHandlerA>()
            .AddResultHandler<TestContext, TestResultHandlerFailure>()
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext { ShouldFail = true };
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();

        var summary = capturedDiagnostics!.FormatResultDiagnostics();

        summary.Should().Contain("Result handlers:");
    }
}
