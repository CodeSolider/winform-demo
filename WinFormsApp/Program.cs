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
            MessageBox.Show("����������һ���������У������ظ�����");
            Thread.Sleep(1000);
            //�Զ��˳�
            Environment.Exit(1);
        }
    }
}
