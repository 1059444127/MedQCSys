// ***********************************************************
// ������Ӳ���ϵͳ,����������Ϣ���������.
// ��Ҫ���汨�������б�,�Լ�����ģ���ļ�,����ظ�����
// Creator:YangMingkun  Date:2012-8-26
// Copyright : Heren Health Services Co.,Ltd.
// ***********************************************************
using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Heren.Common.Libraries;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace Heren.MedQC.Core
{
    public class ReportCache
    {
        private static ReportCache m_instance = null;

        /// <summary>
        /// ��ȡReportCacheʵ��
        /// </summary>
        public static ReportCache Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ReportCache();
                return m_instance;

            }
        }

        private ReportCache()
        {
            this.m_szCacheIndexFile = GlobalMethods.Misc.GetWorkingPath()
                + @"\Templets\Reports.xml";
        }

        /// <summary>
        /// ģ�屾�ػ��������ļ�·��
        /// </summary>
        private string m_szCacheIndexFile = null;

        /// <summary>
        /// ��ű������͵Ĺ�ϣ��
        /// </summary>
        private Dictionary<string, ReportType> m_htReportTypeTable = null;

        /// <summary>
        /// ������ű������͵Ĺ�ϣ��
        /// </summary>
        private Dictionary<string, List<ReportType>> m_htReportClassTable = null;


        /// <summary>
        /// ���ز��������б�
        /// </summary>
        /// <param name="szApplyEnv">Ӧ�û���</param>
        /// <returns>����������Ϣ�б�</returns>
        public List<ReportType> GetReportTypeList(string szApplyEnv)
        {
            if (GlobalMethods.Misc.IsEmptyString(szApplyEnv))
                return null;

            List<ReportType> lstReportTypes = null;
            short shRet = ReportTypeAccess.Instance.GetReportTypes(szApplyEnv, ref lstReportTypes);
            if (shRet != SystemData.ReturnValue.OK)
                return null;
            return lstReportTypes;
            if (this.m_htReportClassTable != null
                && this.m_htReportClassTable.ContainsKey(szApplyEnv))
            {
                return this.m_htReportClassTable[szApplyEnv];
            }


            if (lstReportTypes == null || lstReportTypes.Count <= 0)
                return new List<ReportType>();

            if (this.m_htReportClassTable == null)
                this.m_htReportClassTable = new Dictionary<string, List<ReportType>>();
            this.m_htReportClassTable.Add(szApplyEnv, lstReportTypes);

            if (this.m_htReportTypeTable == null)
                this.m_htReportTypeTable = new Dictionary<string, ReportType>();

            for (int index = 0; index < lstReportTypes.Count; index++)
            {
                ReportType ReportType = lstReportTypes[index];
                if (ReportType == null || string.IsNullOrEmpty(ReportType.ReportTypeID))
                    continue;
                if (!this.m_htReportTypeTable.ContainsKey(ReportType.ReportTypeID))
                    this.m_htReportTypeTable.Add(ReportType.ReportTypeID, ReportType);
            }
            return lstReportTypes;
        }

        /// <summary>
        /// ��ȡָ��ID�ı���������Ϣ
        /// </summary>
        /// <param name="szReportTypeID">�������ʹ���</param>
        /// <returns>����������Ϣ</returns>
        public ReportType GetReportType(string szReportTypeID)
        {
            if (GlobalMethods.Misc.IsEmptyString(szReportTypeID))
                return null;
            //���²�ѯ��ȡ����������Ϣ
            ReportType ReportType = null;
            short shRet = ReportTypeAccess.Instance.GetReportType(szReportTypeID, ref ReportType);
            if (shRet != SystemData.ReturnValue.OK || ReportType == null)
                return null;
            //������Ӳ�������
            if (this.m_htReportTypeTable == null)
                this.m_htReportTypeTable = new Dictionary<string, ReportType>();
            if (this.m_htReportTypeTable.ContainsKey(szReportTypeID))
            {
                if (m_htReportTypeTable[szReportTypeID].ModifyTime == ReportType.ModifyTime)
                    return this.m_htReportTypeTable[szReportTypeID];
                else
                {
                    this.m_htReportTypeTable[szReportTypeID] = ReportType;
                }
            }
            else
                this.m_htReportTypeTable.Add(szReportTypeID, ReportType);
            return ReportType;
        }

        /// <summary>
        /// ��ȡָ��Ӧ�û����µĲ�������ģ��
        /// </summary>
        /// <param name="szApplyEnv">Ӧ�û���</param>
        /// <returns>����ģ����Ϣ</returns>
        public ReportType GetWardReportType(string szApplyEnv)
        {
            List<ReportType> lstReportTypes = this.GetReportTypeList(szApplyEnv);
            if (lstReportTypes == null || lstReportTypes.Count <= 0)
                return null;

            //����ѡȡ���������õı���ģ��
            ReportType hospitalReportType = null;
            ReportType WardReportType = null;
            foreach (ReportType ReportType in lstReportTypes)
            {
                if (ReportType.IsValid && !ReportType.IsFolder)
                {
                    if (this.IshospitalReportType(ReportType.ReportTypeID))
                    {
                        if (hospitalReportType == null)
                            hospitalReportType = ReportType;
                        else
                            continue;
                    }
                    //else if (this.IsWardReportType(ReportType.ReportTypeID))
                    //    WardReportType = ReportType;
                }
            }
            if (WardReportType == null)
                return hospitalReportType;
            else
                return WardReportType;
        }

        /// <summary>
        /// ��ȡָ��Ӧ�û���ָ��������ָ�����Ƶı���ģ��
        /// </summary>
        /// <param name="szApplyEnv">Ӧ�û���</param>
        /// <param name="szReportName">������������</param>
        /// <returns>����ģ����Ϣ</returns>
        public ReportType GetWardReportType(string szApplyEnv, string szReportName)
        {
            List<ReportType> lstReportTypes = this.GetReportTypeList(szApplyEnv);
            if (lstReportTypes == null)
                return null;

            //����ѡȡ������ָ�����Ƶı���ģ��
            //1��ǰ������������ȫƥ��
            //2��ǰ����������ǰ����ƥ��
            //3ȫԺ��������ȫƥ��
            //4ȫԺ������ǰ����ƥ��
            ReportType ReportType1 = null;
            ReportType ReportType2 = null;
            ReportType ReportType3 = null;
            ReportType ReportType4 = null;
            foreach (ReportType ReportType in lstReportTypes)
            {
                if (ReportType.IsValid && !ReportType.IsFolder)
                {
                    if (ReportType.ReportTypeName.StartsWith(szReportName)
                        && this.IshospitalReportType(ReportType.ReportTypeID))
                    {
                        if (ReportType.ReportTypeName == szReportName)
                            ReportType3 = ReportType;
                        if (ReportType4 == null)
                            ReportType4 = ReportType;
                    }
                    else if (ReportType.ReportTypeName.StartsWith(szReportName)
                        //&& this.IsWardReportType(ReportType.ReportTypeID)
                        )
                    {
                        if (ReportType.ReportTypeName == szReportName)
                            ReportType1 = ReportType;
                        if (ReportType2 == null)
                            ReportType2 = ReportType;
                    }
                }
            }
            if (ReportType1 != null)
                return ReportType1;
            else if (ReportType2 != null)
                return ReportType2;
            else if (ReportType3 != null)
                return ReportType3;
            else
                return ReportType4;
        }


        /// <summary>
        /// ��ȡָ���ı��������Ƿ���ȫԺ��
        /// </summary>
        /// <param name="szDocTypeID">ָ���ı�������</param>
        /// <returns>�Ƿ���ȫԺ�Ĳ���</returns>
        public bool IshospitalReportType(string szDocTypeID)
        {
            //if (this.m_htWardReportTable == null)
            //{
            //    if (!this.LoadWardReportTypeList())
            //        return false;
            //}
            //if (this.m_htWardReportTable == null || SystemContext.Instance.LoginUser == null)
            //    return true;
            return true;
            //�����������δ������������,��ô��Ϊ��ͨ��
            //if (string.IsNullOrEmpty(szDocTypeID) || !this.m_htWardReportTable.ContainsKey(szDocTypeID))
            //    return true;
            //return false;
        }

        /// <summary>
        /// ��ȡ����ģ�屾�ػ���·��
        /// </summary>
        /// <param name="szTempletID">ģ��ID</param>
        /// <returns>ģ�屾�ػ���·��</returns>
        private string GetReportCachePath(string szTempletID)
        {
            return string.Format(@"{0}\Templets\Reports\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), szTempletID);
        }

        /// <summary>
        /// ��ȡ����ģ����޸�ʱ��
        /// </summary>
        /// <param name="szTempletID">����ģ��ID(����Ǳ���ģ��)</param>
        /// <returns>DateTime</returns>
        public DateTime GetReportModifyTime(string szTempletID)
        {
            if (!System.IO.File.Exists(this.m_szCacheIndexFile))
                return DateTime.Now;

            XmlDocument xmlDoc = GlobalMethods.Xml.GetXmlDocument(this.m_szCacheIndexFile);
            if (xmlDoc == null)
                return DateTime.Now;

            string szXPath = null;
            if (!GlobalMethods.Misc.IsEmptyString(szTempletID))
                szXPath = string.Format("/Reports/Templet[@ID='{0}']", szTempletID.Trim());
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
        /// ���ݱ���ģ��ID,��ȡ����ģ������
        /// </summary>
        /// <param name="szReportTypeID">����ģ��ID</param>
        /// <param name="byteTempletData">���ص�ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public bool GetReportTemplet(string szReportTypeID, ref byte[] byteTempletData)
        {
            ReportType ReportType = this.GetReportType(szReportTypeID);
            if (ReportType == null)
            {
                LogManager.Instance.WriteLog("FormCache.GetReportTemplet"
                     , new string[] { "szReportTypeID" }, new object[] { szReportTypeID }, "��������!");
                return false;
            }
            return this.GetReportTemplet(ReportType, ref byteTempletData);
        }

        /// <summary>
        /// ���ݱ���ģ����Ϣ,��ȡ����ģ������
        /// </summary>
        /// <param name="ReportType">����ģ����Ϣ</param>
        /// <param name="byteTempletData">����ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public bool GetReportTemplet(ReportType ReportType, ref byte[] byteTempletData)
        {
            if (ReportType == null || GlobalMethods.Misc.IsEmptyString(ReportType.ReportTypeID))
                return false;

            string szTempletPath = this.GetReportCachePath(ReportType.ReportTypeID);

            // ��������Ѿ���ģ�建��,��ô���ر��ص�
            DateTime dtModifyTime = this.GetReportModifyTime(ReportType.ReportTypeID);
            if (dtModifyTime.CompareTo(ReportType.ModifyTime) == 0)
            {
                if (GlobalMethods.IO.GetFileBytes(szTempletPath, ref byteTempletData))
                    return true;
            }

            // ��������Ŀ¼
            string szParentDir = GlobalMethods.IO.GetFilePath(szTempletPath);
            if (!GlobalMethods.IO.CreateDirectory(szParentDir))
            {
                LogManager.Instance.WriteLog("ReportCache.GetReportTemplet"
                    , new string[] { "templetInfo" }, new object[] { ReportType }, "����ģ�建��Ŀ¼����ʧ��!", null);
                return false;
            }

            // ���ر���ģ������
            short shRet = ReportTypeAccess.Instance.GetReportData(ReportType.ReportTypeID, ref byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("ReportCache.GetReportTemplet"
                    , new string[] { "templetInfo" }, new object[] { ReportType }, "����ģ������ʧ��!", null);
                return false;
            }

            // д�û�ģ�屾��������Ϣ
            if (!this.CacheReportTemplet(ReportType, byteTempletData))
            {
                LogManager.Instance.WriteLog("ReportCache.GetReportTemplet"
                    , new string[] { "templetInfo" }, new object[] { ReportType }, "����ģ�建�浽����ʧ��!", null);
            }
            return true;
        }

        /// <summary>
        /// ��ָ�����û�ģ�����ݻ��浽����
        /// </summary>
        /// <param name="ReportType">ģ����Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        private bool CacheReportTemplet(ReportType ReportType, byte[] byteTempletData)
        {
            if (ReportType == null || byteTempletData == null || byteTempletData.Length <= 0)
                return false;

            //����ģ���ļ����ݵ�����
            string szTempletPath = this.GetReportCachePath(ReportType.ReportTypeID);
            if (!GlobalMethods.IO.WriteFileBytes(szTempletPath, byteTempletData))
            {
                LogManager.Instance.WriteLog("ReportCache.CacheReportTemplet"
                    , new string[] { "ReportType", "szTempletPath" }, new object[] { ReportType, szTempletPath }
                    , "����ģ�����ݻ��浽����ʧ��!", null);
                return false;
            }

            //װ��ģ�屾�������ļ�
            if (!System.IO.File.Exists(this.m_szCacheIndexFile))
            {
                if (!GlobalMethods.Xml.CreateXmlFile(this.m_szCacheIndexFile, "Reports"))
                    return false;
            }
            XmlDocument xmlDoc = GlobalMethods.Xml.GetXmlDocument(this.m_szCacheIndexFile);
            if (xmlDoc == null)
            {
                return false;
            }

            //��ӻ���µ�ǰģ�������ڵ�
            string szXPath = string.Format("/Reports/Templet[@ID='{0}']", ReportType.ReportTypeID);
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
                LogManager.Instance.WriteLog("ReportCache.CacheReportTemplet"
                    , new string[] { "ReportType" }, new string[] { ReportType.ToString() }
                    , "����ģ�屾��������Ϣд��ʧ��!", ex);
                return false;
            }
            if (templetXmlNode == null)
            {
                LogManager.Instance.WriteLog("ReportCache.CacheReportTemplet"
                    , new string[] { "docTypeInfo", "szXPath" }, new string[] { ReportType.ToString(), szXPath }
                    , "����ģ�屾��������Ϣд��ʧ��!�޷������ڵ�!", null);
                return false;
            }

            //����ģ�屾�������ڵ������ֵ
            if (!GlobalMethods.Xml.SetXmlAttrValue(templetXmlNode, "ID", ReportType.ReportTypeID.ToString()))
                return false;
            if (!GlobalMethods.Xml.SetXmlAttrValue(templetXmlNode, "Name", ReportType.ReportTypeName))
                return false;
            if (!GlobalMethods.Xml.SetXmlAttrValue(templetXmlNode, "ModifyTime", ReportType.ModifyTime.ToString()))
                return false;
            if (!GlobalMethods.Xml.SaveXmlDocument(xmlDoc, this.m_szCacheIndexFile))
                return false;
            return true;
        }
    }
}
