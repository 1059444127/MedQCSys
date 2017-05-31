// ***********************************************************
// �����ĵ�����ϵͳ����ͼ�괦�����
// Creator:YangMingkun  Date:2009-10-24
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using Heren.Common.Libraries;

namespace Heren.MedQC.QuestionChat.Utilities
{
    internal class TrayIconHandler
    {
        private static TrayIconHandler m_instance = null;
        /// <summary>
        /// ��ȡ��ǰ�û��Ĳ��˹�������ʵ��
        /// </summary>
        public static TrayIconHandler Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new TrayIconHandler();
                return m_instance;
            }
        }
        private TrayIconHandler()
        {
        }

        private NotifyIcon m_taskTray = null;
        private ChatListForm m_chatListForm = null;

        //ͼ��������ر���
        private Thread m_BlinkThread = null;
        private bool m_bTrayBlink = false;
        private bool m_bShowDefaultIcon = false;

        public void ShowTaskTray(ChatListForm chatListForm)
        {
            if (chatListForm == null || chatListForm.IsDisposed)
                throw new Exception("ChatListForm�������ɷ���!");
            this.m_chatListForm = chatListForm;

            if (this.m_taskTray == null)
                this.m_taskTray = new NotifyIcon();
            if (!this.m_taskTray.Visible)
                this.m_taskTray.Visible = true;
            this.LoadPopupMenu();
            this.m_taskTray.Icon = Heren.MedQC.QuestionChat.Properties.Resources.TaskTray1;
            this.m_taskTray.Text = "�ʿ����⹵ͨƽ̨";
            this.m_taskTray.DoubleClick -= new EventHandler(this.TaskTray_DoubleClick);
            this.m_taskTray.DoubleClick += new EventHandler(this.TaskTray_DoubleClick);
            this.m_taskTray.BalloonTipClicked -= new EventHandler(this.TaskTray_BalloonTipClicked);
            this.m_taskTray.BalloonTipClicked += new EventHandler(this.TaskTray_BalloonTipClicked);
        }

        public void LoadPopupMenu()
        {
            if (this.m_taskTray == null || !this.m_taskTray.Visible)
                return;
            if (this.m_taskTray.ContextMenuStrip == null)
                this.m_taskTray.ContextMenuStrip = new ContextMenuStrip();
            if (this.m_taskTray.ContextMenuStrip.Items.Count > 0)
                return;

            ToolStripMenuItem mnuMyTask = new ToolStripMenuItem();
            mnuMyTask.Text = "�ҵ��ʿ���Ϣ";
            mnuMyTask.Font = new Font(mnuMyTask.Font, FontStyle.Bold);
            mnuMyTask.Click += new EventHandler(this.mnuMyTask_Click);
            this.m_taskTray.ContextMenuStrip.Items.Add(mnuMyTask);

            ToolStripMenuItem mnuExitSys = new ToolStripMenuItem();
            mnuExitSys.Text = "�˳�����ϵͳ";
            mnuExitSys.Click += new EventHandler(this.mnuExitSys_Click);
            this.m_taskTray.ContextMenuStrip.Items.Add(mnuExitSys);

            this.m_taskTray.ContextMenuStrip.BringToFront();
            this.m_taskTray.ContextMenuStrip.Opening += new CancelEventHandler(this.ContextMenuStrip_Opening);
        }

        public void CloseTaskTray()
        {
            if (this.m_bTrayBlink)
                this.StopTrayBlink();

            if (this.m_taskTray == null)
                return;
            this.m_taskTray.Visible = false;
            this.m_taskTray.Dispose();
        }

        public void StartTrayBlink()
        {
            this.m_BlinkThread = new Thread(new ThreadStart(this.RunTaryIconBlinkThread));
            this.m_bTrayBlink = true;
            try
            {
                this.m_BlinkThread.Start();
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("�޷�����ͼ�������������!", ex);
            }
        }

        private void RunTaryIconBlinkThread()
        {
            if (this.m_taskTray == null || !this.m_taskTray.Visible)
                return;
            while (this.m_bTrayBlink)
            {
                if (this.m_bShowDefaultIcon)
                    this.m_taskTray.Icon = Heren.MedQC.QuestionChat.Properties.Resources.TaskTray1;
                else
                    this.m_taskTray.Icon = Heren.MedQC.QuestionChat.Properties.Resources.TaskTray2;
                Thread.Sleep(500);
                this.m_bShowDefaultIcon = !this.m_bShowDefaultIcon;
            }
            this.m_taskTray.Icon = Heren.MedQC.QuestionChat.Properties.Resources.TaskTray1;
        }

        public void StopTrayBlink()
        {
            this.m_bTrayBlink = false;
            try
            {
                if (this.m_BlinkThread != null)
                    this.m_BlinkThread.Abort();
            }
            catch { }
            this.m_BlinkThread = null;
            if (this.m_taskTray == null || !this.m_taskTray.Visible)
                return;
            this.m_taskTray.Icon = Heren.MedQC.QuestionChat.Properties.Resources.TaskTray1;
        }

        public void ShowTrayPopupTip(string szTipTitle, string szTipText)
        {
            if (this.m_taskTray == null || GlobalMethods.Misc.IsEmptyString(szTipText))
                return;
            this.m_taskTray.ShowBalloonTip(1000, szTipTitle, szTipText, ToolTipIcon.Info);
        }

        public void ShowToolTipText(string szTipText)
        {
            if (this.m_taskTray == null || GlobalMethods.Misc.IsEmptyString(szTipText))
                return;
            this.m_taskTray.Text = szTipText;
        }

        private void TaskTray_DoubleClick(object sender, System.EventArgs e)
        {
            if (Control.MouseButtons != MouseButtons.Left)
                return;
            this.StopTrayBlink();
            if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
                this.m_chatListForm.ShowTaskForm();
        }

        private void TaskTray_BalloonTipClicked(object sender, System.EventArgs e)
        {
            this.StopTrayBlink();
            if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
                this.m_chatListForm.ShowTaskForm();
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cmenuStrip = sender as ContextMenuStrip;
            if (cmenuStrip != null && !cmenuStrip.IsDisposed)
                cmenuStrip.BringToFront();
        }

        private void mnuMyTask_Click(object sender, System.EventArgs e)
        {
            if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
                this.m_chatListForm.ShowTaskForm();
        }

        private void mnuExitSys_Click(object sender, System.EventArgs e)
        {
            StringBuilder sbMsgInfo = new StringBuilder();
            sbMsgInfo.AppendLine("����ʱЧ����ϵͳ���ڱ��رա�����");
            sbMsgInfo.Append("�رպ�ϵͳ���������Ѳ�����д���Ƿ�رգ�");
            if (MessageBoxEx.ShowConfirm(sbMsgInfo.ToString()) == DialogResult.OK)
            {
                if (this.m_chatListForm != null && !this.m_chatListForm.IsDisposed)
                    this.m_chatListForm.Close();
                Application.Exit();
            }
        }
    }
}
