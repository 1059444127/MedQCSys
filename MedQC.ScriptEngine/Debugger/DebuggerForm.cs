// ***********************************************************
// ���Ӳ���ϵͳ�ű����ù��������򴰿�.
// Creator: YangMingkun  Date:2011-11-15
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.ScriptEngine.Script;
using EMRDBLib;

namespace Heren.MedQC.ScriptEngine.Debugger
{
    public partial class DebuggerForm : HerenForm
    {
        private TempletListForm m_TempletListForm = null;
        private ErrorsListForm m_ErrorsListForm = null;
        public QcCheckPoint QcCheckPoint { get; set; }
        public PatVisitInfo PatVisitInfo { get; set; }
        public QcCheckResult QcCheckResult { get; set; }

        private string m_workingPath = null;
        /// <summary>
        /// ��ȡ�����õ���������·��
        /// </summary>
        [Browsable(false)]
        public string WorkingPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_workingPath))
                    return GlobalMethods.Misc.GetWorkingPath();
                return this.m_workingPath;
            }
            set { this.m_workingPath = value; }
        }

        private ScriptProperty m_scriptProperty = null;
        /// <summary>
        /// ��ȡ�����õ�ǰ�򿪵Ľű�
        /// </summary>
        [Browsable(false)]
        public ScriptProperty ScriptProperty
        {
            get { return this.m_scriptProperty; }
            set
            {
                this.m_scriptProperty = null;
                if (value == null)
                    return;
                this.m_scriptProperty = value.Clone() as ScriptProperty;
            }
        }

        /// <summary>
        /// ��ȡ��ǰ��Ľű��༭����
        /// </summary>
        [Browsable(false)]
        internal ScriptEditForm ActiveScriptForm
        {
            get
            {
                IDockContent content = this.dockPanel1.ActiveDocument;
                ScriptEditForm scriptEditForm = content as ScriptEditForm;
                if (scriptEditForm == null || scriptEditForm.IsDisposed)
                    return null;
                return scriptEditForm;
            }
        }

        #region"ϵͳ��ʼ��"
        public DebuggerForm()
        {
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.RestoreWindowState();
            this.Icon = Heren.MedQC.ScriptEngine.Properties.Resources.SysIcon;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();
            this.ShowScriptSamplesForm();

            //����Ϊ�Զ����洰��״̬
            this.SaveWindowState = true;

            //Ϊ�գ����ʾ����������EXE
            if (this.m_scriptProperty == null)
            {
                this.toolbtnNew.Visible = true;
                this.toolbtnOK.Visible = false;
            }
            //���򣬱�ʾ�ĳ��򱻱�ĳ�����ã�ֱ�Ӵ򿪲����еĴ���
            else
            {
                this.toolbtnNew.Visible = false;
                this.OpenScript(this.m_scriptProperty);
                if (this.ActiveScriptForm != null)
                    this.ActiveScriptForm.CloseButtonVisible = false;
            }
            this.ShowStatusMessage("����!");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            //���SaveWindowStateֵΪfalse,˵��δ��¼
            if (!this.SaveWindowState)
                return;

            //��鲢��ʾ�Ƿ���Ҫ����
            int index = 0;
            DockContentBase content = null;
            while (index < this.dockPanel1.Contents.Count)
            {
                content = this.dockPanel1.Contents[index] as DockContentBase;
                index++;
                if (content == null || content.IsDisposed)
                    continue;
                e.Cancel = !content.CheckModifiedData();
                if (e.Cancel) return;
            }
        }
        #endregion

        #region "�������¼�"
        private void toolbtnNew_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.CreateNewScript();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "��ѡ��ű��ļ�";
            StringBuilder sbFilter = new StringBuilder();
            sbFilter.Append("�ű��ļ�(*.vbs)|*.vbs|");
            sbFilter.Append("�ı��ļ�(*.txt)|*.txt|");
            sbFilter.Append("�����ļ�(*.*)|*.*");
            openDialog.Filter = sbFilter.ToString();
            openDialog.Multiselect = false;
            if (openDialog.ShowDialog() != DialogResult.OK)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.OpenScript(openDialog.FileName);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnSaveAs_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.SaveScript();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnComment_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            ScriptEditForm scriptEditForm = this.ActiveScriptForm;
            if (scriptEditForm != null)
                scriptEditForm.Comment();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnUncomment_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            ScriptEditForm scriptEditForm = this.ActiveScriptForm;
            if (scriptEditForm != null)
                scriptEditForm.Comment();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnTest_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            //this.ShowScriptTestForm();
            this.TestScript();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        private void TestScript()
        {
            if (this.PatVisitInfo == null)
            {
                MessageBoxEx.ShowMessage("����ѡ����");
                return;
            }
            if (this.QcCheckPoint == null)
            {
                MessageBoxEx.ShowMessage("ȱ�ݹ���δ��ʼ��");
                return;
            }

        }
        private void toolbtnOK_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (!this.CompileWithOK())
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnExamples_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowScriptSamplesForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnHelp_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            //���û��ֲ�(Word�ĵ�)
            string helpFile = string.Format("{0}\\Script\\Ԫ�ػ�������VB�ű������ֲ�.doc", this.WorkingPath);
            try
            {
                if (System.IO.File.Exists(helpFile))
                    System.Diagnostics.Process.Start(helpFile);
                else
                    MessageBoxEx.Show("δ�ҵ������ĵ��ļ�!");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("�޷��򿪰����ĵ��ļ�!", ex.Message);
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region "��ʾ�Ӵ���"
        /// <summary>
        /// ��ʾ�ű���������
        /// </summary>
        private void ShowScriptSamplesForm()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_TempletListForm != null && !this.m_TempletListForm.IsDisposed)
            {
                this.m_TempletListForm.Activate();
            }
            else
            {
                this.m_TempletListForm = new TempletListForm(this);
                this.m_TempletListForm.Show(this.dockPanel1);
            }
            this.m_TempletListForm.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// ��ʾ��������б�
        /// </summary>
        internal void ShowCompileErrorForm(CompileErrorCollection errors)
        {
            if (errors == null || errors.Count <= 0)
                return;
            ScriptEditForm activeScriptForm = this.ActiveScriptForm;
            if (activeScriptForm == null || activeScriptForm.IsDisposed)
                return;
            if (this.m_ErrorsListForm == null || this.m_ErrorsListForm.IsDisposed)
            {
                this.m_ErrorsListForm = new ErrorsListForm(this);
                this.m_ErrorsListForm.Show(this.dockPanel1, DockState.DockBottom);
            }
            this.m_ErrorsListForm.Activate();
            this.m_ErrorsListForm.CompileErrors = errors;
            this.m_ErrorsListForm.ScriptEditForm = activeScriptForm;
            this.m_ErrorsListForm.OnRefreshView();
        }

        /// <summary>
        /// ��ʾ�ű����Դ���
        /// </summary>
        internal void ShowScriptTestForm()
        {
            if (this.m_ErrorsListForm != null && !this.m_ErrorsListForm.IsDisposed)
                this.m_ErrorsListForm.Close();
            ScriptEditForm activeScriptForm = this.ActiveScriptForm;
            if (activeScriptForm == null || activeScriptForm.IsDisposed)
                return;
            ScriptProperty scriptProperty = activeScriptForm.ScriptProperty;
            if (scriptProperty == null)
                scriptProperty = new ScriptProperty();

            ScriptCompiler.Instance.WorkingPath = this.WorkingPath;
            CompileResults results = ScriptCompiler.Instance.CompileScript(scriptProperty);
            if (results.HasErrors)
            {
                this.ShowCompileErrorForm(results.Errors);
                MessageBoxEx.Show("����ʧ�ܣ��޷��������Գ���");
                return;
            }
            ScriptTestForm scriptTestForm = new ScriptTestForm();
            scriptTestForm.ShowDialog(scriptProperty.ScriptName, scriptProperty.ScriptText);
        }
        #endregion

        public void ShowStatusMessage(string szMessage)
        {
            this.tsslblSystemStatus.Text = szMessage;
        }

        /// <summary>
        /// ��ȡָ��·���Ľű��༭���ڶ���
        /// </summary>
        /// <param name="szFileName">ָ��·��</param>
        /// <returns>ScriptEditForm</returns>
        private ScriptEditForm GetScriptEditForm(string szFileName)
        {
            IDockContent[] documents = this.dockPanel1.DocumentsToArray();
            for (int index = 0; index < documents.Length; index++)
            {
                ScriptEditForm scriptForm = documents[index] as ScriptEditForm;
                if (scriptForm == null || scriptForm.IsDisposed)
                    continue;
                if (scriptForm.ScriptProperty == null)
                    continue;
                string szFilePath = scriptForm.ScriptProperty.FilePath;
                if (string.Compare(szFilePath, szFileName, true) == 0)
                    return scriptForm;
            }
            return null;
        }

        /// <summary>
        /// ����һ���µĽű��༭����
        /// </summary>
        internal void CreateNewScript()
        {
            ScriptNewForm frmNewScript = new ScriptNewForm();
            if (frmNewScript.ShowDialog() != DialogResult.OK)
                return;
            ScriptEditForm scriptEditForm = new ScriptEditForm(this);
            scriptEditForm.Show(this.dockPanel1, DockState.Document);
            this.dockPanel1.Update();

            string szScriptFile = frmNewScript.FileName;
            string szScriptText = null;
            if (!GlobalMethods.IO.GetFileText(szScriptFile, ref szScriptText))
                return;

            ScriptProperty scriptProperty = new ScriptProperty();
            scriptProperty.ScriptName = frmNewScript.ScriptTitle;
            scriptEditForm.ScriptProperty = scriptProperty;
            if (!scriptEditForm.OpenScriptText(szScriptText))
                MessageBoxEx.ShowError("�ű�����ʧ��!");
        }

        /// <summary>
        /// �򿪱����ļ�
        /// </summary>
        /// <param name="szFullName">�ļ�·��</param>
        internal void OpenScript(string szFullName)
        {
            if (GlobalMethods.Misc.IsEmptyString(szFullName))
                return;
            ScriptProperty scriptProperty = new ScriptProperty();
            string szScriptText = null;
            GlobalMethods.IO.GetFileText(szFullName, ref szScriptText);
            scriptProperty.ScriptText = szScriptText;
            scriptProperty.FilePath = szFullName;
            scriptProperty.ScriptName = GlobalMethods.IO.GetFileName(szFullName, true);
            this.OpenScript(scriptProperty);
        }

        /// <summary>
        /// ��ָ���ű�������Ϣ�Ľű�
        /// </summary>
        /// <param name="scriptProperty">�ű�������Ϣ</param>
        internal void OpenScript(ScriptProperty scriptProperty)
        {
            if (scriptProperty == null)
                return;
            string szScriptFile = scriptProperty.FilePath;
            ScriptEditForm scriptEditForm = this.GetScriptEditForm(szScriptFile);
            if (scriptEditForm != null)
            {
                scriptEditForm.Activate();
                scriptEditForm.OnRefreshView();
                return;
            }
            if (scriptEditForm == null || scriptEditForm.IsDisposed)
            {
                scriptEditForm = new ScriptEditForm(this);
                scriptEditForm.Show(this.dockPanel1, DockState.Document);
            }
            scriptEditForm.ScriptProperty = scriptProperty;
            this.dockPanel1.Update();
            if (!scriptEditForm.OpenScriptText(scriptProperty.ScriptText))
                MessageBoxEx.Show("�ļ���ʧ�ܣ�", MessageBoxIcon.Error);
        }

        /// <summary>
        /// ���ű�������
        /// </summary>
        private void SaveScript()
        {
            ScriptEditForm activeScriptForm = this.ActiveScriptForm;
            if (activeScriptForm != null && !activeScriptForm.IsDisposed)
                activeScriptForm.SaveScript();
        }

        /// <summary>
        /// ����ȷ����ť�����ɶ�����DLL
        /// </summary>
        /// <returns>�Ƿ����ɹ�</returns>
        private bool CompileWithOK()
        {
            if (this.m_scriptProperty == null)
                this.m_scriptProperty = new ScriptProperty();
            ScriptEditForm activeScriptForm = this.GetScriptEditForm(this.m_scriptProperty.FilePath);
            if (activeScriptForm == null)
                return false;
            activeScriptForm.Activate();
            if (!activeScriptForm.Compile())
            {
                MessageBoxEx.Show("����ʧ�ܣ���鿴�����б�");
                return false;
            }
            activeScriptForm.IsModified = false;
            this.m_scriptProperty = activeScriptForm.ScriptProperty;
            return true;
        }
    }
}