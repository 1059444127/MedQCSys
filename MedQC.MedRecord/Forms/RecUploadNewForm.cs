// �����ʿ�ϵͳ������Ϣ����.
// Creator:LiChunYing  Date:2011-09-28
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.DockSuite;

using EMRDBLib.DbAccess;
using EMRDBLib.Entity;
using EMRDBLib;
using Heren.MedQC.Core;
using MedQCSys.DockForms;
using MedQCSys;
using System.Linq;
using Heren.Common.Forms.Editor;
using Heren.MedQC.Utilities;
using Heren.Common.Controls.TableView;
using Heren.MedQC.Core.Services;

namespace Heren.MedQC.MedRecord
{
    public partial class RecUploadNewForm : DockContentBase
    {
        public RecUploadNewForm(MainForm mainForm)
            : base(mainForm)
        {
            InitializeComponent();
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;

        }

        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.dtpTimeBegin.Value = DateTime.Now.AddMonths(-1);
            this.dtpTimeEnd.Value = DateTime.Now;

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
        /// ������Ϣ�ı䷽����д
        /// </summary>
        protected override void OnPatientInfoChanged()
        {
            if (this.IsHidden)
                return;

            if (this.DockState == DockState.DockBottomAutoHide
                || this.DockState == DockState.DockTopAutoHide
                || this.DockState == DockState.DockLeftAutoHide
                || this.DockState == DockState.DockRightAutoHide)
            {
                if (SystemParam.Instance.PatVisitInfo != null)
                    this.DockHandler.Activate();
            }
            this.Update();

            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this)
                this.OnRefreshView();
        }
        private void XTextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool multiSelected = false;

            multiSelected = GlobalMethods.Convert.StringToValue("true", false);
            DataTable table = this.txt_DEPT_NAME.Tag as DataTable;
            if (table == null)
                table = new DataTable();
            table = UtilitiesHandler.Instance.ShowDeptSelectDialog(0, true, table);
            this.txt_DEPT_NAME.Tag = table;
            string strDeptNames = string.Empty;
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow item in table.Rows)
                {
                    if (strDeptNames == string.Empty)
                        strDeptNames = item[1].ToString();
                    else
                        strDeptNames = strDeptNames + ";" + item[1].ToString();
                }
            }
            else
            {
                strDeptNames = "ȫԺ<��˫���޸�>";
            }
            this.txt_DEPT_NAME.Text = strDeptNames;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            this.XDataGrid1.Rows.Clear();
            this.chkAll.Checked = false;
            DateTime dtTimeBegin = dtpTimeBegin.Value;
            DateTime dtTimeEnd = dtpTimeEnd.Value;
            string patientID = txt_PATIENT_ID.Text.Trim();
            string szStatus = cboStatus.Text;
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(" select t.patient_id,t.visit_id,t.visit_no,t.patient_name,diagnosis,patient_sex,visit_time,discharge_time,dept_name,incharge_doctor,t2.status");
            sbSql.Append(" from pat_visit_v t, rec_upload t2");
            sbSql.Append(" where 1=1 and t.patient_id = t2.patient_id(+) and t.visit_id = t2.visit_id(+)");
            if (cboTimeType.Text == "��Ժ����")
            {
                sbSql.AppendFormat(" and t.VISIT_TIME >= to_date('{0}',  'yyyy-MM-dd HH24:mi:ss') and t.visit_time <= to_date('{1}', 'yyyy-MM-dd HH24:mi:ss')"
                    , dtTimeBegin.ToString()
                    , dtTimeEnd.ToString());
            }
            else
            {
                sbSql.AppendFormat(" and t.discharge_time >= to_date('{0}',  'yyyy-MM-dd HH24:mi:ss') and t.discharge_time <= to_date('{1}', 'yyyy-MM-dd HH24:mi:ss')"
                    , dtTimeBegin.ToString()
                    , dtTimeEnd.ToString());
            }
            DataTable dtWardList = this.txt_DEPT_NAME.Tag as DataTable;
            if (dtWardList != null && dtWardList.Rows.Count > 0)
            {
                sbSql.AppendFormat(" and t.DEPT_CODE in ({0}) ", GetStrWards(dtWardList));
            }
            if (!string.IsNullOrEmpty(patientID))
            {
                sbSql.Append(" and t.patient_id = '" + patientID + "'");
            }
            if (szStatus == "δ�ϴ�")
            {
                sbSql.Append(" and t2.status is null ");
            }
            else
            {
                sbSql.Append(" and t2.status = 1");
            }
            DataSet ds = null;
            short shRet = CommonAccess.Instance.ExecuteQuery(sbSql.ToString(), out ds);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    int i = this.XDataGrid1.Rows.Add();
                    DataTableViewRow r = this.XDataGrid1.Rows[i];
                    r.Cells[this.col_����.Index].Value = item["patient_name"];
                    r.Cells[this.col_�ϴ�.Index].Value = item["status"].ToString() == "1" ? "���ϴ�" : "�ϴ�";
                    r.Cells[this.col_����ҽ��.Index].Value = item["incharge_doctor"];
                    r.Cells[this.col_��Ժʱ��.Index].Value = item["visit_time"];
                    r.Cells[this.col_��Ժ��.Index].Value = item["visit_id"];
                    r.Cells[this.col_��Ժʱ��.Index].Value = item["discharge_time"];
                    r.Cells[this.col_������ˮ��.Index].Value = item["visit_no"];
                    r.Cells[this.col_�Ա�.Index].Value = item["patient_sex"];
                    r.Cells[this.col_����ID��.Index].Value = item["patient_id"];
                    r.Cells[this.col_����.Index].Value = item["dept_name"];
                    r.Cells[this.col_���.Index].Value = item["diagnosis"];

                }
            }
            else
            {
                this.lbl_msg.Text = string.Format("δ�ҵ����߲���");
                return;
            }
            this.lbl_msg.Text = string.Format("��{0}�ݻ��߲���", ds.Tables[0].Rows.Count);
        }

        ///��ѡ��Ĳ���װ�����ַ��� ���ڲ�ѯ
        private string GetStrWards(DataTable dtWardInfos)
        {
            string strwards = string.Empty;
            foreach (DataRow dr in dtWardInfos.Rows)
            {
                strwards += "'" + dr[0].ToString() + "',";
            }
            if (string.IsNullOrEmpty(strwards))
                return string.Empty;
            strwards = strwards.Substring(0, strwards.Length - 1);
            return strwards;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (this.XDataGrid1.Rows.Count <= 0)
                return;

            WorkProcess.Instance.Initialize(this, this.XDataGrid1.Rows.Count, "�����ϴ�....");
            foreach (DataTableViewRow row in this.XDataGrid1.Rows)
            {
                WorkProcess.Instance.Show(row.Index, true);
                RecUploadLog(row.Index);
            }
            WorkProcess.Instance.Close();
        }

        private void RecUploadLog(int index)
        {
            string patient_id = this.XDataGrid1.Rows[index].Cells[col_����ID��.Index].Value.ToString();
            string visit_id = this.XDataGrid1.Rows[index].Cells[col_��Ժ��.Index].Value.ToString();
            string visit_no = this.XDataGrid1.Rows[index].Cells[this.col_������ˮ��.Index].Value.ToString();
            string patient_name = this.XDataGrid1.Rows[index].Cells[this.col_����.Index].Value.ToString();
            List<RecUpload> lstRecUpload = null;
            short shRet = RecUploadAccess.Instance.GetRecUploads(patient_id, visit_no, ref lstRecUpload);
            if (lstRecUpload == null)
            {
                RecUpload recUpload = new RecUpload();
                recUpload.UPLOAD_ID = recUpload.MakeID();
                recUpload.PATIENT_ID = patient_id;
                recUpload.PATIENT_NAME = patient_name;
                recUpload.STATUS = 1;
                recUpload.VISIT_ID = visit_id;
                recUpload.VISIT_NO = visit_no;
                shRet = RecUploadAccess.Instance.Insert(recUpload);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.Show("�ϴ�ʧ��");
                    return;
                }
                this.XDataGrid1.Rows[index].Cells[this.col_�ϴ�.Index].Value = "���ϴ�";
                this.XDataGrid1.Rows[index].Cells[this.col_�ϴ�.Index].Tag = recUpload;
            }
            bool result = RecUploadService.Instance.Upload(patient_id, visit_id);
        }

        private void XDataGrid1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == this.col_�ϴ�.Index)
            {
                RecUploadLog(e.RowIndex);
            }
            if (e.ColumnIndex == this.col_Chk.Index)
            {
                if (this.XDataGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null
                    || this.XDataGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToLower() == "false")
                {
                    this.XDataGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
                }
                else
                {
                    this.XDataGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                }
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.XDataGrid1.Rows.Count <= 0)
                return;
            if (this.chkAll.Checked)
            {
                foreach (DataTableViewRow item in this.XDataGrid1.Rows)
                {
                    item.Cells[this.col_Chk.Index].Value = true;
                }
            }
            else
            {
                foreach (DataTableViewRow item in this.XDataGrid1.Rows)
                {
                    item.Cells[this.col_Chk.Index].Value = false;
                }
            }

        }

        private void XDataGrid1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            string patient_id = this.XDataGrid1.Rows[e.RowIndex].Cells[this.col_����ID��.Index].Value.ToString();
            string visit_id = this.XDataGrid1.Rows[e.RowIndex].Cells[this.col_��Ժ��.Index].Value.ToString();
            PatVisitInfo patVisit = new PatVisitInfo() { PATIENT_ID = patient_id, VISIT_ID = visit_id };
            this.MainForm.ShowPatientPageForm(patVisit);
        }
    }
}