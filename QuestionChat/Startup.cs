// *******************************************************************
// ������ϵͳ��������ڳ���,��������ϵͳ�����û���ʵ�����п���
// ע�⣺�Ƕ��û�����ʵ��,���������û�,��ÿ���û�������һ��ʵ��
// Author : YangMingkun, Date : 2011-10-21
// Copyright : Heren Health Services Co.,Ltd.
// *******************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.MedQC.QuestionChat.Forms;
using Heren.MedQC.QuestionChat.Utilities;
using System.IO;
using EMRDBLib;

namespace Heren.MedQC.QuestionChat
{
    public static class Startup
    {
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
            LogManager.Instance.TextLogOnly = true;
            //����EMRS����
            if (File.Exists(string.Format("{0}\\UserData\\MedDocSys.xml", Application.StartupPath)))
                SystemConfig.Instance.ConfigFile = string.Format("{0}\\UserData\\MedDocSys.xml", Application.StartupPath);
            else
                SystemConfig.Instance.ConfigFile = string.Format("{0}\\MedQCSys.xml", Application.StartupPath);
            MessageBoxEx.Caption = "�ʿط�������";
            // args = new string[] { ";;��־Ӣ;;1;0;" };
            // args = new string[] { "20140801;1;��־Ӣ;���Բ���;0;0;201512031412007641"};
            Startup.FileMapping = new FileMapping(SystemData.MappingName.QUESTION_CHAT_SYS);
            if (Startup.FileMapping.IsFirstInstance)
            {
                Startup.StartNewInstance(args);
            }
            else
            {
                Startup.HandleRunningInstance(args, 30);
            }
        }
        private static void HandleRunningInstance(string[] args, int timeoutSeconds)
        {
            IntPtr hMainWnd = Startup.FileMapping.ReadHandleValue(timeoutSeconds);
            if (hMainWnd == IntPtr.Zero)
                return;
            try
            {
                //���ʹ��ڲ�����Ϣ
                Startup.SendStartArgs(hMainWnd, args);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("Startup.HandleRunningProcess", ex);
            }
        }
        /// <summary>
        /// ��ʾ��¼�Ի���
        /// </summary>
        /// <returns>bool</returns>
        private static bool ShowLogin()
        {
            //������������������˳���,����Ҫ��¼
            LoginForm frmLogin = new LoginForm();
            return (frmLogin.ShowDialog() == DialogResult.OK);
        }
        private static bool SendStartArgs(IntPtr hMainWnd, string[] args)
        {
            StartupArgs argsHelper = new StartupArgs();
            try
            {
                if (argsHelper.ParseArrArgs(args) != SystemData.ReturnValue.OK)
                    return false;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("Startup.SendStartArgs", ex);
            }
            IntPtr lParam = argsHelper.ArgsDataHandle;
            return NativeMethods.User32.SendMessage(hMainWnd, NativeConstants.WM_COPYDATA, IntPtr.Zero, lParam) == 1;
        }
        private static void StartNewInstance(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ChatListForm form = new ChatListForm();
            GC.KeepAlive(form);
            IntPtr hMainWnd = form.Handle;
            if (!Startup.FileMapping.WriteHandleValue(hMainWnd))
                return;
            if (args != null && args.Length > 0)
            {
                //������������
                Startup.SendStartArgs(hMainWnd, args);
                Application.Run();
            }
            else
            {
                //��¼�Ƿ�ɹ�
                if (!Startup.ShowLogin())
                {
                    form.Close();
                }
                else
                {
                    MessageHandler.Instance.StartCheckTimer(form);
                    Application.Run(form);
                }

            }
            LogManager.Instance.Dispose();
            SysTimeHelper.Instance.Dispose();
            Startup.FileMapping.Dispose(true);
        }

    }
}