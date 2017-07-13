// ***********************************************************
// �����ʿ�ϵͳ�����ٻع�����.
// Creator:yehui  Date:2014-09-28
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MedQCSys.DockForms;
using Heren.Common.Libraries;
using System.Collections;
using Heren.Common.Controls.TableView;
using EMRDBLib;
using EMRDBLib.DbAccess;
using Heren.MedQC.Utilities;

namespace Heren.MedQC.Systems
{
    public partial class MedCallBackForm : Form
    {
        public MedCallBackForm()
        {
            InitializeComponent();
            this.btnQueryByDept.Image = Properties.Resources.Query;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.LoadDeptList();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        public MedCallBackForm(string patientID) : this()
        {
            this.txtUserID.Text = patientID;

             this.LoadPatientListGridData();
            if (this.dgvPatients.Rows.Count > 0)
            {
                //Ĭ��ѡ�����һ�ξ���
                this.dgvPatients.Rows[this.dgvPatients.Rows.Count - 1].Selected = true;

                PatVisitInfo patVisitLog = this.dgvPatients.Rows[this.dgvPatients.Rows.Count - 1].Tag as PatVisitInfo;
                //���ػ������ڿ���ҽ���б�
                this.QueryByDept(patVisitLog);
                this.dgvDoctors.ClearSelection();
            }
        }

        private Dictionary<string, string> DicDeptInfos;
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }
        /// <summary>
        /// �����ٴ������б�
        /// </summary>
        private void LoadDeptList()
        {
            
            if (!InitControlData.InitCboDeptName(ref this.cboDeptList))
            {
                MessageBoxEx.Show("���ؿ����б�ʧ��");
            }
        }

        private void cboDeptList_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Enter)
            //    this.QueryByDept();
        }

        private void QueryByDept(PatVisitInfo patVisitLog)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            
            string szSql = string.Format("select d.creator_id,d.creator_Name  from emr_doc_t d where d.patient_id='{0}' and d.visit_id={1} group by d.creator_id,d.creator_name",patVisitLog.PATIENT_ID,patVisitLog.VISIT_ID);
            DataSet dataSet = new DataSet();
            short shRet = CommonAccess.Instance.ExecuteQuery(szSql, out dataSet);

            if (shRet != SystemData.ReturnValue.OK)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("�û��б��ѯ����ʧ��!");
                return;
            }
            List<UserInfo> lstUserInfos = new List<UserInfo>() ;
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int index = 0; index < dataSet.Tables[0].Rows.Count; index++)
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.Name = dataSet.Tables[0].Rows[index]["creator_id"].ToString();
                    userInfo.ID = dataSet.Tables[0].Rows[index]["creator_Name"].ToString();
                    lstUserInfos.Add(userInfo);
                }
            }
            else
            {
                szSql = string.Format("SELECT b.name,b.user_name FROM staff_vs_group@link_emr a,staff_dict@link_emr b,dept_dict@link_emr c WHERE b.emp_no=a.emp_no AND a.group_code='{0}' AND a.group_class='����ҽ��' and b.dept_code=c.DEPT_CODE and c.clinic_attr = '0' and c.OUTP_OR_INP = '1'and c.internal_or_sergery = '0' order by b.name", patVisitLog.DEPT_CODE);
                shRet = CommonAccess.Instance.ExecuteQuery(szSql, out dataSet);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    MessageBoxEx.Show("�û��б��ѯ����ʧ��!");
                    return;
                }
                else
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int index = 0; index < dataSet.Tables[0].Rows.Count; index++)
                        {
                            UserInfo userInfo = new UserInfo();
                            userInfo.Name = dataSet.Tables[0].Rows[index]["name"].ToString();
                            userInfo.ID = dataSet.Tables[0].Rows[index]["user_name"].ToString();
                            lstUserInfos.Add(userInfo);
                        }
                    }
                }
            }

            this.LoadDoctors(lstUserInfos);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void LoadDoctors(List<UserInfo> lstUserInfos)
        {

            this.dgvDoctors.Rows.Clear();
            this.Update();
            if (lstUserInfos == null || lstUserInfos.Count <= 0)
            {
                MessageBoxEx.Show("û�в�ѯ�����������ļ�¼��"
                    , MessageBoxIcon.Information);
                return;
            }
            Hashtable htUserRow = new Hashtable();
            for (int index = 0; index < lstUserInfos.Count; index++)
            {
                UserInfo userInfo = lstUserInfos[index];
                if (userInfo == null)
                    continue;
                int nRowIndex = this.dgvDoctors.Rows.Add();
                DataTableViewRow row = this.dgvDoctors.Rows[nRowIndex];
                if (htUserRow.Contains(userInfo.ID))
                    continue;
                htUserRow.Add(userInfo.ID, row);

                if (userInfo.ID != null)
                    row.Cells[this.colUserID.Index].Value = userInfo.ID.Trim();
                if (userInfo.Name != null)
                    row.Cells[this.colUserName.Index].Value = userInfo.Name.Trim();

                if (index % 2 == 0)
                    row.DefaultCellStyle.BackColor = Color.White;
                else
                    row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                row.Tag = userInfo;
                this.dgvDoctors.SetRowState(row, RowState.Normal);

            }

        }

        private void cboDeptList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.QueryByDept();
        }

        private void txtUserID_ButtonClick(object sender, EventArgs e)
        {
            this.LoadPatientListGridData();
            if (this.dgvPatients.Rows.Count > 0)
            {
                //Ĭ��ѡ�����һ�ξ���
                this.dgvPatients.Rows[this.dgvPatients.Rows.Count - 1].Selected = true;

                PatVisitInfo patVisitLog = this.dgvPatients.Rows[this.dgvPatients.Rows.Count - 1].Tag as PatVisitInfo;
                //���ػ������ڿ���ҽ���б�
                this.QueryByDept(patVisitLog);
                this.dgvDoctors.ClearSelection();
            }
        }

        private void LoadPatientListGridData()
        {
            if (this.txtUserID.Text == string.Empty)
            {
                MessageBoxEx.Show("�����뻼��ID��");
                return;
            }
            string szPatientID = this.txtUserID.Text.Trim();
            List<PatVisitInfo> lstPatVisitLog = null;
            short shRet = PatVisitAccess.Instance.GetPatVisitInfos(szPatientID, ref lstPatVisitLog);
            if (shRet != SystemData.ReturnValue.OK)
                return;
            this.dgvPatients.Rows.Clear();
            if (lstPatVisitLog == null || lstPatVisitLog.Count <= 0)
            {
                MessageBoxEx.ShowMessage("δ�ҵ�����");
                return;
            }
            for (int index = 0; index < lstPatVisitLog.Count; index++)
            {
                PatVisitInfo patVisitLog = lstPatVisitLog[index];
                int nRowIndex = this.dgvPatients.Rows.Add();
                DataGridViewRow row = this.dgvPatients.Rows[nRowIndex];
                row.Cells[this.colPatName.Index].Value = patVisitLog.PATIENT_NAME;
                row.Cells[this.colChargeType.Index].Value = patVisitLog.CHARGE_TYPE;
                row.Cells[this.colAdmissionDate.Index].Value = patVisitLog.VISIT_TIME;

                row.Cells[this.colDeptName.Index].Value = patVisitLog.DEPT_NAME;
                row.Cells[this.colDischargeDate.Index].Value = patVisitLog.DISCHARGE_TIME;
                if (this.DicDeptInfos.ContainsKey(patVisitLog.DischargeDeptCode))
                    row.Cells[this.colDeptDischargeFrom.Index].Value = this.DicDeptInfos[patVisitLog.DischargeDeptCode];
                row.Tag = patVisitLog;
                row.Selected = true;
            }

        }

        private void btnCallBack_Click(object sender, EventArgs e)
        {
            if (this.dgvPatients.SelectedRows.Count == 0)
            {
                MessageBoxEx.Show("���ѯ������");
                return;
            }
            if (this.dgvDoctors.SelectedRows.Count == 0)
            {
                MessageBoxEx.Show("δѡ��ҽ��");
                return;
            }
            DataGridViewRow row = this.dgvPatients.SelectedRows[0];
            PatVisitInfo patVisitLog = row.Tag as PatVisitInfo;
            DataGridViewRow row2 = this.dgvDoctors.SelectedRows[0];
            UserInfo userInfo = row2.Tag as UserInfo;

            string szSql = string.Format("Select * from mr_on_line@link_emr where patient_id='{0}' and visit_id='{1}' and status='0'"
                , patVisitLog.PATIENT_ID
                , patVisitLog.VISIT_ID);
            DataSet dataSet = new DataSet();
            if (CommonAccess.Instance.ExecuteQuery(szSql, out dataSet) != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�ٻ�ʧ��");
                return;
            }
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                string szRequestDoctorID = dataSet.Tables[0].Rows[0]["request_doctor_id"].ToString();
                MessageBoxEx.Show(string.Format("�û�����{0}����δ��Ժ�������ٻ�", szRequestDoctorID));
                return;
            }
            short shRet = EmrDocAccess.Instance.MedCallBack(patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID, userInfo.ID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("�ٻ�ʧ��");
                return;
            }
            MessageBoxEx.ShowMessage("�ٻسɹ�");

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

    }
}