using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;

namespace EMRDBLib
{
    /// <summary>
    /// ��ֵ�Ա�
    /// </summary>
    public class KeyValueData : DbTypeBase
    {
        /// <summary>
        /// ����
        /// </summary>
        public string DATA_KEY = "DATA_KEY";
        /// <summary>
        /// ��ֵ
        /// </summary>
        public string DATA_VALUE = "DATA_VALUE";
        /// <summary>
        /// ��ֵ�Է�����
        /// </summary>
        public string DATA_GROUP = "DATA_GROUP";
        /// <summary>
        /// ��չ�ֶ�1
        /// </summary>
        public string VALUE1 = "VALUE1";
        /// <summary>
        /// ����
        /// </summary>
        public string ID = "ID";

        public string MakeID()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            string szRand = rand.Next(0, 9999).ToString().PadLeft(4, '0');
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), szRand);
        }
    }
}
