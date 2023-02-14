using System.Configuration;
using System.Text.RegularExpressions;

namespace TradingReport
{
    internal static class ConsoleManager
    {
        public static void InitializeConsole(ConfigurationValues configurationValues)
        {
            try
            {
                ConfigureOutputPath(configurationValues);
                ConfigureIntervalTimer(configurationValues);
                ConfigureExecutionMode(configurationValues);
            }
            catch
            {
                throw;
            }
           
        }

        public static void ShowProgramConfiguration(ConfigurationValues configValues)
        {
            Console.Clear();
            Console.WriteLine("Program is running...");
            Console.WriteLine();
            Console.WriteLine("Showing configuration: ");
            Console.WriteLine("Output path: {0}", string.IsNullOrEmpty(configValues.outputPath) ? Directory.GetCurrentDirectory() + "\\output" : configValues.outputPath);
            Console.WriteLine($"Interval timer: {configValues.intervalTimer} min");
            Console.WriteLine("Mode: {0}", configValues.automaticExecution ? "Automatic" : "Manual");
            Console.WriteLine("Press 'Enter' to exit the application.");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        private static void ConfigureOutputPath(ConfigurationValues configurationValues)
        {
            Console.Clear();
            Console.WriteLine("Do you wish to set up a path manually? If not, it will take the current one from configuration file. Y/N");
            var outputInfo = string.IsNullOrEmpty(configurationValues.outputPath) ? "No path, file will generate in current directory" : configurationValues.outputPath;
            Console.WriteLine($"Current configuration path: {outputInfo}");
            var input = GetUserInput();
            if (input.ToString().ToUpper() == "Y")
            {
                Console.WriteLine("Please enter the path to write csv output files");
                string? path = Console.ReadLine();
                while (!IsValidPath(path))
                {
                    Console.WriteLine("Invalid path, please enter a new one");
                    path = Console.ReadLine();
                }

                Console.WriteLine("Path is valid, do you wish to update it to configuration file? Y/N");
                //Save path for use on this program
                configurationValues.SetOutputPath(path);

                //Check if value needs to be updated on config file
                input = GetUserInput();
                if (input.ToString().ToUpper() == "Y")
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["outputPath"].Value = path;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }

        private static void ConfigureIntervalTimer(ConfigurationValues configurationValues)
        {
            Console.Clear();
            Console.WriteLine("Do you wish to set up an interval manually? If not, it will take the current one from configuration file. Y/N");
            Console.WriteLine($"Current configuration interval: {configurationValues.intervalTimer}");
            var input = GetUserInput();
            if (input.ToString().ToUpper() == "Y")
            {
                Console.WriteLine("Please enter the interval, in amount of minutes, in which the program will repeat the process");
                string? interval = Console.ReadLine();
                while (!IsValidInterval(interval))
                {
                    Console.WriteLine("Invalid interval, please enter a new one");
                    interval = Console.ReadLine();
                }

                Console.WriteLine("Interval is valid, do you wish to update it to configuration file? Y/N");
                //Save interval for use on this program
                configurationValues.SetIntervalTimer(Convert.ToInt32(interval));

                //Check if value needs to be updated on config file
                input = GetUserInput();
                if (input.ToString().ToUpper() == "Y")
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["intervalTiming"].Value = interval;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }

        private static void ConfigureExecutionMode(ConfigurationValues configurationValues)
        {
            Console.Clear();
            Console.WriteLine("Do you wish to to start the execution in manual mode? Y/N");
            Console.WriteLine("Manual mode will request the user to manually enter the date to obtain trading data");
            
            var input = GetUserInput();
            if (input.ToString().ToUpper() == "Y")
            {
                configurationValues.SetAutomaticExecution(false);
            }
        }

        private static char GetUserInput()
        {
            //Verifies that the user input matches one of the options in Y/N
            char input;
            do
            {
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }
            while (!Regex.IsMatch(input.ToString(), "^[yYnN]$"));

            return input;
        }

        private static bool IsValidInterval(string? interval)
        {
            if(string.IsNullOrEmpty(interval))
            {
                return false;
            }

            if(int.TryParse(interval, out int intValue))
            {
                if(intValue <= 0)
                {
                    Console.WriteLine("Interval value should be greater or equal than 1");
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsValidPath(string? path)
        {
            try
            {
                if(string.IsNullOrEmpty(path))
                {
                    return false;
                }

                //Check if path format is valid
                if(!(Path.IsPathRooted(path) && Path.GetFullPath(path) == path))
                {
                    return false;
                }

                //Check if Directory exists, we are not creating a directory in this execution
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("The provided directory does not currently exist, do you wish to create it? Y/N");
                    var input = GetUserInput();
                    if (input.ToString().ToUpper() == "Y")
                    {
                        //Creating directory
                        FileManager.CreateOutputDirectory(path);
                    }
                    else
                    {
                        //User does not want to create directory, a new one will be requested
                        return false;
                    }
                }

                //Check if the directory can be accesed
                using (FileStream fs = File.Create(Path.Combine(path, "test.txt"), 1, FileOptions.DeleteOnClose))
                {
                    // The user can access the directory.                    
                }

                //All validations have succeded
                return true;

            }
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine("Insufficient permissions to create file on directory");
                return false;
            }
            catch 
            { 
                throw; 
            }
        }
    }
}
