using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoipBackend.Interfaces;
using RoipBackend.Models;

namespace RoipBackend.Services
{

    public class LoggerService : ILoggerService
    {
        private readonly AppDbContext _DBcontext;                

        public LoggerService(AppDbContext context)
        {
            _DBcontext = context;
            _DBcontext.Database.SetCommandTimeout(C.DB_REQUEST_TIMEOUT);
        }

        public async Task LogInfoAsync(string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_INFO_STR;
            try
            {
                var result = await _DBcontext.Logger.AddAsync(new Logger
                {
                    Timestamp = timeNow,
                    LogLevel = logLevel,
                    FriendlyDescribtion = friendlyDescribtion,
                    Exception = string.Empty,
                    UserId = string.Empty
                });
                await _DBcontext.SaveChangesAsync();

                if (result == null)
                {
                    throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                }
            }
            catch (InvalidOperationException e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }       
        }

        public async Task LogWarningAsync(string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_WARNING_STR;
            try
            {
                var result = await _DBcontext.Logger.AddAsync(new Logger
                {
                    Timestamp = timeNow,
                    LogLevel = logLevel,
                    FriendlyDescribtion = friendlyDescribtion,
                    Exception = string.Empty,
                });
                await _DBcontext.SaveChangesAsync();
                if (result == null)
                {
                    throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                }
            }
            catch (InvalidOperationException e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }         
        }

        public async Task LogTraceAsync(string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_TRACE_STR;
            try
            {
                if (Environment.GetEnvironmentVariable(C.ASPNETCORE_ENV_NAME_STR) == C.PRODUCTION_ENV_STR)
                {
                    var result = await _DBcontext.Logger.AddAsync(new Logger
                    {
                        Timestamp = timeNow,
                        LogLevel = logLevel,
                        FriendlyDescribtion = friendlyDescribtion,
                        Exception = string.Empty,
                    });
                    await _DBcontext.SaveChangesAsync();
                    if (result == null)
                    {
                        throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }       
        }

        public async Task LogDebugAsync(string exception, string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_DEBUG_STR;
            try
            {
                if (Environment.GetEnvironmentVariable(C.ASPNETCORE_ENV_NAME_STR) == C.PRODUCTION_ENV_STR)
                {
                    var result = await _DBcontext.Logger.AddAsync(new Logger
                    {
                        Timestamp = timeNow,
                        LogLevel = logLevel,
                        FriendlyDescribtion = friendlyDescribtion,
                        Exception = exception,
                    });
                    await _DBcontext.SaveChangesAsync();
                    if (result == null)
                    {
                        throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }     
        }

        public async Task LogErrorAsync(string exception, string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_ERROR_STR;
            try
            {
                var result = await _DBcontext.Logger.AddAsync(new Logger
                {
                    Timestamp = timeNow,
                    LogLevel = logLevel,
                    FriendlyDescribtion = friendlyDescribtion,
                    Exception = exception,
                });
                await _DBcontext.SaveChangesAsync();
                if (result == null)
                {
                    throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                }
            }
            catch (InvalidOperationException e)
            {
                await _DBcontext.SaveChangesAsync();
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }
            catch (Exception e)
            {
                await _DBcontext.SaveChangesAsync();
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }         
        }

        public async Task LogErrorAsync(string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_ERROR_STR;
            try
            {
                var result = await _DBcontext.Logger.AddAsync(new Logger
                {
                    Timestamp = timeNow,
                    LogLevel = logLevel,
                    FriendlyDescribtion = friendlyDescribtion,
                });
                await _DBcontext.SaveChangesAsync();
                if (result == null)
                {
                    throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                }
            }
            catch (InvalidOperationException e)
            {

                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, string.Empty, e.Message);
            }       
        }

        public async Task LogFatalAsync(string exception, string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_FATAL_STR;
            try
            {
                var result = await _DBcontext.Logger.AddAsync(new Logger
                {
                    Timestamp = timeNow,
                    LogLevel = logLevel,
                    FriendlyDescribtion = friendlyDescribtion,
                    Exception = exception,
                });
                await _DBcontext.SaveChangesAsync();
                if (result == null)
                {
                    throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                }
            }
            catch (InvalidOperationException e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }        
        }

        public async Task LogCriticalAsync(string exception, string friendlyDescribtion)
        {
            DateTime timeNow = DateTime.UtcNow;
            string logLevel = C.LOG_CRITICAL_STR;
            try
            {
                var result = await _DBcontext.Logger.AddAsync(new Logger
                {
                    Timestamp = timeNow,
                    LogLevel = logLevel,
                    FriendlyDescribtion = friendlyDescribtion,
                    Exception = exception,
                });
                await _DBcontext.SaveChangesAsync();
                if (result == null)
                {
                    throw new InvalidOperationException(C.FAILED_ADDING_DATABASE_LOG_STR);
                }
            }
            catch (InvalidOperationException e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }
            catch (Exception e)
            {
                await RetryLoggingAsync(timeNow, logLevel, friendlyDescribtion, exception, e.Message);
            }        
        }

        private async Task RetryLoggingAsync(DateTime dateTime, string logLevel, string friendlyDescribtion,
            string? exception, string loggerServiceException)
        {           
            int retryCount = 1;
            while (retryCount < 3)
            {
                try
                {
                    await _DBcontext.Logger.AddAsync(new Logger
                    {
                        Timestamp = DateTime.UtcNow,
                        LogLevel = logLevel,
                        FriendlyDescribtion = friendlyDescribtion,
                        Exception = exception,
                    });
                    await _DBcontext.SaveChangesAsync();
                    break;
                }
                catch
                {
                    retryCount++;
                    await Task.Delay(1000 * retryCount); // Exponential backoff
                }          
            }
            if (retryCount == 3)
            {                 
                // Log to a file or send an alert
                string logFilePath = C.CASHED_RETRY_LOGS_FILE_LOCATION_STR;
                string logEntry = $"{dateTime}: [{logLevel}]. {C.MESSAGE_STR}: {friendlyDescribtion}. " +
                    $"{C.EXCEPTION_STR}: {exception} " +
                    $"{C.ADDITIONAL_EXCEPTION_MESSAGE_LOG_STR}: {loggerServiceException}";
                await File.AppendAllTextAsync(logFilePath, logEntry + Environment.NewLine);
            }
        }
        public async Task StartFallbackLogMonitorAsync(CancellationToken cancellationToken)
        {
            string logFilePath = C.CASHED_RETRY_LOGS_FILE_LOCATION_STR;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (File.Exists(logFilePath))
                    {
                        var logEntries = await File.ReadAllLinesAsync(logFilePath, cancellationToken);

                        if (logEntries.Length > 0)
                        {
                            foreach (var logEntry in logEntries)
                            {
                                var logParts = logEntry.Split(new[] { ": [" }, StringSplitOptions.None);
                                if (logParts.Length >= 2)
                                {
                                    var timestamp = DateTime.Parse(logParts[0]);
                                    var logLevelAndMessage = logParts[1].Split(new[] { "]. Message: " }, StringSplitOptions.None);
                                    if (logLevelAndMessage.Length >= 2)
                                    {
                                        var logLevel = logLevelAndMessage[0];
                                        var messageParts = logLevelAndMessage[1].Split(new[] { ". Exception: " }, StringSplitOptions.None);
                                        var friendlyDescription = messageParts[0];
                                        var exception = messageParts.Length > 1 ? messageParts[1].Split(new[] { " Additional Exception message from LoggerService: " }, StringSplitOptions.None)[0] : string.Empty;

                                        await _DBcontext.Logger.AddAsync(new Logger
                                        {
                                            Timestamp = timestamp,
                                            LogLevel = logLevel,
                                            FriendlyDescribtion = friendlyDescription,
                                            Exception = exception,
                                            UserId = string.Empty // Default to empty if UserId is not present
                                        });
                                    }
                                }
                            }

                            await _DBcontext.SaveChangesAsync();
                            File.WriteAllText(logFilePath, string.Empty); // Clear the file after processing
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the monitoring process
                    Console.WriteLine($"{C.ERROR_PROCESSING_FALLBACK_LOG_STR}: {ex.Message}");
                }

                await Task.Delay(C.ADDITIONAL_FILE_LOGS_INTERVAL, cancellationToken); // Check every 30 minutes
            }
        }
    }
}
