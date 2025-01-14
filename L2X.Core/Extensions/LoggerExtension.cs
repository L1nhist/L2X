using Microsoft.Extensions.Logging;

namespace L2X.Core.Extensions;

public static class LoggerExtension
{
	public static ILogger WriteLog(this ILogger logger, string message)
	{
		logger.LogWarning("[{0:HH:mm:ss dd/MM/yyyy}] - {1}.", DateTime.Now, message);
		return logger;
	}

	public static ILogger WriteLog(this ILogger logger, Exception exception)
	{
		logger.LogError("[{0:HH:mm:ss dd/MM/yyyy}] - {1}.{2}{3}", DateTime.Now, exception.Message, Environment.NewLine, exception.StackTrace);
		return logger;
	}
}