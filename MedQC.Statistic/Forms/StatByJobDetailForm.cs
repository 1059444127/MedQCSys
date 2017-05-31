// ***********************************************************
// �����ʿ�ϵͳ���ʿ��ߵĹ�����ͳ�ƴ���.
// Creator:YangMingkun  Date:2009-11-13
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
using EMRDBLib;
using System.Linq;
using Heren.Common.Controls;
using Heren.Common.Libraries;
using Heren.Common.Report;
using Heren.Common.VectorEditor;
using Heren.Common.DockSuite;
using EMRDBLib.DbAccess;
using Heren.MedQC.Utilities;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Statistic
{
    public partial class StatByJobDetailForm : DockContentBase
    {
        public StatByJobDetailForm(MainForm parent)
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
            this.ShowStatusMessage("���������û��б����Ժ�...");
            if (!InitControlData.InitcboUserList(ref this.cboUserList))
            {
                MessageBoxEx.ShowError("�����û��б�ʧ��");
            }
            this.ShowStatusMessage(null);
        }

        /// <summary>
        /// ��������Ϣ���ص�DataGridView��
        /// </summary>
        /// <param name="row"></param>
        /// <param name="qcWorkloadStatInfo"></param>
        private void SetRowData(DataGridViewRow row, EMRDBLib.MedicalQcMsg qcQuesiontionInfo, EMRDBLib.PatDoctorInfo patDoctorInfo)
        {
            if (row == null || qcQuesiontionInfo == null)
                return;
            if (row.DataGridView == null)
                return;
            row.Tag = qcQuesiontionInfo;
            row.Cells[this.colInpNO.Index].Value = qcQuesiontionInfo.InpNo;
            row.Cells[this.colPatientName.Index].Value = qcQuesiontionInfo.PATIENT_NAME;
            row.Cells[this.colPatientID.Index].Value = qcQuesiontionInfo.PATIENT_ID;
            row.Cells[this.colVisitID.Index].Value = qcQuesiontionInfo.VISIT_ID;
            row.Cells[this.colTopic.Index].Value = qcQuesiontionInfo.TOPIC;
            row.Cells[this.colCheckName.Index].Value = qcQuesiontionInfo.ISSUED_BY;
            row.Cells[this.colDeptName.Index].Value = qcQuesiontionInfo.DEPT_NAME;
            row.Cells[this.colDiagnosis.Index].Value = qcQuesiontionInfo.Diagnosis;
            row.Cells[this.colContent.Index].Value = qcQuesiontionInfo.MESSAGE;
            row.Cells[this.colQaEventType.Index].Value = qcQuesiontionInfo.QA_EVENT_TYPE;
            row.Cells[this.colLogDesc.Index].Value = qcQuesiontionInfo.LogDesc==null? "�ʿ�������ʼ���Ϣ":qcQuesiontionInfo.LogDesc;
            row.Cells[this.colRequestDoctor.Index].Value = qcQuesiontionInfo.DOCTOR_IN_CHARGE;
            if (patDoctorInfo != null)
            {
                row.Cells[this.colRequestDoctor.Index].Value = patDoctorInfo.RequestDoctorName;
                row.Cells[this.colPARENT_DOCTOR.Index].Value = patDoctorInfo.ParentDoctorName;
                row.Cells[this.colSuperDoctor.Index].Value = patDoctorInfo.SuperDoctorName;
            }
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "������ϸ����ͳ��");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("������ͳ���嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("������ͳ���嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private bool GetSystemContext(string name, ref object value)
        {
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
            string szCheckerName = this.cboUserList.Text;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ͳ�ƹ����������Ժ�...");
            this.dataGridView1.Rows.Clear();

            List<EMRDBLib.MedicalQcMsg> lstQCQuestionInfos = null;
            short shRet = MedicalQcMsgAccess.Instance.GetQCQuestionListByChecker(szCheckerName, DateTime.Parse(dtpStatTimeBegin.Value.ToString("yyyy-M-d 00:00:00")),
                DateTime.Parse(dtpStatTimeEnd.Value.ToString("yyyy-M-d 23:59:59")), ref lstQCQuestionInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯ����ʧ�ܣ�");
                return;
            }
            if (lstQCQuestionInfos == null || lstQCQuestionInfos.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("û�з������������ݣ�", MessageBoxIcon.Information);
                this.ShowStatusMessage(null);
                return;
            }
            
            List<EMRDBLib.PatDoctorInfo> lstPatDoctorInfos = new List<EMRDBLib.PatDoctorInfo>();
           
            Hashtable hashtable = new Hashtable();
            for (int index = 0; index < lstQCQuestionInfos.Count; index++)
            {
                EMRDBLib.MedicalQcMsg questionInfo = lstQCQuestionInfos[index];
                if (!hashtable.ContainsKey(questionInfo.PATIENT_ID + questionInfo.VISIT_ID))
                {
                    EMRDBLib.PatDoctorInfo patDoctorInfo = new EMRDBLib.PatDoctorInfo();
                    patDoctorInfo.PatientID = questionInfo.PATIENT_ID;
                    patDoctorInfo.VisitID = questionInfo.VISIT_ID;
                    hashtable.Add(questionInfo.PATIENT_ID + questionInfo.VISIT_ID, patDoctorInfo);
                    lstPatDoctorInfos.Add(patDoctorInfo);
                }
            }
            //��ȡ����ҽ����Ϣ
            shRet = PatVisitAccess.Instance.GetPatSanjiDoctors(ref lstPatDoctorInfos);
            //������ߡ��������������ʱ������
            lstQCQuestionInfos = lstQCQuestionInfos.OrderBy(m => m.ISSUED_BY).ThenBy(m => m.PATIENT_NAME)
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
                EMRDBLib.MedicalQcMsg questionInfo = lstQCQuestionInfos[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                EMRDBLib.PatDoctorInfo patDoctorInfo = lstPatDoctorInfos.Find(delegate (EMRDBLib.PatDoctorInfo p)
                {
                    if (p.PatientID == lstQCQuestionInfos[index].PATIENT_ID && p.VisitID == lstQCQuestionInfos[index].VISIT_ID)
                        return true;
                    else
                        return false;
                });
                this.SetRowData(row, questionInfo, patDoctorInfo);
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

            if (this.dataGridView1.Rows.Count > 0)
                this.dataGridView1.ClearSelection();

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
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "������ϸ����ͳ��");
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