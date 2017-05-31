// ***********************************************************
// �����ĵ�ϵͳ����ʱЧ�������
// Creator:Tangcheng  Date:2011-12-21
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Collections;
using Heren.Common.Libraries;
using EMRDBLib.Entity;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace MedDocSys.QCEngine.TimeCheck
{
    public class TimeCheckEngine : IDisposable
    {
        private List<TimeQCRule> m_lstTimeQCRules = null;

        private string m_szPatientID = string.Empty;
        private string m_szVisitID = string.Empty;

        private List<TimeCheckResult> m_lstTimeCheckResults = null;

        /// <summary>
        /// ��ȡʱЧ�������Ϣ�б�
        /// </summary>
        public List<TimeCheckResult> TimeCheckResults
        {
            get { return this.m_lstTimeCheckResults; }
        }

        private int m_nEarlyCount = 0;
        /// <summary>
        /// ��ȡ��������ǰ��д��������֮��
        /// </summary>
        public int EarlyCount
        {
            get { return this.m_nEarlyCount; }
            set { this.m_nEarlyCount = value; }
        }


        private int m_nNormalCount = 0;
        /// <summary>
        /// ��ȡ�����ò���������д����֮��
        /// </summary>
        public int NormalCount
        {
            get { return this.m_nNormalCount; }
            set { this.m_nNormalCount = value; }
        }

        private int m_nTimeoutCount = 0;
        /// <summary>
        /// ��ȡ�����ò�����ʱ��д����֮��
        /// </summary>
        public int TimeoutCount
        {
            get { return this.m_nTimeoutCount; }
            set { this.m_nTimeoutCount = value; }
        }

        private int m_nUnwriteCount = 0;
        /// <summary>
        /// ��ȡ�����ò���δ��д��������֮��
        /// </summary>
        public int UnwriteCount
        {
            get { return this.m_nUnwriteCount; }
            set { this.m_nUnwriteCount = value; }
        }

        private static TimeCheckEngine m_instance = null;
        /// <summary>
        /// ��ȡʱЧ�������ʵ��
        /// </summary>
        public static TimeCheckEngine Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new TimeCheckEngine();
                return m_instance;
            }
        }
        private TimeCheckEngine()
        {
            this.m_lstTimeCheckResults = new List<TimeCheckResult>();
        }

        /// <summary>
        /// �ͷ����õ���Դ
        /// </summary>
        public void Dispose()
        {
            if (this.m_lstTimeCheckResults != null)
                this.m_lstTimeCheckResults.Clear();
            this.m_lstTimeCheckResults = null;
            TimeEventHandler.Instance.Dispose();
            DocumentHandler.Instance.Dispose();
        }

        /// <summary>
        /// ִ�в���ʱЧ��˼���
        /// </summary>
        /// <param name="arrTimeCheckQuery">�����б�</param>
        /// <returns>������б�</returns>
        public short PerformTimeCheck(params TimeCheckQuery[] arrTimeCheckQuery)
        {
            short shRet = 0;
            if (this.m_lstTimeCheckResults == null)
                this.m_lstTimeCheckResults = new List<TimeCheckResult>();
            this.m_lstTimeCheckResults.Clear();
            if (arrTimeCheckQuery == null || arrTimeCheckQuery.Length <= 0)
                return SystemData.ReturnValue.CANCEL;

            if (this.m_lstTimeQCRules == null || arrTimeCheckQuery[0].PatientID != this.m_szPatientID || arrTimeCheckQuery[0].VisitID != this.m_szVisitID)
            {
                if (this.m_lstTimeQCRules == null)
                {

                    shRet = TimeQCRuleAccess.Instance.GetTimeQCRules(ref this.m_lstTimeQCRules);
                    if (shRet != SystemData.ReturnValue.OK)
                        return shRet;
                    if (this.m_lstTimeQCRules == null)
                        return shRet;
                }

                this.m_szPatientID = arrTimeCheckQuery[0].PatientID;
                this.m_szVisitID = arrTimeCheckQuery[0].VisitID;
            }

            for (int index = 0; index < arrTimeCheckQuery.Length; index++)
            {
                TimeCheckQuery timeCheckQuery = arrTimeCheckQuery[index];
                if (timeCheckQuery == null)
                    continue;
                TimeEventHandler.Instance.TimeCheckQuery = timeCheckQuery;
                TimeEventHandler.Instance.ClearEventResult();
                DocumentHandler.Instance.ClearDocumentList();
                for (int ruleIndex = 0; ruleIndex < this.m_lstTimeQCRules.Count; ruleIndex++)
                {
                    TimeQCRule timeQCRule = this.m_lstTimeQCRules[ruleIndex];
                    List<TimeCheckResult> lstTimeCheckResults = this.PerformTimeRule(timeQCRule);
                    if (lstTimeCheckResults != null && lstTimeCheckResults.Count > 0)
                        this.m_lstTimeCheckResults.AddRange(lstTimeCheckResults);
                }
            }
            this.m_lstTimeQCRules.Clear();
            this.m_lstTimeQCRules = null;
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ִ��ָ����ʱЧ������
        /// </summary>
        /// <param name="timeQCRule">ʱЧ������Ϣ</param>
        private List<TimeCheckResult> PerformTimeRule(TimeQCRule timeQCRule)
        {
            if (timeQCRule == null || !timeQCRule.IsValid)
                return null;

            List<TimeQCEventResult> lstTimeQCEventResults = null;
            lstTimeQCEventResults = TimeEventHandler.Instance.PerformTimeEvent(timeQCRule.EventID);
            if (lstTimeQCEventResults == null || lstTimeQCEventResults.Count <= 0)
                return null;

            return DocumentHandler.Instance.CheckDocumentTime(timeQCRule, lstTimeQCEventResults);
        }

        public List<TimeCheckResult> GetUnwriteDoc(string szPatientID, string szVisitID)
        {
            TimeCheckQuery timeCheckQuery = new TimeCheckQuery();
            timeCheckQuery.PatientID = szPatientID;
            timeCheckQuery.VisitID = szVisitID;
            this.PerformTimeCheck(timeCheckQuery);
            List<TimeCheckResult> lstCheckResults = this.TimeCheckResults;
            List<TimeCheckResult> m_lstUnWriteResults = new List<TimeCheckResult>();

            for (int index = 0; index < lstCheckResults.Count; index++)
            {
                TimeCheckResult resultInfo = lstCheckResults[index];
                if (resultInfo.WrittenState == WrittenState.Unwrite)
                    m_lstUnWriteResults.Add(resultInfo);
            }
            return m_lstUnWriteResults;
        }
    }
}
