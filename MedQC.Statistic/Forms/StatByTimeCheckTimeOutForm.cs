using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;
using Heren.Common.Report;
using EMRDBLib.DbAccess;
using System.Collections;
using Heren.MedQC.Utilities;
using MedQCSys;
using EMRDBLib;
using MedQCSys.Dialogs;

namespace Heren.MedQC.Statistic
{
    public partial class StatByTimeCheckTimeOutForm : MedQCSys.DockForms.DockContentBase
    {
        public StatByTimeCheckTimeOutForm(MainForm parent)
            : base(parent)
        {
            InitializeComponent();

            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.dataGridView1.Font = new Font("����", 10.5f);
            this.ShowStatusMessage("���������ٴ������б����Ժ�...");
            if (!InitControlData.InitCboDeptName(ref this.cboDeptName))
            {
                MessageBoxEx.Show("���ؿ����б�ʧ��");
            }
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            //��ʼ�������������
            string sql = "select distinct doctype_name from qc_time_record_t";
            DataSet ds = null;
            short shRet = CommonAccess.Instance.ExecuteQuery(sql, out ds);
            if (shRet != EMRDBLib.SystemData.ReturnValue.OK
                || ds == null
                || ds.Tables[0].Rows.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
                column1.Name = ds.Tables[0].Rows[i][0].ToString() + "(��)";
                column1.HeaderText = ds.Tables[0].Rows[i][0].ToString() + "(��)";
                column1.Width = 100;
                column1.SortMode = DataGridViewColumnSortMode.NotSortable;
                column1.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                column1.Visible = false;
                this.dataGridView1.Columns.Add(column1);
                DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
                column2.Name = ds.Tables[0].Rows[i][0].ToString() + "(��ʱ)";
                column2.HeaderText = ds.Tables[0].Rows[i][0].ToString() + "(��ʱ)";
                column2.Width = 100;
                column2.SortMode = DataGridViewColumnSortMode.NotSortable;
                column2.Visible = false;
                column2.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                this.dataGridView1.Columns.Add(column2);
                DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
                column3.Name = ds.Tables[0].Rows[i][0].ToString() + "(��ʱ��)";
                column3.HeaderText = ds.Tables[0].Rows[i][0].ToString() + "(��ʱ��)";
                column3.Width = 100;
                column3.SortMode = DataGridViewColumnSortMode.NotSortable;
                column3.Visible = false;
                column3.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                this.dataGridView1.Columns.Add(column3);
            }
            //���Ӻ���ͳ�� �������� ������ʱ���� ������ʱ��
            DataGridViewTextBoxColumn column4 = new DataGridViewTextBoxColumn();

            column4.Name = "��������";
            column4.HeaderText = "��������";
            column4.Width = 80;
            column4.Visible = true;
            column4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns.Add(column4);
            DataGridViewTextBoxColumn column5 = new DataGridViewTextBoxColumn();
            column5.Name = "������ʱ����";
            column5.HeaderText = "������ʱ����";
            column5.Width = 80;
            column5.Visible = true;
            column5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns.Add(column5);
            DataGridViewTextBoxColumn column6 = new DataGridViewTextBoxColumn();
            column6.Name = "������ʱ��";
            column6.HeaderText = "������ʱ��";
            column6.Width = 80;
            column6.Visible = true;
            column6.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.Columns.Add(column6);

            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            this.ShowStatusMessage(null);
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SetColumnVisable();
            if (this.cboType.Text == "����")
                this.StatTimeCheckByDept();
            if (this.cboType.Text == "ҽ��")
                this.StatTimeCheckByDoctor();
            if (this.cboType.Text == "����")
                this.StatTimeCheckByPatient();
            CalTotal();

        }

        private void StatTimeCheckByDept()
        {
            this.dataGridView1.Rows.Clear();
            DateTime dtBeginTime = DateTime.Parse(this.dtpBeginTime.Value.ToShortDateString());
            DateTime dtEndTime = DateTime.Parse(this.dtpEndTime.Value.AddDays(1).ToShortDateString());
            string szTimeType = string.Empty;
            if (rbtCheckTime.Checked)
                szTimeType = "Check_date";
            else
                szTimeType = "end_date";
            string szDeptCode = string.Empty;
            string szQcResult = string.Empty;
            if (this.cboDeptName.SelectedItem != null && !string.IsNullOrEmpty(this.cboDeptName.Text.Trim()))
            {
                szDeptCode = (this.cboDeptName.SelectedItem as  DeptInfo).DEPT_CODE;
            }
            string szDocTypeIDList = StringHelper.ConventDocTypeIDForQuery(this.txtDocType.Tag as string);
            this.ShowStatusMessage("����ͳ��ʱЧ��¼...");
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select t.dept_stayed,t.doctype_name,t.total,t.timeoutcount,TRUNC((timeoutcount/total),4)*100 as percent from (");
            sb.Append(" select count(*) total,count(case when t.qc_result in (1,3) then 1  else null end ) as timeoutcount,t.dept_stayed,doctype_name from qc_time_record_t t where ");
            sb.AppendFormat("t.{2} >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and t.{2} <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')"
                , dtBeginTime.ToString("yyyy-MM-dd HH:mm:ss")
                , dtEndTime.ToString("yyyy-MM-dd HH:mm:ss")
                , szTimeType);
            if (szDeptCode != string.Empty)
                sb.AppendFormat("and dept_in_charge ='{0}'", szDeptCode);
            if (!string.IsNullOrEmpty(szDocTypeIDList))
            {
                sb.AppendFormat(" and doctype_id in ({0})", szDocTypeIDList);
            }
            sb.Append(" group by dept_stayed,doctype_name order by t.dept_stayed ) t");
            DataSet ds = null;
            short shRet = CommonAccess.Instance.ExecuteQuery(sb.ToString(), out ds);
            if (shRet != EMRDBLib.SystemData.ReturnValue.OK
                || ds == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯʧ��");
                return;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage("δͳ�Ƶ�ʱЧ�����ʱЧ�ʿ����ݲ�����");
                return;
            }
            string pre = string.Empty;
            int rowIndex = 0;
            int ntotal = 0;
            int ntimeoutcount = 0;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string dept_stayed = ds.Tables[0].Rows[i]["dept_stayed"].ToString();
                string doctype_name = ds.Tables[0].Rows[i]["doctype_name"].ToString();
                string total = ds.Tables[0].Rows[i]["total"].ToString();
                string timeoutcount = ds.Tables[0].Rows[i]["timeoutcount"].ToString();
                string percent = ds.Tables[0].Rows[i]["percent"].ToString() + "%";
                if (pre != dept_stayed)
                {
                    ntotal = 0;
                    ntimeoutcount = 0;

                    rowIndex = this.dataGridView1.Rows.Add();
                    pre = dept_stayed;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colDeptName.Index].Value = dept_stayed;
                }
                this.dataGridView1.Columns[doctype_name + "(��)"].Visible = true;
                this.dataGridView1.Columns[doctype_name + "(��ʱ)"].Visible = true;
                this.dataGridView1.Columns[doctype_name + "(��ʱ��)"].Visible = true;


                this.dataGridView1.Columns["��������"].Visible = true;
                this.dataGridView1.Columns["������ʱ����"].Visible = true;
                this.dataGridView1.Columns["������ʱ��"].Visible = true;

                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��)"].Value = total;
                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��ʱ)"].Value = timeoutcount;
                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��ʱ��)"].Value = percent;

                ntotal = ntotal + int.Parse(total);
                ntimeoutcount = ntimeoutcount + int.Parse(timeoutcount);
                this.dataGridView1.Rows[rowIndex].Cells["��������"].Value = ntotal;
                this.dataGridView1.Rows[rowIndex].Cells["������ʱ����"].Value = ntimeoutcount;
                this.dataGridView1.Rows[rowIndex].Cells["������ʱ��"].Value = Math.Round((decimal)ntimeoutcount / ntotal, 2) * 100 + "%";


            }
            this.ShowStatusMessage("ʱЧͳ�����");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void StatTimeCheckByDoctor()
        {
            this.dataGridView1.Rows.Clear();
            DateTime dtBeginTime = DateTime.Parse(this.dtpBeginTime.Value.ToShortDateString());
            DateTime dtEndTime = DateTime.Parse(this.dtpEndTime.Value.AddDays(1).ToShortDateString());
            string szTimeType = string.Empty;
            if (rbtCheckTime.Checked)
                szTimeType = "Check_date";
            else
                szTimeType = "end_date";
            string szDeptCode = string.Empty;
            string szQcResult = string.Empty;
            if (this.cboDeptName.SelectedItem != null && !string.IsNullOrEmpty(this.cboDeptName.Text.Trim()))
            {
                szDeptCode = (this.cboDeptName.SelectedItem as  DeptInfo).DEPT_CODE;
            }
            string szDocTypeIDList = StringHelper.ConventDocTypeIDForQuery(this.txtDocType.Tag as string);
            this.ShowStatusMessage("����ͳ��ʱЧ��¼...");
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StringBuilder sb = new StringBuilder();
            sb.Append("select t.dept_stayed,t.doctype_name,doctor_in_charge,t.total,t.timeoutcount,TRUNC((timeoutcount/total),4)*100 as percent from (");
            sb.Append(" select count(*) total,count(case when t.qc_result in (1,3) then 1  else null end ) as timeoutcount,t.dept_stayed,doctype_name,doctor_in_charge from qc_time_record_t t where ");
            sb.AppendFormat("t.{2} >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and t.{2} <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')"
                , dtBeginTime.ToString("yyyy-MM-dd HH:mm:ss")
                , dtEndTime.ToString("yyyy-MM-dd HH:mm:ss")
                , szTimeType);
            if (szDeptCode != string.Empty)
                sb.AppendFormat("and dept_in_charge ='{0}'", szDeptCode);
            if (!string.IsNullOrEmpty(szDocTypeIDList))
            {
                sb.AppendFormat("and doctype_id in({0})", szDocTypeIDList);
            }
            sb.Append(" and doctor_in_charge is not null ");
            sb.Append(" group by dept_stayed,doctype_name,t.doctor_in_charge order by t.dept_stayed,t.doctor_in_charge ) t");
            DataSet ds = null;
            short shRet =CommonAccess.Instance.ExecuteQuery(sb.ToString(), out ds);
            if (shRet != EMRDBLib.SystemData.ReturnValue.OK
                || ds == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯʧ��");
                return;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage("δͳ�Ƶ�ʱЧ�����ʱЧ�ʿ����ݲ�����");
                return;
            }
            string pre = string.Empty;
            int rowIndex = 0;
            int ntotal = 0;
            int ntimeoutcount = 0;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string dept_stayed = ds.Tables[0].Rows[i]["dept_stayed"].ToString();
                string doctype_name = ds.Tables[0].Rows[i]["doctype_name"].ToString();
                string doctor_in_charge = ds.Tables[0].Rows[i]["doctor_in_charge"].ToString();
                string total = ds.Tables[0].Rows[i]["total"].ToString();
                string timeoutcount = ds.Tables[0].Rows[i]["timeoutcount"].ToString();
                string percent = ds.Tables[0].Rows[i]["percent"].ToString() + "%";
                if (pre != (dept_stayed + doctor_in_charge))
                {
                    ntotal = 0;
                    ntimeoutcount = 0;

                    rowIndex = this.dataGridView1.Rows.Add();
                    pre = dept_stayed + doctor_in_charge;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colDeptName.Index].Value = dept_stayed;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colDoctorInCharge.Index].Value = doctor_in_charge;
                }
                this.dataGridView1.Columns[doctype_name + "(��)"].Visible = true;
                this.dataGridView1.Columns[doctype_name + "(��ʱ)"].Visible = true;
                this.dataGridView1.Columns[doctype_name + "(��ʱ��)"].Visible = true;
                this.dataGridView1.Columns["��������"].Visible = true;
                this.dataGridView1.Columns["������ʱ����"].Visible = true;
                this.dataGridView1.Columns["������ʱ��"].Visible = true;

                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��)"].Value = total;
                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��ʱ)"].Value = timeoutcount;
                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��ʱ��)"].Value = percent;
                ntotal = ntotal + int.Parse(total);
                ntimeoutcount = ntimeoutcount + int.Parse(timeoutcount);
                this.dataGridView1.Rows[rowIndex].Cells["��������"].Value = ntotal;
                this.dataGridView1.Rows[rowIndex].Cells["������ʱ����"].Value = ntimeoutcount;
                this.dataGridView1.Rows[rowIndex].Cells["������ʱ��"].Value = Math.Round((decimal)ntimeoutcount / ntotal, 2) * 100 + "%";

            }
            this.ShowStatusMessage("ʱЧͳ�����");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        private Hashtable htJiZhen = null;
        //������������
        public bool Jizhen(string szPatientID, string szVisitID)
        {
            if (htJiZhen == null)
            {
                htJiZhen = new Hashtable();
            }
            if (htJiZhen.ContainsKey(string.Format("{0}_{1}", szPatientID, szVisitID)))
            {
                bool result = bool.Parse(htJiZhen[string.Format("{0}_{1}", szPatientID, szVisitID)].ToString());
                return result;
            }
            //��ѯ���ߵ���Ժʱ��
            //��ѯ���߱�����Ժ���������ʱ��
            //�ж�����ʱ���Ƿ�����Ժʱ��Ķ�ʮ��Сʱ����
            EMRDBLib.PatVisitInfo patVisitLog = null;
            short shRet = EMRDBLib.DbAccess.PatVisitAccess.Instance.GetPatVisitInfo(szPatientID, szVisitID, ref patVisitLog);
            if (shRet != EMRDBLib.SystemData.ReturnValue.OK)
                return false;
            DateTime dtOperationDate = EMRDBLib.DbAccess.OperationMasterAccess.Instance.GetOperationTime(szPatientID, szVisitID);
            if (dtOperationDate == DateTime.Parse("1900-01-01"))
            {
                htJiZhen.Add(string.Format("{0}_{1}", szPatientID, szVisitID), false);
                return false;
            }
            else
            {
                if (patVisitLog.VISIT_TIME.AddHours(24) > dtOperationDate)
                {
                    htJiZhen.Add(string.Format("{0}_{1}", szPatientID, szVisitID), true);
                    return true;
                }
            }
            return false;
        }


        private void StatTimeCheckByPatient()
        {
            this.dataGridView1.Rows.Clear();
            if (htJiZhen == null)
                htJiZhen = new Hashtable();
            htJiZhen.Clear();

            DateTime dtBeginTime = DateTime.Parse(this.dtpBeginTime.Value.ToShortDateString());
            DateTime dtEndTime = DateTime.Parse(this.dtpEndTime.Value.AddDays(1).ToShortDateString());
            string szDeptCode = string.Empty;
            string szQcResult = string.Empty;
            string szTimeType = string.Empty;
            if (rbtCheckTime.Checked)
                szTimeType = "Check_date";
            else
                szTimeType = "end_date";
            if (this.cboDeptName.SelectedItem != null && !string.IsNullOrEmpty(this.cboDeptName.Text.Trim()))
            {
                szDeptCode = (this.cboDeptName.SelectedItem as  DeptInfo).DEPT_CODE;
            }
            string szDocTypeIDList = StringHelper.ConventDocTypeIDForQuery(this.txtDocType.Tag as string);
            this.ShowStatusMessage("����ͳ��ʱЧ��¼...");
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            StringBuilder sb = new StringBuilder();
            sb.Append("select t.dept_stayed,t.doctype_name,doctor_in_charge,patient_name,patient_id,visit_id,t.total,t.timeoutcount,TRUNC((timeoutcount/total),4)*100 as percent from (");
            sb.Append(" select count(*) total,count(case when t.qc_result in (1,3) then 1  else null end ) as timeoutcount,t.dept_stayed,doctype_name,doctor_in_charge,patient_name,patient_id,visit_id from qc_time_record_t t where ");
            sb.AppendFormat("t.{2} >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and t.{2} <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')"
                , dtBeginTime.ToString("yyyy-MM-dd HH:mm:ss")
                , dtEndTime.ToString("yyyy-MM-dd HH:mm:ss")
                , szTimeType);
            if (szDeptCode != string.Empty)
                sb.AppendFormat("and dept_in_charge ='{0}'", szDeptCode);
            if (!string.IsNullOrEmpty(szDocTypeIDList))
            {
                sb.AppendFormat("and doctype_id in({0})", szDocTypeIDList);
            }
            sb.Append(" and doctor_in_charge is not null ");
            sb.Append(" group by dept_stayed,doctype_name,t.doctor_in_charge,patient_name,patient_id,visit_id order by t.dept_stayed,t.doctor_in_charge,patient_name,patient_id ) t");
            DataSet ds = null;
            short shRet =CommonAccess.Instance.ExecuteQuery(sb.ToString(), out ds);
            if (shRet != EMRDBLib.SystemData.ReturnValue.OK
                || ds == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("��ѯʧ��");
                return;
            }
            if (ds.Tables[0].Rows.Count <= 0)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage("δͳ�Ƶ�ʱЧ�����ʱЧ�ʿ����ݲ�����");
                return;
            }
            string pre = string.Empty;
            int rowIndex = 0;
            int ntotal = 0;
            int ntimeoutcount = 0;
            WorkProcess.Instance.Initialize(this, ds.Tables[0].Rows.Count
             , string.Format("���ڼ���ʱЧͳ�ƣ����Ժ�..."));

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                WorkProcess.Instance.Show(string.Format("���ص�{0}�����Ժ�...", i.ToString()), i);
                if (WorkProcess.Instance.Canceled)
                    break;

                string dept_stayed = ds.Tables[0].Rows[i]["dept_stayed"].ToString();
                string doctype_name = ds.Tables[0].Rows[i]["doctype_name"].ToString();
                string doctor_in_charge = ds.Tables[0].Rows[i]["doctor_in_charge"].ToString();
                string patient_name = ds.Tables[0].Rows[i]["patient_name"].ToString();
                string patient_id = ds.Tables[0].Rows[i]["patient_id"].ToString();
                string visit_id = ds.Tables[0].Rows[i]["visit_id"].ToString();
               
                string total = ds.Tables[0].Rows[i]["total"].ToString();
                string timeoutcount = ds.Tables[0].Rows[i]["timeoutcount"].ToString();
                string percent = ds.Tables[0].Rows[i]["percent"].ToString() + "%";
                if (pre != (dept_stayed + doctor_in_charge + patient_name + patient_id + visit_id))
                {
                    ntotal = 0;
                    ntimeoutcount = 0;

                    rowIndex = this.dataGridView1.Rows.Add();
                    pre = dept_stayed + doctor_in_charge + patient_name + patient_id + visit_id;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colDeptName.Index].Value = dept_stayed;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colDoctorInCharge.Index].Value = doctor_in_charge;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colPatientName.Index].Value = patient_name;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colPatientID.Index].Value = patient_id;
                    this.dataGridView1.Rows[rowIndex].Cells[this.colVisitID.Index].Value = visit_id;
                 
                }
                this.dataGridView1.Columns[doctype_name + "(��)"].Visible = true;
                this.dataGridView1.Columns[doctype_name + "(��ʱ)"].Visible = true;
                this.dataGridView1.Columns[doctype_name + "(��ʱ��)"].Visible = true;

                this.dataGridView1.Columns["��������"].Visible = true;
                this.dataGridView1.Columns["������ʱ����"].Visible = true;
                this.dataGridView1.Columns["������ʱ��"].Visible = true;


                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��)"].Value = total;
                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��ʱ)"].Value = timeoutcount;
                this.dataGridView1.Rows[rowIndex].Cells[doctype_name + "(��ʱ��)"].Value = percent;

                ntotal = ntotal + int.Parse(total);
                ntimeoutcount = ntimeoutcount + int.Parse(timeoutcount);
                this.dataGridView1.Rows[rowIndex].Cells["��������"].Value = ntotal;
                this.dataGridView1.Rows[rowIndex].Cells["������ʱ����"].Value = ntimeoutcount;
                this.dataGridView1.Rows[rowIndex].Cells["������ʱ��"].Value = Math.Round((decimal)ntimeoutcount / ntotal * 100, 2) + "%";

            }
            WorkProcess.Instance.Close();

            this.ShowStatusMessage("ʱЧͳ�����");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        //����ϼ�
        public void CalTotal()
        {
            int rowIndex = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Yellow;
            this.dataGridView1.Rows[rowIndex].Cells[0].Value = "�ϼƣ�";
            if (this.cboType.Text == "����")
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
                {
                    for (int j = 1; j < this.dataGridView1.Columns.Count; j++)
                    {
                        if (this.dataGridView1.Rows[rowIndex].Cells[j].Value == null)
                            this.dataGridView1.Rows[rowIndex].Cells[j].Value = "0";
                        if (this.dataGridView1.Rows[i].Cells[j].Value == null)
                            continue;
                        if (this.dataGridView1.Columns[j].Name.Contains("��ʱ��"))
                        {
                            int ntotal = int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j - 2].Value.ToString());
                            int ntimeoutcount = int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j - 1].Value.ToString());
                            if (ntotal == 0)
                                continue;
                            this.dataGridView1.Rows[rowIndex].Cells[j].Value = Math.Round((decimal)ntimeoutcount / ntotal * 100, 2) + "%";
                            continue;
                        }
                        this.dataGridView1.Rows[rowIndex].Cells[j].Value =
                            int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j].Value.ToString())
                            + int.Parse(this.dataGridView1.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }
            else if (this.cboType.Text == "ҽ��")
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
                {
                    for (int j = 2; j < this.dataGridView1.Columns.Count; j++)
                    {
                        if (this.dataGridView1.Rows[rowIndex].Cells[j].Value == null)
                            this.dataGridView1.Rows[rowIndex].Cells[j].Value = "0";
                        if (this.dataGridView1.Rows[i].Cells[j].Value == null)
                            continue;
                        if (this.dataGridView1.Columns[j].Name.Contains("��ʱ��"))
                        {
                            int ntotal = int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j - 2].Value.ToString());
                            int ntimeoutcount = int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j - 1].Value.ToString());
                            if (ntotal == 0)
                                continue;
                            this.dataGridView1.Rows[rowIndex].Cells[j].Value = Math.Round((decimal)ntimeoutcount / ntotal * 100, 2) + "%";
                            continue;
                        }
                        this.dataGridView1.Rows[rowIndex].Cells[j].Value =
                            int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j].Value.ToString())
                            + int.Parse(this.dataGridView1.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }
            else if (this.cboType.Text == "����")
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
                {
                    for (int j = 6; j < this.dataGridView1.Columns.Count; j++)
                    {
                        if (this.dataGridView1.Rows[rowIndex].Cells[j].Value == null)
                            this.dataGridView1.Rows[rowIndex].Cells[j].Value = "0";
                        if (this.dataGridView1.Rows[i].Cells[j].Value == null)
                            continue;
                        if (this.dataGridView1.Columns[j].Name.Contains("��ʱ��"))
                        {
                            int ntotal = int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j - 2].Value.ToString());
                            int ntimeoutcount = int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j - 1].Value.ToString());
                            if (ntotal == 0)
                                continue;
                            this.dataGridView1.Rows[rowIndex].Cells[j].Value = Math.Round((decimal)ntimeoutcount / ntotal * 100, 2) + "%";
                            continue;
                        }
                        this.dataGridView1.Rows[rowIndex].Cells[j].Value =
                            int.Parse(this.dataGridView1.Rows[rowIndex].Cells[j].Value.ToString())
                            + int.Parse(this.dataGridView1.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }
        }

        private void SetColumnVisable()
        {
            foreach (DataGridViewColumn item in this.dataGridView1.Columns)
            {
                item.Visible = false;
            }
            if (this.cboType.Text=="����")
            {
                this.colDeptName.Visible = true;
                this.colDoctorInCharge.Visible = false;
                this.colPatientID.Visible = false;
                this.colPatientName.Visible = false;
                this.colVisitID.Visible = false;
            }
            else if (this.cboType.Text=="ҽ��")
            {
                this.colDeptName.Visible = true;
                this.colDoctorInCharge.Visible = true;
                this.colPatientID.Visible = false;
                this.colPatientName.Visible = false;
                this.colVisitID.Visible = false;
            }
            else if (this.cboType.Text=="����")
            {
                this.colDeptName.Visible = true;
                this.colDoctorInCharge.Visible = true;
                this.colPatientID.Visible = true;
                this.colPatientName.Visible = true;
                this.colVisitID.Visible = true;
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.Show("��ǰû�пɵ��������ݣ�", MessageBoxIcon.Information);
                return;
            }
            System.Collections.Hashtable htNoExportColunms = new System.Collections.Hashtable();
            StatExpExcelHelper.Instance.HtNoExportColIndex = htNoExportColunms;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.dataGridView1.Update();
            StatExpExcelHelper.Instance.ExportTo(this.dataGridView1, "����ʱЧͳ���嵥");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        
        private void ReportExplorerForm_QueryContext(object sender, Heren.Common.Report.QueryContextEventArgs e)
        {
            object value = e.Value;
            e.Success = this.GetGlobalDataHandler(e.Name, ref value);
            if (e.Success) e.Value = value;
        }
        
        private bool GetGlobalDataHandler(string name, ref object value)
        {
            return false;
        }

        private void txtDocType_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowDocTypeSelectForm();
        }

        /// <summary>
        /// ��ʾ�ĵ��������öԻ���
        /// </summary>
        /// <param name="row">ָ����</param>
        private void ShowDocTypeSelectForm()
        {
            TempletSelectForm templetSelectForm = new TempletSelectForm();
            templetSelectForm.DefaultDocTypeID = txtDocType.Tag as string;
            templetSelectForm.MultiSelect = true;
            templetSelectForm.Text = "ѡ��������";
            templetSelectForm.Description = "��ѡ��Ӧ��д�Ĳ������ͣ�";
            if (templetSelectForm.ShowDialog() != DialogResult.OK)
                return;
            List<DocTypeInfo> lstDocTypeInfos = templetSelectForm.SelectedDocTypes;
            if (lstDocTypeInfos == null || lstDocTypeInfos.Count <= 0)
            {
                return;
            }

            StringBuilder sbDocTypeIDList = new StringBuilder();
            StringBuilder sbDocTypeNameList = new StringBuilder();
            for (int index = 0; index < lstDocTypeInfos.Count; index++)
            {
                DocTypeInfo docTypeInfo = lstDocTypeInfos[index];
                if (docTypeInfo == null)
                    continue;
                sbDocTypeIDList.Append(docTypeInfo.DocTypeID);
                if (index < lstDocTypeInfos.Count - 1)
                    sbDocTypeIDList.Append(";");
                sbDocTypeNameList.Append(docTypeInfo.DocTypeName);
                if (index < lstDocTypeInfos.Count - 1)
                    sbDocTypeNameList.Append(";");
            }
            txtDocType.Text = sbDocTypeNameList.ToString();
            txtDocType.Tag = sbDocTypeIDList.ToString();
        }
    }
}