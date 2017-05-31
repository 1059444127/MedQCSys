// ***********************************************************
// �����ĵ�ϵͳ�ĵ��ʿ�����,�ĵ�ȱ����Ϣ��
// Creator:YangMingkun  Date:2009-8-18
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;

namespace MedDocSys.QCEngine.BugCheck
{
    /// <summary>
    /// �ĵ�ȱ�ݵȼ�ö��
    /// </summary>
    public enum BugLevel
    {
        /// <summary>
        /// ����0
        /// </summary>
        Warn = 0,
        /// <summary>
        /// ����1
        /// </summary>
        Error = 1
    }

    /// <summary>
    /// �ĵ�ȱ����Ϣ��
    /// </summary>
    public class DocuemntBugInfo
    {
        private BugLevel m_bugLevel = BugLevel.Error;
        /// <summary>
        /// ��ȡ��������ʾ��BUG�ȼ�
        /// </summary>
        public BugLevel BugLevel
        {
            get { return this.m_bugLevel; }
            set { this.m_bugLevel = value; }
        }

        private string m_szBugKey = null;
        /// <summary>
        /// ��ȡ��������ʾ��BUG�ؼ���
        /// </summary>
        public string BugKey
        {
            get { return this.m_szBugKey; }
            set { this.m_szBugKey = value; }
        }

        private string m_szBugDesc = null;
        /// <summary>
        /// ��ȡ��������ʾ��BUG����
        /// </summary>
        public string BugDesc
        {
            get { return this.m_szBugDesc; }
            set { this.m_szBugDesc = value; }
        }

        private int m_nBugIndex = 0;
        /// <summary>
        /// ��ȡ������BUGȱ�ݶ�Ӧ���ַ����ĵ��е�����
        /// </summary>
        public int BugIndex
        {
            get { return this.m_nBugIndex; }
            set { this.m_nBugIndex = value; }
        }

        private string m_szResponse = string.Empty;
        /// <summary>
        /// ��ȡ������ȱ�ݵ���Ӧ��ʽ
        /// </summary>
        public string Response
        {
            get { return this.m_szResponse; }
            set { this.m_szResponse = value; }
        }
    }
}
