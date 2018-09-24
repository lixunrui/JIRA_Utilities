﻿using System;
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

        const string subFolderPrefix = "J_";

        /*
        https://social.msdn.microsoft.com/Forums/vstudio/en-US/94b4e59b-33d2-4230-873c-eaea680c973d/get-changed-text-using-filesystemwatcher?forum=csharpgeneral

        */

        Dictionary<FileInfo, long> watchFiles;
        //List<FileInfo> watchFiles;

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
        internal DirectoryWatcher(string dirPath)
        {
            _dirPath = dirPath;
            watchFiles = new Dictionary<FileInfo, long>();

            if (!Directory.Exists(_dirPath))
                return;

            watcher = new FileSystemWatcher(_dirPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Renamed += Watcher_Renamed;
            watcher.Deleted += Watcher_Deleted;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected void StartWater()
        {
          
            DirectoryInfo directoryInfo = new DirectoryInfo(_dirPath);

            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            foreach(FileInfo f in files)
            {
                // add all files into the directory
                watchFiles.Add(f, f.Length);
                Console.WriteLine("Adding file {0} (length:{1}) to list.", f.Name, f.Length);
            }

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
            // search the directory for the file and compare the last write time 
            FileInfo modifiedFile = new FileInfo(Path.Combine(_dirPath, e.Name));
            KeyValuePair<FileInfo, long> TempFile = watchFiles.FirstOrDefault(f => f.Key.Name == e.Name);
            if(TempFile.Equals(default(KeyValuePair<FileInfo, long>)))
            {
                return;
            }

            // output the new added data
            using (FileStream fstream = new FileStream(Path.Combine(_dirPath, e.Name), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fstream.Position = TempFile.Key.Length;
                using (StreamReader reader = new StreamReader(fstream))
                {
                    string newString = reader.ReadToEnd();
                    Console.Write(newString);
                    // create a temp file in a different folder, and write the new strings in
                    using (FileStream wfStream = new FileStream(Path.Combine(_dirPath, subFolderPrefix + e.Name), FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        using (StreamWriter write = new StreamWriter(wfStream))
                        {
                            write.Write(newString);
                            write.Flush();
                        }
                    }
                }
            }
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
