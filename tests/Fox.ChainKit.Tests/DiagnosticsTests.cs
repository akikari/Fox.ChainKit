//==================================================================================================
// Tests for chain diagnostics functionality.
// Verifies diagnostic information collection and reporting.
//==================================================================================================
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for diagnostics functionality.
/// </summary>
//==================================================================================================
public sealed class DiagnosticsTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that diagnostics captures handler execution information.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseDiagnostics_should_capture_handler_execution_info()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;
        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddHandler<TestHandlerB>()
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();
        capturedDiagnostics!.Handlers.Should().HaveCount(2);
        capturedDiagnostics.Handlers[0].HandlerName.Should().Be(nameof(TestHandlerA));
        capturedDiagnostics.Handlers[1].HandlerName.Should().Be(nameof(TestHandlerB));
        capturedDiagnostics.TotalExecutionTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    //==============================================================================================
    /// <summary>
    /// Tests that diagnostics captures early stop information.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseDiagnostics_should_capture_early_stop()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerStop>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;
        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddHandler<TestHandlerStop>()
            .AddHandler<TestHandlerB>()
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext();
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();
        capturedDiagnostics!.StoppedEarly.Should().BeTrue();
        capturedDiagnostics.EarlyStopReason.Should().Contain(nameof(TestHandlerStop));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that diagnostics captures skipped handler information.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseDiagnostics_should_capture_skipped_handlers()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;
        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddConditionalHandler<TestHandlerB>(ctx => ctx.ShouldExecute)
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext { ShouldExecute = false };
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();
        capturedDiagnostics!.Handlers.Should().HaveCount(2);
        capturedDiagnostics.Handlers[1].Skipped.Should().BeTrue();
    }

    //==============================================================================================
    /// <summary>
    /// Tests that diagnostics captures exception information.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseDiagnostics_should_capture_exception_info()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerException>();
        var serviceProvider = services.BuildServiceProvider();

        ChainDiagnostics? capturedDiagnostics = null;
        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerException>()
            .UseExceptionHandler((ex, ctx) => Task.CompletedTask)
            .UseDiagnostics(d => capturedDiagnostics = d)
            .Build();

        var context = new TestContext { ShouldThrow = true };
        await chain.RunAsync(context);

        capturedDiagnostics.Should().NotBeNull();
        capturedDiagnostics!.Handlers.Should().HaveCount(1);
        capturedDiagnostics.Handlers[0].HasException.Should().BeTrue();
        capturedDiagnostics.Handlers[0].ExceptionMessage.Should().Be("Test exception");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that UseDiagnostics throws ArgumentNullException when handler is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void UseDiagnostics_should_throw_when_handler_is_null()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var builder = new ChainBuilder<TestContext>(serviceProvider);

        var act = () => builder.UseDiagnostics(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
