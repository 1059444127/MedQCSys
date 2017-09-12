// ***********************************************************
// �����ʿ�ϵͳ�������߲�ѯ����.
// Creator:yehui  Date:2016-12-20
// Copyright:heren
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

namespace Heren.MedQC.Search
{
    public partial class SearchOperationForm : DockContentBase
    {
        public SearchOperationForm(MainForm mainForm) : base(mainForm)
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
            this.dtpStatTimeBegin.Value = DateTime.Now.AddDays(-1);
            this.BeautifyGrid();
            this.OnRefreshView();
        }
        public void BeautifyGrid()
        {
            this.dataGridView1.Columns[this.colOperationNo.Index].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns[this.colOpeartionDesc.Index].Width = 200;
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
        private void SetRowData(DataGridViewRow row, EMRDBLib.Operation operation)
        {
            if (row == null || operation == null)
                return;
            if (row.DataGridView == null)
                return;
            row.Cells[this.colPatientID.Index].Value = operation.PATIENT_ID;
            row.Cells[this.colVisitID.Index].Value = operation.VISIT_ID==0?string.Empty:operation.VISIT_ID.ToString();
            row.Cells[this.colDeptName.Index].Value = operation.DeptName;
            row.Cells[this.colSex.Index].Value = operation.Sex;
            row.Cells[this.colPatientName.Index].Value = operation.PATIENT_NAME;
            row.Cells[this.colOpeartionDesc.Index].Value = operation.OPERATION_DESC;
            row.Cells[this.colOperationDate.Index].Value = operation.OPERATING_DATE.ToShortDateString();
            row.Cells[this.colOperationNo.Index].Value = operation.OPERATION_NO;
            row.Cells[this.colOperator.Index].Value = operation.OPERATOR;
            row.Cells[this.colAnaesthesiaMethod.Index].Value = operation.ANAESTHESIA_METHOD;
            row.Cells[this.colHeal.Index].Value = operation.HEAL;
            row.Cells[this.col_OPERATION_SCALE.Index].Value = operation.OPERATION_SCALE;
            row.Cells[this.colWoundGrade.Index].Value = operation.WOUND_GRADE;

            row.Tag = operation;
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "Σ�ػ���ͳ���嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("Σ�ػ���ͳ���嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("Σ�ػ���ͳ���嵥������������ʧ��!");
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

        private void btnQuery_Click(object sender, EventArgs e)
        {
            DeptInfo deptInfo = this.cboDeptName.SelectedItem as DeptInfo;
            string szDeptCode = null;
            if (deptInfo != null)
                szDeptCode = deptInfo.DEPT_CODE;
            if (string.IsNullOrEmpty(this.cboDeptName.Text))
                szDeptCode = null;
            string szPatientID = txtPatientID.Text.Trim();
            string szPatientName = txtPatientName.Text.Trim();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("���ڲ�ѯ���ݣ����Ժ�...");
            this.dataGridView1.Rows.Clear();
            List<EMRDBLib.Operation> lstOperation = null;
            short shRet = SystemData.ReturnValue.OK;
            if (!string.IsNullOrEmpty(szPatientID) || !string.IsNullOrEmpty(szPatientName))
                shRet = OperationAccess.Instance.GetOperations(szPatientID, szPatientName, ref lstOperation);
            else
                shRet = OperationAccess.Instance.GetOperations(szDeptCode, DateTime.Parse(dtpStatTimeBegin.Value.ToString("yyyy-M-d 00:00:00")),
                DateTime.Parse(dtpStatTimeEnd.Value.ToString("yyyy-M-d 23:59:59")), ref lstOperation);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯ����ʧ�ܣ�");
                return;
            }
            if (lstOperation == null || lstOperation.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.MainForm.ShowStatusMessage("û���ҵ����������Ľ��");
                return;
            }
            string pre = null;
            int nRowIndex = 0;
            for (int index = 0; index < lstOperation.Count; index++)
            {
                EMRDBLib.Operation operation = lstOperation[index];
                nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                this.SetRowData(row, operation);
            }
            this.ShowStatusMessage(string.Format("��ѯ���������{0}��", this.dataGridView1.Rows.Count.ToString()));
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
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
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "Σ�ػ���ͳ���嵥");
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
            EMRDBLib.Operation operation = row.Tag as EMRDBLib.Operation;

            if (operation == null)
                return;
            if (SystemParam.Instance.LocalConfigOption.IsNewTheme)
            {
                PatVisitInfo patVisit = new PatVisitInfo() { PATIENT_ID = operation.PATIENT_ID, VISIT_ID = operation.VISIT_ID.ToString(),PATIENT_NAME=operation.PATIENT_NAME };
                this.MainForm.SwitchPatient(patVisit);
                return;
            }
            this.MainForm.OpenDocument(string.Empty, operation.PATIENT_ID, operation.VISIT_ID.ToString());
        }
    }
}