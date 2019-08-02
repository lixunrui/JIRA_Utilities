namespace JIRASupport
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtMsg = new System.Windows.Forms.RichTextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.txtJiraCaseNum = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtMsg
            // 
            this.txtMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMsg.Location = new System.Drawing.Point(0, 29);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(284, 100);
            this.txtMsg.TabIndex = 0;
            this.txtMsg.Text = "";
            // 
            // btnRegister
            // 
            this.btnRegister.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRegister.Location = new System.Drawing.Point(0, 0);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(284, 23);
            this.btnRegister.TabIndex = 1;
            this.btnRegister.Text = "Register Keys";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtJiraCaseNum
            // 
            this.txtJiraCaseNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJiraCaseNum.Location = new System.Drawing.Point(12, 135);
            this.txtJiraCaseNum.Name = "txtJiraCaseNum";
            this.txtJiraCaseNum.Size = new System.Drawing.Size(153, 20);
            this.txtJiraCaseNum.TabIndex = 2;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(187, 135);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(85, 22);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(12, 173);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(41, 13);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 195);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtJiraCaseNum);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.txtMsg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JIRA Assistance";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClosingMainFormEvent);
            this.Resize += new System.EventHandler(this.ResizeMainFormEvent);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtMsg;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.TextBox txtJiraCaseNum;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label lblVersion;
    }
}

