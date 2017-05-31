// ***********************************************************
// DockPanel�����ͣ�����ڼ̳л���.
// Creator:YangMingkun  Date:2009-11-7
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
using Heren.Common.Controls;
using EMRDBLib;
using MedQCSys.PatPage;

namespace MedQCSys.DockForms
{
    public partial class DockContentBase : DockContent
    {
        public DockContentBase()
        { }
        private bool m_bNeedRefreshView = false;
        /// <summary>
        /// ��ȡ�Ƿ���Ҫˢ����ͼ
        /// </summary>
        [Browsable(false)]
        public virtual bool NeedRefreshView
        {
            get { return this.m_bNeedRefreshView; }
        }

        private MainForm m_mainForm = null;
        /// <summary>
        /// ��ȡ�����������򴰿�
        /// </summary>
        [Browsable(false)]
        protected virtual MainForm MainForm
        {
            get { return this.m_mainForm; }
            set { this.m_mainForm = value; }
        }

        private bool m_bIsUserHidden = false;
        /// <summary>
        /// ��ȡ�������û��Ƿ����������˵�ǰ����
        /// </summary>
        [Browsable(false)]
        public bool IsUserHidden
        {
            get { return this.m_bIsUserHidden; }
            set { this.m_bIsUserHidden = value; }
        }

        private bool m_bSaveWindowBoundsEnabled = false;
        /// <summary>
        /// ��ȡ�������Ƿ񱣴洰�����귶Χ
        /// </summary>
        [Browsable(false)]
        public bool SaveWindowBoundsEnabled
        {
            get { return this.m_bSaveWindowBoundsEnabled; }
            set { this.m_bSaveWindowBoundsEnabled = value; }
        }


        public DockContentBase(MainForm parent)
        {
            this.m_mainForm = parent;
            this.InitializeComponent();
            this.TabPageContextMenuStrip = this.contextMenuStrip;
            if (this.m_mainForm == null || this.m_mainForm.IsDisposed)
                return;
            this.m_mainForm.PatientInfoChanging += new CancelEventHandler(this.MainForm_PatientInfoChanging);
            this.m_mainForm.PatientInfoChanged += new EventHandler(this.MainForm_PatientInfoChanged);
            this.m_mainForm.PatientListInfoChanged += new EventHandler(this.MainForm_PatientListInfoChanged);
            this.m_mainForm.ActiveContentChanged += new EventHandler(this.MainForm_ActiveContentChanged);
            this.m_mainForm.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.m_mainForm.PatientScoreChanged += new EventHandler(m_mainForm_PatientScoreChanged);
        }
        private PatPage.PatientPageControl m_patientPageControl = null;
        /// <summary>
        /// ��ȡ�����û��ߴ���
        /// </summary>
        [Browsable(false)]
        protected virtual PatPage.PatientPageControl PatientPageControl
        {
            get { return this.m_patientPageControl; }
            set { this.m_patientPageControl = value; }
        }
        public DockContentBase(MainForm mainForm, PatPage.PatientPageControl patientPageControl)
        {
            this.MainForm = mainForm;
            this.m_patientPageControl = patientPageControl;
            this.m_patientPageControl.MainForm = mainForm;
            this.InitializeComponent();
            this.TabPageContextMenuStrip = this.contextMenuStrip;
            if (this.m_mainForm == null || this.m_mainForm.IsDisposed)
                return;
            this.m_mainForm.ActiveContentChanged += new EventHandler(this.MainForm_ActiveContentChanged);
            this.m_mainForm.PatientListInfoChanged += new EventHandler(this.MainForm_PatientListInfoChanged);
            this.m_mainForm.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.m_mainForm.PatientScoreChanged += new EventHandler(m_mainForm_PatientScoreChanged);

            this.m_patientPageControl.ActiveContentChanged += new EventHandler(PatientPageControl_ActiveContentChanged);
            this.m_patientPageControl.PatientInfoChanged += new EventHandler(PatientPageControl_PatientInfoChanged);
        }


        #region"����λ�ü�¼"
            /// <summary>
            /// �ָ����ڴ�С��λ��
            /// </summary>
        public void RestoreWindowLocationAndSize()
        {
            string szFormTypeName = this.GetType().ToString();

            //������������ʾλ�úʹ�С
            int nLeft = SystemConfig.Instance.Get(string.Concat(szFormTypeName, ".Left"), this.Left);
            int nTop = SystemConfig.Instance.Get(string.Concat(szFormTypeName, ".Top"), this.Top);
            int nWidth = SystemConfig.Instance.Get(string.Concat(szFormTypeName, ".Width"), this.Width);
            int nHeight = SystemConfig.Instance.Get(string.Concat(szFormTypeName, ".Height"), this.Height);

            //ʹ����ʼ�մ�����Ļ������
            Rectangle screenBounds = GlobalMethods.UI.GetScreenBounds();
            if (nLeft >= screenBounds.Right - 8 || nLeft + nWidth <= 8)
                nLeft = 0;
            if (nTop >= screenBounds.Bottom - 8 || nTop + nHeight <= 8)
                nTop = 0;

            this.SetBounds(nLeft, nTop, nWidth, nHeight, BoundsSpecified.All);

            //�������״̬
            if (SystemConfig.Instance.Get(string.Concat(szFormTypeName, ".Maximized"), false))
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.DockPanel != null)
                return;
            bool bSaveWindowBoundsEnabled = this.m_bSaveWindowBoundsEnabled;
            this.m_bSaveWindowBoundsEnabled = false;

            this.RestoreWindowLocationAndSize();

            this.m_bSaveWindowBoundsEnabled = bSaveWindowBoundsEnabled;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (!this.m_bSaveWindowBoundsEnabled || this.WindowState != FormWindowState.Normal)
                return;
            string szFormTypeName = this.GetType().ToString();
            SystemConfig.Instance.Write(string.Concat(szFormTypeName, ".Left"), this.Left.ToString());
            SystemConfig.Instance.Write(string.Concat(szFormTypeName, ".Top"), this.Top.ToString());
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!this.m_bSaveWindowBoundsEnabled || this.WindowState != FormWindowState.Normal)
                return;
            string szFormTypeName = this.GetType().ToString();
            SystemConfig.Instance.Write(string.Concat(szFormTypeName, ".Width"), this.Width.ToString());
            SystemConfig.Instance.Write(string.Concat(szFormTypeName, ".Height"), this.Height.ToString());
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!this.SaveUncommitedChange())
                return;

            this.ClearOwnerForms();

            if (!this.m_bSaveWindowBoundsEnabled)
                return;

            string szFormTypeName = this.GetType().ToString();
            if (this.WindowState == FormWindowState.Maximized)
            {
                SystemConfig.Instance.Write(string.Concat(szFormTypeName, ".Maximized"), "True");
            }
            else
            {
                SystemConfig.Instance.Write(string.Concat(szFormTypeName, ".Maximized"), "False");
            }
        }
        #endregion

        private void PatientPageControl_PatientInfoChanging(object sender, PatientInfoChangingEventArgs e)
        {
            this.OnPatientInfoChanging(e);
        }


        private void MainForm_PatientInfoChanging(object sender, CancelEventArgs e)
        {
            this.OnPatientInfoChanging(e);
        }

        private void MainForm_PatientInfoChanged(object sender, EventArgs e)
        {
            //����"�Ƿ���Ҫˢ������"��ʶΪtrue
            this.m_bNeedRefreshView = true;
            //
            this.OnPatientInfoChanged();
            //
            if (this.Pane != null && !this.Pane.IsDisposed && this.Pane.ActiveContent == this)
                this.m_bNeedRefreshView = false;

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
        private void MainForm_PatientListInfoChanged(object sender, EventArgs e)
        {
            //����"�Ƿ���Ҫˢ������"��ʶΪtrue
            this.m_bNeedRefreshView = true;
            //
            this.OnPatientListInfoChanged();
            //
            if (this.Pane != null && !this.Pane.IsDisposed && this.Pane.ActiveContent == this)
                this.m_bNeedRefreshView = false;
        }

        private void MainForm_ActiveContentChanged(object sender, EventArgs e)
        {
            this.OnActiveContentChanged();

            //
            if (this.Pane != null && !this.Pane.IsDisposed && this.Pane.ActiveContent == this)
                this.m_bNeedRefreshView = false;

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //���������ڹر�ʱ,���ᴥ�����Ӵ��ڵ�FormClosing�¼�,����������������
            if (!e.Cancel)
                this.OnFormClosing(e);
        }

        private void m_mainForm_PatientScoreChanged(object sender, EventArgs e)
        {
            //����"�Ƿ���Ҫˢ������"��ʶΪtrue
            this.m_bNeedRefreshView = true;
            //
            this.OnPatientScoreChanged();
        }

        /// <summary>
        /// �����ʼ�����ı�ǰ�Զ����õķ���
        /// </summary>
        protected virtual void OnPatientScoreChanged()
        {

        }
        /// <summary>
        /// ˢ����ͼ����
        /// </summary>
        public virtual void OnRefreshView()
        {
            this.m_bNeedRefreshView = false;
        }

        /// <summary>
        /// ������Ϣ�ı�ǰ�Զ����õķ���
        /// ����������д�˷������ж��Ƿ���Ҫ�ڸı�ǰ��������
        /// </summary>
        protected virtual void OnPatientInfoChanging(CancelEventArgs e)
        {

        }

        /// <summary>
        /// ������Ϣ�ı�ʱ�Զ����õķ���
        /// ����������д�˷�����ˢ�µ�ǰ������
        /// </summary>
        protected virtual void OnPatientInfoChanged()
        {

        }

        /// <summary>
        /// �����б���Ϣ�ı�ʱ�Զ����õķ���
        /// ����������д�˷�����ˢ�µ�ǰ������
        /// </summary>
        protected virtual void OnPatientListInfoChanged()
        {

        }

        /// <summary>
        /// ͣ�����ڵĻ״̬�ı�ʱ�Զ����õķ���
        /// ����������д�˷�������ʼ����ǰ������
        /// </summary>
        protected virtual void OnActiveContentChanged()
        {

        }
        private void PatientPageControl_ActiveContentChanged(object sender, EventArgs e)
        {
            if (!this.DockHandler.IsHidden)
                this.OnActiveContentChanged();
            //
            if (this.Pane != null && !this.Pane.IsDisposed && this.Pane.ActiveContent == this)
                this.m_bNeedRefreshView = false;
        }
        private void PatientPageControl_PatientInfoChanged(object sender, EventArgs e)
        {
            PatVisitInfo patVisit = SystemParam.Instance.PatVisitInfo;
            if (patVisit != null)
            {
                this.m_bNeedRefreshView = true;
                if (!this.DockHandler.IsHidden)
                    this.OnPatientInfoChanged();
            }
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
        public virtual bool SaveUncommitedChange()
        {
            if (!this.HasUncommit())
                return true;
            this.DockHandler.Activate();
            DialogResult result = MessageBoxEx.Show("��ǰ��δ������޸�,�Ƿ񱣴棿"
                , MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return false;
            else if (result == DialogResult.Yes)
                return this.CommitModify();
            return true;
        }

        protected void ShowStatusMessage(string szMessage)
        {
            if (this.m_mainForm != null && !this.m_mainForm.IsDisposed)
                this.m_mainForm.ShowStatusMessage(szMessage);
        }

        private void ClearOwnerForms()
        {
            Form[] arrOwnedForms = this.OwnedForms;
            if (arrOwnedForms == null || arrOwnedForms.Length <= 0)
                return;
            for (int index = 0; index < arrOwnedForms.Length; index++)
            {
                if (arrOwnedForms[index] != null && !arrOwnedForms[index].IsDisposed)
                    arrOwnedForms[index].Owner = null;
            }
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            if (this.DockHandler == null)
                this.Close();
            else
                this.DockHandler.Close();
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            this.OnRefreshView();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (this.m_mainForm == null || this.m_mainForm.IsDisposed)
                return;
            this.m_mainForm.PatientInfoChanging -= new CancelEventHandler(this.MainForm_PatientInfoChanging);
            this.m_mainForm.PatientInfoChanged -= new EventHandler(this.MainForm_PatientInfoChanged);
            this.m_mainForm.PatientListInfoChanged -= new EventHandler(this.MainForm_PatientListInfoChanged);
            this.m_mainForm.FormClosing -= new FormClosingEventHandler(this.MainForm_FormClosing);
            this.m_mainForm.ActiveContentChanged -= new EventHandler(this.MainForm_ActiveContentChanged);
        }

        private void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            bool visible = true;
            if (!this.CloseButtonVisible)
                visible = false;
            if (this.mnuClose.Visible != visible)
                this.mnuClose.Visible = visible;
        }
    }
}