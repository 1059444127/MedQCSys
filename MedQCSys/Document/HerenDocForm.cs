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
using MedQCSys.DockForms;

using MedQCSys.Dialogs;
using Heren.Common.RichEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace MedQCSys.Document
{
    public partial class HerenDocForm : DockContentBase, IDocumentForm
    {
        private TextEditor textEditorPrint = null;
        /// <summary>
        /// ȱ�ݼ�鰴ť
        /// </summary>
        private FlatButton m_CheckDocBugButton = null;

        public HerenDocForm(MainForm mainForm)
            : base(mainForm)
        {
            InitializeComponent();
            this.textEditorPrint = new TextEditor();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.Icon = MedQCSys.Properties.Resources.MedDocIcon;

            //�������ļ��ж��Ƿ���Ҫ��ӡ������ʼ����⡿��ť,���Ϊtrue����ʾȫ���Ҽ��˵���

            this.InsertMenuItem("�����ʼ�����", new EventHandler(mnuAddFeedInfo_Click));
            this.InsertMenuItem("����", new EventHandler(textEditor1_SaveButtonClick));
            this.InsertMenuItem("�Զ����ȱ��", new EventHandler(this.CheckDocBugButton_Click));
            this.InsertMenuItem("������ע", new EventHandler(mnuInsertCommentForm_Click));
            this.InsertMenuItem("ɾ����ע", new EventHandler(mnuDeleteCommentForm_Click));
            this.InsertMenuItem("������׼���", new EventHandler(mnuShowStandardTermForm_Click));
            this.InsertMenuItem("�˻ز���", new EventHandler(mnuRollbackDocument_Click));


            this.textEditor1.BeforeCopy += new CancelEventHandler(this.textEditor1_BeforeCopy);
            this.textEditor1.CommentEditMode = true;
            this.textEditor1.ReviseEnabled = false;
            this.textEditor1.RevisionVisible = false;
            this.textEditor1.FieldFlagColor = Color.Transparent;
        }
        private void InsertMenuItem(string szText, EventHandler handler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Name = null;
            menuItem.Text = szText;
            if (handler != null)
                menuItem.Click += handler;
            this.PopupMenu.Items.Add(menuItem);
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
        private MedDocInfo m_ClickDocument;
        /// <summary>
        /// ѡ�е��ĵ��ڵ�
        /// </summary>
        public MedDocInfo ClickDocument
        {
            get
            {
                if (this.m_ClickDocument == null)
                    this.m_ClickDocument = new MedDocInfo();
                return this.m_ClickDocument;
            }
            set
            {
                this.m_ClickDocument = value;
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
        /// ��ȡ�������б༭���ؼ�,�¿ؼ���ΪTextEditor
        /// </summary>
        public TextEditor TextEditor
        {
            get
            {
                if (this.textEditor1 == null || this.textEditor1.IsDisposed)
                    return null;
                return this.textEditor1;
            }
        }

        MedDocList IDocumentForm.Documents
        {
            get
            {
                return this.Documents;
            }
        }

        MedDocInfo IDocumentForm.Document
        {
            get
            {
                return this.Document;
            }
        }



        MedDocSys.PadWrapper.IMedEditor IDocumentForm.MedEditor
        {
            get { return null; }
        }
        #endregion

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.InitCheckDocBugButton();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            this.CloseDocument();
            if (this.textEditor1 != null && !this.textEditor1.IsDisposed)
                this.textEditor1.Dispose();
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
            bool result = textEditor1.LoadDocument2(byteSmdfData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ����ݼ���ʧ�ܣ�");
                return shRet;
            }
            this.textEditor1.ReviseEnabled = true;
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ָ�����ĵ�
        /// </summary>
        /// <param name="document">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short OpenDocument(MedDocInfo document)
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
            short shRet = SystemData.ReturnValue.OK;
            shRet = EmrDocAccess.Instance.GetDocByID(document.DOC_ID, ref byteSmdfData);

            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ���������ʧ�ܣ�");
                return shRet;
            }

            //�༭���ؼ������ĵ�
            this.textEditor1.LoadDocument2(byteSmdfData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�޷����ĵ����ĵ����ݼ���ʧ�ܣ�");
                return shRet;
            }
            this.AppendHistory(SystemParam.Instance.UserInfo);
            this.GoSection(document);
            
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

            return SystemData.ReturnValue.OK;
        }
        /// <summary>
        /// �رյ�ǰ�ĵ�
        /// </summary>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short CloseDocument()
        {
            //��ղ��������Լ�������Ϣ����
            this.textEditor1.CloseDocument();
            this.m_documents = null;
            return SystemData.ReturnValue.OK;
        }

        public short GotoText(string text, int index)
        {
            bool result = true;
            this.textEditor1.GotoDocumentHead(true);
            while (index >= 0 && result)
            {
                result = this.textEditor1.FindNext(text, false, false);
                index--;
            }
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
            this.m_CheckDocBugButton.Location = new Point(this.textEditor1.Width - this.m_CheckDocBugButton.Width - 16, 24);
            this.m_CheckDocBugButton.Parent = this.textEditor1;
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

            SectionInfo sectionInfo = this.textEditor1.GetCurrentSection();

            if (sectionInfo != null)
            {
                this.m_documents[0].DOC_TITLE = sectionInfo.Name;
            }
            this.MainForm.AddMedicalQcMsg(this.m_documents[0]);
        }

        private void CheckDocBugButton_Click(object sender, EventArgs e)
        {
            this.MainForm.ShowDocumentBugsForm(this);
        }

        private void mnuShowStandardTermForm_Click(object sender, EventArgs e)
        {
            this.MainForm.ShowStandardTermForm(this.textEditor1.SelectedText);
        }

        private void mnuInsertCommentForm_Click(object sender, EventArgs e)
        {
            this.textEditor1.InsertComment();
        }
        private void mnuDeleteCommentForm_Click(object sender, EventArgs e)
        {
            this.textEditor1.DeleteComment();
        }
        /// <summary>
        /// ׷���޶���ʷ��Ϣ
        /// </summary>
        /// <param name="userInfo">�û���Ϣ</param>
        /// <returns></returns>
        public bool AppendHistory(UserInfo userInfo)
        {
            if (userInfo == null)
                return false;
            DocumentHistory history = new DocumentHistory();
            history.Author.Code = userInfo.USER_ID;
            history.Author.Name = userInfo.USER_NAME;
            history.Author.Organization = userInfo.DEPT_NAME;
            history.Timestamp = DateTime.Now;
            if (this.textEditor1.DocumentInfo.Histories.Count <= 0)
                history.Description = "����";
            else
                history.Description = "�޶�";
            this.textEditor1.DocumentInfo.Histories.Add(history);
            return true;
        }
        private void mnuRollbackDocument_Click(object sender, EventArgs e)
        {
            //�ж��Ƿ����ѹ鵵���ߵĲ���,C ��ʾ�ѹ鵵
            if (SystemParam.Instance.PatVisitInfo.MR_STATUS.ToUpper() == "C")
            {
                MessageBoxEx.Show(this, "�û��߲����ѹ鵵���������˻أ���ҽ����ҽ��վ���˲���", MessageBoxIcon.Warning);
                return;
            }
            else if (this.m_ClickDocument.SIGN_CODE == MedDocSys.DataLayer.SystemData.SignState.QC_ROLLBACK)
            {
                MessageBoxEx.Show(this, "�ò��������ʿ���Ա�˻�״̬�����������˻أ�", MessageBoxIcon.Warning);
                return;
            }

            RollbackDocument rbForm = new RollbackDocument();

            if (rbForm.ShowDialog() == DialogResult.OK)
            {
                //��ȡ�ĵ���Ϣ
                MedDocInfo docInfo = this.m_ClickDocument;
                this.m_ClickDocument.SIGN_CODE = MedDocSys.DataLayer.SystemData.SignState.QC_ROLLBACK;
                byte[] temp = null;
                if (this.Document.DOC_ID == this.m_ClickDocument.DOC_ID)
                {
                    this.textEditor1.SaveDocument2(out temp);
                }
                short shRet = EmrDocAccess.Instance.UpdateDoc(this.m_ClickDocument.DOC_ID, this.m_ClickDocument, rbForm.Reason, temp);
                if (shRet == SystemData.ReturnValue.OK)
                {
                    //��ҽ��վ������Ϣ
                    //1.��Medical_QC_LOG����뷴����Ϣ��¼
                    EMRDBLib.MedicalQcLog qcActionLog = new EMRDBLib.MedicalQcLog();
                    if (!QuestionListForm.MakeQCActionLog(ref qcActionLog))
                    {
                        GlobalMethods.UI.SetCursor(this, Cursors.Default);
                        return;
                    }
                    qcActionLog.LOG_DESC = "�ʿ���Ա�˻ز���";
                    qcActionLog.CHECK_TYPE = 6;
                    qcActionLog.AddQCQuestion = false;
                    qcActionLog.DOC_SETID = this.Document.DOC_SETID;
                    shRet = MedicalQcLogAccess.Instance.Insert(qcActionLog);

                    //2.��Medical_QC_MSG������ʿ���Ա�ʼ��¼

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
                    //DataAccess.GetPatChargeDoctorID(SystemParam.Instance.PatVisitLog.PatientID, SystemParam.Instance.PatVisitLog.VisitID, ref inChargeDoctor);
                    qcQuestionInfo.DOCTOR_IN_CHARGE = this.Document.CREATOR_NAME;
                    qcQuestionInfo.PARENT_DOCTOR = SystemParam.Instance.PatVisitInfo.AttendingDoctor;
                    qcQuestionInfo.SUPER_DOCTOR = SystemParam.Instance.PatVisitInfo.SUPER_DOCTOR;
                    qcQuestionInfo.QC_MSG_CODE = "Q1";
                    //���˻�ԭ����Ϊ�ʼ���������
                    if (string.IsNullOrEmpty(rbForm.Reason))
                        qcQuestionInfo.MESSAGE = "���������Բ��������������Ӱ�������";
                    else
                        qcQuestionInfo.MESSAGE = rbForm.Reason;
                    qcQuestionInfo.MSG_STATUS = 0;
                    qcQuestionInfo.QA_EVENT_TYPE = "�ʿ��˻�ԭ��";
                    qcQuestionInfo.POINT_TYPE = 1;
                    qcQuestionInfo.ISSUED_BY = SystemParam.Instance.UserInfo.USER_NAME;
                    qcQuestionInfo.ISSUED_DATE_TIME = SysTimeHelper.Instance.Now;
                    qcQuestionInfo.ISSUED_ID = SystemParam.Instance.UserInfo.USER_ID;
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

        private void textEditor1_BeforeCopy(object sender, CancelEventArgs e)
        {
            if (!SystemParam.Instance.UserRight.CopyFormDocument.Value)
                e.Cancel = true;
        }

        private void textEditor1_SaveButtonClick(object sender, EventArgs e)
        {
            //���ʱ༭���кۼ��������ܣ����ʿؿƷſ����没��Ȩ��
            byte[] temp = new byte[] { };
            this.textEditor1.SaveDocument2(out temp);
            if (this.Document != null)
            {
                if (EmrDocAccess.Instance.UpdateDoc(this.Document.DOC_ID, this.Document, "�ʿؿƷ�������ֱ���޸�", temp) != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.Show("��������ʧ��");
                }
                else
                {
                    MessageBoxEx.Show("����ɹ�", MessageBoxIcon.Information);
                    this.textEditor1.ReadOnly = true;
                }
            }
        }

        private void textEditor1_PrintButtonClick(object sender, EventArgs e)
        {
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
        public short GotoElement(string id, string name)
        {
            this.textEditor1.GotoField2(id, MatchMode.ID);
            return SystemData.ReturnValue.OK;
        }
        /// <summary>
        /// ��ȡ�ĵ���������Ԫ���йص�ȱ����Ϣ
        /// </summary>
        /// <param name="lstElementBugInfos">Ԫ��ȱ����Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short CheckElementBugs(ref List<ElementBugInfo> elementBugInfos)
        {
            if (this.textEditor1 == null || this.textEditor1.IsDisposed)
                return EMRDBLib.SystemData.ReturnValue.PARAM_ERROR;

            //�������Ϊ�յ�Ԫ��,������и����⣺
            //ͬһ��Ԫ���ĵ����ж��ʱ,�û�˫�������б�Ͷ�λ��׼ȷ

            if (elementBugInfos == null)
                elementBugInfos = new List<ElementBugInfo>();
            elementBugInfos.Clear();

            TextField[] textFileds = this.textEditor1.GetFields(null, MatchMode.Type);
            if (textFileds == null || textFileds.Length <= 0)
                return EMRDBLib.SystemData.ReturnValue.OK;
            foreach (TextField field in textFileds)
            {
                string fieldText = string.Empty;
                this.textEditor1.GetFieldText(field, out fieldText);

                ElementBugInfo bugInfo = new ElementBugInfo();
                bugInfo.Tag = field;
                bugInfo.ElementID = field.ID;
                bugInfo.ElementName = field.Name;
                if (field.FieldType == FieldType.TextOption)
                    bugInfo.ElementType = ElementType.SimpleOption;
                else if (field.FieldType == FieldType.RichOption)
                    bugInfo.ElementType = ElementType.ComplexOption;
                else
                    bugInfo.ElementType = ElementType.InputBox;

                if (!field.AllowEmpty && string.IsNullOrEmpty(fieldText))
                {
                    bugInfo.IsFatalBug = true;
                    bugInfo.BugDesc = string.Format("��{0}������Ϊ��!", bugInfo.ElementName);
                    elementBugInfos.Add(bugInfo);
                    continue;
                }

                if (field.HitForced && !field.AlreadyHitted)
                {
                    //�û�������������û�е��
                    bugInfo.BugDesc = string.Format("��{0}�����ص��ע�����������һ��!", field.Name);
                    elementBugInfos.Add(bugInfo);
                    continue;
                }

                //��鳬����Χ�����MaxValueδnull����-1ʱ�����ü��
                if (field.ValueType == FieldValueType.Numeric)
                {
                    decimal currValue = 0;
                    if (!decimal.TryParse(fieldText, out currValue))
                    {
                        bugInfo.IsFatalBug = true;
                        bugInfo.BugDesc = string.Format("���ڡ�{0}������ȷ������ֵ!", field.Name);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }

                    decimal minValue = 0;
                    decimal maxValue = 0;
                    bool result1 = decimal.TryParse(field.MinValue, out minValue);
                    bool result2 = decimal.TryParse(field.MaxValue, out maxValue);

                    //��ֵ���ڷ�Χ��
                    if (result1 && minValue > 0 && currValue < minValue)
                    {
                        bugInfo.BugDesc = string.Format("��{0}������С��{1}!", field.Name, field.MinValue);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }
                    if (result2 && maxValue > 0 && currValue > maxValue && maxValue >= 0)
                    {
                        bugInfo.BugDesc = string.Format("��{0}�����ܴ���{1}!", field.Name, field.MaxValue);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }
                }
                else if (field.ValueType == FieldValueType.DateTime)
                {
                    DateTime currValue = new DateTime();
                    if (!GlobalMethods.Convert.StringToDate(fieldText, ref currValue))
                    {
                        bugInfo.IsFatalBug = true;
                        bugInfo.BugDesc = string.Format("���ڡ�{0}������ȷ��������!", field.Name);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }

                    //���������ȷ��Ϊ���ڣ��ټ�鷶Χ
                    DateTime minValue;
                    DateTime maxValue;
                    bool result1 = DateTime.TryParse(field.MinValue, out minValue);
                    bool result2 = DateTime.TryParse(field.MaxValue, out maxValue);

                    //ʱ�䲻�ڷ�Χ��
                    if (result1 && currValue < minValue)
                    {
                        bugInfo.BugDesc = string.Format("��{0}��ʱ�䲻��С��{1}!", field.Name, field.MinValue);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }
                    if (result2 && currValue > maxValue)
                    {
                        bugInfo.BugDesc = string.Format("��{0}��ʱ�䲻�ܴ���{1}!", field.Name, field.MaxValue);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }
                }
                else
                {
                    int minValue = 0;
                    int maxValue = 0;
                    bool result1 = int.TryParse(field.MinValue, out minValue);
                    bool result2 = int.TryParse(field.MaxValue, out maxValue);

                    //�ı����Ȳ��ڷ�Χ��
                    if (result1 && minValue > 0 && fieldText.Length < minValue)
                    {
                        bugInfo.BugDesc = string.Format("��{0}���ı����Ȳ���С��{1}!", field.Name, field.MinValue);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }
                    if (result2 && maxValue > 0 && fieldText.Length > maxValue)
                    {
                        bugInfo.BugDesc = string.Format("��{0}���ı����Ȳ��ܴ���{1}!", field.Name, field.MaxValue);
                        elementBugInfos.Add(bugInfo);
                        continue;
                    }
                }
            }
            return SystemData.ReturnValue.OK;
        }
        public bool IsNum(string szFieldText)
        {
            if (string.IsNullOrEmpty(szFieldText))
                return false;
            bool result = Regex.IsMatch(szFieldText, @"^-?\d+\.?\d*$");
            return result;
        }
        public bool IsTime(string szFieldText)
        {
            if (string.IsNullOrEmpty(szFieldText))
                return false;
            DateTime dtFieldTime = new DateTime();
            string szTime = szFieldText.Replace("��", "-").Replace("��", "-").Replace("��", "").Replace("ʱ", ":").Replace("��", "").Replace("��", " ");
            bool result = DateTime.TryParse(szTime, out dtFieldTime);
            return result;
        }
        #region IDocumentForm ��Ա

        internal void GoSection(MedDocInfo docInfo)
        {
            this.Text = docInfo.DOC_TITLE;
            this.m_documents[0] = docInfo;
            this.m_ClickDocument = docInfo;
            SectionInfo section = this.TextEditor.GetSection(docInfo.DOC_ID, MatchMode.ID);
            if (this.TextEditor.GotoSection(section))
            {
                this.TextEditor.ScrollToView();
                this.TextEditor.SelectCurrentLine();
            }
        }

        public short OpenDocument(MDSDBLib.MedDocInfo document)
        {
            throw new NotImplementedException();
        }

        public short OpenDocument(MDSDBLib.MedDocList documents)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void toolbtnPrintPreview_Click(object sender, EventArgs e)
        {
            
            SectionInfo curSection= this.textEditor1.GetCurrentSection();
            byte[] byteDocument = null;
            this.textEditor1.SaveSection(curSection, out byteDocument);
            if (byteDocument == null)
            {
                this.textEditor1.SaveDocument2(out byteDocument);
            }
            this.textEditorPrint.ReviseEnabled = false;
            this.textEditorPrint.CommentEditMode = false;
            this.textEditorPrint.CommentVisible = false;
            this.textEditorPrint.RevisionVisible = false;
            this.textEditorPrint.LoadDocument2(byteDocument);
            this.textEditorPrint.Design = true;
            this.textEditorPrint.ShowPreviewDialog();
            this.textEditorPrint.Design = false;
        }

        private void toolbtnPrintAllPreview_Click(object sender, EventArgs e)
        {
            byte[] fileData = null;
            bool result= this.textEditor1.SaveDocument2(out fileData);
            if (fileData == null)
            {
                MessageBoxEx.ShowMessage("��ȡ��ӡ�ĵ�����ʧ�ܣ��޷�Ԥ��");
                return;
            }
            this.textEditorPrint.ReviseEnabled = false;
            this.textEditorPrint.CommentEditMode = false;
            this.textEditorPrint.CommentVisible = false;
            this.textEditorPrint.RevisionVisible = false;
            this.textEditorPrint.LoadDocument2(fileData);
            this.textEditorPrint.ShowPreviewDialog();
        }
    }
}