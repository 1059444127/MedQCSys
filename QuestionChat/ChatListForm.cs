using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.MedQC.QuestionChat.Utilities;
using Heren.Common.Libraries;
using Heren.MedQC.QuestionChat.Forms;
using Heren.Common.Controls.VirtualTreeView;
using EMRDBLib.Entity;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace Heren.MedQC.QuestionChat
{
    public partial class ChatListForm : Form
    {
        public ChatListForm()
        {
            TrayIconHandler.Instance.ShowTaskTray(this);
            InitializeComponent();
        }
        private DateTime m_dtLastQueryTime = DateTime.Now;
        private int m_nLastMessageCount = 0;
        private bool m_bMessageHandling = false;
        /// <summary>
        /// �������Ѽ���߳��������¼�(�߳�����ǰ����)
        /// </summary>
        internal void HandleTaskCheckStart()
        {
            this.LoadQcQuestionMessage();
            this.m_dtLastQueryTime = SysTimeHelper.Instance.Now;
            this.lblTaskTip.Text = "���ڸ��������б����Ժ�...";
            this.lblTaskTip.Update();
            TrayMessageForm.Instance.Hide();
            TrayIconHandler.Instance.StopTrayBlink();
            TrayIconHandler.Instance.ShowToolTipText("����ϵͳ���ڸ��������б�...");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            TrayIconHandler.Instance.CloseTaskTray();
            MessageHandler.Instance.StopCheckTimer();
            Application.Exit();
        }

        /// <summary>
        /// �����ʿ���Ϣ��Ϣ�б�
        /// </summary>
        internal void LoadQcQuestionMessage()
        {

            this.dataGridView1.Rows.Clear();
            try
            {
                if (MessageHandler.Instance.QcMsgChatLogList == null)
                    return;
                foreach (QcMsgChatLog item in MessageHandler.Instance.QcMsgChatLogList)
                {

                    int index = this.dataGridView1.Rows.Add();
                    DataGridViewRow row = this.dataGridView1.Rows[index];
                    if (item.ChatSendDate != item.DefaultTime)
                        row.Cells[this.colChatSendDate.Index].Value = item.ChatSendDate.ToString("yyyy-M-d HH:mm:ss");
                    row.Cells[this.colChatContent.Index].Value = item.ChatContent;
                    row.Cells[this.colSender.Index].Value = SystemParam.Instance.QChatArgs.UserType == "0" ? "�ʿؿ�" : item.Sender;
                    row.Cells[this.colListener.Index].Value = item.Listener;
                    row.Cells[this.colIsRead.Index].Value = item.IsRead ? "�Ѷ�" : "δ��";
                    row.Tag = item;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ChatListForm.LoadQcQuestionMessage" + ex.Message);
            }

        }
        /// <summary>
        /// ��ʾ�����б���
        /// </summary>
        internal void ShowTaskForm()
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
                NativeMethods.User32.ShowWindow(this.Handle, NativeConstants.SW_RESTORE);
            NativeMethods.User32.SetForegroundWindow(this.Handle);
            TrayIconHandler.Instance.StopTrayBlink();
            TrayMessageForm.Instance.Hide();
        }
        /// <summary>
        /// �������Ѽ���߳��ѽ����¼�(�߳̽��������)
        /// </summary>
        internal void HandleTaskCheckStop()
        {
            int count = MessageHandler.Instance.MessageCount;
            int interval = SystemConfig.Instance.Get(SystemData.ConfigKey.TASK_REFRESH_INTERVAL, 30);
            if (interval > 30)
                interval = 30;
            DateTime dtLastQueryTime = this.m_dtLastQueryTime.AddMinutes(interval);

            StringBuilder sbTaskTipText = new StringBuilder();
            sbTaskTipText.AppendFormat("ϵͳ���ռ���{0}��������Ϣ,", count);
            if (interval <= 0)
                sbTaskTipText.Append("ϵͳ�ѱ�����Ϊ����ʱˢ��,�������ֶ�ˢ��!");
            else
                sbTaskTipText.AppendFormat("��һ�ζ�ʱ����:{0}!", dtLastQueryTime);
            this.lblTaskTip.Text = sbTaskTipText.ToString();
            this.lblTaskTip.Update();
            TrayIconHandler.Instance.ShowToolTipText(this.lblTaskTip.Text);

            if (this.m_nLastMessageCount == count)
                return;
            this.m_nLastMessageCount = count;
            if (count <= 0)
                return;
            if (!this.ContainsFocus && SystemConfig.Instance.Get(SystemData.ConfigKey.TASK_POPUP_TIP, true))
                TrayMessageForm.Instance.Show(this);
            if (!this.ContainsFocus && SystemConfig.Instance.Get(SystemData.ConfigKey.TASK_ICON_BLINK, true))
                TrayIconHandler.Instance.StartTrayBlink();
        }
        /// <summary>
        /// ���������б���
        /// </summary>
        internal void HideTaskForm()
        {
            if (this.WindowState != FormWindowState.Minimized)
                this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeConstants.WM_SYSCOMMAND
                && (int)m.WParam == NativeConstants.SC_CLOSE)
            {
                this.HideTaskForm();
                return;
            }
            if (m.Msg == NativeConstants.WM_COPYDATA)
            {
                if (this.m_bMessageHandling)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
                this.m_bMessageHandling = true;
                this.m_nLastMessageCount = 0;
                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                MessageHandler.Instance.ChatListForm = this;
                MessageHandler.Instance.HandleStartupMessage(m.LParam);
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.m_bMessageHandling = false;
            }
            base.WndProc(ref m);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            QcMsgChatLog msg = this.dataGridView1.Rows[e.RowIndex].Tag as QcMsgChatLog;
            if (msg == null)
                return;
            QCQuestionChatForm form = new QCQuestionChatForm();
            form.PatientID = msg.PatientID;
            form.VisitID = msg.VisitID;
            form.Sender = msg.Listener;//�����Ի��򣬴˴������ߡ�������Ӧ����
            form.Listener = msg.Sender;
            form.MsgID = msg.MsgID;
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (this.chkSearch.Checked
                    )
                {
                    this.SearchHistoryMsg();
                }
                else
                {
                    if (MessageHandler.Instance.IsRunning)
                        MessageHandler.Instance.StopCheckTimer();
                    MessageHandler.Instance.StartCheckTimer(this);
                }
            }

        }

        private void fbtnRefresh_Click(object sender, EventArgs e)
        {
            if (MessageHandler.Instance.IsRunning)
                MessageHandler.Instance.StopCheckTimer();
            MessageHandler.Instance.StartCheckTimer(this);
        }

        private void fbtnSetting_Click(object sender, EventArgs e)
        {
            this.ShowSettingsForm();
        }

        private void ShowSettingsForm()
        {
            SettingsForm settingsForm = new SettingsForm();
            if (settingsForm.ShowDialog(this) != DialogResult.OK)
                return;
            if (!MessageHandler.Instance.IsRunning)
                MessageHandler.Instance.StartCheckTimer(this);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk.Checked)
            {
                this.dtpBeginTime.Visible = true;
                this.dtpEndTime.Visible = true;
                this.lblread.Visible = true;
                this.btnSearch.Visible = true;
            }
            else
            {
                this.dtpBeginTime.Visible = false;
                this.dtpEndTime.Visible = false;
                this.lblread.Visible = false;
                this.btnSearch.Visible = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchHistoryMsg();
        }

        private void SearchHistoryMsg()
        {
            if (this.chkSearch.Checked)
            {
                DateTime dtBeginTime = DateTime.Parse(this.dtpBeginTime.Value.ToString("yyyy-MM-dd"));
                DateTime dtEndTime = DateTime.Parse(this.dtpEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                List<QcMsgChatLog> QcMsgChatLogList = new List<QcMsgChatLog>();
                string szListener = SystemParam.Instance.QChatArgs.Sender;//��ǰ����������Ϣ�ļ�����
                short shRet = QcMsgChatAccess.Instance.GetQCMsgChatLogList(szListener, dtBeginTime, dtEndTime, ref QcMsgChatLogList);
                this.dataGridView1.Rows.Clear();
                try
                {
                    foreach (QcMsgChatLog item in QcMsgChatLogList)
                    {
                        if (!item.IsRead)
                            continue;
                        int index = this.dataGridView1.Rows.Add();
                        DataGridViewRow row = this.dataGridView1.Rows[index];
                        if (item.ChatSendDate != item.DefaultTime)
                            row.Cells[this.colChatSendDate.Index].Value = item.ChatSendDate.ToString("yyyy-M-d HH:mm:ss");
                        row.Cells[this.colChatContent.Index].Value = item.ChatContent;
                        row.Cells[this.colSender.Index].Value = SystemParam.Instance.QChatArgs.UserType == "0" ? "�ʿؿ�" : item.Sender;//��ǰ�û�Ϊҽ�����ʿؿ���������
                        row.Cells[this.colListener.Index].Value = item.Listener;
                        row.Cells[this.colIsRead.Index].Value = item.IsRead ? "�Ѷ�" : "δ��";
                        row.Tag = item;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ChatListForm.LoadQcQuestionMessage" + ex.Message);
                }
            }
        }


    }
}