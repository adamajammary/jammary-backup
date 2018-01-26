724 - 11:30:04 PM, Sunday, July 17, 2011

Added application icon.

720 - 10:56:31 PM, Sunday, July 17, 2011
- Restructured the code to separate long methods into smaller components.
- Increased the efficiency/performance of the BackgroundWorkerThread process.
- Restructured the backup job status/progress process to be more scalable and logical.
- Each backup job can be tracked in the Log SQL database table using a unique GUID.
- Improved the Differential/Incremental processes.

694 - 10:21:51 PM, Wednesday, June 01, 2011

Embedded manifest file to the project, and set the "requestedExecutionLevel" to "highestAvailable".

693 - 9:08:15 PM, Wednesday, June 01, 2011

Method: FileIO.GetAllFilesInDirectory
Bug:	System.IO.UnauthorizedAccessException exceptions thrown when accessing system directories 
	where the user does not have access.
Fix:	Added DirectorySecurity/FileSecurity and AccessControl checks for each directory and file.
Note:	Making the application run as administrator doesn't solve the problem, 
	since the first time the system (Vista/Windows 7 with UAC) is started 
	some System32 directories are not accessible even by the administrator until an administrator 
	has accessed it once manually, then the second time around it's accessible.

692 - 7:43:26 PM, Tuesday, May 31, 2011

Bug:	Target name was not logged.

Fixed:	Changed the conditional check for "NO CHANGES" in MainForm to only be true for Incremental checks.

691 - 7:00:14 PM, Tuesday, May 31, 2011

Fixed Incremental and Differential backup operations and logging.

663 - 4:09:00 AM, Monday, May 30, 2011

Added project files for Jammary Backup.

661 - 3:51:10 AM, Monday, May 30, 2011

Created the project 'JammaryBackup'
