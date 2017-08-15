/********************************************************
 * @FileName   : ScriptCache.cs
 * @Description: ��������ϵͳ֮�Զ��������ű����洦��
 * @Author     : ������(YangMingkun)
 * @Date       : 2015-12-29 12:56:30
 * @History    : 
 * @Copyright  : ��Ȩ����(C)�㽭���ʿƼ��ɷ����޹�˾
********************************************************/
using System;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Heren.Common.Libraries;
using Heren.MedQC.ScriptEngine.Script;
using Heren.MedQC.Core;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.ScriptEngine.Script
{
    public class ScriptCache
    {
        private static ScriptCache m_instance = null;
        /// <summary>
        /// ��ȡTempletCacheʵ��
        /// </summary>
        public static ScriptCache Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ScriptCache();
                return m_instance;
            }
        }
        private ScriptCache()
        {
        }

        /// <summary>
        /// �洢�ű�Dll��ʵ��������
        /// </summary>
        private Hashtable m_htScriptInstance = null;
        /// <summary>
        /// �ű�Dll���ػ��������ļ�·��
        /// </summary>
        private string m_cacheIndexFile = null;

        private List<ScriptConfig> m_scriptInfoList = null;
        /// <summary>
        /// ��ȡ���нű�������Ϣ
        /// </summary>
        private List<ScriptConfig> ScriptInfoList
        {
            get
            {
                if (this.m_scriptInfoList == null)
                    ScriptConfigAccess.Instance.GetScriptConfigs(ref this.m_scriptInfoList);
                if (this.m_scriptInfoList == null)
                    this.m_scriptInfoList = new List<ScriptConfig>();
                return this.m_scriptInfoList;
            }
        }

        /// <summary>
        /// ����Ԫ���Զ����������ô˷���
        /// </summary>
        public void Initialize()
        {
            this.LoadScriptData();
        }

        public void Dispose()
        {
            if (this.m_scriptInfoList != null)
            {
                this.m_scriptInfoList.Clear();
                this.m_scriptInfoList = null;
            }
            if (this.m_htScriptInstance != null)
            {
                this.m_htScriptInstance.Clear();
                this.m_htScriptInstance = null;
            }
        }

        /// <summary>
        /// ��ȡָ���������͵����нű�DLLʵ��������
        /// </summary>
        /// <param name="szExecuteTime">ִ��ʱ��</param>
        /// <param name="szDocTypeID">��������</param>
        /// <param name="elementCalculators">ʵ���������б�</param>
        /// <returns>SystemData.ReturnValue</returns>
        public short GetScriptInstances(string szDocTypeID
            , ref List<IElementCalculator> elementCalculators)
        {
            if (this.ScriptInfoList == null)
                return SystemData.ReturnValue.FAILED;

            if (elementCalculators == null)
                elementCalculators = new List<IElementCalculator>();

            short result = SystemData.ReturnValue.OK;
            for (int index = 0; index < this.ScriptInfoList.Count; index++)
            {
                ScriptConfig scriptInfo = this.ScriptInfoList[index];
                if (scriptInfo == null)
                    continue;

             
                bool docTypeMatched = scriptInfo.SCRIPT_ID == szDocTypeID;
                if (string.IsNullOrEmpty(scriptInfo.SCRIPT_ID))
                    docTypeMatched = true;
                if ( docTypeMatched)
                {
                    List<IElementCalculator> elementCalculatorList = null;
                    result = this.GetScriptInstance(scriptInfo, ref elementCalculatorList);
                    if (elementCalculatorList != null && elementCalculatorList.Count > 0)
                        elementCalculators.AddRange(elementCalculatorList);
                }
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ʼ�����ع���
        /// </summary>
        private void LoadScriptData()
        {
            if (this.ScriptInfoList == null || this.ScriptInfoList.Count == 0)
                return;
            for (int index = 0; index < this.ScriptInfoList.Count; index++)
            {
                ScriptConfig scriptInfo = this.ScriptInfoList[index];
                this.DownLoadScript(scriptInfo);
            }
        }

        /// <summary>
        /// ��ȡָ���ű�ID��ʵ��������
        /// </summary>
        /// <param name="szSCRIPT_ID">�ű�������Ϣ</param>
        /// <param name="elementCalculators">ʵ��������</param>
        /// <returns>SystemData.ReturnValue</returns>
        private short GetScriptInstance(ScriptConfig scriptInfo, ref List<IElementCalculator> elementCalculators)
        {
            if (scriptInfo == null)
                return SystemData.ReturnValue.FAILED;
            if (GlobalMethods.Misc.IsEmptyString(scriptInfo.SCRIPT_ID))
                return SystemData.ReturnValue.FAILED;
            if (this.m_htScriptInstance == null)
                this.m_htScriptInstance = new Hashtable();

            if (this.m_htScriptInstance.Contains(scriptInfo.SCRIPT_ID))
            {
                elementCalculators = this.m_htScriptInstance[scriptInfo.SCRIPT_ID] as List<IElementCalculator>;
                return SystemData.ReturnValue.OK;
            }

            short result = SystemData.ReturnValue.OK;
            if (!this.CheckDllIsNewest(scriptInfo))
            {
                result = this.DownLoadScript(scriptInfo);
                if (result != SystemData.ReturnValue.OK)
                    return SystemData.ReturnValue.FAILED;
            }

            string fileName = this.GetScriptCachePath(scriptInfo.SCRIPT_ID);
            elementCalculators = AssemblyHelper.Instance.GetElementCalculator(fileName);
            if (elementCalculators == null || elementCalculators.Count <= 0)
                return SystemData.ReturnValue.FAILED;
            if (!this.m_htScriptInstance.Contains(scriptInfo.SCRIPT_ID))
                this.m_htScriptInstance.Add(scriptInfo.SCRIPT_ID, elementCalculators);
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ���ؽű�DLL����xmlȫ�ļ�·��
        /// </summary>
        /// <returns></returns>
        private string GetDllCacheIndexFilePath()
        {
            if (GlobalMethods.Misc.IsEmptyString(this.m_cacheIndexFile))
            {
                this.m_cacheIndexFile = GlobalMethods.Misc.GetWorkingPath()
                    + @"\Addins\AutoCalc\Script\Caches\ScriptConfig.xml";
            }
            return this.m_cacheIndexFile;
        }

        /// <summary>
        /// ��ȡ�ű�DLL���ػ���ȫ�ļ�·��
        /// </summary>
        /// <param name="SCRIPT_ID">�ű�ID</param>
        /// <returns>�ű�DLL���ػ���ȫ�ļ�·��</returns>
        private string GetScriptCachePath(string SCRIPT_ID)
        {
            return string.Format(@"{0}\Addins\AutoCalc\Script\Caches\Dll\Calc.{1}.dll"
                , GlobalMethods.Misc.GetWorkingPath(), SCRIPT_ID);
        }

        /// <summary>
        /// ��Ȿ��Dll�ļ��ǲ������°汾
        /// </summary>
        /// <param name="scriptInfo">�ű�������Ϣ</param>
        /// <returns>true/false</returns>
        private bool CheckDllIsNewest(ScriptConfig scriptInfo)
        {
            if (scriptInfo == null)
                return false;

            //1 ���ر��ص������ļ�
            string szConfigFile = this.GetDllCacheIndexFilePath();
            XmlDocument xmlDoc = GlobalMethods.Xml.GetXmlDocument(szConfigFile);

            //2 �����ļ������ڣ���˵������û���κ�DLL����ֱ������
            if (xmlDoc == null)
                return false;

            //3 �����������ļ����������ڵ�ǰָ����DLL����ֱ������
            string szXPath = string.Format("/ScriptConfig/Dll[@Name=\"{0}\"]", scriptInfo.SCRIPT_ID);
            XmlNode dllNode = GlobalMethods.Xml.SelectXmlNode(xmlDoc, szXPath);
            if (dllNode == null)
                return false;

            //4 �����������ļ����Ҵ��ڵ�ǰָ���Ľű�������и������ڵıȽ�
            string szUpdateTime = null;
            if (!GlobalMethods.Xml.GetXmlNodeValue(dllNode, "./@UpdateTime", ref szUpdateTime))
            {
                LogManager.Instance.WriteLog("ScriptCache.CheckDllIsNewest", "��ȡ���������ļ�ʧ�ܣ�");
                return false;
            }

            //5 �����������ļ����Ҵ��ڵ�ǰָ���Ľű������������ڲ�һ�£�����������
            DateTime dtUpdateTime = DateTime.Now;
            GlobalMethods.Convert.StringToDate(szUpdateTime, ref dtUpdateTime);
            return GlobalMethods.SysTime.CompareTime(scriptInfo.MODIFY_TIME, dtUpdateTime);
        }

        /// <summary>
        /// ����ָ��������Ϣ�Ľű�Dll
        /// </summary>
        /// <param name="scriptInfo">������Ϣ</param>
        /// <returns>SystemData.ReturnValue</returns>
        public short DownLoadScript(ScriptConfig scriptInfo)
        {
            if (scriptInfo == null)
                return SystemData.ReturnValue.FAILED;
            byte[] byteScriptData = null;
            short result =ScriptDataAccess.Instance.GetScriptData(scriptInfo.SCRIPT_ID, ref byteScriptData);
            if (result != SystemData.ReturnValue.OK)
                return SystemData.ReturnValue.FAILED;

            result = this.SaveScriptConfig(scriptInfo);
            if (result != SystemData.ReturnValue.OK)
                return SystemData.ReturnValue.FAILED;
            string fileName = this.GetScriptCachePath(scriptInfo.SCRIPT_ID);
            return GlobalMethods.IO.WriteFileBytes(fileName, byteScriptData) ?
                SystemData.ReturnValue.OK : SystemData.ReturnValue.FAILED;
        }

        /// <summary>
        /// ����ű�������Ϣ������XML�ļ�
        /// </summary>
        /// <param name="scriptInfo">�ű�������Ϣ</param>
        /// <returns>SystemData.ReturnValue</returns>
        private short SaveScriptConfig(ScriptConfig scriptInfo)
        {
            if (scriptInfo == null)
                return SystemData.ReturnValue.FAILED;
            string szConfigXml = this.GetDllCacheIndexFilePath();
            if (GlobalMethods.Misc.IsEmptyString(szConfigXml))
            {
                LogManager.Instance.WriteLog("ScriptCache.SaveScriptConfig", "�ļ�����Ϊ�գ�");
                return SystemData.ReturnValue.FAILED;
            }

            //1 ��ȡXML�ļ�����Ϊ�գ��򴴽�
            XmlDocument xmlDoc = GlobalMethods.Xml.GetXmlDocument(szConfigXml);
            if (xmlDoc == null)
                xmlDoc = GlobalMethods.Xml.CreateXmlDocument("ScriptConfig");
            if (xmlDoc == null)
                return SystemData.ReturnValue.FAILED;

            //2 ��ȡָ���ڵ㣬��Ϊ�գ��򴴽�
            string szDllXPath = string.Format("/ScriptConfig/Dll[@Name=\"{0}\"]", scriptInfo.SCRIPT_ID);
            XmlNode node = GlobalMethods.Xml.SelectXmlNode(xmlDoc, szDllXPath);
            if (node == null)
            {
                node = GlobalMethods.Xml.CreateXmlNode(xmlDoc, null, "Dll", null);
            }
            if (node == null)
                return SystemData.ReturnValue.FAILED;

            //3 ��������ֵ
            if (!GlobalMethods.Xml.SetXmlAttrValue(node, "Name", scriptInfo.SCRIPT_ID))
                return SystemData.ReturnValue.FAILED;
            string szUpdateTime = scriptInfo.MODIFY_TIME.ToString();
            if (!GlobalMethods.Xml.SetXmlAttrValue(node, "UpdateTime", szUpdateTime))
                return SystemData.ReturnValue.FAILED;

            //4 ����
            bool isSuccess = GlobalMethods.Xml.SaveXmlDocument(xmlDoc, szConfigXml);
            return isSuccess ? SystemData.ReturnValue.OK : SystemData.ReturnValue.FAILED;
        }
    }
}
