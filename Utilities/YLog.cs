using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRASupport
{
    public class LogParameters
    {
        internal string LogDirectory;

        internal string ExceptionFileName;

        internal int MaxLogSize;

        internal int MaxLogNumber;

        internal int MaxTotalMb;

        internal int MaxSaveDays;
    }

    internal class DefaultParameters : LogParameters
    {
        public DefaultParameters()
        {
            LogDirectory = @"C:\Enabler\log\JIRA";
            ExceptionFileName = "JIRAException.log";
            MaxLogSize = 2 * 1024 * 1024; // 1 MB = 1024 kb; 1kb = 1024 byte, 1 byte = 8 bit
            MaxLogNumber = 10;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class YLog
    {
        string logFileName;
        string logFilePath;
        LogParameters LogParams;
        FileStream LogStream;
        StreamWriter LogWriter;
        bool logFileOpen = false;

        object Locker = new object();

        internal YLog(string moduleName, LogParameters parameters = null)
        {
            Init(moduleName, parameters);
        }

        void Init(string logName, LogParameters parameters)
        {
            this.logFileName = logName;
            LogParams = (parameters == null) ? new DefaultParameters(): parameters;
            logFilePath = Path.Combine(LogParams.LogDirectory, logName+".log");
            WritePlain("---------------------------------------------------------------");
            LogDateTime();
        }

        /// <summary>
        /// Log a line without time stamps
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        internal void WritePlain(string message, params object[] parameters)
        {
            if (parameters.Length == 0)
                Write(message, false);
            else
            {
                Write(string.Format(message, parameters), false);
            }
        }

        /// <summary>
        /// Log a line with tiem stamps
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        internal void WriteLine(string message, params object[] parameters)
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
            try
            {
                lock(Locker)
                {
                    OpenLog(logFilePath);

                    ArchiveFiles();

                    if(withTimeStamps)
                    {
                        LogWriter.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss.fff"), message));
                    }
                    else
                    {
                        LogWriter.WriteLine(message);
                    }

                    LogWriter.Flush();
                }
            }
            catch(Exception ex)
            {
                CloseLog();
                LogException(ex);
            }
        }

        private void OpenLog(string filePath)
        {
            if(!logFileOpen)
            {
                CheckDirectoryExists();

                LogStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                LogWriter = new StreamWriter(LogStream);

                GC.SuppressFinalize(LogStream);
                GC.SuppressFinalize(LogWriter);
            }
        }

        private void CheckDirectoryExists()
        {
            if (!Directory.Exists(LogParams.LogDirectory))
                Directory.CreateDirectory(LogParams.LogDirectory);
        }

        private void ArchiveFiles()
        {
            bool differentDay = File.GetLastWriteTime(logFilePath).Day != DateTime.Now.Day;

            if (LogWriter.BaseStream.Length > LogParams.MaxLogSize)
            {
                string dateName = File.GetLastWriteTime(logFilePath).ToString("yyyyMMdd_HHmm");
                // archive this one and open a new one
                LogFileEndMessage();
                CloseLog();
                int seq = -1;
                string archiveFileName;
                do
                {
                    seq++;
                    archiveFileName = Path.Combine(LogParams.LogDirectory, string.Format("{0}_{1}_{2}.log", logFileName, dateName, seq.ToString()));
                } while (File.Exists(archiveFileName));

                File.Move(logFilePath, archiveFileName);

                OpenLog(logFilePath);

                LogDateTime();
                WritePlain("Log continues from "+ Path.GetFileName(archiveFileName));
            }
            else if(differentDay)
            {
                LogDateTime();
            }
        }

        private void LogFileEndMessage()
        {
            try
            {
                LogDateTime();
                LogWriter.WriteLine("Log archived.");
            }
            catch(Exception ex)
            {
                LogException(ex);
            }
        }

        private void CloseLog()
        {
            try
            {
                if (LogWriter != null)
                {
                    LogWriter.Close();
                    LogWriter.Dispose();
                    LogStream.Close();
                    LogStream.Dispose();
                    LogWriter = null;
                    LogStream = null;
                }
            }
            catch
            { }

            logFileOpen = false;
        }

        private void LogDateTime()
        {
            DateTime CurDateTime = DateTime.Now;

            try
            {
                LogWriter.WriteLine("*** Time: " + CurDateTime.ToString("dd/MM/yyyy HH:mm:ss"));
                Flash();
            }
            catch(Exception ex)
            {
                LogException(ex);
            }
            
        }

        private void Flash()
        {
            if (LogWriter != null)
                LogWriter.Flush();
        }

        internal void LogException(Exception ex)
        {
            string exceptionFilePath = Path.Combine(LogParams.LogDirectory, LogParams.ExceptionFileName);

            OpenLog(exceptionFilePath);
            LogWriter.WriteLine(ex.Message);
            LogWriter.Flush();
            CloseLog();
        }
    }
}
