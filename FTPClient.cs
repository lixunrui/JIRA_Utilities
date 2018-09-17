using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// https://www.codeproject.com/Tips/443588/Simple-Csharp-FTP-Class
/// 
/// http://www.codingvision.net/networking/c-connecting-to-ftp-server
/// </summary>

namespace JIRAFolderOpener
{
    internal class FTPClient
    {
        private string host = null;
        
        private string user = null;
        private string password = null;
        private FtpWebRequest ftpWebRequest = null;
        private FtpWebResponse ftpWebResponse = null;
        private Stream ftpStream = null;

        public delegate void MessageDelegate(string message);
        public event MessageDelegate SendMessage;
        
        internal FTPClient(string hostIP, string userName, string pwd)
        {
            //TODO: LAter
            // FTPParameters.LoadServerInfoFromFile();
            host = hostIP; user = userName; password = pwd;
        }

        ~FTPClient()
        {
            DisconnectToServer();
        }

        private void DisconnectToServer()
        {
            ftpStream.Close();
            ftpWebResponse.Close();
            ftpWebRequest = null;
        }

        internal List<FileStruct> ListDirectoryDetails(string directory)
        {
            if(host == null)
            {
                //load server info failed
                return null;
            }

            host += directory;
            ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(host);
            ftpWebRequest.Credentials = new NetworkCredential(user, password);
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.KeepAlive = true;
            ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

            ftpStream = ftpWebResponse.GetResponseStream();

            List<FileStruct> files = new List<FileStruct>();
            string pattern = @"^(-.+-)\s+(\d+)\s+(.+)\s+(.+)\s+(\d+)\s+(\D{3}\s{1,2}\d{1,2}\s{1,2}(\d{4}|\d{2}:\d{2}))\s+(.+)$";
            Regex regex = new Regex(pattern);
            IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");

            using (StreamReader ftpReader = new StreamReader(ftpStream))
            {
                while(!ftpReader.EndOfStream)
                {
                    string line = ftpReader.ReadLine();
                    Console.WriteLine(line);
                    Match match = regex.Match(line);
                    if (match.Length == 0)
                        continue;
                    string fileName = match.Groups[8].Value;
                    string fileSize = match.Groups[5].Value;
                    FileStruct tempFile = new FileStruct { Name = fileName, FileSize = fileSize };
                    files.Add(tempFile);
                    if (SendMessage != null)
                    {
                        SendMessage(string.Format("Found file: {0}, size: {1}", tempFile.Name, tempFile.FileSize));
                    }
                    Console.WriteLine(fileName);
                }
            }
            DisconnectToServer();
            return files;
        }

        internal void DownloadFilesFromServer(List<FileStruct> filesToDownload)
        {
            foreach(FileStruct fileToDownload in filesToDownload)
            {
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileToDownload.Name);
                ftpWebRequest.Credentials = new NetworkCredential(user, password);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.UsePassive = true; 
                ftpWebRequest.KeepAlive = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                ftpStream = ftpWebResponse.GetResponseStream();

                string fileNameToDownload = fileToDownload.Name;
                // check if the file has already exit
                if(File.Exists(Path.Combine(FTPParameters.ServerSettings.ServerPublicIncomingFolderPath, fileToDownload.Name)))
                {
                    // check the size, if the size is different, then rename the download one
                    long existingFileSize = new FileInfo(Path.Combine(FTPParameters.ServerSettings.ServerPublicIncomingFolderPath, fileToDownload.Name)).Length;
                    if(existingFileSize != Convert.ToInt32(fileToDownload.FileSize))
                    {
                        //rename the file to be downloaded
                        fileNameToDownload += string.Format("_{0}",fileToDownload.FileSize);
                        Console.WriteLine("Updated download file name {0}", fileToDownload.Name);
                    }
                }

                FileStream localFileStream = new FileStream(LocalFilePath.SourceFilePath+"/"+ fileNameToDownload, FileMode.Create);

                int numBytesToRead = Convert.ToInt32(fileToDownload.FileSize);
                Console.WriteLine("Total byte to read is {0}", numBytesToRead);
               
                int bufferSize = 1024;
                byte[] byteBuffer = new byte[bufferSize];

                int byteRead = 0;

                try
                {
                    while ((byteRead = ftpStream.Read(byteBuffer, 0, bufferSize)) > 0)
                    {
                        /*
                         * // Summary:
                            //     Writes a block of bytes to the file stream.
                            //
                            // Parameters:
                            //   array:
                            //     The buffer containing data to write to the stream.
                            //
                            //   offset:
                            //     The zero-based byte offset in array from which to begin copying bytes to the
                            //     stream.
                            //
                            //   count:
                            //     The maximum number of bytes to write. <- WRONG comment: The latest comment is: The number of bytes to be written to the current stream. NOT MAXIMUM!!!
                         */
                        localFileStream.Write(byteBuffer, 0, byteRead);
                    }

                    localFileStream.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                localFileStream.Close();
                if (SendMessage != null)
                {
                    SendMessage(string.Format("Downloaded file: {0}", fileToDownload.Name));
                }
                DisconnectToServer();
            }
        }

        /// <summary>
        /// This can be used to remove a file from the server after have it downloaded
        /// </summary>
        /// <param name="fileToRemove"></param>
        internal void RemoveFileFromServer(FileStruct fileToRemove)
        {
            ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileToRemove.Name);
            ftpWebRequest.Credentials = new NetworkCredential(user, password);
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.KeepAlive = true;
            ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

            ftpStream = ftpWebResponse.GetResponseStream();

            ftpStream.Close();
            ftpWebResponse.Close();
            ftpWebRequest = null;
        }
    }
}
