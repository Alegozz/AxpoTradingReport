namespace TradingReport
{
    internal static class Logger
    {
        private static readonly string LogPath;

        static Logger()
        {
            //Logs will be generated daily, hence the file name
            LogPath = Directory.GetCurrentDirectory() + "\\output\\log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        }

        public static void Log(string message) 
        {
            try
            {
                //Create directory for output
                FileManager.CreateOutputDirectoryLocal();

                using (FileStream fileStream = new FileStream(LogPath, FileMode.Append, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine($"{DateTime.Now} - {message}");
                    }
                }
            } catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void Log(Exception ex)
        {
            using (FileStream fileStream = new FileStream(LogPath, FileMode.Append, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine($"{DateTime.Now} - {ex.Message}");
                    streamWriter.WriteLine($"{ex.InnerException}");
                }
            }
        }
    }
}
