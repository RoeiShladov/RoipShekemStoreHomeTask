using Microsoft.EntityFrameworkCore;
using RoipBackend;
using RoipBackend.Interfaces;
using RoipBackend.Services;
using System;
using System.Data.Entity;
using System.Runtime.CompilerServices;

public class DatabaseLogCleanupService : IDisposable
{
    private readonly AppDbContext _DBcontext;
    private Timer _timer;

    public DatabaseLogCleanupService(IServiceProvider serviceProvider, ILogger<DatabaseLogCleanupService> logger, AppDbContext dbContext)
    {
        _DBcontext = dbContext;
    }

    public Task StartAsync()
    {
        _timer = new Timer(CleanDatabaseLogs, null, 0, (int)TimeSpan.FromDays(C.DATABASE_LOGS_CLEANUP_INTERVAL).TotalMilliseconds);
        return Task.CompletedTask;
    }

    private void CleanDatabaseLogs(object? state) // Updated parameter to allow nullability
    {
        Task.Run(async () =>
        {
            try
            {
                var twoWeeksAgo = DateTime.UtcNow.AddDays(C.NEGATIVE_LOGS_CLEANUP_INTERVAL);

                // Remove logs older than two weeks from the Logger table only
                _DBcontext.Logger.Where(log => log.Timestamp < twoWeeksAgo).ExecuteDelete();
                _DBcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                await WritExceptionToLogsFile(ex.Message, C.ERROR_CLEANING_DB_LOGS_STR, C.LOG_ERROR_STR);
            }
        });
    }

    public async Task StopAsync()
    {
        try
        {   // Dispose the timer to stop the periodic task
            _timer?.Dispose();
            // Log that the service has stopped
            await WritExceptionToLogsFile(C.DATABASE_LOG_CLEANUP_SERVICE_STOPPED_STR, C.DATABASE_LOG_CLEANUP_SERVICE_STOPPED_STR, C.LOG_CRITICAL_STR);
        }
        catch (Exception ex)
        {
            // Log any exception that occurs during the disposal process
            await WritExceptionToLogsFile(ex.Message, C.ERROR_STOPPING_TIMER_STR, C.LOG_ERROR_STR);
        }        

        await Task.CompletedTask; // Ensure the method completes as a Task
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private async Task WritExceptionToLogsFile(string exception, string message, string logLevel)
    {
        DateTime dateTime = DateTime.UtcNow;

        string logFilePath = C.CASHED_RETRY_LOGS_FILE_LOCATION_STR;
        string logEntry = $"{dateTime}: [{logLevel}]. {C.MESSAGE_STR}: {message}. " +
            $"{C.EXCEPTION_STR}: {exception} " +
            $"{C.ADDITIONAL_EXCEPTION_MESSAGE_LOG_STR}: {exception}";
        await File.AppendAllTextAsync(logFilePath, logEntry + Environment.NewLine);
    }
}
