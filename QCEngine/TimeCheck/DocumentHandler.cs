// ***********************************************************
// ����ʱЧ�ʿ����没���б������
// ��Ҫ����������������дʱ���Ƿ��ǳ�ʱ��δ��д
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;
using EMRDBLib.Entity;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace MedDocSys.QCEngine.TimeCheck
{
    internal class DocumentHandler : IDisposable
    {
        private static DocumentHandler m_instance = null;
        /// <summary>
        /// ��ȡ��д�����б�����ʵ��
        /// </summary>
        public static DocumentHandler Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new DocumentHandler();
                return m_instance;
            }
        }
        private DocumentHandler()
        {
        }

        /// <summary>
        /// �������没�������벡����Ӧ��ϵ���ֵ�
        /// </summary>
        private Dictionary<string, MedDocList> m_dicDocInfos = null;


        /// <summary>
        /// �ͷ����õ���Դ
        /// </summary>
        public void Dispose()
        {
            if (this.m_dicDocInfos != null)
                this.m_dicDocInfos.Clear();
            this.m_dicDocInfos = null;
        }

        /// <summary>
        /// �������Ĳ����б�
        /// </summary>
        public void ClearDocumentList()
        {
            if (this.m_dicDocInfos != null)
                this.m_dicDocInfos.Clear();
            //�����ÿ�,�Ա����¼���
            this.m_dicDocInfos = null;
        }

       

        /// <summary>
        /// �����ĵ�DocTime���бȽ�
        /// </summary>
        /// <param name="docInfo1"></param>
        /// <param name="docInfo2"></param>
        /// <returns></returns>
        private int CompareByDocTime(MedDocInfo docInfo1, MedDocInfo docInfo2)
        {
            return docInfo1.DOC_TIME.CompareTo(docInfo2.DOC_TIME);
        }
        /// <summary>
        /// �����ĵ�RecordTime���бȽ�
        /// </summary>
        /// <param name="docInfo1"></param>
        /// <param name="docInfo2"></param>
        /// <returns></returns>
        private int CompareByRecordTime(MedDocInfo docInfo1,MedDocInfo docInfo2)
        {
            return docInfo1.RECORD_TIME.CompareTo(docInfo2.RECORD_TIME);
        }


        /// <summary>
        /// ����ָ��ʱЧ�����Լ������Ӧ��ʱЧ�¼�ִ�н��,�����д�����б�
        /// </summary>
        /// <param name="timeQCRule">ʱЧ����</param>
        /// <param name="lstTimeQCEventResults">�����Ӧ��ʱЧ�¼�ִ�н��</param>
        /// <returns>��д�����б�ʱЧ�����</returns>
        public List<TimeCheckResult> CheckDocumentTime(TimeQCRule timeQCRule
            , List<TimeQCEventResult> lstTimeQCEventResults)
        {
            if (timeQCRule == null || string.IsNullOrEmpty(timeQCRule.DocTypeID))
                return null;

            //��ȡʱЧ�¼����Ƶ���Ϣ
            TimeQCEvent timeQCEvent =
                TimeEventHandler.Instance.GetTimeQCEvent(timeQCRule.EventID);
            if (timeQCEvent == null)
                return null;

            //����Ӧ��д�Ĳ���ʱ���
            List<TimeCheckResult> lstTimeCheckResults =
                this.CreateTimeCheckTable(timeQCRule, lstTimeQCEventResults);
            if (lstTimeCheckResults == null || lstTimeCheckResults.Count <= 0)
                return null;
            //���ӱ�����δ��д�Ĳ�������δд����������
            TimeCheckEngine.Instance.UnwriteCount += lstTimeCheckResults.Count;
            List<MedDocInfo> lstDocInfos = null;
            for (int index = 0; index < lstTimeCheckResults.Count; index++)
            {
                TimeCheckResult timeCheckResult = lstTimeCheckResults[index];
                if (timeCheckResult == null)
                    continue;
                timeCheckResult.EventID = timeQCEvent.EventID;
                timeCheckResult.EventName = timeQCEvent.EventName;
                timeCheckResult.IsVeto = timeQCRule.IsVeto;

                //���ҵ�ǰʱЧ�����¼��Ӧ���ĵ�
                string szDocTypeIDList = timeCheckResult.DocTypeID;
                lstDocInfos = this.GetDocumentList(szDocTypeIDList);
                if (lstDocInfos == null || lstDocInfos.Count <= 0)
                {
                    timeCheckResult.WrittenState = WrittenState.Unwrite;
                    continue;
                }
                lstDocInfos.Sort(this.CompareByDocTime);
                //�˶Բ����ڲ�ʱ���Լ�ʵ����дʱ��
                for (int nDocIndex = 0; nDocIndex < lstDocInfos.Count; nDocIndex++)
                {
                    MedDocInfo docInfo = lstDocInfos[nDocIndex];

                    //RecordTime��Ϊ�ĵ���д���ݡ��ĵ��ڲ�ʱ����ʱЧ����ʼʱ��ͽ�ֹʱ��֮�����Ϊ��д�ĵ�
                    //Ȼ��ͨ��DocTime�ĵ�����ʵʱ�����жϳ�ʱ�����
                    if (docInfo.RECORD_TIME != docInfo.DefaultTime
                        && timeCheckResult.IsRepeat
                        && (docInfo.RECORD_TIME < timeCheckResult.StartTime || docInfo.RECORD_TIME > timeCheckResult.EndTime))
                        continue;
                    if (timeCheckResult.EventTime > docInfo.DOC_TIME
                        && timeCheckResult.EventTime.AddHours(-3) > docInfo.RECORD_TIME
                        && (docInfo.DOC_TITLE.Contains("�״β��̼�¼") || docInfo.DOC_TITLE.Contains("������¼") || docInfo.DOC_TITLE.Contains("����")))
                    {
                        continue;
                    }
                    if ((docInfo.DOC_TITLE.Contains("��ǰС��") || docInfo.DOC_TITLE.Contains("��ǰ����"))
                        && timeCheckResult.EventTime > docInfo.RECORD_TIME.AddDays(3))
                    {
                        //��ǰ�������ϵ���ǰ���ۺ���ǰС�ᣬ����Ϊ�����������ĵ�
                        continue;
                    }
                    long nCurrSpan = timeCheckResult.TimeSpan(docInfo); //DocTime-StartTime
                    if (nCurrSpan < 0)
                    {
                        timeCheckResult.WrittenState = WrittenState.Early;
                    }
                    else if (docInfo.DOC_TIME.CompareTo(timeCheckResult.EndTime) <= 0)
                    {
                        timeCheckResult.WrittenState = WrittenState.Normal;
                    }
                    else
                    {
                        timeCheckResult.WrittenState = WrittenState.Timeout;
                    }
                    timeCheckResult.DocTime = docInfo.DOC_TIME;
                    timeCheckResult.RecordTime = docInfo.RECORD_TIME;
                    timeCheckResult.DocID = docInfo.DOC_ID;
                    timeCheckResult.CreatorName = docInfo.CREATOR_NAME;
                    timeCheckResult.CreatorID = docInfo.CREATOR_ID;
                    break;
                }
            }

            //������д���ͳ��
            foreach (TimeCheckResult result in lstTimeCheckResults)
            {
                switch (result.WrittenState)
                {
                    case WrittenState.Early:
                        TimeCheckEngine.Instance.EarlyCount += 1;
                        break;
                    case WrittenState.Normal:
                        TimeCheckEngine.Instance.NormalCount += 1;
                        break;
                    case WrittenState.Unwrite:
                        TimeCheckEngine.Instance.UnwriteCount += 1;
                        break;
                    case WrittenState.Timeout:
                        TimeCheckEngine.Instance.TimeoutCount += 1;
                        break;
                    case WrittenState.Uncheck:
                        result.WrittenState = WrittenState.Unwrite;
                        break;
                    default:
                        break;
                }
            }
            return lstTimeCheckResults;
        }
        /// <summary>
        /// ����һ�Ų���������д�����ڱȶԵ�ʱЧ���ʱ���
        /// </summary>
        /// <param name="timeQCRule">ָ��ʱЧ����</param>
        /// <param name="lstTimeQCEventResults">ʱЧ�¼�ִ�н��</param>
        /// <returns>ʱЧ���ʱ����б�</returns>
        private List<TimeCheckResult> CreateTimeCheckTable(TimeQCRule timeQCRule
            , List<TimeQCEventResult> lstTimeQCEventResults)
        {
            if (lstTimeQCEventResults == null || lstTimeQCEventResults.Count <= 0)
                return null;
            if (TimeEventHandler.Instance.TimeCheckQuery == null)
                return null;

            List<TimeCheckResult> lstTimeCheckResults = new List<TimeCheckResult>();
            string szPatientID = TimeEventHandler.Instance.TimeCheckQuery.PatientID;
            string szPatientName = TimeEventHandler.Instance.TimeCheckQuery.PatientName;
            string szVisitID = TimeEventHandler.Instance.TimeCheckQuery.VisitID;
            string szBedCode = TimeEventHandler.Instance.TimeCheckQuery.BedCode;
            DateTime dtVisitTime = TimeEventHandler.Instance.TimeCheckQuery.VisitTime;

            //����һ�Ų���������д��ʱ���
            for (int index = 0; index < lstTimeQCEventResults.Count; index++)
            {
                TimeQCEventResult timeQCEventResult = lstTimeQCEventResults[index];
                if (timeQCEventResult == null)
                    continue;
                if (!timeQCEventResult.EventTime.HasValue)
                    continue;
                if (!timeQCEventResult.EndTime.HasValue)
                    continue;
                DateTime dtPeriodTime = timeQCEventResult.EventTime.Value;
                ////������д�������׼���޸�ѭ���˳��ж��߼�
                //bool IsRuleAsDocCommited = DataLayer.SystemParam.Instance.SystemOption.DocWriteRule == "2";
                do
                {
                    TimeCheckResult timeCheckResult = new TimeCheckResult();
                    timeCheckResult.PatientID = szPatientID;
                    timeCheckResult.VisitID = szVisitID;
                    timeCheckResult.PatientName = szPatientName;
                    timeCheckResult.DocTypeID = timeQCRule.DocTypeID;
                    timeCheckResult.DocTypeName = timeQCRule.DocTypeName;
                    timeCheckResult.BedCode = szBedCode;
                    timeCheckResult.VisitTime = dtVisitTime;
                    if (!GlobalMethods.Misc.IsEmptyString(timeQCRule.DocTypeAlias))
                        timeCheckResult.DocTypeName = timeQCRule.DocTypeAlias;
                    timeCheckResult.DocTitle = timeCheckResult.DocTypeName;
                    timeCheckResult.QCScore = timeQCRule.QCScore;
                    timeCheckResult.ResultDesc = timeQCRule.RuleDesc;
                    timeCheckResult.IsRepeat = timeQCRule.IsRepeat;
                    timeCheckResult.IsStopRight = timeQCRule.IsStopRight;
                    timeCheckResult.WrittenState = WrittenState.Uncheck;
                    timeCheckResult.EventTime = timeQCEventResult.EventTime.Value;
                    //timeCheckResult.InnerTime = timeCheckResult.DocTime;
                    timeCheckResult.StartTime = dtPeriodTime;
                    string szWrittenPeriod = timeQCRule.WrittenPeriod;
                    string szPeriodDesc = null;
                    timeCheckResult.EndTime =
                        this.GetWrittenPeriod(szWrittenPeriod, dtPeriodTime, ref szPeriodDesc);
                    timeCheckResult.WrittenPeriod = szPeriodDesc;
                    timeCheckResult.DoctorLevel = timeQCEventResult.DoctorLevel;
                    lstTimeCheckResults.Add(timeCheckResult);
                    dtPeriodTime = timeCheckResult.EndTime;
                } while (timeQCRule.IsRepeat && dtPeriodTime < timeQCEventResult.EndTime && dtPeriodTime < timeQCRule.ValidateTime);//��תԺ���������¼�������
                //������ѭ������Ӧ��д������������Ҫ  dtPeriodTime < timeQCEventResult.EndTime��timeQCEventResult.EndTime��תԺ���������¼�����ʱ�䣩

                //while (timeQCRule.IsRepeat && dtPeriodTime < timeQCEventResult.EndTime && (!IsRuleAsDocCommited || (IsRuleAsDocCommited && dtPeriodTime < timeQCRule.ValidateTime))); 
            }
            return lstTimeCheckResults;
        }

        /// <summary>
        /// ��ȡ����д��ָ���������͵Ĳ����б�
        /// </summary>
        /// <param name="szDocTypeIDList">ָ�����������б�</param>
        /// <returns>����д��ָ���������͵Ĳ����б�</returns>
        private List<MedDocInfo> GetDocumentList(string szDocTypeIDList)
        {
            if (this.m_dicDocInfos == null)
                this.InitDocumentDict();
            if (this.m_dicDocInfos == null || this.m_dicDocInfos.Count <= 0)
                return null;
            //��Ч���ID��
            List<MedDocInfo> lstWrittenDocInfos =
                new List< MedDocInfo>();

            //���doctypeidlistΪ��,����������
            if (string.IsNullOrEmpty(szDocTypeIDList))
            {
                foreach (List< MedDocInfo> item in this.m_dicDocInfos.Values)
                {
                    lstWrittenDocInfos.AddRange(item);
                }
            }


            StringBuilder sbDocTypeID = new StringBuilder();
            int index = 0;
            int count = szDocTypeIDList.Length;
            while (index < count)
            {
                char ch = szDocTypeIDList[index++];
                if (ch != ';')
                {
                    if (ch != ' ')
                        sbDocTypeID.Append(ch);
                    if (index < count)
                        continue;
                }
                if (sbDocTypeID.Length > 0)
                {
                    List<MedDocInfo> lstCurrDocInfos = null;
                    if (this.m_dicDocInfos.ContainsKey(sbDocTypeID.ToString()))
                        lstCurrDocInfos = this.m_dicDocInfos[sbDocTypeID.ToString()];
                    if (lstCurrDocInfos != null && lstCurrDocInfos.Count > 0)
                        lstWrittenDocInfos.AddRange(lstCurrDocInfos);
                    sbDocTypeID.Remove(0, sbDocTypeID.Length);
                }
            }

            return lstWrittenDocInfos;
        }

        /// <summary>
        /// ��ʼ����д�����ֵ��
        /// </summary>
        /// <returns>SystemData.ReturnValue</returns>
        private short InitDocumentDict()
        {
            if (TimeEventHandler.Instance.TimeCheckQuery == null)
                return SystemData.ReturnValue.OK;
            if (this.m_dicDocInfos != null)
                return SystemData.ReturnValue.OK;
            if (this.m_dicDocInfos != null && this.m_dicDocInfos.Count <= 0)
                return SystemData.ReturnValue.CANCEL;

            this.m_dicDocInfos = new Dictionary<string,MedDocList>();

            string szPatientID = TimeEventHandler.Instance.TimeCheckQuery.PatientID;
            string szVisitID = TimeEventHandler.Instance.TimeCheckQuery.VisitNO;//�༭����visit_id�����visit_no
            MedDocList lstDocInfos = null;
            short shRet = EmrDocAccess.Instance.GetDocInfos(szPatientID, szVisitID
                , SystemData.VisitType.IP, DateTime.Now, null, ref lstDocInfos);
            if (shRet != SystemData.ReturnValue.OK)
                return shRet;
            if (lstDocInfos == null || lstDocInfos.Count <= 0)
                return SystemData.ReturnValue.CANCEL;

            lstDocInfos.SortByTime(false);
            for (int index = 0; index < lstDocInfos.Count; index++)
            {
                string szDocTypeID = lstDocInfos[index].DOC_TYPE;
                MedDocList lstCurrTypeDocInfos = null;
                if (this.m_dicDocInfos.ContainsKey(szDocTypeID))
                {
                    lstCurrTypeDocInfos = this.m_dicDocInfos[szDocTypeID];
                    lstCurrTypeDocInfos.Add(lstDocInfos[index]);
                }
                else
                {
                    lstCurrTypeDocInfos = new MedDocList();
                    lstCurrTypeDocInfos.Add(lstDocInfos[index]);
                    this.m_dicDocInfos.Add(szDocTypeID, lstCurrTypeDocInfos);
                }
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���û����õ������ַ���ת�����ĵ���д����ʱ��
        /// </summary>
        /// <param name="szWrittenPeriod">�����ַ���</param>
        /// <param name="dtBaseTime">��׼ʱ���</param>
        /// <param name="szPeriodDesc">���ؽ������ȱ������</param>
        /// <returns>�ĵ���д����ʱ��</returns>
        private DateTime GetWrittenPeriod(string szWrittenPeriod, DateTime dtBaseTime, ref string szPeriodDesc)
        {
            int nPeriodTime = 24;
            if (szWrittenPeriod == null)
                goto EXCEPTION_LABEL;
            szWrittenPeriod = szWrittenPeriod.Trim();
            if (szWrittenPeriod.Length <= 1 || szWrittenPeriod.Length > 4)
                goto EXCEPTION_LABEL;

            //��ȡʱ������ֵ
            string szPeriodTimeValue = szWrittenPeriod.Substring(0, szWrittenPeriod.Length - 1);
            if (!int.TryParse(szPeriodTimeValue, out nPeriodTime))
                goto EXCEPTION_LABEL;

            if (szWrittenPeriod.EndsWith("H"))
            {
                szPeriodDesc = nPeriodTime.ToString() + "Сʱ";
                return dtBaseTime.AddHours(nPeriodTime);
            }
            else if (szWrittenPeriod.EndsWith("D"))
            {
                szPeriodDesc = nPeriodTime.ToString() + "��";
                DateTime dtEndTime = dtBaseTime.AddDays(nPeriodTime);
                return DateTime.Parse(dtEndTime.ToString("yyyy-MM-dd 23:59:59"));
            }
            else if (szWrittenPeriod.EndsWith("M"))
            {
                szPeriodDesc = nPeriodTime.ToString() + "��";
                DateTime dtEndTime = dtBaseTime.AddMonths(nPeriodTime);
                int nMonthDays = DateTime.DaysInMonth(dtEndTime.Year, dtEndTime.Month);
                return DateTime.Parse(string.Format("{0}-{1}-{2} 23:59:59", dtEndTime.Year, dtEndTime.Month, nMonthDays));
            }

            EXCEPTION_LABEL:
            szPeriodDesc = nPeriodTime.ToString() + "Сʱ";
            LogManager.Instance.WriteLog("DocumentHandler.GetDocPeriodTime", new string[] { "szWrittenPeriod", "dtBaseTime" }
                    , new object[] { szWrittenPeriod, dtBaseTime }, "����ʱ���޸�ʽ����!");
            return dtBaseTime.AddHours(nPeriodTime);
        }
    }
}
