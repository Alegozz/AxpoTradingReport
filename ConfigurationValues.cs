using System.Configuration;

namespace TradingReport
{
    internal class ConfigurationValues
    {
        //Minimun amount in minutes that an interval is valid
        const int MININTERVAL = 1;

        public string outputPath { get; private set; }
        public int intervalTimer { get; private set; }
        public bool automaticExecution { get; private set; }

        public ConfigurationValues() 
        {
            outputPath = GetConfigurationPath();
            intervalTimer = GetConfigurationInverval();
            automaticExecution = true;
        }

        private int GetConfigurationInverval()
        {
            //Normalize interval values to prevent errors in execution
            try
            {
                string? configInterval = ConfigurationManager.AppSettings.Get("intervalTiming");
                if (string.IsNullOrEmpty(configInterval))
                {
                    //Invalid value for interval timer, it will be updated before execution
                    UpdateIntervalConfig();
                    return MININTERVAL;

                }

                if (int.TryParse(configInterval, out int intValue))
                {
                    if (intValue <= 0)
                    {
                        UpdateIntervalConfig();
                        return MININTERVAL;
                    }
                    
                    return intValue;
                }
                else
                {
                    //Invalid value for interval timer, it will be updated before execution
                    UpdateIntervalConfig();
                    return MININTERVAL;
                }
            }
            catch
            { 
                throw; 
            }
        }

        private void UpdateIntervalConfig()
        {
            try
            {

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["intervalTiming"].Value = MININTERVAL.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            { 
                throw; 
            }
        }

        private string GetConfigurationPath()
        {
            //Normalize output values to avoid errors
            if(string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("outputPath")))
            {
                return "";
            }

            return ConfigurationManager.AppSettings.Get("outputPath");
        }

        public void SetOutputPath(string? path)
        {
            this.outputPath = string.IsNullOrEmpty(path) ? "" : path;
        }

        public void SetIntervalTimer(int intervalTimer)
        {
            this.intervalTimer = intervalTimer;
        }

        public void SetAutomaticExecution(bool automaticExecution)
        {
            this.automaticExecution = automaticExecution;
        }
    }
}
