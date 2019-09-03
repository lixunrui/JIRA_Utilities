using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics;

namespace JIRASupport
{
    public enum OperationStatus
    {
        START,
        END,
        MESSAGE
    }
    /// <summary>
    /// This class uses to OPEN/CREATE folder and DECODE the logs, and send EVENT in order to update the UI
    /// </summary>
    public static class FileOperator
    {
        public delegate void OperationDelegate(string message, OperationStatus OS);
        public static event OperationDelegate OperationEvent;

        private static string editor;
        public static string CaseNumber;

	    public static string Editor
	    {
		    get 
            {
                UpdateEditor("EditTool");
                return editor; 
            }
		    set 
            {
                editor = value;
                UpdateEditor("EditTool", value);
            }
	    }
        public static void UnZipFile(string folder, string filename)
        {
            if (filename != null)
            {
                string fileExt = Path.GetExtension(filename).ToLower();
                IEnumerable<string> matchItem = FileType.ZipFileTypes.Where(ext => ext == fileExt);

                bool hasZipfile = FileType.ZipFileTypes.Contains(fileExt);
                //if (matchItem.Count() > 0)
                if(hasZipfile)
                {
                    try
                    {
                        string sourceFileName = Path.Combine(folder, filename);
                        string targetFileName = Path.Combine(folder, filename.Substring(0, filename.Length - 4));

                        SendEvent(String.Format("Unzipping {0} from {1} to {2}", filename, sourceFileName, targetFileName), OperationStatus.MESSAGE);

                        ZipFile.ExtractToDirectory(sourceFileName, targetFileName);

                        UnZipFile(Path.ChangeExtension(Path.Combine(folder, filename), null), null);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendEvent(String.Format("Exception on file:{0} - {1}", filename, e.Message), OperationStatus.MESSAGE);
                    }

                    SendEvent(null, OperationStatus.END);
                }
                else
                {
                    SendEvent(string.Format("{0} not an zip file", fileExt), OperationStatus.MESSAGE);
                }
            }
            else
            {
                SendEvent( string.Format("Checking {0} for any zip file", folder), OperationStatus.MESSAGE);
                DirectoryInfo directory = new DirectoryInfo(folder);
                DirectoryInfo[] subDir = directory.GetDirectories();
                if (subDir.Length != 0)
                {
                    foreach (DirectoryInfo dir in subDir)
                    {
                        string newDir = dir.FullName;
                        UnZipFile(newDir, null);
                    }
                }

                string[] files = Directory.GetFiles(folder);

                foreach (string file in files)
                {
                    UnZipFile(folder, Path.GetFileName(file));
                }
            }
        }

        public static void DecodeFiles(string folder)
        {
            if(!Directory.Exists(folder))
            {
                SendEvent(string.Format("{0} doesn't exist", folder), OperationStatus.MESSAGE);
                return;
            }

            string[] allLogs = Directory.GetFiles(folder, "*.log", SearchOption.AllDirectories);

            Console.WriteLine("Found {0} logs", allLogs.Length);
            SendEvent(String.Format("Decoding folder {0}", folder), OperationStatus.MESSAGE);

            Process process = new Process();

            //process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.FileName = @"DC.exe";

            foreach (string log in allLogs)
            {
                string newLogName = string.Format("\"{0}\"", log);
                process.StartInfo.Arguments = newLogName;
                SendEvent(String.Format("Decoding: {0}", newLogName), OperationStatus.MESSAGE);
                process.Start();

                process.WaitForExit();
            }
        }

        internal static void OpenFile(string caseNum, out string targetFullPath)
        {
            targetFullPath = null;
            // check this is a EP or ES 
            if (caseNum == null)
            {
                return;
            }

            string[] caseStrs = caseNum.Split('-');
            if (caseStrs.Length == 2)
            {
                string targetPath = null;
                string projectType = caseStrs[0].ToLower();
                if (projectType.Equals("ep"))
                {
                    targetPath = FTPParameters.LocalSettings.LogFolder + "\\EP";
                }
                else if (projectType.Equals("es"))
                {
                    targetPath = FTPParameters.LocalSettings.LogFolder + "\\ES";
                }

                if (targetPath != null)
                {
                    string fullPath = Path.Combine(targetPath, caseNum);

                    CaseNumber = caseNum;

                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    if(FileOperator.MapNetworkDriver(FTPParameters.LocalSettings.LogFolderMapDriver, FTPParameters.LocalSettings.LogFolder))
                    {
                        fullPath = fullPath.Replace(FTPParameters.LocalSettings.LogFolder, FTPParameters.LocalSettings.LogFolderMapDriver);
                    }

                    Process.Start(fullPath);

                    targetFullPath = fullPath;

                    System.Windows.Forms.Clipboard.SetText(fullPath);
                }   
            }
        }

        /// <summary>
        /// Map a network driver to a folder, EG: here we map P: to \\appserv\logs
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="folder"></param>
        /// <returns>True: Mapping succeed, False: Use the full path</returns>
        private static bool MapNetworkDriver(string driver, string folder)
        {
            bool mappingResult = false;

            // get login credential 
            string windowsLogin = System.Security.Principal.WindowsIdentity.GetCurrent().Name; // return INTEGRATION\RAYMONDL   

            if (!UnMapNetworkDriver(driver))
                return mappingResult;

            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "net.exe",
                    Arguments = string.Format("use {0} {1} /user:{2}", driver, folder, windowsLogin),
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            
            //p.StartInfo = psi;
            p.Start();

            string processError = p.StandardError.ReadToEnd();
            p.WaitForExit();

            if(processError == "" || processError.ToLower().Contains("in use"))
                mappingResult = true;
            //windowsLogin = Environment.UserName; // return RAYMONDL

            return mappingResult;
        }

        private static bool UnMapNetworkDriver(string driver)
        {
            bool unmappingResult = false;

            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "NET.EXE",
                    Arguments = string.Format("USE {0} /delete", driver),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute=false
                }
            };

            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            string errorout = p.StandardError.ReadToEnd();

            if(output.ToLower().Contains("force"))
            {
                p.StandardInput.WriteLine("Y");
                unmappingResult = true;
            }

            if (output.ToLower().Contains("successfully"))
                unmappingResult = true;

            if (errorout.ToLower().Contains("not be found"))
                unmappingResult = true;

            return unmappingResult;
        }

        internal static void OpenWithEditor(string workFolder)
        {
            string argu = workFolder;
            
            if (File.Exists(Editor))
            {
                Process process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo.FileName = Editor;
                process.StartInfo.Arguments = argu;
                process.Start();
                process.WaitForExit();
            }
        }

        private static void SendEvent(string message, OperationStatus OS)
        {
            if (OperationEvent != null)
                OperationEvent(message, OS);
        }

        private static void UpdateEditor(string configName, string configValue=null)
        {
            string configPath = Directory.GetCurrentDirectory();

            configPath = Path.Combine(configPath, "Configuration\\settings.txt");

          
            string[] settings = File.ReadAllLines(configPath);
            List<string> settingList = settings.ToList();

            for (int i = 0; i < settingList.Count; i++)
            {
                string[] keyAndValue = settingList[i].Split('=');
                if (keyAndValue.Length != 2) // wrong format
                {
                    continue;
                }
                else // correct format
                {
                    if (String.Compare(keyAndValue[0], configName, true) == 0)
                    {
                        if (configValue == null)// get the value
                        {
                            configValue = keyAndValue[1];
                            editor = configValue;
                            return;
                        }
                        else // update the value
                        {
                            settingList.RemoveAt(i); // remove the item
                            i--; // since we moved the list
                        }
                        
                    }
                    else
                        continue;
                }
            }

            File.WriteAllLines(configPath, settingList.ToArray());

                    // new config or update
            if (configValue != null)
            {
                using (StreamWriter fileWrite = new StreamWriter(configPath, true))
                {
                    fileWrite.WriteLine(configName + "=" + configValue);
                    fileWrite.Flush();
                }
            }
        }
    }
}
