using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace JammaryBackup
{
    /// <summary>
    /// Main Form, displays the user options, job history and backup progress and status.
    /// </summary>
    public partial class MainForm : Form
    {
        // Define private member properties.
        private List<string>    backgroundWorkerThreads = new List<string>();
        private Guid            backupJobID;
        private string          backupResult;
        private string          backupSource;
        private long            backupSourceSize;
        private bool            backupStarted;
        private string          backupTarget;
        private string          backupTargetZipFile;
        private string          backupType;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MainForm()
        {
            // Initialize all the controller components of the form.
            InitializeComponent();

            // Create necessary databases for the application.
            SQL sqlInitDB = new SQL();
            sqlInitDB.InizializeDatabases();
        }

        /* MAIN EVENTS */

        /// <summary>
        /// <para>Runs everytime the main form is loaded.</para>
        /// <para>Since we only have one form, it's only loaded once when the application is started.</para>
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Retrieve the job history log from the database,
            // and print the list to the correct form component.
            this.LoadJobHistoryFromSQL();
            
            // Initialize the boolean variable to false, 
            // since the form has just loaded, no backup job could have been started yet.
            this.backupStarted = false;
        }

        /* MENU EVENTS */
        
        /// <summary>
        /// Displays the About dialog window.
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About form = new About();
            form.ShowDialog();
        }

        /// <summary>
        /// Cancels the backup job.
        /// </summary>
        private void cancelBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackupCancel();
        }

        /// <summary>
        /// Cancels running backups, and closes the Application.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackupCancel();
            this.Close();
        }

        /// <summary>
        /// Starts the backup job.
        /// </summary>
        private void startBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackupStart();
        }

        /* BUTTON EVENTS */

        /// <summary>
        /// Allows the user to browse the local filesystem, and add directories to be backed up.
        /// </summary>
        private void buttonBackupDirectories_Click(object sender, EventArgs e)
        {
            // Define and initialize main variables.
            DialogResult    dialogResult    = this.browserBackupDirectories.ShowDialog();
            string          selectedPath    = this.browserBackupDirectories.SelectedPath;

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // Only add the directory if the user actually selected a directory.
                if (selectedPath.Length > 0)
                {
                    // Make sure the Backup Location and Backup Directory are different.
                    if (selectedPath == this.textBackupLocation.Text) {
                        MessageBox.Show("The Backup Location and Backup Directory cannot be the same,\r\nplease choose another directory.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Only add a new entry if the directory has not already been selected.
                        if (!this.checkedBackupDirectories.Items.Contains(selectedPath))
                        {
                            // Set the Backup Directory.
                            this.checkedBackupDirectories.Items.Add(selectedPath);

                            // Add the backup directory as a queued backup job to the backgroundWorkerThreads list.
                            this.backgroundWorkerThreads.Add(selectedPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows the user to browse the local filesystem, and choose the location where to back everything up to.
        /// </summary>
        private void buttonBackupLocation_Click(object sender, EventArgs e)
        {
            // Define and initialize main variables.
            DialogResult    dialogResult    = this.browserBackupLocation.ShowDialog();
            string          selectedPath    = this.browserBackupLocation.SelectedPath;

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // Make sure the Backup Location and Backup Directory are different.
                if (this.checkedBackupDirectories.Items.Contains(selectedPath)) {
                    MessageBox.Show("The Backup Location and Backup Directory cannot be the same,\r\nplease choose another directory.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else {
                    // Set the Backup Location.
                    this.textBackupLocation.Text = selectedPath;
                }

                // Only display the Differential and Incremental backup options
                // if there already exists a Full backup in the backup location directory.
                if (Directory.Exists(selectedPath.TrimEnd('\\') + @"\JammaryBackup"))
                {
                    if (Directory.GetDirectories(selectedPath.TrimEnd('\\') + @"\JammaryBackup").Count() > 0) {
                        this.radioBackupTypeIncremental.Visible  = true;
                        this.radioBackupTypeDifferential.Visible = true;
                    }
                }
                else
                {
                    this.radioBackupTypeIncremental.Visible  = false;
                    this.radioBackupTypeDifferential.Visible = false;
                }
            }
        }

        /// <summary>
        /// Starts the backup job.
        /// </summary>
        private void buttonBackupStart_Click(object sender, EventArgs e)
        {
            this.BackupStart();
        }

        /// <summary>
        /// Cancels the backup job.
        /// </summary>
        private void buttonCancelBackup_Click(object sender, EventArgs e)
        {
            this.BackupCancel();
        }

        /// <summary>
        /// Removes all previously chosen backup directories.
        /// </summary>
        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            // Clear the list showing selected backup directories.
            this.checkedBackupDirectories.Items.Clear();

            // Clear the backup job queue in the backgroundWorkerThreads list.
            this.backgroundWorkerThreads.Clear();
        }

        /// <summary>
        /// Removes checked backup directories from the list.
        /// </summary>
        private void buttonRemoveBackupDirectory_Click(object sender, EventArgs e)
        {
            // Create a temporary copy of the original List to hold items to remove.
            // This is necessary because if you try to 
            // remove items directly inside a foreach loop,
            // the list structure will change while you remove, 
            // therefore it will fail.
            List<string> directoriesTemporaryCopy = new List<string>();

            // Add the directories to be removed in the temporary string list.
            foreach (string backupDirectory in this.checkedBackupDirectories.CheckedItems)
            {
                directoriesTemporaryCopy.Add(backupDirectory);
            }

            // Remove the directories from the original string collection.
            foreach (string directory in directoriesTemporaryCopy)
            {
                // Remove the directory item from the list showing selected backup directories.
                this.checkedBackupDirectories.Items.Remove(directory);

                // Remove the backup job from the queue in the backgroundWorkerThreads list.
                this.backgroundWorkerThreads.Remove(directory);
            }
        }

        /* CUSTOM METHODS */

        /// <summary>
        /// Cancels the backup job.
        /// </summary>
        private void BackupCancel()
        {
            if (threadBackup.WorkerSupportsCancellation)
            {
                threadBackup.CancelAsync();
            }
        }

        /// <summary>
        /// Kills all running 7zip processes.
        /// </summary>
        private void BackupForceKill()
        {
            // Get an array list of all processes running in the background.
            Process[] procs = Process.GetProcesses();

            // Kill the compression utility if it's still running.
            foreach (Process process in procs)
            {
                if (process.ProcessName == "7zip")
                {
                    process.Close();
                    process.Dispose();
                    process.Kill();
                }
            }
        }

        /// <summary>
        /// Starts the backup job, by queuing the job to the backgroundWorkerThreads list, and calling the threaded job.
        /// </summary>
        private Result BackupStart()
        {
            // Make sure the Backup Location has been chosen.
            if (textBackupLocation.Text.Length < 1)
            {
                MessageBox.Show("Please choose a Backup Location first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.buttonBackupLocation.Focus();

                return Result.Failed;
            }
            // Make sure at least one Backup Directory has been chosen.
            else if (checkedBackupDirectories.Items.Count < 1)
            {
                MessageBox.Show("Please choose a Backup Directory first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.buttonBackupDirectories.Focus();

                return Result.Failed;
            }
            else if (this.backupStarted)
            {
                //MessageBox.Show("Backup has already been started!");
            }
            else
            {
                // Start the backup job if the backgroundworker thread is not already in use.
                if (!threadBackup.IsBusy)
                {
                    // If there is a backup job queued in the backgroundWorkerThreads list, start the backup job.
                    if (this.backgroundWorkerThreads.Count() > 0)
                    {
                        // Set the boolean to true to tell the application that a backup has started.
                        this.backupStarted = true;

                        // Run the first backup job queued in the backgroundWorkerThreads list.
                        this.threadBackup.RunWorkerAsync(backgroundWorkerThreads.First());

                        // Remove the started backup job from the job queue in the backgroundWorkerThreads list.
                        // This way, when the application returns for the next job, 
                        // it will start the first job in the queue again, and then remove it from the queue.
                        // And this ends when there is no more jobs in the queue.
                        this.backgroundWorkerThreads.Remove(backgroundWorkerThreads.First());
                    }
                    // If no backup jobs have been queued yet (the queue is empty), 
                    // Fill the queue with all the directories chosen for backup by
                    // adding all the backup directories chosen to the backgroundWorkerThreads list.
                    else
                    {
                        foreach (string directory in this.checkedBackupDirectories.Items)
                        {
                            this.backgroundWorkerThreads.Add(directory);
                        }
                    }
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// Retrieves the backup job history from the log database, and displays the result in a Grid View.
        /// </summary>
        private Result LoadJobHistoryFromSQL()
        {
            // Create a new SQL object.
            SQL sqlLog = new SQL();

            // Set the SQL Connection string to connect to the Log database.
            sqlLog.SqlConnectionString = Properties.Settings.Default.LogConnectionString;

            // Set the Database Table to use.
            sqlLog.SqlTableName = "job_history";

            // Build the SQL Query string to retrieve the backup job history from the log database.
            sqlLog.SqlQuery = "SELECT date_time, source, target, type, result ";
            sqlLog.SqlQuery += "FROM " + sqlLog.SqlTableName + " ";
            sqlLog.SqlQuery += "ORDER BY date_time DESC";

            // Try to retrieve the backup job history from the log database,
            // and catch possible errors the method can throw.
            try
            {
                // Assign the result set as the data source of the correct Grid View.
                this.gridJobHistory.DataSource = sqlLog.GetData().Tables[sqlLog.SqlTableName];
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Retrieves the backup job history from the XML log File, and displays the result in a Text Box.
        /// </summary>
        private Result LoadJobHistoryXML()
        {
            // Make sure the XML File already exists.
            if (File.Exists(@"log\JobHistory.xml"))
            {
                // Define and initialize main variables.
                List<string> jobHistoryList = new List<string>();
                                
                //this.textJobHistory.Text = "";

                // Try to retrieve the backup job history from the XML log file,
                // and catch possible errors the method can throw.
                try
                {
                    // Create an XmlTextReader object to read from the XML file.
                    XmlTextReader xmlReader = new XmlTextReader(@"log\JobHistory.xml");

                    // Read through the XML file and retrive the log event data.
                    while (xmlReader.Read())
                    {
                        // Skip the Root Element.
                        if (xmlReader.Name != "BackupJobHistory")
                        {
                            // Get the job identifier with the date/time attribute.
                            if ((xmlReader.Name == "BackupJob") && (xmlReader.NodeType != XmlNodeType.EndElement))
                            {
                                //this.textJobHistory.Text += "\r\n" + xmlReader.GetAttribute("DateAndTime") + "\r\n";
                            }

                            // Get the XML element name.
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name != "BackupJob"))
                            {
                                //this.textJobHistory.Text += xmlReader.Name + ": ";
                            }

                            // Get the XML element value.
                            if ((xmlReader.NodeType == XmlNodeType.Text) && (xmlReader.Name != "BackupJob"))
                            {
                                //this.textJobHistory.Text += xmlReader.Value + "\r\n";
                            }
                        }
                    }

                    // Close the XmlTextReader object.
                    xmlReader.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Result.Failed;
                }

            }

            return Result.Success;
        }

        /// <summary>
        /// Starts the actual backup job as a BackgroundWorker Thread.
        /// <para>The threaded job supports cancellation and reports progress.</para>
        /// </summary>
        private Result StartBackup(BackgroundWorker backgroundWorkerThread, string directoryPath)
        {
            // Create a Backup object.
            Backup backup = new Backup();
           
            backup.BackupJobID  = this.backupJobID;     // Set the unique ID of this backup job.
            backup.BackupSource = this.backupSource;    // Set the chosen Backup Source Directory.
            backup.BackupTarget = this.backupTarget;    // Set the chosen Backup Target Location.
            backup.BackupType   = this.backupType;      // Set the chosen Backup Type.

            // Try to start the backup.
            Result result = backup.Start();

            // Otherwise fail with a descriptive error.
            if (result == Result.Failed)
            {
                MessageBox.Show("Failed to start the backup of " + this.backupSource + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Calculates the total size of the backup directory by calculating the size of each files in the backup directory.
        /// <para>@return A numeric Long describing the total directory size in bytes.</para>
        /// </summary>
        private long GetBackupDirectorySize()
        {
            // Define and initialize main variables.
            DirectoryInfo   backupDirectoryInfo;
            long            backupDirectorySize     = 1;
            List<string>    filesInBackupDirectory  = new List<string>();

            // Get the size of the backup directory.
            if (Directory.Exists(this.backupSource))
            {
                // Get the properties of the backup directory.
                backupDirectoryInfo = new DirectoryInfo(this.backupSource);
                backupDirectoryInfo.Refresh();

                // Initialize the backup directory size to0 before starting to calculate its total size.
                backupDirectorySize = 0;

                // Get a list of all files in the backup directory and sub-directories.
                filesInBackupDirectory = FileIO.GetAllFilesInDirectory(this.backupSource);

                // Calculate the total size of the backup directory by 
                // calculating the size of each files in the backup directory.
                foreach (string file in filesInBackupDirectory)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    backupDirectorySize += fileInfo.Length;
                }
            }
            
            // To avoid DivideByZeroException exceptions, 
            // we make sure the directory size is at least 1 byte.
            if (backupDirectorySize < 1)
            {
                backupDirectorySize = 1;
            }
            
            return backupDirectorySize;
        }

        /// <summary>
        /// Checks if a 7-Zip process is running.
        /// <para>@return A Boolean indicating true if it's running or false if not.</para>
        /// </summary>
        private bool SevenZipIsRunning()
        {
            // Get an array list of all processes running in the background.
            Process[] procs = Process.GetProcesses();

            // If the compression process is still running,
            // set the boolean value to back to true and the loop will re-iterate.
            foreach (Process process in procs)
            {
                if (process.ProcessName == "7zip")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the progress of the backup job.
        /// <para>@return A numeric Integer indicating the progress as a percentage.</para>
        /// </summary>
        private Result SetBackupProgress()
        {
            // Define and initialize main variables.
            int         backupProgress          = 0;
            long        backupDirectorySize     = this.backupSourceSize;
            double      backupProgress_double   = 0.0d;
            FileInfo    zipFileInfo;
            long        zipSize                 = 1;

            // Get the current size of the zip file, 
            // will increase throughout the compression process and this while loop.
            if (File.Exists(this.backupTargetZipFile))
            {
                zipFileInfo = new FileInfo(this.backupTargetZipFile);
                zipFileInfo.Refresh();

                zipSize = zipFileInfo.Length;
            }

            // To avoid DivideByZeroException exceptions, 
            // we make sure the zip size is at least 1 byte.
            if (zipSize < 1)
            {
                zipSize = 1;
            }

            // Try to calculate the progress of the backup job,
            // and catch possible errors the method can throw.
            try
            {
                // When dividing zipSize with backupDirectorySize we get a 0.x value, 
                // therefore we need double values otherwise we loose the precision
                // and the result would be "zipSize / backupDirectorySize = 0" and "0 * 100 = 0".
                // After we get the percentage value, we can cast it into an integer value.
                // For example: "200 / 1000 = 0.2" and "0.2 * 100.0 = 20.0" and "(int)20.0 = 20".
                backupProgress_double   = ((double)zipSize / (double)backupDirectorySize) * 100.0d;
                backupProgress          = (int)backupProgress_double;

                // The acceptable progress values are between 0 and 100, 
                // so if the value for some strange reason is negative set it to 0,
                // and if it is higher than 99 set it to 100.
                if (backupProgress < 0)
                {
                    backupProgress = 0;
                }
                else if (backupProgress > 99)
                {
                    backupProgress = 100;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

                return Result.Failed;
            }

            // Update the progress bar by setting the progress value
            this.toolProgress.Value = backupProgress;

            return Result.Success;
        }

        /// <summary>
        /// Query the database and return the filename for the final ZIP file.
        /// <para>@return A string of the filename for the final ZIP file.</para>
        /// </summary>
        private string GetBackupTargetZipFilename()
        {
            // Define and initialize main variables.
            string  zipFilename = "";

            // Create a new SQL object.
            SQL sqlLog = new SQL();

            // Set the SQL Connection string to connect to the Log database.
            sqlLog.SqlConnectionString = Properties.Settings.Default.LogConnectionString;

            // Set the Database Table to use.
            sqlLog.SqlTableName = "job_history";

            // Build the SQL Query string to retrieve the zip filename from the log database.
            sqlLog.SqlQuery = "SELECT target ";
            sqlLog.SqlQuery += "FROM " + sqlLog.SqlTableName + " ";
            sqlLog.SqlQuery += "WHERE job_id = '" + this.backupJobID.ToString() + "'";

            // Try to retrieve the zip filename from the log database,
            // and catch possible errors the method can throw.
            try
            {
                // Get the zip filename from the data set.
                zipFilename = sqlLog.GetData().Tables[sqlLog.SqlTableName].Rows[0]["target"].ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return zipFilename;
        }

        /// <summary>
        /// Logs the backup job completion status to an SQL Database.
        /// </summary>
        private Result LogJobCompletionStatusToSQL()
        {
            // Create a new Log object.
            Log log = new Log();

            // Get the job details and set the Log properties.
            log.BackupJobID     = this.backupJobID;
            log.BackupResult    = this.backupResult;

            // Try to write the log entry to the log database.
            Result result = log.LogJobCompletionStatusToSQL();

            // Otherwise fail with a descriptive error.
            if (result == Result.Failed)
            {
                MessageBox.Show("Failed to log the backup job completion status to the database table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Forcefully stop all running processes, and restore old backup files if they have been overwritten.
        /// </summary>
        private Result CleanUpFailedBackup()
        {
            try
            {
                // Forcefully stop all running compression processes.
                this.BackupForceKill();

                if (this.backupType == "full")
                {
                    // Delete the original file if exists.
                    if (File.Exists(this.backupTargetZipFile))
                    {
                        File.Delete(this.backupTargetZipFile);
                    }

                    // Move the temporary file back to the original file
                    if (File.Exists(this.backupTargetZipFile + ".old"))
                    {
                        File.Move(this.backupTargetZipFile + ".old", this.backupTargetZipFile);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /* BACKGROUNDWORKER THREAD EVENTS */

        /// <summary>
        /// Main handler of the backup thread, it starts the thread and calculates the progress.
        /// </summary>
        private void threadBackup_DoWork(object sender, DoWorkEventArgs e)
        {
            // Define and initialize main variables.
            BackgroundWorker    backgroundWorkerThread  = sender as BackgroundWorker;
            string              methodArgument          = (string)e.Argument;

            // Set the unique ID of this backup job.
            this.backupJobID = Guid.NewGuid();

            // Set the chosen Backup Directory.
            this.backupSource = methodArgument;

            // Set the chosen Backup Target Location.
            this.backupTarget = this.textBackupLocation.Text;

            // Set the chosen Backup Type.
            if (this.radioBackupTypeFull.Checked)
            {
                this.backupType = "full";
            }
            else if (this.radioBackupTypeDifferential.Checked)
            {
                this.backupType = "differential";
            }
            else if (this.radioBackupTypeIncremental.Checked)
            {
                this.backupType = "incremental";
            }
            else
            {
                MessageBox.Show("ERROR! Invalid Backup Type Chosen.");
            }

            // If the backup job was cancelled by the user, 
            // set the boolean Cancel property of the thread to true.
            if (backgroundWorkerThread.CancellationPending)
            {
                // Tell the thread to cancel the currently running job.
                e.Cancel = true;

                // Forcefully stop all running compression services.
                this.BackupForceKill();
            }
            else
            {
                // Start the backup job thread by calling the StartBackup method, 
                // and the backup directory as an argument.
                e.Result = this.StartBackup(backgroundWorkerThread, methodArgument);

                // Set the zip filename of the backup output.
                this.backupTargetZipFile = this.GetBackupTargetZipFilename();

                // Get the size of the backup directory.
                this.backupSourceSize = this.GetBackupDirectorySize();

                do
                {
                    System.Threading.Thread.Sleep(1);

                    // Raise the ProgressChanged event.
                    backgroundWorkerThread.ReportProgress(1);
                } while (this.SevenZipIsRunning());

                /*
                while (this.SevenZipIsRunning())
                {
                    System.Threading.Thread.Sleep(1);

                    // Raise the ProgressChanged event.
                    backgroundWorkerThread.ReportProgress(1);
                } */
            }
        }

        /// <summary>
        /// Handles changes in the progress of the backup thread.
        /// </summary>
        private void threadBackup_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (threadBackup.WorkerReportsProgress)
            {
                // Update the main status field in the bottom left corner
                this.toolStatus.Text = "Backup in Progress ... ";

                // Write the status to the status bar to provide the user with some details.
                if (this.backupTargetZipFile == "No Changes")
                {                
                    this.toolBackupStatus.Text = "No changes since the last backup.";
                }
                else
                {
                    this.toolBackupStatus.Text = "Compressing \"" + this.backupTargetZipFile + "\" ... ";
                }

                // Make the Cancel button and menu item visible so 
                // the user can choose to cancel the backup if needed.
                this.buttonCancelBackup.Visible             = true;
                this.cancelBackupToolStripMenuItem.Visible  = true;

                // Update the Backup Progress Bar
                this.SetBackupProgress();
            }
        }

        /// <summary>
        /// Runs after the backup thread has completed or been terminated.
        /// </summary>
        private void threadBackup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // If the backup job was cancelled by the user,
                // update the status fields and let them know what happened.
                this.toolStatus.Text        =  "Backup Cancelled";
                this.backupResult           =  "Cancelled";

                // Update the status bar only if some files have been backed up.
                if (this.backupTargetZipFile != "No Changes")
                {
                    this.toolBackupStatus.Text += "Cancelled";
                }

                // Clean up all temporary files and processes used.
                this.CleanUpFailedBackup();
            }
            else if (e.Error != null)
            {
                // If the backup job terminates unexpectedly,
                // update the status fields and let them know there has been an error.
                this.toolStatus.Text        =  "Error: " + e.Error.Message;
                this.backupResult           =  "Failed";

                // Update the status bar only if some files have been backed up.
                if (this.backupTargetZipFile != "No Changes")
                {
                    this.toolBackupStatus.Text += "Failed";
                }

                // Clean up all temporary files and processes used.
                this.CleanUpFailedBackup();
            }
            else
            {
                // If the backup job completed successfully,
                // update the status fields and let them know everything is fine.
                this.toolStatus.Text        =  "Ready";
                this.backupResult           =  "Completed";

                // Update the status bar only if some files have been backed up.
                if (this.backupTargetZipFile != "No Changes")
                {
                    this.toolBackupStatus.Text += "Completed";
                }

                // Delete the temporary file.
                if (this.backupType == "full")
                {
                    if (File.Exists(this.backupTargetZipFile + ".old"))
                    {
                        File.Delete(this.backupTargetZipFile + ".old");
                    }
                }
            }

            // Let the application know that the current backup job is no longer running.
            this.backupStarted = false;
            
            // Reset the Progress bar.
            this.toolProgress.Value = 0;
            
            // Hide the Cancel button and cancel menu item.
            this.buttonCancelBackup.Visible             = false;
            this.cancelBackupToolStripMenuItem.Visible  = false;

            // Log the backup job details to an SQL Database.
            this.LogJobCompletionStatusToSQL();

            // Retrieve the job history log from the database,
            // and print the list to the correct form component.
            this.LoadJobHistoryFromSQL();

            // Start the next backup job if the backgroundworker thread is not already in use.
            if (!threadBackup.IsBusy)
            {
                // If there is another backup job queued in the backgroundWorkerThreads list, start the backup job.
                if (this.backgroundWorkerThreads.Count() > 0)
                {
                    // Set the boolean to true to tell the application that a backup has started.
                    this.backupStarted = true;

                    // Run the first backup job queued in the backgroundWorkerThreads list.
                    this.threadBackup.RunWorkerAsync(backgroundWorkerThreads.First());

                    // Remove the started backup job from the job queue in the backgroundWorkerThreads list.
                    this.backgroundWorkerThreads.Remove(backgroundWorkerThreads.First());
                }
                // If no backup jobs have been queued yet, 
                // add all the backup directories chosen to the backgroundWorkerThreads list.
                else
                {
                    foreach (string directory in this.checkedBackupDirectories.Items)
                    {
                        this.backgroundWorkerThreads.Add(directory);
                    }
                }
            }

            // Only display the Differential and Incremental backup options
            // if there already exists a Full backup in the backup location directory.
            if (Directory.Exists(this.browserBackupLocation.SelectedPath.TrimEnd('\\') + @"\JammaryBackup"))
            {
                if (Directory.GetDirectories(this.browserBackupLocation.SelectedPath.TrimEnd('\\') + @"\JammaryBackup").Count() > 0)
                {
                    this.radioBackupTypeIncremental.Visible  = true;
                    this.radioBackupTypeDifferential.Visible = true;
                }
            }
            else
            {
                this.radioBackupTypeIncremental.Visible  = false;
                this.radioBackupTypeDifferential.Visible = false;
            }
        }
    
    }
}

