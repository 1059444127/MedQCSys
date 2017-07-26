// ***********************************************************
// ���������ù���ϵͳ,�û�Ȩ�����ù�����.
// Author : YangMingkun, Date : 2012-6-7
// Copyright : Heren Health Services Co.,Ltd.
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
using Heren.Common.Controls.TableView;
using Heren.Common.DockSuite;
using System.Collections;
using MedQCSys.DockForms;
using EMRDBLib;
using EMRDBLib.DbAccess;
using MedQCSys;
using System.Linq;
namespace Heren.MedQC.Hdp
{
    internal partial class UserManageForm : DockContentBase
    {
        /// <summary>
        /// ��ɫѡ�񴰿�
        /// </summary>
        private UserGrantForm m_UserGrantForm = null;

        public UserManageForm(MainForm form) : base(form)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
            // this.dataGridView1.AutoReadonly = true;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.OnRefreshView();
            this.btnQueryByDept.Image = Properties.Resources.Query;

        }

        public override void OnRefreshView()
        {
            base.OnRefreshView();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.LoadDeptList();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// �����ٴ������б�
        /// </summary>
        private void LoadDeptList()
        {
            this.cboDeptList.Items.Clear();

            List<DeptInfo> lstDeptInfos = null;
            short shResult = EMRDBAccess.Instance.GetAllDeptInfos(ref lstDeptInfos);
            if (shResult != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��ȡ������Ϣ�б�ʱ��������");
                return;
            }
            if (lstDeptInfos == null)
                return;
            for (int index = 0; index < lstDeptInfos.Count; index++)
            {
                DeptInfo deptInfo = lstDeptInfos[index];
                if (deptInfo != null)
                {
                    this.cboDeptList.Items.Add(deptInfo.InputCode.ToLower(), deptInfo);
                }
            }
        }

        /// <summary>
        /// ��ָ�����û��б���ص�DataGridView����
        /// </summary>
        /// <param name="lstUserInfos">�û���Ϣ�б�</param>
        private void LoadGridViewData(List<UserInfo> lstUserInfos)
        {
            this.dataGridView1.Rows.Clear();
            this.Update();
            if (lstUserInfos == null || lstUserInfos.Count <= 0)
            {
                MessageBoxEx.Show("û�в�ѯ�����������ļ�¼��"
                    , MessageBoxIcon.Information);
                return;
            }

            Hashtable htUserRow = new Hashtable();
            short shRet = SystemData.ReturnValue.OK;
            List<QcAdminDepts> lstQcAdminDepts = null;
            shRet = QcAdminDeptsAccess.Instance.GetQcAdminDeptsList(ref lstQcAdminDepts);
            for (int index = 0; index < lstUserInfos.Count; index++)
            {
                UserInfo userInfo = lstUserInfos[index];
                if (userInfo == null)
                    continue;
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                if (htUserRow.Contains(userInfo.USER_ID))
                    continue;
                htUserRow.Add(userInfo.USER_ID, row);
                row.Tag = userInfo;
                if (userInfo.USER_ID != null)
                    row.Cells[this.colUserID.Index].Value = userInfo.USER_ID.Trim();
                if (userInfo.USER_NAME != null)
                    row.Cells[this.colUserName.Index].Value = userInfo.USER_NAME.Trim();
                if (userInfo.DEPT_NAME != null)
                    row.Cells[this.colDeptName.Index].Value = userInfo.DEPT_NAME.Trim();

                if (index % 2 == 0)
                    row.DefaultCellStyle.BackColor = Color.White;
                else
                    row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                List<HdpRoleUser> lstHdpRoleUser = null;
                shRet = HdpRoleUserAccess.Instance.GetHdpRoleUserList(userInfo.USER_ID, ref lstHdpRoleUser);
                if (shRet == SystemData.ReturnValue.OK)
                {
                    string szRoleName = null;
                    foreach (HdpRoleUser item in lstHdpRoleUser)
                    {
                        if (szRoleName == null)
                            szRoleName = item.RoleName;
                        else
                            szRoleName += "," + item.RoleName;
                    }
                    row.Cells[this.colRoleName.Index].Value = szRoleName;
                }
                var result = lstQcAdminDepts.Where(m => m.USER_ID == userInfo.USER_ID).ToList();
                if (result.Count > 0)
                {
                    row.Cells[this.colAdminDepts.Index].Value = string.Join(",", result.Select(m => m.DEPT_NAME).ToArray());
                }
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }

        }

        /// <summary>
        /// ����ָ���û���Ȩ����Ϣ����ǰDataGridView�ؼ�ĩβ
        /// </summary>
        /// <param name="row">Ȩ�޶������Լ���(�ɿ�)</param>
        /// <param name="userRight">�û���Ȩ����Ϣ</param>
        private void SetRowData(DataTableViewRow row, UserRightBase userRight)
        {

        }

        /// <summary>
        /// ��ȡָ���е�ǰ��Ȩ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="userRight">�û���Ȩ����Ϣ</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref UserRightBase userRight)
        {
            if (row == null || row.Index < 0)
                return false;

            object cellValue = row.Cells[this.colUserID.Index].Value;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
                return false;


            return true;
        }

        /// <summary>
        /// ����ָ���е����ݵ�Զ�����ݱ�,��Ҫע����ǣ��е�ɾ��״̬��������״̬����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <returns>SystemData.ReturnValue</returns>
        private short SaveRowData(DataTableViewRow row)
        {
            if (row == null || row.Index < 0)
                return SystemData.ReturnValue.FAILED;

            if (this.dataGridView1.IsNormalRow(row) || this.dataGridView1.IsUnknownRow(row))
            {
                if (!this.dataGridView1.IsDeletedRow(row))
                    return SystemData.ReturnValue.CANCEL;
            }

            UserRightBase userRight = null;
            if (!this.MakeRowData(row, ref userRight))
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;

            return SystemData.ReturnValue.OK;
        }


        private void QueryByDept()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            string szDeptName = this.cboDeptList.Text;
            DeptInfo deptInfo = this.cboDeptList.SelectedItem as DeptInfo;
            if (deptInfo == null || deptInfo.DEPT_NAME != szDeptName)
            {
                object objItem = null;
                if (szDeptName != string.Empty)
                    objItem = this.cboDeptList.GetItem(szDeptName, false);
                deptInfo = objItem as DeptInfo;
            }
            if (deptInfo == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("������Ŀ��Ҳ�����!"
                    , MessageBoxIcon.Information);
                return;
            }
            List<UserInfo> lstUserInfos = null;
            short shRet = EMRDBAccess.Instance.GetDeptUserList(deptInfo.DEPT_CODE, ref lstUserInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("�û��б��ѯ����ʧ��!");
                return;
            }
            this.LoadGridViewData(lstUserInfos);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void QueryByUserID()
        {
            string szUserID = this.txtUserID.Text.Trim();
            if (GlobalMethods.Misc.IsEmptyString(szUserID))
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            UserInfo userInfo = null;
            short shRet = UserAccess.Instance.GetUserInfo(szUserID, ref userInfo);
            if (userInfo == null)
            {
                userInfo = new UserInfo();
                userInfo.USER_ID = szUserID;
                userInfo.USER_NAME = "�ⲿ��Ա";
            }

            List<UserInfo> lstUserInfos = new List<UserInfo>();
            if (userInfo != null) lstUserInfos.Add(userInfo);
            this.LoadGridViewData(lstUserInfos);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void QueryByUserName()
        {
            string szUserName = this.txtUserName.Text.Trim();
            if (GlobalMethods.Misc.IsEmptyString(szUserName))
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            List<UserInfo> lstUserInfos = null;
            short shRet = UserAccess.Instance.GetUserInfo(szUserName, ref lstUserInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("�û��б��ѯ����ʧ��!");
                return;
            }
            this.LoadGridViewData(lstUserInfos);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void QueryAllUsers()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            List<UserInfo> lstUserInfos = null;
            short shRet = UserAccess.Instance.GetAllUserInfos(ref lstUserInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("�û��б��ѯ����ʧ��!");
                return;
            }
            this.LoadGridViewData(lstUserInfos);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void QueryByDeptType(bool bIsWard, bool bIsOutp, bool bIsNurse)
        {
            int nDeptCount = this.cboDeptList.Items.Count;
            if (nDeptCount <= 0)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);

            short shRet = SystemData.ReturnValue.OK;
            List<UserInfo> lstTotalUserInfos = null;
            for (int index = 0; index < nDeptCount; index++)
            {
                object objItem = this.cboDeptList.Items[index];
                DeptInfo deptInfo = objItem as DeptInfo;
                if (deptInfo == null)
                    continue;
                if (bIsWard && !deptInfo.IsWardDept)
                    continue;
                if (bIsOutp && !deptInfo.IsOutpDept)
                    continue;
                if (bIsNurse && !deptInfo.IsNurseDept)
                    continue;
                if (!bIsWard && !bIsOutp && !bIsNurse
                    && (deptInfo.IsWardDept
                    || deptInfo.IsOutpDept
                    || deptInfo.IsNurseDept))
                    continue;

                string szDeptCode = deptInfo.DEPT_CODE;
                List<UserInfo> lstUserInfos = null;
                shRet = EMRDBAccess.Instance.GetDeptUserList(szDeptCode, ref lstUserInfos);
                if (shRet != SystemData.ReturnValue.OK && shRet != SystemData.ReturnValue.RES_NO_FOUND)
                {
                    MessageBoxEx.Show("�û��б��ѯ����ʧ��!");
                    break;
                }
                if (lstUserInfos == null || lstUserInfos.Count <= 0)
                    continue;
                if (lstTotalUserInfos == null)
                    lstTotalUserInfos = new List<UserInfo>();
                lstTotalUserInfos.AddRange(lstUserInfos);
            }
            this.LoadGridViewData(lstTotalUserInfos);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void cboDeptList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                this.QueryByDept();
        }

        private void cboDeptList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.QueryByDept();
        }

        private void txtUserID_ButtonClick(object sender, EventArgs e)
        {
            this.QueryByUserID();
        }

        private void txtUserName_ButtonClick(object sender, EventArgs e)
        {
            this.QueryByUserName();
        }

        private void btnQueryByDept_Click(object sender, EventArgs e)
        {
            this.QueryByDept();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            this.cmenuSelectUser.Show(this.btnSelectAll, 0, this.btnSelectAll.Height);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void mnuDeptAll_Click(object sender, EventArgs e)
        {
            this.QueryAllUsers();
        }

        private void mnuDeptWard_Click(object sender, EventArgs e)
        {
            this.QueryByDeptType(true, false, false);
        }

        private void mnuDeptOutp_Click(object sender, EventArgs e)
        {
            this.QueryByDeptType(false, true, false);
        }

        private void mnuDeptNurse_Click(object sender, EventArgs e)
        {
            this.QueryByDeptType(false, false, true);
        }

        private void mnuDeptOther_Click(object sender, EventArgs e)
        {
            this.QueryByDeptType(false, false, false);
        }



        private void mnuCancelModify_Click(object sender, EventArgs e)
        {
            int nCount = this.dataGridView1.SelectedCells.Count;
            if (nCount <= 0)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            for (int index = 0; index < nCount; index++)
            {
                DataGridViewCell cell = this.dataGridView1.SelectedCells[index];
                DataTableViewRow row = this.dataGridView1.Rows[cell.RowIndex];
                if (this.dataGridView1.IsNormalRow(row))
                    continue;
                if (this.dataGridView1.IsNewRow(row))
                    continue;
                this.SetRowData(row, row.Tag as UserRightBase);
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }



        private void btnUserRightSelect_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_UserGrantForm == null || this.m_UserGrantForm.IsDisposed)
            {
                List<RightInfo> lstDisplayRights = new List<RightInfo>();
                List<RightInfo> lstCheckedRights = new List<RightInfo>();
                for (int index = 0; index < this.dataGridView1.Columns.Count; index++)
                {
                    DataGridViewColumn column = this.dataGridView1.Columns[index];
                    RightInfo rightInfo = column.Tag as RightInfo;
                    lstDisplayRights.Add(rightInfo);
                    if (rightInfo != null && column.Visible)
                        lstCheckedRights.Add(rightInfo);
                }

            }
            if (this.m_UserGrantForm.Visible)
            {
                this.m_UserGrantForm.Visible = false;
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }

            this.m_UserGrantForm.Show();
            this.m_UserGrantForm.BringToFront();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnAuthority_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBoxEx.ShowError("δѡ����");
                return;
            }
            UserInfo userInfo = this.dataGridView1.SelectedRows[0].Tag as UserInfo;
            Authority(userInfo);
        }

        private void Authority(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                MessageBoxEx.ShowError("�û���Ϣ������");
                return;
            }
            UserGrantForm form = new UserGrantForm();

            form.UserInfo = userInfo;
            form.ShowDialog();
            if (form.CheckHdpRoleUserList != null && form.CheckHdpRoleUserList.Count > 0)
            {
                string szRoleName = string.Empty;
                foreach (HdpRoleUser item in form.CheckHdpRoleUserList)
                {
                    if (string.IsNullOrEmpty(szRoleName))
                    {
                        szRoleName = item.RoleName;
                    }
                    else
                    {
                        szRoleName += "," + item.RoleName;
                    }

                }
                this.dataGridView1.SelectedRows[0].Cells[this.colRoleName.Index].Value = szRoleName;
            }
            if (form.CheckHdpRoleUserList.Count == 0)
            {
                this.dataGridView1.SelectedRows[0].Cells[this.colRoleName.Index].Value = string.Empty;
            }
        }


        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            UserInfo userInfo = this.dataGridView1.Rows[e.RowIndex].Tag as UserInfo;
            if (e.ColumnIndex == this.colRoleName.Index)
                Authority(userInfo);
            else if (e.ColumnIndex == this.colAdminDepts.Index)
            {
                SetAdminDepts(userInfo);
            }
        }

        private void SetAdminDepts(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                MessageBoxEx.ShowError("�û���Ϣ������");
                return;
            }
            AdminDeptsForm form = new AdminDeptsForm();
            form.UserInfo = userInfo;
            form.ShowDialog();
            if (form.ListQcAdminDepts != null && form.ListQcAdminDepts.Count > 0)
            {
                string szDeptName = string.Join(",", form.ListQcAdminDepts.Select(m => m.DEPT_NAME).ToArray());
                this.dataGridView1.SelectedRows[0].Cells[this.colAdminDepts.Index].Value = szDeptName;
            }
            if (form.ListQcAdminDepts.Count == 0)
            {
                this.dataGridView1.SelectedRows[0].Cells[this.colAdminDepts.Index].Value ="ȫԺ";
            }
        }
    }
}