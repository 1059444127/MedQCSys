using System;
using System.Collections.Generic;
using System.Text;
using Heren.Common.Libraries;

namespace EMRDBLib
{
    /// <summary>
    /// �������ù����
    /// </summary>
    public class HdpUIConfig : DbTypeBase
    {
        private string m_szUIConfigID = string.Empty;
        private string m_szShowName = string.Empty;
        private string m_szProduct = string.Empty;
        private string m_szShortCuts = string.Empty;
        private string m_szShowType = string.Empty;
        private string m_szUIIcon = string.Empty;
        private string m_szUIIconSize = string.Empty;
        private string m_szUIRightKey = string.Empty;
        private string m_szUIRightDesc = string.Empty;
        private string m_szUICommand = string.Empty;
        private string m_szMicroHelp = string.Empty;
        private string m_szUIType = string.Empty;
        private string m_szParentID = string.Empty;
        private int m_szSortIndex = 0;
        private int m_nUIGrade = 0;
        private string m_szPopMenuResource = string.Empty;
        /// <summary>
        /// �Ҽ��˵�������Դ
        /// </summary>
        public string PopMenuResource
        {
            get
            {
                return this.m_szPopMenuResource;
            }
            set
            {
                this.m_szPopMenuResource = value;
            }
        }
        /// <summary>
        /// �˵�����
        /// </summary>
        public int UIGrade
        {
            get
            {
                return this.m_nUIGrade;
            }
            set
            {
                this.m_nUIGrade = value;
            }
        }
        /// <summary>
        /// ����ID��
        /// </summary>
        public string ParentID
        {
            get
            {
                return this.m_szParentID;
            }
            set
            {
                this.m_szParentID = value;
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public int SortIndex
        {
            get
            {
                return this.m_szSortIndex;
            }
            set
            {
                this.m_szSortIndex = value;
            }
        }

        /// <summary>
        /// ��ȡ�����ñ��
        /// </summary>
        public string UIConfigID
        {
            get { return this.m_szUIConfigID; }
            set { this.m_szUIConfigID = value; }
        }
        public string MakeUIConfigID()
        {
            return GlobalMethods.Misc.Random(100000, 999999).ToString();
        }
        /// <summary>
        /// ��ʾ����
        /// </summary>
        public string ShowName
        {
            get { return this.m_szShowName; }
            set { this.m_szShowName = value; }
        }
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string Product
        {
            get { return this.m_szProduct; }
            set { this.m_szProduct = value; }
        }
        /// <summary>
        /// ��ݼ�
        /// </summary>
        public string ShortCuts
        {
            get { return this.m_szShortCuts; }
            set { this.m_szShortCuts = value; }
        }

        /// <summary>
        /// ��ʾ��ʽ
        /// </summary> 
        public string ShowType
        {
            get { return this.m_szShowType; }
            set { this.m_szShowType = value; }
        }

        /// <summary>
        /// Ԫ��ͼ��
        /// </summary> 
        public string UIIcon
        {
            get { return this.m_szUIIcon; }
            set { this.m_szUIIcon = value; }
        }
        /// <summary>
        /// ͼ���С
        /// </summary> 
        public string UIIconSize
        {
            get { return this.m_szUIIconSize; }
            set { this.m_szUIIconSize = value; }
        }
        /// <summary>
        /// Ԫ��������Ȩ�޵�
        /// </summary> 
        public string UIRightKey
        {
            get { return this.m_szUIRightKey; }
            set { this.m_szUIRightKey = value; }
        }
        /// <summary>
        /// Ԫ��������Ȩ�޵�˵��
        /// </summary> 
        public string UIRightDesc
        {
            get { return this.m_szUIRightDesc; }
            set { this.m_szUIRightDesc = value; }
        }
        /// <summary>
        /// Ԫ������������
        /// </summary> 
        public string UICommand
        {
            get { return this.m_szUICommand; }
            set { this.m_szUICommand = value; }
        }
        /// <summary>
        /// Ԫ������������
        /// </summary> 
        public string MicroHelp
        {
            get { return this.m_szMicroHelp; }
            set { this.m_szMicroHelp = value; }
        }
        /// <summary>
        /// Ԫ������������
        /// </summary> 
        public string UIType
        {
            get { return this.m_szUIType; }
            set { this.m_szUIType = value; }
        }
        public override string ToString()
        {
            return this.ShowName;
        }

    }
}
