using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SCADA.Common.HelpCommon
{
    public class HelpFuctions
    {
        public static string GetFormatString(string data)
        {
            IList<char> filter = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' };
            string result = data;
            foreach (var bit in data.ToCharArray())
            {
                if (!filter.Contains(bit))
                    result = result.Replace(bit.ToString(), System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            //
            return result;
        }

        public static string[] GetFile(string filename)
        {
            try
            {
                return File.ReadAllLines(filename, Encoding.GetEncoding(1251));
            }
            catch
            {
                return null;
            }
        }

        public static double Speed(double start, double end, DateTime starttime, DateTime endtime)
        {
            return Math.Round(3.6 * (Math.Abs((end - start) / (endtime - starttime).TotalSeconds)), 1);
        }
    }
}
