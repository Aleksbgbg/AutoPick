namespace AutoPick.DebugTools
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class ErrorReporting
    {
        private const string InfoFile = "AutoPick.info.log";
        private const string DebugFile = "AutoPick.debug.log";

        [Conditional("DEBUG")]
        public static void Init()
        {
            File.Delete(InfoFile);
            File.Delete(DebugFile);
        }

        [Conditional("DEBUG")]
        public static void ReportInfo(string message, [CallerFilePath] string caller = null!,
                                      [CallerLineNumber] int lineNumber = 0)
        {
            Report(InfoFile, FormatReportLine(message, caller, lineNumber));
        }

        [Conditional("DEBUG")]
        public static void ReportError(Exception exception, string message, [CallerFilePath] string caller = null!,
                                       [CallerLineNumber] int lineNumber = 0)
        {
            StringBuilder errorText = new();
            errorText.AppendLine(FormatReportLine(message, caller, lineNumber));

            Exception? exceptionToLog = exception;

            while (exceptionToLog != null)
            {
                errorText.AppendLine(exception.Message);
                errorText.AppendLine(exception.StackTrace);

                exceptionToLog = exception.InnerException;
            }

            Report(DebugFile, errorText.ToString());
        }

        [Conditional("DEBUG")]
        public static void ReportError(string message, [CallerFilePath] string caller = null!,
                                       [CallerLineNumber] int lineNumber = 0)
        {
            Report(DebugFile, FormatReportLine(message, caller, lineNumber));
        }

        [Conditional("DEBUG")]
        private static void Report(string file, string text)
        {
            using StreamWriter streamWriter = File.AppendText(file);
            streamWriter.WriteLine(text);
        }

        private static string FormatReportLine(string message, string caller, int lineNumber)
        {
            return $"[{Path.GetFileName(caller)}:{lineNumber} @ {DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}] {message}";
        }
    }
}