// **************************************************************
// Ȩ�޵���Ϣ��
// Creator:yehui  Date:2014-10-24
// Copyright : Heren Health Services Co.,Ltd.
// **************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Heren.MedQC.Core
{
    /// <summary>
    /// Ȩ�޵���Ϣ��
    /// </summary>
    public class RightPoint
    {
        public RightPoint(string szRightKey, string szRightDesc, string szRightCommand)
        {
            this.m_RightKey = szRightKey;
            this.m_RightDesc = szRightDesc;
            this.m_RightCommand = szRightCommand;
        }
        private string m_RightKey = string.Empty;
        /// <summary>
        /// Ȩ�޵��ʶ
        /// </summary>
        public string RightKey
        {
            get { return this.m_RightKey; }
            set { this.m_RightKey = value; }
        }
        private string m_RightDesc = string.Empty;
        /// <summary>
        /// Ȩ�޵�����
        /// </summary>
        public string RightDesc
        {
            get { return this.m_RightDesc; }
            set { this.m_RightDesc = value; }
        }
        private string m_RightCommand = string.Empty;
        /// <summary>
        /// Ȩ�޵����������
        /// </summary>
        public string RightCommand
        {
            get { return this.m_RightCommand; }
            set { this.m_RightCommand = value; }
        }
        public override string ToString()
        {
            return this.m_RightKey;
        }
    }
}
