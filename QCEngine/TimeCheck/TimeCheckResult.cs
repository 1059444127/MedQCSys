// ***********************************************************
// ����ʱЧ�ʿ�����ʱЧ���������Ϣ
// ��Ҫ��ʱЧ����������,������������ʱ����Լ�����ʱЧ�����
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using EMRDBLib;
using System;
using System.Collections.Generic;
namespace MedDocSys.QCEngine.TimeCheck
{
    /// <summary>
    /// ʱЧ���״̬ö��
    /// </summary>
    public enum WrittenState
    {
        /// <summary>
        /// δ��ʵ
        /// </summary>
        Uncheck = -1,
        /// <summary>
        /// ����״̬
        /// </summary>
        Normal = 0,
        /// <summary>
        /// δ��д
        /// </summary>
        Unwrite = 1,
        /// <summary>
        /// ��д��ǰ
        /// </summary>
        Early = 2,
        /// <summary>
        /// ��д��ʱ
        /// </summary>
        Timeout = 3
    }

    /// <summary>
    /// ʱЧ������б�
    /// </summary>
    public class TimeCheckResultList : List<TimeCheckResult>
    { }

    /// <summary>
    /// ʱЧ��˽����Ϣ��
    /// </summary>
    public class TimeCheckResult
    {
        private string m_szPatientID = string.Empty;        //���˱��
        private string m_szPatientName = string.Empty;      //��������
        private string m_szVisitID = string.Empty;          //���˵���סԺ��ʶ
        private string m_szVisitNO = string.Empty;
        private string m_szEventID = string.Empty;          //�¼�CODE
        private string m_szEventName = string.Empty;        //�¼�����
        private DateTime m_dtEventTime = DateTime.Now;      //�¼�����ʱ��        
        private string m_szDocID = string.Empty;            //�ĵ����
        private string m_szDocTitle = string.Empty;         //�ĵ�����
        private string m_szDocTypeID = string.Empty;        //�ĵ����ʹ����б�
        private string m_szDocTypeName = string.Empty;      //�ĵ����������б�
        private string m_szCreatorID = string.Empty;        //������ID
        private string m_szCreatorName = string.Empty;      //����������
        private DateTime m_dtDocTime = DateTime.Now;        //����ʵ����дʱ��
        private DateTime m_dtRecordTime = DateTime.Now;      //�����ĵ��ڲ���дʱ��
        private DateTime m_dtStartTime = DateTime.Now;      //��д����ʼʱ��
        private DateTime m_dtEndTime = DateTime.Now;        //�������ʱ��
        private string m_szWrittenPeriod = string.Empty;    //����ʱ������
        private bool m_bIsRepeat = false;                   //�Ƿ���ѭ����д
        private string m_szResultDesc = string.Empty;       //���������
        private float m_fQCScore = 0f;                      //�ʿؿ۷�
        private WrittenState m_writtenState = WrittenState.Normal;//״̬����
        private string m_szBedCode = string.Empty;          //����
        private DateTime m_dtVisitTime = DateTime.Now;      //����ʱ��
        private string m_szDoctorLevel = "1";               //���ҽ���ȼ� Ĭ��1��������ҽʦ
        private DateTime m_dtSignDate = DateTime.Now;       //�ĵ���ǩ��ʱ��
        private bool m_bIsStopRight = false;                //�Ƿ�Ҫֹͣ����Ȩ
        private bool m_IsVeto = false;                      //�Ƿ�����
        /// <summary>
        /// ��ȡĬ��ʱ��
        /// </summary>
        public DateTime DefaultTime
        {
            get { return DateTime.Parse("1900-1-1"); }
        }
        /// <summary>
        /// ��ȡ�����ò��˱��
        /// </summary>
        public string PatientID
        {
            get { return this.m_szPatientID; }
            set { this.m_szPatientID = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�������
        /// </summary>
        public string PatientName
        {
            get { return this.m_szPatientName; }
            set { this.m_szPatientName = value; }
        }
        /// <summary>
        /// ��ȡ�����ò��˵���סԺ��ʶ
        /// </summary>
        public string VisitID
        {
            get { return this.m_szVisitID; }
            set { this.m_szVisitID = value; }
        }
        /// <summary>
        /// ��ȡ�����û��߾�����ˮ��
        /// </summary>
        public string VisitNO
        {
            get { return this.m_szVisitNO; }
            set { this.m_szVisitNO = value; }
        }
        /// <summary>
        /// ��ȡ������ʱЧ������¼�ID
        /// </summary>
        public string EventID
        {
            get { return this.m_szEventID; }
            set { this.m_szEventID = value; }
        }
        /// <summary>
        /// ��ȡ������ʱЧ������¼�����
        /// </summary>
        public string EventName
        {
            get { return this.m_szEventName; }
            set { this.m_szEventName = value; }
        }
        /// <summary>
        /// ��ȡ�������¼�������ʱ��
        /// </summary>
        public DateTime EventTime
        {
            get { return this.m_dtEventTime; }
            set { this.m_dtEventTime = value; }
        }
        /// <summary>
        /// ��ȡ��������д����ʼʱ��
        /// </summary>
        public DateTime StartTime
        {
            get { return this.m_dtStartTime; }
            set { this.m_dtStartTime = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ�����ʱ��
        /// </summary>
        public DateTime EndTime
        {
            get { return this.m_dtEndTime; }
            set { this.m_dtEndTime = value; }
        }
        /// <summary>
        /// ��ȡ��������д��������
        /// </summary>
        public string WrittenPeriod
        {
            get { return this.m_szWrittenPeriod; }
            set { this.m_szWrittenPeriod = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ����
        /// </summary>
        public string DocID
        {
            get { return this.m_szDocID; }
            set { this.m_szDocID = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ�����
        /// </summary>
        public string DocTitle
        {
            get { return this.m_szDocTitle; }
            set { this.m_szDocTitle = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ����ʹ���
        /// </summary>
        public string DocTypeID
        {
            get { return this.m_szDocTypeID; }
            set { this.m_szDocTypeID = value; }
        }
        /// <summary>
        /// ��ȡ�����ò����ĵ���������
        /// </summary>
        public string DocTypeName
        {
            get { return this.m_szDocTypeName; }
            set { this.m_szDocTypeName = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ�ʵ����дʱ��
        /// </summary>
        public DateTime DocTime
        {
            get { return this.m_dtDocTime; }
            set { this.m_dtDocTime = value; }
        }

        /// <summary>
        /// ��ȡ�������ĵ��ڲ���дʱ��
        /// </summary>
        public DateTime RecordTime
        {
            get { return this.m_dtRecordTime; }
            set { this.m_dtRecordTime = value; }
        }

        /// <summary>
        /// ��ȡ�����ô�����ID��
        /// </summary>
        public string CreatorID
        {
            get { return this.m_szCreatorID; }
            set { this.m_szCreatorID = value; }
        }
        /// <summary>
        /// ��ȡ�����ô���������
        /// </summary>
        public string CreatorName
        {
            get { return this.m_szCreatorName; }
            set { this.m_szCreatorName = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�����д״̬����
        /// </summary>
        public WrittenState WrittenState
        {
            get { return this.m_writtenState; }
            set { this.m_writtenState = value; }
        }
        /// <summary>
        /// ��ȡ�������ʿؿ۷�
        /// </summary>
        public float QCScore
        {
            get { return this.m_fQCScore; }
            set { this.m_fQCScore = value; }
        }
        /// <summary>
        /// ��ȡ�������Ƿ�ѭ�����ʱЧ
        /// </summary>
        public bool IsRepeat
        {
            get { return this.m_bIsRepeat; }
            set { this.m_bIsRepeat = value; }
        }
        /// <summary>
        /// ��ȡ�����ü��������
        /// </summary>
        public string ResultDesc
        {
            get { return this.m_szResultDesc; }
            set { this.m_szResultDesc = value; }
        }
        /// <summary>
        /// ��ȡ��������Ժ���˴���
        /// </summary>
        public string BedCode
        {
            get { return this.m_szBedCode; }
            set { this.m_szBedCode = value; }
        }
        /// <summary>
        /// ��ȡ�����ò��˾���ʱ��
        /// </summary>
        public DateTime VisitTime
        {
            get { return this.m_dtVisitTime; }
            set { this.m_dtVisitTime = value; }
        }

        /// <summary>
        /// ���ҽ���ȼ�
        /// </summary>
        public string DoctorLevel
        {
            get { return this.m_szDoctorLevel; }
            set { this.m_szDoctorLevel = value; }
        }

        /// <summary>
        /// ǩ��ʱ��
        /// </summary>
        public DateTime SignDate
        {
            get { return this.m_dtSignDate; }
            set { this.m_dtSignDate = value; }
        }

        /// <summary>
        /// ��ȡ�������Ƿ�ֹͣ����ȨʱЧ
        /// </summary>
        public bool IsStopRight
        {
            get { return this.m_bIsStopRight; }
            set { this.m_bIsStopRight = value; }
        }

        /// <summary>
        /// ��ȡ�������Ƿ�����
        /// </summary>
        public bool IsVeto
        {
            get { return this.m_IsVeto; }
            set { this.m_IsVeto = value; }
        }

        public TimeCheckResult()
        {
            this.m_dtEventTime = this.DefaultTime;
            this.m_dtStartTime = this.DefaultTime;
            this.m_dtEndTime = this.DefaultTime;
            this.m_dtDocTime = this.DefaultTime;
            this.m_dtRecordTime = this.DefaultTime;
            this.m_dtSignDate = this.DefaultTime;
        }

        public string GetKeyWord()
        {
            string szEndTime = this.m_dtEndTime.ToString("yyyy-MM-dd HH:MM:ss");
            return this.m_szPatientID + "_" + this.m_szVisitID + "_" + this.m_szDocTypeID
                + "_" + szEndTime;
        }
        /// <summary>
        /// ���ָ���Ĳ�����Ϣ�Ƿ��ڵ�ǰ�������ֹʱ����
        /// </summary>
        /// <param name="docInfo">������Ϣ</param>
        /// <returns>bool</returns>
        internal bool IsTimeBetween(MedDocInfo docInfo)
        {
            if (docInfo == null)
                return false;

            if (docInfo.RECORD_TIME == docInfo.DefaultTime)
            {
                return (docInfo.DOC_TIME >= this.m_dtStartTime
                    && docInfo.DOC_TIME <= this.m_dtEndTime);
            }
            else
            {
                return (docInfo.RECORD_TIME >= this.m_dtStartTime
                    && docInfo.RECORD_TIME <= this.m_dtEndTime);
            }
        }

        /// <summary>
        /// ��ȡָ��������ʱ���뵱ǰ�������ʱ���
        /// </summary>
        /// <param name="docInfo">������Ϣ</param>
        /// <returns>long</returns>
        internal long TimeSpan(MedDocInfo docInfo)
        {
            if (docInfo == null)
                return -1;

            if (docInfo.DOC_TIME == docInfo.DefaultTime)
                return -1;
            return docInfo.DOC_TIME.Ticks - this.m_dtStartTime.Ticks;
        }

        public override string ToString()
        {
            return string.Format("WrittenState={0};StartTime={1};EndTime={2};DocTime={3};DocTitle={4};"
                , this.m_writtenState, this.m_dtStartTime, this.m_dtEndTime, this.m_dtDocTime, this.m_szDocTitle);
        }
    }
}
