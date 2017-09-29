// ***********************************************************
// �����¼��ʾ����.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.DockSuite;
using Heren.Common.Report;
using Heren.Common.VectorEditor;
using EMRDBLib;
using Heren.MedQC.Utilities;
using EMRDBLib.DbAccess;
using Heren.MedQC.Core;
using Heren.MedQC.Utilities.Dialogs;

namespace MedQCSys.DockForms
{
    public partial class LabResultListForm : DockContentBase
    {
        private List<DataTable> m_lstTestInfo = null;
        public LabResultListForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.HideOnClose = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.CloseButtonVisible = true;
            this.dgvLabResult.Font = new Font("����", 10.5f);
            this.dgvLabMaster.Font = new Font("����", 10.5f);
        }

        public LabResultListForm(MainForm parent, PatPage.PatientPageControl patientPageControl)
            : base(parent, patientPageControl)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.DockAreas = DockAreas.Document | DockAreas.DockBottom | DockAreas.DockLeft
                | DockAreas.DockRight | DockAreas.DockTop;
            this.dgvLabResult.Font = new Font("����", 10.5f);
            this.dgvLabMaster.Font = new Font("����", 10.5f);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!SystemParam.Instance.LocalConfigOption.PrintAndExcel)
            {
                this.btnExportExcel.Visible = false;
                this.btnPrint.Visible = false;
                this.colNeedPrint.Visible = false;
                this.chkPrintAll.Visible = false;
            }
        }
        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("�������ؼ�������¼�����Ժ�...");

            this.LoadLabTestList();
            this.chkPrintAll.Checked = false;
            this.ShowStatusMessage(null);
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
        /// ���л���ĵ�ʱˢ������
        /// </summary>
        protected override void OnActiveContentChanged()
        {
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this && this.NeedRefreshView)
                this.OnRefreshView();
        }
        

        private void LoadLabTestList()
        {
            this.dgvLabMaster.SuspendLayout();
            this.dgvLabMaster.Rows.Clear();
            this.dgvLabMaster.ResumeLayout();

            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            List<LabMaster> lstLabMaster = null;

            if (GlobalMethods.Misc.IsEmptyString(szPatientID) || GlobalMethods.Misc.IsEmptyString(szVisitID))
            {
                return;
            }
            short shRet = LabMasterAccess.Instance.GetList(szPatientID, szVisitID, ref lstLabMaster);
            if (shRet != SystemData.ReturnValue.OK && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��������¼��������ʧ�ܣ�");
                return;
            }
            if (lstLabMaster == null || lstLabMaster.Count <= 0)
                return;
            string szTestIDs = string.Empty;

            foreach (var item in lstLabMaster)
            {
                if (szTestIDs == string.Empty)
                    szTestIDs = "'" + item.TEST_ID + "'";
                else
                    szTestIDs = string.Format("{0},'{1}'", szTestIDs, item.TEST_ID);
            }
            List<LabResult> lstResultInfo = null;
            shRet = LabResultAccess.Instance.GetList2(szTestIDs, ref lstResultInfo);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�����¼��������ʧ�ܣ�");
            }
            this.dgvLabMaster.SuspendLayout();
            for (int index = lstLabMaster.Count - 1; index >= 0; index--)
            {
                LabMaster labMaster = lstLabMaster[index];
                if (labMaster == null)
                    continue;
                int nRowIndex = this.dgvLabMaster.Rows.Add();
                DataGridViewRow row = this.dgvLabMaster.Rows[nRowIndex];
                row.Tag = labMaster;
                row.Cells[this.colSpecimen.Index].Value = labMaster.SPECIMEN;
                if (labMaster.REQUEST_TIME != labMaster.DefaultTime)
                    row.Cells[this.colRequestTime.Index].Value = labMaster.REQUEST_TIME.ToString("yyyy-MM-dd");
                row.Cells[this.colRequestDoctor.Index].Value = labMaster.REQUEST_DOCTOR;
                row.Cells[this.colResultStatus.Index].Value = labMaster.RESULT_STATUS;
                if (labMaster.REPORT_TIME != labMaster.DefaultTime)
                    row.Cells[this.colReportTime.Index].Value = labMaster.REPORT_TIME.ToString("yyyy-MM-dd");
                row.Cells[this.colReportDoctor.Index].Value = labMaster.REPORT_DOCTOR;
                row.Cells[this.colSubject.Index].Value = labMaster.SUBJECT;
                if (lstResultInfo != null)
                {
                    var result = lstResultInfo.Where(m => m.TEST_ID == labMaster.TEST_ID).ToList();
                    if (result == null || result.Count <= 0)
                        continue;
                    row.Cells[this.colYiChang.Index].Tag = result;
                    for (int index1 = 0; index1 < result.Count; index1++)
                    {
                        LabResult labResult = result[index1];
                        if (labResult == null)
                            continue;

                        if (labResult.ABNORMAL_INDICATOR.Trim().Contains("Σ��"))//Σ��ֵ���
                            row.DefaultCellStyle.ForeColor = Color.Red;
                        if (labResult.ABNORMAL_INDICATOR.Trim().Contains("��")
                            || labResult.ABNORMAL_INDICATOR.Contains("��"))
                        {
                            row.Cells[this.colYiChang.Index].Value = "��";
                            row.Cells[this.colYiChang.Index].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
            this.dgvLabMaster.ResumeLayout();
        }

        /// <summary>
        /// ���ؼ����¼�б�
        /// </summary>
        private void LoadResultList(string testNo)
        {
            this.dgvLabResult.SuspendLayout();
            this.dgvLabResult.Rows.Clear();
            this.dgvLabResult.ResumeLayout();

            if (testNo == string.Empty)
                return;
            List<LabResult> lstLabResult = null;
            short shRet = LabResultAccess.Instance.GetList(testNo, ref lstLabResult);
            if (shRet != SystemData.ReturnValue.OK && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�����¼��������ʧ�ܣ�");
                return;
            }
            if (lstLabResult == null || lstLabResult.Count <= 0)
                return;

            this.dgvLabResult.SuspendLayout();
            for (int index = lstLabResult.Count - 1; index >= 0; index--)
            {
                LabResult resultInfo = lstLabResult[index];
                if (resultInfo == null)
                    continue;
                int nRowIndex = this.dgvLabResult.Rows.Add();
                DataGridViewRow row = this.dgvLabResult.Rows[nRowIndex];
                row.Cells[this.colItemName.Index].Value = resultInfo.ITEM_NAME;
                row.Cells[this.colResult.Index].Value = resultInfo.ITEM_RESULT;
                row.Cells[this.colUnit.Index].Value = resultInfo.ITEM_UNITS;

                if (!string.IsNullOrEmpty(resultInfo.ABNORMAL_INDICATOR))
                {
                    row.Cells[this.colAbnormal.Index].Style.ForeColor = Color.Red;
                }
                row.Cells[this.colReferContext.Index].Value = resultInfo.ITEM_REFER;
                if (resultInfo.ABNORMAL_INDICATOR.Contains("��"))
                {
                    row.Cells[this.colAbnormal.Index].Value = "��";
                    row.Cells[this.colAbnormal.Index].Style.ForeColor = Color.Red;
                }
                if (resultInfo.ABNORMAL_INDICATOR.Contains("��"))
                {
                    row.Cells[this.colAbnormal.Index].Value = "��";
                    row.Cells[this.colAbnormal.Index].Style.ForeColor = Color.Red;
                }
            }
            this.dgvLabResult.CurrentCell = null;
            this.dgvLabResult.ResumeLayout();
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

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "report1_deptName")
            {
                value = SystemParam.Instance.PatVisitInfo.DEPT_NAME;
                return true;
            }
            if (name == "report1_PatientID")
            {
                value = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
                return true;
            }
            if (name == "report1_AdmissionTime")
            {
                value = SystemParam.Instance.PatVisitInfo.VISIT_TIME.ToString("yyyy��MM��dd��");
                return true;
            }
            if (name == "report1_PatientName")
            {
                value = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
                return true;
            }
            if (name == "report1_patientSex")
            {
                value = SystemParam.Instance.PatVisitInfo.PATIENT_SEX;
                return true;
            }
            if (name == "report1_patientAge")
            {
                value = GlobalMethods.SysTime.GetAgeText(SystemParam.Instance.PatVisitInfo.BIRTH_TIME, SystemParam.Instance.PatVisitInfo.VISIT_TIME);
                return true;
            }
            if (name == "report1_Diagnosis")
            {
                value = SystemParam.Instance.PatVisitInfo.DIAGNOSIS;
                return true;
            }
            return false;
        }

        /// <summary>
        /// ���ش�ӡģ��
        /// </summary>
        private byte[] GetReportFileData(string szReportName)
        {
            if (GlobalMethods.Misc.IsEmptyString(szReportName))
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "���鱨�浥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("���鱨�浥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("���鱨�浥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private void LabTestInfoList_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dgvLabMaster.SelectedRows.Count <= 0)
            {
                this.dgvLabResult.SuspendLayout();
                this.dgvLabResult.Rows.Clear();
                this.dgvLabResult.ResumeLayout();
                return;
            }
            this.ShowStatusMessage("�������ؼ����¼���ݣ����Ժ�...");
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);

            this.dgvLabResult.Rows.Clear();
            LabMaster labTestInfo = (LabMaster)this.dgvLabMaster.SelectedRows[0].Tag;
            if (labTestInfo != null)
                this.LoadResultList(labTestInfo.TEST_ID);

            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.m_lstTestInfo == null)
                this.m_lstTestInfo = new List<DataTable>();
            else
                this.m_lstTestInfo.Clear();

            if (this.dgvLabMaster.Rows.Count <= 0)
                return;

            for (int index = 0; index < this.dgvLabMaster.Rows.Count; index++)
            {
                DataGridViewRow row = this.dgvLabMaster.Rows[index];
                LabMaster labTestInfo = row.Tag as LabMaster;

                if (labTestInfo == null)
                    continue;
                DataGridViewCell cell = row.Cells[this.colNeedPrint.Index];
                if (cell == null)
                    continue;
                object objValue = cell.Value;
                if (objValue == null || !bool.Parse(objValue.ToString()))
                    continue;

                List<LabResult> lstResultInfo = row.Cells[this.colYiChang.Index].Tag as List<LabResult>;
                //short shRet = LabResultAccess.Instance.GetList(labTestInfo.TEST_ID, ref lstResultInfo);
                //if (shRet != SystemData.ReturnValue.OK)
                //    continue;
                if (lstResultInfo == null || lstResultInfo.Count <= 0)
                    continue;

                DataTable dtResult = this.CreateResultData(lstResultInfo);
                dtResult = this.CreateTestData(labTestInfo, dtResult);
                this.m_lstTestInfo.Add(dtResult);
            }
            if (this.m_lstTestInfo.Count <= 0)
            {
                MessageBoxEx.Show("�빴ѡ��Ҫ��ӡ��¼��ߵĸ�ѡ��");
                return;
            }
            if (SystemParam.Instance.LocalConfigOption.IsLabPrintNewMethod)
            {
                DocumentPrintStyleForm frm = new DocumentPrintStyleForm();
                TempletType templetType = TempletTypeCache.Instance.GetWardDocType(SystemData.TempletTypeApplyEnv.LAB_REPROT);
                if (templetType == null)
                {
                    MessageBoxEx.ShowMessage("��ӡ��δ����");
                    return;
                }
                frm.TempletType = templetType;
                frm.Data = m_lstTestInfo;
                frm.Text = templetType.DocTypeName;
                frm.ShowDialog();
                return;
            }

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            ReportType reportType = ReportCache.Instance.GetWardReportType(SystemData.ReportTypeApplyEnv.LAB_RESULT, this.Text);
            if (reportType == null)
            {
                MessageBoxEx.ShowMessage("��ӡ����û������");
                return;
            }
            byte[] byteReportData = null;
            ReportCache.Instance.GetReportTemplet(reportType, ref byteReportData);
            if (byteReportData != null)
            {
                ReportExplorerForm explorerForm = this.GetReportExplorerForm();
                explorerForm.ReportFileData = byteReportData;
                explorerForm.ReportParamData.Add("�Ƿ�����", false);
                explorerForm.ReportParamData.Add("��ӡ����", this.m_lstTestInfo);
                explorerForm.ShowDialog();
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// �Ѽ�����Ϣ���浽DataTable��
        /// </summary>
        /// <param name="testInfo">testInfo</param>
        /// <param name="dtResult">Ŀ������</param>
        /// <returns>DaTaTable</returns>
        private DataTable CreateTestData(LabMaster testInfo, DataTable dtResult)
        {
            if (dtResult.Rows.Count <= 0)
                return dtResult;

            dtResult.Rows[0]["�����"] = testInfo.TEST_ID;
            dtResult.Rows[0]["����ҽ��"] = testInfo.REPORT_DOCTOR;
            dtResult.Rows[0]["����ҽ��"] = testInfo.REQUEST_DOCTOR;
            dtResult.Rows[0]["����ʱ��"] = testInfo.REPORT_TIME;
            dtResult.Rows[0]["����ʱ��"] = testInfo.REQUEST_TIME;
            dtResult.Rows[0]["�걾����"] = testInfo.SPECIMEN;
            dtResult.Rows[0]["����"] = testInfo.SUBJECT;
            return dtResult;
        }

        /// <summary>
        /// �Ѽ�����ϸ������浽DaTaTable��
        /// </summary>
        /// <param name="lstTestResultInfo">�����ϸ���</param>
        /// <returns>DaTaTable</returns>
        private DataTable CreateResultData(List<LabResult> lstTestResultInfo)
        {
            DataTable dt = new DataTable();
            DataColumn column = new DataColumn("�����");
            dt.Columns.Add(column);
            column = new DataColumn("����ҽ��");
            dt.Columns.Add(column);
            column = new DataColumn("����ҽ��");
            dt.Columns.Add(column);
            column = new DataColumn("����ʱ��");
            dt.Columns.Add(column);
            column = new DataColumn("����ʱ��");
            dt.Columns.Add(column);
            column = new DataColumn("�걾����");
            dt.Columns.Add(column);
            column = new DataColumn("����");
            dt.Columns.Add(column);
            //�����
            column = new DataColumn("��Ŀ");
            dt.Columns.Add(column);
            column = new DataColumn("���");
            dt.Columns.Add(column);
            column = new DataColumn("�ο�ֵ");
            dt.Columns.Add(column);
            column = new DataColumn("��λ");
            dt.Columns.Add(column);
            column = new DataColumn("�쳣���");
            dt.Columns.Add(column);
            for (int index = lstTestResultInfo.Count - 1; index >= 0; index--)
            {
                LabResult resultInfo = lstTestResultInfo[index];
                if (resultInfo == null)
                    continue;

                DataRow row = dt.NewRow();
                row["��Ŀ"] = resultInfo.ITEM_NAME;
                row["���"] = resultInfo.ITEM_RESULT;
                row["��λ"] = resultInfo.ITEM_UNITS;
                if (SystemParam.Instance.LocalConfigOption.IsLabPrintNewMethod)
                {
                    row["�ο�ֵ"] = resultInfo.ITEM_REFER;
                    if (resultInfo.ABNORMAL_INDICATOR.Contains("��"))
                    {
                        row["�쳣���"] = "��";
                    }
                    if (resultInfo.ABNORMAL_INDICATOR.Contains("��"))
                    {
                        row["�쳣���"] = "��";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(resultInfo.ABNORMAL_INDICATOR))
                        row["�ο�ֵ"] = resultInfo.ITEM_REFER + resultInfo.ITEM_UNITS;
                    else
                        row["�ο�ֵ"] = string.Format("{0} {1}{2}", resultInfo.ABNORMAL_INDICATOR, resultInfo.ITEM_REFER, resultInfo.ITEM_UNITS);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        private void ckbPrintAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.dgvLabMaster.Rows.Count <= 0)
                return;
            for (int index = 0; index < this.dgvLabMaster.Rows.Count; index++)
            {
                DataGridViewRow row = this.dgvLabMaster.Rows[index];
                if (CanPrint(index))
                    row.Cells[this.colNeedPrint.Index].Value = this.chkPrintAll.Checked;
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (this.dgvLabMaster.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɵ��������ݣ�", MessageBoxIcon.Information);
                return;
            }
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            Hashtable htNoExportColunms = new Hashtable();
            htNoExportColunms.Add(3, "0");
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            string szPatientName = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            string szTitle = string.Format("{0}��{1}�ξ����{2}", szPatientName, szVisitID, "�����¼�嵥");
            StatExpExcelHelper.Instance.ExportTestResultToExcel(this.dgvLabMaster, this.dgvLabResult, szTitle);
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
        private bool CanPrint(int rowIndex)
        {
            DataGridViewRow row = this.dgvLabMaster.Rows[rowIndex];
            LabMaster labTestInfo = row.Tag as LabMaster;
            if (labTestInfo == null)
                return false;
            DataGridViewCell cell = row.Cells[this.colNeedPrint.Index];
            if (cell == null)
                return false;
            List<LabResult> lstResultInfo = row.Cells[this.colYiChang.Index].Tag as List<LabResult>;
            if (lstResultInfo == null || lstResultInfo.Count <= 0)
                return false;
            return true;
        }
    }
}