// ***********************************************************
// �����ĵ�ϵͳ�ĵ��ʿ�����,�ʿع�����Ϣ��
// Creator:YangMingkun  Date:2009-8-18
// Copyright:supconhealth
// ***********************************************************
using System;
using EMRDBLib;
namespace MedDocSys.QCEngine.BugCheck
{
    internal class BugCheckRule
    {
        private string m_szRuleID = string.Empty;
        /// <summary>
        /// ��ȡ�����ù���ID
        /// </summary>
        public string RuleID
        {
            get { return this.m_szRuleID; }
            set { this.m_szRuleID = value; }
        }

        private BugCheckRule m_subRule1 = null;
        /// <summary>
        /// ��ȡ��������������1
        /// </summary>
        public BugCheckRule SubRule1
        {
            get { return this.m_subRule1; }
            set { this.m_subRule1 = value; }
        }

        private string m_szOperator = string.Empty;
        /// <summary>
        /// ��ȡ�������������������
        /// </summary>
        public string Operator
        {
            get { return this.m_szOperator; }
            set { this.m_szOperator = value; }
        }

        private BugCheckRule m_subRule2 = null;
        /// <summary>
        /// ��ȡ��������������2
        /// </summary>
        public BugCheckRule SubRule2
        {
            get { return this.m_subRule2; }
            set { this.m_subRule2 = value; }
        }

        private BugCheckEntry m_ruleEntry = null;
        /// <summary>
        /// ��ȡ������ԭ�ӹ���ID
        /// </summary>
        public BugCheckEntry RuleEntry
        {
            get { return this.m_ruleEntry; }
            set { this.m_ruleEntry = value; }
        }

        private BugCheckRule m_refRule = null;
        /// <summary>
        /// ��ȡ�����ý������ID
        /// </summary>
        public BugCheckRule RefRule
        {
            get { return this.m_refRule; }
            set { this.m_refRule = value; }
        }

        private string m_szBugKey = string.Empty;
        /// <summary>
        /// ��ȡ������ȱ�ݹؼ���
        /// </summary>
        public string BugKey
        {
            get { return this.m_szBugKey; }
            set { this.m_szBugKey = value; }
        }

        private BugLevel m_bugLevel = BugLevel.Error;
        /// <summary>
        /// ��ȡ������ȱ�ݵȼ�
        /// </summary>
        public BugLevel BugLevel
        {
            get { return this.m_bugLevel; }
            set { this.m_bugLevel = value; }
        }

        private string m_szBugDesc = string.Empty;
        /// <summary>
        /// ��ȡ������ȱ������
        /// </summary>
        public string BugDesc
        {
            get { return this.m_szBugDesc; }
            set { this.m_szBugDesc = value; }
        }

        private int m_nOccurCount = 0;
        /// <summary>
        /// ��ȡ�����ù���ĳ��ִ���,Ҳ��ȱ�ݳ��ִ���
        /// </summary>
        public int OccurCount
        {
            get { return this.m_nOccurCount; }
            set { this.m_nOccurCount = value; }
        }

        private string m_szResponse = string.Empty;
        /// <summary>
        /// ��ȡ������ȱ���ڿͻ��˽������Ӧ��ʽ
        /// </summary>
        public string Response
        {
            get { return this.m_szResponse; }
            set { this.m_szResponse = value; }
        }

        /// <summary>
        /// ִ�е�ǰ����
        /// </summary>
        public void Execute()
        {
            //RuleEntry������null˵����ԭ�ӹ���
            if (this.RuleEntry != null)
            {
                this.BugKey = this.RuleEntry.EntryValue;
                this.OccurCount = this.RuleEntry.OccurCount;
                return;
            }

            bool bSubRule1Result = true;
            if (this.SubRule1 != null)
            {
                this.SubRule1.Execute();
                bSubRule1Result = !(this.SubRule1.OccurCount <= 0);
            }

            bool bSubRule2Result = true;
            if (this.SubRule2 != null)
            {
                this.SubRule2.Execute();
                bSubRule2Result = !(this.SubRule2.OccurCount <= 0);
            }

            bool bConditionResult = bSubRule1Result;
            if (this.Operator == SystemData.Operator.AND)
            {
                bConditionResult = (bSubRule1Result && bSubRule2Result);
            }
            else if (this.Operator == SystemData.Operator.OR)
            {
                bConditionResult = (bSubRule1Result || bSubRule2Result);
            }

            if (!bConditionResult)
            {
                this.OccurCount = 0;
                this.BugKey = string.Empty;
                return;
            }

            //RefRule����null˵�����м����,���������չ���
            if (this.RefRule == null)
            {
                this.BugKey = string.Empty;
                this.OccurCount++;
            }
            else
            {
                this.RefRule.Execute();
                this.BugKey = this.RefRule.BugKey;
                this.OccurCount = this.RefRule.OccurCount;
            }
        }
    }
}
