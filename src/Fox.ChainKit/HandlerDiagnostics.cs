//==================================================================================================
// Contains diagnostic information about a single handler execution.
// Records execution time, result, and exception information.
//==================================================================================================

namespace Fox.ChainKit;

//==================================================================================================
/// <summary>
/// Contains diagnostic information about a single handler execution.
/// </summary>
/// <param name="HandlerName">The name of the handler type.</param>
/// <param name="ExecutionTime">The execution time for this handler.</param>
/// <param name="Result">The result returned by the handler.</param>
/// <param name="Skipped">A value indicating whether this handler was skipped due to a condition.</param>
/// <param name="HasException">A value indicating whether an exception occurred during handler execution.</param>
/// <param name="ExceptionMessage">The exception message if an exception occurred.</param>
//==================================================================================================
public sealed record HandlerDiagnostics(string HandlerName = "", TimeSpan ExecutionTime = default, HandlerResult Result = HandlerResult.Continue, bool Skipped = false, bool HasException = false, string? ExceptionMessage = null);

