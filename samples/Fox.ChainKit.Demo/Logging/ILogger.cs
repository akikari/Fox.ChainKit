//==================================================================================================
// Simple logger interface for demo application.
// Abstracts output mechanism from business logic.
//==================================================================================================

namespace Fox.ChainKit.Demo.Logging;

//==================================================================================================
/// <summary>
/// Simple logger interface for the demo application.
/// </summary>
//==================================================================================================
internal interface ILogger
{
    //==============================================================================================
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    //==============================================================================================
    void LogInformation(string message);

    //==============================================================================================
    /// <summary>
    /// Logs a success message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    //==============================================================================================
    void LogSuccess(string message);

    //==============================================================================================
    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    //==============================================================================================
    void LogError(string message);
}
