namespace iplust
{
    public static class Utils
    {
        public static T? TryRun<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return default;
            }
        }

        public static T TryRunAndExitIfError<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Logger.ExceptionAndExit(ex);
                return default!;
            }
        }

        public static T? RunAndIgnoreException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static string FilterFileName(string name)
        {
            var chars = Path.GetInvalidFileNameChars();
            Span<char> buffer = stackalloc char[name.Length];
            var realLength = 0;
            foreach (var @char in name)
            {
                if (chars.Contains(@char))
                    continue;
                buffer[realLength++] = @char;
            }
            return new string(buffer.Slice(0, realLength));
        }
    }
}
