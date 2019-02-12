using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JIRASupport
{
    public partial class MeterCheckForm : Form
    {
        string folderName;
        ManualResetEvent stopEvent = new ManualResetEvent(false);
        CancellationTokenSource cancelToken;
        CancellationToken ct;
        Process process;
        YLog log;

        public MeterCheckForm()
        {
            InitializeComponent();
        }

        public MeterCheckForm(string folderName, YLog log)
            : this()
        {
            // Load the folder and analyze the content
            if(Directory.Exists(folderName))
            {
                this.folderName = folderName;
                this.log = log;
                AnalyzeFolder();
            }
           
        }
        /// <summary>
        /// Only check Loop, Psrvr and DB files
        /// </summary>
        private void AnalyzeFolder()
        {
            List<FileInfo> sortedFileList = new DirectoryInfo(folderName).GetFiles("*.log", SearchOption.AllDirectories).OrderByDescending(f => f.Name).ToList();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (backgroundSearchWorker.IsBusy)
                backgroundSearchWorker.CancelAsync();
            else
                backgroundSearchWorker.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancelToken.Cancel();
        }

        #region Background Worker
        private void backgroundSearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           
                string searchStr = BuildSearchString();

                cancelToken = new CancellationTokenSource();
                ct = cancelToken.Token;

                var task = Task.Factory.StartNew(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    string searchResult = SearchbyCommand(searchStr, ct);

                }, cancelToken.Token);

                try
                {
                    log.WriteLine("Task waiting");
                    task.Wait(ct);
                    log.WriteLine("Task done");
                }
                catch(Exception)
                {
                    if(ct.IsCancellationRequested)
                    {
                        log.WriteLine("Task canceled");
                        cancelToken.Cancel();
                        if (process != null && !process.HasExited)
                        {
                            log.WriteLine("Process canceled");
                            process.Kill();
                        }
                            
                    }
                }
           
        }
        private void backgroundSearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundSearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Task finished");
        }

        private string SearchbyCommand(string searchStr, CancellationToken ct)
        {
            string searchOutput = string.Empty;

            process = new Process();
            process.StartInfo.FileName = "grep.exe";
            process.StartInfo.Arguments = searchStr;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.Start();
            
            searchOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
           
            return searchOutput;
        }

        /// <summary>
        /// Build string to search all meter readings
        /// </summary>
        /// <returns></returns>
        private string BuildSearchString()
        {
            List<int> pumpIDs = GetInputNumber(txtPumpNumber.Text);
            List<int> hoseNum = GetInputNumber(txtHoseNumber.Text);
            DateTime startDT = Convert.ToDateTime(startDateTime.Value);
            DateTime endDT = Convert.ToDateTime(endDateTime.Value);


            StringBuilder searchStr = new StringBuilder();
          //  searchStr.Append("-i -n \""); // 
            

            // -i ignor case; -n show line number
            string s = "-i -n \"pump 01 delivery complete.*\" c:\\enabler\\log\\*.log";
            searchStr.Append(s);

            return searchStr.ToString();
        }


        private List<int> GetInputNumber(string input)
        {
            List<int> numberList = new List<int>();

            char[] splitChar = new char[] { ' ', ',', '.'}; // Space  ,  .
            string[] split = input.Split(splitChar);
            
            try
            {
                foreach(string s in split)
                {
                    numberList.Add(Convert.ToInt32(s));
                }
            }
            catch(InvalidCastException ex)
            {
                log.WriteLine(ex.Message);
                return null;
            }

            return numberList;
        }
       
        #endregion

    }
}
