// ***********************************************************
// �����ʿ�ϵͳ���Ҳ��������Ƚ�
// Creator:LiChunYing  Date:2012-6-12
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
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Utilities;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Statistic
{
    public partial class StatByDocScoreCompreForm : DockContentBase
    {
        public StatByDocScoreCompreForm(MainForm mainForm)
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
            this.dtpStatTimeBegin.Value = DateTime.Now.AddDays(-1);
            this.cboDate.SelectedIndex = 0;
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

            this.ShowStatusMessage("���������û��б����Ժ�...");
            List<UserInfo> lstUserInfo = null;
            if (UserAccess.Instance.GetAllUserInfos(ref lstUserInfo) != SystemData.ReturnValue.OK)
            {
                this.ShowStatusMessage(null);
                MessageBoxEx.Show("�û��б�����ʧ�ܣ�");
                return;
            }
            if (lstUserInfo == null || lstUserInfo.Count <= 0)
            {
                this.ShowStatusMessage(null);
                return;
            }
            for (int index = 0; index < lstUserInfo.Count; index++)
            {
                UserInfo userInfo = lstUserInfo[index];
                string szInputCode = GlobalMethods.Convert.GetInputCode(userInfo.Name, false, 10);
                this.cboUserList.Items.Add(userInfo);
            }
            this.ShowStatusMessage(null);
        }

        /// <summary>
        /// ��������Ϣ���ص�DataGridView��
        /// </summary>
        /// <param name="row"></param>
        /// <param name="deptDocScoreInfo"></param>
        private void SetRowData(DataGridViewRow row, EMRDBLib.DeptDocScoreInfo deptDocScoreInfo)
        {
            if (row == null || deptDocScoreInfo == null)
                return;
            if (row.DataGridView == null)
                return;

            row.Cells[this.colDeptName.Index].Value = deptDocScoreInfo.DeptName;
            row.Cells[this.colScoreANum.Index].Value = deptDocScoreInfo.ScoreANum;
            row.Cells[this.colScoreBNum.Index].Value = deptDocScoreInfo.ScoreBNum;
            row.Cells[this.colScoreCNum.Index].Value = deptDocScoreInfo.ScoreCNum;
            float fTotalScoreNum = deptDocScoreInfo.ScoreANum + deptDocScoreInfo.ScoreBNum + deptDocScoreInfo.ScoreCNum;
            row.Cells[this.colScoreARate.Index].Value = fTotalScoreNum > 0 ? (deptDocScoreInfo.ScoreANum / fTotalScoreNum * 100).ToString("F2") + "%" : "0%";
            row.Cells[this.colScoreBRate.Index].Value = fTotalScoreNum > 0 ? (deptDocScoreInfo.ScoreBNum / fTotalScoreNum * 100).ToString("F2") + "%" : "0%";
            row.Cells[this.colScoreCRate.Index].Value = fTotalScoreNum > 0 ? (deptDocScoreInfo.ScoreCNum / fTotalScoreNum * 100).ToString("F2") + "%" : "0%";
            row.Tag = deptDocScoreInfo;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            string szDeptCode = null;
            if (!string.IsNullOrEmpty(this.cboDeptName.Text))
            {
                 DeptInfo deptInfo = this.cboDeptName.SelectedItem as  DeptInfo;
                szDeptCode = deptInfo.DEPT_CODE;
            }

            string szCheckerName = null;
            if (!string.IsNullOrEmpty(this.cboUserList.Text))
            {
                 UserInfo userInfo = this.cboUserList.SelectedItem as  UserInfo;
                if (userInfo != null)
                    szCheckerName = userInfo.Name;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ͳ�Ʋ����ȼ������Ժ�...");
            this.dataGridView1.Rows.Clear();
            bool bDischareTime = true;
            if (this.cboDate.SelectedIndex > 0)
                bDischareTime = false;
            short shRet = SystemData.ReturnValue.OK;
            List<EMRDBLib.DeptDocScoreInfo> lstDeptDocScoreInfo = null;

            List<EMRDBLib.QCScore> lstQCScore = null;
            shRet = MedQCAccess.Instance.GetPatQCScores(szDeptCode, szCheckerName, DateTime.Parse(dtpStatTimeBegin.Value.ToString("yyyy-M-d 00:00:00")),
                DateTime.Parse(dtpStatTimeEnd.Value.ToString("yyyy-M-d 23:59:59"))
                , bDischareTime, ref lstQCScore);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯ����ʧ�ܣ�");
                this.ShowStatusMessage(null);
                return;
            }
            if (lstQCScore == null || lstQCScore.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("û�з������������ݣ�", MessageBoxIcon.Information);
                this.ShowStatusMessage(null);
                return;
            }
            Hashtable hsDeptTable = new Hashtable();
            lstDeptDocScoreInfo = new List<DeptDocScoreInfo>();
            for (int index = 0; index < lstQCScore.Count; index++)
            {
                this.ShowStatusMessage(string.Format("����ͳ�Ƶ�{0}/{1}�������������Ժ�...", index + 1, lstQCScore.Count));
                //������ұ�
                if (!hsDeptTable.ContainsKey(lstQCScore[index].DeptCode))
                {
                    hsDeptTable.Add(lstQCScore[index].DeptCode, lstQCScore[index].DEPT_NAME);
                    EMRDBLib.DeptDocScoreInfo deptDocScoreInfo = new EMRDBLib.DeptDocScoreInfo();
                    deptDocScoreInfo.DeptCode = lstQCScore[index].DeptCode;
                    deptDocScoreInfo.DeptName = lstQCScore[index].DEPT_NAME;
                    deptDocScoreInfo.ScoreANum = 0;
                    deptDocScoreInfo.ScoreBNum = 0;
                    deptDocScoreInfo.ScoreCNum = 0;
                    lstDeptDocScoreInfo.Add(deptDocScoreInfo);
                }

                if (lstQCScore[index].DOC_LEVEL == "��")
                {
                    this.SetDeptScoreNum(lstDeptDocScoreInfo, lstQCScore[index].DeptCode, "A");
                }
                else if (lstQCScore[index].DOC_LEVEL == "��")
                {
                    this.SetDeptScoreNum(lstDeptDocScoreInfo, lstQCScore[index].DeptCode, "B");
                }
                else if (lstQCScore[index].DOC_LEVEL == "��")
                {
                    this.SetDeptScoreNum(lstDeptDocScoreInfo, lstQCScore[index].DeptCode, "C");
                }
            }

            float fTotalANum = 0f;
            float fTotalBNum = 0f;
            float fTotalCNum = 0f;
            for (int index = 0; index < lstDeptDocScoreInfo.Count; index++)
            {
                EMRDBLib.DeptDocScoreInfo deptDocScoreInfo = lstDeptDocScoreInfo[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                this.SetRowData(row, deptDocScoreInfo);
                fTotalANum += deptDocScoreInfo.ScoreANum;
                fTotalBNum += deptDocScoreInfo.ScoreBNum;
                fTotalCNum += deptDocScoreInfo.ScoreCNum;
            }
            int nIndex = this.dataGridView1.Rows.Add();
            DataGridViewRow currRow = this.dataGridView1.Rows[nIndex];
            currRow.Cells[this.colDeptName.Index].Value = "�ϼ�";
            currRow.Cells[this.colScoreANum.Index].Value = fTotalANum;
            currRow.Cells[this.colScoreBNum.Index].Value = fTotalBNum;
            currRow.Cells[this.colScoreCNum.Index].Value = fTotalCNum;
            float fTotalNum = fTotalANum + fTotalBNum + fTotalCNum;
            float fScoreARate = fTotalANum / fTotalNum;
            currRow.Cells[this.colScoreARate.Index].Value = fTotalNum > 0 ? (fTotalANum / fTotalNum * 100).ToString("F2") + "%" : "0%";
            currRow.Cells[this.colScoreBRate.Index].Value = fTotalNum > 0 ? (fTotalBNum / fTotalNum * 100).ToString("F2") + "%" : "0%";
            currRow.Cells[this.colScoreCRate.Index].Value = fTotalNum > 0 ? (fTotalCNum / fTotalNum * 100).ToString("F2") + "%" : "0%";
            if (!string.IsNullOrEmpty(szDeptCode))
                this.dataGridView1.Tag = string.Format("���Ʋ��������Ƚ��嵥 ���ң�{0}  ͳ������:  {1} �� {2} ", this.cboDeptName.Text
                    , this.dtpStatTimeBegin.Value.ToString("yyyy-MM-dd"), this.dtpStatTimeEnd.Value.ToString("yyyy-MM-dd"));
            else
                this.dataGridView1.Tag = string.Format("���Ʋ��������Ƚ��嵥 ͳ������:  {0} �� {1} ", this.dtpStatTimeBegin.Value.ToString("yyyy-MM-dd")
                    , this.dtpStatTimeEnd.Value.ToString("yyyy-MM-dd"));
            currRow.DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstDeptDocScoreInfo"></param>
        /// <param name="p"></param>
        /// <param name="p_3"></param>
        private void SetDeptScoreNum(List<EMRDBLib.DeptDocScoreInfo> lstDeptDocScoreInfo, string szDeptCode, string szScoreLeave)
        {
            foreach (EMRDBLib.DeptDocScoreInfo deptScore in lstDeptDocScoreInfo)
            {
                if (deptScore.DeptCode == szDeptCode)
                {
                    if (szScoreLeave.Equals("A"))
                    {
                        deptScore.ScoreANum += 1;
                    }
                    else if (szScoreLeave.Equals("B"))
                    {
                        deptScore.ScoreBNum += 1;
                    }
                    else if (szScoreLeave.Equals("C"))
                    {
                        deptScore.ScoreCNum += 1;
                    }
                }
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "���Ʋ��������Ƚ��嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("���Ʋ��������Ƚ��嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("���Ʋ��������Ƚ��嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "���ڿ���")
            {
                string szDeptName = null;
                if (!this.cboDeptName.Enabled)
                    szDeptName = "ȫԺ";
                else
                {
                    if (!string.IsNullOrEmpty(this.cboDeptName.Text.Trim()))
                        szDeptName = this.cboDeptName.Text;
                    else
                        szDeptName = "ȫԺ";
                }
                value = szDeptName;
                return true;
            }
            if (name == "�����")
            {
                if (!this.cboUserList.Enabled)
                    value = string.Empty;
                else
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

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɵ��������ݣ�", MessageBoxIcon.Information);
                return;
            }
            Hashtable htNoExportColunms = new Hashtable();
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "���Ʋ�������ͳ���嵥");
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
    }
}