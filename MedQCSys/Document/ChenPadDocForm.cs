// ***********************************************************
// �����ʿ�ϵͳ�ĵ���������,���ĵ���Ӧ����ĵ�����
// Creator:LiChunYing  Date:2011-6-7
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.DockSuite;
using MedDocSys.PadWrapper;
using MedQCSys.DockForms;

using MedQCSys.Dialogs;
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Core;

namespace MedQCSys.Document
{
    public partial class ChenPadDocForm : DockContentBase, IDocumentForm
    {
        /// <summary>
        /// ȱ�ݼ�鰴ť
        /// </summary>
        private FlatButton m_CheckDocBugButton = null;

        public ChenPadDocForm(MainForm mainForm)
            : base(mainForm)
        {
            InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.Icon = MedQCSys.Properties.Resources.MedDocIcon;

            //�������ļ��ж��Ƿ���Ҫ���ӡ������ʼ����⡿��ť,���Ϊtrue����ʾȫ���Ҽ��˵���
            if (SystemParam.Instance.QCUserRight.BrowseQCQuestion.Value)
            {
                this.medEditor1.ShowInternalPopupMenu = true;
                this.medEditor1.ShowInternalToolStrip = true;
                this.medEditor1.ShowInternalMenuStrip = true;
                this.medEditor1.PopupMenu.InsertMenuItem("�����ʼ�����", new EventHandler(mnuAddFeedInfo_Click));
                this.medEditor1.PopupMenu.InsertMenuItem("�Զ����ȱ��", new EventHandler(this.CheckDocBugButton_Click));
                this.medEditor1.PopupMenu.InsertMenuItem("������׼���", new EventHandler(mnuShowStandardTermForm_Click));
                this.medEditor1.PopupMenu.InsertMenuItem("������ע...", new EventHandler(mnuInsertCommentForm_Click));
                this.medEditor1.PopupMenu.InsertMenuItem("�˻ز���...", new EventHandler(mnuRollbackDocument_Click));
                this.medEditor1.PopupMenu.InsertMenuItem("-");
            }
            else
            {
                this.medEditor1.ShowInternalPopupMenu = false;
                this.medEditor1.ShowInternalToolStrip = false;
                this.medEditor1.ShowInternalMenuStrip = false;
            }
            this.medEditor1.BeforeCopy += new CancelEventHandler(this.medEditor1_BeforeCopy);
            this.medEditor1.SaveButtonClick += new CancelEventHandler(medEditor1_SaveButtonClick);
            this.medEditor1.PrintButtonClick += new EventHandler(this.medEditor1_PrintButtonClick);
        }

        #region"IDocumentForm"
        /// <summary>
        /// ��ȡ�����õ�ǰ�ĵ���Ϣ
        /// </summary>
        public MedDocInfo Document
        {
            get
            {
                if (this.m_documents == null || this.m_documents.Count <= 0)
                    return null;
                return this.m_documents[0];
            }
        }

        private MedDocList m_documents = null;
        /// <summary>
        /// ��ȡ�������ĵ���Ϣ�б�
        /// </summary>
        public MedDocList Documents
        {
            get { return this.m_documents; }
        }

        /// <summary>
        /// ��ȡ�������б༭���ؼ�
        /// </summary>
        public IMedEditor MedEditor
        {
            get
            {
                if (this.medEditor1 == null || this.medEditor1.IsDisposed)
                    return null;
                return this.medEditor1;
            }
        }
        #endregion

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (SystemParam.Instance.QCUserRight.BrowseQCQuestion.Value)
                this.InitCheckDocBugButton();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            this.CloseDocument();
            if (this.medEditor1 != null && !this.medEditor1.IsDisposed)
                this.medEditor1.Dispose();
            if (this.m_CheckDocBugButton != null && !this.m_CheckDocBugButton.IsDisposed)
                this.m_CheckDocBugButton.Dispose();
        }

        protected override void OnPatientInfoChanged()
        {
            base.OnPatientInfoChanged();
            this.Close();
        }

        /// <summary>
        /// ˢ���ĵ�������ʾ����
        /// </summary>
        /// <param name="szDocTitle">�ĵ���ʾ����</param>
        public void RefreshFormTitle(string szDocTitle)
        {
            string szTabText = szDocTitle;
            if (this.Documents == null || this.Documents.Count <= 0)
                szTabText = "�����ĵ�����";

            if (this.Document != null)
                szTabText = this.Document.DOC_TITLE;
            if (this.Documents.Count > 1)
            {
                if (SystemParam.Instance.PatVisitInfo == null)
                    szTabText = "�����ĵ�����";
                else
                    szTabText = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�Ĳ���");
            }
            if (GlobalMethods.Misc.IsEmptyString(szTabText))
                szTabText = "�����ĵ�����";
            this.Text = szTabText;
            if (this.Document != null)
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
            MedDocInfo docInfo = new MedDocInfo();
            docInfo.PATIENT_ID = questionInfo.PATIENT_ID;
            docInfo.VISIT_ID = questionInfo.VISIT_ID;
            docInfo.DOC_SETID = questionInfo.TOPIC_ID;
            docInfo.DOC_TITLE = questionInfo.TOPIC;
            docInfo.CREATOR_NAME = questionInfo.ISSUED_BY;
            docInfo.DOC_TIME = questionInfo.ISSUED_DATE_TIME;
            docInfo.DOC_ID = questionInfo.TOPIC_ID;
            this.m_documents = new MedDocList();
            this.m_documents.Add(docInfo);
            this.RefreshFormTitle(null);
            byte[] byteSmdfData = null;
            string szRemoteFile = SystemParam.Instance.GetQCFtpDocPath(questionInfo, "smdf");
            short shRet = EmrDocAccess.Instance.GetFtpHistoryDocByID(docInfo.DOC_SETID, questionInfo.ISSUED_DATE_TIME, szRemoteFile, ref byteSmdfData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ���������ʧ�ܣ�");
                return shRet;
            }
            //�༭���ؼ������ĵ�
            shRet = this.medEditor1.OpenDocument(byteSmdfData, true);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ����ݼ���ʧ�ܣ�");
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ָ�����ĵ�
        /// </summary>
        /// <param name="document">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short OpenDocument( MedDocInfo document)
        {
            this.m_documents = new MedDocList();
            this.m_documents.Add(document);
            this.RefreshFormTitle(null);
            if (document == null || GlobalMethods.Misc.IsEmptyString(document.DOC_ID))
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ���Ϣ�Ƿ���");
                return SystemData.ReturnValue.FAILED;
            }

            byte[] byteSmdfData = null;
            short shRet = EmrDocAccess.Instance.GetDocByID(document.DOC_ID, ref byteSmdfData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ���������ʧ�ܣ�");
                return shRet;
            }

            //�༭���ؼ������ĵ�
            shRet = this.medEditor1.OpenDocument(byteSmdfData, true);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ����ݼ���ʧ�ܣ�");
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �ϲ���ָ����һϵ���ĵ�
        /// </summary>
        /// <param name="documents">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short OpenDocument(MedDocList documents)
        {
            this.m_documents = documents;
            if (documents == null || documents.Count <= 0)
                return SystemData.ReturnValue.CANCEL;

             MedDocInfo firstDocInfo = documents[0];
            this.RefreshFormTitle(null);

            WorkProcess.Instance.Initialize(this, documents.Count
                , string.Format("�������ء�{0}�������Ժ�...", firstDocInfo.DOC_TITLE));

            //���ص�һ���ĵ�
            byte[] byteDocData = null;
            short shRet = EmrDocAccess.Instance.GetDocByID(firstDocInfo.DOC_ID, ref byteDocData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                WorkProcess.Instance.Close();
                MessageBoxEx.Show(this, string.Format("�ĵ���{0}������ʧ�ܣ�", firstDocInfo.DOC_TITLE));
                return SystemData.ReturnValue.FAILED;
            }

            if (WorkProcess.Instance.Canceled)
            {
                WorkProcess.Instance.Close();
                return SystemData.ReturnValue.CANCEL;
            }
            WorkProcess.Instance.Show(string.Format("���ڴ򿪡�{0}�������Ժ�...", firstDocInfo.DOC_TITLE), 1);

            //�򿪵�һ���ĵ�
            shRet = this.medEditor1.InitCombinDisplay(byteDocData, firstDocInfo.DOC_ID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                WorkProcess.Instance.Close();
                MessageBoxEx.Show(this, string.Format("�ĵ���{0}������ʧ�ܣ�", firstDocInfo.DOC_TITLE));
                return SystemData.ReturnValue.FAILED;
            }

             DocTypeInfo prevDocTypeInfo = null;
            DataCache.Instance.GetDocTypeInfo(firstDocInfo.DOC_TYPE, ref prevDocTypeInfo);

            //ѭ����������ĵ�
            for (int index = 1; index < documents.Count; index++)
            {
                 MedDocInfo docInfo = documents[index];

                if (WorkProcess.Instance.Canceled)
                    break;
                WorkProcess.Instance.Show(string.Format("�������ء�{0}�������Ժ�...", docInfo.DOC_TITLE), index + 1);

                //�����ĵ�����
                byteDocData = null;
                shRet = EmrDocAccess.Instance.GetDocByID(docInfo.DOC_ID, ref byteDocData);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    string szTipInfo = string.Format("��{0}���ĵ���������ʧ�ܣ��Ƿ������", docInfo.DOC_TITLE);
                    if (MessageBoxEx.Show(szTipInfo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        break;
                }

                if (WorkProcess.Instance.Canceled)
                    break;
                WorkProcess.Instance.Show(string.Format("���ڴ򿪡�{0}�������Ժ�...", docInfo.DOC_TITLE), index + 1);

                //��ǰ�ĵ���ǰһ�ĵ�����Ҫ������ӡ��,��ô����һ��������Ӳ��ҳ��
                 DocTypeInfo docTypeInfo = null;
                DataCache.Instance.GetDocTypeInfo(docInfo.DOC_TYPE, ref docTypeInfo);
                if ((docTypeInfo != null && docTypeInfo.IsTotalPage)
                    || (prevDocTypeInfo != null && prevDocTypeInfo.IsEndEmpty))
                {
                    int nTotalPageCount = 0;

                    //��ȡ��ҳ��.ע��:�����ӡʱֻ��Ҫ����һ���հ�ҳ
                    if (this.medEditor1.PageSettings.PageLayout != MedDocSys.DataLayer.PageLayout.Normal)
                    {
                        //��ʽ��һ���ĵ�,������ȷ��ȡ��ҳ��
                        this.medEditor1.RefreshCombinDisplay();
                        this.medEditor1.GetPageCount(ref nTotalPageCount);
                    }

                    int nBlankPageCount = (nTotalPageCount % 2 == 0) ? 1 : 2;
                    this.AppendBlankToCombin(nBlankPageCount, true);
                }
                prevDocTypeInfo = docTypeInfo;

                if (WorkProcess.Instance.Canceled)
                    break;

                //OCX�༭��װ���ĵ�
                shRet = this.medEditor1.AppendToCombinDisplay(byteDocData, docInfo.DOC_ID);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    string szTipInfo = string.Format("��{0}���ĵ����ݼ���ʧ�ܣ��Ƿ������", docInfo.DOC_TITLE);
                    if (MessageBoxEx.Show(szTipInfo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        break;
                }
            }
            this.medEditor1.RefreshCombinDisplay();
            
            WorkProcess.Instance.Close();
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����հ�ֽ�ŵ��ϲ���ʾ���ĵ�
        /// </summary>
        /// <param name="nBlankCount">��Ҫ׷�ӵĿհ�ҳ����Ŀ</param>
        /// <param name="bNeedPrint">�հ�ҳ�Ƿ���Ҫ��ӡ</param>
        private void AppendBlankToCombin(int nBlankCount, bool bNeedPrint)
        {
            if (this.IsDisposed || nBlankCount <= 0)
                return;
            for (int index = 0; index < nBlankCount; index++)
            {
                this.medEditor1.AppendToCombinDisplay(null, MedDocSys.DataLayer.SystemData.MiscConstant.BLANK_DOCUMENT_ID);
            }
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
            this.medEditor1.CloseDocument();
            this.m_documents = null;
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ʼ���������ϵ��ĵ�ȱ�ݼ�鰴ť
        /// </summary>
        private void InitCheckDocBugButton()
        {
            if (this.m_CheckDocBugButton != null && !this.m_CheckDocBugButton.IsDisposed)
            {
                this.m_CheckDocBugButton.Visible = true;
                this.m_CheckDocBugButton.BringToFront();
                return;
            }
            this.m_CheckDocBugButton = new FlatButton();
            this.m_CheckDocBugButton.Text = "����ĵ�ȱ��";
            this.m_CheckDocBugButton.Image = global::MedQCSys.Properties.Resources.CheckDocBugs;
            this.m_CheckDocBugButton.Size = new Size(100, 22);
            this.m_CheckDocBugButton.Location = new Point(this.medEditor1.Width - this.m_CheckDocBugButton.Width - 16, 24);
            this.m_CheckDocBugButton.Parent = this.medEditor1;
            this.m_CheckDocBugButton.BackColor = Color.FromArgb(244, 243, 232);
            this.m_CheckDocBugButton.ForeColor = Color.Blue;
            this.m_CheckDocBugButton.Visible = true;
            this.m_CheckDocBugButton.BringToFront();
            this.m_CheckDocBugButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.m_CheckDocBugButton.Click += new EventHandler(this.CheckDocBugButton_Click);
        }

        private void AddFeedInfo()
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;

            if (this.m_documents == null)
                return;

            if (this.m_documents.Count <= 0)
                return;

            string szDocTitle = null;
            string szDocSetID = null;
            string szCreatorName = null;
            string szDeptCode = null;
            byte[] byteDocData = null;
            if (this.m_documents.Count == 1)
            {
                szDocTitle = this.m_documents[0].DOC_TITLE;
                szDocSetID = this.m_documents[0].DOC_SETID;
                szCreatorName = this.m_documents[0].CREATOR_NAME;
                szDeptCode = this.m_documents[0].DEPT_CODE;
                EmrDocAccess.Instance.GetDocByID(this.m_documents[0].DOC_ID, ref byteDocData);
            }
            else
            {
                szDocTitle = this.Text;
            }
            this.MainForm.AddFeedBackInfo(szDocTitle, szDocSetID, szCreatorName, szDeptCode, byteDocData);
        }

        private void CheckDocBugButton_Click(object sender, EventArgs e)
        {
            this.MainForm.ShowDocumentBugsForm(this);
        }

        private void mnuShowStandardTermForm_Click(object sender, EventArgs e)
        {
            this.MainForm.ShowStandardTermForm(this.medEditor1.GetSelectedText());
        }

        private void mnuInsertCommentForm_Click(object sender, EventArgs e)
        {
            //����ʿ���Աû���޸Ĳ���Ȩ�ޣ���ִ�в�����ע֮ǰ�޸����ĵ�����ô������עʱ��ʾ������֮ǰ�������޸ġ�
            if (!SystemParam.Instance.QCUserRight.EditAbleDoc.Value)
            {
                if (this.medEditor1.IsModified)
                {
                    do
                    {
                        this.medEditor1.Undo();
                    } while (this.medEditor1.IsModified);
                    MessageBoxEx.Show("��Ϊ��û���޸Ĳ���Ȩ�ޣ�ϵͳ�ѳ�����֮ǰ�����ĸ��ġ�\r\n������ѡ����Ҫ������ע��λ��");
                    return;
                }
            }
            if (this.Documents.Count > 1)
            {
                MessageBoxEx.ShowMessage("�ϲ��򿪵��ĵ����ܲ�����ע");
                return;
            }
            bool bTemp = this.medEditor1.Readonly;
            this.medEditor1.Readonly = false;
            InsertCommentForm insertCommentForm = new InsertCommentForm();
            DialogResult result = insertCommentForm.ShowDialog();
            if (result != DialogResult.OK)
                return;
            this.medEditor1.InsertElement("��ע", string.Empty, false);
            string szComment = string.Format("����ע��{0}��", insertCommentForm.UserComment);
            this.medEditor1.SetElementValue(szComment);
            this.medEditor1.LocateToText(szComment, 0);
            this.medEditor1.SetTextColor(Color.Red);
            byte[] temp = new byte[] { };
            this.medEditor1.SaveDocument(false, false, ref temp);
            this.medEditor1.Readonly = bTemp;
            this.medEditor1.IsModified = false;
            if (this.Document != null)
            {
                EmrDocAccess.Instance.UpdateDoc(this.Document.DOC_ID, this.Document, "", temp);
            }
            DialogResult diagResult = MessageBoxEx.Show("�Ƿ�����һ���ʼ����⣬�Ա�ҽ�����յ����ѣ�"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (diagResult != DialogResult.Yes)
                return;
            this.AddFeedInfo();
        }

        private void mnuRollbackDocument_Click(object sender, EventArgs e)
        {
            //�ж��Ƿ����ѹ鵵���ߵĲ���,C ��ʾ�ѹ鵵
            if (SystemParam.Instance.PatVisitInfo.MR_STATUS.ToUpper() == "C")
            {
                MessageBoxEx.Show(this, "�û��߲����ѹ鵵���������˻أ���ҽ����ҽ��վ���˲���", MessageBoxIcon.Warning);
                return;
            }
            else if (this.Document.SIGN_CODE == MedDocSys.DataLayer.SystemData.SignState.QC_ROLLBACK)
            {
                MessageBoxEx.Show(this, "�ò��������ʿ���Ա�˻�״̬�����������˻أ�", MessageBoxIcon.Warning);
                return;
            }

            RollbackDocument rbForm = new RollbackDocument();

            if (rbForm.ShowDialog() == DialogResult.OK)
            {
                //��ȡ�ĵ���Ϣ
               MedDocInfo docInfo = this.Document;
                string szOldDocID = this.Document.DOC_ID;
                this.Document.DOC_VERSION = this.Document.DOC_VERSION + 1;
                this.Document.DOC_ID = this.Document.DOC_SETID + "_" + this.Document.DOC_VERSION;

                byte[] byteSmdfData = null;
                this.medEditor1.SaveDocument(true, false, ref byteSmdfData);
                this.Document.SIGN_CODE = MedDocSys.DataLayer.SystemData.SignState.QC_ROLLBACK;
                short shRet = EmrDocAccess.Instance.UpdateDoc(szOldDocID, docInfo, "�ʿ���Ա�˻ز���", byteSmdfData);
                if (shRet == SystemData.ReturnValue.OK)
                {
                    //��ҽ��վ������Ϣ

                    //1.��Medical_QC_LOG�����뷴����Ϣ��¼
                    EMRDBLib.MedicalQcLog qcActionLog = new EMRDBLib.MedicalQcLog();
                    if (!QuestionListForm.MakeQCActionLog(ref qcActionLog))
                    {
                        GlobalMethods.UI.SetCursor(this, Cursors.Default);
                        return;
                    }
                    qcActionLog.LOG_DESC = "�ʿ���Ա�˻ز���";
                    qcActionLog.DOC_SETID = this.Document.DOC_SETID;
                    shRet = MedicalQcLogAccess.Instance.Insert(qcActionLog);

                    //2.��Medical_QC_MSG�������ʿ���Ա�ʼ��¼

                    EMRDBLib.MedicalQcMsg qcQuestionInfo = new EMRDBLib.MedicalQcMsg();
                    qcQuestionInfo.QC_MODULE = "DOCTOR_MR";
                    qcQuestionInfo.TOPIC_ID = this.Document.DOC_SETID;
                    qcQuestionInfo.TOPIC = this.Document.DOC_TITLE;
                    qcQuestionInfo.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
                    qcQuestionInfo.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
                    if (!GlobalMethods.Misc.IsEmptyString(SystemParam.Instance.PatVisitInfo.DEPT_CODE))
                        qcQuestionInfo.DEPT_STAYED = SystemParam.Instance.PatVisitInfo.DEPT_CODE;
                    else
                        qcQuestionInfo.DEPT_STAYED = SystemParam.Instance.PatVisitInfo.DischargeDeptCode;
                    string inChargeDoctor = string.Empty;
                    EmrDocAccess.Instance.GetPatChargeDoctorID(SystemParam.Instance.PatVisitInfo.PATIENT_ID, SystemParam.Instance.PatVisitInfo.VISIT_ID, ref inChargeDoctor);
                    qcQuestionInfo.DOCTOR_IN_CHARGE = inChargeDoctor;
                    qcQuestionInfo.PARENT_DOCTOR = SystemParam.Instance.PatVisitInfo.AttendingDoctor;
                    qcQuestionInfo.SUPER_DOCTOR = SystemParam.Instance.PatVisitInfo.SUPER_DOCTOR;
                    qcQuestionInfo.QC_MSG_CODE = "Q1";
                    qcQuestionInfo.MESSAGE = "���������Բ��������������Ӱ�������";
                    qcQuestionInfo.MSG_STATUS = 0;
                    qcQuestionInfo.QA_EVENT_TYPE = "����������������";
                    qcQuestionInfo.POINT_TYPE = 1;
                    qcQuestionInfo.ISSUED_BY = SystemParam.Instance.UserInfo.Name;
                    qcQuestionInfo.ISSUED_DATE_TIME = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
                    qcQuestionInfo.ISSUED_ID = SystemParam.Instance.UserInfo.ID;
                    qcQuestionInfo.POINT_TYPE = 0;
                    qcQuestionInfo.POINT = 0;

                    shRet = MedicalQcMsgAccess.Instance.Insert(qcQuestionInfo);
                    if (shRet == SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show(this, "�ѳɹ��˻ز���", MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void mnuAddFeedInfo_Click(object sender, EventArgs e)
        {
            this.AddFeedInfo();
        }

        private void medEditor1_BeforeCopy(object sender, CancelEventArgs e)
        {
            if (!SystemParam.Instance.UserRight.CopyFormDocument.Value)
                e.Cancel = true;
        }

        private void medEditor1_SaveButtonClick(object sender, CancelEventArgs e)
        {
            bool bQCSaveDocEnable = EmrDocAccess.Instance.CheckQCSaveDocEnable();
            bool bEditAbleDoc = SystemParam.Instance.QCUserRight.EditAbleDoc.Value;
            if (bQCSaveDocEnable && bEditAbleDoc)
            {
                byte[] temp = new byte[] { };
                this.medEditor1.SaveDocument(false, false, ref temp);
                if (this.Document != null)
                {
                    if (EmrDocAccess.Instance.UpdateDoc(this.Document.DOC_ID, this.Document, "�ʿؿƷ�������ֱ���޸�", temp) != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show("��������ʧ��");
                    }
                    else
                    {
                        MessageBoxEx.Show("����ɹ�", MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                if (!bQCSaveDocEnable)
                    MessageBoxEx.Show("�ʿؿƱ��没��δ����!");
                else if (!bEditAbleDoc)
                    MessageBoxEx.Show("��û��Ȩ�ޱ��没����");
                e.Cancel = true;
            }
        }


        private void medEditor1_PrintButtonClick(object sender, EventArgs e)
        {
            bool bReadonly = this.medEditor1.Readonly;
            if (!this.medEditor1.Readonly)
                this.medEditor1.Readonly = true;
            MedDocSys.DataLayer.PrintSettings printSettings = new MedDocSys.DataLayer.PrintSettings();
            this.medEditor1.PrintDocument(printSettings);
            this.medEditor1.Readonly = bReadonly;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}