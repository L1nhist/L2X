using Microsoft.Extensions.Logging;

namespace L2X.Data.Extensions
{
    public static class LoggingExtension
    {
        private static readonly EventId Action = new(1000, "Service");
        private static readonly EventId Database = new(3000, "Database");
        private static readonly EventId System = new(8000, "System");

        public static void ActionLog(this ILogger logger, string message)
            => logger.LogWarning(Action, message);

        public static void ActionLog(this ILogger logger, Exception exxeption)
            => logger.LogCritical(Action, exxeption, "");

        public static void DatabaseLog(this ILogger logger, string message)
            => logger.LogWarning(Database, message);

        public static void DatabaseLog(this ILogger logger, Exception exxeption)
            => logger.LogCritical(Database, exxeption, "");

        public static void SystemLog(this ILogger logger, string message)
            => logger.LogWarning(System, message);

        public static void SystemLog(this ILogger logger, Exception exxeption)
            => logger.LogCritical(System, exxeption, "");
    }
}