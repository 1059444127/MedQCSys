// ***********************************************************
// �����༭�����ù���ϵͳ��ͣ�����ڻ���.
// Creator:YangMingkun  Date:2010-11-29
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;

namespace Designers
{
    public partial class DockContentBase : DockContent
    {
        private DesignForm m_MainForm = null;
        /// <summary>
        /// ��ȡ�����������򴰿�
        /// </summary>
        [Browsable(false)]
        protected virtual DesignForm MainForm
        {
            get { return this.m_MainForm; }
            set { this.m_MainForm = value; }
        }

        public DockContentBase()
            : this(null)
        {
        }

        public DockContentBase(DesignForm parent)
        {
            this.m_MainForm = parent;
        }

        //������Ҫ����λ�õ�ͣ������,�뽫�ؼ������������Load�¼���
        //���������ڱ�����ʱ,�Ͳ�����ؽ���Ԫ��,�������ϵͳ�����ٶ�
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //this.InitializeComponent();
            this.TabPageContextMenuStrip = this.contextMenuStrip;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                return this.CommitModify();
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// ˢ����ͼ����
        /// </summary>
        public virtual void OnRefreshView()
        {
        }

        /// <summary>
        /// ��ȡ�Ƿ���δ����ļ�¼
        /// </summary>
        public virtual bool HasUncommit()
        {
            return false;
        }

        /// <summary>
        /// ���浱ǰ�Լ�¼���޸�
        /// </summary>
        /// <returns></returns>
        public virtual bool CommitModify()
        {
            return true;
        }

        /// <summary>
        /// ����Ƿ�����Ҫ���������
        /// </summary>
        /// <returns>�Ƿ񱣴�ɹ�</returns>
        public virtual bool CheckModifiedData()
        {
            if (!this.HasUncommit())
                return true;
            this.DockHandler.Activate();
            DialogResult result = MessageBoxEx.ShowQuestion("��ǰ��δ������޸�,�Ƿ񱣴棿");
            if (result == DialogResult.Cancel)
                return false;
            else if (result == DialogResult.Yes)
                return this.CommitModify();
            return true;
        }

        protected void ShowStatusMessage(string szMessage)
        {
            if (this.m_MainForm != null && !this.m_MainForm.IsDisposed)
                this.m_MainForm.ShowStatusMessage(szMessage);
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            if (this.DockHandler == null)
            {
                this.Close();
            }
            else
            {
                this.Pane.Focus();
                this.DockHandler.Close();
            }
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            this.OnRefreshView();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            e.Cancel = !this.CheckModifiedData();
        }
        protected virtual void OnActiveDocumentChanged()
        {
        }
    }
}