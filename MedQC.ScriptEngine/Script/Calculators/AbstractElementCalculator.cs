// ***********************************************************
// ���Ӳ���ϵͳ����Ԫ���Զ�����ű�������.
// Creator: YangMingkun  Date:2011-11-10
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Libraries.Ftp;

namespace Heren.MedQC.ScriptEngine.Script
{
    public abstract class AbstractElementCalculator : IElementCalculator
    {
        #region "IRuntimeHandler"
        protected GetElementValueCallback m_getElementValueCallback = null;
        /// <summary>
        /// ��ȡ�����û�ȡԪ��ֵ�Ļص�ί��
        /// </summary>
        public virtual GetElementValueCallback GetElementValueCallback
        {
            get { return this.m_getElementValueCallback; }
            set { this.m_getElementValueCallback = value; }
        }

        protected SetElementValueCallback m_setElementValueCallback = null;
        /// <summary>
        /// ��ȡ����������Ԫ��ֵ�Ļص�ί��
        /// </summary>
        public virtual SetElementValueCallback SetElementValueCallback
        {
            get { return this.m_setElementValueCallback; }
            set { this.m_setElementValueCallback = value; }
        }

        protected HideElementTipCallback m_hideElementTipCallback = null;
        /// <summary>
        /// ��ȡ����������Ԫ����ʾ�Ļص�ί��
        /// </summary>
        public virtual HideElementTipCallback HideElementTipCallback
        {
            get { return this.m_hideElementTipCallback; }
            set { this.m_hideElementTipCallback = value; }
        }

        protected ShowElementTipCallback m_showElementTipCallback = null;
        /// <summary>
        /// ��ȡ��������ʾԪ����ʾ�Ļص�ί��
        /// </summary>
        public virtual ShowElementTipCallback ShowElementTipCallback
        {
            get { return this.m_showElementTipCallback; }
            set { this.m_showElementTipCallback = value; }
        }

        private ThreadInvokeCallback m_threadInvokeCallback = null;
        /// <summary>
        /// ��ȡ������ί�����̵߳��õĻص�ί��
        /// </summary>
        public ThreadInvokeCallback ThreadInvokeCallback
        {
            get { return this.m_threadInvokeCallback; }
            set { this.m_threadInvokeCallback = value; }
        }

        private ExecuteQueryCallback m_executeQueryCallback = null;
        /// <summary>
        /// ��ȡ������ִ��ָ����SQL��ѯ�Ļص�ί��
        /// </summary>
        public ExecuteQueryCallback ExecuteQueryCallback
        {
            get { return this.m_executeQueryCallback; }
            set { this.m_executeQueryCallback = value; }
        }

        private ExecuteUpdateCallback m_executeUpdateCallback = null;
        /// <summary>
        /// ��ȡ������ִ��ָ����SQL���µĻص�ί��
        /// </summary>
        public ExecuteUpdateCallback ExecuteUpdateCallback
        {
            get { return this.m_executeUpdateCallback; }
            set { this.m_executeUpdateCallback = value; }
        }

        protected GetSystemContextCallback m_getSystemContextCallback = null;
        /// <summary>
        /// ��ȡ�����û�ȡϵͳ�������������ݵĻص�ί��
        /// </summary>
        public virtual GetSystemContextCallback GetSystemContextCallback
        {
            get { return this.m_getSystemContextCallback; }
            set { this.m_getSystemContextCallback = value; }
        }

        private CustomEventCallback m_customEventCallback = null;
        /// <summary>
        /// ��ȡ�������Զ����¼��ص�ί��
        /// </summary>
        public CustomEventCallback CustomEventCallback
        {
            get { return this.m_customEventCallback; }
            set { this.m_customEventCallback = value; }
        }

        private LocateToElementCallback m_locateToElementCallback = null;
        /// <summary>
        /// ��ȡ�����ù�궨λ��ָ��Ԫ��λ�ûص�ί��
        /// </summary>
        public LocateToElementCallback LocateToElementCallback
        {
            get { return this.m_locateToElementCallback; }
            set { this.m_locateToElementCallback = value; }
        }

        /// <summary>
        /// �ṩ��������д�����㷽��
        /// </summary>
        /// <param name="szElementName">Ԫ������</param>
        public virtual void Calculate(string szElementName)
        {

        }

        /// <summary>
        /// �ṩ��������д�����㷽��
        /// </summary>
        /// <param name="param">��������</param>
        /// <param name="data">��������</param>
        public virtual bool Calculate(string param, object data)
        {
            return false;
        }
        #endregion

        #region"IDisposable"
        /// <summary>
        /// �ͷŽű���ռ�õ���Դ
        /// </summary>
        public void Dispose()
        {
            this.StopTimer();
            this.StopAllThreads();
            this.CloseAllFtpServices();
        }
        #endregion

        #region"��Ϣ����"
        /// <summary>
        /// ����һ������Ի���Ϣ��
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        protected virtual void ShowError(string message)
        {
            MessageBoxEx.ShowError(message);
        }

        /// <summary>
        /// ����һ������Ի���Ϣ��
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="details">��ϸ��Ϣ</param>
        protected virtual void ShowError(string message, string details)
        {
            MessageBoxEx.ShowError(message, details);
        }

        /// <summary>
        /// ����������Ϣ�Ի���
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        protected virtual void ShowWarning(string message)
        {
            MessageBoxEx.ShowWarning(message);
        }

        /// <summary>
        /// ����������Ϣ�Ի���
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="details">��ϸ��Ϣ</param>
        protected virtual void ShowWarning(string message, string details)
        {
            MessageBoxEx.ShowWarning(message, details);
        }

        /// <summary>
        /// ������ͨ��Ϣ�Ի���
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        protected virtual void ShowMessage(string message)
        {
            MessageBoxEx.ShowMessage(message);
        }

        /// <summary>
        /// ������ͨ��Ϣ�Ի���
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="details">��ϸ��Ϣ</param>
        protected virtual void ShowMessage(string message, string details)
        {
            MessageBoxEx.ShowMessage(message, details);
        }

        /// <summary>
        /// ����ȷ����Ϣ�Ի���
        /// </summary>
        /// <param name="confirm">��Ϣ����</param>
        protected virtual DialogResult ShowConfirm(string confirm)
        {
            return MessageBoxEx.ShowConfirm(confirm);
        }

        /// <summary>
        /// ����ȷ����Ϣ�Ի���
        /// </summary>
        /// <param name="confirm">��Ϣ����</param>
        /// <param name="details">��ϸ��Ϣ</param>
        protected virtual DialogResult ShowConfirm(string confirm, string details)
        {
            return MessageBoxEx.ShowConfirm(confirm, details);
        }

        /// <summary>
        /// �����Ƿ���Ϣ�Ի���
        /// </summary>
        /// <param name="confirm">��Ϣ����</param>
        protected virtual DialogResult ShowYesNo(string confirm)
        {
            return MessageBoxEx.ShowYesNo(confirm);
        }

        /// <summary>
        /// �����Ƿ���Ϣ�Ի���
        /// </summary>
        /// <param name="confirm">��Ϣ����</param>
        /// <param name="details">��ϸ��Ϣ</param>
        protected virtual DialogResult ShowYesNo(string confirm, string details)
        {
            return MessageBoxEx.ShowYesNo(confirm, details);
        }

        /// <summary>
        /// ����ѯ����Ϣ�Ի���
        /// </summary>
        /// <param name="question">��Ϣ����</param>
        protected virtual DialogResult ShowQuestion(string question)
        {
            return MessageBoxEx.ShowQuestion(question);
        }

        /// <summary>
        /// ����ѯ����Ϣ�Ի���
        /// </summary>
        /// <param name="question">��Ϣ����</param>
        /// <param name="details">��ϸ��Ϣ</param>
        protected virtual DialogResult ShowQuestion(string question, string details)
        {
            return MessageBoxEx.ShowQuestion(question, details);
        }
        #endregion

        #region"��ʱ���ӿ�"
        private System.Windows.Forms.Timer m_timer = null;
        /// <summary>
        /// ������ʱ��
        /// </summary>
        /// <param name="interval">ʱ��</param>
        public void StartTimer(int interval)
        {
            if (this.m_timer == null)
            {
                this.m_timer = new System.Windows.Forms.Timer();
                this.m_timer.Interval = interval;
                this.m_timer.Tick += new EventHandler(this.Timer_Tick);
            }
            if (!this.m_timer.Enabled)
                this.m_timer.Start();
        }

        /// <summary>
        /// ��ֹ�����ٶ�ʱ��
        /// </summary>
        public void StopTimer()
        {
            if (this.m_timer != null)
            {
                this.m_timer.Stop();
                this.m_timer.Dispose();
            }
            this.m_timer = null;
        }

        /// <summary>
        /// ��ʱ����ʱִ�еķ���
        /// </summary>
        protected virtual void TimerTick()
        {
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.TimerTick();
        }
        #endregion

        #region"���߳̽ӿ�"
        private Dictionary<string, Thread> m_threads = null;
        /// <summary>
        /// ����һ���߳�
        /// </summary>
        /// <param name="name">�߳�����</param>
        public void StartThread(string name)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return;

            if (this.m_threads == null)
                this.m_threads = new Dictionary<string, Thread>();
            this.StopThread(name);

            ParameterizedThreadStart threadStart =
                new ParameterizedThreadStart(this.ThreadCallback);
            try
            {
                Thread thread = new Thread(threadStart);
                thread.IsBackground = true;
                thread.Name = name;
                thread.Start(name);
                this.m_threads.Add(name, thread);
            }
            catch (Exception ex)
            {
                MessageBoxEx.ShowError("�޷������߳�!", ex.ToString());
            }
        }

        /// <summary>
        /// ֹͣ��ǰ���е��������߳�
        /// </summary>
        public void StopAllThreads()
        {
            if (this.m_threads == null || this.m_threads.Count <= 0)
                return;
            string[] names = new string[this.m_threads.Count];
            this.m_threads.Keys.CopyTo(names, 0);
            foreach (string name in names)
                this.StopThread(name);
        }

        /// <summary>
        /// ��ֹ�������߳�
        /// </summary>
        /// <param name="name">�߳�����</param>
        public void StopThread(string name)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return;
            if (!this.m_threads.ContainsKey(name))
                return;
            try
            {
                Thread thread = this.m_threads[name];
                if (this.IsThreadRunning(name))
                    thread.Abort();
                this.m_threads.Remove(name);
            }
            catch (Exception ex)
            {
                MessageBoxEx.ShowError("�޷���ֹ�߳�!", ex.ToString());
            }
        }

        /// <summary>
        /// �̻߳ص������ӿ�
        /// </summary>
        /// <param name="name">�߳�����</param>
        protected virtual void ThreadCallback(object name)
        {
        }

        /// <summary>
        /// �жϵ�ǰ�߳��Ƿ���������
        /// </summary>
        /// <param name="name">�߳�����</param>
        protected virtual bool IsThreadRunning(string name)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return false;
            if (!this.m_threads.ContainsKey(name))
                return false;
            Thread thread = this.m_threads[name];
            return (thread != null && thread.IsAlive);
        }

        /// <summary>
        /// ί�����̵߳��õ�ί��
        /// </summary>
        /// <param name="method">ί�з���</param>
        /// <param name="args">����</param>
        /// <returns>���ز���</returns>
        protected virtual object Invoke(Delegate method, params object[] args)
        {
            if (this.m_threadInvokeCallback != null)
                return this.m_threadInvokeCallback(method, args);
            return string.Empty;
        }
        #endregion

        #region"FTP����ӿ�"
        private Dictionary<string, FtpAccess> m_ftpAccessDict = null;
        /// <summary>
        /// ����һ���µ�FTP����
        /// </summary>
        /// <param name="name">FTP��������</param>
        /// <param name="ip">IP��ַ</param>
        /// <param name="port">�˿ں�</param>
        /// <param name="user">�û���</param>
        /// <param name="pwd">����</param>
        protected void CreateFtpService(string name, string ip, int port, string user, string pwd)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return;
            if (this.m_ftpAccessDict == null)
                this.m_ftpAccessDict = new Dictionary<string, FtpAccess>();
            if (!this.m_ftpAccessDict.ContainsKey(name))
            {
                FtpAccess ftpAccess = new FtpAccess();
                ftpAccess.FtpIP = ip;
                ftpAccess.FtpPort = port;
                ftpAccess.UserName = user;
                ftpAccess.Password = pwd;
                this.m_ftpAccessDict.Add(name, ftpAccess);
            }
        }

        /// <summary>
        /// ֹͣ��ǰ���е�����FTP����
        /// </summary>
        protected void CloseAllFtpServices()
        {
            if (this.m_ftpAccessDict == null || this.m_ftpAccessDict.Count <= 0)
                return;
            string[] names = new string[this.m_ftpAccessDict.Count];
            this.m_ftpAccessDict.Keys.CopyTo(names, 0);
            foreach (string name in names)
                this.CloseFtpService(name);
        }

        /// <summary>
        /// ��ֹ������FTP����
        /// </summary>
        /// <param name="name">FTP��������</param>
        protected void CloseFtpService(string name)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return;
            if (this.m_ftpAccessDict == null || this.m_ftpAccessDict.Count <= 0)
                return;
            if (this.m_ftpAccessDict.ContainsKey(name))
            {
                this.m_ftpAccessDict[name].CloseConnection();
                this.m_ftpAccessDict.Remove(name);
            }
        }

        /// <summary>
        /// �ϴ�ָ���ı����ļ���ftp������
        /// </summary>
        /// <param name="name">FTP����</param>
        /// <param name="name">�����ļ�</param>
        /// <param name="name">Զ���ļ�</param>
        protected virtual bool UploadFile(string name, string localFile, string remoteFile)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return false;
            if (this.m_ftpAccessDict == null || this.m_ftpAccessDict.Count <= 0)
                return false;
            if (!this.m_ftpAccessDict.ContainsKey(name))
                return false;
            bool success = false;
            FtpAccess ftpAccess = this.m_ftpAccessDict[name];
            if (ftpAccess.OpenConnection())
            {
                string remoteDirectory = GlobalMethods.IO.GetFilePath(remoteFile);
                if (ftpAccess.CreateDirectory(remoteDirectory))
                    success = ftpAccess.Upload(localFile, remoteFile);
            }
            ftpAccess.CloseConnection();
            return success;
        }

        /// <summary>
        /// ��ָ����ftp����������ָ�����ļ�������
        /// </summary>
        /// <param name="name">FTP����</param>
        /// <param name="name">Զ���ļ�</param>
        /// <param name="name">�����ļ�</param>
        protected virtual bool DownloadFile(string name, string remoteFile, string localFile)
        {
            if (GlobalMethods.Misc.IsEmptyString(name))
                return false;
            if (this.m_ftpAccessDict == null || this.m_ftpAccessDict.Count <= 0)
                return false;
            if (!this.m_ftpAccessDict.ContainsKey(name))
                return false;
            bool success = false;
            FtpAccess ftpAccess = this.m_ftpAccessDict[name];
            if (ftpAccess.OpenConnection())
            {
                string localDirectory = GlobalMethods.IO.GetFilePath(localFile);
                if (GlobalMethods.IO.CreateDirectory(localDirectory))
                    success = ftpAccess.Download(remoteFile, localFile);
            }
            ftpAccess.CloseConnection();
            return false;
        }
        #endregion

        /// <summary>
        /// ȫ���ַ�ת����ַ�.
        /// </summary>
        /// <param name="value">��ʼ�ַ���</param>
        /// <returns>ת����İ���ַ���</returns>
        /// <remarks>���ת������ÿ���ű������õ�,���Էŵ�������</remarks>
        protected virtual string SBCToDBC(string value)
        {
            return Heren.Common.Libraries.GlobalMethods.Convert.SBCToDBC(value);
        }

        /// <summary>
        /// ��ָ��˫������ֵ��������.
        /// </summary>
        /// <param name="value">˫������ֵ</param>
        /// <param name="digits">С��λ��</param>
        /// <returns>ת����İ���ַ���</returns>
        /// <remarks>���ת������ÿ���ű������õ�,���Էŵ�������</remarks>
        protected virtual double RoundValue(double value, int digits)
        {
            try
            {
                return Math.Round(value, digits, MidpointRounding.ToEven);
            }
            catch { return value; }
        }

        /// <summary>
        /// ��ָ����������ֵ��������.
        /// </summary>
        /// <param name="value">��������ֵ</param>
        /// <param name="digits">С��λ��</param>
        /// <returns>ת����İ���ַ���</returns>
        /// <remarks>���ת������ÿ���ű������õ�,���Էŵ�������</remarks>
        protected virtual float RoundValue(float value, int digits)
        {
            try
            {
                return (float)Math.Round(value, digits, MidpointRounding.ToEven);
            }
            catch { return value; }
        }

        /// <summary>
        /// �����ⲿӦ�ó���
        /// </summary>
        /// <param name="appFile">�����ļ�·��</param>
        /// <param name="args">���ò���</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool InvokeApp(string appFile, string args)
        {
            try
            {
                System.Diagnostics.Process.Start(appFile, args);
                return true;
            }
            catch (Exception ex)
            {
                Heren.Common.Libraries.LogManager.Instance.WriteLog(
                    "ElementCalculator.InvokeApplication",
                    new string[] { "appFile" }, new object[] { appFile },
                    "�޷������ⲿӦ�ó���", ex);
                return false;
            }
        }

        /// <summary>
        /// ��ȡ�༭��ϵͳ��ǰ����·��
        /// </summary>
        /// <param name="szStartupPath">��ǰ����·��</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool GetStartupPath(out string szStartupPath)
        {
            szStartupPath = Heren.Common.Libraries.GlobalMethods.Misc.GetWorkingPath();
            return true;
        }

        /// <summary>
        /// ִ��һ��SQL��ѯ
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>�Ƿ��ѯ�ɹ�</returns>
        protected virtual bool ExecuteQuery(string sql, out DataSet data)
        {
            data = null;
            if (this.m_executeQueryCallback != null)
                return this.m_executeQueryCallback.Invoke(sql, out data);
            return false;
        }

        /// <summary>
        /// ִ��ָ����һϵ�е�SQL����
        /// </summary>
        /// <param name="isProc">�����SQL�Ƿ��Ǵ洢����</param>
        /// <param name="sql">SQL�������</param>
        /// <returns>�Ƿ���³ɹ�</returns>
        protected virtual bool ExecuteUpdate(bool isProc, params string[] sql)
        {
            if (this.m_executeUpdateCallback != null)
                return this.m_executeUpdateCallback.Invoke(isProc, sql);
            return false;
        }

        /// <summary>
        /// ���ص�ǰԪ����ʾ
        /// </summary>
        /// <returns>ִ�н��</returns>
        protected virtual bool HideElementTip()
        {
            if (this.m_hideElementTipCallback != null)
                return this.m_hideElementTipCallback.Invoke();
            return false;
        }

        /// <summary>
        /// ��ʾ��ǰԪ����ʾ
        /// </summary>
        /// <param name="szTitle">��ʾ����</param>
        /// <param name="szTipText">��ʾ�ı�</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool ShowElementTip(string szTitle, string szTipText)
        {
            if (this.m_showElementTipCallback != null)
                return this.m_showElementTipCallback.Invoke(szTitle, szTipText);
            return false;
        }

        /// <summary>
        /// ����ָ��Ԫ�ص�ֵ
        /// </summary>
        /// <param name="szElementName">Ԫ������</param>
        /// <param name="szElementValue">Ԫ��ֵ</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool SetElementValue(string szElementName, string szElementValue)
        {
            if (this.m_setElementValueCallback != null)
                return this.m_setElementValueCallback.Invoke(szElementName, szElementValue);
            return false;
        }

        /// <summary>
        /// ��ȡָ��Ԫ�ص�ֵ
        /// </summary>
        /// <param name="szElementName">Ԫ������</param>
        /// <param name="szElementValue">���ص�Ԫ��ֵ</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool GetElementValue(string szElementName, out string szElementValue)
        {
            szElementValue = string.Empty;
            if (this.m_getElementValueCallback != null)
                return this.m_getElementValueCallback.Invoke(szElementName, out szElementValue);
            return false;
        }

        /// <summary>
        /// ����궨λ��ָ��Ԫ��
        /// </summary>
        /// <param name="szElementName">Ԫ������</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool LocateToElement(string szElementName)
        {
            if (this.m_locateToElementCallback != null)
                return this.m_locateToElementCallback.Invoke(szElementName);
            return false;
        }

        /// <summary>
        /// ��ȡϵͳ��������������
        /// </summary>
        /// <param name="szContextName">����������</param>
        /// <param name="objContextValue">���ص�������ֵ</param>
        /// <returns>ִ�н��</returns>
        protected virtual bool GetSystemContext(string szContextName, out object objContextValue)
        {
            objContextValue = string.Empty;
            if (this.m_getSystemContextCallback != null)
                return this.m_getSystemContextCallback.Invoke(szContextName, out objContextValue);
            return false;
        }

        /// <summary>
        /// ����һ���Զ����¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="name">�¼�����</param>
        /// <param name="param">�¼�����</param>
        /// <param name="data">�¼�����</param>
        /// <param name="result">�¼����</param>
        /// <returns>�Ƿ�ִ�гɹ�</returns>
        protected virtual bool RaiseCustomEvent(object sender
            , string name, object param, object data, ref object result)
        {
            if (this.m_customEventCallback != null)
                return this.m_customEventCallback.Invoke(sender, name, param, data, ref result);
            return false;
        }
    }
}
