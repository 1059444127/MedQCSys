// ***********************************************************
// �����ʿ�ϵͳ��������ͳ�ƴ���.
// Creator:LiChunYing  Date:2013-08-04
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;

using Heren.Common.Controls;
using Heren.Common.Libraries;
using Heren.Common.Report;
using Heren.Common.VectorEditor;
using Heren.Common.DockSuite;
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Utilities;
using MedQCSys.DockForms;
using MedQCSys;
using System.Linq;

namespace Heren.MedQC.MedRecord
{
    public partial class OutPatientStatisticForm : DockContentBase
    {
        public OutPatientStatisticForm(MainForm mainForm)
            : base(mainForm)
        {
            InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.dtpStatTimeEnd.Value = DateTime.Now;
            this.dtpStatTimeBegin.Value = DateTime.Now.AddMonths(-1);
            this.OnRefreshView();
        }

        public override void OnRefreshView()
        {
            this.Update();
            this.ShowStatusMessage("���������ٴ������б����Ժ�...");
            if (!InitControlData.InitCboDeptName(ref this.cboDeptName))
            {
                MessageBoxEx.Show("���ؿ����б�ʧ��");
            }
            this.ShowStatusMessage(null);
        }

        /// <summary>
        /// ��������Ϣ���ص�DataGridView��
        /// </summary>
        /// <param name="row"></param>
        /// <param name="qcWorkloadStatInfo"></param>
        private void SetRowData(DataGridViewRow row, PatVisitInfo patVisitLog)
        {
            if (row == null || patVisitLog == null)
                return;
            if (row.DataGridView == null)
                return;
            row.Cells[this.col_ORDER_NO.Index].Value = row.Index + 1;
            row.Cells[this.colDeptName.Index].Value = patVisitLog.DEPT_NAME;
            row.Cells[this.colPatientID.Index].Value = patVisitLog.PATIENT_ID;
            row.Cells[this.colPatientName.Index].Value = patVisitLog.PATIENT_NAME;
            row.Cells[this.colPatSex.Index].Value = patVisitLog.PATIENT_SEX;
            row.Cells[this.colVisitTime.Index].Value = patVisitLog.VISIT_TIME.ToString("yyyy-MM-dd");
            row.Cells[this.colChargeType.Index].Value = patVisitLog.CHARGE_TYPE;
            row.Cells[this.colAge.Index].Value = GlobalMethods.SysTime.GetAgeText(patVisitLog.BIRTH_TIME, DateTime.Now);
            row.Cells[this.colDischargeTime.Index].Value = patVisitLog.DISCHARGE_TIME.ToString("yyyy-MM-dd");
            row.Cells[this.col_INDAYS.Index].Value = GlobalMethods.SysTime.GetInpDays(patVisitLog.VISIT_TIME, patVisitLog.DISCHARGE_TIME);
            row.Cells[this.colCost.Index].Value = patVisitLog.TOTAL_COSTS;
            row.Cells[this.colRequestDoc.Index].Value = patVisitLog.INCHARGE_DOCTOR;
            row.Tag = patVisitLog;
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "��Ժ����ͳ���嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("��Ժ����ͳ���嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("��������ͳ���嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "���ڿ���")
            {
                string szDeptName = null;
                if (!string.IsNullOrEmpty(this.cboDeptName.Text.Trim()))
                    szDeptName = this.cboDeptName.Text;
                else
                    szDeptName = "ȫԺ";
                value = szDeptName;
                return true;
            }
            if (name == "��ʼ����")
            {
                value = this.dtpStatTimeBegin.Value;
                return true;
            }
            if (name == "��ֹ����")
            {
                value = this.dtpStatTimeEnd.Value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// ��Ժ�˴�
        /// </summary>
        private decimal m_OutPatientCount;
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                this.m_OutPatientCount = 0;
                DeptInfo deptInfo = this.cboDeptName.SelectedItem as DeptInfo;
                string szDeptCode = null;
                if (deptInfo != null)
                    szDeptCode = deptInfo.DEPT_CODE;
                if (string.IsNullOrEmpty(this.cboDeptName.Text))
                    szDeptCode = null;
                string szPatientID = this.txtPatientID.Text.Trim();
                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                this.ShowStatusMessage("���ڲ�ѯ���ݣ����Ժ�...");
                this.dataGridView1.Rows.Clear();
                List<PatVisitInfo> lstPatVisitLog = null;

                short shRet = PatVisitAccess.Instance.GetPatientListByDisChargeTime(szPatientID, DateTime.Parse(dtpStatTimeBegin.Value.ToString("yyyy-M-d 00:00:00")),
                    DateTime.Parse(dtpStatTimeEnd.Value.ToString("yyyy-M-d 23:59:59")), szDeptCode, ref lstPatVisitLog);

                if (shRet != SystemData.ReturnValue.OK
                    && shRet != SystemData.ReturnValue.RES_NO_FOUND)
                {
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    MessageBoxEx.Show("��ѯ����ʧ�ܣ�");
                    return;
                }
                if (lstPatVisitLog == null || lstPatVisitLog.Count <= 0)
                {
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    MessageBoxEx.Show("û�з������������ݣ�", MessageBoxIcon.Information);
                    return;
                }
                this.m_OutPatientCount = lstPatVisitLog.Count;
                string preDeptName = string.Empty;
                int nRowIndex = 0;
                for (int index = 0; index < lstPatVisitLog.Count; index++)
                {
                    EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLog[index];
                    if (preDeptName == string.Empty)
                        preDeptName = patVisitLog.DEPT_NAME;
                    if (preDeptName != patVisitLog.DEPT_NAME)
                    {
                        nRowIndex = this.dataGridView1.Rows.Add();
                        this.dataGridView1.Rows[nRowIndex].Cells[2].Value = "�ϼ�";
                        this.dataGridView1.Rows[nRowIndex].Cells[3].Value = "��Ժ�˴Σ�";
                        this.dataGridView1.Rows[nRowIndex].DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
                        this.dataGridView1.Rows[nRowIndex].Cells[4].Value = lstPatVisitLog.Where(m => m.DEPT_NAME == preDeptName).Count();
                        preDeptName = patVisitLog.DEPT_NAME;
                    }
                    nRowIndex = this.dataGridView1.Rows.Add();
                    DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                    this.SetRowData(row, patVisitLog);
                }
                nRowIndex = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[nRowIndex].Cells[2].Value = "�ϼ�";
                this.dataGridView1.Rows[nRowIndex].Cells[3].Value = "��Ժ�˴Σ�";
                this.dataGridView1.Rows[nRowIndex].Cells[4].Value = lstPatVisitLog.Where(m => m.DEPT_NAME == preDeptName).Count();
                this.dataGridView1.Rows[nRowIndex].DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
                #region ��ת�Ƽ�¼
                List<Transfer> lstTransfers = null;
                TransferAccess.Instance.GetList(lstPatVisitLog, ref lstTransfers);
                int nTransfer = 0;
                if (lstTransfers != null && lstTransfers.Count > 0)
                {
                    foreach (DataGridViewRow item in this.dataGridView1.Rows)
                    {
                        PatVisitInfo patVisitInfo = item.Tag as PatVisitInfo;
                        if (patVisitInfo == null)
                            continue;
                        var result = lstTransfers.Where(m => m.PATIENT_ID == patVisitInfo.PATIENT_ID && m.VISIT_ID == patVisitInfo.VISIT_ID).ToList();
                        if (result != null && result.Count > 1)
                        {
                            var first = result.FirstOrDefault();
                            if (first.DEPT_STAYED != first.DEPT_TRANSFER_TO)
                                item.Cells[this.colTransferTime.Index].Value = first.DISCHARGE_DATE_TIME.ToString("yyyy-MM-dd");
                            nTransfer++;
                        }
                        if (result != null && result.Count > 0)
                        {
                            var last = result.LastOrDefault();
                            item.Cells[this.col_MEDICAL_GROUP.Index].Value = last.MEDICAL_GROUP_NAME;
                            item.Cells[this.col_BED_CODE.Index].Value = last.BED_LABEL;
                        }
                    }
                }
                #endregion
                #region ��ȡ�����Ͳ������鵵���ڣ�
                List<QcMrIndex> lstQcMrIndexs = null;
                QcMrIndexAccess.Instance.GetList(lstPatVisitLog, ref lstQcMrIndexs);
                if (lstTransfers != null && lstTransfers.Count > 0)
                {
                    foreach (DataGridViewRow item in this.dataGridView1.Rows)
                    {
                        PatVisitInfo patVisitInfo = item.Tag as PatVisitInfo;
                        if (patVisitInfo == null)
                            continue;
                        var result = lstQcMrIndexs.Where(m => m.PATIENT_ID == patVisitInfo.PATIENT_ID && m.VISIT_ID == patVisitInfo.VISIT_ID).FirstOrDefault();
                        if (result != null && result.ARCHIVE_TIME != result.DefaultTime)
                        {
                            item.Cells[this.col_ARCHIVE_TIME.Index].Value = result.ARCHIVE_TIME.ToString("yyyy-MM-dd HH:mm");
                        }
                        item.Cells[this.col_ARCHIVE_TIME.Index].Tag = result;
                    }
                }
                #endregion
                this.lblTransferCount.Text = nTransfer.ToString();
                this.lblOutPatientCount.Text = lstPatVisitLog.Count.ToString();
                this.ShowStatusMessage(null);
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog(ex.ToString());
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɵ��������ݣ�", MessageBoxIcon.Information);
                return;
            }
            System.Collections.Hashtable htNoExportColunms = new System.Collections.Hashtable();
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "��Ժ����ͳ���嵥");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɴ�ӡ���ݣ�");
                return;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            byte[] byteReportData = this.GetReportFileData(null);
            if (byteReportData != null)
            {
                System.Data.DataTable table = GlobalMethods.Table.GetDataTable(this.dataGridView1, false, 0);
                ReportExplorerForm explorerForm = this.GetReportExplorerForm();
                explorerForm.ReportFileData = byteReportData;
                explorerForm.ReportParamData.Add("�Ƿ�����", false);
                explorerForm.ReportParamData.Add("��ӡ����", table);
                explorerForm.ShowDialog();
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            PatVisitInfo patVisit = row.Tag as PatVisitInfo;
            if (patVisit == null)
                return;
            if (SystemParam.Instance.LocalConfigOption.IsNewTheme)
            {
                this.MainForm.SwitchPatient(patVisit);
                return;
            }
            this.MainForm.OpenDocument(string.Empty, patVisit.PATIENT_ID, patVisit.VISIT_ID);
        }

        private void btnArchiveTime_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            PatVisitInfo patVisitInfo = this.dataGridView1.SelectedRows[0].Tag as PatVisitInfo;
            QcMrIndex qcMrIndex = this.dataGridView1.SelectedRows[0].Cells[this.col_ARCHIVE_TIME.Index].Tag as QcMrIndex;
            if (patVisitInfo == null)
            {
                MessageBoxEx.ShowMessage("ȡ������ʧ��");
                return;
            }
            short shRet = SystemData.ReturnValue.OK;
            if (qcMrIndex == null)
            {
                qcMrIndex = new QcMrIndex();
                qcMrIndex.ARCHIVE_DOCTOR = SystemParam.Instance.UserInfo.USER_NAME;
                qcMrIndex.ARCHIVE_DOCTOR_ID = SystemParam.Instance.UserInfo.USER_ID;
                qcMrIndex.ARCHIVE_TIME = SysTimeHelper.Instance.Now;
                qcMrIndex.PATIENT_ID = patVisitInfo.PATIENT_ID;
                qcMrIndex.VISIT_ID = patVisitInfo.VISIT_ID;
                qcMrIndex.VISIT_NO = patVisitInfo.VISIT_NO;
                shRet = QcMrIndexAccess.Instance.Insert(qcMrIndex);
            }
            else
            {
                qcMrIndex.ARCHIVE_TIME = SysTimeHelper.Instance.Now;
                shRet = QcMrIndexAccess.Instance.Update(qcMrIndex);
            }
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowMessage("ȡ������ʧ��");
                return;
            }
            this.dataGridView1.SelectedRows[0].Cells[this.col_ARCHIVE_TIME.Index].Value = qcMrIndex.ARCHIVE_TIME.ToString("yyyy-MM-dd HH:mm");
            MessageBoxEx.ShowMessage("ȡ�����ͳɹ�");
        }

        private void btnWarningArchive_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            PatVisitInfo patVisitInfo = this.dataGridView1.SelectedRows[0].Tag as PatVisitInfo;

            if (patVisitInfo == null)
            {
                MessageBoxEx.ShowMessage("�������ѷ���ʧ��");
                return;
            }
            short shRet = SystemData.ReturnValue.OK;
            ClinicWorklist clinicWorklist = new ClinicWorklist();
            clinicWorklist.CREATE_DEPT = SystemParam.Instance.UserInfo.DEPT_CODE;
            clinicWorklist.CREATE_STAFF = SystemParam.Instance.UserInfo.EMP_NO;
            clinicWorklist.CREATE_TIME = SysTimeHelper.Instance.Now;
            clinicWorklist.TARGET_DEPT = patVisitInfo.DEPT_CODE;
            clinicWorklist.WORKLIST_TYPE = "13";
            clinicWorklist.TARGET_STAFF = patVisitInfo.INCHARGE_DOCTOR_ID;
            clinicWorklist.WORKLIST_CONTENT = string.Format("{0}����{1}���콫����{2}�Ĳ����͵�������"
                , SystemParam.Instance.UserInfo.USER_NAME
                , patVisitInfo.DEPT_NAME
                , patVisitInfo.PATIENT_NAME
                );
            shRet = ClinicWorklistAccess.Instance.UpdateClinicWorklist(clinicWorklist, SystemData.OperFlag.INSERT);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowMessage("�������ѷ���ʧ��");
                return;
            }
            MessageBoxEx.ShowMessage("�������ѷ��ͳɹ�");
        }
    }
}