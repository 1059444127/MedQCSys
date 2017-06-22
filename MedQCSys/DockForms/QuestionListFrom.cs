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

using MedQCSys.Dialogs;
using EMRDBLib.DbAccess;
using EMRDBLib;
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

        private Hashtable m_htMsgCodeTable = null;

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
            this.LoadQCQuestionList();
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
        private void LoadQCQuestionList()
        {
            this.dataGridView1.Rows.Clear();
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            if (this.m_htMsgCodeTable == null)
                this.m_htMsgCodeTable = new Hashtable();
            else
                this.m_htMsgCodeTable.Clear();
            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;

            List<EMRDBLib.MedicalQcMsg> lstQCQuestionInfos = null;
            short shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(szPatientID, szVisitID, ref lstQCQuestionInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�ʿ��ʼ���������ʧ�ܣ�");
                return;
            }
            if (lstQCQuestionInfos == null || lstQCQuestionInfos.Count <= 0)
                return;

            for (int index = 0; index < lstQCQuestionInfos.Count; index++)
            {
                EMRDBLib.MedicalQcMsg qcQuestionInfo = lstQCQuestionInfos[index];
                if (qcQuestionInfo == null)
                    continue;

                if (!this.m_htMsgCodeTable.ContainsKey(qcQuestionInfo.QC_MSG_CODE))
                    this.m_htMsgCodeTable.Add(qcQuestionInfo.QC_MSG_CODE, qcQuestionInfo);
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = qcQuestionInfo;
                this.SetRowData(row, qcQuestionInfo);
            }
            if (this.dataGridView1.Rows.Count > 0)
            {
                this.dataGridView1.ClearSelection();
            }
        }
        List<EMRDBLib.QcMsgDict> lstQCMessageTemplet = null;
        private void LoadQCMsgDict()
        {
            short shRet = QcMsgDictAccess.Instance.GetQcMsgDictList(ref lstQCMessageTemplet);
            if (shRet != SystemData.ReturnValue.OK && lstQCMessageTemplet == null)
                return;
            if (this.m_htMsgDict == null)
                this.m_htMsgDict = new Hashtable();
            for (int index = 0; index < lstQCMessageTemplet.Count; index++)
            {
                EMRDBLib.QcMsgDict messageTemplet = lstQCMessageTemplet[index];
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
        public bool CommitModify(string szDocTitle, string szDocSetID, string szCreatorName, byte[] byteDocData)
        {
            if (this.dataGridView1.Rows.Count <= 0)
                return true;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            int count = 0;
            short shRet = SystemData.ReturnValue.OK;
            for (int index = 0; index < this.dataGridView1.Rows.Count; index++)
            {
                DataTableViewRow row = this.dataGridView1.Rows[index];
                //ȡ����������е�Tag����,�жϵ�ǰ���½����޸Ļ���ɾ��״̬��
                string szRowState = (string)row.Cells[this.colQCEventType.Index].Tag;
                if (szRowState == STATUS_NORMAL)
                    continue;

                shRet = this.SaveRowData(row, szRowState, szDocTitle, szDocSetID, szCreatorName, byteDocData);
                if (shRet == SystemData.ReturnValue.OK)
                    count++;
                else if (shRet == SystemData.ReturnValue.FAILED)
                    break;
            }
            if (count > 0)
            {
                //hhhh
                //�����ʿ���Ա��������¼
                EMRDBLib.MedicalQcLog actionLog = new EMRDBLib.MedicalQcLog();
                bool bOK = QuestionListForm.MakeQCActionLog(ref actionLog);

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
                            actionLog.DOC_SETID = szDocID;
                            LogManager.Instance.WriteLog(szDocID);
                        }
                    }
                    else
                        actionLog.DOC_SETID = document.Document.DOC_ID;
                }
                else
                    actionLog.DOC_SETID = szDocSetID;
                if (bOK)
                {
                    actionLog.LOG_DESC = "�ʿ�������ʼ�����";

                    this.SaveQCActionLog(actionLog);
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
            row.Cells[this.colQCEventType.Index].Value = medicalQcMsg.QA_EVENT_TYPE;
            row.Cells[this.colQCEventType.Index].Tag = STATUS_NORMAL;
            if (this.m_htMsgDict.Contains(medicalQcMsg.QC_MSG_CODE))
            {
                EMRDBLib.QcMsgDict qcMessage = this.m_htMsgDict[medicalQcMsg.QC_MSG_CODE] as EMRDBLib.QcMsgDict;
                if (qcMessage == null)
                    return false;

                row.Cells[this.colMessage.Index].Value = string.IsNullOrEmpty(qcMessage.MESSAGE_TITLE) ? qcMessage.QA_EVENT_TYPE : qcMessage.MESSAGE_TITLE;
                row.Cells[this.colScore.Index].Value =
                        medicalQcMsg.POINT;
                row.Cells[this.colScore.Index].Tag =
                medicalQcMsg.POINT;
            }
            row.Cells[this.colMessage.Index].Tag = medicalQcMsg.QC_MSG_CODE;
            if (medicalQcMsg.ISSUED_DATE_TIME != medicalQcMsg.DefaultTime)
                row.Cells[this.colCheckTime.Index].Value = medicalQcMsg.ISSUED_DATE_TIME.ToString("yyyy-M-d HH:mm");
            if (medicalQcMsg.ASK_DATE_TIME != medicalQcMsg.DefaultTime)
                row.Cells[this.colConfirmTime.Index].Value = medicalQcMsg.ASK_DATE_TIME.ToString("yyyy-M-d HH:mm");
            row.Cells[this.colDocTitle.Index].Value = medicalQcMsg.TOPIC;
            row.Cells[this.colDocTitle.Index].Tag = medicalQcMsg.TOPIC_ID;
            row.Cells[this.colFeedBackInfo.Index].Value = medicalQcMsg.DOCTOR_COMMENT;
            row.Cells[this.colDetailInfo.Index].Value = medicalQcMsg.MESSAGE;
            row.Cells[this.colCreator.Index].Value = medicalQcMsg.DOCTOR_IN_CHARGE;
            row.Cells[this.colDept_Code.Index].Value = medicalQcMsg.DEPT_STAYED;
            //�ʼ���������ʾȨ�ޣ���Ȩ���������Լ���ӵ�
            //��ʾȨ�޸ĵ��ʿ�Ȩ�޿���
            row.Cells[this.colIssusdBy.Index].Value = medicalQcMsg.ISSUED_BY;

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
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="qcQuestionInfo">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref EMRDBLib.MedicalQcMsg qcQuestionInfo)
        {
            if (row == null || row.Index < 0)
            {
                MessageBoxEx.Show("�ʿ��ʼ����������зǷ��������ã�");
                return false;
            }

            if (SystemParam.Instance.UserInfo == null)
            {
                MessageBoxEx.Show("����û�е�¼ϵͳ��");
                return false;
            }

            if (SystemParam.Instance.PatVisitInfo == null)
            {
                MessageBoxEx.Show("��û��ѡ���κλ��ߣ�");
                return false;
            }

            EMRDBLib.MedicalQcMsg oldQCQuestionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
            if (oldQCQuestionInfo == null)
            {
                MessageBoxEx.Show("�ʿ��ʼ�������������ϢΪ�գ�");
                return false;
            }
            string szQCEventType = (string)row.Cells[this.colQCEventType.Index].Value;
            if (GlobalMethods.Misc.IsEmptyString(szQCEventType))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colQCEventType.Index];
                MessageBoxEx.Show("������ѡ�����������ͣ�");
                return false;
            }

            string szMessageCode = (string)row.Cells[this.colMessage.Index].Tag;
            string szMessageContent = (string)row.Cells[this.colDetailInfo.Index].Value;
            if (GlobalMethods.Misc.IsEmptyString(szMessageCode)
                || GlobalMethods.Misc.IsEmptyString(szMessageContent))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colMessage.Index];
                MessageBoxEx.Show("�����������ʼ����⣡");
                return false;
            }

            if (qcQuestionInfo == null)
                qcQuestionInfo = new EMRDBLib.MedicalQcMsg();
            qcQuestionInfo.MSG_ID = oldQCQuestionInfo.MSG_ID;
            qcQuestionInfo.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            qcQuestionInfo.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            qcQuestionInfo.VISIT_NO = SystemParam.Instance.PatVisitInfo.VISIT_NO;
            //ת�ƺ�BUG�޸�
            //����ȡ��д����ʱ���ڵĿ���
            qcQuestionInfo.DEPT_STAYED = row.Cells[this.colDept_Code.Index].Value != null ? row.Cells[this.colDept_Code.Index].Value.ToString() : string.Empty;
            //DoctorInChargeȡ������
            qcQuestionInfo.DOCTOR_IN_CHARGE = row.Cells[this.colCreator.Index].Value != null ? row.Cells[this.colCreator.Index].Value.ToString() : string.Empty;
            qcQuestionInfo.PARENT_DOCTOR = SystemParam.Instance.PatVisitInfo.AttendingDoctor;
            qcQuestionInfo.SUPER_DOCTOR = SystemParam.Instance.PatVisitInfo.SUPER_DOCTOR;
            qcQuestionInfo.QA_EVENT_TYPE = szQCEventType;
            qcQuestionInfo.QC_MSG_CODE = szMessageCode;
            qcQuestionInfo.MESSAGE = szMessageContent;
            if (oldQCQuestionInfo.MSG_STATUS != 0)
                qcQuestionInfo.MSG_STATUS = oldQCQuestionInfo.MSG_STATUS;
            else
                qcQuestionInfo.MSG_STATUS = 0;
            qcQuestionInfo.ISSUED_BY = SystemParam.Instance.UserInfo.Name;
            qcQuestionInfo.ISSUED_DATE_TIME = oldQCQuestionInfo.ISSUED_DATE_TIME;
            qcQuestionInfo.ISSUED_ID = SystemParam.Instance.UserInfo.ID;
            qcQuestionInfo.POINT_TYPE = 0;
            qcQuestionInfo.QCDOC_TYPE = oldQCQuestionInfo.QCDOC_TYPE;
            qcQuestionInfo.ISSUED_TYPE = SystemData.IssuedType.NORMAL;
            if (row.Cells[this.colScore.Index].Tag != null)
                qcQuestionInfo.POINT = float.Parse(row.Cells[this.colScore.Index].Tag.ToString());
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
        private short SaveRowData(DataTableViewRow row, string szRowState, string szDocTitle, string szDocSetID
            , string szCreatorName, byte[] byteDocData)
        {
            DateTime dtBeginTime1 = DateTime.Now;
            if (row == null || row.Index < 0)
                return SystemData.ReturnValue.FAILED;

            EMRDBLib.MedicalQcMsg qcQuestionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
            if (qcQuestionInfo == null)
                return SystemData.ReturnValue.FAILED;

            string szPatientID = qcQuestionInfo.PATIENT_ID;
            string szVisitID = qcQuestionInfo.VISIT_ID;
            string szQuestionCode = qcQuestionInfo.QC_MSG_CODE;
            DateTime dtCheckTime = qcQuestionInfo.ISSUED_DATE_TIME;
            qcQuestionInfo = null;
            if (!this.MakeRowData(row, ref qcQuestionInfo))
                return SystemData.ReturnValue.FAILED;

            //��������Ĵ����߲�Ϊ�գ��޸ľ���ҽ��������Ϊ�����ߣ�������ҽ��վ�����߾����յ��ʿط������ʼ����⡣
            if (!string.IsNullOrEmpty(szCreatorName))
                qcQuestionInfo.DOCTOR_IN_CHARGE = szCreatorName;

            short shRet = SystemData.ReturnValue.OK;
            if (szRowState == STATUS_DELETE)
            {
                if (!this.dataGridView1.IsNewRow(row))
                    shRet = MedicalQcMsgAccess.Instance.Delete(qcQuestionInfo.MSG_ID);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                this.dataGridView1.Rows.Remove(row);
                if (this.m_htMsgCodeTable.ContainsKey(qcQuestionInfo.QC_MSG_CODE))
                    this.m_htMsgCodeTable.Remove(qcQuestionInfo.QC_MSG_CODE);
            }
            else if (szRowState == STATUS_MODIFY)
            {
                DateTime dtNewCheckDate = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
                shRet = MedicalQcMsgAccess.Instance.Update(qcQuestionInfo);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                if (!string.IsNullOrEmpty(szDocSetID))
                {
                    qcQuestionInfo.TOPIC_ID = szDocSetID;
                    qcQuestionInfo.TOPIC = szDocTitle;
                    string szRemotePath = SystemParam.Instance.GetQCFtpDocPath(qcQuestionInfo, "smdf");
                    EmrDocAccess.Instance.UpdateQCDocToFtp(qcQuestionInfo, szRemotePath, dtNewCheckDate);
                }
                row.Cells[this.colQCEventType.Index].Tag = STATUS_NORMAL;
                row.Cells[this.colCheckTime.Index].Value = dtNewCheckDate.ToString("yyyy-M-d HH:mm");
                row.Cells[this.colScore.Index].Value = qcQuestionInfo.POINT;
                qcQuestionInfo.ISSUED_DATE_TIME = dtNewCheckDate;
                row.Tag = qcQuestionInfo;
                if (!this.m_htMsgCodeTable.ContainsKey(qcQuestionInfo.QC_MSG_CODE))
                    this.m_htMsgCodeTable.Add(qcQuestionInfo.QC_MSG_CODE, qcQuestionInfo);
            }
            else if (szRowState == STATUS_NEW)
            {
                qcQuestionInfo.QC_MODULE = "DOCTOR_MR";
                qcQuestionInfo.TOPIC_ID = szDocSetID;
                qcQuestionInfo.TOPIC = szDocTitle;
                qcQuestionInfo.MSG_STATUS = 0;
                shRet = MedicalQcMsgAccess.Instance.Insert(qcQuestionInfo);


                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Cells[this.colCheckTime.Index].Value = qcQuestionInfo.ISSUED_DATE_TIME.ToString("yyyy-M-d HH:mm");
                row.Cells[this.colDocTitle.Index].Value = szDocTitle;
                row.Cells[this.colDocTitle.Index].Tag = szDocSetID;
                row.Cells[this.colDetailInfo.Index].Value = qcQuestionInfo.MESSAGE;
                row.Cells[this.colScore.Index].Value = qcQuestionInfo.POINT;
                row.Cells[this.colQCEventType.Index].Tag = STATUS_NORMAL;
                row.Tag = qcQuestionInfo;
                if (!this.m_htMsgCodeTable.ContainsKey(qcQuestionInfo.QC_MSG_CODE))
                    this.m_htMsgCodeTable.Add(qcQuestionInfo.QC_MSG_CODE, qcQuestionInfo);
                //�Ѳ������浽FTP��
                if (byteDocData != null)
                {
                    string szRemotePath = SystemParam.Instance.GetQCFtpDocPath(qcQuestionInfo, "smdf");
                    DateTime dtBeginTime = DateTime.Now;
                    shRet = EmrDocAccess.Instance.SaveQCDocToFTP(qcQuestionInfo.TOPIC_ID, qcQuestionInfo.ISSUED_DATE_TIME, szRemotePath, byteDocData);
                    DateTime dtEndTime = DateTime.Now;
                    LogManager.Instance.WriteLog(DateDiff("�����ĵ��浽ftp", dtBeginTime, dtEndTime));
                }

            }
            DateTime dtEndTime1 = DateTime.Now;
            LogManager.Instance.WriteLog(DateDiff("�����ʿ���Ϣ", dtBeginTime1, dtEndTime1));
            row.State = RowState.Normal;
            return SystemData.ReturnValue.OK;
        }

        private string DateDiff(string szMessage, DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            TimeSpan ts = DateTime1.Subtract(DateTime2).Duration();
            dateDiff = szMessage + "��ʱ" + ts.Minutes.ToString() + "����" + ts.Seconds.ToString() + "��" + ts.Milliseconds + "����";
            return dateDiff;
        }
        /// <summary>
        /// ����һ���������������־
        /// </summary>
        /// <param name="qcActionLog">���ɵļ����־��Ϣ</param>
        /// <returns>bool</returns>
        public static bool MakeQCActionLog(ref EMRDBLib.MedicalQcLog qcActionLog)
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return false;
            if (qcActionLog == null)
                qcActionLog = new EMRDBLib.MedicalQcLog();
            qcActionLog.CHECK_DATE = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
            if (!GlobalMethods.Misc.IsEmptyString(SystemParam.Instance.PatVisitInfo.DEPT_CODE))
                qcActionLog.DEPT_STAYED = SystemParam.Instance.PatVisitInfo.DEPT_CODE;
            else
                qcActionLog.DEPT_STAYED = SystemParam.Instance.PatVisitInfo.DischargeDeptCode;
            qcActionLog.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            qcActionLog.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            qcActionLog.CHECKED_BY = SystemParam.Instance.UserInfo.Name;
            qcActionLog.CHECKED_ID = SystemParam.Instance.UserInfo.ID;
            qcActionLog.DEPT_CODE = SystemParam.Instance.UserInfo.DeptCode;
            qcActionLog.DEPT_NAME = SystemParam.Instance.UserInfo.DeptName;
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
        private bool SaveQCActionLog(EMRDBLib.MedicalQcLog qcActionLog)
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
            List<EMRDBLib.MedicalQcMsg> lstQCQuestionInfos = null;
            short shRet = SystemData.ReturnValue.OK;
            shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(szPatientID, szVisitID, ref lstQCQuestionInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯ�ʿ��ʼ������б�ʱ����,�޷����Ϊ�ϸ�");
                return false;
            }
            if (lstQCQuestionInfos == null || lstQCQuestionInfos.Count <= 0)
                return true;
            for (int index = 0; index < lstQCQuestionInfos.Count; index++)
            {
                EMRDBLib.MedicalQcMsg qcQuestionInfo = lstQCQuestionInfos[index];
                if (qcQuestionInfo.MSG_STATUS.ToString() != MDSDBLib.SystemData.QCQuestionStatus.PASSCODE)//�ʼ����ⶼ�����Ϊ�ϸ�
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


            EMRDBLib.MedicalQcLog qcActionLog = new EMRDBLib.MedicalQcLog();
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
        /// <summary>
        /// ����һ�м�¼
        /// </summary>
        /// <param name="szDocTitle">��������</param>
        /// <param name="szDocSetID">�����ĵ���ID</param>
        /// <param name="szCreatorName">����������</param>
        /// <param name="szDeptCode">���������ڿ���</param>
        /// <param name="byteDocData">�ĵ�����</param>
        internal void AddNewItem(string szDocTitle, string szDocSetID, string szCreatorName, string szDeptCode, byte[] byteDocData)
        {
            if (!HasRightAddItem())
                return;

            //��������
            EMRDBLib.MedicalQcMsg qcQuestionInfo = new EMRDBLib.MedicalQcMsg();
            qcQuestionInfo.ISSUED_DATE_TIME = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
            if (szDocTitle == null && szDocSetID == null && szCreatorName == null && szDeptCode == null)
            {
                short shRet = this.GetSelectedNodeDocInfo(ref szDocTitle, ref szDocSetID, ref szCreatorName, ref szDeptCode, ref byteDocData);
                if (shRet != SystemData.ReturnValue.OK)
                    return;
            }
            //������
            int index = this.dataGridView1.Rows.Add();
            DataTableViewRow row = this.dataGridView1.Rows[index];
            row.Tag = qcQuestionInfo;
            SelectQuestionForm questionTypeForm = new SelectQuestionForm();
            questionTypeForm.QCMessageTemplet = null;
            questionTypeForm.DocTitle = szDocTitle;
            questionTypeForm.Text = "�ʼ���������";
            questionTypeForm.QCCheckTime = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
            if (questionTypeForm.ShowDialog() != DialogResult.OK)
            {
                this.dataGridView1.Rows.Remove(row);
                return;
            }
            this.SetSelectedRowValues(row, questionTypeForm.QCMessageTemplet);
            this.dataGridView1.CurrentCell = row.Cells[this.colConfirmTime.Index];
            row.Cells[this.colQCEventType.Index].Tag = STATUS_NEW;
            row.Cells[this.colMsgStatus.Index].Value = "δ����";
            row.Cells[this.colDept_Code.Index].Value = string.IsNullOrEmpty(szDeptCode) ? SystemParam.Instance.PatVisitInfo.DEPT_CODE : szDeptCode;
            row.Cells[this.colCreator.Index].Value = string.IsNullOrEmpty(szCreatorName) ? SystemParam.Instance.PatVisitInfo.INCHARGE_DOCTOR : szCreatorName; ;
            this.CommitModify(szDocTitle, szDocSetID, szCreatorName, byteDocData);
            this.UpdatePatientScore();
        }

        /// <summary>
        ///�޸�ѡ��������
        /// </summary>
        private void ModifySelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            if (SystemParam.Instance.UserRight == null)
                return;

            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            if (row.Cells[this.colQCEventType.Index].Value == null)
                return;

            if (row.Cells[this.colConfirmTime.Index].Value != null)
            {
                MessageBoxEx.Show("���ʼ������Ѿ���ȷ�ϣ����������޸ģ�", MessageBoxIcon.Warning);
                return;
            }
            EMRDBLib.MedicalQcMsg questionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
            if (questionInfo == null)
                return;

            if (questionInfo.ISSUED_BY != SystemParam.Instance.UserInfo.Name
                && !SystemConfig.Instance.Get(SystemData.ConfigKey.MODIFY_OR_DELETE_QUESTION, false))
            {
                MessageBoxEx.Show("���ʼ����ⲻ������ӵģ����������޸ģ�", MessageBoxIcon.Warning);
                return;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            row.Cells[this.colQCEventType.Index].Tag = STATUS_MODIFY;
            EMRDBLib.QcMsgDict messageTemplet = new EMRDBLib.QcMsgDict();
            messageTemplet.QA_EVENT_TYPE = row.Cells[this.colQCEventType.Index].Value.ToString();
            messageTemplet.MESSAGE = row.Cells[this.colDetailInfo.Index].Value.ToString();
            if (row.Cells[this.colMessage.Index].Value != null)
                messageTemplet.MESSAGE_TITLE = row.Cells[this.colMessage.Index].Value.ToString();
            messageTemplet.QC_MSG_CODE = row.Cells[this.colMessage.Index].Tag.ToString();
            if (row.Cells[this.colScore.Index].Value != null)
                messageTemplet.SCORE = float.Parse(row.Cells[this.colScore.Index].Value.ToString());
            messageTemplet.QCDocType = questionInfo.QCDOC_TYPE;
            SelectQuestionForm frmQuestion = new SelectQuestionForm();
            frmQuestion.QCMessageTemplet = messageTemplet;
            frmQuestion.QCAskDateTime = (string)row.Cells[this.colConfirmTime.Index].Value;
            frmQuestion.DoctorComment = (string)row.Cells[this.colFeedBackInfo.Index].Value;
            frmQuestion.Text = "�ʼ������ѯ�޸�";
            frmQuestion.DocTitle = (string)row.Cells[this.colDocTitle.Index].Value;
            if (row.Cells[this.colCheckTime.Index].Value != null)
                frmQuestion.QCCheckTime = DateTime.Parse(row.Cells[this.colCheckTime.Index].Value.ToString());
            if (frmQuestion.ShowDialog() != DialogResult.OK)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.SetSelectedRowValues(row, frmQuestion.QCMessageTemplet);
            string szDocTitle = (string)row.Cells[this.colDocTitle.Index].Value;
            string szDocSetID = (string)row.Cells[this.colDocTitle.Index].Tag;
            string szCreatorName = (string)row.Cells[this.colCheckTime.Index].Tag;
            this.CommitModify(szDocTitle, szDocSetID, szCreatorName, null);
            this.UpdatePatientScore();
            this.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
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
            EMRDBLib.MedicalQcMsg questionInfo = selectedRow.Tag as EMRDBLib.MedicalQcMsg;
            if (questionInfo == null)
                return;
            
                if ( selectedRow.Cells[this.colConfirmTime.Index].Value != null)
                {
                    MessageBoxEx.Show("���ʼ������ѱ�ȷ�ϣ�����ɾ����", MessageBoxIcon.Warning);
                    return;
                }
                if (questionInfo.ISSUED_BY != SystemParam.Instance.UserInfo.Name
                    && !SystemConfig.Instance.Get(SystemData.ConfigKey.MODIFY_OR_DELETE_QUESTION, false))
                {
                    MessageBoxEx.Show("���ʼ����ⲻ������ӵģ���������ɾ����", MessageBoxIcon.Warning);
                    return;
                }
           

            selectedRow.Cells[this.colQCEventType.Index].Tag = STATUS_DELETE;
            string szDocTitle = (string)selectedRow.Cells[this.colDocTitle.Index].Value;
            string szDocSetID = (string)selectedRow.Cells[this.colDocTitle.Index].Tag;
            string szCreatorName = (string)selectedRow.Cells[this.colCheckTime.Index].Tag;
            this.CommitModify(szDocTitle, szDocSetID, szCreatorName, null);
            this.UpdatePatientScore();
        }

        //�鿴��ʷ����
        private void OpenSelectedDocument()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataTableViewRow selectedRow = this.dataGridView1.SelectedRows[0];
            if (selectedRow == null)
                return;
            EMRDBLib.MedicalQcMsg questionInfo = selectedRow.Tag as EMRDBLib.MedicalQcMsg;
            if (questionInfo == null)
                return;
            this.MainForm.OpenHistoryDocument(questionInfo);

        }

        /// <summary>
        /// ��ѡ���еĸ����и�ֵ
        /// </summary>
        /// <param name="messageTemplet">�ʼ�����ģ�����</param>
        private void SetSelectedRowValues(DataTableViewRow selectedRow, EMRDBLib.QcMsgDict messageTemplet)
        {
            if (messageTemplet == null)
                return;
            if (m_htMsgDict == null)
                return;

            selectedRow.Cells[this.colQCEventType.Index].Value = messageTemplet.QA_EVENT_TYPE;
            if (this.m_htMsgDict.ContainsKey(messageTemplet.QC_MSG_CODE))
                selectedRow.Cells[this.colMessage.Index].Value = string.IsNullOrEmpty(messageTemplet.MESSAGE_TITLE) ?
                    this.m_htMsgDict[messageTemplet.QC_MSG_CODE] : messageTemplet.MESSAGE_TITLE;
            selectedRow.Cells[this.colMessage.Index].Tag = messageTemplet.QC_MSG_CODE;
            selectedRow.Cells[this.colDetailInfo.Index].Value = messageTemplet.MESSAGE;
            selectedRow.Cells[this.colScore.Index].Value = Math.Round(new decimal(GlobalMethods.Convert.StringToValue(messageTemplet.SCORE, 0f)), 1).ToString("F1");
            selectedRow.Cells[this.colScore.Index].Tag = Math.Round(new decimal(GlobalMethods.Convert.StringToValue(messageTemplet.SCORE, 0f)), 1).ToString("F1");
            EMRDBLib.MedicalQcMsg qcQuestionInfo = selectedRow.Tag as EMRDBLib.MedicalQcMsg;
            if (qcQuestionInfo != null)
            {
                qcQuestionInfo.QCDOC_TYPE = messageTemplet.QCDocType;
            }
        }

        /// <summary>
        /// ��ȡ���̼�¼����ѡ�нڵ�Ĳ����ĵ���Ϣ
        /// </summary>
        /// <param name="szDocTitle">�����ĵ�����</param>
        /// <param name="szDocSetID">�����ĵ�ID</param>
        /// <param name="szCreatorName">��������������</param>
        private short GetSelectedNodeDocInfo(ref string szDocTitle, ref string szDocSetID, ref string szCreatorName, ref string szDeptCode, ref byte[] byteDocData)
        {
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
                szDocTitle = docInfo.DocTitle;
                if (docInfo.FileType != "BAODIAN" && docInfo.FileType != "CHENPAD" && docInfo.FileType != "HEREN")
                    szDocSetID = string.Empty;
                else
                    szDocSetID = docInfo.DocSetID;
                szCreatorName = docInfo.CreatorName;
                szDeptCode = docInfo.DeptCode;
                EmrDocAccess.Instance.GetDocByID(docInfo.DocID, ref byteDocData);
            }
            else if (selectedNode.Data.Equals(COMBIN_NODE_TAG))
                szDocTitle = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�Ĳ���");
            else if (selectedNode.Data.Equals(DOCTOR_NODE_TAG))
                szDocTitle = "ҽ��д�Ĳ���";
            else if (selectedNode.Data.Equals(NURSE_NODE_TAG))
                szDocTitle = "��ʿд�Ĳ���";
            else if (selectedNode.Data.Equals(UNKNOWN_NODE_TAG))
                szDocTitle = "δ������Ĳ���";
            else
                szDocTitle = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�������ɲ���");
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
            if (SystemParam.Instance.PatVisitInfo == null)
            {
                MessageBoxEx.Show("��ѡ��һ�����ߣ�", MessageBoxIcon.Warning);
                return;
            }
            //��ˢ���²����б�
            //if (this.MainForm.DocumentListForm.NeedRefreshView)
            //{
            //    this.MainForm.DocumentListForm.OnRefreshView();
            //}

            Document.IDocumentForm documentForm = this.MainForm.ActiveDocument as Document.IDocumentForm;
            DockForms.DockContentBase ActiveDocument = this.MainForm.ActiveDocument as DockForms.DockContentBase;

            //�ĵ����͵��ʿ�
            if (documentForm != null)
            {
                if (documentForm.Documents.Count == 1) //���ĵ���
                    this.AddNewItem(null, null, null, null, null);
                else if (documentForm.Documents.Count > 1) //�ϲ���
                    this.AddNewItem(ActiveDocument != null ? ActiveDocument.Text : "�ϲ��򿪲����ʿ�", null,
                        SystemParam.Instance.PatVisitInfo.INCHARGE_DOCTOR, SystemParam.Instance.PatVisitInfo.DEPT_CODE, null);
            }
            else if (ActiveDocument != null)
            {
                if (ActiveDocument.Text == "��������")//�����б������ʿ�
                    this.AddNewItem(null, null, null, null, null);
                else
                    this.AddNewItem(ActiveDocument.Text + "�ʿ�", null, SystemParam.Instance.PatVisitInfo.INCHARGE_DOCTOR, SystemParam.Instance.PatVisitInfo.DEPT_CODE, null);
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

        private void mnuSaveItems_Click(object sender, EventArgs e)
        {
            this.CommitModify();
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
                    EMRDBLib.MedicalQcMsg qcQuestionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
                    if (qcQuestionInfo == null)
                        continue;
                    if (qcQuestionInfo.TOPIC_ID == docInfo.DOC_SETID)
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
                    EMRDBLib.MedicalQcMsg qcQuestionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
                    if (qcQuestionInfo == null)
                        continue;
                    if (qcQuestionInfo.TOPIC_ID == szCurrentDocSetID)
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
            if (row.Cells[this.colQCEventType.Index].Value == null)
                return;

            EMRDBLib.MedicalQcMsg questionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
            if (questionInfo == null)
                return;
            if (questionInfo.MSG_STATUS == 3)
            {
                MessageBoxEx.Show("��ǰ�ʼ������Ѿ��Ǻϸ�״̬��", MessageBoxIcon.Warning);
                return;
            }
            else if (questionInfo.MSG_STATUS != 2)
            {
                MessageBoxEx.Show("��ǰ�ʼ�����ҽ��δ�޸�,�������޸ĳɺϸ�״̬��", MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBoxEx.Show("�����Ҫ�޸ĵ�ǰ���ʼ�����Ϊ�ϸ���?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return;
            questionInfo.MSG_STATUS = 3;
            short shRet = MedicalQcMsgAccess.Instance.Update(questionInfo);
            if (shRet == SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�ʼ��������޸�Ϊ�ϸ�", MessageBoxIcon.Information);
                this.SetRowData(row, questionInfo);
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
            if (row.Cells[this.colQCEventType.Index].Value == null)
                return;

            EMRDBLib.MedicalQcMsg questionInfo = row.Tag as EMRDBLib.MedicalQcMsg;
            if (questionInfo == null)
                return;

            StringBuilder sbArgs = new StringBuilder();
            string sbContent = GetMsgContent(questionInfo);
            UserInfo u = GetLinstenID(questionInfo);
            sbArgs.AppendFormat("{0};{1};{2}", SystemParam.Instance.UserInfo.ID, u == null ? "" : u.ID, sbContent);

            IntPtr fromHandle = GlobalMethods.Win32.GetSystemHandle(SystemData.MappingName.QUESTION_CHAT_CLIENT_SYS);
            IntPtr intParam = GlobalMethods.Win32.StringToPtr(sbArgs.ToString());
            NativeMethods.User32.ShowWindow(fromHandle, NativeConstants.SW_RESTORE);
            NativeMethods.User32.SetForegroundWindow(fromHandle);
            NativeMethods.User32.SendMessage(fromHandle, NativeConstants.WM_COPYDATA, IntPtr.Zero, intParam);
        }
        private UserInfo GetLinstenID(EMRDBLib.MedicalQcMsg questionInfo)
        {
            UserInfo u = null;
            if (questionInfo == null || string.IsNullOrEmpty(questionInfo.DOCTOR_IN_CHARGE))
            {
                u = ListUserInfo.Find(i => i.Name == SystemParam.Instance.PatVisitInfo.INCHARGE_DOCTOR);
            }
            else
            {
                u = ListUserInfo.Find(i => i.Name == questionInfo.DOCTOR_IN_CHARGE);
            }
            return u;
        }

        /// <summary>
        /// ��ȡ������Ϣ������
        /// </summary>
        /// <param name="questionInfo"></param>
        /// <returns></returns>
        private string GetMsgContent(EMRDBLib.MedicalQcMsg questionInfo)
        {
            if (questionInfo == null)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("����:{0} סԺ��:{1}{2}", SystemParam.Instance.PatVisitInfo.PATIENT_NAME,
                                                   SystemParam.Instance.PatVisitInfo.INP_NO, Environment.NewLine);
            sb.AppendFormat("��������:{0}{1}", questionInfo.TOPIC, Environment.NewLine);
            EMRDBLib.QcMsgDict qcMessageTemplet = lstQCMessageTemplet.Find(
              delegate (EMRDBLib.QcMsgDict q)
              {
                  return q.QC_MSG_CODE == questionInfo.QC_MSG_CODE;
              }
              );
            sb.AppendFormat("��Ŀ:{0}{1}", qcMessageTemplet != null ? qcMessageTemplet.MESSAGE : "", Environment.NewLine);
            sb.AppendFormat("����:{0}{1}", questionInfo.MESSAGE, Environment.NewLine);
            sb.AppendFormat("�۷�:{0}{1}", questionInfo.POINT, Environment.NewLine);

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
            EMRDBLib.MedicalQcMsg qcQuestionInfo = this.dataGridView1.SelectedRows[0].Tag as EMRDBLib.MedicalQcMsg;
            if (qcQuestionInfo == null)
            {
                return;
            }
            short shRet = SystemData.ReturnValue.OK;
            if (qcQuestionInfo.LOCK_STATUS)
            {
                shRet = MedicalQcMsgAccess.Instance.UpdateMesageLockStatus(false, qcQuestionInfo.MSG_ID);
                if (shRet == SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.ShowMessage("��������ɹ�");
                    qcQuestionInfo.LOCK_STATUS = false;
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
            shRet = MedicalQcMsgAccess.Instance.UpdateMesageLockStatus(true, qcQuestionInfo.MSG_ID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowError("ǿ������ʧ��");
            }
            MessageBoxEx.ShowMessage("�����ɹ�");
            qcQuestionInfo.LOCK_STATUS = true;
            this.dataGridView1.SelectedRows[0].Cells[this.colLockStatus.Index].Value = Properties.Resources._lock;
            this.toolbtnLock.Image = Properties.Resources.unlock;
            this.toolbtnLock.Text = "�������";
            return;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            EMRDBLib.MedicalQcMsg qcQuestionInfo = this.dataGridView1.Rows[e.RowIndex].Tag as EMRDBLib.MedicalQcMsg;
            if (qcQuestionInfo == null)
                return;
            if (qcQuestionInfo.LOCK_STATUS)
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

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            EMRDBLib.MedicalQcMsg qcQuestionInfo = this.dataGridView1.Rows[e.RowIndex].Tag as EMRDBLib.MedicalQcMsg;
            if (qcQuestionInfo == null)
                return;
            if (qcQuestionInfo.LOCK_STATUS)
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
                e.Graphics.DrawString(string.Format("��ӡ�ߣ�{0}    ��ӡʱ�䣺{1}", SystemParam.Instance.UserInfo.Name, DateTime.Now.ToString()), new Font(new FontFamily("����"), 10, FontStyle.Regular), System.Drawing.Brushes.Black, 400, 70);
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
                EMRDBLib.MedicalQcMsg que = dvr.Tag as EMRDBLib.MedicalQcMsg;
                if (que != null && !string.IsNullOrEmpty(que.PATIENT_ID))
                {
                    string szLineText = (this.m_iPrintedNum + 1).ToString() + ". ";
                    szLineText += string.Format("���ʱ�䣺{0}    ����ˣ�{1}    ����״̬��{2}", que.ISSUED_DATE_TIME.ToString(), dvr.Cells[this.colIssusdBy.Index].Value.ToString(), dvr.Cells[this.colMsgStatus.Index].Value.ToString());
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


        private string GetMsgStatusDesc(string statusCode)
        {
            switch (statusCode)
            {
                case "0":
                    return "δ����";
                case "1":
                    return "�ѽ���";
                case "2":
                    return "���޸�";
                case "3":
                    return "�Ѻϸ�";
                default:
                    return "δ����";
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

                shRet = EmrDocAccess.Instance.UpdateDocSignCode(questionInfo.TOPIC_ID, MedDocSys.DataLayer.SystemData.SignState.QC_ROLLBACK);
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