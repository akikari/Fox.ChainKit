//==================================================================================================
// Tests for defensive code in internal types.
// Verifies null parameter checks in internal constructors and invalid builder types.
//==================================================================================================
using FluentAssertions;
using Fox.ChainKit.ResultKit.Extensions;
using Fox.ChainKit.ResultKit.Internal;

namespace Fox.ChainKit.ResultKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for defensive code in internal types.
/// </summary>
//==================================================================================================
public sealed class InternalDefensiveCodeTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that ResultHandlerAdapter constructor throws for null resultHandler.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ResultHandlerAdapter_constructor_should_throw_for_null_handler()
    {
        var act = () => new ResultHandlerAdapter<TestContext>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("resultHandler");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddResultHandler throws when builder is not ChainBuilder.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddResultHandler_should_throw_for_invalid_builder_type()
    {
        var fakeBuilder = new FakeChainBuilder<TestContext>();

        var act = () => fakeBuilder.AddResultHandler<TestContext, TestResultHandlerA>();

        act.Should().Throw<InvalidOperationException>().WithMessage("Builder must be a ChainBuilder instance to add Result handlers.");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddResultHandler with callback throws when builder is not ChainBuilder.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddResultHandler_with_callback_should_throw_for_invalid_builder_type()
    {
        var fakeBuilder = new FakeChainBuilder<TestContext>();

        var act = () => fakeBuilder.AddResultHandler<TestContext, TestResultHandlerA>(r => { });

        act.Should().Throw<InvalidOperationException>().WithMessage("Builder must be a ChainBuilder instance to add Result handlers.");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that AddConditionalResultHandler throws when builder is not ChainBuilder.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void AddConditionalResultHandler_should_throw_for_invalid_builder_type()
    {
        var fakeBuilder = new FakeChainBuilder<TestContext>();

        var act = () => fakeBuilder.AddConditionalResultHandler<TestContext, TestResultHandlerA>(ctx => true);

        act.Should().Throw<InvalidOperationException>().WithMessage("Builder must be a ChainBuilder instance to add Result handlers.");
    }

    private sealed class FakeChainBuilder<TContext> : IChainBuilder<TContext>
    {
        public IChainBuilder<TContext> AddHandler<THandler>() where THandler : IHandler<TContext> => this;

        public IChainBuilder<TContext> AddConditionalHandler<THandler>(Func<TContext, bool> predicate) where THandler : IHandler<TContext> => this;

        public IChainBuilder<TContext> UseExceptionHandler(Func<Exception, TContext, Task> handler) => this;

        public IChainBuilder<TContext> UseDiagnostics(Action<ChainDiagnostics> handler) => this;

        public IChain<TContext> Build() => throw new NotImplementedException();
    }
}

