using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Game.Native;
using Game.Utilities;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

namespace Game;

internal static class Entry
{
    [UnmanagedCallersOnly]
    private static unsafe void Init(InitParams* init_params, InitResult* init_result)
    {
        InitLogger();

        AppDomain.CurrentDomain.UnhandledException += static (_, e) =>
        {
            if (e.ExceptionObject is Exception exception)
            {
                Log.Error(exception, "Unhandled Exception");
            }
            else
            {
                Log.Error("{Error}", e.ExceptionObject);
            }
        };

        App.s_appVars = init_params->p_vas;
        App.s_native_app = init_params->p_native_app;
        
        RandomNumberGenerator.Fill(new Span<byte>(&App.s_appVars->a_hash_rand_0, sizeof(ulong) * 4));

        ref var fn_vtb = ref *init_result->fn_vtb;
        fn_vtb.main = &Main;
        
        fn_vtb.logger_cstr = &Logger;
        fn_vtb.logger_wstr = &Logger;
        fn_vtb.logger_str8 = &Logger;
        fn_vtb.logger_str16 = &Logger;

        Log.Debug("Init Dotnet");
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_minimumLevel")]
    private static extern ref LogEventLevel GetMinimumLevel(Logger logger);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int Main()
    {
        try
        {
            int r = 0;
            var version = new Option<bool>(["--version", "-v"], " Show version information");
            var debug = new Option<bool>(["--debug", "-D"], "Enable debug mode");
            var root = new RootCommand("A game of automation")
            {
                debug,
                version,
            };
            root.SetHandler(ctx =>
            {
                if (ctx.ParseResult.FindResultFor(version) is { })
                {
                    Console.WriteLine(Utils.GetAsmVer(typeof(Entry).Assembly));
                    return;
                }
                if (ctx.ParseResult.FindResultFor(debug) is { })
                {
                    App.Vars.debug = true;
                    GetMinimumLevel((Logger)Log.Logger) = LogEventLevel.Debug;
                    Log.Warning("Debug mode enabled");
                }
                r = App.Main();
            });
            var parser = new CommandLineBuilder(root)
                .UseHelp()
                .UseEnvironmentVariableDirective()
                .UseParseDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                .UseExceptionHandler()
                .CancelOnProcessTermination()
                .Build();
            parser.Invoke(Environment.GetCommandLineArgs().Skip(1).ToArray());
            return r;
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled Exception");
            return -1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void InitLogger()
    {
        if (File.Exists("./logs/latest.log"))
        {
            try
            {
                var time = File.GetCreationTime("./logs/latest.log");
                var time_name = $"{time:yyyy-MM-dd}";
                var max_count = Directory.GetFiles("./logs/")
                    .Where(static n => Path.GetExtension(n) == ".log")
                    .Select(static n => Path.GetFileName(n))
                    .Where(n => n.StartsWith(time_name))
                    .Select(n => n.Substring(time_name.Length))
                    .Select(static n => (n, i: n.IndexOf('.')))
                    .Where(static a => a.i > 1)
                    .Select(static a => (s: uint.TryParse(a.n.Substring(1, a.i - 1), out var n), n))
                    .Where(static a => a.s)
                    .OrderByDescending(static a => a.n)
                    .Select(static a => a.n)
                    .FirstOrDefault();
                var count = max_count + 1;
                File.Move("./logs/latest.log", $"./logs/{time_name}_{count}.log");
            }
            catch (Exception e)
            {
                Log.Error(e, "");
            }
        }
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Async(c => c.File("./logs/latest.log"))
            .CreateLogger();
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void Logger(FLogLevel level, byte* msg)
    {
        var str = Marshal.PtrToStringUTF8((IntPtr)msg);
        Log.Write(level.ToLogEventLevel(), "[Native] {Message}", str);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void Logger(FLogLevel level, char* msg)
    {
        var str = new string(msg);
        Log.Write(level.ToLogEventLevel(), "[Native] {Message}", str);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Logger(FLogLevel level, FrStr8 msg)
    {
        var str = msg.ToString();
        Log.Write(level.ToLogEventLevel(), "[Native] {Message}", str);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Logger(FLogLevel level, FrStr16 msg)
    {
        var str = msg.ToString();
        Log.Write(level.ToLogEventLevel(), "[Native] {Message}", str);
    }
}
