// ***********************************************************
// ���������ù���ϵͳ,��ģ�����ģ�������.
// ��Ҫ�����ģ���ļ�������,ʵ���������,�����ر�ģ���ļ�
// Author : YangMingkun, Date : 2012-6-6
// Copyright : Heren Health Services Co.,Ltd.
// ***********************************************************
using System;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using Heren.Common.Forms.Loader;
using EMRDBLib;
using EMRDBLib.DbAccess;
using Heren.MedQC.ScriptEngine.Debugger;
using Heren.MedQC.ScriptEngine.Script;

namespace Heren.MedQC.ScriptEngine
{
    internal class ScriptHandler
    {
        private DebuggerForm m_mainForm = null;

        /// <summary>
        /// ��ȡ��ǰ�����򴰿ڶ���
        /// </summary>
        public DebuggerForm MainForm
        {
            get
            {
                if (this.m_mainForm == null)
                    return null;
                if (this.m_mainForm.IsDisposed)
                    return null;
                return this.m_mainForm;
            }
        }

        public ScriptEditForm ActiveScript
        {
            get
            {
                ScriptEditForm activeScript =
                    this.MainForm.ActiveScriptForm as ScriptEditForm;
                if (activeScript == null || activeScript.IsDisposed)
                    return null;
                return activeScript;
            }
        }

        private static ScriptHandler m_instance = null;

        /// <summary>
        /// ��ȡ��ǰģ�崦����ʵ��
        /// </summary>
        public static ScriptHandler Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ScriptHandler();
                return m_instance;
            }
        }

        private ScriptHandler()
        {
        }

        /// <summary>
        /// ��ʼ��ģ�崦��������
        /// </summary>
        /// <param name="mainForm">�����򴰿�</param>
        public void InitTempletHandler(DebuggerForm mainForm)
        {
            this.m_mainForm = mainForm;
        }

        private Script.CompileErrorCollection GetCompileErrors(Script.CompileResults results)
        {
            Script.CompileErrorCollection errors = null;
            if (results == null)
                return null;
            errors = results.Errors;
            return errors;
        }

        internal void ShowScriptTestForm()
        {
            //ScriptEditForm scriptForm = this.ActiveScript;
            //if (scriptForm == null)
            //    return;
            //scriptForm = this.GetScriptForm();
            //FormFileParser parser = new FormFileParser();
            //string szScriptData = null;
            //if (scriptForm != null)
            //    szScriptData = scriptForm.Save();
            //else
            //    szScriptData = parser.GetScriptData(designForm.HndfFile);

            //string szDesignData = null;
            //if (designForm != null)
            //    designForm.Save(ref szDesignData);
            //else
            //    szDesignData = parser.GetDesignData(scriptForm.HndfFile);

            ////����ű�
            //ScriptProperty scriptProperty = new ScriptProperty();
            //scriptProperty.ScriptText = szScriptData;
            //CompileResults results = null;
            //results = ScriptCompiler.Instance.CompileScript(scriptProperty);
            //if (!results.HasErrors)
            //{
            //    this.MainForm.ShowCompileErrorForm(null);
            //}
            //else
            //{
            //    if (scriptForm == null)
            //        this.OpenScriptEditForm(designForm);
            //    this.MainForm.ShowCompileErrorForm(this.GetCompileErrors(results));
            //    MessageBoxEx.Show("����ʧ�ܣ��޷��������Գ���");
            //    return;
            //}

            //ScriptTestForm scriptTestForm = new ScriptTestForm();
            ////scriptTestForm.ScriptData = szScriptData;
            ////scriptTestForm.DesignData = szDesignData;
            //scriptTestForm.ShowDialog();
        }

        internal ScriptEditForm GetScriptForm(ScriptConfig scriptConfig)
        {
            if (this.MainForm == null)
                return null;
            foreach (IDockContent content in this.MainForm.Documents)
            {
                ScriptEditForm scriptForm = content as ScriptEditForm;
                if (scriptForm == null|| scriptForm.ScriptConfig==null)
                    continue;
                if (scriptForm.ScriptConfig.SCRIPT_ID == scriptConfig.SCRIPT_ID)
                    return scriptForm;
            }
            return null;
        }
        internal short OpenScriptConfig(ScriptConfig scriptConfig)
        {
            if (scriptConfig == null)
                return SystemData.ReturnValue.FAILED;

            ScriptEditForm scriptEditForm = this.GetScriptForm(scriptConfig);
            if (scriptEditForm != null)
            {
                scriptEditForm.Activate();
                return SystemData.ReturnValue.OK;
            }
            scriptEditForm = new ScriptEditForm(this.MainForm);
            scriptEditForm.FlagCode = Guid.NewGuid().ToString();
            scriptEditForm.ScriptConfig = scriptConfig;

            this.MainForm.OpenScriptEditForm(scriptEditForm);

            string szScriptID = scriptConfig.SCRIPT_ID;
            string szHndfFile = string.Format("{0}\\Cache\\{1}.vbs"
                , GlobalMethods.Misc.GetWorkingPath(), szScriptID);

            string byteTempletData = null;
            if (szScriptID != string.Empty)
            {
                short shRet = ScriptConfigAccess.Instance.GetScriptSource(szScriptID, ref byteTempletData);
                if (shRet != SystemData.ReturnValue.OK)
                    return shRet;
            }

            GlobalMethods.IO.WriteFileText(szHndfFile, byteTempletData);
            return scriptEditForm.OpenScriptFile(szHndfFile) ?
                SystemData.ReturnValue.OK : SystemData.ReturnValue.FAILED;
        }

        //internal short OpenTemplet(string szFileName)
        //{
        //    if (string.IsNullOrEmpty(szFileName))
        //        return SystemData.ReturnValue.PARAM_ERROR;
        //    ScriptEditForm designEditForm = new ScriptEditForm(this.MainForm);
        //    designEditForm.FlagCode = Guid.NewGuid().ToString();
        //    this.MainForm.OpenScriptEditForm(designEditForm);

        //    return designEditForm.Open(szFileName) ?
        //        SystemData.ReturnValue.OK : SystemData.ReturnValue.FAILED;
        //}

        internal short OpenTemplet()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "�ű�ģ��(*.vbs)|*.vbs|�����ļ�(*.*)|*.*";
            openDialog.FilterIndex = 1;
            if (openDialog.ShowDialog() != DialogResult.OK)
                return SystemData.ReturnValue.CANCEL;

            ScriptEditForm designEditForm = new ScriptEditForm(this.MainForm);

            designEditForm.FlagCode = Guid.NewGuid().ToString();
            this.MainForm.OpenScriptEditForm(designEditForm);

            return designEditForm.OpenScriptFile(openDialog.FileName) ?
                SystemData.ReturnValue.OK : SystemData.ReturnValue.FAILED;
        }

        //internal short OpenTemplet(DocTypeInfo docTypeInfo)
        //{
        //    if (docTypeInfo == null)
        //        return SystemData.ReturnValue.FAILED;

        //    DesignEditForm designEditForm = this.GetDesignForm(docTypeInfo);
        //    if (designEditForm != null)
        //    {
        //        designEditForm.Activate();
        //        return SystemData.ReturnValue.OK;
        //    }
        //    designEditForm = new DesignEditForm(this.MainForm);
        //    designEditForm.FlagCode = Guid.NewGuid().ToString();
        //    this.MainForm.OpenDesignEditForm(designEditForm);

        //    string szDocTypeID = docTypeInfo.DocTypeID;
        //    string szHndfFile = string.Format("{0}\\Cache\\{1}.hndt"
        //        , GlobalMethods.Misc.GetWorkingPath(), szDocTypeID);

        //    byte[] byteTempletData = null;
        //    //short shRet = TempletService.Instance.GetFormTemplet(szDocTypeID, ref byteTempletData);
        //    //if (shRet != SystemData.ReturnValue.OK)
        //    //    return shRet;

        //    GlobalMethods.IO.WriteFileBytes(szHndfFile, byteTempletData);
        //    return designEditForm.Open(docTypeInfo, szHndfFile) ?
        //        SystemData.ReturnValue.OK : SystemData.ReturnValue.FAILED;
        //}

        /// <summary>
        /// ���浱ǰ���ڱ༭��ģ���ļ�
        /// </summary>
        /// <returns>bool</returns>
        internal bool SaveTemplet()
        {
            ScriptEditForm scriptEditForm = this.ActiveScript;
            if (scriptEditForm == null)
                return false;
            FormFileParser parser = new FormFileParser();

            string szScriptSource = scriptEditForm.ScriptProperty.ScriptText;
            byte[] byteScriptData = scriptEditForm.ScriptProperty.ScriptData;

            DialogResult result = MessageBoxEx.ShowQuestion("�Ƿ��ύ����������"
                + "\r\n�ύ��������,�������ǡ���ť!\r\n�����浽����,�������񡱰�ť!");
            if (result == DialogResult.Cancel)
                return false;
            bool success = true;
            if (result == DialogResult.No)
                success = this.SaveTempletToLocal(szScriptSource);
            else
                success = this.SaveTempletToServer(szScriptSource, byteScriptData);
            if (success)
            {
                if (scriptEditForm != null) scriptEditForm.IsModified = false;
                //ScriptCache.Instance.Dispose();
                //ScriptCache.Instance.Initialize();
            }
            return success;
        }

        private bool SaveTempletToLocal(string byteTempletData)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "�ű�ģ��(*.vbs)|�����ļ�(*.*)|*.*";
            saveDialog.Title = "���Ϊ����ģ���ļ�";
            saveDialog.RestoreDirectory = true;

            string szFileName = string.Empty;
            if (this.ActiveScript != null)
                szFileName = this.ActiveScript.Text + ".vbs";
            szFileName = szFileName.Replace("(�ű�)", string.Empty);
            szFileName = szFileName.Replace("*", string.Empty);
            saveDialog.FileName = szFileName.Replace(" ", string.Empty);

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return false;

            if (!GlobalMethods.IO.WriteFileText(saveDialog.FileName, byteTempletData))
            {
                MessageBoxEx.Show("�ĵ�����д���ļ�ʧ��!");
                return false;
            }
            return true;
        }

        private bool SaveTempletToServer(string szScriptSource,byte[] byteScriptData)
        {
            //��ȡ��ǰģ��������Ϣ
            ScriptConfig scriptConfig = null;
            if (this.ActiveScript != null)
                scriptConfig = this.ActiveScript.ScriptConfig;
            else if (this.ActiveScript != null)
                scriptConfig = this.ActiveScript.ScriptConfig;

            ScriptSelectForm frmTempletSelect = new ScriptSelectForm();
            frmTempletSelect.Description = "��ѡ����Ҫ���µ�Ŀ�겡��ģ�壺";
            frmTempletSelect.MultiSelect = false;
            if (scriptConfig != null)
            {
                frmTempletSelect.DefaultDocTypeID = scriptConfig.SCRIPT_ID;
            }
            if (frmTempletSelect.ShowDialog() != DialogResult.OK)
                return false;
            if (frmTempletSelect.SelectedScriptConfigs == null)
                return false;
            if (frmTempletSelect.SelectedScriptConfigs.Count <= 0)
                return false;

            scriptConfig = frmTempletSelect.SelectedScriptConfigs[0];
            scriptConfig.MODIFY_TIME = SysTimeHelper.Instance.Now;
            short shRet = ScriptConfigAccess.Instance.SaveScriptDataToDB(scriptConfig, szScriptSource, byteScriptData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                string szDocTypeName = scriptConfig.SCRIPT_NAME;
                MessageBoxEx.Show(string.Format("ģ�塰{0}������ʧ��!", szDocTypeName));
                return false;
            }
            return true;
        }
    }
}
