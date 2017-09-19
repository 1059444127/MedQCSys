// ***********************************************************
// �����ʿ�ϵͳ�������ͳ�ƴ���.
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
using EMRDBLib;

using Heren.Common.Controls;
using Heren.Common.Libraries;
using Heren.Common.Report;
using Heren.Common.VectorEditor;
using Heren.Common.DockSuite;
using EMRDBLib.DbAccess;
using System.Linq;
using Heren.MedQC.Utilities;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Statistic
{
    public partial class StatByCheckerForm : DockContentBase
    {
        public StatByCheckerForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
           
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.dtpStatTimeEnd.Value = DateTime.Now;
            this.dtpStatTimeBegin.Value = DateTime.Now.AddDays(-1);
        }

        public override void OnRefreshView()
        {
            this.Update();
            this.ShowStatusMessage("���������û��б����Ժ�...");
            if (!InitControlData.InitcboUserList(ref this.cboUserList))
            {
                MessageBoxEx.Show("�û��б�����ʧ�ܣ�");
                
            }
            this.ShowStatusMessage(null);
        }

        /// <summary>
        /// ��������Ϣ���ص�DataGridView��
        /// </summary>
        /// <param name="row"></param>
        /// <param name="qcQuestionInfo"></param>
        private void SetRowData(DataGridViewRow row, EMRDBLib.MedicalQcMsg qcQuestionInfo)
        {
            if (row == null || qcQuestionInfo == null)
                return;
            if (row.DataGridView == null)
                return;
            if (IsNeedShowSamePatientColumn(row.Index, qcQuestionInfo))
            {
                row.Cells[this.patientIDDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.PATIENT_ID;
                row.Cells[this.patientNameDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.PATIENT_NAME;
                row.Cells[this.colInpNO.Index].Value = qcQuestionInfo.InpNo;
            }
            row.Cells[this.col_BED_CODE.Index].Value = qcQuestionInfo.BED_CODE;
            row.Cells[this.deptStayedNameDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.DEPT_NAME;
            row.Cells[this.deptStayedIDDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.DEPT_STAYED;
            row.Cells[this.colQaEventType.Index].Value = qcQuestionInfo.QA_EVENT_TYPE;
            row.Cells[this.questionContentDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.MESSAGE;
            row.Cells[this.doctorInchargeDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.DOCTOR_IN_CHARGE;
            row.Cells[this.colPARENT_DOCTOR.Index].Value = qcQuestionInfo.PARENT_DOCTOR;
            row.Cells[this.col_BED_CODE.Index].Value = qcQuestionInfo.BED_CODE;
            //��ʾȨ�޸ĵ��ʿ�Ȩ�޿���
            //if (SystemConfig.Instance.Get(SystemData.ConfigKey.STAT_SHOW_CHECKER_NAME, false))
           
                row.Cells[this.checkerDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.ISSUED_BY;
            
            if (qcQuestionInfo.ISSUED_DATE_TIME != qcQuestionInfo.DefaultTime)
                row.Cells[this.dateCheckedDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.ISSUED_DATE_TIME;
            if (qcQuestionInfo.ASK_DATE_TIME != qcQuestionInfo.DefaultTime)
                row.Cells[this.dateConfirmedDataGridViewTextBoxColumn.Index].Value = qcQuestionInfo.ASK_DATE_TIME;
            row.Tag = qcQuestionInfo;
        }

        /// <summary>
        /// �Ƿ���Ҫ��ʾͬһ���ߵ���ͬ��
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsNeedShowSamePatientColumn(int rowIndex, EMRDBLib.MedicalQcMsg currentQCQuestionInfo)
        {
            if (SystemParam.Instance.LocalConfigOption.IsShowSameColumn)
            {
                return true;
            }
            else//�ж��Ƿ���ͬһ�����ߣ�������Ҫ��ʾ
            {
                if (rowIndex == 0 || currentQCQuestionInfo == null)
                    return true;
                EMRDBLib.MedicalQcMsg preQCQuestionInfo = this.dataGridView1.Rows[rowIndex - 1].Tag as EMRDBLib.MedicalQcMsg;
                if (preQCQuestionInfo == null)
                    return true;
                if (preQCQuestionInfo.PATIENT_ID == currentQCQuestionInfo.PATIENT_ID
                    && preQCQuestionInfo.VISIT_ID == currentQCQuestionInfo.VISIT_ID)
                    return false;
                else
                    return true;
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "�������ͳ�������嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("�������ͳ�������嵥����û������!");
                return null;
            }
            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("�������ͳ�������嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "�����")
            {
                value = this.cboUserList.Text;
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
            string szChecker = this.cboUserList.Text;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("���ڰ������ͳ���ʼ����⣬���Ժ�...");
            this.dataGridView1.Rows.Clear();
            List<EMRDBLib.MedicalQcMsg> lstQCQuestionInfos = null;
            short shRet =MedicalQcMsgAccess.Instance.GetQCQuestionListByChecker(szChecker, DateTime.Parse(dtpStatTimeBegin.Value.ToString("yyyy-M-d 00:00:00")),
                DateTime.Parse(dtpStatTimeEnd.Value.ToString("yyyy-M-d 23:59:59")), ref lstQCQuestionInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯ����ʧ�ܣ�");
                this.ShowStatusMessage(null);
                return;
            }
            if (lstQCQuestionInfos == null || lstQCQuestionInfos.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("û�з������������ݣ�", MessageBoxIcon.Information);
                this.ShowStatusMessage(null);
                return;
            }
            //���ռ���ߣ��������������ʱ������
            lstQCQuestionInfos= lstQCQuestionInfos.OrderBy(m => m.ISSUED_ID)
                .ThenBy(m => m.PATIENT_NAME)
                .ThenBy(m => m.ISSUED_DATE_TIME).ToList();
            //����߼�������
            int iCount = 0;
            Hashtable htPidVid = new Hashtable();
            for (int index = 0; index < lstQCQuestionInfos.Count; index++)
            {
                iCount++;
                if (!htPidVid.ContainsKey(lstQCQuestionInfos[index].PATIENT_ID + lstQCQuestionInfos[index].VISIT_ID))
                {
                    htPidVid.Add(lstQCQuestionInfos[index].PATIENT_ID + lstQCQuestionInfos[index].VISIT_ID,
                        lstQCQuestionInfos[index].PATIENT_ID + lstQCQuestionInfos[index].VISIT_ID);
                }
                EMRDBLib.MedicalQcMsg qcQuestionInfo = lstQCQuestionInfos[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                this.SetRowData(row, qcQuestionInfo);
                if ((index + 1) == lstQCQuestionInfos.Count
                    || (lstQCQuestionInfos[index].ISSUED_BY != lstQCQuestionInfos[index + 1].ISSUED_BY))
                {
                    DataGridViewRow sumrow = this.dataGridView1.Rows[this.dataGridView1.Rows.Add()];
                    sumrow.Cells[0].Value = "�ϼ�";
                    sumrow.Cells[1].Value = "���������";
                    sumrow.Cells[2].Value = iCount;
                    sumrow.Cells[3].Value = "����������";
                    sumrow.Cells[4].Value = htPidVid.Count;
                    sumrow.DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
                    iCount = 0;
                    htPidVid.Clear();
                }
            }
            this.dataGridView1.Tag = this.cboUserList.Text;

            this.ShowStatusMessage(null);
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
            System.Data.DataTable table = GlobalMethods.Table.GetDataTable(this.dataGridView1, false, 0);
            ReportPrintHelper.Instance.UserName = this.cboUserList.Text;
            ReportPrintHelper.Instance.StatTimeBegin = this.dtpStatTimeBegin.Value;
            ReportPrintHelper.Instance.StatTimeEnd = this.dtpStatTimeEnd.Value;
            ReportPrintHelper.Instance.ShowPrintView(table, this.Text);
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
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "�������ͳ�������嵥");
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
            EMRDBLib.MedicalQcMsg info = row.Tag as EMRDBLib.MedicalQcMsg;
            if (info == null)
                return;
            this.MainForm.OpenDocument(info.TOPIC_ID, info.PATIENT_ID, info.VISIT_ID);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}