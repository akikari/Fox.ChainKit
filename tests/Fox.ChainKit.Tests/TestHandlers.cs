//==================================================================================================
// Test handlers for chain tests.
// Various handler implementations to test different chain behaviors.
//==================================================================================================

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Simple test handler that adds its name to the context.
/// </summary>
//==================================================================================================
internal sealed class TestHandlerA : IHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestHandlerA));
        return Task.FromResult(HandlerResult.Continue);
    }
}

//==================================================================================================
/// <summary>
/// Simple test handler that adds its name to the context.
/// </summary>
//==================================================================================================
internal sealed class TestHandlerB : IHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestHandlerB));
        return Task.FromResult(HandlerResult.Continue);
    }
}

//==================================================================================================
/// <summary>
/// Test handler that returns Stop to test early exit.
/// </summary>
//==================================================================================================
internal sealed class TestHandlerStop : IHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestHandlerStop));
        return Task.FromResult(HandlerResult.Stop);
    }
}

//==================================================================================================
/// <summary>
/// Test handler that throws an exception when configured.
/// </summary>
//==================================================================================================
internal sealed class TestHandlerException : IHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestHandlerException));

        if (context.ShouldThrow)
        {
            throw new InvalidOperationException("Test exception");
        }

        return Task.FromResult(HandlerResult.Continue);
    }
}

//==================================================================================================
/// <summary>
/// Simple test handler C.
/// </summary>
//==================================================================================================
internal sealed class TestHandlerC : IHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<HandlerResult> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestHandlerC));
        return Task.FromResult(HandlerResult.Continue);
    }
}
