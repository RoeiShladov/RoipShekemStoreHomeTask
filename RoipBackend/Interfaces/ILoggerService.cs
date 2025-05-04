namespace RoipBackend.Interfaces
{
    public interface ILoggerService
    {
        Task LogInfoAsync(string friendlyDescribtion);

        Task LogWarningAsync(string friendlyDescribtion);

        //Only in production
        Task LogTraceAsync(string friendlyDescribtion);

        //Only in production
        Task LogDebugAsync(string exception, string friendlyDescribtion);

        Task LogErrorAsync(string exception, string friendlyDescribtion);

        Task LogCriticalAsync(string exception, string friendlyDescribtion);

        Task LogFatalAsync(string exception, string friendlyDescribtion);




    }
}
