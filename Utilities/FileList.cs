using System;
using System.Collections.Generic;
using System.IO;

namespace JIRASupport
{
    internal static class FileTypes
    {
        static internal List<string> ZipFileTypes = new List<string>() 
            {
                ".zip",
                ".rar"
            };
    }
    
    internal class FileList<T> : List<T>
    {
        internal delegate void FileChange(int fileCount, bool hasZipFile);
        internal event FileChange OnFileChange;
       
     //   private List<string> items = new List<string>();

        internal new void Add(T item)
        {
            base.Add(item);
            if (OnFileChange != null)
            {
                if (ContainsZipFile())
                {
                    OnFileChange(base.Count, true);
                }
                else
                {
                    OnFileChange(base.Count, false);
                }
            }     
        }

        internal new void Remove(T item)
        {
            base.Remove(item);
            if (OnFileChange != null)
            {
                if (!ContainsZipFile())
                {
                    OnFileChange(base.Count, false);
                }
                else
                {
                    OnFileChange(base.Count, true);
                }
            }
        }

        internal new void Clear()
        {
            base.Clear();
            if (OnFileChange != null)
            {
                OnFileChange(base.Count, false);
            }
        }

        private bool ContainsZipFile()
        {
            T item = default(T);
           
            try
            {
                foreach (string zipFileExt in FileTypes.ZipFileTypes)
                {
                    item = Find(file => Path.GetExtension(Convert.ToString(file)).ToLower() == zipFileExt);
                    if (item != null)
                    {
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("ContainsZipFile error: {0}", ex.Message);
            }

            if (item == null)
            {
                return false;
            }
            return true;

        }

        internal new T Find(Predicate<T> match)
        {
            return base.Find(match);
        }

        internal void SortByTime(string a, string b, bool ascending)
        {
            FileInfo fileA = new FileInfo(a);
            FileInfo fileB = new FileInfo(b);
        }
    }
}
