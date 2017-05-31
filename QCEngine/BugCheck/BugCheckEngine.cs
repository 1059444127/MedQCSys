// ***********************************************************
// �����ĵ�ϵͳ�ĵ�����ȱ���ʿ�����
// Creator:YangMingkun  Date:2009-8-18
// Copyright:supconhealth
// ***********************************************************
using EMRDBLib;
using EMRDBLib.DbAccess;
using System;
using System.Collections.Generic;

namespace MedDocSys.QCEngine.BugCheck
{
    public class BugCheckEngine : IDisposable
    {
        #region"���Զ���"
        private UserInfo m_userInfo = null;
        /// <summary>
        /// ��ȡ�������û���Ϣ
        /// </summary>
        public UserInfo UserInfo
        {
            get { return this.m_userInfo; }
            set { this.m_userInfo = value; }
        }

        private PatientInfo m_patientInfo = null;
        /// <summary>
        /// ��ȡ�����ò�����Ϣ
        /// </summary>
        public PatientInfo PatientInfo
        {
            get { return this.m_patientInfo; }
            set { this.m_patientInfo = value; }
        }

        private VisitInfo m_visitInfo = null;
        /// <summary>
        /// ��ȡ�����þ�����Ϣ
        /// </summary>
        public VisitInfo VisitInfo
        {
            get { return this.m_visitInfo; }
            set { this.m_visitInfo = value; }
        }

        private string m_szDocType = string.Empty;
        /// <summary>
        /// ��ȡ�������ĵ�����
        /// </summary>
        public string DocType
        {
            get { return this.m_szDocType; }
            set { this.m_szDocType = value; }
        }

        private string m_szDocText = string.Empty;
        /// <summary>
        /// ��ȡ�������ĵ����ı�����
        /// </summary>
        public string DocText
        {
            get { return this.m_szDocText; }
            set { this.m_szDocText = value; }
        }

        private List<OutlineInfo> m_lstOutlineInfos = null;
        /// <summary>
        /// ��ȡ�������ĵ�Section�б�
        /// </summary>
        public List<OutlineInfo> OutlineInfos
        {
            get { return this.m_lstOutlineInfos; }
            set { this.m_lstOutlineInfos = value; }
        }
        #endregion

        /// <summary>
        /// ��ʼ���ʿ�����
        /// </summary>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short InitializeEngine()
        {
            if (BugCheckRuleManager.RuleTable != null && BugCheckRuleManager.RuleTable.Count > 0)
                return SystemData.ReturnValue.OK;

            //��ʼ������Entry�б�
            List<MedQCEntry> lstQCEntryList = null;
            short shRet =MedQCEntryAccess.Instance.GetQCEntryList(ref lstQCEntryList);
            if (shRet != SystemData.ReturnValue.OK)
                return SystemData.ReturnValue.FAILED;
            if (lstQCEntryList == null || lstQCEntryList.Count <= 0)
                return SystemData.ReturnValue.CANCEL;

            for (int index = 0; index < lstQCEntryList.Count; index++)
            {
                BugCheckEntryManager.AddEntry(this, lstQCEntryList[index]);
            }

            //��ʼ�������б�
            List<MedQCRule> lstQCRuleList = null;
            shRet =MedQCRuleAccess.Instance.GetQCRuleList(ref lstQCRuleList);
            if (shRet != SystemData.ReturnValue.OK)
                return SystemData.ReturnValue.FAILED;
            if (lstQCRuleList == null || lstQCRuleList.Count <= 0)
                return SystemData.ReturnValue.CANCEL;

            for (int index = 0; index < lstQCRuleList.Count; index++)
            {
                BugCheckRuleManager.AddRule(lstQCRuleList[index]);
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ִ���ĵ�ȱ�ݼ��
        /// </summary>
        /// <param name="lstBugList">�ĵ�ȱ���б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public List<DocuemntBugInfo> PerformBugCheck()
        {
            BugCheckEntryManager.ComputeEntryOccurCount();
            return BugCheckRuleManager.ExecuteRule();
        }

        public void Dispose()
        {
            BugCheckEntryManager.Dispose();
            BugCheckRuleManager.Dispose();
        }
    }
}
