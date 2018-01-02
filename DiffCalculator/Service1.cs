using EmailSender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace DiffCalculator
{
    public partial class DiffCalculator : ServiceBase
    {
        private Timer timer = null;
        private ServiceConfig serviceConfig = null;
        private Email email = null;

        private string link1Address = null;
        private string link2Address = null;
        private string link1Regex = null;
        private string link2Regex = null;

        private string[] receivers = null;
        public DiffCalculator()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serviceConfig = Utility.ReadFromXmlFile<ServiceConfig>("ServiceConfig.xml");
            if (serviceConfig != null)
            {
                timer = new Timer();
                timer = new Timer(serviceConfig.ServiceTimeInterval);
                timer.Elapsed += new ElapsedEventHandler(time_tick);
                timer.Enabled = true;
                ReadServiceConfig();                
                Logger.WriteLogs("Service", LogLevel.Information, "Service Started");
            }
            else
            {
                Logger.WriteLogs("Service", LogLevel.Error, "Not able to read ServiceConfig.xml");
            }

        }

        private void ReadServiceConfig()
        {
            link1Address = serviceConfig.WebLinks.Link1.name;
            link1Regex = serviceConfig.WebLinks.Link1.regex;
            link2Address = serviceConfig.WebLinks.Link2.name;
            link2Regex = serviceConfig.WebLinks.Link2.regex;

            receivers = serviceConfig.Receivers;
        }

        private void time_tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                int link1Price = Convert.ToInt32(GetRegexOutput(Utility.GetLinkData(link1Address), link1Regex));
                int link2Price = Convert.ToInt32(GetRegexOutput(Utility.GetLinkData(link2Address), link2Regex));
                int diff = link1Price - link2Price;
                Logger.WriteLogs("Link1Value", LogLevel.Information, link1Address+ ": "+ link1Price.ToString());
                Logger.WriteLogs("Link1Value", LogLevel.Information, link2Address + ": " + link2Price.ToString());
                Logger.WriteLogs("Diff", LogLevel.Information,  "Diff: " + diff.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLogs("Service", LogLevel.Error, "Output from link can not be converted to integers Exception :"+ ex.Message);
            }
        }

        private string GetRegexOutput(string input, string pattern)
        {
            string outputdata = null;

            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(link1Regex))
            {                
                try
                {
                    outputdata = Regex.Match(input, pattern).ToString();
                    outputdata = outputdata.Replace(",", "");
                }
                catch (Exception ex)
                {
                    Logger.WriteLogs("Service", LogLevel.Error, "Exception occured during regex pattern match :" + ex.Message);
                }               
            }
            else
            {
                Logger.WriteLogs("Service", LogLevel.Error, "Either data retrieved from link is null or regex is null");
            }

            return outputdata;
        }

        protected override void OnStop()
        {
            Logger.WriteLogs("Service", LogLevel.Error, "Service Stopped");
        }
    }
}
