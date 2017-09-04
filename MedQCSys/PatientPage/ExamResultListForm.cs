// ***********************************************************
// ����¼��ʾ����.
// Creator:YangMingkun  Date:2009-11-7
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using Heren.Common.Controls;
using EMRDBLib;
using EMRDBLib.DbAccess;
using Heren.MedQC.Utilities;
using Heren.MedQC.Core;
using System.IO;
using System.Linq;

namespace MedQCSys.DockForms
{
    public partial class ExamResultListForm : DockContentBase
    {
        public ExamResultListForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.HideOnClose = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.CloseButtonVisible = true;
            this.ExamList.Font = new Font("����", 10.5f);
            this.txtReportDetial.Font = new Font("����", 10.5f);
        }
        public ExamResultListForm(MainForm parent, PatPage.PatientPageControl patientPageControl)
            : base(parent, patientPageControl)
        {
            this.InitializeComponent();
            this.HideOnClose = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.CloseButtonVisible = true;
            this.ExamList.Font = new Font("����", 10.5f);
            this.txtReportDetial.Font = new Font("����", 10.5f);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!SystemParam.Instance.LocalConfigOption.PrintAndExcel)
                this.btnExportExcel.Visible = false;
            if (!SystemParam.Instance.LocalConfigOption.IsOpenPrintPacs)
            {
                this.btnPrintPacs.Visible = false;
                this.colCheckBox.Visible = false;
                this.colFilePath.Visible = false;
            }
        }
        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.Update();

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("�������ؼ���¼�����Ժ�...");

            this.LoadExamList();
            this.UnselectedAllRows();

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

        /// <summary>
        /// ȡ��ѡ������������
        /// </summary>
        private void UnselectedAllRows()
        {
            while (this.ExamList.SelectedRows.Count > 0)
            {
                this.ExamList.SelectedRows[0].Selected = false;
            }
        }

        /// <summary>
        /// ���ؼ���б�
        /// </summary>
        private void LoadExamList()
        {
            this.ExamList.SuspendLayout();
            this.ExamList.Rows.Clear();
            this.ExamList.ResumeLayout();

            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            List<ExamMaster> lstExamInfo = null;
            if (GlobalMethods.Misc.IsEmptyString(szPatientID) || GlobalMethods.Misc.IsEmptyString(szVisitID))
            {
                return;
            }

            short shRet = ExamMasterAccess.Instance.GetInpExamList(szPatientID, szVisitID, ref lstExamInfo);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND
                && shRet != SystemData.ReturnValue.CANCEL)
            {
                MessageBoxEx.Show("����¼����ʧ�ܣ�");
                return;
            }

            if (lstExamInfo == null || lstExamInfo.Count <= 0)
                return;

            this.ExamList.SuspendLayout();
            for (int index = lstExamInfo.Count - 1; index >= 0; index--)
            {
                ExamMaster examInfo = lstExamInfo[index];
                if (examInfo == null)
                    continue;

                int nRowIndex = this.ExamList.Rows.Add();
                DataGridViewRow row = this.ExamList.Rows[nRowIndex];
                row.Tag = examInfo;
                if (examInfo.RESULT_STATUS == "ȷ�ϱ���"
                    && SystemParam.Instance.LocalConfigOption.IsOpenPrintPacs)
                {
                    ExamResult examResultInfo = null;
                    shRet = ExamResultAccess.Instance.GetExamResultInfo(examInfo.EXAM_ID, ref examResultInfo);
                    if (!string.IsNullOrEmpty(examResultInfo.FILE_PATH))
                    {
                        row.Cells[this.colFilePath.Index].Value = Properties.Resources.pdf;
                        row.Cells[this.colFilePath.Index].Tag = examResultInfo;
                    }
                }
                row.Cells[this.colExamClass.Index].Value = examInfo.SUBJECT;
                if (examInfo.REQUEST_TIME != examInfo.DefaultTime)
                    row.Cells[this.colRequestTime.Index].Value = examInfo.REQUEST_TIME.ToString("yyyy-MM-dd");
                row.Cells[this.colRequestDoctor.Index].Value = examInfo.REQUEST_DOCTOR;
                if (examInfo.REPORT_TIME != examInfo.DefaultTime)
                    row.Cells[this.colReportTime.Index].Value = examInfo.REPORT_TIME.ToString("yyyy-MM-dd");
                row.Cells[this.colReportDoctor.Index].Value = examInfo.REPORT_DOCTOR;
                row.Cells[this.colResultStatus.Index].Value = examInfo.RESULT_STATUS;
            }
            this.ExamList.ResumeLayout();
        }

        /// <summary>
        /// ���ؼ�鱨����Ϣ
        /// </summary>
        private void LoadExamReportDetial(string szExamNo)
        {
            this.txtReportDetial.SuspendLayout();
            this.txtReportDetial.Clear();
            this.txtReportDetial.ResumeLayout();

            if (szExamNo == string.Empty)
                return;
            ExamResult examReportInfo = null;
            short shRet = ExamResultAccess.Instance.GetExamResultInfo(szExamNo, ref examReportInfo);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND
                && shRet != SystemData.ReturnValue.CANCEL)
            {
                MessageBoxEx.Show("��鱨������ʧ�ܣ�");
                return;
            }
            if (examReportInfo == null)
                return;

            this.txtReportDetial.SuspendLayout();
            Font boldFont = new Font("SimSun", 10, System.Drawing.FontStyle.Bold);
            Font regularFont = new Font(this.txtReportDetial.Font, System.Drawing.FontStyle.Regular);

            int nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("[������]");
            this.txtReportDetial.SelectionStart = 0;
            this.txtReportDetial.SelectionLength = 6;
            this.txtReportDetial.SelectionColor = Color.Black;
            this.txtReportDetial.SelectionFont = boldFont;


            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.PARAMETERS);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + "[�������]");
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionColor = Color.Black;
            this.txtReportDetial.SelectionFont = boldFont;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.DESCRIPTION);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + "[ӡ��]");
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = boldFont;
            this.txtReportDetial.SelectionColor = Color.Black;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.IMPRESSION);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + "[����]");
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = boldFont;
            this.txtReportDetial.SelectionColor = Color.Black;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.RECOMMENDATION);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + "[�Ƿ�����]");
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = boldFont;
            this.txtReportDetial.SelectionColor = Color.Black;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.IS_ABNORMAL);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + "[ʹ������]");
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = boldFont;
            this.txtReportDetial.SelectionColor = Color.Black;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.DEVICE);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + "[������ͼ����]");
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = boldFont;
            this.txtReportDetial.SelectionColor = Color.Black;

            nLength = this.txtReportDetial.TextLength;
            this.txtReportDetial.AppendText("\n" + examReportInfo.USE_IMAGE);
            this.txtReportDetial.SelectionStart = nLength;
            this.txtReportDetial.SelectionLength = this.txtReportDetial.TextLength - nLength;
            this.txtReportDetial.SelectionFont = regularFont;
            this.txtReportDetial.SelectionColor = Color.Blue;


            this.txtReportDetial.ResumeLayout();
        }

        private void ExamList_SelectionChanged(object sender, EventArgs e)
        {
            if (this.ExamList.SelectedRows.Count <= 0)
            {
                this.txtReportDetial.Clear();
                return;
            }
            this.ShowStatusMessage("�������ؼ�鱨�棬���Ժ�...");
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);

            ExamMaster examInfo = (ExamMaster)this.ExamList.SelectedRows[0].Tag;
            if (examInfo != null)
                this.LoadExamReportDetial(examInfo.EXAM_ID);

            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (this.ExamList.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɵ��������ݣ�", MessageBoxIcon.Information);
                return;
            }
            Hashtable htNoExportColunms = new Hashtable();
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StatExpExcelHelper.Instance.ExportToExcel(this.ExamList, "����¼�嵥");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ExamList.Rows.Count <= 0)
            {
                return;
            }
            foreach (DataGridViewRow item in this.ExamList.Rows)
            {
                item.Cells[this.colCheckBox.Index].Value = chkAll.Checked;
            }
        }

        private void ExamList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == this.colFilePath.Index)
            {
                ExamResult examResult = this.ExamList.Rows[e.RowIndex].Cells[this.colFilePath.Index].Tag as ExamResult;
                if (examResult == null)
                    return;
                if (string.IsNullOrEmpty(examResult.FILE_PATH))
                {
                    return;
                }
                string dstFilePath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}.pdf"
                    , SystemParam.Instance.WorkPath
                    , "temp"
                    , SystemParam.Instance.PatVisitInfo.PATIENT_ID
                    , SystemParam.Instance.PatVisitInfo.VISIT_ID
                    , examResult.EXAM_ID);
                if (!File.Exists(dstFilePath))
                {
                    bool result = ShareFolderRead.Download(examResult.FILE_PATH, dstFilePath);
                    if (!result)
                    {
                        MessageBoxEx.ShowError("��������ʧ��");
                        return;
                    }
                }
                CommandHandler.Instance.SendCommand("����鿴", this.MainForm, dstFilePath);
            }
        }

        private void btnPrintPacs_Click(object sender, EventArgs e)
        {
            if (this.ExamList.Rows.Count <= 0)
            {
                MessageBoxEx.ShowMessage("û��Ҫ��ӡ�ı���");
                return;
            }
            List<string> lstFileList = new List<string>();
            List<string> lstExamID = new List<string>();
            string szMergeFileName = string.Empty;
            WorkProcess.Instance.Initialize(this, this.ExamList.Rows.Count, "�������ؼ�鱨��....");
            foreach (DataGridViewRow item in this.ExamList.Rows)
            {
                if (WorkProcess.Instance.Canceled)
                {
                    WorkProcess.Instance.Close();
                    return;
                }
                WorkProcess.Instance.Show(item.Index+1);
                if (item.Cells[this.colResultStatus.Index].Value.ToString() != "ȷ�ϱ���")
                    continue;
                if (item.Cells[this.colCheckBox.Index].Value != null
                    && item.Cells[this.colCheckBox.Index].Value.ToString().ToLower() == "true"
                    )
                {

                    ExamResult examResult = item.Cells[this.colFilePath.Index].Tag as ExamResult;
                    if (examResult != null
                        && !string.IsNullOrEmpty(examResult.FILE_PATH))
                    {
                        //�����ļ�
                        string dstFilePath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}.pdf"
                   , SystemParam.Instance.WorkPath
                   , "temp"
                   , SystemParam.Instance.PatVisitInfo.PATIENT_ID
                   , SystemParam.Instance.PatVisitInfo.VISIT_ID
                   , examResult.EXAM_ID);
                        if (!File.Exists(dstFilePath))
                        {
                            bool result = ShareFolderRead.Download(examResult.FILE_PATH, dstFilePath);
                            if (!result)
                            {
                                MessageBoxEx.ShowError("��������ʧ��");
                                WorkProcess.Instance.Close();
                                return;
                            }
                        }
                        lstFileList.Add(examResult.FILE_PATH);
                        lstExamID.Add(examResult.EXAM_ID);
                    }
                }
            }
            WorkProcess.Instance.Close();
            if (lstFileList.Count <= 0)
            {
                MessageBoxEx.ShowMessage("û��Ҫ��ӡ�ı���");
                
                return;
            }
            szMergeFileName = string.Format("{0}\\temp\\{1}\\{2}\\{3}.pdf"
                , SystemParam.Instance.WorkPath
                , SystemParam.Instance.PatVisitInfo.PATIENT_ID
                , SystemParam.Instance.PatVisitInfo.VISIT_ID
                , string.Join("_", lstExamID.ToArray()));
            if (!File.Exists(szMergeFileName))
            {
                bool result = PdfHelper.MergePDFFiles(lstFileList.ToArray(), szMergeFileName);
                if (!result)
                {
                    MessageBoxEx.ShowError("����ϲ���ӡʧ��");
                    return;
                }
            }
            CommandHandler.Instance.SendCommand("����鿴", this.MainForm, szMergeFileName);
        }
    }
}