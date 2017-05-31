using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;
using EMRDBLib.Entity;
using EMRDBLib;

namespace Heren.MedQC.QuestionChat.Utilities
{
     /// <summary>
    /// ��������ϵͳ��������������
    /// </summary>
    internal class StartupArgs
    {
        private IntPtr m_hArgsData = IntPtr.Zero;
        /// <summary>
        /// ��ȡ�������������������ڴ��еľ��
        /// </summary>
        public IntPtr ArgsDataHandle
        {
            get { return this.m_hArgsData; }
        }

        private QChatArgs m_qChatArgs = null;
        /// <summary>
        /// ��ȡ������Ϣ�е��û���Ϣ
        /// </summary>
        public QChatArgs QChatArgs
        {
            get { return this.m_qChatArgs; }
        }
       

        /// <summary>
        /// �����ⲿϵͳ������ϵͳ����������;
        /// �����ɹ�������ñ����е�ArgsDataHandle����
        /// </summary>
        /// <param name="args">��������</param>
        public short ParseArrArgs(params string[] args)
        {
            if (args == null || args.Length <= 0)
                return SystemData.ReturnValue.PARAM_ERROR;

            StringBuilder sbArgsData = new StringBuilder();
            foreach (string arg in args)
            {
                sbArgsData.Append(arg);
            }
            this.m_hArgsData = GlobalMethods.Win32.StringToPtr(sbArgsData.ToString());
            if (this.m_hArgsData == IntPtr.Zero)
                return SystemData.ReturnValue.PARAM_ERROR;
            return SystemData.ReturnValue.OK;
        }

        public short ParsePtrArgs(IntPtr hArgsData)
        {
            string szArgsData = GlobalMethods.Win32.PtrToString(hArgsData);
            if (GlobalMethods.Misc.IsEmptyString(szArgsData))
            {
                LogManager.Instance.WriteLog("StartupArgs.ParsePtrArgs", "��������Ϊ��!");
                return SystemData.ReturnValue.PARAM_ERROR;
            }
            szArgsData = szArgsData.Replace(SystemData.StartupArgs.ESCAPE_CHAR
                , SystemData.StartupArgs.ESCAPED_CHAR).Trim();
            szArgsData = this.InitQChatArgs(szArgsData);
            return SystemData.ReturnValue.OK;
        }

        private string InitQChatArgs(string szArgsData)
        {
            this.m_qChatArgs = null;
            if (GlobalMethods.Misc.IsEmptyString(szArgsData))
                return string.Empty;
            int nGroupIndex = szArgsData.IndexOf(SystemData.StartupArgs.GROUP_SPLIT);
            string szUserData = null;
            if (nGroupIndex < 0)
                szUserData = szArgsData;
            else
                szUserData = szArgsData.Substring(0, nGroupIndex);

            if (GlobalMethods.Misc.IsEmptyString(szUserData))
                throw new Exception("�û���������Ϊ��!");
            this.m_qChatArgs = QChatArgs.GetQChatArgsFromStr(szUserData, SystemData.StartupArgs.FIELD_SPLIT);

            if (nGroupIndex < 0)
                return string.Empty;
            return (szArgsData.Length <= nGroupIndex + 1) ? string.Empty : szArgsData.Substring(nGroupIndex + 1);
        }

       
    }
}
