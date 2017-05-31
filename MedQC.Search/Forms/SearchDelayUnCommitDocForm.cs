// ***********************************************************
// �����ʿ�ϵͳ����δ�ύ������ѯ����.
// Creator:LiChunYing  Date:2012-03-15
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
using Heren.Common.DockSuite;

using Heren.Common.Report;
using Heren.Common.VectorEditor;
using MedQCSys.Utility;
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Utilities;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Search
{
    public partial class SearchDelayUnCommitDocForm : DockContentBase
    {
        public SearchDelayUnCommitDocForm(MainForm parent)
            : base(parent)
        {
            InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
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

        private void LoadDelyUnCommitDocInfos(List<EMRDBLib.PatVisitInfo> lstPatVisitLog)
        {
            for (int index = 0; index < lstPatVisitLog.Count; index++)
            {
                EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLog[index];
                if (patVisitLog == null)
                    continue;
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Cells[this.colPatientID.Index].Value = patVisitLog.PATIENT_ID;
                row.Cells[this.colVisitID.Index].Value = patVisitLog.VISIT_ID;
                row.Cells[this.colDeptName.Index].Value = patVisitLog.DEPT_NAME;
                row.Cells[this.colPatName.Index].Value = patVisitLog.PATIENT_NAME;
                row.Cells[this.colAdmissionDate.Index].Value = patVisitLog.VISIT_TIME.ToString("yyyy-MM-dd");
                if (patVisitLog.DISCHARGE_TIME != patVisitLog.DefaultTime)
                {
                    row.Cells[this.colDischargeDate.Index].Value = patVisitLog.DISCHARGE_TIME.ToString("yyyy-MM-dd");
                    row.Cells[this.colDelayDays.Index].Value = (DateTime.Now - patVisitLog.DISCHARGE_TIME).Days;
                }
                row.Tag = patVisitLog;
            }
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "����δ�ύ������ѯ�嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("����δ�ύ������ѯ�嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("����δ�ύ������ѯ�嵥������������ʧ��!");
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
            if (name == "��Ժ����")
            {
                value = this.numericUpDown1.Value;
                return true;
            }
            return false;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ͳ������δ�ύ������Ϣ�����Ժ�...");
            this.dataGridView1.Rows.Clear();
            DeptInfo deptInfo = this.cboDeptName.SelectedItem as DeptInfo;
            string szDeptCode = null;
            if (deptInfo != null)
                szDeptCode = deptInfo.DEPT_CODE;
            if (string.IsNullOrEmpty(this.cboDeptName.Text))
                szDeptCode = null;
            string szDelayDays = this.numericUpDown1.Value.ToString();
            List<EMRDBLib.PatVisitInfo> lstPatVisitLog = null;
            short shRet = SystemData.ReturnValue.OK;
            shRet = MedQCAccess.Instance.GetDelayUnCommitDocInfos(szDeptCode, szDelayDays, ref lstPatVisitLog);
            if (shRet != SystemData.ReturnValue.OK || lstPatVisitLog == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage(null);
                return;
            }
            this.LoadDelyUnCommitDocInfos(lstPatVisitLog);
            this.dataGridView1.Tag = "����δ�ύ������ѯ�嵥";
            this.ShowStatusMessage(null);
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
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "����δ�ύ������ѯ�嵥");
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
            string szPId = this.dataGridView1.Rows[e.RowIndex].Cells[this.colPatientID.Index].Value != null ?
                this.dataGridView1.Rows[e.RowIndex].Cells[this.colPatientID.Index].Value.ToString() : "";
            string szVid = this.dataGridView1.Rows[e.RowIndex].Cells[this.colVisitID.Index].Value != null ?
              this.dataGridView1.Rows[e.RowIndex].Cells[this.colVisitID.Index].Value.ToString() : "";
            if (string.IsNullOrEmpty(szPId) || string.IsNullOrEmpty(szVid))
                return;

            if (SystemParam.Instance.LocalConfigOption.IsNewTheme)
            {
                PatVisitInfo patVisit = new PatVisitInfo() { PATIENT_ID = szPId, VISIT_ID = szVid };
                this.MainForm.SwitchPatient(patVisit);
                return;
            }
            this.MainForm.OpenDocument(string.Empty, szPId, szVid);
        }
    }
}