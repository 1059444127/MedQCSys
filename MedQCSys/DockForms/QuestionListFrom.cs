// ***********************************************************
// �����ʿ�ϵͳ�ʿ����������,���������������ʾ\�޸�\ɾ��
// �Լ�����������.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using Heren.Common.Controls;
using Heren.Common.Controls.TableView;
using Heren.Common.Controls.VirtualTreeView;

using EMRDBLib.DbAccess;
using EMRDBLib;
using MedQCSys.Dialogs;
using Heren.Common.VectorEditor.Print;
using System.Drawing.Printing;
using MedQCSys.Document;

namespace MedQCSys.DockForms
{
    public partial class QuestionListForm : DockContentBase
    {
        /// <summary>
        /// ���в����б��кϲ��ĵ��ڵ��ʶ
        /// </summary>
        private const string COMBIN_NODE_TAG = "COMBIN_NODE";
        /// <summary>
        /// ҽ��д�Ĳ���
        /// </summary>
        private const string DOCTOR_NODE_TAG = "DOC_NODE";
        /// <summary>
        /// ��ʿд�Ĳ���
        /// </summary>
        private const string NURSE_NODE_TAG = "NUR_NODE";
        /// <summary>
        /// δ֪���͵Ĳ���
        /// </summary>
        private const string UNKNOWN_NODE_TAG = "UNKNOWN_NODE";
        /// <summary>
        /// ��״̬���½�
        /// </summary>
        private const string STATUS_NEW = "NEW";
        /// <summary>
        /// ��״̬���޸�
        /// </summary>
        private const string STATUS_MODIFY = "MODIFY";
        /// <summary>
        /// ��״̬��ɾ��
        /// </summary>
        private const string STATUS_DELETE = "DELETE";
        /// <summary>
        /// ��״̬������
        /// </summary>
        private const string STATUS_NORMAL = "NORMAL";

        /// <summary>
        /// �����ʼ�����ģ��Ĺ�ϣ��
        /// </summary>
        private Hashtable m_htMsgDict = null;

        private string m_szDocSetID;
        /// <summary>
        /// ��ȡ�����ò������ĵ������
        /// </summary>
        public string DocSetID
        {
            get { return this.m_szDocSetID; }
            set { this.m_szDocSetID = value; }
        }

        private string m_szDocTitle;
        /// <summary>
        /// ��ȡ�����ò����ı���
        /// </summary>
        public string DocTitle
        {
            get { return this.m_szDocTitle; }
            set { this.m_szDocTitle = value; }
        }


        private string m_PrintName = string.Empty;
        private int m_iQTotalNum = 0;//��������
        private int m_iPrintedNum = 0;//�Ѵ�ӡ������
        private int m_iPrintingPageIndex = 0; //��ǰ����ӡ�ڼ�ҳ
        private int m_iTotalPageNum = 0; //��Ҫ��ӡ����ҳ��
        private const int NUM_PER_PAGE = 22;//ÿҳ�����¼�����
        List<UserInfo> m_lstUserInfo = null;
        public List<UserInfo> ListUserInfo
        {
            get
            {
                if (m_lstUserInfo == null)
                {
                    m_lstUserInfo = new List<UserInfo>();
                    UserAccess.Instance.GetAllUserInfos(ref m_lstUserInfo);
                }
                return m_lstUserInfo;
            }
        }

        public QuestionListForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.DockBottom;
            this.DockAreas = DockAreas.DockBottom | DockAreas.DockLeft
                | DockAreas.DockRight | DockAreas.DockTop;
            this.HideOnClose = true;
            this.CloseButtonVisible = false;
            this.dataGridView1.Font = new Font("����", 10.5f);
            this.dataGridView1.AutoReadonly = true;
            this.toolbthQChat.Visible = SystemParam.Instance.LocalConfigOption.IsShowChat;
            this.toolbtnLock.Visible = SystemParam.Instance.LocalConfigOption.IsShowDocLock;
            this.colLockStatus.Visible = SystemParam.Instance.LocalConfigOption.IsShowDocLock;
        }

        /// <summary>
        /// ˢ�»����ʿ��ʼ������б�
        /// </summary>
        public override void OnRefreshView()
        {
            base.OnRefreshView();
            if (!this.SaveUncommitedChange())
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ˢ���ʿ��ʼ����⣬���Ժ�...");
            this.LoadQCMsgDict();
            this.LoadMedicalQcMsgs();
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        protected override void OnPatientInfoChanging(CancelEventArgs e)
        {
            //ȡ���л����»���ʱ�ĵڶ��ε���
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            //����δ������ĸı�
            if (!this.SaveUncommitedChange())
                e.Cancel = true;
            else
                this.dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// ������Ϣ�ı䷽����д
        /// </summary>
        protected override void OnPatientInfoChanged()
        {
            if (this.IsHidden)
                return;
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this)
                this.OnRefreshView();
        }
        /// <summary>
        /// ���л���ĵ�ʱˢ������
        /// </summary>
        protected override void OnActiveContentChanged()
        {
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this && this.NeedRefreshView && this.IsActivated)
                this.OnRefreshView();
        }

        /// <summary>
        /// װ���ʿ��ʼ�����
        /// </summary>
        private void LoadMedicalQcMsgs()
        {
            this.dataGridView1.Rows.Clear();
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;

            List<MedicalQcMsg> lstMedicalQcMsgs = null;
            short shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(szPatientID, szVisitID, ref lstMedicalQcMsgs);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�ʿ��ʼ���������ʧ�ܣ�");
                return;
            }
            if (lstMedicalQcMsgs == null || lstMedicalQcMsgs.Count <= 0)
                return;

            for (int index = 0; index < lstMedicalQcMsgs.Count; index++)
            {
                MedicalQcMsg medicalQcMsg = lstMedicalQcMsgs[index];
                if (medicalQcMsg == null)
                    continue;

                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = medicalQcMsg;
                this.SetRowData(row, medicalQcMsg);
            }
            if (this.dataGridView1.Rows.Count > 0)
            {
                this.dataGridView1.ClearSelection();
            }
        }
        List<QcMsgDict> m_lstQcMsgDict = null;
        private void LoadQCMsgDict()
        {
            short shRet = QcMsgDictAccess.Instance.GetQcMsgDictList(ref m_lstQcMsgDict);
            if (shRet != SystemData.ReturnValue.OK && m_lstQcMsgDict == null)
                return;
            if (this.m_htMsgDict == null)
                this.m_htMsgDict = new Hashtable();
            for (int index = 0; index < m_lstQcMsgDict.Count; index++)
            {
                QcMsgDict messageTemplet = m_lstQcMsgDict[index];
                if (!m_htMsgDict.ContainsKey(messageTemplet.QC_MSG_CODE))
                    this.m_htMsgDict.Add(messageTemplet.QC_MSG_CODE, messageTemplet);
            }
        }

        /// <summary>
        /// ����Ƿ���δ�ύ���޸�
        /// </summary>
        /// <returns>bool</returns>
        public override bool HasUncommit()
        {
            if (this.dataGridView1.Rows.Count <= 0)
                return false;
            foreach (DataTableViewRow row in this.dataGridView1.Rows)
            {
                if (this.dataGridView1.IsDeletedRow(row))
                    return true;
                if (this.dataGridView1.IsNewRow(row))
                    return true;
                if (this.dataGridView1.IsModifiedRow(row))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �����ʿط������������б���޸�
        /// </summary>
        /// <returns>bool</returns>
        public override bool CommitModify()
        {
            if (this.dataGridView1.Rows.Count <= 0)
                return true;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            int count = 0;
            short shRet = SystemData.ReturnValue.OK;
            for (int index = 0; index < this.dataGridView1.Rows.Count; index++)
            {
                DataTableViewRow row = this.dataGridView1.Rows[index];
                shRet = this.SaveRowData(row);
                if (shRet == SystemData.ReturnValue.OK)
                    count++;
                else if (shRet == SystemData.ReturnValue.FAILED)
                    break;
            }
            if (count > 0)
            {
                //hhhh
                //�����ʿ���Ա��������¼
                MedicalQcLog medicalQcLog = new MedicalQcLog();
                bool bOK = QuestionListForm.MakeQCActionLog(ref medicalQcLog);

                //��ȡѡ�еĲ�����DocID��¼��medical_qc_log��
                if (this.MainForm.ActiveDocument is IDocumentForm)
                {
                    IDocumentForm document = this.MainForm.ActiveDocument as IDocumentForm;
                    if (this.MainForm.ActiveDocument is HerenDocForm)
                    {
                        HerenDocForm herendocument = this.MainForm.ActiveDocument as HerenDocForm;
                        if (herendocument.TextEditor.GetCurrentSection() != null)
                        {
                            string szDocID = herendocument.TextEditor.GetCurrentSection().ID;
                            medicalQcLog.DOC_SETID = szDocID;
                            LogManager.Instance.WriteLog(szDocID);
                        }
                    }
                    else
                        medicalQcLog.DOC_SETID = document.Document.DOC_ID;
                }
                if (bOK)
                {
                    medicalQcLog.LOG_DESC = "�ʿ�������ʼ�����";

                    this.SaveQCActionLog(medicalQcLog);
                }
            }

            this.RefreshQCResultStatus();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            string szMessageText = null;
            if (shRet == SystemData.ReturnValue.FAILED)
            {
                szMessageText = string.Format("����ʧ��,�ѱ���{0}����¼��", count);
                MessageBoxEx.Show(szMessageText, MessageBoxIcon.Information);
            }
            return shRet == SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="medicalQcMsg">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataTableViewRow row, MedicalQcMsg medicalQcMsg)
        {
            if (row == null || row.Index < 0 || medicalQcMsg == null)
                return false;
            if (this.m_htMsgDict == null)
                return false;

            row.Tag = medicalQcMsg;
            if (medicalQcMsg.LOCK_STATUS)
                row.Cells[this.colLockStatus.Index].Value = Properties.Resources._lock;
            row.Cells[this.colQaEventType.Index].Value = medicalQcMsg.QA_EVENT_TYPE;
            row.Cells[this.colMessage.Index].Value = medicalQcMsg.MESSAGE;
            row.Cells[this.col_POINT.Index].Value = medicalQcMsg.POINT;
            if (medicalQcMsg.ISSUED_DATE_TIME != medicalQcMsg.DefaultTime)
                row.Cells[this.colIssuedDateTime.Index].Value = medicalQcMsg.ISSUED_DATE_TIME.ToString("yyyy-MM-dd HH:mm");
            if (medicalQcMsg.ASK_DATE_TIME != medicalQcMsg.DefaultTime)
                row.Cells[this.col_ASK_DATE_TIME.Index].Value = medicalQcMsg.ASK_DATE_TIME.ToString("yyyy-MM-dd HH:mm");
            row.Cells[this.colTopic.Index].Value = medicalQcMsg.TOPIC;
            row.Cells[this.colTopic.Index].Tag = medicalQcMsg.TOPIC_ID;
            row.Cells[this.col_DOCTOR_COMMENT.Index].Value = medicalQcMsg.DOCTOR_COMMENT;
            row.Cells[this.colMessage.Index].Value = medicalQcMsg.MESSAGE;
            row.Cells[this.col_DOCTOR_IN_CHARGE.Index].Value = medicalQcMsg.DOCTOR_IN_CHARGE;

            //�ʼ���������ʾȨ�ޣ���Ȩ���������Լ���ӵ�
            //��ʾȨ�޸ĵ��ʿ�Ȩ�޿���
            row.Cells[this.col_ISSUED_BY.Index].Value = medicalQcMsg.ISSUED_BY;
            row.Cells[this.colMsgStatus.Index].Value = SystemData.MsgStatus.GetCnMsgState(medicalQcMsg.MSG_STATUS);
            if (medicalQcMsg.MSG_STATUS == SystemData.MsgStatus.UnCheck) //0Ϊδ���ա�δȷ��״̬
            {
                row.Cells[this.colMsgStatus.Index].Style.ForeColor = Color.Red;
            }
            else if (medicalQcMsg.MSG_STATUS ==
               SystemData.MsgStatus.Checked)//1Ϊ�ѽ��ա���ȷ��״̬
            {
                row.Cells[this.colMsgStatus.Index].Style.ForeColor = Color.Yellow;
            }
            else if (medicalQcMsg.MSG_STATUS == SystemData.MsgStatus.Modified)//2Ϊ���޸�״̬
            {
                row.Cells[this.colMsgStatus.Index].Style.ForeColor = Color.Blue;
            }
            else if (medicalQcMsg.MSG_STATUS == SystemData.MsgStatus.Pass)
            {
                row.Cells[this.colMsgStatus.Index].Style.ForeColor = Color.Green;
            }
            row.State = RowState.Normal;
            return true;
        }


        /// <summary>
        /// ����ָ���е����ݵ�Զ�����ݱ�,��Ҫע����ǣ��е�ɾ��״̬��������״̬����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="szDocSetID">�����ĵ���ID</param>
        /// <param name="szDocTitle">�����ĵ�����</param>
        /// <param name="szRowState">��״̬</param>
        /// <returns>SystemData.ReturnValue</returns>
        private short SaveRowData(DataTableViewRow row)
        {
            DateTime dtBeginTime1 = DateTime.Now;
            if (row == null || row.Index < 0)
                return SystemData.ReturnValue.FAILED;

            MedicalQcMsg medicalQcMsg = row.Tag as MedicalQcMsg;
            if (medicalQcMsg == null)
                return SystemData.ReturnValue.FAILED;
            short shRet = SystemData.ReturnValue.OK;
            if (row.State == RowState.Delete)
            {
                if (!this.dataGridView1.IsNewRow(row))
                    shRet = MedicalQcMsgAccess.Instance.Delete(medicalQcMsg.MSG_ID);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                this.dataGridView1.Rows.Remove(row);
            }
            else if (row.State == RowState.Update)
            {
                DateTime dtNewCheckDate = SysTimeHelper.Instance.Now;
                shRet = MedicalQcMsgAccess.Instance.Update(medicalQcMsg);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                string szRemotePath = SystemParam.Instance.GetQCFtpDocPath(medicalQcMsg, "smdf");
                EmrDocAccess.Instance.UpdateQCDocToFtp(medicalQcMsg, szRemotePath, dtNewCheckDate);
                row.Tag = medicalQcMsg;
            }
            else if (row.State == RowState.New)
            {
                shRet = MedicalQcMsgAccess.Instance.Insert(medicalQcMsg);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                //�Ѳ������浽FTP��
                if (string.IsNullOrEmpty(medicalQcMsg.TOPIC_ID))
                {
                    byte[] byteDocData = null;
                    shRet = EmrDocAccess.Instance.GetDocByID(medicalQcMsg.TOPIC_ID, ref byteDocData);
                    if (shRet == SystemData.ReturnValue.OK)
                    {
                        string szRemotePath = SystemParam.Instance.GetQCFtpDocPath(medicalQcMsg, "smdf");
                        DateTime dtBeginTime = DateTime.Now;
                        shRet = EmrDocAccess.Instance.SaveQCDocToFTP(medicalQcMsg.TOPIC_ID, medicalQcMsg.ISSUED_DATE_TIME, szRemotePath, byteDocData);
                        DateTime dtEndTime = DateTime.Now;
                    }
                }

            }
            row.State = RowState.Normal;
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ���������������־
        /// </summary>
        /// <param name="qcActionLog">���ɵļ����־��Ϣ</param>
        /// <returns>bool</returns>
        public static bool MakeQCActionLog(ref MedicalQcLog qcActionLog)
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return false;
            if (qcActionLog == null)
                qcActionLog = new MedicalQcLog();
            qcActionLog.CHECK_DATE = SysTimeHelper.Instance.Now;
            if (!GlobalMethods.Misc.IsEmptyString(SystemParam.Instance.PatVisitInfo.DEPT_CODE))
                qcActionLog.DEPT_STAYED = SystemParam.Instance.PatVisitInfo.DEPT_CODE;
            else
                qcActionLog.DEPT_STAYED = SystemParam.Instance.PatVisitInfo.DischargeDeptCode;
            qcActionLog.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            qcActionLog.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            qcActionLog.CHECKED_BY = SystemParam.Instance.UserInfo.USER_NAME;
            qcActionLog.CHECKED_ID = SystemParam.Instance.UserInfo.USER_ID;
            qcActionLog.DEPT_CODE = SystemParam.Instance.UserInfo.DEPT_CODE;
            qcActionLog.DEPT_NAME = SystemParam.Instance.UserInfo.DEPT_NAME;
            qcActionLog.DOC_SETID = string.Empty;
            qcActionLog.CHECK_TYPE = 5;
            qcActionLog.LOG_TYPE = 1;
            qcActionLog.LOG_DESC = "�ʿ�������ʼ�����";
            qcActionLog.AddQCQuestion = true;
            return true;
        }

        /// <summary>
        /// ����һ���������������־
        /// </summary>
        /// <returns>bool</returns>
        private bool SaveQCActionLog(MedicalQcLog qcActionLog)
        {
            short shRet = MedicalQcLogAccess.Instance.Insert(qcActionLog);
            if (shRet != SystemData.ReturnValue.OK)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show(string.Format("�������������־����ʧ�ܣ�"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// ˢ�²����ʿؽ��״̬
        /// </summary>
        private void RefreshQCResultStatus()
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            string szQCResultStatus = string.Empty;
            float fScore = 0;
            short shRet = MedQCAccess.Instance.GetQCResultStatus(szPatientID, szVisitID, SystemParam.Instance.LocalConfigOption.GradingLow, ref fScore, ref szQCResultStatus);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("���²������ʿؽ��״̬ʧ�ܣ�");
                return;
            }
            SystemParam.Instance.PatVisitInfo.QCResultStatus = szQCResultStatus;
            fScore = -1;
            //ˢ�²���״̬��־
            if (this.MainForm != null && !this.MainForm.IsDisposed)
                this.MainForm.RefreshQCResultStatus(-1);
        }

        /// <summary>
        /// ����Ƿ�������ǰ��������Ϊ���ͨ��
        /// </summary>
        /// <returns>bool</returns>
        private bool IsAllowSetAsPassed()
        {
            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            string szQCResultStatus = SystemParam.Instance.PatVisitInfo.QCResultStatus;

            //����д���δȷ�ϵ��ʼ����⣬�������־Ϊ�ϸ�
            List<MedicalQcMsg> lstmedicalQcMsgs = null;
            short shRet = SystemData.ReturnValue.OK;
            shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(szPatientID, szVisitID, ref lstmedicalQcMsgs);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯ�ʿ��ʼ������б�ʱ����,�޷����Ϊ�ϸ�");
                return false;
            }
            if (lstmedicalQcMsgs == null || lstmedicalQcMsgs.Count <= 0)
                return true;
            for (int index = 0; index < lstmedicalQcMsgs.Count; index++)
            {
                MedicalQcMsg medicalQcMsg = lstmedicalQcMsgs[index];
                if (medicalQcMsg.MSG_STATUS.ToString() != MDSDBLib.SystemData.QCQuestionStatus.PASSCODE)//�ʼ����ⶼ�����Ϊ�ϸ�
                {
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    MessageBoxEx.Show("�ò�������δ�ϸ���ʿ��ʼ�����,�޷����Ϊ�ϸ�");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ����ǰ��������Ϊ���ͨ��
        /// </summary>
        private void SetMedRecordPassed()
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            if (SystemParam.Instance.UserRight == null)
                return;


            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (!this.IsAllowSetAsPassed())
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }


            MedicalQcLog qcActionLog = new MedicalQcLog();
            if (!QuestionListForm.MakeQCActionLog(ref qcActionLog))
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            qcActionLog.LOG_DESC = "��������Ϊ�ϸ�";
            short shRet = MedicalQcLogAccess.Instance.Insert(qcActionLog);

            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�������������־����ʧ�ܣ�");
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.RefreshQCResultStatus();
            DialogResult reslut = DialogResult.OK;
            reslut = MessageBoxEx.Show("�����ѳɹ���־Ϊ�ϸ������Ƿ�Բ����������֣�", MessageBoxButtons.OKCancel
                , MessageBoxIcon.Question);
            if (reslut != DialogResult.OK)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.MainForm.ShowDocScoreForm();

            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }


        internal void AddNewItem(string szTopic, MedDocInfo docInfo)
        {
            if (!HasRightAddItem())
                return;

            //��������
            MedicalQcMsg medicalQcMsg = new MedicalQcMsg();
            medicalQcMsg.APPLY_ENV = "MEDDOC";
            medicalQcMsg.ERROR_COUNT = 1;
            medicalQcMsg.InpNo = SystemParam.Instance.PatVisitInfo.INP_NO;
            medicalQcMsg.ISSUED_BY = SystemParam.Instance.UserInfo.USER_NAME;
            medicalQcMsg.ISSUED_DATE_TIME = SysTimeHelper.Instance.Now;
            medicalQcMsg.ISSUED_ID = SystemParam.Instance.UserInfo.USER_ID;
            medicalQcMsg.ISSUED_TYPE = 0;
            medicalQcMsg.LOCK_STATUS = false;
            medicalQcMsg.MSG_STATUS = 0;
            medicalQcMsg.PARENT_DOCTOR = SystemParam.Instance.PatVisitInfo.PARENT_DOCTOR;
            medicalQcMsg.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            medicalQcMsg.PATIENT_NAME = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
            medicalQcMsg.POINT_TYPE = 1;
            medicalQcMsg.SUPER_DOCTOR = SystemParam.Instance.PatVisitInfo.SUPER_DOCTOR;
            medicalQcMsg.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            medicalQcMsg.VISIT_NO = SystemParam.Instance.PatVisitInfo.VISIT_NO;
            if (docInfo == null)
            {
                short shRet = this.GetSelectedNodeDocInfo(ref docInfo);
                if (shRet !=SystemData.ReturnValue.OK)
                    medicalQcMsg.TOPIC = szTopic;
            }
            else
            {
                medicalQcMsg.CREATOR_ID = docInfo.CREATOR_ID;
                medicalQcMsg.DEPT_NAME = docInfo.DEPT_NAME;
                medicalQcMsg.DEPT_STAYED = docInfo.DEPT_CODE;
                medicalQcMsg.DOCTOR_IN_CHARGE = docInfo.CREATOR_NAME;
                medicalQcMsg.TOPIC = docInfo.DOC_TITLE;
                medicalQcMsg.TOPIC_ID = docInfo.DOC_SETID;
                medicalQcMsg.DOC_ID = docInfo.DOC_ID;
            }
            SelectQuestionForm selectQuestionForm = new SelectQuestionForm();
            selectQuestionForm.MedicalQcMsg = medicalQcMsg;
            selectQuestionForm.Text = "�ʼ���������";
            if (selectQuestionForm.ShowDialog() == DialogResult.OK)
            {
                int index = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[index];
                row.Tag = medicalQcMsg;
                row.Cells[this.colQaEventType.Index].Value = medicalQcMsg.QA_EVENT_TYPE;
                row.Cells[this.colMessage.Index].Value = string.IsNullOrEmpty(medicalQcMsg.MESSAGE);
                row.Cells[this.colMessage.Index].Tag = medicalQcMsg.QC_MSG_CODE;
                row.Cells[this.colTopic.Index].Value = medicalQcMsg.TOPIC;
                row.Cells[this.colIssuedDateTime.Index].Value = medicalQcMsg.ISSUED_DATE_TIME.ToString("yyyy-MM-dd HH:mm");
                row.Cells[this.colMessage.Index].Value = medicalQcMsg.MESSAGE;
                row.Cells[this.col_ISSUED_BY.Index].Value = medicalQcMsg.ISSUED_BY;
                row.Cells[this.col_POINT.Index].Value = Math.Round(new decimal(GlobalMethods.Convert.StringToValue(medicalQcMsg.POINT, 0f)), 1).ToString("F1");
                row.Cells[this.colMsgStatus.Index].Value = "δ����";
                row.Cells[this.colMsgStatus.Index].Style.ForeColor = Color.Red;
                row.Cells[this.col_DOCTOR_IN_CHARGE.Index].Value = medicalQcMsg.DOCTOR_IN_CHARGE;
                row.Tag = medicalQcMsg;

                this.CommitModify();
                //this.UpdatePatientScore();
                return;
            }
        }
        /// <summary>
        ///�޸�ѡ��������
        /// </summary>
        private void ModifySelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            MedicalQcMsg medicalQcMsg = row.Tag as MedicalQcMsg;
            if (medicalQcMsg == null)
                return;

            if (medicalQcMsg.ISSUED_BY != SystemParam.Instance.UserInfo.USER_NAME
                && !SystemConfig.Instance.Get(SystemData.ConfigKey.MODIFY_OR_DELETE_QUESTION, false))
            {
                MessageBoxEx.Show("���ʼ����ⲻ������ӵģ����������޸ģ�", MessageBoxIcon.Warning);
                return;
            }
            SelectQuestionForm frmQuestion = new SelectQuestionForm();
            frmQuestion.MedicalQcMsg = medicalQcMsg;
            frmQuestion.Text = "�ʼ������ѯ�޸�";
            if (frmQuestion.ShowDialog() == DialogResult.OK)
            {
                row.State = RowState.Update;
                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                row.Cells[this.colMessage.Index].Value = medicalQcMsg.MESSAGE;
                row.Cells[this.col_POINT.Index].Value = medicalQcMsg.POINT;
                row.Cells[this.colQaEventType.Index].Value = medicalQcMsg.QA_EVENT_TYPE;
                this.CommitModify();
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
            }
            //this.UpdatePatientScore();
            //this.OnRefreshView();
        }

        /// <summary>
        /// ɾ��ѡ��������
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            if (SystemParam.Instance.UserRight == null)
                return;

            DialogResult result = MessageBoxEx.Show("�����Ҫɾ����ǰ����?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return;

            DataTableViewRow selectedRow = this.dataGridView1.SelectedRows[0];
            if (selectedRow == null)
                return;
            MedicalQcMsg questionInfo = selectedRow.Tag as MedicalQcMsg;
            if (questionInfo == null)
                return;

            if (selectedRow.Cells[this.col_ASK_DATE_TIME.Index].Value != null)
            {
                MessageBoxEx.Show("���ʼ������ѱ�ȷ�ϣ�����ɾ����", MessageBoxIcon.Warning);
                return;
            }
            if (questionInfo.ISSUED_BY != SystemParam.Instance.UserInfo.USER_NAME
                && !SystemConfig.Instance.Get(SystemData.ConfigKey.MODIFY_OR_DELETE_QUESTION, false))
            {
                MessageBoxEx.Show("���ʼ����ⲻ������ӵģ���������ɾ����", MessageBoxIcon.Warning);
                return;
            }
            selectedRow.State = RowState.Delete;
            this.CommitModify();
            //this.UpdatePatientScore();
        }

        //�鿴��ʷ����
        private void OpenSelectedDocument()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataTableViewRow selectedRow = this.dataGridView1.SelectedRows[0];
            if (selectedRow == null)
                return;
            MedicalQcMsg questionInfo = selectedRow.Tag as MedicalQcMsg;
            if (questionInfo == null)
                return;
            this.MainForm.OpenHistoryDocument(questionInfo);

        }


        /// <summary>
        /// ��ȡ���̼�¼����ѡ�нڵ�Ĳ����ĵ���Ϣ
        /// </summary>
        /// <param name="szDocTitle">�����ĵ�����</param>
        /// <param name="szDocSetID">�����ĵ�ID</param>
        /// <param name="szCreatorName">��������������</param>
        private short GetSelectedNodeDocInfo(ref MedDocInfo meddocInfo)
        {
            if (meddocInfo == null)
                meddocInfo = new MedDocInfo();
            short shRet = SystemData.ReturnValue.OK;
            if (this.MainForm.DocumentListForm == null)
                return SystemData.ReturnValue.FAILED;
            if (this.MainForm.DocumentListForm.SelectedNode == null)
            {
                MessageBoxEx.Show("û��ѡ�в����������κ�һ�ݲ������޷�����ʼ����⣡", MessageBoxIcon.Warning);
                return SystemData.ReturnValue.FAILED;
            }
            VirtualNode selectedNode = this.MainForm.DocumentListForm.SelectedNode;
            MDSDBLib.MedDocInfo docInfo = selectedNode.Data as MDSDBLib.MedDocInfo;
            if (docInfo != null)
            {
                meddocInfo.DOC_TITLE = docInfo.DocTitle;
                if (docInfo.FileType != "BAODIAN" && docInfo.FileType != "CHENPAD" && docInfo.FileType != "HEREN")
                    meddocInfo.DOC_SETID = string.Empty;
                else
                    meddocInfo.DOC_SETID = docInfo.DocSetID;
                meddocInfo.CREATOR_NAME = docInfo.CreatorName;
                meddocInfo.CREATOR_ID = docInfo.CreatorID;
                meddocInfo.DEPT_CODE = docInfo.DeptCode;
                meddocInfo.DEPT_NAME = docInfo.DeptName;
            }
            else if (selectedNode.Data.Equals(COMBIN_NODE_TAG))
                meddocInfo.DOC_TITLE = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�Ĳ���");
            else if (selectedNode.Data.Equals(DOCTOR_NODE_TAG))
                meddocInfo.DOC_TITLE = "ҽ��д�Ĳ���";
            else if (selectedNode.Data.Equals(UNKNOWN_NODE_TAG))
                meddocInfo.DOC_TITLE = "δ������Ĳ���";
            else
                meddocInfo.DOC_TITLE = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�������ɲ���");
            return shRet;
        }

        /// <summary>
        /// �жϵ�ǰ��½�û��Ƿ���Ȩ������ʼ�����
        /// </summary>
        /// <returns></returns>
        private bool HasRightAddItem()
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return false;

            if (SystemParam.Instance.UserRight == null)
                return false;


            return true;
        }


        private void toolbtnNew_Click(object sender, EventArgs e)
        {
            this.AddNewQuestion();
        }

        private void AddNewQuestion()
        {
            try
            {
                if (SystemParam.Instance.PatVisitInfo == null)
                {
                    MessageBoxEx.Show("��ѡ��һ�����ߣ�", MessageBoxIcon.Warning);
                    return;
                }
                if (SystemParam.Instance.LocalConfigOption.DefaultEditor == "2")
                {
                    if (this.MainForm.PatientPageForm == null)
                    {
                        MessageBoxEx.ShowMessage("��ѡ���ʿصĻ��߲���");
                        return;
                    }
                    DockContentBase content = null;
                    this.MainForm.PatientPageForm.GetActiveContent(ref content);
                    if (content == null)
                    {
                        MessageBoxEx.ShowMessage("��ѡ���ʼ����ݣ�");
                        return;
                    }
                    if (content is DocumentListNewForm)
                    {
                        DocumentListNewForm frm = content as DocumentListNewForm;
                        HerenDocForm document = null;
                        frm.GetActiveDocument(ref document);
                        if (document == null)
                            this.AddNewItem("��������", null);
                        else
                            this.AddNewItem(null, document.Document);
                    }
                    else
                    {
                        this.AddNewItem(content.Text + "�ʿ�", null);
                    }
                }
                else
                {
                    IDocumentForm documentForm = this.MainForm.ActiveDocument as IDocumentForm;
                    DockContentBase ActiveDocument = this.MainForm.ActiveDocument as DockContentBase;
                    //�ĵ����͵��ʿ�
                    if (documentForm != null)
                    {
                        this.AddNewItem("��������", documentForm.Documents[0]);
                    }
                    else if (ActiveDocument != null)
                    {
                        this.AddNewItem(ActiveDocument.Text + "�ʿ�", null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.ShowError("����ʼ�����ʧ�ܣ�",ex.ToString());
                return;
            }
        }

        /// <summary>
        /// ˢ�´˻��߲�������
        /// </summary>
        private void UpdatePatientScore()
        {
            this.MainForm.OnPatientScoreChanged(EventArgs.Empty);
        }

        private void toolbtnModify_Click(object sender, EventArgs e)
        {

            this.ModifySelectedItem();
        }

        private void toolbtnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedItem();
        }

        private void toolbtnSave_Click(object sender, EventArgs e)
        {
            this.CommitModify();
        }

        private void toolbtnPass_Click(object sender, EventArgs e)
        {

            this.SetMedRecordPassed();
        }

        private void mnuAddItem_Click(object sender, EventArgs e)
        {
            this.AddNewQuestion();
        }

        private void mnuModifyItem_Click(object sender, EventArgs e)
        {
            this.ModifySelectedItem();
        }

        private void mnuDeleteItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedItem();
        }

        private void mnuOpenDoc_Click(object sender, EventArgs e)
        {
            this.OpenSelectedDocument();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            this.ModifySelectedItem();
        }

        private void toolbtnRollback_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// ��ʾ��ǰ�������ʼ����⣬Ϊ����ʾȫ��
        /// </summary>
        /// <param name="docInfo"></param>
        public void ShowQuestionListByDocInfo(MedDocInfo docInfo)
        {
            if (docInfo == null)
            {
                this.chbShowAll.Checked = true;
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    if (row.Visible == false)
                        row.Visible = true;
                }
            }
            else
            {
                this.chbShowAll.Checked = false;
                szCurrentDocSetID = docInfo.DOC_SETID;
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    MedicalQcMsg medicalQcMsg = row.Tag as MedicalQcMsg;
                    if (medicalQcMsg == null)
                        continue;
                    if (medicalQcMsg.TOPIC_ID == docInfo.DOC_SETID)
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }

        }
        private string szCurrentDocSetID = string.Empty;
        private void chbShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chbShowAll.Checked == true)
            {
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    if (row.Visible == false)
                        row.Visible = true;
                }
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    MedicalQcMsg medicalQcMsg = row.Tag as MedicalQcMsg;
                    if (medicalQcMsg == null)
                        continue;
                    if (medicalQcMsg.TOPIC_ID == szCurrentDocSetID)
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }
        }

        private void toolbtnQCPass_Click(object sender, EventArgs e)
        {

            this.PassSelectedItem();
        }

        /// <summary>
        /// ��ѡ�е��������óɺϸ�
        /// </summary>
        private void PassSelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            if (SystemParam.Instance.UserRight == null)
                return;

            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            if (row.Cells[this.colQaEventType.Index].Value == null)
                return;

            MedicalQcMsg medicalQcMsg = row.Tag as MedicalQcMsg;
            if (medicalQcMsg == null)
                return;
            if (medicalQcMsg.MSG_STATUS == 3)
            {
                MessageBoxEx.Show("��ǰ�ʼ������Ѿ��Ǻϸ�״̬��", MessageBoxIcon.Warning);
                return;
            }
            else if (medicalQcMsg.MSG_STATUS != 2)
            {
                MessageBoxEx.Show("��ǰ�ʼ�����ҽ��δ�޸�,�������޸ĳɺϸ�״̬��", MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBoxEx.Show("�����Ҫ�޸ĵ�ǰ���ʼ�����Ϊ�ϸ���?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return;
            medicalQcMsg.MSG_STATUS = 3;
            short shRet = MedicalQcMsgAccess.Instance.Update(medicalQcMsg);
            if (shRet == SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�ʼ��������޸�Ϊ�ϸ�", MessageBoxIcon.Information);
                this.SetRowData(row, medicalQcMsg);
                return;
            }
            else
            {
                MessageBoxEx.Show("�ʼ������޸�ʧ�ܣ�");
                return;
            }
        }

        private void toolbthQChat_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBoxEx.Show("��ѡ��һ���ʼ����⣡");
                return;
            }
            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            if (row.Cells[this.colQaEventType.Index].Value == null)
                return;

            MedicalQcMsg questionInfo = row.Tag as MedicalQcMsg;
            if (questionInfo == null)
                return;

            StringBuilder sbArgs = new StringBuilder();
            string sbContent = GetMsgContent(questionInfo);
            UserInfo u = GetLinstenID(questionInfo);
            sbArgs.AppendFormat("{0};{1};{2}", SystemParam.Instance.UserInfo.USER_ID, u == null ? "" : u.USER_ID, sbContent);

            IntPtr fromHandle = GlobalMethods.Win32.GetSystemHandle(SystemData.MappingName.QUESTION_CHAT_CLIENT_SYS);
            IntPtr intParam = GlobalMethods.Win32.StringToPtr(sbArgs.ToString());
            NativeMethods.User32.ShowWindow(fromHandle, NativeConstants.SW_RESTORE);
            NativeMethods.User32.SetForegroundWindow(fromHandle);
            NativeMethods.User32.SendMessage(fromHandle, NativeConstants.WM_COPYDATA, IntPtr.Zero, intParam);
        }
        private UserInfo GetLinstenID(MedicalQcMsg questionInfo)
        {
            UserInfo u = null;
            if (questionInfo == null || string.IsNullOrEmpty(questionInfo.DOCTOR_IN_CHARGE))
            {
                u = ListUserInfo.Find(i => i.USER_NAME == SystemParam.Instance.PatVisitInfo.INCHARGE_DOCTOR);
            }
            else
            {
                u = ListUserInfo.Find(i => i.USER_NAME == questionInfo.DOCTOR_IN_CHARGE);
            }
            return u;
        }

        /// <summary>
        /// ��ȡ������Ϣ������
        /// </summary>
        /// <param name="medicalQcMsg"></param>
        /// <returns></returns>
        private string GetMsgContent(MedicalQcMsg medicalQcMsg)
        {
            if (medicalQcMsg == null)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("����:{0} סԺ��:{1}{2}", SystemParam.Instance.PatVisitInfo.PATIENT_NAME,
                                                   SystemParam.Instance.PatVisitInfo.INP_NO, Environment.NewLine);
            sb.AppendFormat("��������:{0}{1}", medicalQcMsg.TOPIC, Environment.NewLine);
            QcMsgDict qcMessageTemplet = m_lstQcMsgDict.Find(
              delegate (QcMsgDict q)
              {
                  return q.QC_MSG_CODE == medicalQcMsg.QC_MSG_CODE;
              }
              );
            sb.AppendFormat("��Ŀ:{0}{1}", qcMessageTemplet != null ? qcMessageTemplet.MESSAGE : "", Environment.NewLine);
            sb.AppendFormat("����:{0}{1}", medicalQcMsg.MESSAGE, Environment.NewLine);
            sb.AppendFormat("�۷�:{0}{1}", medicalQcMsg.POINT, Environment.NewLine);

            return sb.ToString();
        }
        private void mnuPassItem_Click(object sender, EventArgs e)
        {

            this.PassSelectedItem();
        }

        private void toolbtnLock_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBoxEx.ShowMessage("��ѡ���������������");
                return;
            }
            MedicalQcMsg medicalQcMsg = this.dataGridView1.SelectedRows[0].Tag as MedicalQcMsg;
            if (medicalQcMsg == null)
            {
                return;
            }
            short shRet = SystemData.ReturnValue.OK;
            if (medicalQcMsg.LOCK_STATUS)
            {
                shRet = MedicalQcMsgAccess.Instance.UpdateMesageLockStatus(false, medicalQcMsg.MSG_ID);
                if (shRet == SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.ShowMessage("��������ɹ�");
                    medicalQcMsg.LOCK_STATUS = false;
                    this.dataGridView1.SelectedRows[0].Cells[this.colLockStatus.Index].Value = Properties.Resources.empty;
                    this.toolbtnLock.Image = Properties.Resources._lock;
                    this.toolbtnLock.Text = "ǿ������";
                    return;
                }
                else
                {
                    MessageBoxEx.ShowError("�������ʧ��");
                }
            }

            if (MessageBoxEx.ShowConfirm("ҽ�������޸ĸ�������ܼ�����д��������ȷ��ǿ��������") != DialogResult.OK)
                return;
            shRet = MedicalQcMsgAccess.Instance.UpdateMesageLockStatus(true, medicalQcMsg.MSG_ID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowError("ǿ������ʧ��");
            }
            MessageBoxEx.ShowMessage("�����ɹ�");
            medicalQcMsg.LOCK_STATUS = true;
            this.dataGridView1.SelectedRows[0].Cells[this.colLockStatus.Index].Value = Properties.Resources._lock;
            this.toolbtnLock.Image = Properties.Resources.unlock;
            this.toolbtnLock.Text = "�������";
            return;
        }


        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            MedicalQcMsg medicalQcMsg = this.dataGridView1.Rows[e.RowIndex].Tag as MedicalQcMsg;
            if (medicalQcMsg == null)
                return;
            if (medicalQcMsg.LOCK_STATUS)
            {
                this.toolbtnLock.Image = Properties.Resources.unlock;
                this.toolbtnLock.Text = "�������";
            }
            else
            {
                this.toolbtnLock.Image = Properties.Resources._lock;
                this.toolbtnLock.Text = "ǿ������";
            }
        }

        private void toolbtnPrint_Click(object sender, EventArgs e)
        {
            this.m_iQTotalNum = this.dataGridView1.Rows.Count;
            this.m_iPrintedNum = 0;
            this.m_iPrintingPageIndex = 1;
            this.m_iTotalPageNum = (this.m_iQTotalNum / NUM_PER_PAGE) + (this.m_iQTotalNum % NUM_PER_PAGE == 0 ? 0 : 1);
            this.printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString("�ʿ������嵥", new Font(new FontFamily("����"), 18, FontStyle.Bold), System.Drawing.Brushes.Black, 300, 35);
            int i = 0, width = 740, x = 50, y = 90, fontSize = 10;
            if (this.m_iPrintingPageIndex == 1)
            {
                e.Graphics.DrawString(string.Format("��ӡ�ߣ�{0}    ��ӡʱ�䣺{1}", SystemParam.Instance.UserInfo.USER_NAME, DateTime.Now.ToString()), new Font(new FontFamily("����"), 10, FontStyle.Regular), System.Drawing.Brushes.Black, 400, 70);
            }
            else
            {
                y -= 10;
            }
            e.Graphics.DrawLine(Pens.Black, x, y, width, y);
            y += 10;
            for (; m_iPrintedNum < this.dataGridView1.Rows.Count && m_iPrintedNum < this.m_iPrintingPageIndex * NUM_PER_PAGE; m_iPrintedNum++)
            {
                i++;
                DataGridViewRow dvr = this.dataGridView1.Rows[m_iPrintedNum];
                MedicalQcMsg que = dvr.Tag as MedicalQcMsg;
                if (que != null && !string.IsNullOrEmpty(que.PATIENT_ID))
                {
                    string szLineText = (this.m_iPrintedNum + 1).ToString() + ". ";
                    szLineText += string.Format("���ʱ�䣺{0}    ����ˣ�{1}    ����״̬��{2}", que.ISSUED_DATE_TIME.ToString(), dvr.Cells[this.col_ISSUED_BY.Index].Value.ToString(), dvr.Cells[this.colMsgStatus.Index].Value.ToString());
                    e.Graphics.DrawString(szLineText, new Font(new FontFamily("����"), 8, FontStyle.Regular), System.Drawing.Brushes.Black, x, y);
                    y += 15;
                    e.Graphics.DrawString("���⣺" + que.MESSAGE, new Font(new FontFamily("����"), fontSize, FontStyle.Bold), System.Drawing.Brushes.Black, x, y);
                    y += 20;
                    e.Graphics.DrawLine(Pens.Black, x, y, width, y);
                    y += 10;
                }
            }
            this.m_iPrintingPageIndex++;
            if (this.m_iPrintingPageIndex - 1 < this.m_iTotalPageNum)
            {
                e.HasMorePages = true;
                LogManager.Instance.WriteLog("��ҳ��ӡִ�й���(if)��¼��" + e.HasMorePages.ToString());
            }
            else
            {
                e.HasMorePages = false;
                LogManager.Instance.WriteLog("��ҳ��ӡִ�й���(else)��¼��" + e.HasMorePages.ToString());
            }
        }

        private void printDocument1_BeginPrint(object sender, PrintEventArgs e)
        {
            this.printDocument1.DocumentName = "�ʿ�����";

            //����ֽ�Ż�ȡָ���Ĵ�ӡ��
            float fWidth = 210f;
            float fHeight = 297f;
            PaperInfo paperInfo = new PaperInfo(System.Drawing.Printing.PaperKind.A4, fWidth, fHeight);
            this.m_PrintName = GlobalMethods.Win32.GetSysDefaultPrinter();
            paperInfo.Printer = this.m_PrintName;

            SelectPrinterForm frmSelectPrinter = new SelectPrinterForm();
            frmSelectPrinter.SelectedPrinter = paperInfo.Printer;
            if (frmSelectPrinter.ShowDialog() != DialogResult.OK)
                return;

            if (!GlobalMethods.Misc.IsEmptyString(paperInfo.Printer) && paperInfo.Printer != this.m_PrintName)
                GlobalMethods.Win32.SetSysDefaultPrinter(paperInfo.Printer);
        }

        private void printDocument1_EndPrint(object sender, PrintEventArgs e)
        {
            //�ָ��û�PC�ϵ�Ĭ�ϴ�ӡ��,�Բ�Ӱ�����������ӡ
            if (!GlobalMethods.Misc.IsEmptyString(this.m_PrintName))
                GlobalMethods.Win32.SetSysDefaultPrinter(this.m_PrintName);
        }

        private void mnuNotPassItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBoxEx.Show("��ѡ����Ϊ���ϸ������");
                return;
            }
            if (MessageBoxEx.ShowConfirm("���������Ⲣδ�޸ģ�ȷ����������Ϊδ�޸Ĳ�����ҽ�������޸���") != DialogResult.OK)
                return;
            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            MedicalQcMsg questionInfo = this.dataGridView1.SelectedRows[0].Tag as MedicalQcMsg;
            if (questionInfo == null)
            {
                MessageBoxEx.Show("�ʿ���Ϣ�����ڣ��޷���������Ϊδ�޸�");
                return;
            }
            questionInfo.MSG_STATUS = 0;
            short shRet = MedicalQcMsgAccess.Instance.Update(questionInfo);
            if (shRet == SystemData.ReturnValue.OK)
            {
                //������״̬��ΪΪ�ʿ��˻�

                shRet = EmrDocAccess.Instance.UpdateDocSignCode(questionInfo.TOPIC_ID, SystemData.SignState.QC_ROLLBACK);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.Show("�����˻�ʧ��");
                    return;
                }
                MessageBoxEx.Show("�ʼ�������Ϊ���ϸ�ɹ���", MessageBoxIcon.Information);
                this.SetRowData(row, questionInfo);
                return;
            }
            else
            {
                MessageBoxEx.Show("�ʼ�����ʧ�ܣ�");
                return;
            }
        }
    }
}