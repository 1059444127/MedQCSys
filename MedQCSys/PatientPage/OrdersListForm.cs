// ***********************************************************
// ҽ���б���ʾ����.
// Creator:YangMingkun  Date:2009-11-7
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
using EMRDBLib.Entity;
using EMRDBLib;
using Heren.MedQC.Utilities;
using Heren.MedQC.Core;

namespace MedQCSys.DockForms
{
    public partial class OrdersListForm : DockContentBase
    {
        public OrdersListForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.HideOnClose = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.CloseButtonVisible = true;
            this.dataGridView1.Font = new Font("����", 10.5f);
        }

        public OrdersListForm(MainForm parent, PatPage.PatientPageControl patientPageControl)
            : base(parent, patientPageControl)
        {
            this.InitializeComponent();
            this.HideOnClose = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.CloseButtonVisible = true;
            this.dataGridView1.Font = new Font("����", 10.5f);
        }
        /// <summary>
        /// ��ʶֵ�ı��¼��Ƿ����,�����ظ�ִ��
        /// </summary>
        private bool m_bValueChangedEnabled = true;
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!SystemParam.Instance.LocalConfigOption.PrintAndExcel)
            {
                this.btnExport.Visible = false;
                this.btnPrint.Visible = false;
            }
            this.m_bValueChangedEnabled = false;
            if (this.toolcboOrdersType.Items.Count > 0)
                this.toolcboOrdersType.SelectedIndex = 0;
            
            this.dataGridView1.GroupHeight = 0;
            int i = this.dataGridView1.Groups.Add();
            this.dataGridView1.Groups[i].BeginColumn = 3;
            this.dataGridView1.Groups[i].EndColumn = 7;
            this.dataGridView1.Groups[i].Text = "ҽ������";
            this.dataGridView1.Groups[i].BackColor = Color.White;
            this.m_bValueChangedEnabled = true;
        }
        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("��������ҽ����¼�����Ժ�...");
            //ҽ���´�ҽ����־
            int nOrderFlag = 0;
            this.LoadOrderInfoList(nOrderFlag);
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
        /// ����ҽ���б���ʾ��edited by FYB in 20091117
        /// <param name="nOrderFlag">ҽ����ʶ(0-ҽ���´��ҽ�� 1-��ʿת����ҽ��)</param>
        /// </summary>
        private void LoadOrderInfoList(int nOrderFlag)
        {
            this.dataGridView1.Rows.Clear();
            this.lblOrdersInfo.Text = "����ִ�е�ҽ����Ŀ��0 ��";
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;
            bool bIsRepeat = this.toolcboOrdersType.SelectedIndex == 1;
            String bIsRepeatAll = this.toolcboOrdersType.Text;
            string szOrderText = this.tbOrderText.Text;
            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            List<MedOrderInfo> lstOrderInfo = null;
            if (GlobalMethods.Misc.IsEmptyString(szPatientID) || GlobalMethods.Misc.IsEmptyString(szVisitID))
            {
                return;
            }
            short shRet = OrdersAccess.Instance.GetInpOrderList(szPatientID, szVisitID, szOrderText, nOrderFlag, ref lstOrderInfo);
            if (shRet != SystemData.ReturnValue.OK && shRet != SystemData.ReturnValue.CANCEL
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("����ҽ���б�ʧ�ܣ�");
                return;
            }
            if (lstOrderInfo == null || lstOrderInfo.Count <= 0)
            {
                return;
            }
            lstOrderInfo.Sort(new Comparison<MedOrderInfo>(this.ComparisonOrderInfo));
            DataGridViewRow row = null;
            int nRowIndex = 0;
            int nUnStopedOrderCount = 0;
            for (int index = lstOrderInfo.Count - 1; index >= 0; index--)
            {
                MedOrderInfo orderInfo = lstOrderInfo[index];
                if (bIsRepeatAll != "ȫ��")
                {
                    if (orderInfo.IsRepeat != bIsRepeat)
                        continue;
                }
                if (orderInfo != null && orderInfo.OrderSubNO == "1")
                {
                    nRowIndex = this.dataGridView1.Rows.Add();
                    row = this.dataGridView1.Rows[nRowIndex];
                    row.Cells[this.colBeginTime.Index].Value = orderInfo.EnterTime.ToString("yyyy-MM-dd HH:mm");
                    string orderText = string.Empty;
                    if (!string.IsNullOrEmpty(orderInfo.Frequency))
                        orderText = orderInfo.OrderText.Replace(orderInfo.Frequency, "");
                    else
                        orderText = orderInfo.OrderText;
                    row.Cells[this.colOrderContent.Index].Value = orderText;
                    if (orderInfo.Dosage > 0)
                        row.Cells[this.colDosage.Index].Value = string.Concat(orderInfo.Dosage, orderInfo.DosageUnits);
                    if (orderInfo.START_DATE_TIME != orderInfo.DefaultTime)
                        row.Cells[this.col_START_DATE_TIME.Index].Value = orderInfo.START_DATE_TIME.ToString("yyyy-MM-dd HH:mm");
                    row.Cells[this.colAdministration.Index].Value = orderInfo.Administration;
                    row.Cells[this.colFrequency.Index].Value = orderInfo.Frequency;
                    row.Cells[this.colRepeatIndicator.Index].Value = orderInfo.IsRepeat ? "����" : "��ʱ";
                    row.Cells[this.colOrderClass.Index].Value = orderInfo.OrderClass;
                    row.Cells[this.colOrderDoctor.Index].Value = orderInfo.Doctor;
                    row.Cells[this.colNurse.Index].Value = orderInfo.Nurse;
                    if (orderInfo.StopTime != orderInfo.DefaultTime
                        && orderInfo.StopTime != orderInfo.DefaultTime3)
                    {
                        row.Cells[this.colStopTime.Index].Value = orderInfo.StopTime.ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        row.Cells[this.colStopTime.Index].Value = string.Empty;

                    }
                    row.Cells[this.colOrderStatus.Index].Value = SystemData.OrderStatus.GetOrderStatusDesc(orderInfo.OrderStatus);
                    if (orderInfo.OrderStatus == "4")
                    {
                        row.Cells[this.colOrderStatus.Index].Style.ForeColor = Color.Red;
                    }
                    else if (orderInfo.OrderStatus == "2")
                    {
                        if (orderInfo.StopTime != orderInfo.DefaultTime
                            && orderInfo.StopTime < DateTime.Now)
                        {
                            row.Cells[this.colOrderStatus.Index].Value = "ֹͣ";
                        }
                        else
                        {
                            nUnStopedOrderCount++;
                        }
                    }
                    //if (orderInfo.OrderStatus == "2")//����ִ��״̬Ϊ2
                    row.Tag = orderInfo.OrderNO;
                }
            }

            for (int index = 0; index < lstOrderInfo.Count; index++)
            {
                MedOrderInfo orderInfo = lstOrderInfo[index];
                if (orderInfo != null && orderInfo.OrderSubNO != "1")
                {
                    nRowIndex = -1;
                    for (int rowIndex = 0; rowIndex < this.dataGridView1.Rows.Count; rowIndex++)
                    {
                        DataGridViewRow tmpRow = this.dataGridView1.Rows[rowIndex];
                        if ((string)tmpRow.Tag == orderInfo.OrderNO)
                        {
                            tmpRow.Cells[this.colflag.Index].Value = "��";
                            nRowIndex = rowIndex;
                            break;
                        }
                    }
                    if (nRowIndex < 0)
                        break;
                    while (nRowIndex < this.dataGridView1.Rows.Count - 1)
                    {
                        DataGridViewRow tmpRow = this.dataGridView1.Rows[nRowIndex + 1];
                        if (((string)tmpRow.Tag == "-1") && (int.Parse((string)tmpRow.Cells[this.colBeginTime.Index].Tag) < int.Parse(orderInfo.OrderSubNO)))
                        {
                            nRowIndex = nRowIndex + 1;
                            tmpRow.Cells[this.colflag.Index].Value = "��";
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    this.dataGridView1.Rows.Insert(nRowIndex + 1, 1);
                    row = this.dataGridView1.Rows[nRowIndex + 1];
                    row.Cells[this.colBeginTime.Index].Value = string.Empty;
                    row.Cells[this.colOrderContent.Index].Value = orderInfo.OrderText;
                    row.Cells[this.colDosage.Index].Value = string.Concat(orderInfo.Dosage, orderInfo.DosageUnits);
                    row.Cells[this.colRepeatIndicator.Index].Value = string.Empty;
                    row.Cells[this.colOrderDoctor.Index].Value = orderInfo.Doctor;
                    row.Cells[this.colStopTime.Index].Value = string.Empty;
                    row.Cells[this.colflag.Index].Value = "��";
                    row.Tag = "-1";
                    row.Cells[this.colBeginTime.Index].Tag = orderInfo.OrderSubNO;
                }
            }
            //���ݸ���ҽԺ,��ҽ������ʧ��ʱִ��
            if (this.dataGridView1.Rows.Count <= 0)
            {
                string szPreOrderNo = string.Empty;
                for (int index = lstOrderInfo.Count - 1; index >= 0; index--)
                {
                    MedOrderInfo orderInfo = lstOrderInfo[index];
                    if (orderInfo != null)
                    {
                        nRowIndex = this.dataGridView1.Rows.Add();
                        row = this.dataGridView1.Rows[nRowIndex];
                        if (szPreOrderNo != orderInfo.OrderNO)
                        {
                            //�µ�һ��ҽ��
                            szPreOrderNo = orderInfo.OrderNO;
                            row.Cells[this.colBeginTime.Index].Value = orderInfo.EnterTime.ToString("yyyy-MM-dd HH:mm");
                            row.Cells[this.colRepeatIndicator.Index].Value = orderInfo.IsRepeat ? "����" : "��ʱ";
                            row.Cells[this.colAdministration.Index].Value = orderInfo.Administration;
                            row.Cells[this.colFrequency.Index].Value = orderInfo.Frequency;
                            DataGridViewCellStyle cellstyle = new DataGridViewCellStyle();

                            row.Cells[this.colFrequency.Index].Style = new DataGridViewCellStyle();
                        }

                        row.Cells[this.colOrderContent.Index].Value = orderInfo.OrderText;
                        if (orderInfo.Dosage > 0)
                            row.Cells[this.colDosage.Index].Value = string.Concat(orderInfo.Dosage, orderInfo.DosageUnits);


                        row.Cells[this.colOrderDoctor.Index].Value = orderInfo.Doctor;
                        if (orderInfo.StopTime != orderInfo.DefaultTime)
                        {
                            row.Cells[this.colStopTime.Index].Value = orderInfo.StopTime.ToString("yyyy-MM-dd HH:mm");
                            row.DefaultCellStyle.ForeColor = Color.Blue;
                        }
                        else
                        {
                            row.Cells[this.colStopTime.Index].Value = string.Empty;
                            nUnStopedOrderCount++;
                        }
                        row.Tag = orderInfo.OrderNO;
                    }
                }
            }
            this.dataGridView1.Tag = string.Format("����{0}��{1}�ξ���ĵ�ҽ����Ϣ�굥", szPatientID, szVisitID);
            this.lblOrdersInfo.Text = string.Format("����ִ�е�ҽ����Ŀ��{0} ��", nUnStopedOrderCount);
        }

        private int ComparisonOrderInfo(MedOrderInfo medOrderInfoA, MedOrderInfo medOrderInfoB)
        {
            if (medOrderInfoA == null && medOrderInfoB != null)
                return 1;
            if (medOrderInfoA != null && medOrderInfoB == null)
                return -1;
            if (medOrderInfoA == null && medOrderInfoB == null)
                return 0;
            int i = DateTime.Compare(medOrderInfoB.EnterTime, medOrderInfoA.EnterTime);
            return i;
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
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "ҽ����Ϣ�嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("ҽ����Ϣ�嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("ҽ����Ϣ�嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "��������")
            {
                value = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
                return true;
            }
            if (name == "����ID")
            {
                value = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
                return true;
            }
            if (name == "���ڿ���")
            {
                value = SystemParam.Instance.PatVisitInfo.DEPT_NAME;
                return true;
            }
            return false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɴ�ӡ���ݣ�");
                return;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            ReportType reportType = ReportCache.Instance.GetWardReportType(SystemData.ReportTypeApplyEnv.ORDERS, this.Text);
            if (reportType == null)
            {
                MessageBoxEx.ShowMessage("��ӡ����û������");
                return;
            }
            byte[] byteReportData = null;
            ReportCache.Instance.GetReportTemplet(reportType, ref byteReportData);
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
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "ҽ�������嵥");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void ckbSwitch_CheckedChanged(object sender, EventArgs e)
        {
            int nOrderFlag = 0;
            //if (this.ckbSwitch.Checked)
            //    nOrderFlag = 1;
            this.LoadOrderInfoList(nOrderFlag);
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

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            int nOrderFlag = 0;
            this.LoadOrderInfoList(nOrderFlag);
        }

        private void OrdersListForm_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbOrderText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.LoadOrderInfoList(0);
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex >= 3 && e.ColumnIndex <= 7) & e.RowIndex > -1)
            {
                using (var rowPen = new Pen(Color.Gray, 1))
                using (var gridlinePen = new Pen(Color.White, 1))
                {
                    var topLeftPoint = new Point(e.CellBounds.Left - 1, e.CellBounds.Top - 1);
                    var topRightPoint = new Point(e.CellBounds.Right - 1, e.CellBounds.Top - 1);
                    var bottomRightPoint = new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                    var bottomLeftPoint = new Point(e.CellBounds.Left - 1, e.CellBounds.Bottom - 1);
                    e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    if (e.ColumnIndex == 3)
                    {
                        e.Graphics.DrawLine(rowPen, bottomLeftPoint, topLeftPoint);
                        e.Graphics.DrawLine(gridlinePen, bottomRightPoint, topRightPoint);

                    }
                    else if (e.ColumnIndex == 7)
                    {
                        e.Graphics.DrawLine(gridlinePen, bottomLeftPoint, topLeftPoint);
                        e.Graphics.DrawLine(rowPen, bottomRightPoint, topRightPoint);
                    }
                    else
                    {
                        e.Graphics.DrawLine(gridlinePen, bottomRightPoint, topRightPoint);
                        e.Graphics.DrawLine(gridlinePen, bottomLeftPoint, topLeftPoint);
                    }
                    e.Graphics.DrawLine(rowPen, bottomRightPoint, bottomLeftPoint);
                    e.Graphics.DrawLine(rowPen, topLeftPoint, topRightPoint);
                    e.Handled = true;
                }

            }
        }

        private void toolcboOrdersType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.m_bValueChangedEnabled)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.LoadOrderInfoList(0);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
    }
}