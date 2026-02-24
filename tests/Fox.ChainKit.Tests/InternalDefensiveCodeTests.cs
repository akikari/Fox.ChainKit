//==================================================================================================
// Tests for defensive code in internal types.
// Verifies null parameter checks in internal constructors and methods.
//==================================================================================================
using FluentAssertions;
using Fox.ChainKit.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Tests for defensive code in internal types.
/// </summary>
//==================================================================================================
public sealed class InternalDefensiveCodeTests
{
    //==============================================================================================
    /// <summary>
    /// Tests that Chain constructor throws for null serviceProvider.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Chain_constructor_should_throw_for_null_service_provider()
    {
        var act = () => new Chain<TestContext>(null!, [], null, null);

        act.Should().Throw<ArgumentNullException>().WithParameterName("serviceProvider");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that Chain constructor throws for null handlers.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void Chain_constructor_should_throw_for_null_handlers()
    {
        var services = new ServiceCollection().BuildServiceProvider();

        var act = () => new Chain<TestContext>(services, null!, null, null);

        act.Should().Throw<ArgumentNullException>().WithParameterName("handlers");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that ChainBuilder.AddHandlerDescriptor throws for null descriptor.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ChainBuilder_AddHandlerDescriptor_should_throw_for_null_descriptor()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var builder = new ChainBuilder<TestContext>(services);

        var act = () => builder.AddHandlerDescriptor(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("descriptor");
    }

    //==============================================================================================
    /// <summary>
    /// Tests that ChainBuilder.AddHandlerDescriptor adds descriptor successfully.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ChainBuilder_AddHandlerDescriptor_should_add_descriptor_successfully()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestHandlerA>();
        var serviceProvider = services.BuildServiceProvider();
        var builder = new ChainBuilder<TestContext>(serviceProvider);

        var descriptor = new HandlerDescriptor<TestContext>
        {
            Factory = sp => sp.GetRequiredService<TestHandlerA>(),
            HandlerType = typeof(TestHandlerA)
        };

        builder.AddHandlerDescriptor(descriptor);

        var chain = builder.Build();
        chain.Should().NotBeNull();
    }

    //==============================================================================================
    /// <summary>
    /// Tests that ChainBuilder constructor throws for null serviceProvider.
    /// </summary>
    //==============================================================================================
    [Fact]
    public void ChainBuilder_constructor_should_throw_for_null_service_provider()
    {
        var act = () => new ChainBuilder<TestContext>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("serviceProvider");
    }
}
