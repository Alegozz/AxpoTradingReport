using TradingReport;


try
{
    //Initialize configuration instance to operate the program
    Logger.Log("PROGRAM STARTED");
    ConfigurationValues configValues = new();    

    //Set and update configuration options
    ConsoleManager.InitializeConsole(configValues);

    //The first iteration must be as soon as the program starts
    //Based on configuration it will either be automatic or user will be expected to input a date
    Logger.Log("RUNNING FIRST EXECUTION");
    if (configValues.automaticExecution)
    {
        PowerTrading.ExecutePowerTradingAsync(configValues.outputPath);
    }
    else
    {
        PowerTrading.ExecutePowerTrading(configValues.outputPath);
    }

    //Set Timer of GetTrades to match the interval, where X is in minutes
    Logger.Log("STARTING INTERVAL");
    PowerTrading.StartPowerTradingInterval(configValues);
    
    //Show information to the user about the selected values
    ConsoleManager.ShowProgramConfiguration(configValues);
}
catch(Exception ex)
{
    Logger.Log(ex);
    Console.WriteLine("There was an error during the execution, please check the log for more information");
}