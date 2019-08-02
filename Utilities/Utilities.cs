using System;
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
            if (type == RegisterStatus.SUCCESS)
            {
                MessagerEvent?.Invoke(MessageType.MESSAGE ,"Register hot keys succeed.");
            }
            else if (type == RegisterStatus.FAILURE)
            {
                MessagerEvent?.Invoke(MessageType.MESSAGE, "Register hot keys failed.");
            }
        }
        #endregion
    }
}
