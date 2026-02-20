//==================================================================================================
// Test context for chain tests.
// Simple context object to pass through the handler chain.
//==================================================================================================

namespace Fox.ChainKit.Tests;

//==================================================================================================
/// <summary>
/// Simple test context for testing chains.
/// </summary>
//==================================================================================================
public sealed class TestContext
{
    //==============================================================================================
    /// <summary>
    /// Gets or sets the list of executed handler names.
    /// </summary>
    //==============================================================================================
    public List<string> ExecutedHandlers { get; set; } = [];

    //==============================================================================================
    /// <summary>
    /// Gets or sets a value used for conditional handler tests.
    /// </summary>
    //==============================================================================================
    public bool ShouldExecute { get; set; } = true;

    //==============================================================================================
    /// <summary>
    /// Gets or sets a value indicating whether a handler should throw an exception.
    /// </summary>
    //==============================================================================================
    public bool ShouldThrow { get; set; }
}
