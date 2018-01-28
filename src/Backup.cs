using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JammaryBackup
{
    /// <summary>
    /// Takes care of all backup operations in the application.
    /// <para>Supports compressing directories using 7zip.</para>
    /// <para>Can perform Full, Differential and Incremental backups.</para>
    /// </summary>
    public class Backup
    {
        // Define private member properties.
        private Guid         backupJobID;
        private string       backupSource;
        private string       backupTarget;
        private string       backupTargetZipFile;
        private string       backupType;
        private List<string> filesToCompress = new List<string>();
        private string       incrementalBackupDirectory;

        // Define private member properties.
        public Guid   BackupJobID  { set { backupJobID  = value; } }
        public string BackupSource { set { backupSource = value; } }
        public string BackupTarget { set { backupTarget = value; } }
        public string BackupType   { set { backupType   = value; } }

        /// <summary>
        /// Starts the Backup process.
        /// <para>Requires the following member properties:</para>
        /// <para>@param backupSource String - Defines the path of what was backed up.</para>
        /// <para>@param backupTarget String - Defines the location where the source was saved.</para>
        /// <para>@param backupType   String - Defines the type of backup (full, differential or incremenetal).</para>
        /// </summary>
        public Result Start()
        {
            try
            {
                // Make sure the required member properties are not blank.
                if (((this.backupSource != "") && (this.backupSource != null)) &&
                    ((this.backupTarget != "") && (this.backupTarget != null)) &&
                    ((this.backupType   != "") && (this.backupType   != null)))
                {
                    // Remove the trailing backslash from the backup target and source paths.
                    // This is required because if a root drive was selected, such as C:\ or D:\,
                    // it comes with a trailing back-slash (\),
                    // But if a path was selected, such as C:\Path or D:\Path\SubDir,
                    // it does not come with a trailing back-slash (\).
                    // This will make a problem later when building path and file names,
                    // so to be sure we have the same root path to work with, 
                    // we remove the trailing back-slash, 
                    // and end-up with the same format, such as C:, D:, C:\Path or D:\Path\SubDir.
                    this.backupSource.TrimEnd('\\');
                    this.backupTarget.TrimEnd('\\');

                    // Try to start compressing the chosen directory.
                    Result result = this.CompressDirectory();

                    // Otherwise fail with a descriptive error.
                    if (result == Result.Failed)
                    {
                        MessageBox.Show("Failed to compress " + this.backupSource + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return Result.Failed;
                    }
                }
                // Otherwise fail with a descriptive error.
                else
                {
                    throw new System.ArgumentNullException("backupSource, backupTarget and backupType cannot be blank, \r\nplease set them to valid values.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Compresses the chosen directory into one ZIP file and stores it in the target location.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// </summary>
        private Result CompressDirectory()
        {
            // Define and initialize main variables.
            DirectoryInfo directoryInfo       = new DirectoryInfo(this.backupSource);
            string        backupDirectoryName = directoryInfo.Name;

            // Since filenames cannot contain the colon character (:),
            // we need to remove it to avoid errors when creating the compressed file.
            if (backupDirectoryName.Contains(':'))
            {
                backupDirectoryName = backupDirectoryName.TrimEnd('\\');
                backupDirectoryName = backupDirectoryName.TrimEnd(':');
            }

            // Try to create a folder structure based on the backup type chosen.
            this.CreateDirectoryStructure();

            // Set the filename for the final ZIP file based on the backup type chosen.
            this.backupTargetZipFile = this.GetZipFileNameAndPath(backupDirectoryName);

            // Try to compresses the chosen directory into one ZIP file,
            // and store it in the correct target location based on the backup type chosen, 
            // and catch possible errors the method can throw.
            try
            {
                // Start by renaming existing ZIP files with an .old extension, 
                // this way if the backup process is terminated the latest backup file 
                // can be recovered by just renaming the file back (removing the .old extension).
                if (this.backupType == "full")
                {
                    if (File.Exists(this.backupTargetZipFile))
                    {
                        File.Move(this.backupTargetZipFile, this.backupTargetZipFile + ".old");
                    }
                }

                // Select which files should be backed up based on the backup type,
                // and if the files have changed or not.
                this.SelectFilesToCompress();

                // After deciding which files to backup/compress,
                // start the actual backup process using 7zip to compress the files and directories.
                this.StartCompress(this.backupTargetZipFile);

                // Delete empty directories created during the process.
                this.RemoveEmptyDirectories();

                // Log the backup job details to an SQL Database.
                this.LogJobHistoryToSQL();
            }
            catch (Win32Exception win32Exception)
            {
                MessageBox.Show(win32Exception.Message + "\r\nError using 7zip", "Win32 Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }
            finally
            {
                this.filesToCompress.Clear();
            }

            return Result.Success;
        }

        /// <summary>
        /// Creates a folder structure based on the backup type chosen.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// </summary>
        private Result CreateDirectoryStructure()
        {
            // Define and initialize main variables.
            string diffBackupDirectory = "";
            string incBackupDirectory  = "";
            string fullBackupDirectory = this.backupTarget + @"\JammaryBackup\" + DateTime.Now.ToString("yyyyMMdd");

            // Try to create a folder structure based on the backup type chosen, 
            // and catch possible errors the method can throw.            
            try
            {
                switch (this.backupType)
                {
                    case "full":
                        if (!Directory.Exists(fullBackupDirectory))
                        {
                            // Create backup directory structure.
                            Directory.CreateDirectory(fullBackupDirectory);
                            Directory.CreateDirectory(fullBackupDirectory + @"\Differential");
                            Directory.CreateDirectory(fullBackupDirectory + @"\Incremental");
                        }

                        break;
                    case "differential":
                        // Set the Differential directory to be created within 
                        // the latest/newest Full backup directory.
                        diffBackupDirectory = this.GetDifferentialBackupDirectoryName();

                        if (!Directory.Exists(diffBackupDirectory))
                        {
                            // Create directory for Differential backups.
                            Directory.CreateDirectory(diffBackupDirectory);
                        }

                        break;
                    case "incremental":
                        // Set the Incremental directory to be created within 
                        // the latest/newest Full backup directory,
                        // and create a subdirectory for each Incremental job using 
                        // the current date/time for the subdirectory name.
                        incBackupDirectory = this.GetIncrementalBackupDirectoryName();

                        if (!Directory.Exists(incBackupDirectory))
                        {
                            // Create directory for Incremental backups.
                            Directory.CreateDirectory(incBackupDirectory);
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Build and return directory name and path of the Differential backup location.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// <para>@return A string of the directory name and path of the Differential backup location.</para>
        /// </summary>
        private string GetDifferentialBackupDirectoryName()
        {
            // Define and initialize main variables.
            string   diffBackupDirectory = "";
            string[] fullDirectories     = { };

            try
            {
                // Get a list of all directories containing Full backup jobs.
                fullDirectories = this.GetFullBackupDirectories();

                // Set the Differential directory to be created within 
                // the latest/newest Full backup directory.
                diffBackupDirectory = fullDirectories.Max() + @"\Differential";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return diffBackupDirectory;
        }

        /// <summary>
        /// Build and return a list of backup directories of the Full backup type.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// <para>@return A string-list of backup directories of the Full backup type.</para>
        /// </summary>
        private string[] GetFullBackupDirectories()
        {
            // Define and initialize main variables.
            string[] fullDirectories = { };

            try
            {
                // Get a list of all directories containing Full backup jobs.
                fullDirectories = Directory.GetDirectories(this.backupTarget + @"\JammaryBackup");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return fullDirectories;
        }

        /// <summary>
        /// Build and return directory name and path of the Incremental backup location.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// <para>@return A string of the directory name and path of the Incremental backup location.</para>
        /// </summary>
        private string GetIncrementalBackupDirectoryName()
        {
            // Define and initialize main variables.
            string   incBackupDirectory = "";
            string[] fullDirectories    = { };

            try
            {
                // Get a list of all directories containing Full backup jobs.
                fullDirectories = this.GetFullBackupDirectories();

                // Set the Incremental directory to be created within 
                // the latest/newest Full backup directory,
                // and create a subdirectory for each Incremental job using 
                // the current date/time for the subdirectory name.
                incBackupDirectory = fullDirectories.Max() + @"\Incremental\" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return incBackupDirectory;
        }

        /// <summary>
        /// Build and return the filename for the final ZIP file for based on the backup type chosen.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// <para>Takes the following input parameter(s):</para>
        /// <para>@param backupDirectoryName String - Defines the name of the directory to backup.</para>
        /// <para>@return A string of the filename for the final ZIP file.</para>
        /// </summary>
        private string GetZipFileNameAndPath(string backupDirectoryName)
        {
            // Define and initialize main variables.
            string diffBackupDirectory      = this.GetDifferentialBackupDirectoryName();
            this.incrementalBackupDirectory = this.GetIncrementalBackupDirectoryName();
            string fullBackupDirectory      = this.backupTarget + @"\JammaryBackup\" + DateTime.Now.ToString("yyyyMMdd");
            string zipFileNameAndPath       = "";

            // Try to build and return the filename for the final ZIP file.
            try
            {
                switch (this.backupType)
                {
                    // Set the filename for the final ZIP file for Full backups.
                    case "full":
                        zipFileNameAndPath = fullBackupDirectory + @"\" + backupDirectoryName + ".zip";
                        break;
                    // Set the filename for the final ZIP file for Differential backups.
                    case "differential":
                        zipFileNameAndPath = diffBackupDirectory + @"\" + backupDirectoryName + "-DIFF.zip";
                        break;
                    // Set the filename for the final ZIP file for Incremental backups.
                    case "incremental":
                        zipFileNameAndPath = this.incrementalBackupDirectory + @"\" + backupDirectoryName + "-INC.zip";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return zipFileNameAndPath;
        }

        /// <summary>
        /// Logs the backup job details to an SQL Database.
        /// </summary>
        private Result LogJobHistoryToSQL()
        {
            // Create a new Log object.
            Log log = new Log();

            // Get the job details and set the Log properties.
            log.BackupJobID  = this.backupJobID;
            log.BackupSource = this.backupSource;
            log.BackupType   = this.backupType;

            // Don't log the zip filename if it's not created, 
            // that is, there was nothing to backup.
            if (this.filesToCompress.Count() > 0)
            {
                log.BackupTarget = this.backupTargetZipFile;
            }
            else
            {
                log.BackupTarget = "No Changes";
            }

            // Try to write the log entry to the log database.
            Result result = log.LogJobHistoryToSQL();

            // Otherwise fail with a descriptive error.
            if (result == Result.Failed)
            {
                MessageBox.Show("Failed to log the backup job details to the database table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Delete empty directories created during the backup process.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// </summary>
        private Result RemoveEmptyDirectories()
        {
            try
            {
                switch (this.backupType)
                {
                    case "full":
                        break;
                    case "differential":
                        break;
                    case "incremental":
                        // If no Incremental backup was performed, delete the empty directory that was created.
                        if (Directory.Exists(this.incrementalBackupDirectory) && (this.filesToCompress.Count() == 0))
                        {
                            Directory.Delete(this.incrementalBackupDirectory, true);
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Select which files should be backed up based on the backup type and if the files have changed or not.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// </summary>
        private Result SelectFilesToCompress()
        {
            // Define and initialize main variables.
            List<string> filesInBackupDirectory = new List<string>();
            string[]     fullDirectories        = this.GetFullBackupDirectories();
            string[]     incrementalDirectories = { };

            // Get a list of all the files in the backup directory and its subdirectories.
            filesInBackupDirectory = FileIO.GetAllFilesInDirectory(this.backupSource);

            try
            {
                switch (this.backupType)
                {
                    case "full":
                        // Add a string containing the backup path using a wildcard to the list of files to backup.
                        // The wildcard means to backup all the files in the backup directory and its subdirectories.
                        this.filesToCompress.Add(this.backupSource + @"\*");

                        break;
                    case "differential":
                        // Check each file if it should be backed up or not (if it has changed).
                        foreach (string file in filesInBackupDirectory)
                        {
                            // Define and initialize main variables.
                            FileInfo        fileInfo                = new FileInfo(file);
                            DateTime        fileLastUpdated         = fileInfo.LastWriteTime;
                            DirectoryInfo   fullDirectoryInfo       = new DirectoryInfo(fullDirectories.Max());
                            DateTime        fullDirectoryCreated    = fullDirectoryInfo.CreationTime;

                            // Add files that have changed since the last Full backup, 
                            // to the list of files to backup.
                            if (DateTime.Compare(fileLastUpdated, fullDirectoryCreated) > 0)
                            {
                                this.filesToCompress.Add(file);
                            }
                        }

                        break;
                    case "incremental":
                        // Get a list of all directories containing Incremental backup jobs.
                        incrementalDirectories = Directory.GetDirectories(fullDirectories.Max() + @"\Incremental");

                        // Check each file if it should be backed up or not (if it has changed).
                        foreach (string file in filesInBackupDirectory)
                        {
                            // Define and initialize main variables.
                            FileInfo        fileInfo        = new FileInfo(file);
                            DateTime        fileLastUpdated = fileInfo.LastWriteTime;
                            DirectoryInfo   incrementalDirectoryInfo;
                            DateTime        incrementalDirectoryCreated;

                            // Use the (second) last Incremental backup for comparison 
                            // (excluding the empty directory we just created for this backup).
                            if (incrementalDirectories.Count() > 1)
                            {
                                incrementalDirectoryInfo = new DirectoryInfo(incrementalDirectories.Reverse().ElementAt(1));
                            }
                            // Otherwise, if no other Incremental backups exist, 
                            // use the last Full backup for comparison.
                            else
                            {
                                incrementalDirectoryInfo = new DirectoryInfo(fullDirectories.Max());
                            }

                            incrementalDirectoryCreated = incrementalDirectoryInfo.CreationTime;

                            // Add files that have changed since the last Incremental backup 
                            // or the last Full backup to be compressed/backed up.
                            if (DateTime.Compare(fileLastUpdated, incrementalDirectoryCreated) > 0)
                            {
                                this.filesToCompress.Add(file);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Start the actual backup process using 7zip to compress the files and directories.
        /// <para>A private method used internally by the Backup Class methods.</para>
        /// <para>Takes the following input parameter(s):</para>
        /// <para>@param zipFileNameAndPath String - Defines the name of the final output backup/zip file.</para>
        /// </summary>
        private Result StartCompress(string zipFileNameAndPath)
        {
            // Define and initialize main variables.
            Process sevenZip = new Process();

            try
            {
                foreach (string file in filesToCompress)
                {
                    // Set the compression application (7zip).
                    sevenZip.StartInfo.FileName = @"3rd\\7za.exe";

                    // Set the working directory, 
                    // this way we can provide relative paths to the backup directory
                    sevenZip.StartInfo.WorkingDirectory = this.backupSource;

                    // Append recursively files to the zipFileNameAndPath zip file.
                    sevenZip.StartInfo.Arguments = @"a -ssw -tzip -r -y """ + zipFileNameAndPath + @""" ";

                    // The file to be appended to the zip file.
                    sevenZip.StartInfo.Arguments += @"""" + file.Substring(this.backupSource.Length + 1) + @"""";

                    // Don't open any windows, just run in a shell in the background.
                    sevenZip.StartInfo.CreateNoWindow  = true;
                    sevenZip.StartInfo.UseShellExecute = false;

                    // Start the Compression process.
                    sevenZip.Start();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }
    }
}
