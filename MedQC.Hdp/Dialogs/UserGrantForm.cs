using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls.TableView;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.Hdp
{
    public partial class UserGrantForm : Form
    {
        private UserInfo m_UserInfo = null;
        private List<HdpRoleUser> m_lstHdpRoleUser = null;
        public UserInfo UserInfo
        {
            get { return this.m_UserInfo; }
            set { this.m_UserInfo = value; }
        }
        public UserGrantForm()
        {
            InitializeComponent();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.LoadRoleUserList();
            this.LoadHdpRoleList();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            this.Text = string.Format("���ڸ�{0}��Ȩ", this.m_UserInfo.USER_NAME);
        }

        private void LoadRoleUserList()
        {
            if (this.m_lstHdpRoleUser == null)
                this.m_lstHdpRoleUser = new List<HdpRoleUser>();
            if (this.m_UserInfo == null)
                return;
            this.m_lstHdpRoleUser.Clear();
            short shRet = HdpRoleUserAccess.Instance.GetHdpRoleUserList(this.m_UserInfo.USER_ID, ref m_lstHdpRoleUser);

        }

        private void LoadHdpRoleList()
        {
            this.dataGridView1.Rows.Clear();
            List<HdpRole> lstHdpRoles = null;
            short shRet =HdpRoleAccess.Instance.GetHdpRoleList(string.Empty, ref lstHdpRoles);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                return;
            }
            if (lstHdpRoles == null || lstHdpRoles.Count <= 0)
                return;
            for (int index = 0; index < lstHdpRoles.Count; index++)
            {
                HdpRole hdpRole = lstHdpRoles[index];

                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = hdpRole; //����¼��Ϣ���浽����
                this.SetRowData(row, hdpRole);
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
        }
        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="hdpRole">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataGridViewRow row, HdpRole hdpRole)
        {
            if (row == null || row.Index < 0 || hdpRole == null)
                return false;
            row.Tag = hdpRole;
            row.Cells[this.colRoleName.Index].Value = hdpRole.RoleName;
            row.Cells[this.colRoleCode.Index].Value = hdpRole.RoleCode;
            row.Cells[this.colStatus.Index].Value = hdpRole.Status == "1" ? "����" : "�ر�";
            row.Cells[this.colCheckBox.Index].Value = false;
            foreach (HdpRoleUser item in this.m_lstHdpRoleUser)
            {
                if (item.RoleCode == hdpRole.RoleCode)
                {
                    row.Cells[this.colCheckBox.Index].Value = true;
                    break;
                }
            }
            return true;
        }
        private List<HdpRoleUser> m_lstCheckHdpRoleUser = null;
        public List<HdpRoleUser> CheckHdpRoleUserList
        {
            get
            {
                if (this.m_lstCheckHdpRoleUser == null)
                    this.m_lstCheckHdpRoleUser = new List<HdpRoleUser>();
                return this.m_lstCheckHdpRoleUser;
            }
            set
            {
                this.m_lstCheckHdpRoleUser = value;
            }
        }
        private void toolbtnSave_Click(object sender, EventArgs e)
        {
            if (this.m_lstCheckHdpRoleUser == null)
                this.m_lstCheckHdpRoleUser = new List<HdpRoleUser>();
            m_lstCheckHdpRoleUser.Clear();
            for (int index = 0; index < this.dataGridView1.Rows.Count; index++)
            {
                HdpRole hdpRole = this.dataGridView1.Rows[index].Tag as HdpRole;
                if ((bool)this.dataGridView1.Rows[index].Cells[this.colCheckBox.Index].Value == true)
                {
                    HdpRoleUser hdpRoleUser = new HdpRoleUser();
                    hdpRoleUser.UserID = this.m_UserInfo.USER_ID;
                    hdpRoleUser.RoleCode = hdpRole.RoleCode;
                    m_lstCheckHdpRoleUser.Add(hdpRoleUser);
                }
            }
            short shRet =HdpRoleUserAccess.Instance.SaveRoleUserList(this.m_UserInfo.USER_ID, CheckHdpRoleUserList);
            //�����û���¼������һ���û��˺�������Ϣ��USER_RIGHT_T����
            UserRightBase userRight = UserRightBase.Create(UserRightType.MedQC);
            userRight.UserID = this.m_UserInfo.USER_ID;
            userRight.RightType = UserRightType.MedQC;
            shRet =RightAccess.Instance.GetUserRight(this.m_UserInfo.USER_ID, UserRightType.MedQC, ref userRight);

            shRet =RightAccess.Instance.SaveUserRight(userRight);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowError("��Ȩʧ��");
                return;
            }
            MessageBoxEx.ShowMessage("��Ȩ�ɹ�");

        }

        private void UserGrantForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.m_lstCheckHdpRoleUser == null)
                this.m_lstCheckHdpRoleUser = new List<HdpRoleUser>();
            this.m_lstCheckHdpRoleUser.Clear();
            for (int index = 0; index < this.dataGridView1.Rows.Count; index++)
            {
                HdpRole hdpRole = this.dataGridView1.Rows[index].Tag as HdpRole;
                if ((bool)this.dataGridView1.Rows[index].Cells[this.colCheckBox.Index].Value == true)
                {
                    HdpRoleUser hdpRoleUser = new HdpRoleUser();
                    hdpRoleUser.UserID = this.m_UserInfo.USER_ID;
                    hdpRoleUser.RoleCode = hdpRole.RoleCode;
                    hdpRoleUser.RoleName = this.dataGridView1.Rows[index].Cells[this.colRoleName.Index].Value.ToString();
                    this.m_lstCheckHdpRoleUser.Add(hdpRoleUser);
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if ((bool)this.dataGridView1.Rows[e.RowIndex].Cells[this.colCheckBox.Index].Value == true)
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[this.colCheckBox.Index].Value = false;
            }
            else
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[this.colCheckBox.Index].Value = true;
            }
        }
    }
}