namespace QLog
{
    public class LogManager
    {
        private static ILogger _logger;

        public static ILogger GetLogger(Config config = null)
        {
            config = config ?? new Config();
            return _logger ?? (_logger = new QLogger(config));
        }
        public static bool StopLogger()
        {
            return true;
        }
    }
}
