// ***********************************************************
// ����ʱЧ�ʿ�����ʱЧ����ѯ��
// ��Ҫ�����ⲿ���������������ѯ����ҽ������
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace MedDocSys.QCEngine.AuditCheck
{
    public class AuditCheckQuery
    {
        private string m_currentUserID = string.Empty;
        /// <summary>
        /// ��ȡ�����õ�ǰ�û�ID
        /// </summary>
        public string CurrentUserID
        {
            get { return this.m_currentUserID; }
            set { this.m_currentUserID = value; }
        }

        private string m_requestDoctorID = string.Empty;
        /// <summary>
        /// ��ȡ�������������ﾭ��ҽʦ
        /// </summary>
        public string RequestDoctorID
        {
            get { return this.m_requestDoctorID; }
            set { this.m_requestDoctorID = value; }
        }

        private string m_parentDoctorID = string.Empty;
        /// <summary>
        /// ��ȡ������������������ҽʦ
        /// </summary>
        public string ParentDoctorID
        {
            get { return this.m_parentDoctorID; }
            set { this.m_parentDoctorID = value; }
        }

        private string m_superDoctorID = string.Empty;
        /// <summary>
        /// ��ȡ������������������ҽʦ
        /// </summary>
        public string SuperDoctorID
        {
            get { return this.m_superDoctorID; }
            set { this.m_superDoctorID = value; }
        }

        private DateTime m_dtBeginDocTime = DateTime.Now;
        /// <summary>
        /// ��ȡ�����������ʼ�ĵ�ʱ��
        /// </summary>
        public DateTime BeginDocTime
        {
            get { return this.m_dtBeginDocTime; }
            set { this.m_dtBeginDocTime = value; }
        }
    }
}
