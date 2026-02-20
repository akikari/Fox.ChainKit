//==================================================================================================
// Extension methods for ResultKit-aware diagnostics.
// Provides diagnostic information specific to Result-based handlers.
//==================================================================================================

namespace Fox.ChainKit.ResultKit.Extensions;

//==================================================================================================
/// <summary>
/// Extension methods for Result-aware diagnostics.
/// </summary>
//==================================================================================================
public static class DiagnosticsExtensions
{
    //==============================================================================================
    /// <summary>
    /// Creates a formatted diagnostic message for chain execution.
    /// </summary>
    /// <param name="diagnostics">The chain diagnostics.</param>
    /// <returns>A formatted string with diagnostic information.</returns>
    //==============================================================================================
    public static string FormatResultDiagnostics(this ChainDiagnostics diagnostics)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        var totalHandlers = diagnostics.Handlers.Count;

        if (totalHandlers == 0)
        {
            return "No handlers in chain.";
        }

        var failedHandlers = diagnostics.Handlers.Where(h => h.Result == HandlerResult.Stop && !h.HasException).ToList();
        var skippedHandlers = diagnostics.Handlers.Where(h => h.Skipped).ToList();

        return $"Result handlers: {totalHandlers}, Failed: {failedHandlers.Count}, Skipped: {skippedHandlers.Count}";
    }
}
