using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.AppData
{
    public static class AppSettings
    {
        private const string FirstRunKey = "FirstRun";

        public static bool IsFirstRun
        {
            get
            {
                if (bool.TryParse(ConfigurationManager.AppSettings[FirstRunKey], out bool result))
                    return result;
                return true; // По умолчанию true
            }
            set
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config.AppSettings.Settings[FirstRunKey] == null)
                {
                    config.AppSettings.Settings.Add(FirstRunKey, value.ToString());
                }
                else
                {
                    config.AppSettings.Settings[FirstRunKey].Value = value.ToString();
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }
}
