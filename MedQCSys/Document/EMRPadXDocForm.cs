// ***********************************************************
// �����ʿ�ϵͳ�����ҿؼ��ĵ���ʾ����,��Ҫ������ǰ�Ĳ���.
// Creator:YangMingkun  Date:2010-11-10
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Windows.Forms;
using MedDocSys.PadWrapper;
 
using MedQCSys.DockForms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.DockSuite;
using EMRDBLib;

namespace MedQCSys.Document
{
    internal partial class EMRPadXDocForm : DockContentBase, IDocumentForm
    {
        public EMRPadXDocForm(MainForm mainForm)
            : base(mainForm)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.Icon = MedQCSys.Properties.Resources.MedDocIcon;
        }

        #region"IDocumentForm����"
        /// <summary>
        /// ��ȡ�����õ�ǰ�ĵ���Ϣ
        /// </summary>
        public  MedDocInfo Document
        {
            get
            {
                if (this.m_documents == null || this.m_documents.Count <= 0)
                    return null;
                return this.m_documents[0];
            }
        }

        private  MedDocList m_documents = null;
        /// <summary>
        /// ��ȡ�������ĵ���Ϣ�б�
        /// </summary>
        public  MedDocList Documents
        {
            get { return this.m_documents; }
        }

        private bool m_bIsEMRPad3 = true;
        /// <summary>
        /// ��ȡ�������б༭���ؼ�
        /// </summary>
        public IMedEditor MedEditor
        {
            get
            {
                if (this.m_bIsEMRPad3)
                {
                    if (this == null || this.EMRPad3Ctrl1.IsDisposed)
                        return null;
                    return this.EMRPad3Ctrl1;
                }
                else
                {
                    if (this == null || this.EPRPad2Ctrl1.IsDisposed)
                        return null;
                    return this.EPRPad2Ctrl1;
                }
            }
        }
        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            this.CloseDocument();
            if (this.MedEditor != null) this.EMRPad3Ctrl1.Dispose();
        }

        protected override void OnPatientInfoChanged()
        {
            base.OnPatientInfoChanged();
            this.Close();
        }

        /// <summary>
        /// ˢ���ĵ����ĵ���ǩ����ʾ�ı���
        /// </summary>
        /// <param name="szDocTitle">ȱʡ����</param>
        public void RefreshFormTitle(string szDocTitle)
        {
            string szTabText = szDocTitle;
            if (!GlobalMethods.Misc.IsEmptyString(szTabText))
                szTabText = szDocTitle;
            else if (this.Document == null)
                szTabText = "������";
            else
                szTabText = this.Document.DOC_TITLE;

            this.Text = string.Concat(szTabText, "(��)");

            if (this.Document == null)
                this.TabSubhead = string.Empty;
            else
            {
                this.TabSubhead = string.Format("{0} {1}", this.Document.CREATOR_NAME
                    , this.Document.DOC_TIME.ToString("yyyy-M-d HH:mm"));
            }
        }

        /// <summary>
        /// �ʿ���Ա����ʷ����
        /// </summary>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short OpenHistoryDocument(EMRDBLib.MedicalQcMsg questionInfo)
        {
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ָ����EMRPad�ĵ�
        /// </summary>
        /// <param name="document">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short OpenDocument( MedDocInfo document)
        {
            this.m_documents = new  MedDocList();
            this.m_documents.Add(document);
            return this.OpenDocument(this.m_documents);
        }

        /// <summary>
        /// �ϲ���ָ����һϵ���ĵ�
        /// </summary>
        /// <param name="documents">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short OpenDocument( MedDocList documents)
        {
            this.m_documents = documents;
            this.RefreshFormTitle(null);
            if (this.m_documents == null || this.m_documents.Count <= 0)
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            for (int index = 0; index < this.m_documents.Count; index++)
            {
                 MedDocInfo document = this.m_documents[index];
                if (GlobalMethods.Misc.IsEmptyString(document.PATIENT_ID))
                {
                    MessageBoxEx.Show("�޷��򿪾ɲ���������IDΪ�գ�");
                    return SystemData.ReturnValue.FAILED;
                }
                if (GlobalMethods.Misc.IsEmptyString(document.VISIT_ID))
                {
                    MessageBoxEx.Show("�޷��򿪾ɲ��������߾���ID�Ƿ���");
                    return SystemData.ReturnValue.FAILED;
                }

                string szPatientID = document.PATIENT_ID.Trim().PadLeft(10, '0');
                string szLastTwoChars = szPatientID.Substring(szPatientID.Length - 2);
                string szPrefixChars = szPatientID.Substring(0, szPatientID.Length - 2);

                string szLocalDocDir = string.Format("{0}\\Cache\\EMRPadX", GlobalMethods.Misc.GetWorkingPath());
                GlobalMethods.IO.CreateDirectory(szLocalDocDir);

                string szRemoteFile = string.Format("{0}\\{1}\\{2}", szLastTwoChars, szPrefixChars, document.FileName);
                string szLocalFile = string.Format("{0}\\{1}{2}.emr", szLocalDocDir, document.PATIENT_ID, document.FileName);
               
                if (!GlobalMethods.IO.DeleteFile(szLocalFile))
                {
                    MessageBoxEx.Show(string.Format("������{0}������ʧ�ܣ�", document.DOC_TITLE));
                    return SystemData.ReturnValue.FAILED;
                }
                shRet = MedDocSys.DataLayer.FsrvAccess.Instance.DownloadFile(szRemoteFile, szLocalFile);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.Show(string.Format("������{0}������ʧ�ܣ�", document.DOC_TITLE));
                    return SystemData.ReturnValue.FAILED;
                }

                if (index == 0 && !this.InitMedEditor(szLocalFile))
                {
                    MessageBoxEx.Show("������ʧ�ܣ��޷����������༭���ؼ���");
                    return SystemData.ReturnValue.FAILED;
                }
                if (index == 0)
                {
                    shRet = this.MedEditor.OpenDocument(szLocalFile);
                }
                else
                {
                    if (this.MedEditor.Readonly)
                        this.MedEditor.Readonly = false;
                }
                GlobalMethods.IO.DeleteFile(szLocalFile);
                if (shRet != MedDocSys.DataLayer.SystemData.ReturnValue.OK)
                    break;
            }
            this.MedEditor.Readonly = true;
            return shRet;
        }

        /// <summary>
        /// ��ӡ��ǰ�ĵ�
        /// </summary>
        /// <param name="bNeedPreview">�Ƿ���Ҫ��Ԥ���ٴ�ӡ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short PrintDocument(bool bNeedPreview)
        {
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �رյ�ǰ�ĵ�
        /// </summary>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short CloseDocument()
        {
            //��ղ��������Լ�������Ϣ����
            this.m_documents = null;
            if (this.MedEditor != null)
            {
                this.MedEditor.CloseDocument();
                this.MedEditor.Dispose();
            }

            return SystemData.ReturnValue.OK;
        }

        private bool InitMedEditor(string szFilePath)
        {
            this.m_bIsEMRPad3 = true;

            byte[] byteDocData = null;
            if (!GlobalMethods.IO.GetFileBytes(szFilePath, ref byteDocData))
                return false;

            //����������ݳ��Ȳ�����С��5
            if (byteDocData == null || byteDocData.Length < 5)
                return false;

            //���ǰ2���ֽ���MR,��ôʹ��EMRPad.ocx��
            if (byteDocData[0] == 0x4D && byteDocData[1] == 0x52)
            {
                this.m_bIsEMRPad3 = true;
                this.EMRPad3Ctrl1.BringToFront();
            }

            //���ǰ3���ֽ���EPR,��ôʹ��EPRPad.ocx��
            else if (byteDocData[0] == 0x45 && byteDocData[1] == 0x50 && byteDocData[2] == 0x52)
            {
                this.m_bIsEMRPad3 = false;
                this.EPRPad2Ctrl1.BringToFront();
            }
            return true;
        }
    }
}