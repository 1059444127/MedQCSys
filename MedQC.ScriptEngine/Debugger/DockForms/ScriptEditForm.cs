// ***********************************************************
// ���Ӳ���ϵͳ����Ԫ���Զ�����ű��༭����.
// Creator: YangMingkun  Date:2011-11-10
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.DockSuite;
using Heren.Common.TextEditor;
using Heren.Common.TextEditor.Actions;
using Heren.Common.TextEditor.Document;
using Heren.Common.Libraries;
using Heren.MedQC.ScriptEngine.Script;

namespace Heren.MedQC.ScriptEngine.Debugger
{
    internal partial class ScriptEditForm : DockContentBase
    {
        private ScriptProperty m_scriptProperty = null;
        /// <summary>
        /// ��ȡ�����õ�ǰ�򿪵Ľű�Դ��
        /// </summary>
        public ScriptProperty ScriptProperty
        {
            get
            {
                this.RefreshScriptProperty();
                return this.m_scriptProperty;
            }
            set { this.m_scriptProperty = value; }
        }

        private bool m_bIsModified = false;
        /// <summary>
        /// ��ȡ�������ĵ��Ƿ��Ѿ������仯
        /// </summary>
        public bool IsModified
        {
            get { return this.m_bIsModified; }
            set { this.m_bIsModified = value; }
        }

        public ScriptEditForm(DebuggerForm mainForm)
            : base(mainForm)
        {
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.m_scriptProperty = new ScriptProperty();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitializeComponent();
            this.UpdateBounds();
            this.textEditorControl1.ShowEOLMarkers = false;
            this.textEditorControl1.ShowTabs = false;
            this.textEditorControl1.ShowSpaces = false;
            this.textEditorControl1.ShowMatchingBracket = true;
            this.textEditorControl1.EnableFolding = true;
            this.textEditorControl1.ShowInvalidLines = false;
            this.textEditorControl1.SetHighlighting("VBNET");
            this.textEditorControl1.IndentStyle = IndentStyle.Auto;
            this.textEditorControl1.Font = new Font("����", 9f, FontStyle.Regular);
            this.textEditorControl1.Document.DocumentChanged +=
                new DocumentEventHandler(this.TextEditor_DocumentChanged);
        }

        /// <summary>
        /// ˢ�´��ڱ���
        /// </summary>
        private void RefreshWindowTitle()
        {
            if (this.m_scriptProperty == null)
                return;
            this.Text = this.m_scriptProperty.ScriptName;
            if (this.m_bIsModified)
                this.Text += " *";
            this.TabSubhead = "��ţ�"
                + GlobalMethods.IO.GetFileName(this.m_scriptProperty.FilePath, false);
        }

        private void TextEditor_DocumentChanged(object sender, DocumentEventArgs e)
        {
            this.m_bIsModified = true;
            this.RefreshWindowTitle();
        }

        /// <summary>
        /// ˢ�½ű�������Ϣ����
        /// </summary>
        private void RefreshScriptProperty()
        {
            if (this.m_scriptProperty == null)
                this.m_scriptProperty = new ScriptProperty();
            this.m_scriptProperty.ScriptText = this.textEditorControl1.Text;
        }

        /// <summary>
        /// ��鵱ǰ���ڵ������Ƿ����޸�
        /// </summary>
        /// <returns>bool</returns>
        public override bool HasUncommit()
        {
            if (this.textEditorControl1 == null || this.textEditorControl1.IsDisposed)
                return false;
            return this.m_bIsModified;
        }

        /// <summary>
        /// �ύ��ǰ�ѱ��޸ĵĽű�
        /// </summary>
        /// <returns>bool</returns>
        public override bool CommitModify()
        {
            return this.SaveScript();
        }

        /// <summary>
        /// ��սű������ı�
        /// </summary>
        public void Clear()
        {
            TextArea textArea = null;
            try
            {
                textArea = this.textEditorControl1.ActiveTextAreaControl.TextArea;
                textArea.Document.TextContent = string.Empty;
                textArea.Invalidate();
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("ScriptEditForm.Clear", ex);
            }
        }

        /// <summary>
        /// ��ָ���ű�����
        /// </summary>
        /// <param name="source">�ű�����</param>
        /// <returns>bool</returns>
        public bool OpenScriptText(string source)
        {
            this.Clear();
            if (this.m_scriptProperty == null)
                this.m_scriptProperty = new ScriptProperty();
            this.m_scriptProperty.ScriptText = source;

            try
            {
                TextArea textArea =
                    this.textEditorControl1.ActiveTextAreaControl.TextArea;
                textArea.Document.TextContent = source;
                textArea.Invalidate();
                this.m_bIsModified = false;
                this.RefreshWindowTitle();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("ScriptEditForm.OpenDoument", ex);
                return false;
            }
        }

        /// <summary>
        /// ��ָ��·�����ļ�
        /// </summary>
        /// <param name="szFullName">�ļ�·��</param>
        /// <returns>bool</returns>
        public bool OpenScriptFile(string szFullName)
        {
            this.Clear();
            if (this.m_scriptProperty == null)
                this.m_scriptProperty = new ScriptProperty();
            this.m_scriptProperty.FilePath = szFullName;

            if (GlobalMethods.Misc.IsEmptyString(szFullName))
                return true;
            string szSource = null;
            if (!GlobalMethods.IO.GetFileText(szFullName, ref szSource))
                return false;
            return this.OpenScriptText(szSource);
        }

        /// <summary>
        /// ���浱ǰ�ű�
        /// </summary>
        /// <returns>bool</returns>
        public bool SaveScript()
        {
            if (this.m_scriptProperty == null)
                return false;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "��ѡ�񱣴�·��";
            StringBuilder sbFilter = new StringBuilder();
            sbFilter.Append("�ű��ļ�(*.vbs)|*.vbs|");
            sbFilter.Append("�ı��ļ�(*.txt)|*.txt|");
            sbFilter.Append("�����ļ�(*.*)|*.*");
            saveDialog.Filter = sbFilter.ToString();
            saveDialog.FileName = this.m_scriptProperty.ScriptName;
            if (saveDialog.ShowDialog() != DialogResult.OK)
                return false;
            return this.SaveScript(saveDialog.FileName);
        }

        /// <summary>
        /// ���浱ǰ�ű�
        /// </summary>
        /// <returns>bool</returns>
        public bool SaveScript(string szFileName)
        {
            if (GlobalMethods.Misc.IsEmptyString(szFileName))
                return false;
            try
            {
                this.textEditorControl1.SaveFile(szFileName);
                this.m_bIsModified = false;
                this.RefreshWindowTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("�ű����ʧ�ܣ�\r\n" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// ���뵱ǰ�ű�
        /// </summary>
        /// <returns>bool</returns>
        public bool Compile()
        {
            if (this.DebuggerForm == null || this.DebuggerForm.IsDisposed)
                return false;

            if (this.m_scriptProperty == null)
                this.m_scriptProperty = new ScriptProperty();
            ScriptProperty scriptProperty =
                this.m_scriptProperty.Clone() as ScriptProperty;
            scriptProperty.ScriptText = this.textEditorControl1.Text;

            ScriptCompiler.Instance.WorkingPath = this.DebuggerForm.WorkingPath;
            CompileResults results = ScriptCompiler.Instance.CompileScript(scriptProperty);
            if (results.HasErrors)
            {
                this.DebuggerForm.ShowCompileErrorForm(results.Errors);
                return false;
            }
            this.m_scriptProperty = scriptProperty;
            return !results.HasErrors;
        }

        public void SetAsReadonly()
        {
            try
            {
                this.textEditorControl1.ActiveTextAreaControl.Document.ReadOnly = true;
            }
            catch { }
        }

        public bool Comment()
        {
            TextArea textArea = null;
            ToggleComment toggle = null;
            try
            {
                textArea = this.textEditorControl1.ActiveTextAreaControl.TextArea;
                toggle = new ToggleComment();
                toggle.Execute(textArea);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("ScriptEditForm.Comment", ex);
                return false;
            }
        }

        public bool Uncomment()
        {
            return this.Comment();
        }

        public bool FormatScript()
        {
            TextArea textArea = null;
            FormatBuffer formatBuffer = null;
            try
            {
                textArea = this.textEditorControl1.ActiveTextAreaControl.TextArea;
                formatBuffer = new FormatBuffer();
                formatBuffer.Execute(textArea);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("ScriptEditForm.FormatScript", ex);
                return false;
            }
        }

        public void LocateTo(int nLine, int nColumn)
        {
            try
            {
                this.Activate();
                this.textEditorControl1.ActiveTextAreaControl.JumpTo(nLine - 1, nColumn);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("ScriptEditForm.LocateTo", ex);
            }
        }
    }
}