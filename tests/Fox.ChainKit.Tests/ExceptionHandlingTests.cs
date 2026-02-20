//==================================================================================================
// Tests for exception handling in chains.
// Verifies exception handling behavior and diagnostics.
//==================================================================================================
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for exception handling functionality.
/// </summary>
//==================================================================================================
public sealed class ExceptionHandlingTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that exceptions are caught and handled by custom exception handler.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task UseExceptionHandler_should_catch_and_handle_exceptions()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        services.AddTransient<TestHandlerException>();
        services.AddTransient<TestHandlerB>();
        var serviceProvider = services.BuildServiceProvider();

        Exception? caughtException = null;
        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerA>()
            .AddHandler<TestHandlerException>()
            .AddHandler<TestHandlerB>()
            .UseExceptionHandler((ex, ctx) =>
            {
                caughtException = ex;
                return Task.CompletedTask;
            })
            .Build();

        var context = new TestContext { ShouldThrow = true };
        await chain.RunAsync(context);

        caughtException.Should().NotBeNull();
        caughtException.Should().BeOfType<InvalidOperationException>();
        context.ExecutedHandlers.Should().Contain(nameof(TestHandlerB));
    }

    //==============================================================================================
    /// <summary>
    /// Tests that exceptions are re-thrown when no exception handler is configured.
    /// </summary>
    //==============================================================================================
    [Fact]
    public async Task RunAsync_should_rethrow_exception_when_no_handler_configured()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerException>();
        var serviceProvider = services.BuildServiceProvider();

        var chain = new ChainBuilder<TestContext>(serviceProvider)
            .AddHandler<TestHandlerException>()
            .Build();

        var context = new TestContext { ShouldThrow = true };
        var act = async () => await chain.RunAsync(context);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that UseExceptionHandler throws ArgumentNullException when handler is null.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void UseExceptionHandler_should_throw_when_handler_is_null()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var builder = new ChainBuilder<TestContext>(serviceProvider);

        var act = () => builder.UseExceptionHandler(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
