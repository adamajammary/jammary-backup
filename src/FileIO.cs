using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace JammaryBackup
{
    /// <summary>
    /// Custom methods for File Input/Output Operations.
    /// </summary>
    public class FileIO
    {
        // Define private member properties.
        private static List<string> filesInDirectory = new List<string>();

        /// <summary>
        /// Replaces the default System.IO.Directory.GetFiles() method with Recursive option since it throws System.IO.UnAuthorizedAccessException exceptions and "Access Denied" errors when parsing directories like "My Documents" because they contain Junction Points.
        /// <para>This static method takes care of checking each file's attributes before returning it.</para>
        /// <para>Requires the following member properties:</para>
        /// <para>@param    directoryPath   String - Defines the path of the directory to retrieve files from.</para>
        /// <para>@return   A string list of all files found in the directory and sub directories.</para>
        /// </summary>
        public static List<string> GetAllFilesInDirectory(string directoryPath)
        {
            // Clear/Empty the list before starting.
            FileIO.filesInDirectory.Clear();
            
            // Use the internal helper method to get all files in the directory.
            FileIO.GetAllFilesInDirectoryInternal(directoryPath);

            // After adding files from the directory and all sub directories, return the list of files.
            return FileIO.filesInDirectory;
        }

        private static void GetAllFilesInDirectoryInternal(string directoryPath)
        {
            // Get the filesystem and security properties of the root directory.
            DirectoryInfo     directoryInfo        = new DirectoryInfo(directoryPath);
            DirectorySecurity directoryPermissions = new DirectorySecurity();

            try
            {
                // Check if the root directory is accessible.
                directoryPermissions = Directory.GetAccessControl(directoryPath);

                // Check if the root directory is a real folder or if it is a Junction/Reparse point.
                if ((directoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    // Get a list of all files in the root directory.
                    string[] rootFiles = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly);

                    // Check each file.
                    foreach (string file in rootFiles)
                    {
                        // Get the filesystem and security properties of the root files.
                        FileInfo     fileInfo        = new FileInfo(file);
                        FileSecurity filePermissions = new FileSecurity();

                        try
                        {
                            // Check if the root file is accessible.
                            filePermissions = File.GetAccessControl(file);

                            // Check if the file is a real file or if it is a Junction/Reparse point.
                            if ((fileInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                            {
                                // Add the file to the final list of files we will return.
                                FileIO.filesInDirectory.Add(fileInfo.FullName);
                            }
                        } catch (Exception e) when (e is UnauthorizedAccessException || e is InvalidOperationException || e is PathTooLongException) {
                            // Avoid application crashing because of unhandled exceptions.
                        }
                    }
                }

                try
                {
                    // Check if the root directory is accessible.
                    directoryPermissions = Directory.GetAccessControl(directoryPath);

                    // Check if the folder is a real folder or if it is a Junction/Reparse point.
                    if ((directoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {
                        // Get a list of all sub directories in the root directory.
                        string[] subDirectories = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);

                        // Check each sub directory.
                        foreach (string subDirectory in subDirectories)
                        {
                            // Get the filesystem and security properties of the sub directories.
                            DirectoryInfo     subDirectoryInfo        = new DirectoryInfo(subDirectory);
                            DirectorySecurity subDirectoryPermissions = new DirectorySecurity();

                            try
                            {
                                // Check if the sub directories are accessible.
                                subDirectoryPermissions = Directory.GetAccessControl(subDirectory);

                                // Check if the folder is a real folder or if it is a Junction/Reparse point.
                                if ((directoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                                {
                                    // Recursively check files in each sub directory.
                                    FileIO.GetAllFilesInDirectoryInternal(subDirectory);
                                }
                            } catch (Exception e) when (e is UnauthorizedAccessException || e is InvalidOperationException || e is PathTooLongException) {
                                // Avoid application crashing because of unhandled exceptions.
                            }
                        }
                    }
                } catch (Exception e) when (e is UnauthorizedAccessException || e is InvalidOperationException || e is PathTooLongException) {
                    // Avoid application crashing because of unhandled exceptions.
                }
            } catch (Exception e) when (e is UnauthorizedAccessException || e is InvalidOperationException || e is PathTooLongException) {
                // Avoid application crashing because of unhandled exceptions.
            }
        }
    }
}
