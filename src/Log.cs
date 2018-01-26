using System;
using System.Windows.Forms;

namespace JammaryBackup
{
    /// <summary>
    /// Takes care of all logging operations in the application.
    /// <para>Supports writing and retrieving log data from a log source.</para>
    /// <para>Supports logging to and from SQL Ce Databases and XML Files.</para>
    /// </summary>
    public class Log
    {
        // Define private member properties.
        private Guid   backupJobID;
        private string backupResult;
        private string backupSource;
        private string backupTarget;
        private string backupType;

        // Define public member properties.
        public Guid   BackupJobID  { set { backupJobID  = value; } }
        public string BackupSource { set { backupSource = value; } }
        public string BackupResult { set { backupResult = value; } }
        public string BackupTarget { set { backupTarget = value; } }
        public string BackupType   { set { backupType   = value; } }

        /// <summary>
        /// Logs the backup job completion status to an SQL Database.
        /// <para>@param backupJobID  Guid   - Defines the unique backup job ID.</para>
        /// <para>@param backupResult String - Defines the result of the action (Completed, Failed or Cancelled).</para>
        /// </summary>
        public Result LogJobCompletionStatusToSQL()
        {
            try
            {
                // Make sure the required member properties are not blank.
                if ((this.backupJobID != null) && ((this.backupResult != "")  && (this.backupResult != null)))
                {
                    // Create a new SQL object.
                    SQL sqlLog = new SQL();

                    // Set the SQL Connection string to connect to the Log database.
                    sqlLog.SqlConnectionString = Properties.Settings.Default.LogConnectionString;

                    // Set the Database Table to use.
                    sqlLog.SqlTableName = "job_history";

                    // Build the SQL Query string to update the log in the database table.
                    sqlLog.SqlQuery =  "UPDATE " + sqlLog.SqlTableName + " ";
                    sqlLog.SqlQuery += "SET result = @result ";
                    sqlLog.SqlQuery += "WHERE job_id = @job_id";

                    // Replace @-parameters in the query with actual values.
                    sqlLog.SqlCommandParameters.Add("@job_id", this.backupJobID.ToString());
                    sqlLog.SqlCommandParameters.Add("@result", this.backupResult);

                    // Try to update the log in the database table.
                    Result result = sqlLog.UpdateData();

                    // Otherwise fail with a descriptive error.
                    if (result == Result.Failed)
                    {
                        MessageBox.Show("Failed to write Log entry to the SQL Database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return Result.Failed;
                    }
                }
                // Otherwise fail with a descriptive error.
                else
                {
                    throw new System.ArgumentNullException("backupJobID and backupResult cannot be blank, \r\nplease set them to valid values.");
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
        /// Logs the backup job details to an SQL Database.
        /// <para>@param backupJobID  Guid   - Defines the unique backup job ID.</para>
        /// <para>@param backupSource String - Defines the path of what was backed up.</para>
        /// <para>@param backupTarget String - Defines the location where the source was saved.</para>
        /// <para>@param backupType   String - Defines the type of backup (full, differential or incremenetal).</para>
        /// </summary>
        public Result LogJobHistoryToSQL()
        {
            try
            {
                // Make sure the required member properties are not blank.
                if (
                    (this.backupJobID   != null) &&
                    ((this.backupSource != "")   && (this.backupSource != null)) &&
                    ((this.backupTarget != "")   && (this.backupTarget != null)) &&
                    ((this.backupType   != "")   && (this.backupType   != null))
                ) {
                    // Create a new SQL object.
                    SQL sqlLog = new SQL();

                    // Set the SQL Connection string to connect to the Log database.
                    sqlLog.SqlConnectionString = Properties.Settings.Default.LogConnectionString;

                    // Set the Database Table to use.
                    sqlLog.SqlTableName = "job_history";

                    // Build the SQL Query string to write the log to the database table.
                    sqlLog.SqlQuery  = "INSERT INTO " + sqlLog.SqlTableName + " ";
                    sqlLog.SqlQuery += "(job_id, date_time, source, target, type, result) ";
                    sqlLog.SqlQuery += "VALUES (@job_id, @date_time, @source, @target, @type, @result)";

                    // Replace @-parameters in the query with actual values.
                    sqlLog.SqlCommandParameters.Add("@job_id", this.backupJobID.ToString());
                    sqlLog.SqlCommandParameters.Add("@date_time", DateTime.Now.ToString());
                    sqlLog.SqlCommandParameters.Add("@source", this.backupSource);
                    sqlLog.SqlCommandParameters.Add("@target", this.backupTarget);
                    sqlLog.SqlCommandParameters.Add("@type", this.backupType);
                    sqlLog.SqlCommandParameters.Add("@result", "Started");

                    // Try to write the log entry to the database table.
                    Result result = sqlLog.InsertData();

                    // Otherwise fail with a descriptive error.
                    if (result == Result.Failed)
                    {
                        MessageBox.Show("Failed to write Log entry to the SQL Database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return Result.Failed;
                    }
                }
                // Otherwise fail with a descriptive error.
                else
                {
                    throw new System.ArgumentNullException("backupJobID, backupSource, backupTarget, backupType and backupResult cannot be blank, \r\nplease set them to valid values.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        //public CompletionStatus LogJobHistoryToXML()
        //{
        //    /**
        //    * Writes the log to an XML File. \r\n
        //    * \r\n
        //    * Requires the following member properties: \r\n
        //    * @param backupSource   String      - Defines the path of what was backed up.
        //    * @param backupTarget   String      - Defines the location where the source was saved.
        //    * @param backupType     String      - Defines the type of backup (full, differential or incremenetal).
        //    * @param backupResult   String      - Defines the result of the action (Completed, Failed or Cancelled).
        //    */

        //    try
        //    {
        //        // Make sure the required member properties are not blank.
        //        if (
        //            ((this.backupSource != "") && (this.backupSource != null)) &&
        //            ((this.backupTarget != "") && (this.backupTarget != null)) &&
        //            ((this.backupType != "") && (this.backupType != null)) &&
        //            ((this.backupResult != "") && (this.backupResult != null))
        //            )
        //        {
        //            // Set the log time to the current time.
        //            string currentTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss z");

        //            // Set the path and filename of the XML log file.
        //            string logPath = "log";
        //            string logFile = "JobHistory.xml";
        //            string logPathAndFile = String.Format(@"{0}\{1}", logPath, logFile);

        //            // Create the folder structure if has not been created yet.
        //            if (!Directory.Exists(logPath))
        //            {
        //                Directory.CreateDirectory(logPath);
        //            }

        //            // Try to create create a new XML log file, 
        //            // and catch possible errors the method can throw.
        //            if (!File.Exists(logPathAndFile))
        //            {
        //                try
        //                {
        //                    // Create an XmlTextWriter object to write to the XML file.
        //                    XmlTextWriter xmlWriter = new XmlTextWriter(logPathAndFile, Encoding.UTF8);

        //                    // Write the XML parts and a main element for the XML file.
        //                    xmlWriter.WriteStartDocument();

        //                    xmlWriter.WriteStartElement("BackupJobHistory");
        //                    xmlWriter.WriteString("");
        //                    xmlWriter.WriteEndElement();

        //                    xmlWriter.WriteEndDocument();

        //                    // Close the XmlTextWriter object connection.
        //                    xmlWriter.Close();
        //                }
        //                catch (Exception exception)
        //                {
        //                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //                    return CompletionStatus.Failed;
        //                }
        //            }

        //            // After the XML file has been created,
        //            // try to write/add the log entry to the XML log file, 
        //            // and catch possible errors the method can throw.
        //            try
        //            {
        //                // Open the XML file as an XmlDocument object.
        //                XmlDocument xmlDoc = new XmlDocument();
        //                xmlDoc.Load(logPathAndFile);

        //                // Set the Root Element of the XML document.
        //                XmlNode xmlRoot = xmlDoc.DocumentElement;

        //                // Write the log entry elements to the XML document.

        //                XmlNode xmlBackupJob = xmlDoc.CreateElement("BackupJob");
        //                XmlAttribute xmlDateAndTime = xmlDoc.CreateAttribute("DateAndTime");
        //                xmlDateAndTime.Value = currentTime;
        //                xmlBackupJob.Attributes.Append(xmlDateAndTime);
        //                xmlRoot.AppendChild(xmlBackupJob);

        //                XmlNode xmlBackupSource = xmlDoc.CreateElement("BackupSource");
        //                xmlBackupSource.InnerText = this.backupSource;
        //                xmlBackupJob.AppendChild(xmlBackupSource);

        //                XmlNode xmlBackupTarget = xmlDoc.CreateElement("BackupTarget");
        //                xmlBackupTarget.InnerText = this.backupTarget;
        //                xmlBackupJob.AppendChild(xmlBackupTarget);

        //                XmlNode xmlBackupType = xmlDoc.CreateElement("BackupType");
        //                xmlBackupType.InnerText = this.backupType;
        //                xmlBackupJob.AppendChild(xmlBackupType);

        //                XmlNode xmlBackupResult = xmlDoc.CreateElement("BackupResult");
        //                xmlBackupResult.InnerText = this.backupResult;
        //                xmlBackupJob.AppendChild(xmlBackupResult);

        //                // Save and close the XmlDocument object.
        //                xmlDoc.Save(logPathAndFile);
        //            }
        //            catch (Exception exception)
        //            {
        //                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //                return CompletionStatus.Failed;
        //            }
        //        }
        //        // Otherwise fail with a descriptive error.
        //        else
        //        {
        //            throw new System.ArgumentNullException("backupSource, backupTarget, backupType and backupResult cannot be blank, \r\nplease set them to valid values.");
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        return CompletionStatus.Failed;
        //    }

        //    return CompletionStatus.Success;
        //}
    }
}
