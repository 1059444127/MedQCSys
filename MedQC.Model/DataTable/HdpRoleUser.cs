using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;

namespace EMRDBLib
{

    /// <summary>
    /// �û���ɫ������
    /// </summary>
    public class HdpRoleUser : DbTypeBase
    {
        private string m_szUserID = string.Empty;
        private string m_szRoleCode = string.Empty;
        private string m_szRoleName = string.Empty;
        public string UserID
        {
            get { return this.m_szUserID; }
            set { this.m_szUserID = value; }
        }
        /// <summary>
        /// ��ȡ�����ý�ɫ����
        /// </summary>
        public string RoleCode
        {
            get { return this.m_szRoleCode; }
            set { this.m_szRoleCode = value; }
        }
        public string RoleName
        {
            get { return this.m_szRoleName; }
            set { this.m_szRoleName = value; }
        }
        public override string ToString()
        {
            return this.m_szRoleName;
        }
    }
}
