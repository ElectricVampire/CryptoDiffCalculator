using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffCalculator
{
    public static class Logger
    {
        private static StreamWriter sw = null;
        public static void WriteLogs(string source, LogLevel loglevel, string message)
        {
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + "----Source:: " + source + "----LogLevel:: " + loglevel.ToString() + "----Message:: " + message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public enum LogLevel
    {
        Information,
        Error
    }
}
