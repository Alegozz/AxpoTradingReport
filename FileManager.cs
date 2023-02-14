namespace TradingReport
{
    internal static class FileManager
    {
        public static void GenerateCSV(string path, string name, double[] totalPerPeriod)
        {
            try
            {
                string file = Path.Combine(path,name);
                CreateOutputDirectory(path);
                
                if (!File.Exists(file))
                {
                    using (FileStream fileStream = new FileStream(file, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(fileStream))
                        {
                            //Write the header
                            writer.WriteLine("Local Time, Volume");

                            //Write the information to file
                            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddHours(-1);
                            for (int i = 0; i < totalPerPeriod.Length; i++)
                            {
                                writer.WriteLine($"{time.AddHours(i).ToString("HH:mm")},{totalPerPeriod[i].ToString("0.0#")}");
                            }
                        }
                    }
                }
                else
                {
                    //If a file already exists with that name, the program won't overwrite it but create a new version of that file
                    int version = 1;
                    do
                    {
                        name = name.Replace(".csv",$"_{version}.csv");
                        file = Path.Combine(path,name);
                        version++;
                    }while(File.Exists(file));

                    GenerateCSV(path, name, totalPerPeriod);
                }
            }
            catch(UnauthorizedAccessException ex) 
            {
                Console.WriteLine("Unable to access or create a file in the provided directory");
                throw ex;
            }
            catch
            {
                throw;
            }
        }

        public static void CreateOutputDirectory(string path)
        {
            //Process will attempt to create a directory in the provided path if it does not exist, as well as all parents
            try
            {
                if (string.IsNullOrEmpty(path)) return;

                if (!Directory.Exists(path))
                {
                    CreateOutputDirectory(Path.GetDirectoryName(path));

                    Directory.CreateDirectory(path);
                }
                
            }
            catch (UnauthorizedAccessException ex)
            {
                throw;
            }
            catch 
            {
                throw;
            }

        }

        public static void CreateOutputDirectoryLocal()
        {
            try
            {


                string path = Directory.GetCurrentDirectory() + "\\output";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {                
                throw;
            }
        }
    }
}
