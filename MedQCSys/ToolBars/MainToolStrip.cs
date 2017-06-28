// ***********************************************************
// �����ʿ�ϵͳ���������������ؼ�.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;

using EMRDBLib;
using Heren.MedQC.Core;

namespace MedQCSys.ToolBars
{
    internal class MainToolStrip : MqsToolStripBase
    {
        #region"����������ʼ��"
        private ToolStripButton toolbtnShowPatientList;
        private ToolStripButton toolbtnCheckDocTime;
        private ToolStripButton toolbtnStatByBugs;
        private ToolStripButton toolbtnQCMsgTemplet;
        private ToolStripButton toolbtnQCEventTypes;
        private ToolStripButton toolbtnShowQuestionList;
        private ToolStripButton toolbtnHideAllDockWindow;
        private ToolStripButton toolbtnExitSys;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolbtnSnap;

        private void InitializeComponent()
        {
            this.toolbtnExitSys = new ToolStripButton();
            this.toolbtnShowQuestionList = new ToolStripButton();
            this.toolbtnQCEventTypes = new ToolStripButton();
            this.toolbtnQCMsgTemplet = new ToolStripButton();
            this.toolbtnStatByBugs = new ToolStripButton();
            this.toolbtnCheckDocTime = new ToolStripButton();
            this.toolbtnShowPatientList = new ToolStripButton();
            this.toolbtnHideAllDockWindow = new ToolStripButton();
            this.toolbtnSnap = new ToolStripButton();

            this.toolStripSeparator1 = new ToolStripSeparator();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.toolStripSeparator3 = new ToolStripSeparator();
            // 
            // toolbtnShowQuestionList
            // 
            this.toolbtnShowQuestionList.AutoSize = false;
            this.toolbtnShowQuestionList.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnShowQuestionList.Image = MedQCSys.Properties.Resources.QuestionList;
            this.toolbtnShowQuestionList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnShowQuestionList.Name = "toolbtnShowQuestionList";
            this.toolbtnShowQuestionList.Size = new System.Drawing.Size(24, 24);
            this.toolbtnShowQuestionList.Text = "�ʼ�����";
            this.toolbtnShowQuestionList.ToolTipText = "�ʼ�����";
            this.toolbtnShowQuestionList.Click += new EventHandler(this.toolbtnShowQuestionList_Click);
            // 
            // toolbtnQCEventTypes
            // 
            this.toolbtnQCEventTypes.AutoSize = false;
            this.toolbtnQCEventTypes.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnQCEventTypes.Image = MedQCSys.Properties.Resources.QCEventTypes;
            this.toolbtnQCEventTypes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnQCEventTypes.Name = "toolbtnQCEventTypeDict";
            this.toolbtnQCEventTypes.Size = new System.Drawing.Size(24, 24);
            this.toolbtnQCEventTypes.Text = "��������ֵ�";
            this.toolbtnQCEventTypes.ToolTipText = "��������ֵ�";
            this.toolbtnQCEventTypes.Click += new EventHandler(this.toolbtnQCEventTypes_Click);
            // 
            // toolbtnQCMsgTemplet
            // 
            this.toolbtnQCMsgTemplet.AutoSize = false;
            this.toolbtnQCMsgTemplet.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnQCMsgTemplet.Image = MedQCSys.Properties.Resources.QCMsgTemplet;
            this.toolbtnQCMsgTemplet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnQCMsgTemplet.Name = "toolbtnQCMessageDict";
            this.toolbtnQCMsgTemplet.Size = new System.Drawing.Size(24, 24);
            this.toolbtnQCMsgTemplet.Text = "�ʼ������ֵ�";
            this.toolbtnQCMsgTemplet.ToolTipText = "�ʼ������ֵ�";
            this.toolbtnQCMsgTemplet.Click += new EventHandler(this.toolbtnQCMsgTemplet_Click);
            // 
            // toolbtnStatByBugs
            // 
            this.toolbtnStatByBugs.AutoSize = false;
            this.toolbtnStatByBugs.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnStatByBugs.Image = MedQCSys.Properties.Resources.StatByBugs;
            this.toolbtnStatByBugs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnStatByBugs.Name = "toolbtnStatByBugs";
            this.toolbtnStatByBugs.Size = new System.Drawing.Size(24, 24);
            this.toolbtnStatByBugs.Text = "��������嵥";
            this.toolbtnStatByBugs.ToolTipText = "��������嵥";
            this.toolbtnStatByBugs.Click += new EventHandler(this.toolbtnStatByBugs_Click);
            // 
            // toolbtnCheckDocTime
            // 
            this.toolbtnCheckDocTime.AutoSize = false;
            this.toolbtnCheckDocTime.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnCheckDocTime.Image = MedQCSys.Properties.Resources.CheckDocTime;
            this.toolbtnCheckDocTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnCheckDocTime.Name = "toolbtnCheckDocTime";
            this.toolbtnCheckDocTime.Size = new System.Drawing.Size(24, 24);
            this.toolbtnCheckDocTime.Text = "����ʱЧ";
            this.toolbtnCheckDocTime.ToolTipText = "����ʱЧ";
            this.toolbtnCheckDocTime.Click += new EventHandler(this.toolbtnCheckDocTime_Click);
            // 
            // toolbtnSnap
            // 
            this.toolbtnSnap.AutoSize = false;
            this.toolbtnSnap.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnSnap.Image = MedQCSys.Properties.Resources.jietu;
            this.toolbtnSnap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnSnap.Name = "toolbtnSnap";
            this.toolbtnSnap.Size = new System.Drawing.Size(24, 24);
            this.toolbtnSnap.Text = "��ͼ";
            this.toolbtnSnap.ToolTipText = "��ͼ";
            this.toolbtnSnap.Click += new EventHandler(this.toolbtnSnap_Click);
            // 
            // toolbtnShowPatientList
            // 
            this.toolbtnShowPatientList.AutoSize = false;
            this.toolbtnShowPatientList.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnShowPatientList.Image = MedQCSys.Properties.Resources.PatientList;
            this.toolbtnShowPatientList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnShowPatientList.Name = "toolbtnShowPatientList";
            this.toolbtnShowPatientList.Size = new System.Drawing.Size(24, 24);
            this.toolbtnShowPatientList.Text = "�����б�";
            this.toolbtnShowPatientList.ToolTipText = "�����б�";
            this.toolbtnShowPatientList.Click += new EventHandler(this.toolbtnShowPatientList_Click);
            // 
            // toolbtnExitSys
            // 
            this.toolbtnExitSys.AutoSize = false;
            this.toolbtnExitSys.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnExitSys.Image = MedQCSys.Properties.Resources.ExitSys;
            this.toolbtnExitSys.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnExitSys.Name = "toolbtnExitSys";
            this.toolbtnExitSys.Size = new System.Drawing.Size(24, 24);
            this.toolbtnExitSys.Text = "�˳�";
            this.toolbtnExitSys.ToolTipText = "�˳�";
            this.toolbtnExitSys.Click += new EventHandler(this.toolbtnExitSys_Click);
            // 
            // toolbtnHideAllDockWindow
            // 
            this.toolbtnHideAllDockWindow.AutoSize = false;
            this.toolbtnHideAllDockWindow.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.toolbtnHideAllDockWindow.Image = MedQCSys.Properties.Resources.HideAllDockWindow;
            this.toolbtnHideAllDockWindow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnHideAllDockWindow.Name = "toolbtnHideAllDockWindow";
            this.toolbtnHideAllDockWindow.Size = new System.Drawing.Size(24, 24);
            this.toolbtnHideAllDockWindow.Text = "����ͣ������";
            this.toolbtnHideAllDockWindow.ToolTipText = "����ͣ������";
            this.toolbtnHideAllDockWindow.Click += new EventHandler(this.toolbtnHideAllDockWindow_Click);
            this.toolbtnHideAllDockWindow.CheckedChanged += new EventHandler(this.toolbtnHideAllDockWindow_CheckedChanged);
            // 
            // MainToolStrip
            // 
            this.Text = "MainToolStrip";
            this.Items.AddRange(new ToolStripItem[] {
                this.toolbtnShowPatientList,
                this.toolbtnCheckDocTime,
                this.toolbtnShowQuestionList,
                this.toolStripSeparator1,
                this.toolbtnStatByBugs,
                this.toolStripSeparator2,
                this.toolbtnSnap,
                this.toolbtnQCMsgTemplet,
                this.toolbtnQCEventTypes,
                this.toolStripSeparator3,
                this.toolbtnHideAllDockWindow,
                this.toolbtnExitSys});
            this.Location = new System.Drawing.Point(0, 24);
            this.ImageScalingSize = new System.Drawing.Size(22, 22);
        }
        #endregion

        public MainToolStrip()
            : this(null)
        {
        }

        public MainToolStrip(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
        }

        private void toolbtnShowQuestionList_Click(object sender, System.EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.MainForm.ShowQuestionListForm();
        }

        private void toolbtnQCEventTypes_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�޸���������ֵ�", this.MainForm, null);
        }

        private void toolbtnQCMsgTemplet_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�޸��ʼ������ֵ�", this.MainForm, null);
        }

        private void toolbtnStatByBugs_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��������嵥",this.MainForm,null);
        }

        private void toolbtnCheckDocTime_Click(object sender, System.EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.MainForm.ShowDocumentTimeForm();
        }

        private void toolbtnSnap_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��ͼ", this.MainForm, null);
        }

        private void toolbtnShowPatientList_Click(object sender, System.EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.MainForm.ShowPatientListForm();
        }

        private void toolbtnHideAllDockWindow_Click(object sender, System.EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            GlobalMethods.UI.SetCursor(this.MainForm, Cursors.WaitCursor);
            if (!this.toolbtnHideAllDockWindow.Checked)
                this.MainForm.HideAllDockWindow();
            else
                this.MainForm.RestoreAllDockWindow();
            GlobalMethods.UI.SetCursor(this.MainForm, Cursors.Default);
        }

        private void toolbtnHideAllDockWindow_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.toolbtnHideAllDockWindow.Checked)
            {
                this.toolbtnHideAllDockWindow.Text = "��ʾ���д���";
                this.toolbtnHideAllDockWindow.ToolTipText = "��ʾ���д���";
            }
            else
            {
                this.toolbtnHideAllDockWindow.Text = "�������д���";
                this.toolbtnHideAllDockWindow.ToolTipText = "�������д���";
            }
        }

        private void toolbtnExitSys_Click(object sender, System.EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.MainForm.Close();
        }

        protected override void OnAllDockWindowHidden(bool bIsHidden)
        {
            if (this.toolbtnHideAllDockWindow != null && !this.toolbtnHideAllDockWindow.IsDisposed)
                this.toolbtnHideAllDockWindow.Checked = bIsHidden;
        }
    }
}
