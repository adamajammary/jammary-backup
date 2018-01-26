using System;
using System.Windows.Forms;

namespace JammaryBackup
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class MainProgram
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
