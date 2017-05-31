// ***********************************************************
// ����ʱЧ�ʿ�����ʱЧ���������Ϣ
// ��Ҫ��ʱЧ����������,������������ʱ����Լ�����ʱЧ�����
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using EMRDBLib;
using System;
using System.Collections.Generic;

namespace MedDocSys.QCEngine.AuditCheck
{
    /// <summary>
    /// ��ǩ������б�
    /// </summary>
    public class AuditCheckResultList : List<AuditCheckResult>
    { }

    /// <summary>
    /// ������ǩ��˽����Ϣ��
    /// </summary>
    public class AuditCheckResult
    {
        private string m_szPatientID = string.Empty;        //���˱��
        private string m_szPatientName = string.Empty;      //��������
        private string m_szVisitID = string.Empty;          //���˵���סԺ��ʶ
        private string m_szDocID = string.Empty;            //�ĵ����
        private string m_szDocTitle = string.Empty;         //�ĵ�����
        private string m_szDocTypeID = string.Empty;        //�ĵ����ʹ����б�
        private string m_szDocTypeName = string.Empty;      //�ĵ����������б�
        private string m_szCreatorID = string.Empty;        //������ID
        private string m_szCreatorName = string.Empty;      //����������
        private string m_szModifierID = string.Empty;       //�޸���ID
        private string m_szModifierName = string.Empty;     //�޸�������
        private DateTime m_dtModifyTime = DateTime.Now;     //�޸�ʱ��
        private DateTime m_dtDocTime = DateTime.Now;        //������дʱ��
        private string m_szResultDesc = string.Empty;       //���������
        private string m_szAuditStatus = string.Empty;      //������ǩ״̬
        private DateTime m_dtVisitTime = DateTime.Now;      //����ʱ��

        /// <summary>
        /// ��ȡĬ��ʱ��
        /// </summary>
        public DateTime DefaultTime
        {
            get { return DateTime.Parse("1900-1-1"); }
        }
        /// <summary>
        /// ��ȡ�����ò��˱��
        /// </summary>
        public string PatientID
        {
            get { return this.m_szPatientID; }
            set { this.m_szPatientID = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�������
        /// </summary>
        public string PatientName
        {
            get { return this.m_szPatientName; }
            set { this.m_szPatientName = value; }
        }
        /// <summary>
        /// ��ȡ�����ò��˵���סԺ��ʶ
        /// </summary>
        public string VisitID
        {
            get { return this.m_szVisitID; }
            set { this.m_szVisitID = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ����
        /// </summary>
        public string DocID
        {
            get { return this.m_szDocID; }
            set { this.m_szDocID = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ�����
        /// </summary>
        public string DocTitle
        {
            get { return this.m_szDocTitle; }
            set { this.m_szDocTitle = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ����ʹ���
        /// </summary>
        public string DocTypeID
        {
            get { return this.m_szDocTypeID; }
            set { this.m_szDocTypeID = value; }
        }
        /// <summary>
        /// ��ȡ�����ò����ĵ���������
        /// </summary>
        public string DocTypeName
        {
            get { return this.m_szDocTypeName; }
            set { this.m_szDocTypeName = value; }
        }
        /// <summary>
        /// ��ȡ�������ĵ�ʵ��ʱ��
        /// </summary>
        public DateTime DocTime
        {
            get { return this.m_dtDocTime; }
            set { this.m_dtDocTime = value; }
        }
        /// <summary>
        /// ��ȡ�����ô�����ID��
        /// </summary>
        public string CreatorID
        {
            get { return this.m_szCreatorID; }
            set { this.m_szCreatorID = value; }
        }
        /// <summary>
        /// ��ȡ�����ô���������
        /// </summary>
        public string CreatorName
        {
            get { return this.m_szCreatorName; }
            set { this.m_szCreatorName = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�������޸���ID
        /// </summary>
        public string ModifierID
        {
            get { return this.m_szModifierID; }
            set { this.m_szModifierID = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�������޸�������
        /// </summary>
        public string ModifierName
        {
            get { return this.m_szModifierName; }
            set { this.m_szModifierName = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�������޸�ʱ��
        /// </summary>
        public DateTime ModifyTime
        {
            get { return this.m_dtModifyTime; }
            set { this.m_dtModifyTime = value; }
        }
        /// <summary>
        /// ��ȡ�����ü��������
        /// </summary>
        public string ResultDesc
        {
            get { return this.m_szResultDesc; }
            set { this.m_szResultDesc = value; }
        }
        /// <summary>
        /// ��ȡ��������ǩ״̬����
        /// </summary>
        public string AuditStatus
        {
            get { return this.m_szAuditStatus; }
            set { this.m_szAuditStatus = value; }
        }
        /// <summary>
        /// ��ȡ�����ü��������
        /// </summary>
        public DateTime VisitTime
        {
            get { return this.m_dtVisitTime; }
            set { this.m_dtVisitTime = value; }
        }

        public AuditCheckResult()
        {
            this.m_dtDocTime = this.DefaultTime;
            this.m_dtModifyTime = this.DefaultTime;
        }

        /// <summary>
        /// ����������ǩ��˽����Ӧ�Ĳ�����Ϣ
        /// </summary>
        /// <param name="docInfo">������Ϣ</param>
        internal void UpdateDocInfo(MedDocInfo docInfo)
        {
            if (docInfo == null)
            {
                this.m_szDocID = string.Empty;
                this.m_szCreatorID = string.Empty;
                this.m_szCreatorName = string.Empty;
                this.m_dtDocTime = this.DefaultTime;
                this.m_szDocTitle = string.Empty;
                this.m_szDocTypeID = string.Empty;
                this.m_szDocTypeName = string.Empty;
                this.m_szModifierID = string.Empty;
                this.m_szModifierName = string.Empty;
                this.m_dtModifyTime = this.DefaultTime;
                this.m_szPatientID = string.Empty;
                this.m_szPatientName = string.Empty;
                this.m_szVisitID = string.Empty;
                this.m_dtVisitTime = this.DefaultTime;
                this.m_szResultDesc = string.Empty;
                this.m_szAuditStatus = string.Empty;
            }
            else
            {
                this.m_szDocID = docInfo.DOC_ID;
                this.m_szCreatorID = docInfo.CREATOR_ID;
                this.m_szCreatorName = docInfo.CREATOR_NAME;
                this.m_dtDocTime = docInfo.DOC_TIME;
                this.m_szDocTitle = docInfo.DOC_TITLE;
                this.m_szDocTypeID = docInfo.DOC_TITLE;
                this.m_szDocTypeName = docInfo.DOC_TITLE;
                this.m_szModifierID = docInfo.MODIFIER_ID;
                this.m_szModifierName = docInfo.MODIFIER_NAME;
                this.m_dtModifyTime = docInfo.MODIFY_TIME;
                this.m_szPatientID = docInfo.PATIENT_ID;
                this.m_szPatientName = docInfo.PATIENT_NAME;
                this.m_szVisitID = docInfo.VISIT_ID;
                this.m_dtVisitTime = docInfo.VISIT_TIME;
                this.m_szResultDesc = docInfo.StatusDesc;
                if (docInfo.SIGN_CODE == SystemData.SignState.CREATOR_SAVE || docInfo.SIGN_CODE == ""//����SignCodeΪ���ַ���������
                    || (docInfo.SIGN_CODE == SystemData.SignState.CREATOR_COMMIT)) 
                    this.m_szAuditStatus = SystemData.SignState.CREATOR_SAVE_CH;
                else if (docInfo.SIGN_CODE == SystemData.SignState.CREATOR_COMMIT)
                    this.m_szAuditStatus = SystemData.SignState.CREATOR_COMMIT_CH;
                else if (docInfo.SIGN_CODE == SystemData.SignState.PARENT_COMMIT)
                    this.m_szAuditStatus = SystemData.SignState.PARENT_COMMIT_CH;
                else if (docInfo.SIGN_CODE == SystemData.SignState.SUPER_COMMIT)
                    this.m_szAuditStatus = SystemData.SignState.SUPER_COMMIT_CH;
                else if (docInfo.SIGN_CODE == SystemData.SignState.PARENT_ROLLBACK)
                    this.m_szAuditStatus = SystemData.SignState.PARENT_ROLLBACK_CH;
                else if (docInfo.SIGN_CODE == SystemData.SignState.SUPER_ROLLBACK)
                    this.m_szAuditStatus = SystemData.SignState.SUPER_ROLLBACK_CH;
                else if (docInfo.SIGN_CODE == SystemData.SignState.QC_ROLLBACK)
                    this.m_szAuditStatus = SystemData.SignState.QC_ROLLBACK_CH;
            }
        }

        public override string ToString()
        {
            return string.Format("DocID={0};DocTitle={1};DocTime={2};AuditState={3};ResultDesc={4};"
                , this.m_szDocID, this.m_szDocTitle, this.m_dtDocTime, this.m_szAuditStatus, this.m_szResultDesc);
        }
    }
}
