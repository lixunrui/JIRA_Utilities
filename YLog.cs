using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRAFolderOpener
{
    internal class LogParameters
    {
        internal string LogDirectory;

        internal int MaxLogSize;

        internal int MaxLogNumber;

        internal int MaxTotalMb;

        internal int MaxSaveDays;
    }

    
    /// <summary>
    /// 
    /// </summary>
    internal class YLog
    {
        string fileName;
        LogParameters logParams;
        StreamWriter fileStream;

        internal YLog(string moduleName, LogParameters parameters)
        {
            Init(moduleName, parameters);
        }

        void Init(string logName, LogParameters parameters)
        {
            this.fileName = logName;
            logParams = parameters;
            LogLine("---------------------------------------------------------------");
            LogDateTime();
        }

        internal void LogLine(string message, params object[] parameters)
        {
            if (parameters.Length == 0)
                Write(message, false);
            else
            {
                Write(string.Format(message, parameters), false);
            }
        }

        internal void Log(string moduleName, string message, params object[] parameters)
        {
            if (parameters.Length == 0)
                Write(message);
            else
            {
                Write(string.Format(message, parameters));
            }
        }

        private void Write(string message, bool withTimeStamps = true)
        {

        }

        private void LogDateTime()
        {
            DateTime CurDateTime = DateTime.Now;

            LogLine("*** Time: " + CurDateTime.ToString("dd/MM/yyyy HH:mm:ss"));
            Flash();
        }

        private void Flash()
        {
            if (fileStream != null)
                fileStream.Flush();
        }
    }
}
