namespace JammaryBackup
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
            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.toolStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolBackupStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain = new System.Windows.Forms.Panel();
            this.buttonCancelBackup = new System.Windows.Forms.Button();
            this.groupJobHistory = new System.Windows.Forms.GroupBox();
            this.gridJobHistory = new System.Windows.Forms.DataGridView();
            this.buttonBackupStart = new System.Windows.Forms.Button();
            this.groupBackupDirectories = new System.Windows.Forms.GroupBox();
            this.buttonRemoveAll = new System.Windows.Forms.Button();
            this.buttonRemoveSelected = new System.Windows.Forms.Button();
            this.labelBackupDirectories = new System.Windows.Forms.Label();
            this.buttonBackupDirectories = new System.Windows.Forms.Button();
            this.textBackupDirectores = new System.Windows.Forms.TextBox();
            this.checkedBackupDirectories = new System.Windows.Forms.CheckedListBox();
            this.groupBackupType = new System.Windows.Forms.GroupBox();
            this.radioBackupTypeIncremental = new System.Windows.Forms.RadioButton();
            this.radioBackupTypeDifferential = new System.Windows.Forms.RadioButton();
            this.radioBackupTypeFull = new System.Windows.Forms.RadioButton();
            this.groupBackupLocation = new System.Windows.Forms.GroupBox();
            this.labelBackupLocation = new System.Windows.Forms.Label();
            this.buttonBackupLocation = new System.Windows.Forms.Button();
            this.textBackupLocation = new System.Windows.Forms.TextBox();
            this.browserBackupLocation = new System.Windows.Forms.FolderBrowserDialog();
            this.browserBackupDirectories = new System.Windows.Forms.FolderBrowserDialog();
            this.threadBackup = new System.ComponentModel.BackgroundWorker();
            this.statusMain.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.groupJobHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridJobHistory)).BeginInit();
            this.groupBackupDirectories.SuspendLayout();
            this.groupBackupType.SuspendLayout();
            this.groupBackupLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusMain
            // 
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStatus,
            this.toolProgress,
            this.toolBackupStatus});
            this.statusMain.Location = new System.Drawing.Point(0, 538);
            this.statusMain.Name = "statusMain";
            this.statusMain.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusMain.Size = new System.Drawing.Size(990, 26);
            this.statusMain.TabIndex = 0;
            this.statusMain.Text = "statusMain";
            // 
            // toolStatus
            // 
            this.toolStatus.Name = "toolStatus";
            this.toolStatus.Size = new System.Drawing.Size(50, 21);
            this.toolStatus.Text = "Ready";
            // 
            // toolProgress
            // 
            this.toolProgress.Name = "toolProgress";
            this.toolProgress.Size = new System.Drawing.Size(100, 20);
            // 
            // toolBackupStatus
            // 
            this.toolBackupStatus.Name = "toolBackupStatus";
            this.toolBackupStatus.Size = new System.Drawing.Size(0, 21);
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.backupToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuMain.Size = new System.Drawing.Size(990, 28);
            this.menuMain.TabIndex = 1;
            this.menuMain.Text = "menuMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // backupToolStripMenuItem
            // 
            this.backupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelBackupToolStripMenuItem,
            this.startBackupToolStripMenuItem});
            this.backupToolStripMenuItem.Name = "backupToolStripMenuItem";
            this.backupToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.backupToolStripMenuItem.Text = "Backup";
            // 
            // cancelBackupToolStripMenuItem
            // 
            this.cancelBackupToolStripMenuItem.Name = "cancelBackupToolStripMenuItem";
            this.cancelBackupToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.cancelBackupToolStripMenuItem.Text = "Cancel Backup";
            this.cancelBackupToolStripMenuItem.Visible = false;
            this.cancelBackupToolStripMenuItem.Click += new System.EventHandler(this.cancelBackupToolStripMenuItem_Click);
            // 
            // startBackupToolStripMenuItem
            // 
            this.startBackupToolStripMenuItem.Name = "startBackupToolStripMenuItem";
            this.startBackupToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.startBackupToolStripMenuItem.Text = "Start Backup";
            this.startBackupToolStripMenuItem.Click += new System.EventHandler(this.startBackupToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(119, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.buttonCancelBackup);
            this.panelMain.Controls.Add(this.groupJobHistory);
            this.panelMain.Controls.Add(this.buttonBackupStart);
            this.panelMain.Controls.Add(this.groupBackupDirectories);
            this.panelMain.Controls.Add(this.groupBackupType);
            this.panelMain.Controls.Add(this.groupBackupLocation);
            this.panelMain.Location = new System.Drawing.Point(0, 31);
            this.panelMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(990, 507);
            this.panelMain.TabIndex = 2;
            // 
            // buttonCancelBackup
            // 
            this.buttonCancelBackup.Location = new System.Drawing.Point(493, 466);
            this.buttonCancelBackup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonCancelBackup.Name = "buttonCancelBackup";
            this.buttonCancelBackup.Size = new System.Drawing.Size(113, 26);
            this.buttonCancelBackup.TabIndex = 9;
            this.buttonCancelBackup.Text = "Cancel Backup";
            this.buttonCancelBackup.UseVisualStyleBackColor = true;
            this.buttonCancelBackup.Visible = false;
            this.buttonCancelBackup.Click += new System.EventHandler(this.buttonCancelBackup_Click);
            // 
            // groupJobHistory
            // 
            this.groupJobHistory.Controls.Add(this.gridJobHistory);
            this.groupJobHistory.Location = new System.Drawing.Point(292, 14);
            this.groupJobHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupJobHistory.Name = "groupJobHistory";
            this.groupJobHistory.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupJobHistory.Size = new System.Drawing.Size(695, 434);
            this.groupJobHistory.TabIndex = 8;
            this.groupJobHistory.TabStop = false;
            this.groupJobHistory.Text = "Job History";
            // 
            // gridJobHistory
            // 
            this.gridJobHistory.AllowUserToAddRows = false;
            this.gridJobHistory.AllowUserToDeleteRows = false;
            this.gridJobHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridJobHistory.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.gridJobHistory.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.gridJobHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridJobHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridJobHistory.Location = new System.Drawing.Point(3, 17);
            this.gridJobHistory.Name = "gridJobHistory";
            this.gridJobHistory.ReadOnly = true;
            this.gridJobHistory.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridJobHistory.RowTemplate.Height = 24;
            this.gridJobHistory.Size = new System.Drawing.Size(689, 415);
            this.gridJobHistory.TabIndex = 1;
            // 
            // buttonBackupStart
            // 
            this.buttonBackupStart.Location = new System.Drawing.Point(384, 466);
            this.buttonBackupStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonBackupStart.Name = "buttonBackupStart";
            this.buttonBackupStart.Size = new System.Drawing.Size(104, 26);
            this.buttonBackupStart.TabIndex = 6;
            this.buttonBackupStart.Text = "Start Backup";
            this.buttonBackupStart.UseVisualStyleBackColor = true;
            this.buttonBackupStart.Click += new System.EventHandler(this.buttonBackupStart_Click);
            // 
            // groupBackupDirectories
            // 
            this.groupBackupDirectories.Controls.Add(this.buttonRemoveAll);
            this.groupBackupDirectories.Controls.Add(this.buttonRemoveSelected);
            this.groupBackupDirectories.Controls.Add(this.labelBackupDirectories);
            this.groupBackupDirectories.Controls.Add(this.buttonBackupDirectories);
            this.groupBackupDirectories.Controls.Add(this.textBackupDirectores);
            this.groupBackupDirectories.Controls.Add(this.checkedBackupDirectories);
            this.groupBackupDirectories.Location = new System.Drawing.Point(3, 187);
            this.groupBackupDirectories.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBackupDirectories.Name = "groupBackupDirectories";
            this.groupBackupDirectories.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBackupDirectories.Size = new System.Drawing.Size(283, 261);
            this.groupBackupDirectories.TabIndex = 5;
            this.groupBackupDirectories.TabStop = false;
            this.groupBackupDirectories.Text = "Select Directories to Backup";
            // 
            // buttonRemoveAll
            // 
            this.buttonRemoveAll.Location = new System.Drawing.Point(166, 226);
            this.buttonRemoveAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonRemoveAll.Name = "buttonRemoveAll";
            this.buttonRemoveAll.Size = new System.Drawing.Size(108, 25);
            this.buttonRemoveAll.TabIndex = 6;
            this.buttonRemoveAll.Text = "Remove All";
            this.buttonRemoveAll.UseVisualStyleBackColor = true;
            this.buttonRemoveAll.Click += new System.EventHandler(this.buttonRemoveAll_Click);
            // 
            // buttonRemoveSelected
            // 
            this.buttonRemoveSelected.Location = new System.Drawing.Point(12, 226);
            this.buttonRemoveSelected.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonRemoveSelected.Name = "buttonRemoveSelected";
            this.buttonRemoveSelected.Size = new System.Drawing.Size(147, 25);
            this.buttonRemoveSelected.TabIndex = 3;
            this.buttonRemoveSelected.Text = "Remove Selected";
            this.buttonRemoveSelected.UseVisualStyleBackColor = true;
            this.buttonRemoveSelected.Click += new System.EventHandler(this.buttonRemoveBackupDirectory_Click);
            // 
            // labelBackupDirectories
            // 
            this.labelBackupDirectories.AutoSize = true;
            this.labelBackupDirectories.Location = new System.Drawing.Point(9, 26);
            this.labelBackupDirectories.Name = "labelBackupDirectories";
            this.labelBackupDirectories.Size = new System.Drawing.Size(120, 17);
            this.labelBackupDirectories.TabIndex = 3;
            this.labelBackupDirectories.Text = "Backup Directory:";
            // 
            // buttonBackupDirectories
            // 
            this.buttonBackupDirectories.Location = new System.Drawing.Point(199, 44);
            this.buttonBackupDirectories.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonBackupDirectories.Name = "buttonBackupDirectories";
            this.buttonBackupDirectories.Size = new System.Drawing.Size(75, 25);
            this.buttonBackupDirectories.TabIndex = 5;
            this.buttonBackupDirectories.Text = "Browse";
            this.buttonBackupDirectories.UseVisualStyleBackColor = true;
            this.buttonBackupDirectories.Click += new System.EventHandler(this.buttonBackupDirectories_Click);
            // 
            // textBackupDirectores
            // 
            this.textBackupDirectores.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBackupDirectores.Location = new System.Drawing.Point(12, 45);
            this.textBackupDirectores.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBackupDirectores.Name = "textBackupDirectores";
            this.textBackupDirectores.ReadOnly = false;
            this.textBackupDirectores.Size = new System.Drawing.Size(181, 22);
            this.textBackupDirectores.TabIndex = 4;
            // 
            // checkedBackupDirectories
            // 
            this.checkedBackupDirectories.FormattingEnabled = true;
            this.checkedBackupDirectories.Location = new System.Drawing.Point(12, 82);
            this.checkedBackupDirectories.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkedBackupDirectories.Name = "checkedBackupDirectories";
            this.checkedBackupDirectories.Size = new System.Drawing.Size(262, 140);
            this.checkedBackupDirectories.TabIndex = 0;
            // 
            // groupBackupType
            // 
            this.groupBackupType.Controls.Add(this.radioBackupTypeIncremental);
            this.groupBackupType.Controls.Add(this.radioBackupTypeDifferential);
            this.groupBackupType.Controls.Add(this.radioBackupTypeFull);
            this.groupBackupType.Location = new System.Drawing.Point(3, 110);
            this.groupBackupType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBackupType.Name = "groupBackupType";
            this.groupBackupType.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBackupType.Size = new System.Drawing.Size(283, 64);
            this.groupBackupType.TabIndex = 4;
            this.groupBackupType.TabStop = false;
            this.groupBackupType.Text = "Select the Backup Type";
            // 
            // radioBackupTypeIncremental
            // 
            this.radioBackupTypeIncremental.AutoSize = true;
            this.radioBackupTypeIncremental.Location = new System.Drawing.Point(172, 29);
            this.radioBackupTypeIncremental.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioBackupTypeIncremental.Name = "radioBackupTypeIncremental";
            this.radioBackupTypeIncremental.Size = new System.Drawing.Size(102, 21);
            this.radioBackupTypeIncremental.TabIndex = 2;
            this.radioBackupTypeIncremental.Text = "Incremental";
            this.radioBackupTypeIncremental.UseVisualStyleBackColor = true;
            this.radioBackupTypeIncremental.Visible = false;
            // 
            // radioBackupTypeDifferential
            // 
            this.radioBackupTypeDifferential.AutoSize = true;
            this.radioBackupTypeDifferential.Location = new System.Drawing.Point(69, 29);
            this.radioBackupTypeDifferential.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioBackupTypeDifferential.Name = "radioBackupTypeDifferential";
            this.radioBackupTypeDifferential.Size = new System.Drawing.Size(97, 21);
            this.radioBackupTypeDifferential.TabIndex = 1;
            this.radioBackupTypeDifferential.Text = "Differential";
            this.radioBackupTypeDifferential.UseVisualStyleBackColor = true;
            this.radioBackupTypeDifferential.Visible = false;
            // 
            // radioBackupTypeFull
            // 
            this.radioBackupTypeFull.AutoSize = true;
            this.radioBackupTypeFull.Checked = true;
            this.radioBackupTypeFull.Location = new System.Drawing.Point(12, 29);
            this.radioBackupTypeFull.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioBackupTypeFull.Name = "radioBackupTypeFull";
            this.radioBackupTypeFull.Size = new System.Drawing.Size(51, 21);
            this.radioBackupTypeFull.TabIndex = 0;
            this.radioBackupTypeFull.TabStop = true;
            this.radioBackupTypeFull.Text = "Full";
            this.radioBackupTypeFull.UseVisualStyleBackColor = true;
            // 
            // groupBackupLocation
            // 
            this.groupBackupLocation.Controls.Add(this.labelBackupLocation);
            this.groupBackupLocation.Controls.Add(this.buttonBackupLocation);
            this.groupBackupLocation.Controls.Add(this.textBackupLocation);
            this.groupBackupLocation.Location = new System.Drawing.Point(3, 14);
            this.groupBackupLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBackupLocation.Name = "groupBackupLocation";
            this.groupBackupLocation.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBackupLocation.Size = new System.Drawing.Size(283, 82);
            this.groupBackupLocation.TabIndex = 3;
            this.groupBackupLocation.TabStop = false;
            this.groupBackupLocation.Text = "Select Where to Save the Backup";
            // 
            // labelBackupLocation
            // 
            this.labelBackupLocation.AutoSize = true;
            this.labelBackupLocation.Location = new System.Drawing.Point(5, 30);
            this.labelBackupLocation.Name = "labelBackupLocation";
            this.labelBackupLocation.Size = new System.Drawing.Size(117, 17);
            this.labelBackupLocation.TabIndex = 0;
            this.labelBackupLocation.Text = "Backup Location:";
            // 
            // buttonBackupLocation
            // 
            this.buttonBackupLocation.Location = new System.Drawing.Point(199, 48);
            this.buttonBackupLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonBackupLocation.Name = "buttonBackupLocation";
            this.buttonBackupLocation.Size = new System.Drawing.Size(75, 25);
            this.buttonBackupLocation.TabIndex = 2;
            this.buttonBackupLocation.Text = "Browse";
            this.buttonBackupLocation.UseVisualStyleBackColor = true;
            this.buttonBackupLocation.Click += new System.EventHandler(this.buttonBackupLocation_Click);
            // 
            // textBackupLocation
            // 
            this.textBackupLocation.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBackupLocation.Location = new System.Drawing.Point(8, 49);
            this.textBackupLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBackupLocation.Name = "textBackupLocation";
            this.textBackupLocation.ReadOnly = false;
            this.textBackupLocation.Size = new System.Drawing.Size(185, 22);
            this.textBackupLocation.TabIndex = 1;
            // 
            // browserBackupLocation
            // 
            //this.browserBackupLocation.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // threadBackup
            // 
            this.threadBackup.WorkerReportsProgress = true;
            this.threadBackup.WorkerSupportsCancellation = true;
            this.threadBackup.DoWork += new System.ComponentModel.DoWorkEventHandler(this.threadBackup_DoWork);
            this.threadBackup.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.threadBackup_ProgressChanged);
            this.threadBackup.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.threadBackup_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 564);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.statusMain);
            this.Controls.Add(this.menuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Jammary Backup";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.groupJobHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridJobHistory)).EndInit();
            this.groupBackupDirectories.ResumeLayout(false);
            this.groupBackupDirectories.PerformLayout();
            this.groupBackupType.ResumeLayout(false);
            this.groupBackupType.PerformLayout();
            this.groupBackupLocation.ResumeLayout(false);
            this.groupBackupLocation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.GroupBox groupBackupDirectories;
        private System.Windows.Forms.Button buttonRemoveSelected;
        private System.Windows.Forms.Label labelBackupDirectories;
        private System.Windows.Forms.Button buttonBackupDirectories;
        private System.Windows.Forms.TextBox textBackupDirectores;
        private System.Windows.Forms.CheckedListBox checkedBackupDirectories;
        private System.Windows.Forms.GroupBox groupBackupType;
        private System.Windows.Forms.RadioButton radioBackupTypeIncremental;
        private System.Windows.Forms.RadioButton radioBackupTypeDifferential;
        private System.Windows.Forms.RadioButton radioBackupTypeFull;
        private System.Windows.Forms.GroupBox groupBackupLocation;
        private System.Windows.Forms.Label labelBackupLocation;
        private System.Windows.Forms.Button buttonBackupLocation;
        private System.Windows.Forms.TextBox textBackupLocation;
        private System.Windows.Forms.FolderBrowserDialog browserBackupLocation;
        private System.Windows.Forms.FolderBrowserDialog browserBackupDirectories;
        private System.Windows.Forms.Button buttonBackupStart;
        private System.Windows.Forms.Button buttonRemoveAll;
        private System.Windows.Forms.ToolStripMenuItem backupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startBackupToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupJobHistory;
        private System.Windows.Forms.ToolStripStatusLabel toolStatus;
        private System.Windows.Forms.ToolStripProgressBar toolProgress;
        private System.Windows.Forms.Button buttonCancelBackup;
        private System.ComponentModel.BackgroundWorker threadBackup;
        private System.Windows.Forms.ToolStripStatusLabel toolBackupStatus;
        private System.Windows.Forms.ToolStripMenuItem cancelBackupToolStripMenuItem;
        private System.Windows.Forms.DataGridView gridJobHistory;
    }
}

