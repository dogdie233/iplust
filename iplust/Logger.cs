namespace iplust
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error
    }

    public static class Logger
    {
        public static bool debugEnabled = false;

        public static void Log(LogLevel logLevel, string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}][{logLevel}] {message}");
        }

        public static void Debug(string message)
        {
            if (debugEnabled)
                Log(LogLevel.Debug, message);
        }
        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Warn(string message) => Log(LogLevel.Warn, message);
        public static void Error(string message) => Log(LogLevel.Error, message);
        public static void Exception(Exception e)
        {
            Log(LogLevel.Error, e.ToString());
        }
        public static void ExceptionAndExit(Exception e)
        {
            Log(LogLevel.Error, e.ToString());
            Environment.Exit(1);
        }
    }
}
