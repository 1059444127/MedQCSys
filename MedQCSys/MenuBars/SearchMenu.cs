// ***********************************************************
// �����ʿ�ϵͳͳ�������˵��ؼ�.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EMRDBLib;
using MedQCSys.Utility;
using Heren.MedQC.Core;

namespace MedQCSys.MenuBars
{
    internal class SearchMenu : MqsMenuItemBase
    {
        private MainForm m_mainForm = null;
        public virtual MainForm MainForm
        {
            get { return this.m_mainForm; }
            set { this.m_mainForm = value; }
        }

        #region"��ѯ�˵���ʼ��"
        private MqsMenuItemBase mnuDelayNoCommitDoc;
        private MqsMenuItemBase mnuStatBySeriousCtitical;
        private MqsMenuItemBase mnuStatByDeath;
        private MqsMenuItemBase mnuStatByOutPatient;
        private MqsMenuItemBase mnuStatByOperation;
        private void InitializeComponent()
        {
            this.mnuDelayNoCommitDoc = new MqsMenuItemBase();
            this.mnuStatBySeriousCtitical = new MqsMenuItemBase();
            this.mnuStatByDeath = new MqsMenuItemBase();
            this.mnuStatByOutPatient = new MqsMenuItemBase();
            this.mnuStatByOperation = new MqsMenuItemBase();

            //
            //����δ�ύ������ѯ
            //
            this.mnuDelayNoCommitDoc.Name = "mnuDelayNoCommitDoc";
            this.mnuDelayNoCommitDoc.Text = "����δ�ύ������ѯ";
            this.mnuDelayNoCommitDoc.Click += new EventHandler(this.mnuDelayNoCommitDoc_Click);

            //
            //Σ�ػ��߲�ѯ
            //
            this.mnuStatBySeriousCtitical.Name = "mnuStatBySeriousCtitical";
            this.mnuStatBySeriousCtitical.Text = "Σ�ػ��߲�ѯ";
            this.mnuStatBySeriousCtitical.Click += new EventHandler(this.mnuStatBySeriousCtitical_Click);
            //
            //�������߲�ѯ
            //
            this.mnuStatByDeath.Name = "mnuStatByDeath";
            this.mnuStatByDeath.Text = "�������߲�ѯ";
            this.mnuStatByDeath.Click += new EventHandler(this.mnuStatByDeath_Click);
            //
            //��Ժ���߲�ѯ
            //
            this.mnuStatByOutPatient.Name = "mnuStatByOutPatient";
            this.mnuStatByOutPatient.Text = "��Ժ���߲�ѯ";
            this.mnuStatByOutPatient.Click += new EventHandler(this.mnuStatByOutPatient_Click);
            //
            //�������߲�ѯ
            //
            this.mnuStatByOperation.Name = "mnuStatByOutPatient";
            this.mnuStatByOperation.Text = "�������߲�ѯ";
            this.mnuStatByOperation.Click += new EventHandler(this.mnuStatByOperation_Click);

            // 
            // menuStatistic
            // 
            this.Name = "menuStatistic";
            this.Text = "��ѯ(&C)";
            this.DropDownItems.AddRange(new ToolStripItem[] {
                this.mnuStatBySeriousCtitical,
                this.mnuStatByDeath,
                this.mnuStatByOutPatient,
                this.mnuStatByOperation,
                new ToolStripSeparator(),
                this.mnuDelayNoCommitDoc,
            });
        }
        #endregion

        public SearchMenu(MainForm parent)
        {
            this.m_mainForm = parent;

            this.InitializeComponent();
        }
        private void mnuDelayNoCommitDoc_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("����δ�ύ������ѯ", this.MainForm, null);
        }
        private void mnuStatBySeriousCtitical_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("Σ�ػ��߲�ѯ", this.MainForm, null);
        }

        private void mnuStatByDeath_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�������߲�ѯ",this.MainForm,null);
        }
        private void mnuStatByOutPatient_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��Ժ���߲�ѯ", this.MainForm, null);
        }
        private void mnuStatByOperation_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�������߲�ѯ", this.MainForm, null);
        }
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (this.m_mainForm == null || this.m_mainForm.IsDisposed)
                return;
        }
    }
}
