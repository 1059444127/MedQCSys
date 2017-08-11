// ***********************************************************
// �����ĵ�ϵͳ����ʱЧ����ʱЧ�¼�������
// Creator:Tangcheng  Date:2011-12-21
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Heren.Common.Libraries;
using EMRDBLib.Entity;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace MedDocSys.QCEngine.TimeCheck
{
    public class TimeEventHandler : IDisposable
    {
        private static TimeEventHandler m_instance = null;
        /// <summary>
        /// ��ȡʱЧ�¼�������ʵ��
        /// </summary>
        public static TimeEventHandler Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new TimeEventHandler();
                return m_instance;
            }
        }

        private TimeCheckQuery m_timeCheckQuery = null;
        /// <summary>
        /// ��ȡ�������¼���ѯ��Ϣ
        /// </summary>
        public TimeCheckQuery TimeCheckQuery
        {
            get { return this.m_timeCheckQuery; }
            set { this.m_timeCheckQuery = value; }
        }

        /// <summary>
        /// ���没��ʱЧ�¼�ID��Ӧ���¼���Ϣ
        /// </summary>
        private Dictionary<string, TimeQCEvent> m_dicTimeQCEvents = null;

        /// <summary>
        /// ���浱����ѯ�������¼�ִ�н���б�
        /// </summary>
        private Dictionary<string, List<TimeQCEventResult>> m_dicEventResults = null;

        /// <summary>
        /// ���ڷ�ֹ�û��������¼����ó���ѭ�����¼�ִ�ж�ջ
        /// </summary>
        private Stack<string> m_stkEventStack = null;

        private TimeEventHandler()
        {
        }

        /// <summary>
        /// �ͷ����õ���Դ
        /// </summary>
        public void Dispose()
        {
            if (this.m_dicEventResults != null)
                this.m_dicEventResults.Clear();
            this.m_dicEventResults = null;
            if (this.m_dicTimeQCEvents != null)
                this.m_dicTimeQCEvents.Clear();
            this.m_dicTimeQCEvents = null;
        }

        /// <summary>
        /// ���������¼�ִ�н��
        /// </summary>
        public void ClearEventResult()
        {
            if (this.m_dicEventResults != null)
                this.m_dicEventResults.Clear();
        }
        public void ClearTimeQCEvent()
        {
            if (this.m_dicTimeQCEvents != null)
                this.m_dicTimeQCEvents.Clear();
        }
        /// <summary>
        /// ��ȡָ��ID�ŵ�ʱЧ�ʿ��¼���Ϣ
        /// </summary>
        /// <param name="szEventID">�¼�ID</param>
        /// <returns>MDSDBLib.TimeQCEvent</returns>
        public TimeQCEvent GetTimeQCEvent(string szEventID)
        {
            if (this.m_dicTimeQCEvents != null&&this.m_dicTimeQCEvents.Count>0)
            {
                if (this.m_dicTimeQCEvents.ContainsKey(szEventID))
                    return this.m_dicTimeQCEvents[szEventID];
                return null;
            }

            this.m_dicTimeQCEvents =
                new Dictionary<string, TimeQCEvent>();

            List<TimeQCEvent> lstTimeQCEvents = null;
            TimeQCEventAccess.Instance.GetTimeQCEvents(ref lstTimeQCEvents);
            if (lstTimeQCEvents == null)
                return null;
            for (int index = 0; index < lstTimeQCEvents.Count; index++)
            {
                TimeQCEvent timeQCEvent = lstTimeQCEvents[index];
                if (this.m_dicTimeQCEvents.ContainsKey(timeQCEvent.EventID))
                    continue;
                this.m_dicTimeQCEvents.Add(timeQCEvent.EventID, timeQCEvent);
            }
            if (this.m_dicTimeQCEvents.ContainsKey(szEventID))
                return this.m_dicTimeQCEvents[szEventID];
            return null;
        }

        /// <summary>
        /// ����ָ����ʱЧ�¼�,�������¼��ж����SQL��ִ�н��
        /// </summary>
        /// <param name="szEventID">�¼�ID</param>
        /// <param name="lstTimeQCEventResults">�¼�ִ�н��</param>
        /// <returns>MDSDBLib.TimeQCEventResultList</returns>
        public List<TimeQCEventResult> PerformTimeEvent(string szEventID)
        {
            //׼���¼��ظ���������ջ
            if (this.m_stkEventStack == null)
                this.m_stkEventStack = new Stack<string>();
            this.m_stkEventStack.Clear();

            List<TimeQCEventResult> lstTimeQCEventResults = null;
            lstTimeQCEventResults = this.PerformEventInternal(szEventID);
            return lstTimeQCEventResults;
        }

        /// <summary>
        /// ��ȡָ���¼�ID���¼�����ʱ��
        /// </summary>
        /// <param name="szEventID">�¼�ID</param>
        /// <param name="lstTimeQCEventResults">�¼�ִ�н��</param>
        /// <returns>�¼���Ϣ�б�</returns>
        private List<TimeQCEventResult> PerformEventInternal(string szEventID)
        {
            if (GlobalMethods.Misc.IsEmptyString(szEventID))
                return null;

            //����ǰ�¼���ջ
            if (!this.m_stkEventStack.Contains(szEventID))
                this.m_stkEventStack.Push(szEventID);

            //��ǰ�¼���ִ�й�
            this.m_dicEventResults =
                new Dictionary<string, List<TimeQCEventResult>>();
            if (this.m_dicEventResults.ContainsKey(szEventID))
                return this.m_dicEventResults[szEventID];

            //��ȡ��ǰ�¼���Ϣ
            TimeQCEvent timeQCEvent = this.GetTimeQCEvent(szEventID);
            if (timeQCEvent == null)
                return null;

            //��ִ�е�ǰ�¼��������¼�
             List<TimeQCEventResult> lstTimeQCEventResults = null;
            if (!string.IsNullOrEmpty(timeQCEvent.DependEvent))
            {
                if (this.m_stkEventStack.Contains(timeQCEvent.DependEvent))
                {
                    string szLogText = "�Ƿ���ʱЧ�¼�����,����ѭ�����������¼�!";
                    szLogText += "�¼����=" + szEventID;
                    LogManager.Instance.WriteLog(szLogText);
                    return null;
                }
                lstTimeQCEventResults = this.PerformEventInternal(timeQCEvent.DependEvent);
                if (lstTimeQCEventResults == null || lstTimeQCEventResults.Count <= 0)
                    return null;
            }
            lstTimeQCEventResults = this.ExecuteEventSQL(timeQCEvent);
            if (!this.m_dicEventResults.ContainsKey(timeQCEvent.EventID))
                this.m_dicEventResults.Add(timeQCEvent.EventID, lstTimeQCEventResults);
            return lstTimeQCEventResults;
        }

        /// <summary>
        /// �ӵ�ǰϵͳ�е��û������˵����ݶ����л�ȡ��ָ�����Ե�ֵ
        /// </summary>
        /// <param name="szPropertyText">���������б�</param>
        /// <param name="objValue">���ص�����ֵ</param>
        /// <returns>bool</returns>
        private bool GetPropertyValue(string szPropertyText, ref object objValue)
        {
            if (this.m_timeCheckQuery == null)
                return false;
            string[] arrPropertyName = szPropertyText.Split('.');
            if (arrPropertyName == null)
                return false;

            object target = this;
            PropertyInfo propertyInfo = null;
            for (int index = 0; index < arrPropertyName.Length; index++)
            {
                string szPropertyName = arrPropertyName[index];
                if (target == null || string.IsNullOrEmpty(szPropertyName))
                    continue;
                propertyInfo = this.GetPropertyInfo(target.GetType(), szPropertyName);
                if (propertyInfo != null)
                    target = this.GetPropertyValue(target, propertyInfo);
            }
            if (target != this) objValue = target;
            return true;
        }

        /// <summary>
        /// ��ָ�������л�ȡָ�����Ƶ�������Ϣ
        /// </summary>
        /// <param name="type">ָ������</param>
        /// <param name="szPropertyName">ָ������������</param>
        /// <returns>PropertyInfo</returns>
        private PropertyInfo GetPropertyInfo(Type type, string szPropertyName)
        {
            if (type == null || GlobalMethods.Misc.IsEmptyString(szPropertyName))
                return null;
            try
            {
                return type.GetProperty(szPropertyName);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("TimeEventHandler.GetPropertyInfo", ex);
                return null;
            }
        }

        /// <summary>
        /// ��ָ���Ķ���ʵ���л�ȡָ�������Ե�ֵ
        /// </summary>
        /// <param name="obj">ָ���Ķ���ʵ��</param>
        /// <param name="propertyInfo">ָ��������</param>
        /// <returns>object</returns>
        private object GetPropertyValue(object obj, PropertyInfo propertyInfo)
        {
            if (obj == null || propertyInfo == null)
                return null;
            try
            {
                return propertyInfo.GetValue(obj, null);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("TimeEventHandler.GetPropertyValue", ex);
                return null;
            }
        }

        /// <summary>
        /// ʹ��args�����ڵ���������滻szFormatText�ַ�������"{}"��ʾ��ռλ��
        /// </summary>
        /// <param name="szFormatText">ָ�����ַ���</param>
        /// <param name="args">�����滻ռλ���Ĳ���</param>
        /// <returns>���滻�ɹ���ô���ط�null,��������쳣�򷵻�null</returns>
        private string FormatString(string szFormatText, params string[] args)
        {
            try
            {
                return string.Format(szFormatText, args);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("TimeEventHandler.FormatString", ex);
                return null;
            }
        }

        /// <summary>
        /// ִ�к�̨���õ��¼�SQL,��ȡ�¼�������ʱ��
        /// </summary>
        /// <param name="timeQCEvent">�¼�������Ϣ</param>
        /// <returns>SystemData.ReturnValue</returns>
        private List<TimeQCEventResult> ExecuteEventSQL(TimeQCEvent timeQCEvent)
        {
            if (timeQCEvent == null)
                return null;

            string szSqlCondition = timeQCEvent.SqlCondition;
            if (szSqlCondition == null)
                szSqlCondition = string.Empty;
            else
                szSqlCondition = szSqlCondition.Trim();

            string[] arrPropertyName = szSqlCondition.Split(';');
            string[] arrPropertyData = new string[arrPropertyName.Length];
            string level = "1"; //Ĭ���Ǽ�ؾ���ҽ�� 1 ����ҽʦ��2 �ϼ�ҽ����3 ����ҽ��
            bool bExistLevelDefinition = false;
            int i = 0;
            for (i = 0; i < arrPropertyName.Length; i++)
            {
                if (arrPropertyName[i] == "TimeCheckQuery.DoctorLevel")
                {
                    bExistLevelDefinition = true;
                    break;
                }
            }
            for (int nField = 0; nField < arrPropertyName.Length; nField++)
            {
                string szPropertyData = arrPropertyName[nField].Trim();
                if (GlobalMethods.Misc.IsEmptyString(szPropertyData))
                    continue;
                object objValue = null;
                if (this.GetPropertyValue(szPropertyData, ref objValue))
                    arrPropertyData[nField] = objValue as string;
                if (bExistLevelDefinition && nField == i)
                {
                    level = arrPropertyData[nField];
                }
            }

            //��ȡ�����������õ�SQL�����
            string szQuerySQL = timeQCEvent.SqlText.Trim();
            szQuerySQL = this.FormatString(szQuerySQL, arrPropertyData);
            List<TimeQCEventResult> lstTimeQCEventResults = null;
            if (!string.IsNullOrEmpty(szQuerySQL)
                && szQuerySQL!="null")
                TimeQCEventAccess.Instance.ExecuteTimeEventSQL(szQuerySQL, ref lstTimeQCEventResults);
            if (lstTimeQCEventResults != null)
            {
                foreach (TimeQCEventResult result in lstTimeQCEventResults)
                {
                    result.DoctorLevel = level;
                }
            }
            return lstTimeQCEventResults;
        }
    }
}
