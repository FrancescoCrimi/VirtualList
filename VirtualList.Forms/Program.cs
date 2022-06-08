using System;
using System.Windows.Forms;

namespace VirtualList.Forms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Net 6.0
            //ApplicationConfiguration.Initialize();

            // Net 5.0
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainView());
        }
    }
}