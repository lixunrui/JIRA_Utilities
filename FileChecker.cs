using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace JIRAFolderOpener
{
    internal static class FileChecker
    {
        internal static void OpenFile(string caseNum, out string targetFullPath)
        {
            // check this is a EP or ES 
            if (caseNum == null)
            {
                targetFullPath = null;
                return;
            }

            string project = caseNum.Substring(1, 2).ToLower();
            caseNum = caseNum.Substring(1, caseNum.Length - 2);
            string targetPath;
            if (project.Equals("ep"))
            {
                targetPath = @"\\appserv\Logs\EP";
            }
            else if (project.Equals("es"))
            {
                targetPath = @"\\appserv\Logs\ES";
            }
            else
            {
                targetFullPath = null;
                return;
            }
            string fullPath = Path.Combine(targetPath, caseNum);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            //fullPath = fullPath.Replace(@"\\appserv\Logs", @"P:");

            targetFullPath = fullPath;

            Process.Start(fullPath);

            fullPath = fullPath.Replace(@"\\appserv\Logs", @"P:");

            System.Windows.Forms.Clipboard.SetText(fullPath);

            //MessageBox.Show("Folder path has been copied to Clipboard");
        }
    }
}
