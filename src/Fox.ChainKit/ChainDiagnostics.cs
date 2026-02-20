//==================================================================================================
// Provides diagnostic information about chain execution.
// Includes handler execution times, results, and early exit reasons.
//==================================================================================================

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Contains diagnostic information about a chain execution.
/// </summary>
//==================================================================================================
public sealed class ChainDiagnostics
{
    //==============================================================================================
    /// <summary>
    /// Gets the list of handler execution results.
    /// </summary>
    //==============================================================================================
    public List<HandlerDiagnostics> Handlers { get; } = [];

    //==============================================================================================
    /// <summary>
    /// Gets the total execution time for the entire chain.
    /// </summary>
    //==============================================================================================
    public TimeSpan TotalExecutionTime { get; internal set; }

    //==============================================================================================
    /// <summary>
    /// Gets a value indicating whether the chain was stopped early.
    /// </summary>
    //==============================================================================================
    public bool StoppedEarly { get; internal set; }

    //==============================================================================================
    /// <summary>
    /// Gets the reason why the chain was stopped early, if applicable.
    /// </summary>
    //==============================================================================================
    public string? EarlyStopReason { get; internal set; }
}

