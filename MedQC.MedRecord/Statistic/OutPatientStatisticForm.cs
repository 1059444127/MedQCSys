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

                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                this.ShowStatusMessage("���ڲ�ѯ���ݣ����Ժ�...");
                this.dataGridView1.Rows.Clear();
                List<PatVisitInfo> lstPatVisitLog = null;

                short shRet = PatVisitAccess.Instance.GetPatientListByDisChargeTime(DateTime.Parse(dtpStatTimeBegin.Value.ToString("yyyy-M-d 00:00:00")),
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
            EMRDBLib.PatVisitInfo patVisit = row.Tag as EMRDBLib.PatVisitInfo;

            if (patVisit == null)
                return;

            if (SystemParam.Instance.LocalConfigOption.IsNewTheme)
            {

                this.MainForm.SwitchPatient(patVisit);
                return;
            }
            this.MainForm.OpenDocument(string.Empty, patVisit.PATIENT_ID, patVisit.VISIT_ID);
        }
    }
}