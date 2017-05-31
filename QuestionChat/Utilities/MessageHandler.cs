using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;
using System.Windows.Forms;
using System.Threading;
using Heren.MedQC.QuestionChat.Forms;
using EMRDBLib.Entity;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace Heren.MedQC.QuestionChat.Utilities
{
    public class MessageHandler
    {
        public delegate void UpdateCheckProgressInvoker(int count, int index);

        private System.Windows.Forms.Timer m_timer = null;
        private Thread m_thread = null;
        private static MessageHandler m_instance = null;
        /// <summary>
        /// ��ȡ��Ϣ�ռ�����ʵ��
        /// </summary>
        public static MessageHandler Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new MessageHandler();
                return m_instance;
            }
        }
        private ChatListForm m_chatListForm = null;
        /// <summary>
        /// ��ȡ�����ù�����������Ϣ����
        /// </summary>
        public ChatListForm ChatListForm
        {
            get { return this.m_chatListForm; }
            set { this.m_chatListForm = value; }
        }

        private bool m_bIsRunning = false;
        /// <summary>
        /// ��ȡ��ǰ�Ƿ�����ִ����Ϣ������
        /// </summary>
        public bool IsRunning
        {
            get { return this.m_bIsRunning; }
        }
        /// <summary>
        /// ��ȡ��ǰ��ʾ����Ϣ����
        /// </summary>
        public int MessageCount
        {
            get
            {
                if (m_qcMsgChatLogList == null)
                    return 0;
                return this.m_qcMsgChatLogList.Count;
            }
        }

        private List<QcMsgChatLog> m_qcMsgChatLogList = null;
        /// <summary>
        /// �ʿ�������Ϣ�б�
        /// </summary>
        public List<QcMsgChatLog> QcMsgChatLogList
        {
            get { return this.m_qcMsgChatLogList; }
        }

        /// <summary>
        /// ������Ϣ����ʱ��
        /// </summary>
        public void StartCheckTimer(ChatListForm form)
        {
            if (this.m_chatListForm != form)
                this.m_chatListForm = form;

            //��������һ�μ��
            this.StartTaskCheckThread();

            //��ȡ��ʱʱ������
            string key = SystemData.ConfigKey.TASK_REFRESH_INTERVAL;
            int interval = SystemConfig.Instance.Get(key, 30);
            if (interval <= 0)
            {
                if (this.m_timer != null)
                    this.m_timer.Enabled = false;
                return;
            }
            if (interval > 30) interval = 30;

            //��ʼ��Timer��ʱ��
            if (this.m_timer == null)
            {
                this.m_timer = new System.Windows.Forms.Timer();
                this.m_timer.Tick +=
                    delegate { this.StartTaskCheckThread(); };
            }
            this.m_timer.Interval = interval * 60 * 1000;
            if (!this.m_timer.Enabled) this.m_timer.Enabled = true;
        }
        /// <summary>
        /// �����������߳�
        /// </summary>
        private void StartTaskCheckThread()
        {
            if (this.IsRunning)
                return;
            this.m_bIsRunning = true;
            try
            {
                this.m_thread = new Thread(new ThreadStart(this.StartTaskCheck));
                this.m_thread.IsBackground = true;
                this.m_thread.Start();
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("TaskHandler.StartTaskCheckThread", ex);
            }
        }
        /// <summary>
        /// ��ʼһ�����������
        /// </summary>
        private void StartTaskCheck()
        {
            this.HandleTaskCheckStart();

            ////ִ�в������ʿ����ⷴ����Ϣ
            this.ExecuteQcQuestionCheck();

            this.HanldeTaskCheckStop();
        }
        /// <summary>
        /// �����ʿ����ⷴ���������
        /// </summary>
        private void ExecuteQcQuestionCheck()
        {
            if (this.m_chatListForm == null || this.m_chatListForm.IsDisposed)
                return;
            if (SystemParam.Instance.QChatArgs == null)
                return;

            if (this.m_qcMsgChatLogList == null)
                this.m_qcMsgChatLogList = new List<QcMsgChatLog>();
            this.m_qcMsgChatLogList.Clear();
            short shRet;
            //�ҳ�������ǰ�û�δ������Ϣ[��ǰ�������Ǳ��˵Ľ����߶���]
            string szListener = SystemParam.Instance.QChatArgs.Sender;
            bool bIsRead = false;
            shRet = QcMsgChatAccess.Instance.GetQCMsgChatLogList(szListener, bIsRead, ref this.m_qcMsgChatLogList);
            if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
                this.m_chatListForm.Invoke(new MethodInvoker(this.m_chatListForm.LoadQcQuestionMessage));
        }
        /// <summary>
        /// ������������ǰ����
        /// </summary>
        private void HandleTaskCheckStart()
        {
            this.m_bIsRunning = true;

            if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
            {
                this.m_chatListForm.Invoke(
                    new MethodInvoker(this.m_chatListForm.HandleTaskCheckStart));
            }
            //PatientHandler.Instance.Initialize(this.m_chatListFormorm);
        }
        /// <summary>
        /// ����������Ϣ����������
        /// </summary>
        private void HanldeTaskCheckStop()
        {
            this.m_bIsRunning = false;
            if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
                this.m_chatListForm.Invoke(new MethodInvoker(this.m_chatListForm.HandleTaskCheckStop));
        }
        /// <summary>
        /// �������������е�ϵͳ������Ϣ����
        /// </summary>
        /// <param name="hSatrtupData">������Ϣ����</param>
        public void HandleStartupMessage(IntPtr hSatrtupData)
        {
            StartupArgs argsHelper = new StartupArgs();
            try
            {
                argsHelper.ParsePtrArgs(hSatrtupData);
            }
            catch (Exception ex)
            {
                TrayIconHandler.Instance.ShowTrayPopupTip("�������������", ex.Message);
                return;
            }
            if (argsHelper.QChatArgs.ArgType == "1")
                this.SwtichUserInfo(argsHelper.QChatArgs);
            else
            {
                SystemParam.Instance.QChatArgs = argsHelper.QChatArgs;
                TrayIconHandler.Instance.CloseTaskTray();
                QCQuestionChatForm form = new QCQuestionChatForm();
                form.PatientID = argsHelper.QChatArgs.PatientID;
                form.VisitID = argsHelper.QChatArgs.VisitID;
                form.Sender = argsHelper.QChatArgs.Sender;
                form.Listener = argsHelper.QChatArgs.Listener;
                form.MsgID = argsHelper.QChatArgs.MsgID;
                form.Show();
            }
        }
        /// <summary>
        /// �л���ǰ�û���Ϣ�Լ�������Ϣ
        /// </summary>
        /// <param name="userInfo">�û���Ϣ</param>
        /// <param name="patientInfo">������Ϣ</param>
        /// <param name="visitInfo">������Ϣ</param>
        private void SwtichUserInfo(QChatArgs qChatArgs)
        {
            if (this.m_chatListForm == null || this.m_chatListForm.IsDisposed)
                return;
            if (qChatArgs == null)
                return;

            //����û���Ϣ�����˸ı�
            bool bQChatArgshanged = false;
            if (SystemParam.Instance.QChatArgs == null
                || SystemParam.Instance.QChatArgs.Listener != qChatArgs.Listener)
            {
                bQChatArgshanged = true;
                this.StopCheckTimer();
                SystemParam.Instance.QChatArgs = qChatArgs.Clone() as QChatArgs;
            }

            string key = SystemData.ConfigKey.TASK_AUTO_POPOP_MESSAGE;
            bool bAutoPopupMessage = SystemConfig.Instance.Get(key, true);

            //����û���Ϣ�����˸ı�,��ô������������
            if (bQChatArgshanged)
            {
                if (this.IsRunning)
                    this.StopCheckTimer();
                this.StartCheckTimer(this.m_chatListForm);
            }
        }
        /// <summary>
        /// ֹͣ��Ϣ����ʱ��
        /// </summary>
        public void StopCheckTimer()
        {
            if (this.m_timer != null)
            {
                this.m_timer.Enabled = false;
                this.m_timer.Dispose();
                this.m_timer = null;
            }
            this.StopTaskCheck();
        }
        /// <summary>
        /// ֹͣ��ǰ���ڽ��е�������
        /// </summary>
        private void StopTaskCheck()
        {
            bool bIsRunning = this.m_bIsRunning;
            this.m_bIsRunning = false;
            try
            {
                if (this.m_thread != null)
                    this.m_thread.Abort();
                this.m_thread = null;

            }
            catch (Exception ex)
            {
                this.m_thread = null;
                LogManager.Instance.WriteLog("TaskHandler.StopCheck", ex);
            }
            if (bIsRunning) this.HanldeTaskCheckStop();
        }
    }
}
