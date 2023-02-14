using Axpo;

namespace TradingReport
{
    internal static class PowerTrading
    {
        const int MINUTEINMS= 60000; //Conversion rate for a minute in Miliseconds

        static System.Timers.Timer timer = new System.Timers.Timer();

        public static void StartPowerTradingInterval(ConfigurationValues configurationValues)
        {   
            //Logic to choose which method will be delegated to the timer based on execution logic
            //The manual mode will expect the user to input the date, so the execution is synchronous
            Action<string> myDelegate = configurationValues.automaticExecution ? new Action<string>(ExecutePowerTradingAsync) : new Action<string>(ExecutePowerTrading);

            timer.Interval = MINUTEINMS * configurationValues.intervalTimer;
            timer.Elapsed += delegate { myDelegate(configurationValues.outputPath); };
            
            //If the execution is automatic the timer won't stop and wait for user input
            timer.AutoReset= configurationValues.automaticExecution ? false : true;
            timer.Enabled = true;
        }

        public static void ExecutePowerTrading(string outputPath)
        {
            Logger.Log("Executing power trading");
            PowerService PS = new();
            //Don't repeat the process unless the PowerServiceClass had a communication error
            bool repeat;
            do
            {
                try
                {
                    repeat = false;
                    //Get date value to use in power service
                    DateTime date = GetDate(false);
                    var trades = PS.GetTrades(date);
                    double[] totals = CalculatePowerPosition(trades);

                    string? path = outputPath;
                    if (string.IsNullOrEmpty(path))
                    {
                        path = Directory.GetCurrentDirectory() + "\\output";
                    }

                    string fileName = "PowerPosition_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                    FileManager.GenerateCSV(path, fileName, totals);
                }
                catch (PowerServiceException ex)
                {
                    //This is an error with the Power Service interface, the execution will repeat
                    Logger.Log(ex);
                    repeat = true;
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            } while (repeat);

            timer.Start();

        }
        public static async void ExecutePowerTradingAsync(string outputPath)
        {
            Logger.Log("Executing power trading async");
            PowerService PS = new();
            //Don't repeat the process unless the PowerServiceClass had a communication error
            bool repeat;
            do
            {
                try
                {
                    repeat = false;
                    //Get date value to use in power service
                    DateTime date = GetDate(true);
                    var trades = await PS.GetTradesAsync(date);
                    double[] totals = CalculatePowerPosition(trades);

                    string? path = outputPath;
                    if (string.IsNullOrEmpty(path))
                    {
                        path = Directory.GetCurrentDirectory() + "\\output";
                    }

                    string fileName = "PowerPosition_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                    FileManager.GenerateCSV(path, fileName, totals);
                }
                catch (PowerServiceException ex)
                {
                    //This is an error with the Power Service interface, the execution will repeat
                    Logger.Log(ex);
                    repeat = true;
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            } while (repeat);
        }
        private static double[] CalculatePowerPosition(IEnumerable<PowerTrade> powerTrades)
        {
            try
            {
                //Log all Ids that beling to the power trades
                Logger.Log("IDs in trade: ");
                foreach(PowerTrade powerTrade in powerTrades)
                {
                    Logger.Log(powerTrade.TradeId);
                }

                //Verfication that all Periods have the same length
                int distinctLenghtAmount = powerTrades.Select(array => array.Periods.Length).Distinct().Count();
                if(distinctLenghtAmount > 1)
                {
                    //log the error;
                    throw new ArgumentException("The amount of periods does not correspond with the correct format");
                }

                //Get number of periods and instance data structure to load data
                int numPeriods = powerTrades.Select(array => array.Periods.Length).First();
                double[] totalPerPeriod = new double[numPeriods];

                //We iterate through all positions in PowerTrade.Periods and Sum the values for the same index on all the arrays into a new one
                totalPerPeriod = Enumerable.Range(0, numPeriods)
                    .Select(i => powerTrades.Sum(item => item.Periods[i].Volume)).ToArray();


                return totalPerPeriod;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        private static DateTime GetDate(bool isAutomatic = true)
        {
            //Execution of method as automatic would return a date that is independant of user input
            //This simulates, for example, getting the date from an API
            if(isAutomatic)
            {
                return DateTime.Now;
            }

            //Manual mode will ask the user for input and validate result
            DateTime date;
            Console.Clear();
            Console.Write("Please enter the date you wish to get trade information from: ");
            string? dateValue = Console.ReadLine();

            if (DateTime.TryParse(dateValue, out date))
            {
                return date;
            }
            else
            {
                return isDateValid(dateValue);
            }
        }
        private static DateTime isDateValid(string? dateString)
        {
            DateTime date = new();
            bool isValid = false;
            while(!isValid)
            {
                Console.Write("Invalid date, please enter a new one: ");
                dateString = Console.ReadLine();
                isValid = DateTime.TryParse(dateString, out date);
            }

            return date;
        }

    }
}
