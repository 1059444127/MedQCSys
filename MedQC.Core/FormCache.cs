// ***********************************************************
// ������Ӳ���ϵͳ,��������Ϣ���������.
// ��Ҫ����������б�,�Լ���ģ���ļ�,����ظ�����
// Creator:YangMingkun  Date:2012-8-26
// Copyright : Heren Health Services Co.,Ltd.
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Heren.Common.Libraries;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.Core
{
    public class FormCache
    {
        private static FormCache m_instance = null;

        /// <summary>
        /// ��ȡDocTypeCacheʵ��
        /// </summary>
        public static FormCache Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new FormCache();
                return m_instance;
            }
        }

        private FormCache()
        {
            this.m_szCacheIndexFile = GlobalMethods.Misc.GetWorkingPath()
                + @"\Templets\Forms.xml";
        }

        /// <summary>
        /// ģ�屾�ػ��������ļ�·��
        /// </summary>
        private string m_szCacheIndexFile = null;

        /// <summary>
        /// ��ű��ĵ����͵Ĺ�ϣ��
        /// </summary>
        private Dictionary<string, TempletType> m_htDocTypeTable = null;

        /// <summary>
        /// ������ű��ĵ����͵Ĺ�ϣ��
        /// </summary>
        private Dictionary<string, List<TempletType>> m_htDocClassTable = null;

        ///// <summary>
        ///// �ж��ĵ�������������ʱʹ�õ�Hash��
        ///// </summary>
        //private Dictionary<string, WardDocType> m_htWardDocTable = null;

        /// <summary>
        /// ���ز��������б�
        /// </summary>
        /// <param name="szApplyEnv">Ӧ�û���</param>
        /// <returns>�ĵ�������Ϣ�б�</returns>
        public List<TempletType> GetDocTypeList(string szApplyEnv)
        {
            if (GlobalMethods.Misc.IsEmptyString(szApplyEnv))
                return null;
            if (this.m_htDocClassTable != null
                && this.m_htDocClassTable.ContainsKey(szApplyEnv))
            {
                return this.m_htDocClassTable[szApplyEnv];
            }

            List<TempletType> lstDocTypeInfos = null;
            short shRet = TempletTypeAccess.Instance.GetTempletTypes(szApplyEnv, ref lstDocTypeInfos);
            if (shRet != SystemData.ReturnValue.OK)
                return null;

            if (lstDocTypeInfos == null || lstDocTypeInfos.Count <= 0)
                return new List<TempletType>();

            if (this.m_htDocClassTable == null)
                this.m_htDocClassTable = new Dictionary<string, List<TempletType>>();
            this.m_htDocClassTable.Add(szApplyEnv, lstDocTypeInfos);

            if (this.m_htDocTypeTable == null)
                this.m_htDocTypeTable = new Dictionary<string, TempletType>();

            for (int index = 0; index < lstDocTypeInfos.Count; index++)
            {
                TempletType docTypeInfo = lstDocTypeInfos[index];
                if (docTypeInfo == null || string.IsNullOrEmpty(docTypeInfo.DocTypeID))
                    continue;
                if (!this.m_htDocTypeTable.ContainsKey(docTypeInfo.DocTypeID))
                    this.m_htDocTypeTable.Add(docTypeInfo.DocTypeID, docTypeInfo);
            }
            return lstDocTypeInfos;
        }

        /// <summary>
        /// ��ȡָ��ID���ĵ�������Ϣ
        /// </summary>
        /// <param name="szDocTypeID">�ĵ����ʹ���</param>
        /// <returns>�ĵ�������Ϣ</returns>
        public TempletType GetDocTypeInfo(string szDocTypeID)
        {
            if (GlobalMethods.Misc.IsEmptyString(szDocTypeID))
                return null;

            //������Ӳ�������
            if (this.m_htDocTypeTable == null)
                this.m_htDocTypeTable = new Dictionary<string, TempletType>();
            if (this.m_htDocTypeTable.ContainsKey(szDocTypeID))
                return this.m_htDocTypeTable[szDocTypeID];

            //���²�ѯ��ȡ�ĵ�������Ϣ
            TempletType docTypeInfo = null;
            short shRet = TempletTypeAccess.Instance.GetTempletType(szDocTypeID, ref docTypeInfo);
            if (shRet != SystemData.ReturnValue.OK || docTypeInfo == null)
                return null;
            this.m_htDocTypeTable.Add(szDocTypeID, docTypeInfo);
            return docTypeInfo;
        }

        /// <summary>
        /// ��ȡָ��Ӧ�û����µĲ����ĵ�ģ��
        /// </summary>
        /// <param name="szApplyEnv">Ӧ�û���</param>
        /// <returns>�ĵ�ģ����Ϣ</returns>
        public TempletType GetWardDocType(string szApplyEnv)
        {
            List<TempletType> lstDocTypeInfos = this.GetDocTypeList(szApplyEnv);
            if (lstDocTypeInfos == null || lstDocTypeInfos.Count <= 0)
                return null;

            //����ѡȡ���������õı�ģ��
            TempletType hospitalDocTypeInfo = null;
            foreach (TempletType docTypeInfo in lstDocTypeInfos)
            {
                if (docTypeInfo.IsValid && !docTypeInfo.IsFolder)
                {
                    hospitalDocTypeInfo = docTypeInfo;
                    if (this.IsWardDocType(docTypeInfo.DocTypeID))
                        return docTypeInfo;
                }
            }
            return hospitalDocTypeInfo;
        }

        /// <summary>
        /// ��ȡָ��Ӧ�û���ָ��������ָ�����Ƶı�ģ��
        /// </summary>
        /// <param name="szApplyEnv">Ӧ�û���</param>
        /// <param name="szDocTypeName">����������</param>
        /// <returns>��ģ����Ϣ</returns>
        public TempletType GetWardDocType(string szApplyEnv, string szDocTypeName)
        {
            List<TempletType> lstDocTypeInfos = this.GetDocTypeList(szApplyEnv);
            if (lstDocTypeInfos == null || lstDocTypeInfos.Count <= 0)
                return null;

            //����ѡȡ������ָ�����Ƶı�ģ��
            TempletType docTypeInfo1 = null;
            TempletType docTypeInfo2 = null;
            foreach (TempletType docTypeInfo in lstDocTypeInfos)
            {
                if (docTypeInfo.IsValid && !docTypeInfo.IsFolder)
                {
                    if (docTypeInfo.DocTypeName.StartsWith(szDocTypeName))
                        docTypeInfo1 = docTypeInfo;
                    if (this.IsWardDocType(docTypeInfo.DocTypeID))
                        docTypeInfo2 = docTypeInfo;
                }
            }
            if (docTypeInfo2 != null && docTypeInfo2.DocTypeName.StartsWith(szDocTypeName))
                return docTypeInfo2;
            return docTypeInfo1;
        }

        /// <summary>
        /// ���ص�ǰ�����Ĳ��������б�
        /// </summary>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        private bool LoadWardDocTypeList()
        {
            if (SystemParam.Instance.UserInfo == null)
                return false;

            //List<WardDocType> lstWardDocTypes = null;
            //short shRet = DocTypeService.Instance.GetWardDocTypeList(string.Empty, ref lstWardDocTypes);
            //if (shRet != SystemConst.ReturnValue.OK)
            //    return false;

            //if (this.m_htWardDocTable == null)
            //    this.m_htWardDocTable = new Dictionary<string, WardDocType>();
            //this.m_htWardDocTable.Clear();

            //if (lstWardDocTypes == null || lstWardDocTypes.Count <= 0)
            //    return true;

            //string szWardDocType = null;
            //for (int index = 0; index < lstWardDocTypes.Count; index++)
            //{
            //    WardDocType wardDocType = lstWardDocTypes[index];
            //    if (wardDocType == null || string.IsNullOrEmpty(wardDocType.DocTypeID))
            //        continue;

            //    //���������͵ĵ�һ������������Ϣ���뵽hash��
            //    //�Ա�����Ϸ�����жϸò��������Ƿ���������������
            //    if (!this.m_htWardDocTable.ContainsKey(wardDocType.DocTypeID))
            //        this.m_htWardDocTable.Add(wardDocType.DocTypeID, wardDocType);

            //    //ͬʱ�����������벡���Ķ�Ӧ��ϵ���뵽hash��
            //    szWardDocType = string.Concat(wardDocType.DocTypeID, "_", wardDocType.WardCode);
            //    if (!this.m_htWardDocTable.ContainsKey(szWardDocType))
            //        this.m_htWardDocTable.Add(szWardDocType, wardDocType);
            //}
            //return true;
            return false;
        }

        /// <summary>
        /// ��ȡָ�����ĵ������Ƿ��ǵ�ǰ�û�������
        /// </summary>
        /// <param name="szDocTypeID">ָ�����ĵ�����</param>
        /// <returns>�Ƿ��ǵ�ǰ��������</returns>
        public bool IsWardDocType(string szDocTypeID)
        {
            //if (this.m_htWardDocTable == null)
            //{
            //    if (!this.LoadWardDocTypeList())
            //        return false;
            //}
            //if (this.m_htWardDocTable == null || SystemContext.Instance.LoginUser == null)
            //    return true;

            ////�����������δ������������,��ô��Ϊ��ͨ��
            //if (string.IsNullOrEmpty(szDocTypeID) || !this.m_htWardDocTable.ContainsKey(szDocTypeID))
            //    return true;

            ////���ظò������������������Ƿ��е�ǰ�û�����
            //string szWardDocType = string.Concat(szDocTypeID, "_", SystemContext.Instance.LoginUser.WardCode);
            //if (this.m_htWardDocTable.ContainsKey(szWardDocType))
            //    return true;

            ////���ظò������������û������Ƿ��и��û����ڵ�С��
            //return RightController.Instance.CanOpenNurDoc(m_htWardDocTable, szDocTypeID);
            return true;
        }

        /// <summary>
        /// ��ȡָ�����ĵ����Ͷ�Ӧ�ı��Ƿ�������Ϊ�����ӡ
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <returns>�Ƿ������ӡ</returns>
        public bool IsFormPrintable(string szDocTypeID)
        {
            TempletType docTypeInfo = FormCache.Instance.GetDocTypeInfo(szDocTypeID);
            if (docTypeInfo == null)
                return false;
            if (docTypeInfo.PrintMode == FormPrintMode.Form || docTypeInfo.PrintMode == FormPrintMode.FormAndList)
                return true;
            return false;
        }

        /// <summary>
        /// ��ȡָ�����ĵ����Ͷ�Ӧ����д���б��Ƿ�������Ϊ�����ӡ
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <returns>�Ƿ������ӡ</returns>
        public bool IsFormListPrintable(string szDocTypeID)
        {
            TempletType docTypeInfo = FormCache.Instance.GetDocTypeInfo(szDocTypeID);
            if (docTypeInfo == null)
                return false;
            if (docTypeInfo.PrintMode == FormPrintMode.List || docTypeInfo.PrintMode == FormPrintMode.FormAndList)
                return true;
            return false;
        }

        /// <summary>
        /// ��ȡ��ģ�屾�ػ���·��
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <returns>ģ�屾�ػ���·��</returns>
        private string GetFormCachePath(string szDocTypeID)
        {
            return string.Format(@"{0}\Templets\Forms\{1}.hndt", GlobalMethods.Misc.GetWorkingPath(), szDocTypeID);
        }

        /// <summary>
        /// ��ȡ�ĵ�ģ����޸�ʱ��
        /// </summary>
        /// <param name="szDocTypeID">������ID(����Ǳ�ģ��)</param>
        /// <returns>DateTime</returns>
        public DateTime GetFormModifyTime(string szDocTypeID)
        {
            if (!System.IO.File.Exists(this.m_szCacheIndexFile))
                return DateTime.Now;

            XmlDocument xmlDoc = GlobalMethods.Xml.GetXmlDocument(this.m_szCacheIndexFile);
            if (xmlDoc == null)
                return DateTime.Now;

            string szXPath = null;
            if (!GlobalMethods.Misc.IsEmptyString(szDocTypeID))
                szXPath = string.Format("/Forms/Templet[@ID='{0}']", szDocTypeID.Trim());
            else
                return DateTime.Now;

            XmlNode templetNode = GlobalMethods.Xml.SelectXmlNode(xmlDoc, szXPath);
            if (templetNode == null)
                return DateTime.Now;

            string szModifyTime = null;
            if (!GlobalMethods.Xml.GetXmlNodeValue(templetNode, "./@ModifyTime", ref szModifyTime))
                return DateTime.Now;

            DateTime dtModifyTime = DateTime.Now;
            GlobalMethods.Convert.StringToDate(szModifyTime, ref dtModifyTime);
            return dtModifyTime;
        }

        /// <summary>
        /// ��ȡָ�������Ͷ�Ӧ�ı�ģ������
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <param name="byteTempletData">���ص�ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public bool GetFormTemplet(string szDocTypeID, ref byte[] byteTempletData)
        {
            TempletType docTypeInfo = this.GetDocTypeInfo(szDocTypeID);
            if (docTypeInfo == null)
            {
                LogManager.Instance.WriteLog("FormCache.GetFormTemplet"
                     , new string[] { "szDocTypeID" }, new object[] { szDocTypeID }, "��������!");
                return false;
            }
            return this.GetFormTemplet(docTypeInfo, ref byteTempletData);
        }

        /// <summary>
        /// ��ȡָ�������Ͷ�Ӧ�ı�ģ������
        /// </summary>
        /// <param name="docTypeInfo">�ĵ�������Ϣ</param>
        /// <param name="byteTempletData">���ص�ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public bool GetFormTemplet(TempletType docTypeInfo, ref byte[] byteTempletData)
        {
            if (docTypeInfo == null || GlobalMethods.Misc.IsEmptyString(docTypeInfo.DocTypeID))
                return false;

            string szTempletPath = this.GetFormCachePath(docTypeInfo.DocTypeID);

            // ��������Ѿ���ģ�建��,��ô���ر��ص�
            DateTime dtModifyTime = this.GetFormModifyTime(docTypeInfo.DocTypeID);
            if (dtModifyTime.CompareTo(docTypeInfo.ModifyTime) == 0)
            {
                if (GlobalMethods.IO.GetFileBytes(szTempletPath, ref byteTempletData))
                    return true;
            }

            // ��������Ŀ¼
            string szParentDir = GlobalMethods.IO.GetFilePath(szTempletPath);
            if (!GlobalMethods.IO.CreateDirectory(szParentDir))
            {
                LogManager.Instance.WriteLog("FormCache.GetFormTemplet"
                    , new string[] { "docTypeInfo" }, new object[] { docTypeInfo }, "��ģ�建��Ŀ¼����ʧ��!", null);
                return false;
            }

            // �����ĵ�ģ������
            short shRet = TempletTypeAccess.Instance.GetTempletData(docTypeInfo.DocTypeID, ref byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("FormCache.GetFormTemplet"
                    , new string[] { "docTypeInfo" }, new object[] { docTypeInfo }, "��ģ������ʧ��!", null);
                return false;
            }

            // д�û�ģ�屾��������Ϣ
            if (!this.CacheFormTemplet(docTypeInfo, byteTempletData))
            {
                LogManager.Instance.WriteLog("FormCache.GetFormTemplet"
                    , new string[] { "docTypeInfo" }, new object[] { docTypeInfo }, "��ģ�建�浽����ʧ��!", null);
            }
            return true;
        }

        /// <summary>
        /// ��ָ����ϵͳģ�����ݻ��浽����
        /// </summary>
        /// <param name="docTypeInfo">�ĵ�������Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        private bool CacheFormTemplet(TempletType docTypeInfo, byte[] byteTempletData)
        {
            if (docTypeInfo == null || byteTempletData == null || byteTempletData.Length <= 0)
                return false;

            //����ϵͳģ�����ݵ�����
            string szTempletPath = this.GetFormCachePath(docTypeInfo.DocTypeID);
            if (!GlobalMethods.IO.WriteFileBytes(szTempletPath, byteTempletData))
            {
                LogManager.Instance.WriteLog("FormCache.CacheFormTemplet"
                    , new string[] { "docTypeInfo", "szTempletPath" }, new object[] { docTypeInfo, szTempletPath }
                    , "��ģ�����ݻ��浽����ʧ��!", null);
                return false;
            }

            //װ��ģ�屾�������ļ�
            if (!System.IO.File.Exists(this.m_szCacheIndexFile))
            {
                if (!GlobalMethods.Xml.CreateXmlFile(this.m_szCacheIndexFile, "Forms"))
                    return false;
            }
            XmlDocument xmlDoc = GlobalMethods.Xml.GetXmlDocument(this.m_szCacheIndexFile);
            if (xmlDoc == null)
            {
                return false;
            }

            // ��ӻ����ģ�屾�������ڵ�
            string szXPath = string.Format("/Forms/Templet[@ID='{0}']", docTypeInfo.DocTypeID);
            XmlNode templetXmlNode = null;
            try
            {
                templetXmlNode = GlobalMethods.Xml.SelectXmlNode(xmlDoc, szXPath);
                if (templetXmlNode == null)
                {
                    templetXmlNode = GlobalMethods.Xml.CreateXmlNode(xmlDoc, null, "Templet", null);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("FormCache.CacheFormTemplet"
                    , new string[] { "docTypeInfo" }, new object[] { docTypeInfo }, "��ģ�屾��������Ϣд��ʧ��!", ex);
                return false;
            }
            if (templetXmlNode == null)
            {
                LogManager.Instance.WriteLog("FormCache.CacheFormTemplet"
                    , new string[] { "docTypeInfo", "szXPath" }, new object[] { docTypeInfo, szXPath }
                    , "��ģ�屾��������Ϣд��ʧ��!�޷������ڵ�!", null);
                return false;
            }

            //����ģ�������ڵ������ֵ
            if (!GlobalMethods.Xml.SetXmlAttrValue(templetXmlNode, "ID", docTypeInfo.DocTypeID))
                return false;
            if (!GlobalMethods.Xml.SetXmlAttrValue(templetXmlNode, "Name", docTypeInfo.DocTypeName))
                return false;
            if (!GlobalMethods.Xml.SetXmlAttrValue(templetXmlNode, "ModifyTime", docTypeInfo.ModifyTime.ToString()))
                return false;
            if (!GlobalMethods.Xml.SaveXmlDocument(xmlDoc, this.m_szCacheIndexFile))
                return false;
            return true;
        }
    }
}
