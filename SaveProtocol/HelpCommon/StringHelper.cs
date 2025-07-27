using System;


namespace SCADA.Common.HelpCommon
{
    public class StringHelper
    {
        public static string GetFullFileClick(string namefile, string arguments)
        {
            return  $"{namefile}${arguments}";
        } 

        public static void GetNameFileAndArguments(string fullname, out string namefile, out string arguments)
        {
            var cells = fullname.Split(new string[] { "$" }, 2, StringSplitOptions.RemoveEmptyEntries);
            namefile = (cells.Length > 0) ? cells[0] : string.Empty;
            arguments = (cells.Length > 1) ? cells[1] : string.Empty;
        }
    }
}
