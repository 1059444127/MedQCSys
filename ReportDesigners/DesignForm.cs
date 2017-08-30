// ***********************************************************
// �����༭�����ù���ϵͳ�����򴰿�.
// Creator:YangMingkun  Date:2010-11-29
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Designers;
using Designers.Report;
using Designers.Templet;
using System.ComponentModel;
using Designers.FindReplace;
using EMRDBLib;
using System.Diagnostics;
using MedQCSys;

namespace Designers
{
    public partial class DesignForm : MedQCSys.DockForms.DockContentBase
    {
        private ToolboxListForm m_ToolboxListForm = null;
        private PropertyEditForm m_PropertyEditForm = null;
        private ErrorsListForm m_ErrorsListForm = null;
        private FindReplaceForm m_FindReplaceForm = null;
        private FindResultForm m_FindResultForm = null;


        #region"ϵͳ��ʼ��"
        public DesignForm()
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;

        }
        public DesignForm(MainForm form) : base(form)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //this.RestoreWindowState();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //����Ϊ�Զ����洰��״̬
            //this.SaveWindowState = true;

            //��ȡ���沼������
            this.LoadLayoutConfig();
            this.ShowStatusMessage("�Ѿ���!");

            ReportHandler.Instance.InitReportHandler(this);
            Templet.TempletHandler.Instance.InitTempletHandler(this);


            //˫��ģ���
            if (!string.IsNullOrEmpty(FilePath))
            {
                OpenFile(FilePath);
            }
            //�޸�ע���Ĭ�ϴ�
            RegisterFileType();
            this.ShowReportTreeForm();
            this.ShowTempletTreeForm();
        }

        private void RegisterFileType()
        {
            Register.FileTypeRegInfo fileRegInfo = new Register.FileTypeRegInfo();

            fileRegInfo.ExePath = Process.GetCurrentProcess().MainModule.FileName;
            fileRegInfo.ExtendName = ".hndt";
            fileRegInfo.IconPath = System.Windows.Forms.Application.StartupPath + @"\Resources\SysIcon.ico";
            Register.FileTypeRegister.RegisterFileType(fileRegInfo);
            fileRegInfo.ExtendName = ".hrdt";
            Register.FileTypeRegister.RegisterFileType(fileRegInfo);
        }

        private void OpenFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            if (filePath.ToUpper().EndsWith("HRDT"))
            {
                ReportHandler.Instance.OpenReport(filePath);
            }
            else if (filePath.ToUpper().EndsWith("HNDT"))
            {
                TempletHandler.Instance.OpenTemplet(filePath);
            }

        }

        /// <summary>
        /// ����DockPanel���沼�������ļ�
        /// </summary>
        private void LoadLayoutConfig()
        {

        }

        #endregion


        #region"����"
        /// <summary>
        /// ��ȡ����Ŀ�ͣ������
        /// </summary>
        [Description("��ȡ����Ŀ�ͣ������")]
        internal IDockContent ActiveContent
        {
            get { return this.dockPanel1.ActiveContent; }
        }

        /// <summary>
        /// ��ȡ������ĵ�����
        /// </summary>
        [Description("��ȡ������ĵ�����")]
        internal IDockContent ActiveDocument
        {
            get { return this.dockPanel1.ActiveDocument; }
        }

        /// <summary>
        /// ��ȡ���п�ͣ�����ڵ��б�
        /// </summary>
        [Description("��ȡ���п�ͣ�����ڵ��б�")]
        internal DockContentCollection Contents
        {
            get { return this.dockPanel1.Contents; }
        }

        /// <summary>
        /// ��ȡ�����ĵ����ڵ��б�
        /// </summary>
        [Description("��ȡ�����ĵ����ڵ��б�")]
        internal IDockContent[] Documents
        {
            get { return this.dockPanel1.DocumentsToArray(); }
        }

        /// <summary>
        /// ��ȡ��������������
        /// </summary>
        [Description("��ȡ��������������")]
        internal IDesignEditForm ActiveDesign
        {
            get { return this.dockPanel1.ActiveDocument as IDesignEditForm; }
        }

        /// <summary>
        /// ��ȡ����Ľű��༭����
        /// </summary>
        [Description("��ȡ����Ľű��༭����")]
        internal IScriptEditForm ActiveScript
        {
            get { return this.dockPanel1.ActiveDocument as IScriptEditForm; }
        }
        #endregion

        #region"�¼�"
        /// <summary>
        /// ������ĵ����ڸı�״̬ʱ����
        /// </summary>
        [Description("������ĵ����ڸı�״̬ʱ����")]
        internal event EventHandler ActiveDocumentChanged;

        internal virtual void OnActiveDocumentChanged(EventArgs e)
        {
            if (this.ActiveDocumentChanged == null)
                return;
            try
            {
                this.ActiveDocumentChanged(this, e);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("MainForm.OnActiveDocumentChanged", ex);
            }
        }

        /// <summary>
        /// �����ͣ�����ڸı�״̬ʱ����
        /// </summary>
        [Description("�����ͣ�����ڸı�״̬ʱ����")]
        internal event EventHandler ActiveContentChanged;

        internal virtual void OnActiveContentChanged(EventArgs e)
        {
            if (this.ActiveContentChanged == null)
                return;
            try
            {
                this.ActiveContentChanged(this, e);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("MainForm.OnActiveContentChanged", ex);
            }
        }

        private void dockPanel1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            this.OnActiveDocumentChanged(e);
        }
        /// <summary>
        /// �򿪱��򱨱����������
        /// </summary>
        /// <param name="designEditForm">���������</param>
        internal void OpenDesignEditForm(IDesignEditForm designEditForm)
        {
            if (designEditForm == null || designEditForm.IsDisposed)
                return;
            this.dockPanel1.Update();
            IDockContent content = designEditForm as IDockContent;
            if (content != null)
                content.DockHandler.Show(this.dockPanel1);
        }
        /// <summary>
        /// ��ʾ�ؼ������䴰��
        /// </summary>
        internal void ShowToolboxListForm()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_ToolboxListForm == null || this.m_ToolboxListForm.IsDisposed)
                this.m_ToolboxListForm = new ToolboxListForm(this);
            this.m_ToolboxListForm.Show(this.dockPanel1);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        /// <summary>
        /// ��ʾ�������Ա༭����
        /// </summary>
        internal void ShowPropertyEditForm()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_PropertyEditForm == null || this.m_PropertyEditForm.IsDisposed)
                this.m_PropertyEditForm = new PropertyEditForm(this);
            this.m_PropertyEditForm.Show(this.dockPanel1);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        public void ShowStatusMessage(string szMessage)
        {
            object result = null;

        }
        /// <summary>
        /// ��ʾ�ű���������б���
        /// </summary>
        internal void ShowCompileErrorForm(ErrorsListForm.CompileError[] errors)
        {
            if (this.m_ErrorsListForm != null && !this.m_ErrorsListForm.IsDisposed)
                this.m_ErrorsListForm.Close();
            if (errors == null || errors.Length <= 0)
                return;
            if (this.m_ErrorsListForm == null || this.m_ErrorsListForm.IsDisposed)
            {
                this.m_ErrorsListForm = new ErrorsListForm(this);
                this.m_ErrorsListForm.Show(this.dockPanel1, DockState.DockBottom);
            }
            this.m_ErrorsListForm.Activate();
            this.m_ErrorsListForm.CompileErrors = errors;
            this.m_ErrorsListForm.ScriptEditForm = this.ActiveScript;
            this.m_ErrorsListForm.OnRefreshView();
        }

        /// <summary>
        /// �򿪱��򱨱�ű��༭������
        /// </summary>
        /// <param name="scriptEditForm">�ű��༭������</param>
        internal void OpenScriptEditForm(IScriptEditForm scriptEditForm)
        {
            if (scriptEditForm == null || scriptEditForm.IsDisposed)
                return;
            this.dockPanel1.Update();
            IDockContent content = scriptEditForm as IDockContent;
            if (content != null)
                content.DockHandler.Show(this.dockPanel1);
        }
        /// <summary>
        /// ��ʾ�ű��ı������滻����
        /// </summary>
        internal void ShowFindReplaceForm()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_FindReplaceForm == null || this.m_FindReplaceForm.IsDisposed)
                this.m_FindReplaceForm = new FindReplaceForm(this);
            this.m_FindReplaceForm.Show(this.dockPanel1);
            if (this.ActiveScript != null)
                this.m_FindReplaceForm.DefaultFindText = this.ActiveScript.GetSelectedText();
            this.m_FindReplaceForm.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        /// <summary>
        /// ��ʾ�ű��ı����ҽ������
        /// </summary>
        /// <param name="scriptEditForm">�ű��༭������</param>
        /// <param name="szFindText">�����ı�</param>
        /// <param name="results">���ҽ����</param>
        /// <param name="bshowFileName">�����Ƿ���ʾ�ļ�����</param>
        internal void ShowFindResultForm(IScriptEditForm scriptEditForm
            , string szFindText, List<FindResult> results, bool bshowFileName)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_FindResultForm == null || this.m_FindResultForm.IsDisposed)
                this.m_FindResultForm = new FindResultForm(this);
            this.m_FindResultForm.ScriptEditForm = scriptEditForm;
            this.m_FindResultForm.FindText = szFindText;
            this.m_FindResultForm.Results = results;

            this.m_FindResultForm.Show(this.dockPanel1);
            this.m_FindResultForm.ShowFileName = bshowFileName;
            this.m_FindResultForm.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        #endregion



        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }



        private void �򿪱���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportHandler.Instance.OpenReport();
        }

        private void �½�����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportType reportTypeInfo = new ReportType();
            reportTypeInfo.ReportTypeName = "�½�����";
            ReportHandler.Instance.NewReport(reportTypeInfo);
        }

        private void �򿪱�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Templet.TempletHandler.Instance.OpenTemplet();
        }

        private void �½���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TempletType typeInfo = new TempletType();
            typeInfo.DocTypeName = "�½�ģ��";
            TempletHandler.Instance.OpenTemplet(typeInfo);
        }

        /// <summary>
        /// ˫�����ĵ�·��
        /// </summary>
        public string FilePath
        { get; set; }

        private void �����б�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowReportTreeForm();
        }
        private ReportTreeForm m_ReportTreeForm;
        /// <summary>
        /// ��ʾ����ģ�������
        /// </summary>
        internal void ShowReportTreeForm()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_ReportTreeForm == null || this.m_ReportTreeForm.IsDisposed)
                this.m_ReportTreeForm = new ReportTreeForm(this);
            this.m_ReportTreeForm.Show(this.dockPanel1);
            this.m_ReportTreeForm.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        private TempletTreeForm m_TempletTreeForm;
        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTempletTreeForm();
        }

        private void ShowTempletTreeForm()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_TempletTreeForm == null || this.m_TempletTreeForm.IsDisposed)
                this.m_TempletTreeForm = new TempletTreeForm(this);
            this.m_TempletTreeForm.Show(this.dockPanel1);
            this.m_TempletTreeForm.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
    }
}