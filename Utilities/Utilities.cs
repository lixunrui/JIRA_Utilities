using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JIRASupport.Utilities
{
    public class Utilities
    {
        HotKeyController KeyController;
        public string testString;
        bool ftpConnectionStatus;
        public bool FtpConnectionStatus
        {
            get { return ftpConnectionStatus; }
        }

        public delegate void MessagerDelegate(MessageType msgType, string message);
        public event MessagerDelegate MessagerEvent;

        public Utilities()
        {
            KeyController = new HotKeyController();
            KeyController.RegisterHotKeyEvent += KeyController_RegisterHotKeyEvent;
            // loads everything on startup
            ftpConnectionStatus = FTPParameters.LoadServerInfoFromFile(out testString);
            
        }

        public bool AddHotKey(int modifier, Keys key, IntPtr Handle)
        {
            bool _keyAdded = false;

            KeyHandler hotKey = new KeyHandler(modifier, key, Handle);

            _keyAdded = KeyController.RegisterHotKey(hotKey);

            return _keyAdded;
        }

        public void RemoveHotKey()
        {
            KeyController.UnregisterAllHotKey();
        }

        public string GetTargetPath()
        {
            string targetPath;
            
            FileOperator.OpenFile(URLExtractor.GetJIRANumberFromURL(), out targetPath);

            if (targetPath == null)
            {
                MessagerEvent?.Invoke(MessageType.ERROR, "Folder location invalid, no JIRA # found");
            }
                
            return targetPath;
        }

        #region Events
        private void KeyController_RegisterHotKeyEvent(KeyHandler key, RegisterStatus type)
        {
            string registerMsg = string.Empty;
            switch(type)
            {
                case RegisterStatus.FAILURE:
                    registerMsg = string.Format("Register hot keys {0} failed.", key.Key.ToString());
                    break;
                case RegisterStatus.SUCCESS:
                    registerMsg = string.Format("Register hot keys {0} succeed.", key.Key.ToString());
                    break;
                case RegisterStatus.UNREGISTERED:
                    registerMsg = string.Format("Unregister hot keys {0} succeed.", key.Key.ToString());
                    break;
                case RegisterStatus.UNREGISTER_FAILED:
                    registerMsg = string.Format("Unregister hot keys {0} failed.", key.Key.ToString());
                    break;
            }
            MessagerEvent?.Invoke(MessageType.MESSAGE, registerMsg);
        }
        #endregion

        #region file operation
        public List<FileInfo> GetFilesByLastWriteTime(string folderPath, string searchPattern, SearchOption searchOption)
        {
            List<FileInfo> fileList = GetFiles(folderPath, searchPattern, searchOption);

            fileList = fileList.OrderByDescending(f => f.LastWriteTime).ToList();

            return fileList;
        }

        public List<FileInfo> GetFiles(string folderPath, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<FileInfo> fileList = new List<FileInfo>();

            string[] searchPatterns = searchPattern.Split('|');

            foreach (string sp in searchPatterns)
            {
                List<FileInfo> tempFileList = new DirectoryInfo(folderPath).GetFiles(sp, searchOption).ToList();
                fileList.AddRange(tempFileList);
            }
            return fileList;
        }
        #endregion
    }
}
