using Spectre.Console;
using System;

namespace Xlf.Commands;

internal static class CliHelper
{
    public static void DisplayException(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        AnsiConsole.MarkupLineInterpolated($"[red]{ex.Message}[/]");
        Exception? inner = ex.InnerException;
        while (inner != null)
        {
            AnsiConsole.MarkupLineInterpolated($"- [red]{inner.Message}[/]");
            inner = inner.InnerException;
        }
        AnsiConsole.MarkupLineInterpolated($"[yellow]{ex.StackTrace}[/]");
    }
}
