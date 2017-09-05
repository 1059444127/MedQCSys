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
using EMRDBLib;
using Heren.MedQC.ScriptEngine.FindReplace;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.ScriptEngine.Debugger
{
    internal partial class ScriptEditForm : DockContentBase
    {
        public string FlagCode { get; set; }
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
        /// <summary>
        /// �ű�����
        /// </summary>
        public ScriptConfig ScriptConfig { get; set; }
        private bool m_bIsModified = false;
        /// <summary>
        /// ��ȡ�������ĵ��Ƿ��Ѿ������仯
        /// </summary>
        public bool IsModified
        {
            get { return this.m_bIsModified; }
            set { this.m_bIsModified = value;
                this.RefreshFormText();
            }
        }

        private string m_vbsFile = null;

        /// <summary>
        /// ��ȡ���򿪵��Ǳ����ļ�ʱ,
        /// ��������ڵ�ǰ�����ı����ļ�·��
        /// </summary>
        public string VbsFile
        {
            get { return this.m_vbsFile; }
        }
        public DebuggerForm MainForm { get; set; }
        public ScriptEditForm(DebuggerForm mainForm)
            : base(mainForm)
        {
            this.MainForm = mainForm;
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
        /// ���浱ǰ���ڵ������޸�
        /// </summary>
        /// <returns>bool</returns>
        public override bool CommitModify()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (!this.DebuggerForm.CompileWithOK())
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return false;
            }
            bool success = ScriptHandler.Instance.SaveTemplet();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            return success;
        }
        /// <summary>
        /// ˢ�±�����
        /// </summary>
        private void RefreshFormText()
        {
            string title = string.Empty;
            string subhead = string.Empty;
            if (this.ScriptConfig == null)
            {
                title = "��ģ��";
            }
            if (this.m_vbsFile != null)
            {
                title = GlobalMethods.IO.GetFileName(this.m_vbsFile, true);
            }
            if (this.ScriptConfig != null)
            {
                title = this.ScriptConfig.SCRIPT_NAME;
                subhead = "����ʱ��:"
                    + this.ScriptConfig.MODIFY_TIME.ToString("yyyy��M��d�� HH:mm");
            }
            if (!this.IsModified)
                this.Text = title + "(�ű�)";
            else
                this.Text = title + "(�ű�) *";
            this.TabSubhead = subhead;
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
            this.IsModified = true;
            //this.RefreshWindowTitle();
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

        ///// <summary>
        ///// �ύ��ǰ�ѱ��޸ĵĽű�
        ///// </summary>
        ///// <returns>bool</returns>
        //public override bool CommitModify()
        //{
        //    return this.SaveScript();
        //}

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
                this.IsModified = false;
                //this.RefreshWindowTitle();
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
            this.m_vbsFile = szFullName;
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
                this.IsModified = false;
                //this.RefreshWindowTitle();
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

        /// <summary>
        /// ��ȡ��ǰѡ�е��ı�
        /// </summary>
        /// <returns>ѡ�е��ı�</returns>
        public string GetSelectedText()
        {
            return this.textEditorControl1.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
        }

        /// <summary>
        /// ������ָ�����ı�ƥ��������ı�
        /// </summary>
        /// <param name="szFindText">�ı�</param>
        /// <param name="bMatchCase">�Ƿ�ƥ���Сд</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public void FindText(string szFindText, bool bMatchCase)
        {
            List<FindResult> results = FindHandler.Instance.FindAll(this.textEditorControl1, szFindText, bMatchCase);
            if (this.MainForm != null && !this.MainForm.IsDisposed)
                this.MainForm.ShowFindResultForm(this, szFindText, results, false);
        }

        /// <summary>
        /// �����б����в���ָ�����ı�
        /// </summary>
        /// <param name="szFindText">�ı�</param>
        /// <param name="bMatchCase">�Ƿ�ƥ���Сд</param>
        public void FindTextInAllTemplet(string szFindText, bool bMatchCase)
        {
            if (!bMatchCase)
                szFindText = szFindText.ToLower();
            List<ScriptConfig> lstScriptConfig = new List<ScriptConfig>();
            ScriptConfigAccess.Instance.GetScriptConfigs(ref lstScriptConfig);
            if (lstScriptConfig.Count <= 0)
                return;

            List<FindResult> Result = new List<FindResult>();
            int indextext = 0;//������
            int indexLine = 0;//�к�
            int i = 0;//���ڱȶԵ��ַ����
            int indexCol = 0;
            char chFindText = new char();
            string sztextFormat = string.Empty;
            char[] arrFindText = szFindText.Trim().ToCharArray();
            for (int index = 0; index < lstScriptConfig.Count; index++)
            {
                if (lstScriptConfig[index].IS_FOLDER==1)
                    continue;
                string szSource = string.Empty;
                ScriptConfigAccess.Instance.GetScriptSource(lstScriptConfig[index].SCRIPT_ID, ref szSource);
                if (string.IsNullOrEmpty(szSource))
                    continue;
                //string szScripData = parser.GetScriptData(lstScriptConfig[index].REPORT_DATA);
                string[] arrScripText = szSource.Split(new Char[] { '\n' }, StringSplitOptions.None);

                indexLine = 0; //�к�����
                indextext = 0; //����������
                foreach (string sztext in arrScripText)
                {
                    if (string.IsNullOrEmpty(sztext))
                        continue;
                    if (!bMatchCase)
                        sztextFormat = sztext.ToLower();
                    else
                        sztextFormat = sztext;
                    char[] arrCtext = sztextFormat.ToCharArray();
                    i = 0;//���ڱȶԵ��ַ����
                    for (indexCol = 0; indexCol < arrCtext.Length; indexCol++)
                    {
                        chFindText = arrCtext[indexCol];
                        indextext++;
                        if (i != 0 && chFindText != arrFindText[i])
                        { indexCol -= i - 1; indextext -= i - 1; i = 0; continue; }
                        if (chFindText != arrFindText[i])
                        { i = 0; continue; }
                        if (i == arrFindText.Length - 1)
                        {
                            Result.Add(new FindResult(indextext - szFindText.Trim().Length
                                , szFindText.Trim().Length
                                , indexLine
                                , sztext
                                , lstScriptConfig[index].SCRIPT_ID
                                , lstScriptConfig[index].SCRIPT_NAME
                                , SystemData.FileType.SCRIPT));
                            i = 0;
                            continue;
                        }
                        i++;
                    }
                    indextext++;//��������ȥ��'\n'������ƫ����
                    indexLine++;
                }
            }

            if (this.MainForm != null && !this.MainForm.IsDisposed)
                this.MainForm.ShowFindResultForm(this, szFindText, Result, true);
        }

        /// <summary>
        /// ���Ҳ��滻ָ�����ı�
        /// </summary>
        /// <param name="szFindText">�����ı�</param>
        /// <param name="szReplaceText">�滻�ı�</param>
        /// <param name="bMatchCase">�Ƿ�ƥ���Сд</param>
        /// <param name="bReplaceAll">�Ƿ��滻����</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public void ReplaceText(string szFindText, string szReplaceText, bool bMatchCase, bool bReplaceAll)
        {
            if (bReplaceAll)
                FindHandler.Instance.ReplaceAll(this.textEditorControl1, szFindText, szReplaceText, bMatchCase);
            else
                FindHandler.Instance.ReplaceNext(this.textEditorControl1, szFindText, szReplaceText, bMatchCase);
        }
        
        /// <summary>
        /// ����ǰ�ű��༭�����ڹ�궨λ��ָ�����ı�
        /// </summary>
        /// <param name="offset">����λ��</param>
        /// <param name="length">����</param>
        public void LocateToText(int offset, int length)
        {
            this.Activate();
            this.textEditorControl1.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
            if (offset < 0 || length < 0
                || offset >= this.textEditorControl1.Document.TextLength
                || offset + length >= this.textEditorControl1.Document.TextLength)
                return;
            Point startPos = this.textEditorControl1.Document.OffsetToPosition(offset);
            Point endPos = this.textEditorControl1.Document.OffsetToPosition(offset + length);
            this.textEditorControl1.ActiveTextAreaControl.TextArea.SelectionManager.SetSelection(startPos, endPos);
            this.textEditorControl1.ActiveTextAreaControl.TextArea.Caret.Position = this.textEditorControl1.Document.OffsetToPosition(offset);
        }
        
        private void toolmnuFind_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowFindReplaceForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolmnuFindSelected_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.FindText(this.GetSelectedText(), false);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuUndo_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.textEditorControl1.Undo();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuRedo_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.textEditorControl1.Redo();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuCut_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.textEditorControl1.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.textEditorControl1.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.textEditorControl1.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
    }
}