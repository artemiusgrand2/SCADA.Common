using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using System.Windows.Media;
using SCADA.Common.ImpulsClient.requests;

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

        public static string GetDiagnostInfoForAnswerCommand(RequestError answer, int station, string command)
        {
            switch (answer)
            {
                case RequestError.Successful:
                    return $"Команда '{command}', станция - {station} успешно отправлена.";
                case RequestError.AccessDenied:
                    return $"Команда '{command}', станция - {station} не выполнена. Доступ закрыт";
                case RequestError.IOError:
                    return $"Команда '{command}', станция - {station} не выполнена. Ошибка связи";
                case RequestError.UnknownCommand:
                    return $"Команда '{command}', станция - {station} не выполнена. Неизвестная команда";
                case RequestError.UnknownRequest:
                    return $"Команда '{command}', станция - {station} не выполнена. Неизвестный запрос";
                case RequestError.UnknownStation:
                    return $"Команда '{command}', станция - {station} не выполнена. Неизвестная станция";
                default:
                    return $"Команда '{command}', станция - {station} не выполнена. Неизветсная ошибка - '{(int)answer}'";
            }
        }

        public static int RGBtoInt(Color color)
        {
            return (256 * 256 * color.R + 256 * color.G + color.B);
        }

        public static int RGBtoInt(byte r, byte g, byte b)
        {
            return (256 * 256 * r + 256 * g + b);
        }
    }
}
