using System;
using System.Collections.Generic;
using System.Text;

namespace MedDocSys.QCEngine.BugCheck
{
    internal class BugCheckEntry
    {
        private string m_szEntryID = string.Empty;          //ԭ�ӹ���Entry��ID
        private string m_szEntryName = string.Empty;        //ԭ�ӹ���Entry������
        private string m_szEntryType = string.Empty;        //ԭ�ӹ���Entry������
        private string m_szEntryDesc = string.Empty;        //ԭ�ӹ���Entry������
        private string m_szOperator = string.Empty;         //ԭ�ӹ���Entry��ֵ��ʵ�����ݵ������
        private string m_szEntryValue = string.Empty;       //ԭ�ӹ���Entry��ֵ
        private string m_szValueType = string.Empty;        //ԭ�ӹ���Entry��ֵ����������
        private int m_nOccurCount = 0;                      //ԭ�ӹ���Entry�ĳ��ִ���

        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry��ID
        /// </summary>
        public string EntryID
        {
            get { return this.m_szEntryID; }
            set { this.m_szEntryID = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry������
        /// </summary>
        public string EntryName
        {
            get { return this.m_szEntryName; }
            set { this.m_szEntryName = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry������
        /// </summary>
        public string EntryType
        {
            get { return this.m_szEntryType; }
            set { this.m_szEntryType = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry������
        /// </summary>
        public string EntryDesc
        {
            get { return this.m_szEntryDesc; }
            set { this.m_szEntryDesc = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry��ֵ��ʵ�����ݵ������
        /// </summary>
        public string Operator
        {
            get { return this.m_szOperator; }
            set { this.m_szOperator = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry��ֵ
        /// </summary>
        public string EntryValue
        {
            get { return this.m_szEntryValue; }
            set { this.m_szEntryValue = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry��ֵ����������
        /// </summary>
        public string ValueType
        {
            get { return this.m_szValueType; }
            set { this.m_szValueType = value; }
        }
        /// <summary>
        /// ��ȡ������ԭ�ӹ���Entry�ĳ��ִ���
        /// </summary>
        public int OccurCount
        {
            get { return this.m_nOccurCount; }
            set { this.m_nOccurCount = value; }
        }
    }
}
