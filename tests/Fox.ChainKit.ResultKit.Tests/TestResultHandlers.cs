//==================================================================================================
// Test Result handlers for ResultKit chain tests.
// Various Result handler implementations to test different behaviors.
//==================================================================================================
using Fox.ResultKit;

namespace Fox.ChainKit.ResultKit.Tests;

//==================================================================================================
/// <summary>
/// Simple test Result handler that adds its name to the context and succeeds.
/// </summary>
//==================================================================================================
internal sealed class TestResultHandlerA : IResultHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<Result> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestResultHandlerA));
        return Task.FromResult(Result.Success());
    }
}

//==================================================================================================
/// <summary>
/// Simple test Result handler that adds its name to the context and succeeds.
/// </summary>
//==================================================================================================
internal sealed class TestResultHandlerB : IResultHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<Result> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestResultHandlerB));
        return Task.FromResult(Result.Success());
    }
}

//==================================================================================================
/// <summary>
/// Test Result handler that returns failure based on context configuration.
/// </summary>
//==================================================================================================
internal sealed class TestResultHandlerFailure : IResultHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<Result> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestResultHandlerFailure));

        if (context.ShouldFail)
        {
            return Task.FromResult(Result.Failure(context.ErrorMessage ?? "Test failure"));
        }

        return Task.FromResult(Result.Success());
    }
}

//==================================================================================================
/// <summary>
/// Simple test Result handler C.
/// </summary>
//==================================================================================================
internal sealed class TestResultHandlerC : IResultHandler<TestContext>
{
    //==============================================================================================
    /// <inheritdoc />
    //==============================================================================================
    public Task<Result> HandleAsync(TestContext context, CancellationToken cancellationToken = default)
    {
        context.ExecutedHandlers.Add(nameof(TestResultHandlerC));
        return Task.FromResult(Result.Success());
    }
}
