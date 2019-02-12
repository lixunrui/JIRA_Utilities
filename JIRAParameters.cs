using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace JIRASupport
{
    internal struct FileStruct
    {
        internal string Name;
        internal DateTime LastWriteTime;
        internal string FileSize;

        internal FileStruct(string name, DateTime lastWriteTime, string size)
        {
            Name = name;
            LastWriteTime = lastWriteTime;
            FileSize = size;
        }
    }
    public static class LocalFilePath
    {
        static string sourceFilePath = @"\\appserv\Logs\Incoming\\";
        public static string SourceFilePath
        {
            get { return sourceFilePath; }
            //set { sourceFilePath = value; }
        }
        static string enablerDBPath = @"C:\EnablerDB";
        public static string EnablerDBPath
        {
            get { return enablerDBPath; }
            //set { enablerDBPath = value; }
        }
        public static string JIRAMappingFileName = "JIRA_File_Mapping.txt";
    }

    public static class FTPParameters
    {
        public static class ServerSettings
        {
            static string loginUserName;
            public static string LoginUserName
            {
                get { return loginUserName; }
                set { loginUserName = value; }
            }

            static string loginPassword ;
            public static string LoginPassword
            {
                get { return loginPassword; }
                set { loginPassword = value; }
            }

            static string serverAddress;
            public static string ServerAddress
            {
                get { return serverAddress; }
                set { serverAddress = value; }
            }

            static string serverPublicIncomingFolderPath ;
            public static string ServerPublicIncomingFolderPath
            {
                get { return serverPublicIncomingFolderPath; }
                set { serverPublicIncomingFolderPath = value; }
            }
        }

        public static class LocalSettings
        {
            static string editor;
            public static string Editor
            {
                get { return editor; }
                set { editor = value; }
            }

            static string logFolder;
            public static string LogFolder
            {
                get { return logFolder; }
                set { logFolder = value; }
            }

            static string logFolderMapDriver;
            public static string LogFolderMapDriver
            {
                get { return logFolderMapDriver; }
                set { logFolderMapDriver = value; }
            }
        }

        public static void LoadServerInfoFromFile()
        {
            // NOTE: Serializer only works with class that can have instance
            #region
            //string configPath = Directory.GetCurrentDirectory();

            //configPath = Path.Combine(configPath, "Configuration\\settings.xml");

            //XmlDocument doc = new XmlDocument();

            //XmlSerializer deSerializer = new XmlSerializer(typeof(Settings));

            //TextReader reader = new StreamReader(configPath);

            //Settings globalSettings = (Settings)deSerializer.Deserialize(reader);

            //reader.Close();
            #endregion

            XmlDocument doc = new XmlDocument();

            string xmlPath = Directory.GetCurrentDirectory();

            xmlPath = Path.Combine(xmlPath, "Configuration\\settings.xml");

            doc.Load(xmlPath);
            
            XmlNode serverNode = doc.DocumentElement.SelectSingleNode("Server");

            FTPParameters.ServerSettings.LoginUserName = serverNode.SelectSingleNode("LoginUserName").InnerText;

            FTPParameters.ServerSettings.LoginPassword = serverNode.SelectSingleNode("LoginPassword").InnerText;

            FTPParameters.ServerSettings.ServerAddress = serverNode.SelectSingleNode("ServerAddress").InnerText;

            FTPParameters.ServerSettings.ServerPublicIncomingFolderPath = serverNode.SelectSingleNode("IncomingPath").InnerText;

            XmlNode localNode = doc.DocumentElement.SelectSingleNode("Local");

            XmlNode localFolderNode = localNode.SelectSingleNode("LogFolder");
            FTPParameters.LocalSettings.LogFolder = localFolderNode.InnerText;

            FTPParameters.LocalSettings.LogFolderMapDriver = localFolderNode.Attributes["Network"].Value;
        }
    }

    /// <summary>
    /// Copy file function, as File.Copy gets stuck / takes longer time to complete.
    /// </summary>
    internal static class JIRAFile
    {
        internal static void Copy(string source, string destination)
        {
            int length = (int)(new FileInfo(source).Length);
            byte[] dataArray = new byte[length];
            using (FileStream fsRead = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.None, length))
            {
                using (BinaryReader bwRead = new BinaryReader(fsRead))
                {
                    using (FileStream fsWrite = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, length))
                    {
                        using (BinaryWriter bwWrite = new BinaryWriter(fsWrite))
                        {
                            for(; ;)
                            {
                                int read = bwRead.Read(dataArray, 0, length);
                                if (read == 0)
                                    break;
                                bwWrite.Write(dataArray, 0, length);
                            }
                        }
                    }
                }
            }
        }
    }

}
