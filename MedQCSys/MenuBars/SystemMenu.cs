// ***********************************************************
// �����ʿ�ϵͳϵͳ�����˵��ؼ�.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;

using MedQCSys.Utility;
using EMRDBLib;
using Heren.MedQC.Core;

namespace MedQCSys.MenuBars
{
    internal class SystemMenu : MqsMenuItemBase
    {
        private MainForm m_mainForm = null;
        public virtual MainForm MainForm
        {
            get { return this.m_mainForm; }
            set { this.m_mainForm = value; }
        }

        #region"ϵͳ�˵���ʼ��"
        private MqsMenuItemBase mnuShowPatientList;
        private MqsMenuItemBase mnuShowDocList;
        private MqsMenuItemBase mnuShowTempletStatForm;
        private MqsMenuItemBase mnuMedTemplet;
        private MqsMenuItemBase mnuShowQuestionList;
        private MqsMenuItemBase mnuExitSys;
        private MqsMenuItemBase mnuMedCallBack;
        private MqsMenuItemBase mnuQcTrackForm;
        public MqsMenuItemBase mnuMedQCSpecial;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;

        private void InitializeComponent()
        {
            this.mnuShowPatientList = new MqsMenuItemBase();
            this.mnuShowDocList = new MqsMenuItemBase();
           
            this.mnuShowTempletStatForm = new MqsMenuItemBase();
            this.mnuMedTemplet = new MqsMenuItemBase();
            this.mnuShowQuestionList = new MqsMenuItemBase();
            this.mnuExitSys = new MqsMenuItemBase();
            this.mnuMedCallBack = new MqsMenuItemBase();
            this.mnuMedQCSpecial = new MqsMenuItemBase();
            this.mnuQcTrackForm = new MqsMenuItemBase();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.toolStripSeparator3 = new ToolStripSeparator();
            // 
            // mnuShowPatientList
            // 
            this.mnuShowPatientList.Name = "mnuShowPatientList";
            this.mnuShowPatientList.ShortcutKeys = Keys.F2;
            this.mnuShowPatientList.Text = "�����б�";
            this.mnuShowPatientList.Click += new System.EventHandler(this.mnuPatientList_Click);
            // 
            // mnuShowQuestionList
            // 
            this.mnuShowQuestionList.Name = "mnuShowQuestionList";
            this.mnuShowQuestionList.ShortcutKeys = ((Keys)((Keys.Control | Keys.Q)));
            this.mnuShowQuestionList.Text = "�ʼ�����";
            this.mnuShowQuestionList.Click += new System.EventHandler(this.mnuShowQuestionList_Click);
            // 
            // mnuMedCallBack
            // 
            this.mnuMedCallBack.Name = "mnuMedCallBack";
            this.mnuMedCallBack.Text = "�����ٻ�";
            this.mnuMedCallBack.Click += new System.EventHandler(this.mnuMedCallBack_Click);
            // 
            // mnuVitalSignsGraph
            // 
            this.mnuMedTemplet.Name = "mnuMedTemplet";
            this.mnuMedTemplet.Text = "ģ�����";
            this.mnuMedTemplet.Click += new System.EventHandler(this.mnuMedTemplet_Click);
            // 
            // mnuQcTrackForm
            // 
            this.mnuQcTrackForm.Name = "mnuQcTrackForm";
            this.mnuQcTrackForm.Text = "�ʿ�׷��";
            this.mnuQcTrackForm.Click += new System.EventHandler(this.mnuQcTrackForm_Click);
        
            // 
            // mnuMedQCSpecial
            // 
            this.mnuMedQCSpecial.Name = "mnuMedQCSpecial";
            this.mnuMedQCSpecial.Text = "ר���ʿ�";
            this.mnuMedQCSpecial.Click += new EventHandler(mnuMedQCSpecial_Click);

           

            // 
            // mnuExitSys
            // 
            this.mnuExitSys.Name = "mnuExitSys";
            this.mnuExitSys.ShortcutKeys = ((Keys)((Keys.Alt | Keys.F4)));
            this.mnuExitSys.Text = "�л��˺�(&X)";
            this.mnuExitSys.Click += new System.EventHandler(this.mnuExitSys_Click);
            // 
            // menuSystem
            // 
            this.Name = "menuSystem";
            this.Text = "ϵͳ(&S)";
            this.DropDownItems.AddRange(new ToolStripItem[] {
                this.mnuShowPatientList,
                this.toolStripSeparator1,
                this.mnuMedQCSpecial,
                this.mnuShowQuestionList,
                this.mnuMedTemplet,
                this.mnuMedCallBack,
                this.mnuQcTrackForm,
                this.toolStripSeparator3,      
                this.mnuExitSys});
        }
        #endregion
        public SystemMenu(MainForm parent)
        {
            this.m_mainForm = parent;
            this.InitializeComponent();
        }

        private void mnuPatientList_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�����б�", this.MainForm, null);
        }
        private void mnuMedTemplet_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("ģ�����", this.MainForm, null);
        }

        private void mnuQcTrackForm_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�ʿ�׷��", this.MainForm, null);
        }
        private void mnuShowQuestionList_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�ʼ�����", this.MainForm, null);
        }
        private void mnuMedCallBack_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�����ٻ�", this.MainForm, null);
        }
        private void mnuMedQCSpecial_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("ר���ʿ�", this.MainForm, null);
        }
        private void mnuExitSys_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�л��˺�", this.MainForm, null);
        }
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (this.m_mainForm == null || this.m_mainForm.IsDisposed)
                return;

            //�Ƿ���ʾģ����˹���
            if (!SystemParam.Instance.LocalConfigOption.IsShowMedTemplet)
                this.mnuMedTemplet.Visible = false;
            //�Ƿ���ʾר���ʿ�
            if (!SystemParam.Instance.LocalConfigOption.IsShowSpecialCheck)
                this.mnuMedQCSpecial.Visible = false;
            if (!SystemParam.Instance.LocalConfigOption.IsOpenQcTrack)
            {
                this.mnuQcTrackForm.Visible = false;
            }
        }
    }
}
