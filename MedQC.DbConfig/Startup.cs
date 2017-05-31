using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Heren.MedQC.DbConfig
{
    static class Startup
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //string szCommonDllPath = Application.StartupPath + "\\Common.Libraries.dll";
            string szCommonDllPath = AppDomain.CurrentDomain.BaseDirectory + "\\Common.Libraries.dll";
            if (!System.IO.File.Exists(szCommonDllPath))
            {
                string szTipText = string.Format("δ�ҵ���{0}���ļ�", szCommonDllPath);
                MessageBox.Show(szTipText, "���ù���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new MainForm());
        }
    }
}