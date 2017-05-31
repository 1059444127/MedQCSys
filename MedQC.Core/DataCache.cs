// *****************************************************************************
// ȫ�����ݻ�����.
// Creator:YangMingkun  Date:2009-11-15
// Copyright:supconhealth
// *****************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Heren.Common.Libraries;
using EMRDBLib;
using EMRDBLib.DbAccess;
using System.Linq;
namespace Heren.MedQC.Core
{
    public class DataCache
    {
        private static DataCache m_Instance = null;
        /// <summary>
        /// ��ȡDataCache����ʵ��
        /// </summary>
        public static DataCache Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new DataCache();
                return m_Instance;
            }
        }
        private Dictionary<string, string> m_dicHdpParameter = null;
        public Dictionary<string, string> DicHdpParameter
        {
            get
            {
                if (m_dicHdpParameter == null)
                {
                    m_dicHdpParameter = new Dictionary<string, string>();
                    List<HdpParameter> lstHdpParameter=null;
                    short shRet = HdpParameterAccess.Instance.GetHdpParameters(null, null, null, ref lstHdpParameter);
                    if (lstHdpParameter != null)
                    {
                        foreach (var item in lstHdpParameter)
                        {
                            if (m_dicHdpParameter.ContainsKey(item.CONFIG_NAME))
                                continue;
                            m_dicHdpParameter.Add(item.CONFIG_NAME, item.CONFIG_VALUE);
                        }
                    }
                }
                return m_dicHdpParameter;
            }
            set {
                this.m_dicHdpParameter = value;
            }
        }
        /// <summary>
        /// ��ǰϵͳ��Ʒ
        /// </summary>
        public HdpProduct HdpProduct { get; set; }

        private string m_QcAdminDepts = null;
        public string QcAdminDepts
        {
            get
            {
                if (this.m_QcAdminDepts == null)
                {
                    List<QcAdminDepts> lstQcAdminDeptsList = new List<QcAdminDepts>();
                    if (SystemParam.Instance.UserInfo == null)
                        return string.Empty;
                    short shRet = QcAdminDeptsAccess.Instance.GetQcAdminDeptsList(SystemParam.Instance.UserInfo.ID, ref lstQcAdminDeptsList);
                    if (lstQcAdminDeptsList.Count > 0)
                    {
                        m_QcAdminDepts = string.Join(",", lstQcAdminDeptsList.Select(m => m.DEPT_NAME).ToArray());
                    }
                    else
                        m_QcAdminDepts = string.Empty;
                }
                return m_QcAdminDepts;
            }
            set
            {
                this.m_QcAdminDepts = value;
            }
        }
        private DataCache()
        {

        }

        private Hashtable m_htDocTypeInfos = null;
        private Hashtable m_htDeptInfos = null;

        /// <summary>
        /// �����ĵ�����ID��ȡָ�����ĵ�������Ϣ
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <param name="docTypeInfo">�ĵ�������Ϣ</param>
        /// <returns>MedDocSys.Common.SystemData.ReturnValue</returns>
        public short GetDocTypeInfo(string szDocTypeID, ref DocTypeInfo docTypeInfo)
        {
            if (GlobalMethods.Misc.IsEmptyString(szDocTypeID))
                return SystemData.ReturnValue.FAILED;
            if (this.m_htDocTypeInfos == null)
                this.m_htDocTypeInfos = new Hashtable();
            if (this.m_htDocTypeInfos.Count > 0)
            {
                docTypeInfo = this.m_htDocTypeInfos[szDocTypeID] as DocTypeInfo;
                return SystemData.ReturnValue.OK;
            }

            List<DocTypeInfo> lstDocTypeInfos = null;

            short shRet = EMRDBLib.DbAccess.DocTypeAccess.Instance.GetDocTypeInfos(ref lstDocTypeInfos);
            if (shRet != SystemData.ReturnValue.OK || lstDocTypeInfos == null)
                return shRet;

            for (int index = 0; index < lstDocTypeInfos.Count; index++)
            {
                DocTypeInfo docTypeInfoItem = lstDocTypeInfos[index];
                if (this.m_htDocTypeInfos.Contains(docTypeInfoItem.DocTypeID))
                    continue;
                this.m_htDocTypeInfos.Add(docTypeInfoItem.DocTypeID, docTypeInfoItem);
            }

            docTypeInfo = this.m_htDocTypeInfos[szDocTypeID] as DocTypeInfo;
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ��ȡ�������Ӧ�Ŀ�����Ϣ
        /// </summary>
        /// <param name="szDeptCode">������</param>
        /// <param name="deptInfo">������Ϣ</param>
        /// <returns>MedDocSys.Common.SystemData.ReturnValue</returns>
        public short GetDeptInfo(string szDeptCode, ref DeptInfo deptInfo)
        {
            if (szDeptCode == null || szDeptCode.Trim() == string.Empty)
                return SystemData.ReturnValue.OK;
            if (this.m_htDeptInfos == null) this.m_htDeptInfos = new Hashtable();
            if (this.m_htDeptInfos.ContainsKey(szDeptCode))
            {
                deptInfo = this.m_htDeptInfos[szDeptCode] as DeptInfo;
                return SystemData.ReturnValue.OK;
            }
            List<DeptInfo> lstDeptInfos = null;
            short shRet = EMRDBLib.DbAccess.EMRDBAccess.Instance.GetClinicDeptList(ref lstDeptInfos);
            if (shRet != SystemData.ReturnValue.OK)
                return shRet;
            if (lstDeptInfos == null || lstDeptInfos.Count <= 0)
                return SystemData.ReturnValue.OK;
            for (int index = 0; index < lstDeptInfos.Count; index++)
            {
                deptInfo = lstDeptInfos[index];
                if (!this.m_htDeptInfos.ContainsKey(deptInfo.DEPT_CODE))
                    this.m_htDeptInfos.Add(deptInfo.DEPT_CODE, deptInfo);
            }
            deptInfo = this.m_htDeptInfos[szDeptCode] as DeptInfo;
            return SystemData.ReturnValue.OK;
        }

    }
}
