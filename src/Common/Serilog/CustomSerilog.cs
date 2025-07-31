using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Common.Serilog
{
    public static class CustomSerilog
    {
        public static void Initialize(IConfiguration config)
        {
        }

        public static void LogMessage(LogEventLevel level, string message)
        {
            switch (level)
            {
                case LogEventLevel.Verbose:
                    Log.Verbose(message);
                    break;

                case LogEventLevel.Debug:
                    Log.Debug(message);
                    break;

                case LogEventLevel.Information:
                    Log.Information(message);
                    break;

                case LogEventLevel.Warning:
                    Log.Warning(message);
                    break;

                case LogEventLevel.Error:
                    Log.Error(message);
                    break;

                case LogEventLevel.Fatal:
                    Log.Fatal(message);
                    break;

                default:
                    Log.Error(message);
                    break;
            }
        }
    }
}