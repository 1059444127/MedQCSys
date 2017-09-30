// ***********************************************************
// �����ʿ�ϵͳϵͳ��������.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Windows.Forms;
using System.ComponentModel;
using Heren.Common.Libraries;
using MedQCSys.Dialogs;
using MedQCSys.Utility;
using EMRDBLib;
using Heren.MedQC.CheckPoint;
using Heren.MedQC.Core;
using System.Collections.Generic;
using System.Reflection;

namespace MedQCSys
{
    public static class Startup
    {
        private const string SINGLE_INSTANCE_MONIKER_NAME = "mrqc";

        private static FileMapping m_FileMapping = null;
        /// <summary>
        /// ��ȡ�������ڴ��ļ�ӳ�����
        /// </summary>
        private static FileMapping FileMapping
        {
            get { return m_FileMapping; }
            set { m_FileMapping = value; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();
                LogManager.Instance.TextLogOnly = true;
                SystemConfig.Instance.ConfigFile = SystemParam.Instance.ConfigFile;
                System.Data.DataSet ds = null;
                if (EMRDBLib.DbAccess.CommonAccess.Instance.ExecuteQuery("select sysdate from dual", out ds) != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.ShowMessage("ϵͳ�޷��������ݿ⣬�����޷���������,����ϵ����Ա","�ڳ����Ŀ¼���ҵ�dbconfig.exe,�������ݿ���������");
                    return;
                }
                CommandHandler.Instance.Initialize();
                Startup.FileMapping = new FileMapping(SINGLE_INSTANCE_MONIKER_NAME);
                //  �汾���¼��
                string arg = (args != null && args.Length > 0) ? args[0] : null;
                Heren.MedQC.Upgrade.UpgradeHandler.Instance.Execute(arg);
                if (Startup.FileMapping.IsFirstInstance)
                {
                    Startup.StartNewInstance(args);
                }
                else
                {
                    Startup.HandleRunningInstance(args, 30);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("����ʧ��", ex);
            }
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Name))
                return null;
            string assemblyName = args.Name;
            int index = assemblyName.IndexOf(',');
            if (index <= 0)
                assemblyName = assemblyName + ".dll";
            else
                assemblyName = assemblyName.Substring(0, index) + ".dll";

            string searchPath = GlobalMethods.Misc.GetWorkingPath();
            List<string> files = GlobalMethods.IO.SearchDirectory(searchPath, assemblyName);
            if (files != null && files.Count > 0)
            {
                try
                {
                    return Assembly.LoadFile(files[0]);
                }
                catch(Exception ex) {
                    LogManager.Instance.WriteLog(ex.ToString());
                }
            }
            return null;
        }

        private static void StartNewInstance(string[] args)
        {
            //Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainFrame = new MainForm();
            mainFrame.FormClosed += new FormClosedEventHandler(MainForm_FormClosed);
            IntPtr hMainHandle = mainFrame.Handle;
            if (!Startup.FileMapping.WriteHandleValue(hMainHandle))
                return;
            mainFrame.Show();
            mainFrame.Update();
            if (mainFrame == null || mainFrame.IsDisposed)
            {
                Application.Exit();
                return;
            }
            GlobalMethods.UI.SetCursor(mainFrame, Cursors.Default);
            Application.Run(mainFrame);
            LogManager.Instance.Dispose();
        }

        private static void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Startup.FileMapping.Dispose(true);
        }

        private static void HandleRunningInstance(string[] args, int timeoutSeconds)
        {
            IntPtr hMainWnd = Startup.FileMapping.ReadHandleValue(timeoutSeconds);
            if (hMainWnd == IntPtr.Zero)
                return;
            try
            {
                if (NativeMethods.User32.IsIconic(hMainWnd))
                {
                    NativeMethods.User32.ShowWindow(hMainWnd, NativeConstants.SW_RESTORE);
                }
                NativeMethods.User32.SetForegroundWindow(hMainWnd);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("SysStartup.HandleRunningInstance", ex);
            }
        }
    }
}