using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace JIRASupport
{
    public partial class MainForm1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private KeyHandler openFolderHotKey;

        private KeyHandler downloadFileHotKey;

        private KeyHandler windowOnTopHotKey;

        private HotKeyController KeyController;

        private IntPtr currentActiveWindow;

        private System.Windows.Forms.NotifyIcon notifyIcon1;

        bool allowVisible;

        string targetPath;

        bool registerStatus = false;

        FilePicker filePicker; // only one instance is allowed, otherwise, some odd errors will happen
        FileManager fileManager;
        YLog log;

        public MainForm1()
        {
            InitializeComponent();

            log = new YLog("JIRA");

            DisplayVersion();

            allowVisible = false;

            this.components = new System.ComponentModel.Container();

            KeyController = new HotKeyController();

            GenerateIcon();
            
            // Open folder only
            openFolderHotKey = new KeyHandler(Constants.ALT + Constants.SHIFT, Keys.F, this.Handle);

            KeyController.AddHotKey(openFolderHotKey);

            // Open folder and the application
            downloadFileHotKey = new KeyHandler(Constants.ALT + Constants.SHIFT, Keys.D, this.Handle);

            KeyController.AddHotKey(downloadFileHotKey);

            windowOnTopHotKey = new KeyHandler(Constants.ALT + Constants.SHIFT, Keys.T, this.Handle);

            KeyController.AddHotKey(windowOnTopHotKey);

            KeyController.RegisterHotKeyEvent += KeyController_RegisterHotKeyEvent;

            KeyController.RegisterHotKey();

            lblVersion.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();

            // loads everything on startup
            FTPParameters.LoadServerInfoFromFile();


            //TODO: Testing watcher function
            DirectoryWatcher watcher = new DirectoryWatcher("C:\\Enabler\\log", log);
            watcher.EnableWatcher = true;

#if TEST
            RunTest();
#endif
        }

        #region HotKey Register and Function
        void KeyController_RegisterHotKeyEvent(KeyHandler key, RegisterType type)
        {
            string buttonText = "Start";
            if (type == RegisterType.ADD)
            {
                buttonText = "Stop";
                WriteLine("Hotkey registered.");
                registerStatus = true;
            }
            else if (type == RegisterType.REMOVEALL)
            {
                WriteLine("Hotkey unregistered.");
                registerStatus = false;
            }
                
            this.BeginInvoke((Action)(() =>
            {
                btnStart.Text = buttonText;
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
             * Hotkeys that involve the Windows key are reserved for use by the operating system.
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
                        HandleHotKey();
                    }
                    else if (keys == Convert.ToInt32(Keys.D))
                    {
                        HandleHotKey(true);
                    }
                    else if (keys == Convert.ToInt32(Keys.T))
                    {
                        SetWindowsOnTop();
                    }
                }
            }
            base.WndProc(ref m);
        }

        private void HandleHotKey(bool download = false)
        {
            OpenFolder();

            if (download)
            {
                ShowFilePicker();
            }
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
                WriteLine("Windows Title is {0}", returnString);
            }
            else if (returnString == null)
            {
                currentActiveWindow = GetActiveWindow();
                if (GetWindowText(currentActiveWindow, buff, nChars) > 0)
                {
                    returnString = buff.ToString();
                    WriteLine("Windows Title is {0}", returnString);
                }
            }
            
            if(returnString  == null)
            {
                WriteLine("Cannot find Foreground Window");
                WriteLine(Form.ActiveForm.Text);
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

        private string GetActiveWindowsTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            string returnString = null;
            currentActiveWindow = GetForegroundWindow();

            if (GetWindowText(currentActiveWindow, buff, nChars) > 0)
            {
                returnString = buff.ToString(); 
                WriteLine("Windows Title is {0}", returnString);
            }
            else
            {
                WriteLine("Cannot find Foreground Window");
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
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            notifyIcon1.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;

            notifyIcon1.Visible = true;

            notifyIcon1.Text = "JIRA Support Tool";

            notifyIcon1.ContextMenu = GenerateIconMenu();
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
            ShowFilePicker();
        }

        void OpenFolder_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            allowVisible = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            txtJiraCaseNum.Focus();
            this.CenterToScreen();
        }
        #endregion

        #region Button Click Event
        void Exit_Click(object sender, EventArgs e)
        {
            KeyController.UnregisterAllHotKey();
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (registerStatus)
            {
                KeyController.UnregisterAllHotKey();
            }
            else
                KeyController.RegisterHotKey();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            // get number from text JIRA box
            string caseNumStr = txtJiraCaseNum.Text;

            FileOperator.OpenFile(caseNumStr, out caseNumStr);

            txtJiraCaseNum.Text = "";
        }
        #endregion

        #region Actions
        private void ShowFilePicker()
        {
            if (targetPath == null)
            {
                FileOperator.OpenFile(URLExtractor.GetJIRANumberFromURL(), out targetPath);
                if (targetPath == null)
                {
                    MessageBox.Show("Folder location invalid", "No JIRA # found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            /*
            if (filePicker == null)
            {
                filePicker = new FilePicker(targetPath, log);//, fileoperator);
                //filePicker.TopMost = true;
                filePicker.FormClosing += filePicker_FormClosing;
                
                filePicker.Show(this);
                filePicker.Activate();
            }
            */
            if (fileManager == null)
            {
                fileManager = new FileManager(targetPath, log);//, fileoperator);
                //filePicker.TopMost = true;
                fileManager.FormClosing += filePicker_FormClosing;

                fileManager.Show(this);
                fileManager.Activate();
            }
        }

        void filePicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            filePicker = null;
            fileManager = null;
        }

        private void OpenFolder()
        {
            string caseNum = URLExtractor.GetJIRANumberFromURL();
            if (caseNum == "" || caseNum == null)
            {
                WriteLine("No JIRA case found");
                MessageBox.Show("Folder location invalid", "No JIRA # found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            caseNum = caseNum.Substring(1, caseNum.Length - 2);
            WriteLine("Found case #: {0}", caseNum);
            FileOperator.OpenFile(caseNum, out targetPath);
        }
        #endregion     

        #region Form Event
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyController.UnregisterAllHotKey();
            //this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyController.RegisterHotKey();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            FireIconNotification(this.WindowState, "Minimize to Tray App", "Form Minimized");
        }

        // to be used by other operations
        private void FireIconNotification(FormWindowState state, string titleMsg, string textmsg)
        {
            notifyIcon1.BalloonTipTitle = titleMsg;
            notifyIcon1.BalloonTipText = textmsg;

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }
        #endregion

        // true to make the control visible; otherwise, false.
        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated)
                {
                    CreateHandle();
                }
            }
            base.SetVisibleCore(value);
        }

        #region Support Functions
        private void DisplayVersion()
        {
            WriteLine("Version: {0}",Application.ProductVersion);
        }

        private void WriteLine(string text, params string[] args)
        {
            WriteLine(String.Format(text, args));
        }

        private void WriteLine(string text)
        {
            text = DateTime.Now + " " + text;
            txtMsg.Text += text + Environment.NewLine;
            log.WriteLine(text);
        }
        #endregion


        #region Test Function
        public void RunTest()
        {
            MeterCheckForm testCheckDelForm = new MeterCheckForm("C:\\Enabler\\Log", log);
            testCheckDelForm.ShowDialog();
        }
        #endregion
    }
}
