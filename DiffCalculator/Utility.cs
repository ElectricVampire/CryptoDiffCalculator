using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffCalculator
{
    class Utility
    {

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static string GetLinkData(string link)
        {
            string data = string.Empty;
            try
            {
                string address = link;
                using (var webpage = new WebClient())
                {
                    try
                    {
                        data = webpage.DownloadString(address);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLogs("Utility", LogLevel.Error, "Not able to read the data from link.\n Exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLogs("Utility", LogLevel.Error, "Not able to read the data from link.\n Exception: " + ex.Message);
            }

            return data;
        }

        public static string USDtoINR(decimal amount)
        {
            return CurrencyConvert(amount, "USD", "INR");
        }
        public static string CurrencyConvert(decimal amount, string fromCurrency, string toCurrency)
        {

            //Grab your values and build your Web Request to the API
            string apiURL = String.Format("https://www.google.com/finance/converter?a={0}&from={1}&to={2}&meta={3}", amount, fromCurrency, toCurrency, Guid.NewGuid().ToString());

            //Make your Web Request and grab the results
            var request = WebRequest.Create(apiURL);

            //Get the Response
            var streamReader = new StreamReader(request.GetResponse().GetResponseStream(), System.Text.Encoding.ASCII);

            //Grab your converted value (ie 2.45 USD)
            var result = Regex.Matches(streamReader.ReadToEnd(), "<span class=\"?bld\"?>([^<]+)</span>")[0].Groups[1].Value;

            //Get the Result
            return result;
        }

    }
}
