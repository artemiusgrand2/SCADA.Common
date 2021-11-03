using System;
using System.IO;
using log4net;

namespace SCADA.Common.Log
{
    public class Logger
    {

        static readonly ILog logCommon = LogManager.GetLogger("CommonLog");
        static readonly ILog logCommands = LogManager.GetLogger("CommandsLog");
        /// <summary>
        /// логирование (общий файл)
        /// </summary>
        public static ILog LogCommon
        {
            get
            {
                return logCommon;
            }
        }

        /// <summary>
        /// логирование (файл комманд)
        /// </summary>
        public static ILog LogCommand
        {
            get
            {
                return logCommands;
            }
        }
    }
}
