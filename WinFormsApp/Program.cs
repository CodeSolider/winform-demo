using System;
using System.Threading;
using System.Windows.Forms;

namespace WinFormsApp
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using Mutex mutex = new Mutex(initiallyOwned: false, "MachineTest", out bool isCreated);
            if (isCreated)
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmUpload());
                return;
            }
            MessageBox.Show("程序已在另一个窗口运行，不能重复运行");
            Thread.Sleep(1000);
            //自动退出
            Environment.Exit(1);
        }
    }
}
