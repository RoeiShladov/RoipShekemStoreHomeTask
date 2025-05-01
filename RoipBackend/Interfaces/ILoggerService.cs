namespace RoipBackend.Interfaces
{
    public interface ILoggerService
    {
        Task LogInfoAsync(string message, string details);
        Task LogWarningAsync(string message, string details);
        Task LogDebugAsync(Exception message, string details);

        Task LogErrorAsync(Exception exception, string friendlyMessage);
        Task LogFatalAsync(Exception exception, string friendlyMessage);

    }
}
