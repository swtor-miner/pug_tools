using System;
using System.Windows.Forms;

namespace tor_tools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Using Server GC: " + System.Runtime.GCSettings.IsServerGC);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Tools());
        }
    }
}
