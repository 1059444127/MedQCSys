using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;

namespace EMRDBLib
{
    /// <summary>
    /// ��ɫȨ�޹����
    /// </summary>
    public class HdpRoleGrant : DbTypeBase
    {
        private string m_szGrantID = string.Empty;
        private string m_szRoleCode = string.Empty;
        private string m_szRoleRightKey = string.Empty;
        private string m_szRoleRightDesc = string.Empty;
        private string m_szRoleRightCommand = string.Empty;
        private string m_szProduct = string.Empty;
        public string GrantID
        {
            get { return this.m_szGrantID; }
            set { this.m_szGrantID = value; }
        }
        /// <summary>
        /// ��ȡ���������
        /// </summary>
        public string RoleCode
        {
            get { return this.m_szRoleCode; }
            set { this.m_szRoleCode = value; }
        }
        /// <summary>
        /// ��������Դ
        /// </summary>
        public string RoleRightKey
        {
            get { return this.m_szRoleRightKey; }
            set { this.m_szRoleRightKey = value; }
        }
        /// <summary>
        /// �Ƿ�Ϊ�б���Դ
        /// </summary>
        public string RoleRightDesc
        {
            get { return this.m_szRoleRightDesc; }
            set { this.m_szRoleRightDesc = value; }
        }
        /// <summary>
        /// ��Ʒ
        /// </summary>
        public string RoleRightCommand
        {
            get { return this.m_szRoleRightCommand; }
            set { this.m_szRoleRightCommand = value; }
        }
        /// <summary>
        /// ��Ʒ
        /// </summary>
        public string Product
        {
            get { return this.m_szProduct; }
            set { this.m_szProduct = value; }
        }
      
        public string MakeGrantID()
        {
            return GlobalMethods.Misc.Random(100000, 999999).ToString();
        }
    }
}
