using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xlf.Core;

namespace Xlf.Commands;

internal sealed class FilterUnitsCommand : AsyncCommand<FilterUnitsCommandSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context,
        FilterUnitsCommandSettings settings)
    {
        try
        {
            AnsiConsole.MarkupLine("[green underline]FILTER UNITS[/]");
            AnsiConsole.MarkupLine($"Unit tag: [cyan]{settings.UnitTag}[/]");
            AnsiConsole.MarkupLine($"Input: [cyan]{settings.InputPath}[/]");
            AnsiConsole.MarkupLine($"Output: [cyan]{settings.OutputPath}[/]");
            if (settings.PreserveUntagged)
                AnsiConsole.MarkupLine("Preserve untagged: [cyan]yes[/]");

            // load input
            AnsiConsole.MarkupLine($"[green]Loading {settings.InputPath}[/]");
            XDocument doc = XDocument.Load(settings.InputPath,
                LoadOptions.PreserveWhitespace);

            // process document
            AnsiConsole.MarkupLine("[green]Processing...[/]");
            int removed = UnitFilterer.Filter(doc, settings.UnitTag,
                settings.PreserveUntagged);

            // save output
            AnsiConsole.MarkupLine($"[green]Saving {settings.OutputPath}[/]");
            doc.Save(settings.OutputPath, SaveOptions.None);

            AnsiConsole.MarkupLine($"[green]Completed: removed={removed}[/].");
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            CliHelper.DisplayException(ex);
            return Task.FromResult(2);
        }
    }
}

public class FilterUnitsCommandSettings : CommandSettings
{
    [CommandArgument(0, "<UnitTag>")]
    [Description("The unit tag to preserve while filtering all the others out.")]
    public string UnitTag { get; set; } = "";

    [CommandArgument(1, "<InputFilePath>")]
    [Description("The input XLIFF file path.")]
    public string InputPath { get; set; } = "";

    [CommandArgument(2, "<OutputFilePath>")]
    [Description("The output XLIFF file path.")]
    public string OutputPath { get; set; } = "";

    [CommandOption("-u|--untagged")]
    [Description("Preserve untagged units.")]
    public bool PreserveUntagged { get; set; }
}
