using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;

namespace EMRDBLib
{
    public class RecCodeCompasion : DbTypeBase
    {
        /// <summary>
        /// ���
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// ���ڴ������
        /// </summary>
        public  string DMLB { get; set; }
        /// <summary>
        /// �����ֵ����
        /// </summary>
        public  string DM { get; set; }
        /// <summary>
        /// �����ֵ�����
        /// </summary>
        public  string MC { get; set; }
        /// <summary>
        /// �����ֵ����
        /// </summary>
        public  string CODE_ID { get; set; }
        /// <summary>
        /// �����ֵ�����
        /// </summary>
        public  string CODE_NAME { get; set; }
        /// <summary>
        /// ���ʴ������
        /// </summary>
        public string CODETYPE_NAME { get; set; }
        /// <summary>
        /// �Ƿ�Ϊ������� 0 �� 1��
        /// </summary>
        public  decimal IS_CONFIG { get; set; }
        /// <summary>
        /// ��ѯ��Դ����sql����
        /// </summary>
        public  string FORM_SQL { get; set; }
        /// <summary>
        /// ��ѯĿ�����sql����
        /// </summary>
        public  string TO_SQL { get; set; }
        /// <summary>
        /// �ֵ��������
        /// </summary>
        public  string CONFIG_NAME { get; set; }
        public string MakeID()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            string szRand = rand.Next(0, 9999).ToString().PadLeft(4, '0');
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), szRand);
        }

        public override string ToString()
        {
            return this.CONFIG_NAME.ToString();
        }
    }
}
