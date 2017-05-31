// ***********************************************************
// �����ʿ�ϵͳ�ʿ���������ά������.
// Creator:YangMingkun  Date:2009-11-13
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
using Heren.Common.Controls.TableView;
using Heren.Common.DockSuite;
using MedQCSys.DockForms;
using EMRDBLib;
using EMRDBLib.DbAccess;
using MedQCSys;

namespace Heren.MedQC.Hdp
{
    public partial class RoleManageForm : DockContentBase
    {
        public RoleManageForm(MainForm form):base(form)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dgvRoleList.Font = new Font("����", 10.5f);
            //this.dataGridView1.AutoReadonly = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();
            this.OnRefreshView();
        }

        public override void OnRefreshView()
        {
            if (!this.SaveUncommitedChange())
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ˢ��������������ֵ䣬���Ժ�...");
            //if (this.colApplyEnv.Items.Count > 0)
            //    this.colApplyEnv.Items.Clear();
            //this.colApplyEnv.Items.Add(string.Empty);
            //if (GlobalDataHandler.Instance.QCUserRight.BrowseDocumentList.Value)
            //    this.colApplyEnv.Items.Add("ҽ������");
            //if (GlobalDataHandler.Instance.QCUserRight.BrowseNurDocList.Value)
            //    this.colApplyEnv.Items.Add("��������");
            this.LoadHdpRoleList();
            this.UpdateUIState();

            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void ShowStatusMessage(string p)
        {
            //throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// ����Ƿ�����Ҫ���������
        /// </summary>
        /// <returns>�Ƿ񱣴�ɹ�</returns>
        public virtual bool SaveUncommitedChange()
        {
            if (!this.HasUncommit())
                return true;
            this.DockHandler.Activate();
            DialogResult result = MessageBoxEx.Show("��ǰ��δ������޸�,�Ƿ񱣴棿"
                , MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return false;
            else if (result == DialogResult.Yes)
                return this.CommitModify();
            return true;
        }
        /// <summary>
        /// װ�ز����������������Ϣ
        /// </summary>
        private void LoadHdpRoleList()
        {
            this.dgvRoleList.Rows.Clear();
            List<HdpRole> lstHdpRoles = null;
            short shRet =HdpRoleAccess.Instance.GetHdpRoleList(string.Empty,ref lstHdpRoles);
            if (shRet != SystemData.ReturnValue.OK
                && shRet!=SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                return;
            }
            if (lstHdpRoles == null || lstHdpRoles.Count <= 0)
                return;
            for (int index = 0; index < lstHdpRoles.Count; index++)
            {
                HdpRole hdpRole = lstHdpRoles[index];

                int nRowIndex = this.dgvRoleList.Rows.Add();
                DataTableViewRow row = this.dgvRoleList.Rows[nRowIndex];
                row.Tag = hdpRole; //����¼��Ϣ���浽����
                this.SetRowData(row, hdpRole);
                this.dgvRoleList.SetRowState(row, RowState.Normal);
            }
        }

        /// <summary>
        /// �޸Ľ��水ť״̬
        /// </summary>
        private void UpdateUIState()
        {
            this.toolbtnModify.Enabled = false;
            this.mnuModifyItem.Enabled = false;
            this.toolbtnDelete.Enabled = false;
            this.mnuDeleteItem.Enabled = false;
            this.toolbtnAuthority.Enabled = false;

            if (this.dgvRoleList.SelectedRows.Count <= 0) return;
            DataTableViewRow currRow = this.dgvRoleList.SelectedRows[0];
            if (!this.dgvRoleList.IsDeletedRow(currRow))
            {
                if (this.dgvRoleList.IsNormalRow(currRow) || this.dgvRoleList.IsModifiedRow(currRow))
                {
                    this.toolbtnModify.Enabled = true;
                    this.mnuModifyItem.Enabled = true;
                    this.toolbtnAuthority.Enabled = true;
                }
            }

            this.toolbtnDelete.Text = "ɾ��";
            this.mnuDeleteItem.Text = "ɾ��";
            this.toolbtnModify.Text = "�޸�";
            this.mnuModifyItem.Text = "�޸�";
            if (this.dgvRoleList.IsDeletedRow(currRow))
            {
                this.toolbtnDelete.Text = "ȡ��ɾ��";
                this.mnuDeleteItem.Text = "ȡ��ɾ��";
            }
            else if (this.dgvRoleList.IsModifiedRow(currRow))
            {
                this.toolbtnModify.Text = "ȡ���޸�";
                this.mnuModifyItem.Text = "ȡ���޸�";
            }
            this.toolbtnDelete.Enabled = true;
            this.mnuDeleteItem.Enabled = true;
            this.toolbtnSave.Enabled = true;
            this.mnuSaveItems.Enabled = true;
        }

        /// <summary>
        /// ����Ƿ���δ�ύ���޸�
        /// </summary>
        /// <returns>bool</returns>
        public  bool HasUncommit()
        {
            if (this.dgvRoleList.Rows.Count <= 0)
                return false;
            foreach (DataTableViewRow row in this.dgvRoleList.Rows)
            {
                if (this.dgvRoleList.IsDeletedRow(row))
                    return true;
                if (this.dgvRoleList.IsNewRow(row))
                    return true;
                if (this.dgvRoleList.IsModifiedRow(row))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �����ʿط������������б���޸�
        /// </summary>
        /// <returns>bool</returns>
        public  bool CommitModify()
        {
            if (this.dgvRoleList.Rows.Count <= 0) return true;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            int index = 0;
            int count = 0;
            short shRet = SystemData.ReturnValue.OK;
            this.dgvRoleList.EndEdit();
            while (index < this.dgvRoleList.Rows.Count)
            {
                DataTableViewRow row = this.dgvRoleList.Rows[index];
                bool bIsDeletedRow = this.dgvRoleList.IsDeletedRow(row);
                shRet = this.SaveRowData(row);
                if (shRet == SystemData.ReturnValue.OK) count++;
                else if (shRet == SystemData.ReturnValue.FAILED) break;
                if (!bIsDeletedRow) index++;
            }
            this.UpdateUIState();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            string szMessageText = null;
            if (shRet == SystemData.ReturnValue.FAILED) szMessageText = string.Format("������ֹ,�ѱ���{0}����¼��", count);
            else szMessageText = string.Format("����ɹ�,�ѱ���{0}����¼��", count);
            MessageBoxEx.Show(szMessageText, MessageBoxIcon.Information);
            return shRet == SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="hdpRole">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataGridViewRow row,  HdpRole hdpRole)
        {
            if (row == null || row.Index < 0 || hdpRole == null)
                return false;
            row.Tag = hdpRole;
            row.Cells[this.colSerialNO.Index].Value = hdpRole.SerialNo;
            row.Cells[this.colRoleName.Index].Value = hdpRole.RoleName;
            row.Cells[this.colRoleCode.Index].Value = hdpRole.RoleCode;
            row.Cells[this.colStatus.Index].Value = hdpRole.Status=="1"?"����":"�ر�";
            row.Cells[this.colRoleBak.Index].Value = hdpRole.RoleBak;

            return true;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="hdpRole">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref  HdpRole hdpRole)
        {
            if (row == null || row.Index < 0)
                return false;
            hdpRole = new  HdpRole();
             HdpRole oldHdpRole = row.Tag as  HdpRole;
            if (!this.dgvRoleList.IsNewRow(row))
            {
                if (oldHdpRole == null)
                {
                    MessageBoxEx.Show("������������ֵ���������ϢΪ�գ�");
                    return false;
                }
            }

            if (this.dgvRoleList.IsDeletedRow(row))
            {
                hdpRole = oldHdpRole;
                return true;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colSerialNO.Index].Value))
            {
                this.dgvRoleList.CurrentCell = row.Cells[this.colSerialNO.Index];
                this.dgvRoleList.BeginEdit(true);
                MessageBoxEx.Show("������������ţ�");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colRoleName.Index].Value))
            {
                this.dgvRoleList.CurrentCell = row.Cells[this.colRoleName.Index];
                this.dgvRoleList.BeginEdit(true);
                MessageBoxEx.Show("���������ý�ɫ���ƣ�");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colRoleCode.Index].Value))
            {
                this.dgvRoleList.CurrentCell = row.Cells[this.colRoleCode.Index];
                this.dgvRoleList.BeginEdit(true);
                MessageBoxEx.Show("���������ý�ɫ���룡");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colStatus.Index].Value))
            {
                MessageBoxEx.Show("����������״̬��");
                return false;
            }
            if (hdpRole == null)
                hdpRole = new  HdpRole();
            hdpRole.SerialNo = (string)row.Cells[this.colSerialNO.Index].Value;
            hdpRole.RoleName = (string)row.Cells[this.colRoleName.Index].Value;
            hdpRole.RoleCode = (string)row.Cells[this.colRoleCode.Index].Value;
            hdpRole.Status = (string)row.Cells[this.colStatus.Index].Value=="����"?"1":"0";
            hdpRole.RoleBak = row.Cells[this.colRoleBak.Index].Value == null ? "" : (string)row.Cells[this.colRoleBak.Index].Value;
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
            if (this.dgvRoleList.IsNormalRow(row) || this.dgvRoleList.IsUnknownRow(row))
            {
                if (!this.dgvRoleList.IsDeletedRow(row))
                    return SystemData.ReturnValue.CANCEL;
            }

             HdpRole hdpRole = row.Tag as  HdpRole;
            if (hdpRole == null)
                return SystemData.ReturnValue.FAILED;
            string szRoleCode = hdpRole.RoleCode;

            hdpRole = null;
            if (!this.MakeRowData(row, ref hdpRole))
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            if (this.dgvRoleList.IsDeletedRow(row))
            {
                if (!this.dgvRoleList.IsNewRow(row))

                    shRet = HdpRoleAccess.Instance.DeleteHdpRole(szRoleCode);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dgvRoleList.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                this.dgvRoleList.Rows.Remove(row);
            }
            else if (this.dgvRoleList.IsModifiedRow(row))
            {

                shRet =HdpRoleAccess.Instance.ModifyHdpRole(szRoleCode,hdpRole);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dgvRoleList.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpRole;
                this.dgvRoleList.SetRowState(row, RowState.Normal);
            }
            else if (this.dgvRoleList.IsNewRow(row))
            {

                shRet = HdpRoleAccess.Instance.SaveHdpRole(hdpRole);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dgvRoleList.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpRole;
                this.dgvRoleList.SetRowState(row, RowState.Normal);
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ�м�¼
        /// </summary>
        private void AddNewItem()
        {
            //��������
             HdpRole qcEventInfo = new  HdpRole();
            //������
            int index = this.dgvRoleList.Rows.Add();
            DataTableViewRow row = this.dgvRoleList.Rows[index];
            row.Tag = qcEventInfo;
            this.dgvRoleList.SetRowState(row, RowState.New);
            this.UpdateUIState();

            this.dgvRoleList.CurrentCell = row.Cells[this.colSerialNO.Index];
            this.dgvRoleList.BeginEdit(true);
        }

        /// <summary>
        ///�޸�ѡ��������
        /// </summary>
        private void ModifySelectedItem()
        {
            if (this.dgvRoleList.SelectedRows.Count <= 0)
                return;

            
            DataTableViewRow row = this.dgvRoleList.SelectedRows[0];
            if (this.dgvRoleList.IsNormalRow(row))
            {
                this.dgvRoleList.SetRowState(row, RowState.Update);
            }
            else if (this.dgvRoleList.IsModifiedRow(row))
            {
                this.dgvRoleList.SetRowState(row, RowState.Normal);
                this.SetRowData(row, row.Tag as  HdpRole);
            }
            this.UpdateUIState();
            this.dgvRoleList.CurrentCell = row.Cells[this.colSerialNO.Index];
            this.dgvRoleList.BeginEdit(true);
        }

        /// <summary>
        /// ɾ��ѡ���м�¼
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.dgvRoleList.SelectedRows.Count <= 0)
                return;

            
            DataTableViewRow row = this.dgvRoleList.SelectedRows[0];
            if (this.dgvRoleList.IsDeletedRow(row))
            {
                if (this.dgvRoleList.IsNewRow(row))
                    this.dgvRoleList.SetRowState(row, RowState.New);
                else if (this.dgvRoleList.IsModifiedRow(row))
                    this.dgvRoleList.SetRowState(row, RowState.Update);
                else if (this.dgvRoleList.IsNormalRow(row))
                    this.dgvRoleList.SetRowState(row, RowState.Normal);
                else
                    this.dgvRoleList.SetRowState(row, RowState.Unknown);
            }
            else
            {
                this.dgvRoleList.SetRowState(row, RowState.Delete);
            }
            this.UpdateUIState();
        }

        private void toolbtnNew_Click(object sender, EventArgs e)
        {
            this.AddNewItem();
        }

        private void toolbtnModify_Click(object sender, EventArgs e)
        {
            this.ModifySelectedItem();
        }

        private void toolbtnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedItem();
        }

        private void toolbtnSave_Click(object sender, EventArgs e)
        {
            this.Focus();
            this.CommitModify();
        }

        private void mnuAddItem_Click(object sender, EventArgs e)
        {
            this.AddNewItem();
        }

        private void mnuModifyItem_Click(object sender, EventArgs e)
        {
            this.ModifySelectedItem();
        }

        private void mnuDeleteItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedItem();
        }

        private void mnuSaveItems_Click(object sender, EventArgs e)
        {
            this.CommitModify();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvRoleList.CurrentCell != null)
                this.dgvRoleList.BeginEdit(true);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.UpdateUIState();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            //�������޸���ɾ����
            DataTableViewRow row = this.dgvRoleList.Rows[e.RowIndex];
            if (this.dgvRoleList.IsDeletedRow(row))
            {
                e.Cancel = true;
                return;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell currCell = this.dgvRoleList.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colRoleCode.Index)
                return;

            TextBox textBoxExitingControl = e.Control as TextBox;
            if (textBoxExitingControl == null || textBoxExitingControl.IsDisposed)
                return;
            textBoxExitingControl.ImeMode = ImeMode.Alpha;
            textBoxExitingControl.KeyPress -= new KeyPressEventHandler(this.TextBoxExitingControl_KeyPress);
            textBoxExitingControl.KeyPress += new KeyPressEventHandler(this.TextBoxExitingControl_KeyPress);
        }

        private void TextBoxExitingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            DataGridViewCell currCell = this.dgvRoleList.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colRoleCode.Index)
                return;

            if (currCell.ColumnIndex == this.colSerialNO.Index)
            {
                if (e.KeyChar == (char)((int)Keys.Back))
                    return;
                if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
                    return;
                e.Handled = true;
            }
            
        }

        private void toolbtnAuthority_Click(object sender, EventArgs e)
        {
            if(this.dgvRoleList.SelectedRows.Count<=0)
                return;
            HdpRole hdpRole=this.dgvRoleList.SelectedRows[0].Tag as HdpRole;
            if (hdpRole == null)
            {
                MessageBoxEx.Show("��ȡ��ɫ��Ϣʧ��");
                return;
            }
            //RoleGrantForm form = new RoleGrantForm();
            RoleGrantCheckListForm form = new RoleGrantCheckListForm();
            form.RoleInfo = hdpRole;
            form.ShowDialog();
        }
    }
}