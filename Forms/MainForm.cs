using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace JIRASupport
{
    public partial class MainForm : Form
    {
        #region Import Dlls
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        #endregion

        private JIRASupport.Utilities.Utilities utilities;

        private IntPtr currentActiveWindow;

        private System.Windows.Forms.NotifyIcon notifyIcon;

        bool canViewMainForm;

        string JIRADirectory;

        //DownloadForm downloadForm; // only one instance is allowed, otherwise, some odd errors will happen
        DownloadFormByDataTable downloadForm;
        YLog log;

        public MainForm()
        {
            // Auto code
            InitializeComponent();

            log = new YLog("JIRA");

            utilities = new Utilities.Utilities();

            LogLine(utilities.testString);

            if(utilities.FtpConnectionStatus == false)
            {
                LogLine("Unable to read xml");
            }
            utilities.MessagerEvent += UtilitiesMessagerEvent;

            DisplayVersion();

            // by default, the form is hidden
            canViewMainForm = false;

            this.components = new System.ComponentModel.Container();

            RegisterHotKeys();

            GenerateIcon();

            //TODO: Testing watcher function
            DirectoryWatcher watcher = new DirectoryWatcher("C:\\Enabler\\log", log);
            // TODO:
            // watcher.EnableWatcher = true;
        }

        #region HotKey Register and Function
        private void RegisterHotKeys()
        {
            bool buttonVisible = true;
            buttonVisible &= utilities.AddHotKey(Constants.ALT + Constants.SHIFT, Keys.F, this.Handle);
            buttonVisible &= utilities.AddHotKey(Constants.ALT + Constants.SHIFT, Keys.D, this.Handle);
            buttonVisible &= utilities.AddHotKey(Constants.ALT + Constants.SHIFT, Keys.T, this.Handle);

            this.BeginInvoke((Action)(() =>
            {
                btnRegister.Enabled = buttonVisible;
            }));
        }

        protected override void WndProc(ref Message m)
        {
            // https://msdn.microsoft.com/en-us/library/ms646279(v=vs.85).aspx     - LParam: 0x004f0005; WParam: 0x000b0a73
            /*
             MOD_ALT0x0001 Either ALT key was held down.
 
             MOD_CONTROL0x0002 Either CTRL key was held down.
 
             MOD_SHIFT0x0004 Either SHIFT key was held down.
 
             MOD_WIN0x0008 Either WINDOWS key was held down. 
             * 
             * These keys are labeled with the Windows logo. 
             * Hot keys that involve the Windows key are reserved for use by the operating system.
            */

            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                // find out which key(s) is (are) pressed
                int keys = m.LParam.ToInt32() >> 16;
                int systemKeys = m.LParam.ToInt32() & 0xFF;
                if ((systemKeys & Constants.SHIFT) == Constants.SHIFT
                    && (systemKeys & Constants.ALT) == Constants.ALT)
                {
                    if (keys == Convert.ToInt32(Keys.F))
                    {
                        OpenFolder();
                    }
                    else if (keys == Convert.ToInt32(Keys.D))
                    {
                        OpenFolder();
                        ShowFileDialog();
                    }
                    else if (keys == Convert.ToInt32(Keys.T))
                    {
                        SetWindowsOnTop();
                    }
                }
            }
            base.WndProc(ref m);
        }

        private void SetWindowsOnTop()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            string returnString = null;
            currentActiveWindow = GetForegroundWindow();

            if (GetWindowText(currentActiveWindow, buff, nChars) > 0)
            {
                returnString = buff.ToString();
                LogLine("Windows Title is {0}", returnString);
            }
            else if (returnString == null)
            {
                currentActiveWindow = GetActiveWindow();
                if (GetWindowText(currentActiveWindow, buff, nChars) > 0)
                {
                    returnString = buff.ToString();
                    LogLine("Windows Title is {0}", returnString);
                }
            }
            
            if(returnString  == null)
            {
                LogLine("Cannot find Foreground Window");
                LogLine(Form.ActiveForm.Text);
            }

            if (returnString != null)
            {
                Form form = (Form)Control.FromHandle(currentActiveWindow);
                if (form != null)
                {
                    form.TopMost = true;
                }
            }
        }

        // true to make the control visible; otherwise, false.
        protected override void SetVisibleCore(bool value)
        {
            if (!canViewMainForm)
            {
                value = false;
                if (!this.IsHandleCreated)
                {
                    CreateHandle();
                }
            }
            base.SetVisibleCore(value);
        }

        private string GetActiveWindowsTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            string returnString = null;
            currentActiveWindow = GetForegroundWindow();

            if (GetWindowText(currentActiveWindow, buff, nChars) > 0)
            {
                returnString = buff.ToString(); 
                LogLine("Windows Title is {0}", returnString);
            }
            else
            {
                LogLine("Cannot find Foreground Window");
            }
            //currentActiveWindow = GetActiveWindow();
            //if (GetWindowText(currentActiveWindow, buff, nChars) > 0)
            //{
            //    returnString = buff.ToString(); 
            //    WriteLine("Windows Title for handle 2 is {0}", returnString);
            //}

            return null;
        }

        #endregion

        #region Create Icon and Click Events
        private void GenerateIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);

            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            notifyIcon.Visible = true;

            notifyIcon.Text = "JIRA Support Tool";

            notifyIcon.ContextMenu = GenerateIconMenu();
        }

        private ContextMenu GenerateIconMenu()
        {
            ContextMenu iconMenu = new ContextMenu();

            MenuItem showFilePickerOne = new MenuItem();
            showFilePickerOne.Text = "Open Folder (ALT+SHIFT+F)";
            showFilePickerOne.Index = 0;
            showFilePickerOne.Click += OpenFolder_Click;

            MenuItem showFilePickerTow = new MenuItem();
            showFilePickerTow.Text = "Open Folder & App (ALT+SHIFT+D)";
            showFilePickerTow.Index = 1;
            showFilePickerTow.Click += ShowFileList_Click;

            MenuItem windowsOnTopMenu = new MenuItem();
            windowsOnTopMenu.Text = "(Pending)Set Window On Top (ALT+SHIFT+T)";
            windowsOnTopMenu.Index = 2;
            windowsOnTopMenu.Click += windowsOnTopMenu_Click;

            iconMenu.MenuItems.AddRange( 
                new MenuItem[] 
                {
                    showFilePickerOne,
                    showFilePickerTow,
                    windowsOnTopMenu,
                });
            iconMenu.MenuItems.Add("-");

            MenuItem ExitOption = new MenuItem();
            ExitOption.Text = "Exit";
            ExitOption.Index = 3;
            ExitOption.Click += Exit_Click;

            iconMenu.MenuItems.Add(ExitOption);

            return iconMenu;
        }

        void windowsOnTopMenu_Click(object sender, EventArgs e)
        {
            SetWindowsOnTop();
        }

        void ShowFileList_Click(object sender, EventArgs e)
        {
            ShowFileDialog();
        }

        void OpenFolder_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            canViewMainForm = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            txtJiraCaseNum.Focus();
            this.CenterToScreen();
        }
        #endregion

        #region Button Click Event
        void Exit_Click(object sender, EventArgs e)
        {
            utilities.RemoveHotKey();
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // unregister all keys first
            utilities.RemoveHotKey();

            RegisterHotKeys();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            // get number from text JIRA box
            string caseNumStr = txtJiraCaseNum.Text;

            FileOperator.OpenFile(caseNumStr, out caseNumStr);

            txtJiraCaseNum.Clear();
        }
        #endregion

        #region Actions
        private void ShowFileDialog()
        {
            JIRADirectory = utilities.GetTargetPath();
            //if (downloadForm == null)
            //{
                //downloadForm = new DownloadForm(JIRADirectory, log);//, fileoperator);
            downloadForm = new DownloadFormByDataTable(JIRADirectory, log);
            //}
            //else
            //{
            //    // Only update the folder
            //    downloadForm.JIRADirectory = JIRADirectory;
            //}

            downloadForm.Show(this);
            downloadForm.Activate();
        }

        private void OpenFolder()
        {
            string caseNum = URLExtractor.GetJIRANumberFromURL();
            if (caseNum == null)
            {
                LogLine("No JIRA case found");
                MessageBox.Show("Folder location invalid", "No JIRA # found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogLine("Found case #: {0}", caseNum);
            FileOperator.OpenFile(caseNum, out JIRADirectory);
        }
        #endregion     

        #region Event
        private void ClosingMainFormEvent(object sender, FormClosingEventArgs e)
        {
            utilities.RemoveHotKey();
        }

        private void ResizeMainFormEvent(object sender, EventArgs e)
        {
            FireIconNotification(this.WindowState, "Minimize to Tray App", "Form Minimized");
        }

        // to be used by other operations
        private void FireIconNotification(FormWindowState state, string titleMsg, string textmsg)
        {
            notifyIcon.BalloonTipTitle = titleMsg;
            notifyIcon.BalloonTipText = textmsg;

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void UtilitiesMessagerEvent(MessageType msgType, string message)
        {
            LogLine(message);
            if (msgType == MessageType.ERROR)
            {
                FireIconNotification(this.WindowState, "Error", message);
            }
        }
        #endregion

        #region Support Functions
        private void DisplayVersion()
        {
            lblVersion.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
            LogLine("Version: {0}", Application.ProductVersion);
        }

        private void LogLine(string message, params string[] args)
        {
            if (args.Length > 0)
                message = String.Format(message, args);

            log.WriteLine(message);
            message = DateTime.Now + " " + message;
            txtMsg.Text += message + Environment.NewLine;
        }
        #endregion
    }
}