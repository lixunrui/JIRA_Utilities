using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace JIRAFolderOpener
{
    /// <summary>
    /// This class uses to monitor a directory for any change, and then display/decode it
    /// </summary>
    internal class DirectoryWatcher
    {
        internal delegate void WatcherNotificationDelegate(string message);

        internal event WatcherNotificationDelegate WatcherNotification;

        string _dirPath;

        /*
        https://social.msdn.microsoft.com/Forums/vstudio/en-US/94b4e59b-33d2-4230-873c-eaea680c973d/get-changed-text-using-filesystemwatcher?forum=csharpgeneral

    */

        Dictionary<string, int> _filesLastWriteTime;

        /// <summary>
        /// Enable or disable the watcher
        /// </summary>
        public bool EnableWatcher
        {
            get
            {
                if (watcher != null)
                    return watcher.EnableRaisingEvents;
                else
                    return false;
            }
            set
            {
                if (watcher != null)
                {
                    if (value)
                    {
                        StartWater();
                    }
                    else
                        watcher.EnableRaisingEvents = false;
                }
                else
                    RaiseNotification("Watcher object is null");
            }
        }
        FileSystemWatcher watcher;
        protected DirectoryWatcher(string dirPath)
        {
            _dirPath = dirPath;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected void StartWater()
        {
            if (!Directory.Exists(_dirPath))
                return;
            _filesLastWriteTime = new Dictionary<string, int>();

            watcher = new FileSystemWatcher(_dirPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Renamed += Watcher_Renamed;
            watcher.Deleted += Watcher_Deleted;

            string[] files = Directory.GetFiles(_dirPath);

            foreach(File f in )

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {

        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {

        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File {0} created", e.Name);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File {0} changed", e.Name);
        }

        private void RaiseNotification(string message)
        {
            if (WatcherNotification != null)
                WatcherNotification(message);
        }
    }

    // Once a change event is fired, get the file and decode it, then write it into a new file Loop1.log => J_Loop1.log
    internal static class Decrypo
    {

    }
}
