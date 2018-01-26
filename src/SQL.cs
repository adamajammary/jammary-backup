using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JammaryBackup
{
    /// <summary>
    /// Performs SQL queries with the Database.
    /// <para>Supports inserting, updating, deleting and retrieving data.</para>
    /// <para>Takes care of connection handling (opening/closing).</para>
    /// </summary>
    class SQL
    {
        // Define private member properties.
        private SqlCeConnection sqlConnection;

        // Define public member properties.
        public string                     SqlConnectionString;
        public string                     SqlTableName;
        public string                     SqlQuery;
        public Dictionary<string, string> SqlCommandParameters = new Dictionary<string,string> ();

        /// <summary>
        /// Initializes and builds a connection to the Database.
        /// <para>A private method used internally by the SQL Class methods.</para>
        /// <para>@param SqlConnectionString String - Defines the SQL Database Connection String.</para>
        /// </summary>
        private Result InitConnection()
        {
            // Build the connection as long as the connection string has been set.
            if ((this.SqlConnectionString != "") && (this.SqlConnectionString != null))
            {
                this.sqlConnection = new SqlCeConnection(this.SqlConnectionString);
            }
            // Otherwise fail with a descriptive error.
            else
            {
                MessageBox.Show("SQL Connection String cannot be blank, \r\nplease set it to a valid value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }
            
            return Result.Success;
        }

        /// <summary>
        /// Initializes and creates all needed databases for this application.
        /// <para>This method is run once when the main form is constructed(the application is opened/started).</para>
        /// </summary>
        public Result InizializeDatabases()
        {
            // Define main variables.
            SqlCeEngine     sqlEngine;
            SqlCeCommand    sqlCommand;

            // Create the database used for Logging as long as it has not already been created.
            if (!File.Exists("Log.sdf"))
            {
                // Set the name of the new database.
                this.SqlConnectionString = JammaryBackup.Properties.Settings.Default.LogConnectionString;

                // Build an SQL Ce Engine using the above data source string.
                // The Engine object will be used later to create the new database.
                sqlEngine = new SqlCeEngine(this.SqlConnectionString);

                // Try to create the new database, and catch possible errors the method can throw.
                try
                {
                    sqlEngine.CreateDatabase();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Result.Failed;
                }

                // Connect to the database.
                this.InitConnection();

                // Open a new database connection.
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    this.sqlConnection.Open();
                }

                // Try to create a new table in the new database, and catch possible errors the method can throw.
                try
                {
                    // Create an SQL Ce Command object whích we will use to execute an SQL query.
                    sqlCommand = this.sqlConnection.CreateCommand();

                    // Assign the SQL Query/Command string we want to perform to the SQL Command object.
                    sqlCommand.CommandText = @"
                        CREATE TABLE job_history (
                            id int NOT NULL IDENTITY (1, 1), 
                            job_id uniqueidentifier, 
                            date_time datetime, 
                            source nvarchar(256), 
                            target nvarchar(256), 
                            type nvarchar(100), 
                            result nvarchar(100), 
                            PRIMARY KEY (id), 
                            UNIQUE (id)
                        )
                    ";

                    // Execute the SQL Command object using the above assigned query.
                    sqlCommand.ExecuteNonQuery();
                }
                catch (SqlCeException sqlException)
                {
                    MessageBox.Show(sqlException.Message, "SQL Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Result.Failed;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Result.Failed;
                }
                // Make sure we close the connection after completing the operation.
                finally
                {
                    sqlConnection.Close();
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// Updates existing data in a database table.
        /// <para>@param SqlConnectionString  String     - Defines the SQL Database Connection String.</para>
        /// <para>@param SqlQuery             String     - Defines the Database SQL Query.</para>
        /// <para>@param SqlCommandParameters Dictionary - Optional String, String Dictionary containing Query Parameters.</para>
        /// </summary>
        public Result UpdateData()
        {
            // Try to run the SQL query.
            Result result = this.InsertData();

            // Otherwise fail with a descriptive error.
            if (result == Result.Failed)
            {
                MessageBox.Show("Failed to update data in the database table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }

            return Result.Success;
        }

        /// <summary>
        /// Inserts data into a database table.
        /// <para>@param SqlConnectionString  String     - Defines the SQL Database Connection String.</para>
        /// <para>@param SqlQuery             String     - Defines the Database SQL Query.</para>
        /// <para>@param SqlCommandParameters Dictionary - Optional String, String Dictionary containing Query Parameters.</para>
        /// </summary>
        public Result InsertData()
        {
            // Connect to the database.
            this.InitConnection();

            // Define main variables.
            SqlCeCommand sqlCommand;

            // Open a new database connection.
            if (sqlConnection.State == ConnectionState.Closed)
            {
                this.sqlConnection.Open();
            }

            // Try to insert data into a database table, and catch possible errors the method can throw.
            try
            {
                // Make sure the required SQL Query is not blank.
                if ((this.SqlQuery != "") && (this.SqlQuery != null))
                {
                    // Create an SQL Ce Command object whích we will use to execute an SQL query.
                    sqlCommand = new SqlCeCommand(this.SqlQuery, this.sqlConnection);
                }
                // Otherwise fail with a descriptive error.
                else
                {
                    MessageBox.Show("SQL Query String cannot be blank, \r\nplease set it to a valid value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Result.Failed;
                }

                // When the SQL Query contaings parameters (@-variables), 
                // add the paremeter values to the SQL Command object.
                if (this.SqlCommandParameters.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> parameter in this.SqlCommandParameters)
                    {
                        sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }

                // Execute the SQL Command object using the above assigned query.
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlCeException sqlException)
            {
                MessageBox.Show(sqlException.Message, "SQL Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return Result.Failed;
            }
            // Make sure we close the connection after completing the operation.
            finally
            {
                sqlConnection.Close();
            }

            return Result.Success;
        }

        /// <summary>
        /// Retrieves data from a database table.
        /// <para>@param SqlConnectionString String - Defines the SQL Database Connection String.</para>
        /// <para>@param SqlQuery            String - Defines the Database SQL Query.</para>
        /// </summary>
        public DataSet GetData()
        {
            // Connect to the database.
            this.InitConnection();

            // Define main variables.
            SqlCeDataAdapter    sqlTableAdapter;
            DataSet             sqlDataSet      = null;

            // Open a new database connection.
            if (sqlConnection.State == ConnectionState.Closed)
            {
                this.sqlConnection.Open();
            }

            // Try to retrieve data from a database table, and catch possible errors the method can throw.
            try
            {
                // Make sure the required SQL Query is not blank.
                if ((this.SqlQuery != "") && (this.SqlQuery != null))
                {
                    // Create an SQL Ce Adapter object whích we will use to execute an SQL query for retrieval.
                    sqlTableAdapter = new SqlCeDataAdapter(this.SqlQuery, this.sqlConnection);
                }
                // Otherwise fail with a descriptive error.
                else
                {
                    //MessageBox.Show("SQL Query String cannot be blank, \r\nplease set it to a valid value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new System.ArgumentNullException("SqlQuery", "SQL Query String cannot be blank, \r\nplease set it to a valid value.");
                }
                
                // Create an SQL DataSet object which will hold the retrieved data records.
                sqlDataSet = new DataSet();
                
                // Fill the DataSet with the retrieved data records, and assign the DataSet to the SQL Adapter.
                sqlTableAdapter.Fill(sqlDataSet, this.SqlTableName);
            }
            catch (SqlCeException sqlException)
            {
                MessageBox.Show(sqlException.Message, "SQL Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Make sure we close the connection after completing the operation.
            finally
            {
                sqlConnection.Close();
            }

            // Return the DataSet and all the retrieved data records.
            return sqlDataSet;
        }
    }
}
