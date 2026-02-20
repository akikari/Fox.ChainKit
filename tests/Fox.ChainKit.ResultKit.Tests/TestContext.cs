//==================================================================================================
// Test context for ResultKit chain tests.
// Simple context object for testing Result-based handlers.
//==================================================================================================

namespace Fox.ChainKit.ResultKit.Tests;

//==================================================================================================
/// <summary>
/// Simple test context for testing Result-based chains.
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
    /// Gets or sets a value indicating whether a handler should return failure.
    /// </summary>
    //==============================================================================================
    public bool ShouldFail { get; set; }

    //==============================================================================================
    /// <summary>
    /// Gets or sets the error code to use when failing.
    /// </summary>
    //==============================================================================================
    public string? ErrorCode { get; set; }

    //==============================================================================================
    /// <summary>
    /// Gets or sets the error message to use when failing.
    /// </summary>
    //==============================================================================================
    public string? ErrorMessage { get; set; }
}
