﻿using Serilog;
using Spectre.Console.Cli;
using Spectre.Console;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;
using Xlf.Commands;

namespace Xlf;

public static class Program
{
#if DEBUG
    private static void DeleteLogs()
    {
        foreach (var path in Directory.EnumerateFiles(
            AppDomain.CurrentDomain.BaseDirectory, "xliff-log*.txt"))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
#endif

    public static async Task<int> Main(string[] args)
    {
        try
        {
            // https://github.com/serilog/serilog-sinks-file
            string logFilePath = Path.Combine(
                Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) ?? "",
                    "xliff-log.txt");
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
#if DEBUG
            DeleteLogs();
#endif
            Stopwatch stopwatch = new();
            stopwatch.Start();

            CommandApp app = new();
            app.Configure(config =>
            {
                config.AddCommand<FilterUnitsCommand>("filter")
                    .WithDescription(".");
            });

            int result = await app.RunAsync(args);

            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                Console.WriteLine("\nTime: {0}h{1}'{2}\"",
                    stopwatch.Elapsed.Hours,
                    stopwatch.Elapsed.Minutes,
                    stopwatch.Elapsed.Seconds);
            }

            return result;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            Debug.WriteLine(ex.ToString());
            AnsiConsole.WriteException(ex);
            return 2;
        }
    }
}
