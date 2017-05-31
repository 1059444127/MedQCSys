// ***********************************************************
// ����ʱЧ�ʿ�����ʱЧ����ѯ��
// ��Ҫ�����ⲿ���������������ѯ����ҽ������
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MedDocSys.QCEngine.TimeCheck
{
    public class TimeCheckQuery
    {
        private string m_szUserID = string.Empty;
        /// <summary>
        /// ��ȡ�������û�ID
        /// </summary>
        public string UserID
        {
            get { return this.m_szUserID; }
            set { this.m_szUserID = value; }
        }

        private string m_szPatientID = string.Empty;
        /// <summary>
        /// ��ȡ�����ò���ID
        /// </summary>
        public string PatientID
        {
            get { return this.m_szPatientID; }
            set { this.m_szPatientID = value; }
        }

        private string m_szVisitID = string.Empty;
        /// <summary>
        /// ��ȡ�����þ���ID
        /// </summary>
        public string VisitID
        {
            get { return this.m_szVisitID; }
            set { this.m_szVisitID = value; }
        }
        private string m_szVisitNo = string.Empty;
        /// <summary>
        /// ��ȡ�����þ�����ˮ��
        /// </summary>
        public string VisitNO
        {
            get { return this.m_szVisitNo; }
            set { this.m_szVisitNo = value; }
        }

        private string m_szBedCode = string.Empty;
        /// <summary>
        /// ��ȡ�����þ��ﴲ��
        /// </summary>
        public string BedCode
        {
            get { return this.m_szBedCode; }
            set { this.m_szBedCode = value; }
        }

        private string m_szDiagnosis = string.Empty;
        /// <summary>
        /// ��ȡ��������Ժ���
        /// </summary>
        public string Diagnosis
        {
            get { return this.m_szDiagnosis; }
            set { this.m_szDiagnosis = value; }
        }

        private string m_szPatientName = string.Empty;
        /// <summary>
        /// ��ȡ�����ò�������
        /// </summary>
        public string PatientName
        {
            get { return this.m_szPatientName; }
            set { this.m_szPatientName = value; }
        }

        private DateTime m_dtVisitTime = DateTime.Now;
        /// <summary>
        /// ��ȡ�����ò��˾���ʱ��
        /// </summary>
        public DateTime VisitTime
        {
            get { return this.m_dtVisitTime; }
            set { this.m_dtVisitTime = value; }
        }

        private string m_szDeptCode = string.Empty;
        /// <summary>
        /// ��ȡ�����ÿ��ұ���
        /// </summary>
        public string DeptCode
        {
            get { return this.m_szDeptCode; }
            set { this.m_szDeptCode = value; }
        }

        private string m_szDoctorLevel = string.Empty;
        /// <summary>
        /// ҽ���ȼ�
        /// <para>1.����ҽ��; 2.�ϼ�ҽ��; 3. ����ҽ��</para>
        /// </summary>
        public string DoctorLevel
        {
            get { return this.m_szDoctorLevel; }
            set { this.m_szDoctorLevel = value; }
        }
        public TimeCheckQuery()
        {
            this.m_szDoctorLevel = "1";
        }
    }
}
