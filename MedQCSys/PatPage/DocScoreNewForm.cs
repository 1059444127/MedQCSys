// ***********************************************************
// �����ʿ�ϵͳ�ʼ�����Ի���.
// Creator:LiChunYing  Date:2012-02-09
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using MedDocSys.QCEngine.TimeCheck;
using Heren.Common.Report;
using EMRDBLib;
using EMRDBLib.DbAccess;
using System.Linq;
using Heren.MedQC.CheckPoint;

namespace MedQCSys.DockForms
{
    public partial class DocScoreNewForm : DockContentBase
    {

        private float m_fDocScore = 0;
        /// <summary>
        /// ��ȡ�����÷�
        /// </summary>
        public float DocScore
        {
            get { return this.m_fDocScore; }
        }



        private string m_szQcChecker = string.Empty;
        /// <summary>
        /// ��ȡ���ּ����
        /// </summary>
        public string QcChecker
        {
            get { return this.m_szQcChecker; }
        }
        private bool m_NeedSave = false;
        /// <summary>
        /// �Ƿ���Ҫ���没������
        /// </summary>
        public bool NeedSave
        {
            get { return m_NeedSave; }
            set { m_NeedSave = value; }
        }

        public DocScoreNewForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.DockRight;
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.DockAreas = DockAreas.Document | DockAreas.DockBottom | DockAreas.DockLeft
                | DockAreas.DockRight | DockAreas.DockTop;
            //this.dgvHummanScore.Font = new Font("����", 10.5f);
            //this.dgvSystemScore.Font = new Font("����", 10.5f);
        }

        public DocScoreNewForm(MainForm parent, PatPage.PatientPageControl patientPageControl)
            : base(parent, patientPageControl)
        {
            this.InitializeComponent();
            if (SystemParam.Instance.LocalConfigOption.IsScoreRightShow)
                this.ShowHint = DockState.DockRight;
            else
            {
                this.colItem.Width = 350;
                this.ShowHint = DockState.Document;

            }
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.DockAreas = DockAreas.Document | DockAreas.DockBottom | DockAreas.DockLeft
                | DockAreas.DockRight | DockAreas.DockTop;
            this.dgvHummanScore.Font = new Font("����", 10.5f);
            this.dgvSystemScore.Font = new Font("����", 10.5f);
            foreach (DataGridViewColumn item in this.dgvHummanScore.Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn item in this.dgvSystemScore.Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgvHummanScore.Columns[this.colItem.Index].DefaultCellStyle.WrapMode =
             DataGridViewTriState.True;
            //dgvHummanScore.AutoHeightForAllColumns = true;
            //�����Զ������߶�  
            //this.dgvHummanScore.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //this.dgvSystemScore.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //�����Զ������߶�  
            //this.dgvSystemScore.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        protected override void OnPatientScoreChanged()
        {
            base.OnPatientScoreChanged();
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.NeedRefreshView)
                this.OnRefreshView();
            this.NeedSave = true;
        }

        public override void OnRefreshView()
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);


            this.ShowStatusMessage("���ڷ����˹����۷����...");
            this.LoadHummanScoreInfos();
            this.LoadSystemScoreInfos();
            this.ShowStatusMessage(null);
            this.dgvSystemScore.Columns[this.col_2_Item.Index].Width = 350;
            this.dgvHummanScore.Columns[this.colItem.Index].Width = 350;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
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
        /// ͣ������任ʱ��д
        /// </summary>
        protected override void OnActiveContentChanged()
        {
            base.OnActiveContentChanged();
            if (this.IsHidden)
                return;
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this && this.NeedRefreshView)
                this.OnRefreshView();
        }


        /// <summary>
        /// �������б����Ϣ�ı�ʱ����
        /// </summary>
        [Description(" ���������ֱ���ɹ��󴥷�")]
        public event EventHandler HummanScoreSaved = null;
        internal virtual void OnHummanScoreSaved(System.EventArgs e)
        {
            if (this.HummanScoreSaved == null)
                return;
            try
            {
                this.HummanScoreSaved(this, e);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("DocScoreNewForm.OnHummanScoreSaved", ex);
            }
        }

        /// <summary>
        /// �����˹����۷���Ϣ
        /// </summary>
        private void LoadHummanScoreInfos()
        {
            this.dgvHummanScore.Rows.Clear();
            List<QaEventTypeDict> lstQaEventTypeDict = null;
            short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQaEventTypeDict);
            List<QcMsgDict> lstQcMsgDict = null;
            shRet = QcMsgDictAccess.Instance.GetQcMsgDictList(ref lstQcMsgDict);

            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            List<QcCheckResult> lstQcCheckResult = null;
            shRet = QcCheckResultAccess.Instance.GetQcCheckResults(SystemParam.Instance.DefaultTime, SystemParam.Instance.DefaultTime, szPatientID, szVisitID, null, null, null, SystemData.StatType.Artificial, ref lstQcCheckResult);

            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�ʿ��ʼ���������ʧ�ܣ�");
                return;
            }

            var firstQaEventTypeDict = lstQaEventTypeDict.Where(m => m.PARENT_CODE == string.Empty).ToList();
            int nRowIndex = 0;
            foreach (var item in firstQaEventTypeDict)
            {
                //���һ������
                nRowIndex = this.dgvHummanScore.Rows.Add();
                DataGridViewRow row = this.dgvHummanScore.Rows[nRowIndex];
                row.Tag = item;
                row.Cells[this.colItem.Index].Value = item.QA_EVENT_TYPE;
                row.ReadOnly = true;
                row.DefaultCellStyle.BackColor = Color.FromArgb(185, 185, 185);
                var secondQaEventTypeDict = lstQaEventTypeDict.Where(m => m.PARENT_CODE == item.INPUT_CODE).ToList();
                if (secondQaEventTypeDict.Count > 0)
                {
                    foreach (var childItem in secondQaEventTypeDict)
                    {
                        //���Ӷ�������
                        nRowIndex = this.dgvHummanScore.Rows.Add();
                        DataGridViewRow childrow = this.dgvHummanScore.Rows[nRowIndex];
                        childrow.Tag = childItem;
                        childrow.Cells[this.colItem.Index].Value = "  " + childItem.QA_EVENT_TYPE;
                        childrow.ReadOnly = true;
                        childrow.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
                        var secondMsgdict = lstQcMsgDict.Where(m => m.MESSAGE_TITLE == childItem.QA_EVENT_TYPE).ToList();
                        if (secondMsgdict.Count > 0)
                        {
                            foreach (var itemMsgDict in secondMsgdict)
                            {
                                //��Ӷ��������µ��ʼ�������
                                nRowIndex = this.dgvHummanScore.Rows.Add();
                                DataGridViewRow row2 = this.dgvHummanScore.Rows[nRowIndex];
                                row2.Tag = itemMsgDict;
                                row2.ReadOnly = true;
                                row2.Cells[this.colItem.Index].Value = string.Format("    {0}��{1}", secondMsgdict.IndexOf(itemMsgDict, 0) + 1, itemMsgDict.MESSAGE);
                                row2.Cells[this.colPoint.Index].Value = itemMsgDict.SCORE;
                                if (lstQcCheckResult != null)
                                {
                                    QcCheckResult qcCheckResult = lstQcCheckResult.Where(m => m.MSG_DICT_CODE == itemMsgDict.QC_MSG_CODE).FirstOrDefault();
                                    if (qcCheckResult != null)
                                    {
                                        row2.Cells[this.colRemark.Index].Value = qcCheckResult.REMARKS;
                                        row2.Cells[this.colCheckBox.Index].Value = true;
                                        row2.Cells[this.colErrorCount.Index].Value = qcCheckResult.ERROR_COUNT;
                                        row2.Cells[this.colRemark.Index].Tag = qcCheckResult;
                                    }
                                }

                            }
                        }
                    }
                }
                var firstMsgDict = lstQcMsgDict.Where(m => m.MESSAGE_TITLE == string.Empty && m.QA_EVENT_TYPE == item.QA_EVENT_TYPE).ToList();
                if (firstMsgDict.Count > 0)
                {
                    foreach (var item3 in firstMsgDict)
                    {
                        //���һ�������µ��ʼ�������
                        nRowIndex = this.dgvHummanScore.Rows.Add();
                        DataGridViewRow row2 = this.dgvHummanScore.Rows[nRowIndex];
                        row2.Tag = item3;
                        row2.Cells[this.colItem.Index].Value = string.Format("  {0}��{1}", firstMsgDict.IndexOf(item3, 0) + 1, item3.MESSAGE);
                        row2.Cells[this.colPoint.Index].Value = item3.SCORE;
                        row2.ReadOnly = true;
                        //row2.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
                        if (lstQcCheckResult != null)
                        {
                            QcCheckResult qcCheckResult = lstQcCheckResult.Where(m => m.MSG_DICT_CODE == item3.QC_MSG_CODE).FirstOrDefault();
                            if (qcCheckResult != null)
                            {
                                row2.Cells[this.colRemark.Index].Value = qcCheckResult.REMARKS;
                                row2.Cells[this.colCheckBox.Index].Value = true;
                                row2.Cells[this.colErrorCount.Index].Value = qcCheckResult.ERROR_COUNT;
                                row2.Cells[this.colRemark.Index].Tag = qcCheckResult;
                            }
                        }
                    }
                }
            }
            this.dgvHummanScore.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            this.dgvHummanScore.Refresh();
            //�������ֽ��
            QCScore qcScore = new QCScore();
            shRet = QcScoreAccess.Instance.GetQCScore(szPatientID, szVisitID, ref qcScore);
            this.tpHummanScore.Tag = qcScore;
            //this.CalHummanScore();
            this.tpHummanScore.Text = string.Format("�˹���⣨{0}��", qcScore.HOS_ASSESS);
        }

        /// <summary>
        /// ����ϵͳ���۷���Ϣ
        /// </summary>
        private void LoadSystemScoreInfos()
        {
            this.dgvSystemScore.Rows.Clear();
            List<QaEventTypeDict> lstQaEventTypeDict = null;
            short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQaEventTypeDict);
            List<QcMsgDict> lstQcMsgDict = null;
            shRet = QcMsgDictAccess.Instance.GetQcMsgDictList(ref lstQcMsgDict);

            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            List<QcCheckResult> lstQcCheckResult = null;
            shRet = QcCheckResultAccess.Instance.GetQcCheckResults(SystemParam.Instance.DefaultTime, SystemParam.Instance.DefaultTime, szPatientID, szVisitID, null, null, null, SystemData.StatType.System, ref lstQcCheckResult);

            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�ʿ��ʼ���������ʧ�ܣ�");
                return;
            }

            var firstQaEventTypeDict = lstQaEventTypeDict.Where(m => m.PARENT_CODE == string.Empty).ToList();
            int nRowIndex = 0;
            foreach (var item in firstQaEventTypeDict)
            {
                //���һ������
                nRowIndex = this.dgvSystemScore.Rows.Add();
                DataGridViewRow row = this.dgvSystemScore.Rows[nRowIndex];
                row.Tag = item;
                row.Cells[this.col_2_Item.Index].Value = item.QA_EVENT_TYPE;
                row.ReadOnly = true;
                row.DefaultCellStyle.BackColor = Color.FromArgb(185, 185, 185);
                var secondQaEventTypeDict = lstQaEventTypeDict.Where(m => m.PARENT_CODE == item.INPUT_CODE).ToList();
                if (secondQaEventTypeDict.Count > 0)
                {
                    foreach (var childItem in secondQaEventTypeDict)
                    {
                        //���Ӷ�������
                        nRowIndex = this.dgvSystemScore.Rows.Add();
                        DataGridViewRow childrow = this.dgvSystemScore.Rows[nRowIndex];
                        childrow.Tag = childItem;
                        childrow.Cells[this.col_2_Item.Index].Value = "  " + childItem.QA_EVENT_TYPE;
                        childrow.ReadOnly = true;
                        childrow.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
                        var secondMsgdict = lstQcMsgDict.Where(m => m.MESSAGE_TITLE == childItem.QA_EVENT_TYPE).ToList();
                        if (secondMsgdict.Count > 0)
                        {
                            foreach (var itemMsgDict in secondMsgdict)
                            {
                                //��Ӷ��������µ��ʼ�������
                                nRowIndex = this.dgvSystemScore.Rows.Add();
                                DataGridViewRow row2 = this.dgvSystemScore.Rows[nRowIndex];
                                row2.Tag = itemMsgDict;
                                row2.ReadOnly = true;
                                row2.Cells[this.col_2_Item.Index].Value = string.Format("    {0}��{1}", secondMsgdict.IndexOf(itemMsgDict, 0) + 1, itemMsgDict.MESSAGE);
                                row2.Cells[this.col_2_Score.Index].Value = itemMsgDict.SCORE;
                                if (lstQcCheckResult != null)
                                {
                                    QcCheckResult qcCheckResult = lstQcCheckResult.Where(m => m.MSG_DICT_CODE == itemMsgDict.QC_MSG_CODE && m.QC_RESULT == SystemData.QcResult.UnPass).FirstOrDefault();
                                    if (qcCheckResult != null)
                                    {
                                        row2.Cells[this.col_2_ErrorCount.Index].Value = qcCheckResult.ERROR_COUNT;
                                        row2.Cells[this.col_2_QC_EXPLAIN.Index].Value = qcCheckResult.QC_EXPLAIN;
                                        row2.Cells[this.col_2_Score.Index].Value = qcCheckResult.SCORE;
                                        row2.Cells[this.col_2_QC_EXPLAIN.Index].Tag = qcCheckResult;
                                        row2.DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }

                            }
                        }
                    }
                }
                var firstMsgDict = lstQcMsgDict.Where(m => m.MESSAGE_TITLE == string.Empty && m.QA_EVENT_TYPE == item.QA_EVENT_TYPE).ToList();
                if (firstMsgDict.Count > 0)
                {
                    foreach (var item3 in firstMsgDict)
                    {
                        //���һ�������µ��ʼ�������
                        nRowIndex = this.dgvSystemScore.Rows.Add();
                        DataGridViewRow row2 = this.dgvSystemScore.Rows[nRowIndex];
                        row2.Tag = item3;
                        row2.Cells[this.col_2_Item.Index].Value = string.Format("  {0}��{1}", firstMsgDict.IndexOf(item3, 0) + 1, item3.MESSAGE);
                        row2.Cells[this.col_2_Score.Index].Value = item3.SCORE;
                        row2.ReadOnly = true;
                        //row2.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
                        if (lstQcCheckResult != null)
                        {
                            QcCheckResult qcCheckResult = lstQcCheckResult.Where(m => m.MSG_DICT_CODE == item3.QC_MSG_CODE && m.QC_RESULT == SystemData.QcResult.UnPass).FirstOrDefault();
                            if (qcCheckResult != null)
                            {
                                row2.Cells[this.col_2_QC_EXPLAIN.Index].Value = qcCheckResult.QC_EXPLAIN;
                                row2.Cells[this.col_2_ErrorCount.Index].Value = qcCheckResult.ERROR_COUNT;
                                row2.Cells[this.col_2_Score.Index].Value = qcCheckResult.SCORE;
                                row2.Cells[this.col_2_QC_EXPLAIN.Index].Tag = qcCheckResult;
                                row2.DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
            this.dgvSystemScore.Refresh();
            //�������ֽ��
            QCScore qcScore = new QCScore();
            shRet = QcScoreAccess.Instance.GetQCScore(szPatientID, szVisitID, ref qcScore);
            this.tpHummanScore.Tag = qcScore;
            this.CalSystemScore();
            this.tpHummanScore.Text = string.Format("�˹���⣨{0}��", qcScore.HOS_ASSESS);
        }
        /// <summary>
        /// ���没�����ݿ۷�
        /// </summary>
        private void SaveHummanScore()
        {
            if (this.dgvHummanScore.Rows.Count <= 0)
                return;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            short shRet = SystemData.ReturnValue.OK;
            for (int index = 0; index < this.dgvHummanScore.Rows.Count; index++)
            {
                DataGridViewRow row = this.dgvHummanScore.Rows[index];
                QcMsgDict qcMsgDict = row.Tag as QcMsgDict;
                if (qcMsgDict == null)
                    continue;

                QcCheckResult qcCheckResult = row.Cells[this.colRemark.Index].Tag as QcCheckResult;
                if (row.Cells[this.colCheckBox.Index].Value == null)
                    continue;
                bool isCheck = bool.Parse(row.Cells[this.colCheckBox.Index].Value.ToString());
                //����۷���δ��ѡ����ϵͳ�Ѿ��п۷ּ�¼����ɾ��
                if (!isCheck && qcCheckResult != null)
                {
                    shRet = QcCheckResultAccess.Instance.Delete(qcCheckResult.CHECK_RESULT_ID);
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show(string.Format("��{0}��ȡ�������۷���Ϣ����ʧ�ܣ�", index + 1), MessageBoxIcon.Error);
                        continue;
                    }
                    row.Cells[this.colRemark.Index].Tag = null;
                }
                if (!isCheck)
                    continue;
                DateTime dtCheckTime = DateTime.Now;
                string szMessgCode = string.Empty;
                if (qcCheckResult == null)
                {
                    qcCheckResult = new EMRDBLib.QcCheckResult();
                    qcCheckResult.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
                    qcCheckResult.CHECKER_NAME = SystemParam.Instance.UserInfo.Name;
                    qcCheckResult.CHECKER_ID = SystemParam.Instance.UserInfo.ID;
                    qcCheckResult.CHECK_DATE = SysTimeHelper.Instance.Now;
                    qcCheckResult.BUG_CLASS = SystemData.BugClass.ERROR;
                    qcCheckResult.CHECK_POINT_ID = string.Empty;
                    qcCheckResult.CHECK_RESULT_ID = qcCheckResult.MakeID();
                    qcCheckResult.CHECK_TYPE = string.Empty;
                    qcCheckResult.CREATE_ID = string.Empty;
                    qcCheckResult.CREATE_NAME = string.Empty;
                    qcCheckResult.DEPT_CODE = SystemParam.Instance.PatVisitInfo.DEPT_CODE;
                    qcCheckResult.DEPT_IN_CHARGE = SystemParam.Instance.PatVisitInfo.DEPT_NAME;//��Ҫ���ĵ���أ�����ת��ǰ���������ο���Ϊ��������
                    qcCheckResult.INCHARGE_DOCTOR = SystemParam.Instance.PatVisitInfo.INCHARGE_DOCTOR;
                    qcCheckResult.DOCTYPE_ID = string.Empty;
                    qcCheckResult.DOC_SETID = string.Empty;
                    qcCheckResult.DOC_TIME = SysTimeHelper.Instance.DefaultTime;
                    qcCheckResult.DOC_TITLE = string.Empty;
                    qcCheckResult.ERROR_COUNT = int.Parse(row.Cells[this.colErrorCount.Index].Value.ToString());
                    qcCheckResult.ISVETO = false;
                    qcCheckResult.MODIFY_TIME = SysTimeHelper.Instance.DefaultTime;
                    qcCheckResult.MR_STATUS = SystemParam.Instance.PatVisitInfo.MR_STATUS;
                    qcCheckResult.MSG_DICT_CODE = qcMsgDict.QC_MSG_CODE;
                    qcCheckResult.MSG_DICT_MESSAGE = qcMsgDict.MESSAGE;
                    qcCheckResult.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
                    qcCheckResult.PATIENT_NAME = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
                    qcCheckResult.QA_EVENT_TYPE = qcMsgDict.QA_EVENT_TYPE;
                    qcCheckResult.QC_EXPLAIN = string.Empty;
                    qcCheckResult.QC_RESULT = SystemData.QcResult.UnPass;
                    qcCheckResult.SCORE = qcMsgDict.SCORE;
                    qcCheckResult.ORDER_VALUE = qcMsgDict.SERIAL_NO;
                    qcCheckResult.STAT_TYPE = SystemData.StatType.Artificial;
                    qcCheckResult.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
                    qcCheckResult.VISIT_NO = SystemParam.Instance.PatVisitInfo.VISIT_NO;
                    if (row.Cells[this.colRemark.Index].Value != null)
                        qcCheckResult.REMARKS = row.Cells[this.colRemark.Index].Value.ToString();
                    shRet = QcCheckResultAccess.Instance.Insert(qcCheckResult);
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show(string.Format("��{0}�в����۷���Ϣ����ʧ�ܣ�", index + 1), MessageBoxIcon.Error);
                        continue;
                    }
                    row.Cells[this.colRemark.Index].Tag = qcCheckResult;
                }
                else
                {
                    qcCheckResult.CHECKER_ID = SystemParam.Instance.UserInfo.ID;
                    qcCheckResult.CHECKER_NAME = SystemParam.Instance.UserInfo.Name;
                    qcCheckResult.ORDER_VALUE = qcMsgDict.SERIAL_NO;
                    qcCheckResult.MR_STATUS = SystemParam.Instance.PatVisitInfo.MR_STATUS;
                    qcCheckResult.CHECK_DATE = SysTimeHelper.Instance.Now;
                    qcCheckResult.ERROR_COUNT = int.Parse(row.Cells[this.colErrorCount.Index].Value.ToString());
                    if (row.Cells[this.colRemark.Index].Value != null)
                        qcCheckResult.REMARKS = row.Cells[this.colRemark.Index].Value.ToString();
                    shRet = QcCheckResultAccess.Instance.Update(qcCheckResult);
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show(string.Format("��{0}�в����۷���Ϣ����ʧ�ܣ�", index + 1), MessageBoxIcon.Error);
                        continue;
                    }
                }
            }
            //������ϸ�����ϣ��������ֽ����QC_SCORE��
            this.CalHummanScore();
            QCScore qcScore = this.tpHummanScore.Tag as QCScore;

            shRet = QcScoreAccess.Instance.Save(qcScore);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("���ֽ������ʧ��");
                return;
            }
            MessageBoxEx.ShowMessage("����ɹ�");
            this.OnHummanScoreSaved(System.EventArgs.Empty);

        }
        private ReportExplorerForm GetReportExplorerForm()
        {
            ReportExplorerForm reportExplorerForm = new ReportExplorerForm();
            reportExplorerForm.WindowState = FormWindowState.Maximized;
            reportExplorerForm.QueryContext +=
                new QueryContextEventHandler(this.ReportExplorerForm_QueryContext);
            reportExplorerForm.NotifyNextReport +=
                new NotifyNextReportEventHandler(this.ReportExplorerForm_NotifyNextReport);
            return reportExplorerForm;
        }

        /// <summary>
        /// ���ش�ӡģ��
        /// </summary>
        private byte[] GetReportFileData(string szReportName)
        {
            if (GlobalMethods.Misc.IsEmptyString(szReportName))
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "���������嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("���������嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("���������嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveHummanScore();
        }
        /// <summary>
        /// ����һ���ʼ�LOG��Ϣ
        /// </summary>
        private bool AddQCActionLog()
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return false;
            EMRDBLib.MedicalQcLog qcActionLog = new EMRDBLib.MedicalQcLog();
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
            qcActionLog.CHECK_TYPE = 2;
            qcActionLog.LOG_TYPE = 1;
            qcActionLog.LOG_DESC = "�ʿ��߱��没������";
            qcActionLog.AddQCQuestion = false;
            return MedicalQcLogAccess.Instance.Insert(qcActionLog) == SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ��ӡ����
        /// </summary>
        /// <returns>System.Data.DataTable</returns>
        private DataTable GetPrintData()
        {
            System.Data.DataTable table = GlobalMethods.Table.GetDataTable(this.dgvHummanScore, false, 0);
            DataRow row = null;

            row = table.NewRow();
            row[0] = string.Format("�ܵ÷֣�{0} �ȼ���{1} "
              , this.txtLevel.Text);
            row[1] = string.Empty;
            row[2] = string.Empty;
            table.Rows.Add(row);
            return table;
        }

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "���ڿ���")
            {
                value = SystemParam.Instance.PatVisitInfo.DEPT_NAME;
                return true;
            }
            if (name == "��������")
            {

                value = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
                return true;
            }
            if (name == "����ID")
            {
                value = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
                return true;
            }
            return false;
        }

        private void dgvDocContent_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //DataGridViewCell currCell = this.dgvHummanScore.CurrentCell;
            //if (currCell == null)
            //    return;

            //TextBox textBoxExitingControl = e.Control as TextBox;
            //if (textBoxExitingControl == null || textBoxExitingControl.IsDisposed)
            //    return;
            //textBoxExitingControl.ImeMode = ImeMode.Alpha;
            //textBoxExitingControl.KeyPress -= new KeyPressEventHandler(this.TextBoxExitingControl_KeyPress);
        }

        private void TextBoxExitingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            //DataGridViewCell currCell = this.dgvHummanScore.CurrentCell;
            //if (currCell == null)
            //    return;

            //if (currCell.ColumnIndex == this.colErrorCount.Index)
            //{
            //    if (e.KeyChar == (char)((int)Keys.Back))
            //        return;
            //    if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
            //        return;
            //    if (e.KeyChar.CompareTo('.') == 0)
            //        return;
            //    e.Handled = true;
            //}
        }


        private void dgvDocContent_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == this.colErrorCount.Index)
                CalHummanScore();
        }

        private void ReportExplorerForm_QueryContext(object sender, Heren.Common.Report.QueryContextEventArgs e)
        {
            object value = e.Value;
            e.Success = this.GetSystemContext(e.Name, ref value);
            if (e.Success) e.Value = value;
        }

        private void ReportExplorerForm_NotifyNextReport(object sender, Heren.Common.Report.NotifyNextReportEventArgs e)
        {
            e.ReportData = this.GetReportFileData(e.ReportName);
        }

        protected override void OnPatientInfoChanging(CancelEventArgs e)
        {
            base.OnPatientInfoChanging(e);
            if (!this.NeedSave)
                return;
            DialogResult diaResult = MessageBoxEx.ShowConfirm("��ǰ��������û�б��棬�Ƿ񱣴�?");
            if (diaResult == DialogResult.OK)
            {
                this.SaveHummanScore();
            }
            this.NeedSave = false;
        }

        private void dgvHummanScore_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            DataGridViewRow row = this.dgvHummanScore.Rows[e.RowIndex];
            bool flag = row.Cells[this.colCheckBox.Index].Value != null ? bool.Parse(row.Cells[this.colCheckBox.Index].Value.ToString()) : false;
            var qcMsgDict = row.Tag as QcMsgDict;
            if (qcMsgDict == null)
                return;
            if (e.ColumnIndex == this.colCheckBox.Index)
            {
                row.Cells[this.colCheckBox.Index].ReadOnly = false;
                row.Cells[this.colCheckBox.Index].Value = !flag;
                flag = !flag;
                if (flag)
                {
                    row.Cells[this.colErrorCount.Index].ReadOnly = false;
                    row.Cells[this.colErrorCount.Index].Value = 1;
                }
                else
                {
                    row.Cells[this.colErrorCount.Index].Value = string.Empty;
                }
            }
            else
            {
                row.Cells[this.colRemark.Index].ReadOnly = !flag;
                row.Cells[this.colErrorCount.Index].ReadOnly = !flag;
            }
            this.NeedSave = true;
        }
        private void CalHummanScore()
        {
            float totalScore = 100;
            foreach (DataGridViewRow item in this.dgvHummanScore.Rows)
            {
                var qcMsgDict = item.Tag as QcMsgDict;
                if (qcMsgDict == null)
                    continue;
                if (item.Cells[this.colCheckBox.Index].Value != null &&
                    item.Cells[this.colCheckBox.Index].Value.ToString() == "True")
                {
                    float point = float.Parse(item.Cells[this.colPoint.Index].Value.ToString());
                    int errorCount = int.Parse(item.Cells[this.colErrorCount.Index].Value.ToString());
                    totalScore -= point * errorCount;
                }
            }
            this.tpHummanScore.Text = string.Format("�˹���⣨{0}��", totalScore);
            this.txtLevel.Text = DocLevel.GetDocLevel(totalScore);
            QCScore qcScore = this.tpHummanScore.Tag as QCScore;
            if (qcScore == null)
            {
                qcScore = new QCScore();
            }
            qcScore.DeptCode = SystemParam.Instance.PatVisitInfo.DEPT_CODE;
            qcScore.DEPT_NAME = SystemParam.Instance.PatVisitInfo.DEPT_NAME;
            qcScore.PATIENT_NAME = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
            qcScore.DOC_LEVEL = DocLevel.GetDocLevel(totalScore);
            qcScore.HOS_ASSESS = totalScore;
            qcScore.HOS_DATE = SysTimeHelper.Instance.Now;
            qcScore.HOS_QCMAN = SystemParam.Instance.UserInfo.Name;
            qcScore.HOS_QCMAN_ID = SystemParam.Instance.UserInfo.ID;
            qcScore.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            qcScore.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            qcScore.VISIT_NO = SystemParam.Instance.PatVisitInfo.VISIT_NO;
        }
        private void CalSystemScore()
        {
            float totalScore = 100;
            foreach (DataGridViewRow item in this.dgvSystemScore.Rows)
            {
                var qcCheckResult = item.Cells[this.col_2_QC_EXPLAIN.Index].Tag as QcCheckResult;
                if (qcCheckResult == null)
                    continue;
                if (qcCheckResult.QC_RESULT == SystemData.QcResult.UnPass)
                {
                    float point = float.Parse(item.Cells[this.col_2_Score.Index].Value.ToString());
                    int errorCount = int.Parse(item.Cells[this.col_2_ErrorCount.Index].Value.ToString());
                    totalScore -= point * errorCount;
                }
            }
            this.tpSystemScore.Text = string.Format("ϵͳ��⣨{0}��", totalScore);
        }
        private void dgvHummanScore_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = this.dgvHummanScore.Rows[e.RowIndex];
            bool flag = row.Cells[this.colCheckBox.Index].Value != null ? bool.Parse(row.Cells[this.colCheckBox.Index].Value.ToString()) : false;
            var qcMsgDict = row.Tag as QcMsgDict;
            if (qcMsgDict == null)
            {
                QaEventTypeDict qaEventTypeDict = row.Tag as QaEventTypeDict;
                this.PatientPageControl.LoadModule(qaEventTypeDict.QA_EVENT_TYPE);
                return;
            }
            row.Cells[this.colCheckBox.Index].ReadOnly = false;
            row.Cells[this.colCheckBox.Index].Value = !flag;
            flag = !flag;
            if (flag)
            {
                row.Cells[this.colErrorCount.Index].ReadOnly = false;
                row.Cells[this.colErrorCount.Index].Value = 1;
            }
            else
            {
                row.Cells[this.colErrorCount.Index].Value = string.Empty;
            }
            this.NeedSave = true;

        }

        private void btnSystemCheck_Click(object sender, EventArgs e)
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return;
            try
            {
                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                WorkProcess.Instance.Initialize(this, 2, "ϵͳ�ؼ쿪ʼ������");
                WorkProcess.Instance.Show(0);
                CheckPointHelper.Instance.CheckPatient(SystemParam.Instance.PatVisitInfo);
                WorkProcess.Instance.Show("���ڼ��ؽ��������", 1);
                LoadSystemScoreInfos();
                WorkProcess.Instance.Show(3);
                this.tpSystemScore.Focus();
                WorkProcess.Instance.Close();
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("�������������й������", ex);
                MessageBoxEx.ShowErrorFormat("�������������й������", ex.ToString(), null);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dialogs.ModifyNoticeForm frm = new Dialogs.ModifyNoticeForm();
            if (frm.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }

}