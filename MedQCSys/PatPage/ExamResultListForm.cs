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
        public ExamResultListForm(MainForm parent,PatPage.PatientPageControl patientPageControl)
            : base(parent,patientPageControl)
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

            short shRet =ExamMasterAccess.Instance.GetInpExamList(szPatientID, szVisitID, ref lstExamInfo);
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
            short shRet =ExamResultAccess.Instance.GetExamResultInfo(szExamNo, ref examReportInfo);
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
            this.txtReportDetial.AppendText( "\n" + "[�������]");
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
            this.txtReportDetial.AppendText( "\n" + "[ӡ��]");
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
            this.txtReportDetial.AppendText( "\n" + "[����]");
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
            if (!SystemParam.Instance.QCUserRight.PrintExamList.Value)
            {
                MessageBoxEx.Show("��û��Ȩ�޵�������¼��", MessageBoxIcon.Information);
                return;
            }
            Hashtable htNoExportColunms = new Hashtable();
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StatExpExcelHelper.Instance.ExportToExcel(this.ExamList, "����¼�嵥");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
    }
}