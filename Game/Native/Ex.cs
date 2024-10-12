using System.Runtime.CompilerServices;
using Serilog.Events;

namespace Game.Native;

public static partial class FFI
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogEventLevel ToLogEventLevel(this FLogLevel level) => level switch
    {
        FLogLevel.Fatal => LogEventLevel.Fatal,
        FLogLevel.Error => LogEventLevel.Error,
        FLogLevel.Warn => LogEventLevel.Warning,
        FLogLevel.Info => LogEventLevel.Information,
        FLogLevel.Debug => LogEventLevel.Debug,
        FLogLevel.Trace => LogEventLevel.Verbose,
        _ => LogEventLevel.Information,
    };
}
