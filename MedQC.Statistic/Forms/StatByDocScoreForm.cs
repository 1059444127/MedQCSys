// ***********************************************************
// ��������ͳ����ʾ����.
// Creator:LiChunYing  Date:2012-01-16
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using Heren.Common.Report;
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Utilities;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Statistic
{
    public partial class StatByDocScoreForm : DockContentBase
    {
        private Hashtable m_htPatVisitLog = null;
        ///// <summary>
        ///// �Ѿ����ֵĻ���
        ///// </summary>
        //private Hashtable m_htScoredPatVisitLog = null;
        public StatByDocScoreForm(MainForm mainForm)
            : base(mainForm)
        {
            InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
            InitalizeCombobox();
            this.chboxSimple.Checked = !SystemParam.Instance.LocalConfigOption.IsShowScoreAsSimple;
            this.chboxSimple.CheckedChanged += ChboxSimple_CheckedChanged;
        }

        private void ChboxSimple_CheckedChanged(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            //LoadDocScoreInfos();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void InitalizeCombobox()
        {
            //�Ƿ�������
            this.cbx.SelectedIndex = 0;
            //������
            int[] arrSocreLow = new int[] { 0, 5, 10, 15,20,25,30,35,40,45,50,
                                         55,60,65,70,75,80,85,90,95,100};
            int[] arrSocreHigh = new int[] { 0, 5, 10, 15,20,25,30,35,40,45,50,
                                         55,60,65,70,75,80,85,90,95,100};
            this.cbbScoreLow.DataSource = arrSocreLow;
            this.cbbScoreHigh.DataSource = arrSocreHigh;
            this.cbbScoreLow.SelectedIndex = 0;
            this.cbbScoreHigh.SelectedIndex = 20;
            this.cbbMrStatus.SelectedIndex = 0;
        }

        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.Update();
            InitalizeCombobox();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("�������ز���������Ϣ�����Ժ�...");
            //this.LoadDocScoreInfos();
            this.dataGridView1.Rows.Clear();
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
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
        /// �����б���Ϣ�ı�ʱˢ������
        /// </summary>
        protected override void OnPatientListInfoChanged()
        {
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this && this.NeedRefreshView)
                this.OnRefreshView();
        }

        /// <summary>
        /// ���ز���������Ϣ
        /// </summary>
        private void LoadDocScoreInfos()
        {
            this.dataGridView1.Rows.Clear();
            if (SystemParam.Instance.PatVisitLogList == null)
                return;
            if (this.m_htPatVisitLog == null)
                this.m_htPatVisitLog = new Hashtable();
            else
                this.m_htPatVisitLog.Clear();

            //if (this.m_htScoredPatVisitLog == null)
            //    this.m_htScoredPatVisitLog = new Hashtable();
            //else
            //    this.m_htScoredPatVisitLog.Clear();

            List<EMRDBLib.PatVisitInfo> lstPatVisitLogList = SystemParam.Instance.PatVisitLogList;
            bool bNeedChange = false;
            int iRowCount = 0;
            WorkProcess.Instance.Initialize(this, lstPatVisitLogList.Count, "���ڼ�������һ�������Ժ�");
            for (int index = 0; index < lstPatVisitLogList.Count; index++)
            {
                WorkProcess.Instance.Show(string.Format("�Ѿ�����{0}/{1}",index,lstPatVisitLogList.Count),index);
                if (WorkProcess.Instance.Canceled)
                {
                    WorkProcess.Instance.Close();
                    break;
                }
                Color backColor = Color.FromArgb(235, 235, 235);
                if (iRowCount % 2 == 0)
                    backColor = Color.White;
                if (!chboxSimple.Checked)//���Ǿ����Ͳ�������ɫ
                    backColor = Color.White;
                bNeedChange = this.LoadQCQuestionInfo(lstPatVisitLogList[index], backColor);
                if (bNeedChange)
                    iRowCount++;
            }
            //�����ȡ����ʾ������
            WorkProcess.Instance.Close();
            this.colDocName.Visible = chboxSimple.Checked;
            this.colQuestionList.Visible = chboxSimple.Checked;
            this.colDocChecker.Visible = chboxSimple.Checked;
        }

        /// <summary>
        /// ���ػ��ߵľ�����־��Ϣ
        /// </summary>
        /// <param name="patVisitLog">���߾�����־��Ϣ</param>
        /// <param name="backColor">�б���ɫ</param>
        /// <returns>�Ƿ���Ҫ������ɫ������</returns>
        private bool LoadQCQuestionInfo(EMRDBLib.PatVisitInfo patVisitLog, Color backColor)
        {
            List<EMRDBLib.MedicalQcMsg> lstQCQuestionInfo = null;
            short shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID, ref lstQCQuestionInfo);
            if (shRet != SystemData.ReturnValue.OK && shRet != SystemData.ReturnValue.RES_NO_FOUND)
                return false;
            #region �Ƿ���Ҫ�������ж�
            //��ȡ���ߵĲ����÷�
            EMRDBLib.QCScore qcScore = null;
            shRet = QcScoreAccess.Instance.GetQCScore(patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID, ref qcScore);
            if (shRet != SystemData.ReturnValue.OK || qcScore == null)
                return false;

            float fScore = -1;
            //Ժ������HosAssessĬ��ֵΪ-1�����HosAssess>=0����˻��ߵĲ����Ѿ������֡�
            if (qcScore.HOS_ASSESS >= 0)
            {
                fScore = qcScore.HOS_ASSESS;
            }
            //ɸѡ�����ֵķ���
            if (this.cbx.SelectedIndex == 1)
            {
                if (fScore >= 0)
                {
                    float scoreLow = 0.0f;
                    float.TryParse(this.cbbScoreLow.Text.ToString(), out scoreLow);
                    float scoreHigh = 0.0f;
                    float.TryParse(this.cbbScoreHigh.Text.ToString(), out scoreHigh);
                    if (fScore > scoreHigh || fScore < scoreLow)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else if (this.cbx.SelectedIndex == 2)//δ����
            {
                if (fScore > 0)
                    return false;
            }

            //�ύ״̬ ��ĸoΪδ�ύ
            if (this.cbbMrStatus.SelectedItem.ToString() == "δ�ύ" && patVisitLog.MR_STATUS.ToUpper() != "O")
            {
                return false;
            }
            else if (this.cbbMrStatus.SelectedItem.ToString() == "���ύ" && patVisitLog.MR_STATUS.ToUpper() == "O")
            {
                return false;
            }

            #endregion

            #region ����δ�����Ļ��߻�����Ϣ
            if (lstQCQuestionInfo == null || lstQCQuestionInfo.Count <= 0)
            {
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.DefaultCellStyle.BackColor = backColor;
                row.Cells[this.colPatientID.Index].Value = patVisitLog.PATIENT_ID;
                row.Cells[this.colPatName.Index].Value = patVisitLog.PATIENT_NAME;
                row.Cells[this.colDoctorInCharge.Index].Value = patVisitLog.INCHARGE_DOCTOR;
                row.Cells[this.colPARENT_DOCTOR.Index].Value = patVisitLog.AttendingDoctor;
                row.Cells[this.colDeptAdmissionTo.Index].Value = patVisitLog.DEPT_NAME;
                row.Cells[this.colAdmissionDate.Index].Value = patVisitLog.VISIT_TIME;
                row.Cells[this.colDeptDischargeFrom.Index].Value = patVisitLog.DischargeDeptCode;
                row.Cells[this.colMrStatus.Index].Value = patVisitLog.MR_STATUS.ToUpper() == "O" ? "δ�ύ" : "���ύ";
                if (patVisitLog.DISCHARGE_TIME != patVisitLog.DefaultTime)
                    row.Cells[this.colDischargeDate.Index].Value = patVisitLog.DISCHARGE_TIME;
                //��ʾȨ�޸ĵ��ʿ�Ȩ�޿���
                //if (SystemConfig.Instance.Get(SystemData.ConfigKey.STAT_SHOW_CHECKER_NAME, false))

                row.Cells[this.colDocChecker.Index].Value = qcScore.HOS_QCMAN;

                row.Cells[this.colScore.Index].Value = fScore >= 0 ? fScore.ToString() : string.Empty;
                row.Cells[this.colDocLevel.Index].Value = qcScore.DOC_LEVEL;
                row.Tag = patVisitLog;
                if (!this.m_htPatVisitLog.ContainsKey(patVisitLog))
                    this.m_htPatVisitLog.Add(patVisitLog, row);
                return true;
            }
            #endregion

            #region �����ʿ���Ϣ
            for (int index = 0; index < lstQCQuestionInfo.Count; index++)
            {
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.DefaultCellStyle.BackColor = backColor;
                row.Tag = patVisitLog;
                if (index == 0)
                {
                    row.Cells[this.colPatientID.Index].Value = patVisitLog.PATIENT_ID;
                    row.Cells[this.colPatName.Index].Value = patVisitLog.PATIENT_NAME;
                    row.Cells[this.colDeptAdmissionTo.Index].Value = patVisitLog.DEPT_NAME;
                    row.Cells[this.colAdmissionDate.Index].Value = patVisitLog.VISIT_TIME.ToString("yyyy-MM-dd");
                    row.Cells[this.colDoctorInCharge.Index].Value = patVisitLog.INCHARGE_DOCTOR;
                    row.Cells[this.colPARENT_DOCTOR.Index].Value = patVisitLog.AttendingDoctor;
                    row.Cells[this.colDeptDischargeFrom.Index].Value = patVisitLog.DischargeDeptCode;
                    row.Cells[this.colMrStatus.Index].Value = patVisitLog.MR_STATUS.ToUpper() == "O" ? "δ�ύ" : "���ύ";
                    if (patVisitLog.DISCHARGE_TIME != patVisitLog.DefaultTime)
                        row.Cells[this.colDischargeDate.Index].Value = patVisitLog.DISCHARGE_TIME.ToString("yyyy-MM-dd");
                    row.Cells[this.colScore.Index].Value = fScore >= 0 ? fScore.ToString() : string.Empty;
                    row.Cells[this.colDocLevel.Index].Value = qcScore.DOC_LEVEL;
                    if (!this.m_htPatVisitLog.ContainsKey(patVisitLog))
                        this.m_htPatVisitLog.Add(patVisitLog, row);
                    //����Ǿ��������
                    if (!chboxSimple.Checked)
                        break;
                }
                row.Cells[this.colDocChecker.Index].Value = lstQCQuestionInfo[index].ISSUED_BY;
                row.Cells[this.colDocName.Index].Value = lstQCQuestionInfo[index].TOPIC;
                row.Cells[this.colQuestionList.Index].Value = lstQCQuestionInfo[index].MESSAGE;
                row.Cells[this.colDocSetID.Index].Value = lstQCQuestionInfo[index].TOPIC_ID;
            }
            #endregion
            return true;
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "��������ͳ���嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("��������ͳ���嵥����û������!");
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
                if (SystemParam.Instance.PatVisitLogList == null)
                    value = string.Empty;
                else
                    value = SystemParam.Instance.PatVisitLogList[0].DEPT_NAME;
                return true;
            }
            return false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɵ��������ݣ�", MessageBoxIcon.Information);
                return;
            }
            Hashtable htNoExportColunms = new Hashtable();
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "�����÷��嵥");
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

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewCell currCell = this.dataGridView1.CurrentCell;
            if (currCell == null)
                return;
            if (currCell.ColumnIndex != this.colScore.Index)
                return;
            if (SystemParam.Instance.PatVisitLogTable == null)
                return;

            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            EMRDBLib.PatVisitInfo patVisitLog = row.Tag as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;

            SystemParam.Instance.PatVisitInfo = patVisitLog;


            this.MainForm.ShowDocScoreForm();
            if (this.MainForm.DocScoreForm != null)
                this.MainForm.DocScoreForm.DocScoreSaved += new EventHandler(DocScoreForm_DocScoreSaved);
        }

        private void DocScoreForm_DocScoreSaved(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            if (this.MainForm.DocScoreForm == null || this.MainForm.DocScoreForm.IsDisposed)
                return;
            if (this.m_htPatVisitLog == null || this.m_htPatVisitLog.Count <= 0)
                return;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            DataGridViewRow selectRow = null;
            if (this.m_htPatVisitLog.Contains(SystemParam.Instance.PatVisitInfo))
                selectRow = this.m_htPatVisitLog[SystemParam.Instance.PatVisitInfo] as DataGridViewRow;
            if (selectRow == null)
                return;

            selectRow.Cells[this.colScore.Index].Value = this.MainForm.DocScoreForm.DocScore;
            selectRow.Cells[this.colDocLevel.Index].Value = this.MainForm.DocScoreForm.DocLevel;
            selectRow.Cells[this.colDocChecker.Index].Value = this.MainForm.DocScoreForm.QcChecker;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (SystemParam.Instance.PatVisitLogList == null || SystemParam.Instance.PatVisitLogList.Count == 0)
            {
                MessageBoxEx.Show("�����б�Ϊ�գ��������ѡ����!");
                return;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("�������ز���������Ϣ�����Ժ�...");
            this.LoadDocScoreInfos();
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbx.SelectedIndex == 1)
            {
                this.label1.Visible = true;
                this.label2.Visible = true;
                this.cbbScoreLow.Visible = true;
                this.cbbScoreHigh.Visible = true;
            }
            else
            {
                this.label1.Visible = false;
                this.label2.Visible = false;
                this.cbbScoreLow.Visible = false;
                this.cbbScoreHigh.Visible = false;
            }
        }

        private void cbbScoreLow_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleScoreKeyPress(sender, e);
        }

        private void HandleScoreKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //��ֹ�ո��  
            if ((e.KeyChar == 0x2D) && (((ComboBox)sender).Text.Length == 0)) return;   //������  
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((ComboBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //����Ƿ��ַ�  
                }
            }
        }

        private void cbbScoreHigh_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleScoreKeyPress(sender, e);
        }

        private void cbbScoreLow_MouseLeave(object sender, EventArgs e)
        {
            HandleScoreMouseLeave(sender, e);
        }

        private void cbbScoreHigh_MouseLeave(object sender, EventArgs e)
        {
            HandleScoreMouseLeave(sender, e);
        }

        private void HandleScoreMouseLeave(object sender, EventArgs e)
        {
            try
            {
                ComboBox comBox = (ComboBox)sender;
                string szText = comBox.Text;
                if (string.IsNullOrEmpty(szText))
                    comBox.Text = "0.0";
                int result = 0;
                if (szText.Contains("."))
                    result = szText.Length - szText.IndexOf('.') - 1;
                if (result > 1)
                {
                    MessageBox.Show("�������辫ȷ��С����һλ");
                    comBox.Text = szText.Remove(szText.IndexOf('.') + 2, result - 1);
                }
                float scoreLow = 0.0f;
                if (!float.TryParse(szText.ToString(), out scoreLow))
                {
                    MessageBox.Show("��������Ч�ķ�����");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("��������Ч�ķ�����");
            }
        }

        /// <summary>
        /// ˫���򿪲�������ͳ�Ʊ�������˫��ʱ�䣬�����������б���DocSetID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            EMRDBLib.PatVisitInfo patVisitLog = row.Tag as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;
            string szDocSetID = this.dataGridView1.Rows[e.RowIndex].Cells[this.colDocSetID.Index].Value != null
                ? this.dataGridView1.Rows[e.RowIndex].Cells[this.colDocSetID.Index].Value.ToString()
                : string.Empty;
            this.MainForm.OpenDocument(szDocSetID, patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID);
        }

    }
}