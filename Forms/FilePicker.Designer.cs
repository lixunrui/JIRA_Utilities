namespace JIRAFolderOpener
{
    partial class FilePicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilePicker));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.dataViewRemote = new System.Windows.Forms.DataGridView();
            this.ckbDecode = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBoxFolder = new System.Windows.Forms.GroupBox();
            this.btnMergeFiles = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBoxActivity = new System.Windows.Forms.GroupBox();
            this.richtxtActivity = new System.Windows.Forms.RichTextBox();
            this.ckbUnzip = new System.Windows.Forms.CheckBox();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWithDefaultEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkDeliveryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDefaultEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataViewRemote)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBoxFolder.SuspendLayout();
            this.groupBoxActivity.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(12, 511);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Exit";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(491, 511);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(81, 30);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // dataViewRemote
            // 
            this.dataViewRemote.AllowUserToAddRows = false;
            this.dataViewRemote.AllowUserToDeleteRows = false;
            this.dataViewRemote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataViewRemote.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataViewRemote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataViewRemote.Location = new System.Drawing.Point(0, 46);
            this.dataViewRemote.Name = "dataViewRemote";
            this.dataViewRemote.ReadOnly = true;
            this.dataViewRemote.Size = new System.Drawing.Size(584, 300);
            this.dataViewRemote.TabIndex = 3;
            this.dataViewRemote.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataView_CellContentClick);
            this.dataViewRemote.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataView_CellMouseLeave);
            this.dataViewRemote.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataView_CellMouseMove);
            // 
            // ckbDecode
            // 
            this.ckbDecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbDecode.AutoSize = true;
            this.ckbDecode.Location = new System.Drawing.Point(405, 520);
            this.ckbDecode.Name = "ckbDecode";
            this.ckbDecode.Size = new System.Drawing.Size(64, 17);
            this.ckbDecode.TabIndex = 4;
            this.ckbDecode.Text = "Decode";
            this.ckbDecode.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 540);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(584, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusProgress
            // 
            this.statusProgress.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.statusProgress.Name = "statusProgress";
            this.statusProgress.Size = new System.Drawing.Size(560, 16);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // groupBoxFolder
            // 
            this.groupBoxFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFolder.Controls.Add(this.btnMergeFiles);
            this.groupBoxFolder.Controls.Add(this.btnRestore);
            this.groupBoxFolder.Controls.Add(this.lblFileCount);
            this.groupBoxFolder.Controls.Add(this.btnSwitch);
            this.groupBoxFolder.Controls.Add(this.btnRefresh);
            this.groupBoxFolder.Location = new System.Drawing.Point(0, 27);
            this.groupBoxFolder.Name = "groupBoxFolder";
            this.groupBoxFolder.Size = new System.Drawing.Size(584, 352);
            this.groupBoxFolder.TabIndex = 7;
            this.groupBoxFolder.TabStop = false;
            this.groupBoxFolder.Text = "Server Folder";
            // 
            // btnMergeFiles
            // 
            this.btnMergeFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMergeFiles.Location = new System.Drawing.Point(114, 326);
            this.btnMergeFiles.Name = "btnMergeFiles";
            this.btnMergeFiles.Size = new System.Drawing.Size(69, 25);
            this.btnMergeFiles.TabIndex = 4;
            this.btnMergeFiles.Text = "Merge Files";
            this.btnMergeFiles.UseVisualStyleBackColor = true;
            this.btnMergeFiles.Click += new System.EventHandler(this.btnMergeFiles_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.Location = new System.Drawing.Point(405, 327);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 23);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "RestoreDB";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // lblFileCount
            // 
            this.lblFileCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Location = new System.Drawing.Point(88, 353);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(0, 13);
            this.lblFileCount.TabIndex = 2;
            // 
            // btnSwitch
            // 
            this.btnSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSwitch.Location = new System.Drawing.Point(18, 325);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(75, 27);
            this.btnSwitch.TabIndex = 1;
            this.btnSwitch.Text = "Local Folder";
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(491, 327);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(81, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBoxActivity
            // 
            this.groupBoxActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxActivity.Controls.Add(this.richtxtActivity);
            this.groupBoxActivity.Location = new System.Drawing.Point(0, 385);
            this.groupBoxActivity.Name = "groupBoxActivity";
            this.groupBoxActivity.Size = new System.Drawing.Size(584, 120);
            this.groupBoxActivity.TabIndex = 8;
            this.groupBoxActivity.TabStop = false;
            this.groupBoxActivity.Text = "Activities";
            // 
            // richtxtActivity
            // 
            this.richtxtActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richtxtActivity.Location = new System.Drawing.Point(6, 19);
            this.richtxtActivity.Name = "richtxtActivity";
            this.richtxtActivity.ReadOnly = true;
            this.richtxtActivity.Size = new System.Drawing.Size(572, 95);
            this.richtxtActivity.TabIndex = 0;
            this.richtxtActivity.Text = "";
            this.richtxtActivity.TextChanged += new System.EventHandler(this.richtxtActivity_TextChanged);
            // 
            // ckbUnzip
            // 
            this.ckbUnzip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbUnzip.AutoSize = true;
            this.ckbUnzip.Location = new System.Drawing.Point(346, 519);
            this.ckbUnzip.Name = "ckbUnzip";
            this.ckbUnzip.Size = new System.Drawing.Size(53, 17);
            this.ckbUnzip.TabIndex = 9;
            this.ckbUnzip.Text = "Unzip";
            this.ckbUnzip.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openWithDefaultEditorToolStripMenuItem,
            this.checkDeliveryToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openWithDefaultEditorToolStripMenuItem
            // 
            this.openWithDefaultEditorToolStripMenuItem.CheckOnClick = true;
            this.openWithDefaultEditorToolStripMenuItem.Name = "openWithDefaultEditorToolStripMenuItem";
            this.openWithDefaultEditorToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openWithDefaultEditorToolStripMenuItem.Text = "Open With Default Editor";
            this.openWithDefaultEditorToolStripMenuItem.Click += new System.EventHandler(this.openWithDefaultEditorToolStripMenuItem_Click);
            // 
            // checkDeliveryToolStripMenuItem
            // 
            this.checkDeliveryToolStripMenuItem.Name = "checkDeliveryToolStripMenuItem";
            this.checkDeliveryToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.checkDeliveryToolStripMenuItem.Text = "Check Delivery";
            this.checkDeliveryToolStripMenuItem.Click += new System.EventHandler(this.checkDeliveryToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setDefaultEditorToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // setDefaultEditorToolStripMenuItem
            // 
            this.setDefaultEditorToolStripMenuItem.Name = "setDefaultEditorToolStripMenuItem";
            this.setDefaultEditorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.setDefaultEditorToolStripMenuItem.Text = "Set default Editor";
            this.setDefaultEditorToolStripMenuItem.Click += new System.EventHandler(this.setDefaultEditorToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FilePicker
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.ckbUnzip);
            this.Controls.Add(this.groupBoxActivity);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.ckbDecode);
            this.Controls.Add(this.dataViewRemote);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBoxFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FilePicker";
            this.Text = "File Picker";
            ((System.ComponentModel.ISupportInitialize)(this.dataViewRemote)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxFolder.ResumeLayout(false);
            this.groupBoxFolder.PerformLayout();
            this.groupBoxActivity.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.DataGridView dataViewRemote;
        private System.Windows.Forms.CheckBox ckbDecode;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.GroupBox groupBoxFolder;
        private System.Windows.Forms.GroupBox groupBoxActivity;
        private System.Windows.Forms.RichTextBox richtxtActivity;
        private System.Windows.Forms.CheckBox ckbUnzip;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setDefaultEditorToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWithDefaultEditorToolStripMenuItem;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.ToolStripMenuItem checkDeliveryToolStripMenuItem;
        private System.Windows.Forms.Button btnMergeFiles;
    }
}