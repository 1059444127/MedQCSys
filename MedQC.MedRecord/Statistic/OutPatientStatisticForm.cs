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
        private void SetRowData(DataGridViewRow row, EMRDBLib.PatVisitInfo patVisitLog, EMRDBLib.PatDoctorInfo patDoctorInfo)
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
            row.Cells[this.colDiagnosis.Index].Value = patVisitLog.DIAGNOSIS;
            TimeSpan timeSpan = patVisitLog.DISCHARGE_TIME - patVisitLog.VISIT_TIME;
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
            List<EMRDBLib.PatVisitInfo> lstPatVisitLog = null;

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
            List<EMRDBLib.PatDoctorInfo> lstPatDoctorInfos = new List<EMRDBLib.PatDoctorInfo>();
            Hashtable hashtable = new Hashtable();
            for (int index = 0; index < lstPatVisitLog.Count; index++)
            {
                EMRDBLib.PatVisitInfo patLog = lstPatVisitLog[index];
                if (!hashtable.ContainsKey(patLog.PATIENT_ID + patLog.VISIT_ID))
                {
                    EMRDBLib.PatDoctorInfo patDoctorInfo = new EMRDBLib.PatDoctorInfo();
                    patDoctorInfo.PatientID = patLog.PATIENT_ID;
                    patDoctorInfo.VisitID = patLog.VISIT_ID;
                    hashtable.Add(patLog.PATIENT_ID + patLog.VISIT_ID, patDoctorInfo);
                    lstPatDoctorInfos.Add(patDoctorInfo);
                }
            }
            //��ȡ����ҽ����Ϣ
            shRet = PatVisitAccess.Instance.GetPatSanjiDoctors(ref lstPatDoctorInfos);

            for (int index = 0; index < lstPatVisitLog.Count; index++)
            {
                EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLog[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                EMRDBLib.PatDoctorInfo patDoctorInfo = lstPatDoctorInfos.Find(delegate (EMRDBLib.PatDoctorInfo p)
                {
                    if (p.PatientID == lstPatVisitLog[index].PATIENT_ID && p.VisitID == lstPatVisitLog[index].VISIT_ID)
                        return true;
                    else
                        return false;
                });
                this.SetRowData(row, patVisitLog, patDoctorInfo);
            }
            #region ���°󶨳�Ժ���
            List<EMRDBLib.DiagnosisInfo> lstDiagnosInfo = new List<EMRDBLib.DiagnosisInfo>();
            shRet = PatVisitAccess.Instance.GetOutPatientFirstDiagnosis(lstPatVisitLog, lstDiagnosInfo);
            if (shRet == SystemData.ReturnValue.OK
                && lstDiagnosInfo.Count > 0)
            {
                for (int index = 0; index < this.dataGridView1.Rows.Count; index++)
                {
                    DataGridViewRow row = this.dataGridView1.Rows[index];
                    EMRDBLib.PatVisitInfo patVisitLog = row.Tag as EMRDBLib.PatVisitInfo;
                    if (patVisitLog == null)
                        continue;
                    EMRDBLib.DiagnosisInfo diagnosisInfo = lstDiagnosInfo.Find(
                        delegate (EMRDBLib.DiagnosisInfo p)
                        {
                            return p.PatientID == patVisitLog.PATIENT_ID && p.VisitID == patVisitLog.VISIT_ID;
                        });
                    if (diagnosisInfo != null)
                        row.Cells[this.colDiagnosis.Index].Value =
                            string.Format("{0}({1})", diagnosisInfo.DiagnosisDesc, string.IsNullOrEmpty(diagnosisInfo.TreatResult) ? "δ֪" : diagnosisInfo.TreatResult);
                }
            }
            #endregion
            #region ���°󶨷���
            PatVisitAccess.Instance.GetPatConstInfo(ref lstPatVisitLog);
            if (lstPatVisitLog == null || lstPatVisitLog.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("������Ϣ��ѯʧ�ܣ�", MessageBoxIcon.Information);
                return;
            }
            for (int index = 0; index < this.dataGridView1.Rows.Count; index++)
            {
                DataGridViewRow row = this.dataGridView1.Rows[index];
                EMRDBLib.PatVisitInfo patVisitLog = row.Tag as EMRDBLib.PatVisitInfo;
                if (patVisitLog == null)
                    continue;
                EMRDBLib.PatVisitInfo findPatVisitLog = lstPatVisitLog.Find(
                    delegate (EMRDBLib.PatVisitInfo p)
                    {
                        return p.PATIENT_ID == patVisitLog.PATIENT_ID && p.VISIT_ID == patVisitLog.VISIT_ID;
                    });
                if (findPatVisitLog != null)
                    row.Cells[this.colCost.Index].Value = Math.Round(findPatVisitLog.TOTAL_COSTS, 2).ToString();
            }
            #endregion
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