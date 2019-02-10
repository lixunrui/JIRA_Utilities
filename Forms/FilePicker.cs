using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Threading;


namespace JIRAFolderOpener
{
    public partial class FilePicker : Form
    {  
       // const string decodeMiddlePaht = @"Enabler\Log"; // Folder\ITLXXXX\Enabler\Log
       // List<string> fileList;
        FileList<string> fileList;
        string targetFolder;
        AutoResetEvent resetEvent = new AutoResetEvent(false);
        bool isServerFolderOpen = true;
        bool openWithEditor = false;
        YLog log;

        const int NAME_INDEX = 0;
        const int LAST_WRITE_INDEX = 1;
        const int FILE_SIZE_INDEX = 2;
        const int FULL_PATH_INDEX = 3;

      //  Boolean localFolder = false;
        //  System.ComponentModel.BackgroundWorker backgroundWorker;
 
        // FileOperator fileOperator;

        public FilePicker(string folder, YLog log)//, FileOperator fileOperator)
        {
            InitializeComponent();
            InitComponents();
            this.CenterToScreen();
            this.log = log;
            this.targetFolder = folder;
            this.Text = folder;
            fileList = new FileList<string>();
            fileList.OnFileChange += fileList_OnFileChange;
            LoadFileList(LocalFilePath.SourceFilePath);
           // FTPParameters.LoadServerInfoFromFile();
            FileOperator.OperationEvent += FileOperator_OperationEvent;
        }

        // Init default text
        private void InitComponents()
        {
            groupBoxFolder.Text = Properties.Resources.Txt_Switch_Server;
            this.toolTips.SetToolTip(btnStart, Properties.Resources.Txt_Btn_Start_Tooltip);
            this.toolTips.SetToolTip(ckbUnzip, Properties.Resources.Txt_Unzip);
            this.toolTips.SetToolTip(ckbDecode, Properties.Resources.Txt_Decode);
            fileList_OnFileChange(0, false);
            if(isServerFolderOpen)
            {
                btnRestore.Visible = false;
                btnRestore.Enabled = false;
            }
        }

        void fileList_OnFileChange(int fileCount, bool hasZipFile)
        {
            if (fileCount == 0)
            {
                ckbDecode.Enabled = false;
                UpdateSelectedFileCount(Properties.Resources.Txt_File_No_File);
            }
            else
            {
                ckbDecode.Enabled = true;
                UpdateSelectedFileCount(String.Format(Properties.Resources.Txt_File_Count, fileCount));
            }

            if (!hasZipFile)
            {
                ckbUnzip.Enabled = false;
            }
            else
            {
                ckbUnzip.Enabled = true;
            }
        }

        void FileOperator_OperationEvent(string message, OperationStatus OS)
        {
            if (OS == OperationStatus.START)
            {
                backgroundWorker.ReportProgress(0);
            }
            else if (OS == OperationStatus.MESSAGE)
            {
                backgroundWorker.ReportProgress(1);

                UpdateActivityMessage(message);
            }
            else
            {        
                backgroundWorker.ReportProgress(100);//, message);
            }
        }

        void UpdateSelectedFileCount(string message)
        {
           // this.BeginInvoke(new Action(() =>
            //{
                lblFileCount.Text = message;
           // }));
        }

        void UpdateActivityMessage(string message)
        {
            log.WriteLine(message);
            this.BeginInvoke(new Action(() =>
            {
                richtxtActivity.Text += message + "\n";
            }));
        }

        #region Background Worker
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker.ReportProgress(0, "Start");
            foreach (string fileName in fileList)
            {
                if (isServerFolderOpen)
                {
                    FileInfo tempFileInfo = new FileInfo(fileName);
                    string destFile = Path.Combine(targetFolder, tempFileInfo.Name); // \\appserv\EP\EP-XXX\Enabler.XXX
                    string sourceFile = Path.Combine(LocalFilePath.SourceFilePath, fileName); // \\appserv\incoming\ENabler.XXX or \\appserv\Logs\EX-####
                    Console.WriteLine("Done copy");

                    System.IO.File.Copy(tempFileInfo.FullName, destFile, true);
                    Console.WriteLine("File {0} Copied", fileName);
                    UpdateActivityMessage(String.Format("File {0} Copied", fileName));
                }

                if (ckbUnzip.Checked)
                {
                    FileOperator.UnZipFile(targetFolder, fileName);
                }
                
                // check whether we need to decode
                if (ckbDecode.Checked)
                {
                    Console.WriteLine("Need to decode the file");
                    FileOperator.DecodeFiles(Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(fileName)));
                }
            }
            //resetEvent.Set();
            backgroundWorker.ReportProgress(100);
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // TODO: when create two instances, and close one of them, 
            // then star doing something with the rest instance, 
            // the statusProgress will raise an exception: Object reference not set, not sure why
            try
            {
                if (e.ProgressPercentage > 0 && e.ProgressPercentage < 100)
                {
                    if (Thread.CurrentThread.IsBackground)
                    {
                        Console.WriteLine("Background thread.");
                    }
                    else
                    {
                        Console.WriteLine(Thread.CurrentThread.Name);
                    }
                    if (statusProgress.Value >= 0 && (statusProgress.Value + e.ProgressPercentage) <= 100)
                    {
                        statusProgress.Value += e.ProgressPercentage;
                    }
                    else
                        statusProgress.Value = 0;
                }
                else if (e.ProgressPercentage == 100)
                {
                    statusProgress.Value = e.ProgressPercentage;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Here");
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusProgress.Value = 100;
            dataViewRemote.Enabled = true;
            btnStart.Enabled = true;
            btnCancel.Enabled = true;
            ckbDecode.Enabled = true;
            ckbDecode.Checked = false;
            ckbUnzip.Enabled = true;
            ckbUnzip.Checked = false;
            UpdateActivityMessage("Done");
            if (openWithEditor)// open the folder
            {
                FileOperator.OpenWithEditor(targetFolder);
            }
        }
        #endregion

        #region Create UI Table
        // Load Remote folder files
        private void LoadFileList(string folderPath)
        {
            // int: ID, string: name
            fileList.Clear();
            // latest one comes first
            List<FileInfo> sortedFileList = new DirectoryInfo(folderPath).GetFiles("*.zip", SearchOption.AllDirectories).OrderByDescending(f => f.LastWriteTime).ToList();

            CreateDataTableGrid(sortedFileList);

            Console.WriteLine("Count {0}", sortedFileList.Count);
        }

        private void CreateDataTableGrid(List<FileInfo> sortedFileList)
        {
            dataViewRemote.Columns.Clear();
            dataViewRemote.DataSource = null;

            if (sortedFileList.Count == 0)
            {
                return;
            }

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            dataViewRemote.Columns.Add(chk);
            chk.Width = 10;
            chk.HeaderText = "Select";
            chk.Name = "chk";
            chk.FillWeight = 20;
            chk.Selected = false;

            DataTable dt = new DataTable();
            //dt.Columns.Add(chk);

            DataColumn fileNameCol = new DataColumn("File Name");

            dt.Columns.Add("File Name");
            dt.Columns.Add("Last Write Time");
            dt.Columns.Add("Size");
            dt.Columns.Add("Full Path");

            foreach (FileInfo f in sortedFileList)
            {
                DataRow dr = dt.NewRow();
                dr[NAME_INDEX] = f.Name;
                dr[LAST_WRITE_INDEX] = f.LastWriteTime;
                dr[FILE_SIZE_INDEX] = String.Format("{0:0.0#} KB", (f.Length / 1024));
                dr[FULL_PATH_INDEX] = f.FullName;
                
                dt.Rows.Add(dr);
                Console.WriteLine("Adding {0}", f.Name);
            }
          
            //view.DataSource = dt;
            
            dataViewRemote.AutoGenerateColumns = false;
            dataViewRemote.AllowUserToResizeColumns = false;
            dataViewRemote.AllowUserToResizeRows = false;
            dataViewRemote.AllowUserToOrderColumns = false;
            dataViewRemote.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            
            DataGridViewTextBoxColumn  NameColumn = new DataGridViewTextBoxColumn ();
            NameColumn.HeaderText = "Name";
            NameColumn.FillWeight = 100;
            NameColumn.DataPropertyName = "File Name";
            dataViewRemote.Columns.Add(NameColumn);

            DataGridViewTextBoxColumn LastWriteTimeColumn = new DataGridViewTextBoxColumn();
            LastWriteTimeColumn.HeaderText = "Last Write Time";
            LastWriteTimeColumn.FillWeight = 40;
            LastWriteTimeColumn.DataPropertyName = "Last Write Time";
            dataViewRemote.Columns.Add(LastWriteTimeColumn);

            DataGridViewTextBoxColumn LengthColumn = new DataGridViewTextBoxColumn();
            LengthColumn.HeaderText = "Size";
            LengthColumn.FillWeight = 30;
            LengthColumn.DataPropertyName = "Size";
            dataViewRemote.Columns.Add(LengthColumn);

            DataGridViewTextBoxColumn FullPathColumn = new DataGridViewTextBoxColumn();
            FullPathColumn.HeaderText = "Full Path";
            FullPathColumn.FillWeight = 3;
            FullPathColumn.DataPropertyName = "Full Path";
            // dataViewRemote.Columns[FULL_PATH_INDEX].Visible = false;
            dataViewRemote.Columns.Add(FullPathColumn);

            dataViewRemote.DataSource = dt;
        }
        #endregion

        #region Button Event
        private void btnCancel_Click(object sender, EventArgs e)
        {
            FileOperator.OperationEvent -= FileOperator_OperationEvent;
            this.Close();
        }
        /// <summary>
        /// Refresh the server to get the latest file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //richtxtActivity.Text += JIRAFolderOpener.Properties.Resources.Txt_File_Refreshing + "\n";
            dataViewRemote.DataSource = null;
            if (isServerFolderOpen)
            {
                Task RefreshServer = RefreshServerAndDownload();
                
                RefreshServer.ContinueWith((task) => 
                {
                    if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        UpdateActivityMessage("Updating file list...");
                        if(this.InvokeRequired)
                        {
                            this.BeginInvoke(new Action(()=> 
                            {
                                LoadFileList(LocalFilePath.SourceFilePath);
                            }));
                        }
                        UpdateActivityMessage("Updating completed.");
                    }
                });
            }
            else
                LoadFileList(targetFolder);

            UpdateActivityMessage(JIRAFolderOpener.Properties.Resources.Txt_File_Updated);
        }

        /// <summary>
        /// Login to the server and check the public incoming folder for any new files, if has, then copy to P:\Incoming then reload the local file list
        /// </summary>
        private Task RefreshServerAndDownload()
        {
            return Task.Factory.StartNew(()=>
            {
                UpdateActivityMessage("Fetching from server...");

                FTPClient client = new FTPClient(FTPParameters.ServerSettings.ServerAddress, FTPParameters.ServerSettings.LoginUserName, FTPParameters.ServerSettings.LoginPassword);

                client.SendMessage += Client_SendMessage;

                List<FileStruct> newFiles = client.ListDirectoryDetails(FTPParameters.ServerSettings.ServerPublicIncomingFolderPath);

                if (newFiles!= null && newFiles.Count > 0)
                {
                    UpdateActivityMessage("Start downloading files ...");
                    client.DownloadFilesFromServer(newFiles);
                }
            });
        }

        private void Client_SendMessage(string message)
        {
            UpdateActivityMessage(message);
        }

        // Copy & Unzip
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
            richtxtActivity.Text = "";
            backgroundWorker.RunWorkerAsync();
           // resetEvent.WaitOne();
            dataViewRemote.Enabled = false;
            btnStart.Enabled = false;
            btnCancel.Enabled = false;
            ckbDecode.Enabled = false;
            ckbUnzip.Enabled = false;
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (!isServerFolderOpen)
            {
                groupBoxFolder.Text = JIRAFolderOpener.Properties.Resources.Txt_Switch_Server;
                btnSwitch.Text = JIRAFolderOpener.Properties.Resources.Txt_Switch_Local;
                btnSwitch.TextAlign = ContentAlignment.MiddleCenter;
                fileList.Clear();
                LoadFileList(LocalFilePath.SourceFilePath);
                isServerFolderOpen = true;
            }
            else
            {
                groupBoxFolder.Text = JIRAFolderOpener.Properties.Resources.Txt_Switch_Local;
                btnSwitch.Text = JIRAFolderOpener.Properties.Resources.Txt_Switch_Server;
                btnSwitch.TextAlign = ContentAlignment.MiddleCenter;
                fileList.Clear();
                LoadFileList(targetFolder);
                isServerFolderOpen = false;
            }
            btnRestore.Visible = !isServerFolderOpen;
            btnRestore.Enabled = false; // !isServerFolderOpen;
        }

        private void btnMergeFiles_Click(object sender, EventArgs e)
        {
            // Reload folder
        }

        private void setDefaultEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"C:\";

            openFileDialog1.Title = "Browse ExE Files";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "exe";

            openFileDialog1.Filter = "Text files (*.exe)|*.exe|All files (*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                FileOperator.Editor = openFileDialog1.FileName ;
            }
        }

        #endregion

        #region Data View Remote
        private void AddorRemoveItem(string fileName)
        {
            if (fileList.Find(file => file.Equals(fileName)) != null)
            {
                Console.WriteLine("Remove from the list {0}", fileName);
                fileList.Remove(fileName);
            }
            else
            {
                Console.WriteLine("Add into the list {0}", fileName);
                fileList.Add(fileName);
            }
        }

        private void dataView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataViewRemote.CurrentCell is DataGridViewCheckBoxCell)
                {
                    int row = e.RowIndex;
                    if (row >= 0)
                    {
                        // check the checkbox value
                        string fileFullName = dataViewRemote.Rows[row].Cells[FULL_PATH_INDEX + 1].Value.ToString();
                        if (dataViewRemote.Rows[row].Cells[NAME_INDEX].Value != null)
                        {
                            if ((bool)dataViewRemote.Rows[row].Cells[NAME_INDEX].Value == false)
                            {
                                dataViewRemote.Rows[row].Cells[NAME_INDEX].Value = true;
                                Console.WriteLine("Add {0}", fileFullName);
                                AddorRemoveItem(fileFullName);
                            }
                            else
                            {
                                dataViewRemote.Rows[row].Cells[NAME_INDEX].Value = false;
                                Console.WriteLine("remove {0}", fileFullName);
                                AddorRemoveItem(fileFullName);
                            }
                        }
                        else
                        {
                            dataViewRemote.Rows[row].Cells[0].Value = true;
                            Console.WriteLine("Add {0} ---- null cell", fileFullName);
                            AddorRemoveItem(fileFullName);
                        }
                    }
                }
                Console.WriteLine("not a check box");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
                return;
            }
        }

        private void dataView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            int row = e.RowIndex;
            if (row >= 0)
            {
                dataViewRemote.Rows[row].Selected = true;
            }
        }

        private void dataView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            if (row >= 0)
            {
                dataViewRemote.Rows[row].Selected = false;
            }
        }
#endregion

        private void richtxtActivity_TextChanged(object sender, EventArgs e)
        {
            richtxtActivity.SelectionStart = richtxtActivity.Text.Length;
            richtxtActivity.ScrollToCaret();
        }

        private void openWithDefaultEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openWithDefaultEditorToolStripMenuItem.Checked)
                openWithEditor = true;
            else
                openWithEditor = false;
        }

        /// <summary>
        /// Copy the selected db to C:\EnablerdB, check backup and restore the db then adding user role
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            // open a dialog for selecting the database
            using (var files = new OpenFileDialog())
            {
                files.Filter = "Enabler Database File |*.*|*.dmp";
                files.Title = "Select a Enabler Database";
                files.InitialDirectory = this.targetFolder;
                DialogResult result = files.ShowDialog();
                
                if(result == DialogResult.OK)
                {
                    filePath = files.FileName;
                }
            }

            // copy the database and add the rename mapping into a text file
            if(File.Exists(filePath))
            {
                // Copy to EnablerDB folder with adding Date and time as suffix
                string fileNewName = string.Format("{0}_EnablerDB_{1}.dmp", FileOperator.CaseNumber, DateTime.Now.Ticks);

                string JIRAMappingPath = Path.Combine(LocalFilePath.EnablerDBPath, LocalFilePath.JIRAMappingFileName);
                if(!File.Exists(JIRAMappingPath))
                {
                    File.Create(JIRAMappingPath).Close();
                }

                // move other db files into DB Backup folder with replacement
                string[] dmpFiles = Directory.GetFiles(LocalFilePath.EnablerDBPath, "*.dmp");
                if(dmpFiles.Length > 0)
                {
                    //move them to DB backup folder
                    foreach(string fileName in dmpFiles)
                    {
                        string tempFileName = Path.Combine(Path.Combine(LocalFilePath.EnablerDBPath, "DB backup"), Path.GetFileName(fileName));

                        if (File.Exists(tempFileName))
                        {
                            // TODO: check the target folder, if the file is the same and already in the target folder, delete it from the source path
                            continue;
                        }
                            
                        try
                        {
                            File.Move(fileName, tempFileName);
                            UpdateActivityMessage(string.Format("Moved {0} to db backup folder", fileName));
                        }
                        catch(Exception ex)
                        {
                            UpdateActivityMessage(string.Format("Unable to moved {0} to db backup folder: {1}", fileName, ex.Message));
                        }
                    }
                }

                Task copyToPath = CopyTask(filePath, Path.Combine(LocalFilePath.EnablerDBPath, fileNewName));

                copyToPath.ContinueWith((task) =>
                {
                    UpdateActivityMessage("Copying...");
                    if(task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                    {
                        UpdateActivityMessage(String.Format("DB file {0} copied to Enablerdb folder", filePath));

                        #region Pending Section
                        
                        // open and add the mapping entry
                        using (StreamWriter writer = new StreamWriter(JIRAMappingPath, true))
                        {
                            writer.WriteLine(string.Format("Ori File: {0} - New File: {1}", filePath, fileNewName));
                        }

                        // TODO: The batch is not running as expected, need to check why.
                        // restore the database
                        using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                        {
                            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                            //process.StartInfo = new System.Diagnostics.ProcessStartInfo()
                            //process.StartInfo.WorkingDirectory = @"C:\Enabler";
                            process.StartInfo.FileName = @"C:\\Enabler\\EnablerRestore.bat";
                            //process.StartInfo.Arguments = string.Format("{0}", "C:\\Enabler\\EnablerRestore.bat");
                            //process.StartInfo.FileName = "C:\\Enabler\\EnablerRestore.bat";
                            process.StartInfo.Arguments = fileNewName;
                            process.StartInfo.RedirectStandardInput = false;
                            process.StartInfo.RedirectStandardOutput = false;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            process.WaitForExit();

                            UpdateActivityMessage("Enabler Database restored");
                        }

                        // System.Diagnostics.Process.Start("C:\\Enabler\\EnablerRestore.bat");

                        // insert user role
                        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                        {
                            conn.ConnectionString = @"Data Source=.;Database=EnablerDB;Trusted_Connection=True;";
                            conn.Open();
                            string commandText = @"UPDATE USERS set Login_Name='admin', Password='21232f297a57a5a743894a0e4a801fc3', Role_ID = 1 where User_ID =1;";
                            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                            UpdateActivityMessage("Admin role added into the restored database");
                        }

                        #endregion
                    }
                    else
                    {
                        UpdateActivityMessage("Copy failed");
                    }
                });
            }
        }

        Task CopyTask(string sourcePath, string DesPath)
        {
            return Task.Factory.StartNew(()=>
            {
                JIRAFile.Copy(sourcePath, DesPath);
            });
        }

        private void checkDeliveryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MeterCheckForm CheckDelForm = new MeterCheckForm();
            
            CheckDelForm.Show(this);
        }

       
    }
}
