// Copyright (c) 2024 citrus - https://unrealist.org
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using EpicGames.UHT.Utils;

namespace Citrus.Plugins.SpecifierReferenceViewer.ReferenceGenerator;

/// <summary>
/// Replaces the original logging methods for <see cref="UhtSession"/> with new methods that automatically include the
/// file path and line number.
/// </summary>
internal static class LogExtensions
{
    /// <summary>
    /// Logs an informational message with the caller's file path and line number.
    /// </summary>
    public static void LogInfo(this UhtSession session, string message,
        [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Log(session, UhtMessageType.Info, message, filePath, lineNumber);
    }

    /// <summary>
    /// Logs a warning with the caller's file path and line number.
    /// </summary>
    public static void LogWarning(this UhtSession session, string message,
        [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Log(session, UhtMessageType.Warning, message, filePath, lineNumber);
    }

    /// <summary>
    /// Logs an error with the caller's file path and line number.
    /// </summary>
    public static void LogError(this UhtSession session, string message,
        [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Log(session, UhtMessageType.Error, message, filePath, lineNumber);
    }

    private static void Log(UhtSession session, UhtMessageType messageType, string message,
        string filePath, int lineNumber)
    {
        session.AddMessage(
            new UhtMessage
            {
                MessageType = messageType,
                Message = message,
                FilePath = filePath,
                LineNumber = lineNumber
            });
    }
}
