//
// ---------------------------------------------------------------------------
//
using System;
using System.Linq;
//
using Newtonsoft.Json;
//
using NSG.PrimeNG.LazyLoading;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Valid an IP address.
        /// </summary>
        /// <param name="ipAddressString">IP address string.</param>
        /// <returns>boolean value of true or false.</returns>
        public static bool ValidateIPv4(this string ipAddressString)
        {
            //
            if (String.IsNullOrWhiteSpace(ipAddressString))
            {
                return false;
            }
            string[] splitValues = ipAddressString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }
            byte tempForParsing;
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
            //
        }
        //
        /// <summary>
        /// Parse the LazyLoadEvent string to check if valid.
        /// </summary>
        /// <param name="jsonString">JSON formated string.</param>
        /// <returns>boolean value of true or false.</returns>
        public static bool IsValidLazyLoadEventString(string jsonString)
        {
            bool _ret = true;
            try
            {
                LazyLoadEvent result = JsonConvert.DeserializeObject<LazyLoadEvent>(jsonString);
            }
            catch
            {
                _ret = false;
            }
            return _ret;
        }
        //
    }
}
