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
using EMRDBLib;
using EMRDBLib.DbAccess;
using MedQCSys.DockForms;
using Heren.MedQC.Core;
namespace Heren.MedQC.Hdp
{
    public partial class RoleGrantForm : DockContentBase
    {
        private Dictionary<string, string> m_DicResource;
        private HdpRole m_roleInfo = null;
        public HdpRole RoleInfo
        {
            get { return this.m_roleInfo; }
            set { this.m_roleInfo = value; }
        }
        public RoleGrantForm()
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
            this.Update();
            this.OnRefreshView();
            if (this.m_roleInfo != null)
                this.Text = string.Format("���ڸ�{0}��Ȩ", this.m_roleInfo.RoleName);
        }

        public override void OnRefreshView()
        {
            if (!this.SaveUncommitedChange())
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ˢ����Դ�б����Ժ�...");

            if (!this.LoadCboProducts())
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.UpdateUIState();

            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }


        private bool LoadCboProducts()
        {
            this.toolcboProduct.Items.Clear();
            this.Update();
            List<HdpProduct> lstHdpProducts = null;
            short shRet =HdpProductAccess.Instance.GetHdpProductList(ref lstHdpProducts);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ��Ʒ��Ϣʧ�ܣ�");
                return false;
            }
            if (lstHdpProducts == null || lstHdpProducts.Count <= 0)
                return false;
            for (int index = 0; index < lstHdpProducts.Count; index++)
            {
                HdpProduct hdpProduct = lstHdpProducts[index];
                toolcboProduct.Items.Add(hdpProduct);
            }
            this.toolcboProduct.SelectedIndex = 0;
            return true;
        }
        private void LoadAllOperationType()
        {
            throw new Exception("The method or operation is not implemented.");
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
        private void LoadHdpRoleGrantList()
        {
            this.dataGridView1.Rows.Clear();
            if (this.toolcboProduct.SelectedItem == null)
                return;
            string szProduct = (this.toolcboProduct.SelectedItem as HdpProduct).NAME_SHORT;
            List<HdpRoleGrant> lstHdpRoleGrant = null;
            short shRet =HdpRoleGrantAccess.Instance.GetHdpRoleGrantList(this.m_roleInfo.RoleCode, szProduct, ref lstHdpRoleGrant);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                return;
            }
            if (lstHdpRoleGrant == null || lstHdpRoleGrant.Count <= 0)
                return;
            for (int index = 0; index < lstHdpRoleGrant.Count; index++)
            {
                HdpRoleGrant hdpRoleGrant = lstHdpRoleGrant[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = hdpRoleGrant; //����¼��Ϣ���浽����
                this.SetRowData(row, hdpRoleGrant);
                this.dataGridView1.SetRowState(row, RowState.Normal);
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

            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataTableViewRow currRow = this.dataGridView1.SelectedRows[0];
            if (!this.dataGridView1.IsDeletedRow(currRow))
            {
                if (this.dataGridView1.IsNormalRow(currRow)
                    || this.dataGridView1.IsModifiedRow(currRow))
                {
                    this.toolbtnModify.Enabled = true;
                    this.mnuModifyItem.Enabled = true;
                }
            }

            this.toolbtnDelete.Text = "ɾ��";
            this.mnuDeleteItem.Text = "ɾ��";
            this.toolbtnModify.Text = "�޸�";
            this.mnuModifyItem.Text = "�޸�";
            if (this.dataGridView1.IsDeletedRow(currRow))
            {
                this.toolbtnDelete.Text = "ȡ��ɾ��";
                this.mnuDeleteItem.Text = "ȡ��ɾ��";
            }
            else if (this.dataGridView1.IsModifiedRow(currRow))
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
        public bool HasUncommit()
        {
            if (this.dataGridView1.Rows.Count <= 0)
                return false;
            foreach (DataTableViewRow row in this.dataGridView1.Rows)
            {
                if (this.dataGridView1.IsDeletedRow(row))
                    return true;
                if (this.dataGridView1.IsNewRow(row))
                    return true;
                if (this.dataGridView1.IsModifiedRow(row))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �����ʿط������������б���޸�
        /// </summary>
        /// <returns>bool</returns>
        public bool CommitModify()
        {
            if (this.dataGridView1.Rows.Count <= 0)
                return true;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            int index = 0;
            int count = 0;
            short shRet = SystemData.ReturnValue.OK;
            this.dataGridView1.EndEdit();
            while (index < this.dataGridView1.Rows.Count)
            {
                DataTableViewRow row = this.dataGridView1.Rows[index];
                bool bIsDeletedRow = this.dataGridView1.IsDeletedRow(row);
                shRet = this.SaveRowData(row);
                if (shRet == SystemData.ReturnValue.OK)
                    count++;
                else if (shRet == SystemData.ReturnValue.FAILED)
                    break;
                if (!bIsDeletedRow) index++;
            }
            this.UpdateUIState();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            string szMessageText = null;
            if (shRet == SystemData.ReturnValue.FAILED)
                szMessageText = string.Format("������ֹ,�ѱ���{0}����¼��", count);
            else
                szMessageText = string.Format("����ɹ�,�ѱ���{0}����¼��", count);
            MessageBoxEx.Show(szMessageText, MessageBoxIcon.Information);
            return shRet == SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="hdpResource">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataGridViewRow row, HdpRoleGrant hdpRoleGrant)
        {
            if (row == null || row.Index < 0 || hdpRoleGrant == null)
                return false;
            row.Tag = hdpRoleGrant;

            row.Cells[this.colRoleRightKey.Index].Value = hdpRoleGrant.RoleRightKey;

            row.Cells[this.colRoleRightDesc.Index].Value = hdpRoleGrant.RoleRightDesc;

            row.Cells[this.colRoleRightCommand.Index].Value = hdpRoleGrant.RoleRightCommand;
            return true;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="hdpResource">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref  HdpRoleGrant hdpRoleGrant)
        {
            if (row == null || row.Index < 0)
                return false;
            hdpRoleGrant = new HdpRoleGrant();
            HdpRoleGrant oldHdpRoleGrant = row.Tag as HdpRoleGrant;
            if (!this.dataGridView1.IsNewRow(row))
            {
                if (oldHdpRoleGrant == null)
                {
                    MessageBoxEx.Show("��ɫ��Ȩ��ϢΪ�գ�");
                    return false;
                }
            }

            if (this.dataGridView1.IsDeletedRow(row))
            {
                hdpRoleGrant = oldHdpRoleGrant;
                return true;
            }

            if (this.toolcboProduct.SelectedItem == null || this.toolcboProduct.Text == string.Empty)
            {
                MessageBoxEx.Show("������ѡ���Ʒ��");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colRoleRightKey.Index].Value))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colRoleRightKey.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("������ѡ��Ȩ�޵㣡");
                return false;
            }
            if (hdpRoleGrant == null)
                hdpRoleGrant = new HdpRoleGrant();

            hdpRoleGrant.GrantID = oldHdpRoleGrant.GrantID;
            hdpRoleGrant.RoleCode = this.m_roleInfo.RoleCode;
            hdpRoleGrant.Product = (this.toolcboProduct.SelectedItem as HdpProduct).NAME_SHORT;

            hdpRoleGrant.RoleRightKey = (string)row.Cells[this.colRoleRightKey.Index].Value;

            hdpRoleGrant.RoleRightDesc = (string)row.Cells[this.colRoleRightDesc.Index].Value;
            if (row.Cells[this.colRoleRightCommand.Index].Value != null)

                hdpRoleGrant.RoleRightCommand = (string)row.Cells[this.colRoleRightCommand.Index].Value; ;

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

            HdpRoleGrant hdpRoleGrant = row.Tag as HdpRoleGrant;
            if (hdpRoleGrant == null)
                return SystemData.ReturnValue.FAILED;
            string szGrantID = hdpRoleGrant.GrantID;

            hdpRoleGrant = null;
            if (!this.MakeRowData(row, ref hdpRoleGrant))
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (!this.dataGridView1.IsNewRow(row))

                    shRet =HdpRoleGrantAccess.Instance.DeleteHdpRoleGrant(szGrantID);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                this.dataGridView1.Rows.Remove(row);
            }
            else if (this.dataGridView1.IsModifiedRow(row))
            {

                shRet =HdpRoleGrantAccess.Instance.ModifyHdpRoleGrant(szGrantID, hdpRoleGrant);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpRoleGrant;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            else if (this.dataGridView1.IsNewRow(row))
            {

                shRet =HdpRoleGrantAccess.Instance.SaveHdpRoleGrant(hdpRoleGrant);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpRoleGrant;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ�м�¼
        /// </summary>
        private void AddNewItem()
        {
            

            //��������
            HdpRoleGrant hdpRoleGrant = new HdpRoleGrant();
            //������
            int index = this.dataGridView1.Rows.Add();
            DataTableViewRow row = this.dataGridView1.Rows[index];
            hdpRoleGrant.GrantID = hdpRoleGrant.MakeGrantID();
            row.Tag = hdpRoleGrant;
            this.dataGridView1.SetRowState(row, RowState.New);
            this.UpdateUIState();

            this.dataGridView1.CurrentCell = row.Cells[this.colRoleRightKey.Index];
            this.dataGridView1.BeginEdit(true);
        }

        /// <summary>
        ///�޸�ѡ��������
        /// </summary>
        private void ModifySelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            if (this.dataGridView1.IsNormalRow(row))
            {
                this.dataGridView1.SetRowState(row, RowState.Update);
            }
            else if (this.dataGridView1.IsModifiedRow(row))
            {
                this.dataGridView1.SetRowState(row, RowState.Normal);
                this.SetRowData(row, row.Tag as HdpRoleGrant);
            }
            this.UpdateUIState();
            this.dataGridView1.CurrentCell = row.Cells[this.colRoleRightKey.Index];
            this.dataGridView1.BeginEdit(true);
        }

        /// <summary>
        /// ɾ��ѡ���м�¼
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (this.dataGridView1.IsNewRow(row))
                    this.dataGridView1.SetRowState(row, RowState.New);
                else if (this.dataGridView1.IsModifiedRow(row))
                    this.dataGridView1.SetRowState(row, RowState.Update);
                else if (this.dataGridView1.IsNormalRow(row))
                    this.dataGridView1.SetRowState(row, RowState.Normal);
                else
                    this.dataGridView1.SetRowState(row, RowState.Unknown);
            }
            else
            {
                this.dataGridView1.SetRowState(row, RowState.Delete);
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
            if (this.dataGridView1.CurrentCell != null)
                this.dataGridView1.BeginEdit(true);
            this.UpdateUIState();

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //if(this.colResourceCnName.Index==)
            this.UpdateUIState();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            //�������޸���ɾ����
            DataTableViewRow row = this.dataGridView1.Rows[e.RowIndex];
            if (this.dataGridView1.IsDeletedRow(row))
            {
                e.Cancel = true;
                return;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell currCell = this.dataGridView1.CurrentCell;

            if (this.dataGridView1.CurrentCell.ColumnIndex == this.colRoleRightKey.Index)
            {
                Heren.Common.Controls.TableView.FindComboBoxEditingControl control = (e.Control as Heren.Common.Controls.TableView.FindComboBoxEditingControl);
                control.SelectedIndexChanged -= new EventHandler(colResource_SelectedIndexChanged);
                control.SelectedIndexChanged += new EventHandler(colResource_SelectedIndexChanged);
            }
            TextBox textBoxExitingControl = e.Control as TextBox;
            if (textBoxExitingControl == null || textBoxExitingControl.IsDisposed)
                return;
            textBoxExitingControl.ImeMode = ImeMode.Alpha;
            textBoxExitingControl.KeyPress -= new KeyPressEventHandler(this.TextBoxExitingControl_KeyPress);
            textBoxExitingControl.KeyPress += new KeyPressEventHandler(this.TextBoxExitingControl_KeyPress);
        }
        private void colResource_SelectedIndexChanged(object sender, EventArgs e)
        {

            Heren.Common.Controls.TableView.FindComboBoxEditingControl control = sender as Heren.Common.Controls.TableView.FindComboBoxEditingControl;
            if (control.SelectedItem != null)
            {

                RightPoint rightPoint = control.SelectedItem as RightPoint;
                this.dataGridView1.CurrentRow.Cells[this.colRoleRightDesc.Index].Value = rightPoint.RightDesc;
                this.dataGridView1.CurrentRow.Cells[this.colRoleRightCommand.Index].Value = rightPoint.RightCommand;

            }
        }

        private void TextBoxExitingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            DataGridViewCell currCell = this.dataGridView1.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colRoleRightKey.Index)
                return;


        }




        private void toolcboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadGridCboList();
            this.LoadHdpRoleGrantList();
            this.UpdateUIState();
        }
        private void LoadGridCboList()
        {
            if (this.toolcboProduct.SelectedItem == null)
            {
                return;
            }
            string szProduct = (this.toolcboProduct.SelectedItem as HdpProduct).NAME_SHORT;
            //if (GlobalDataHandler.Instance.RightResourceInfo != null)
            //{
            //    foreach (AbstractRight item in GlobalDataHandler.Instance.RightResourceInfo)
            //    {
            //        RightPoint[] rights = item.GetRightPoints(szProduct);
            //        if (rights != null)
            //        {
            //            this.colRoleRightKey.Items.AddRange(rights);
            //        }
            //    }
            //}
        }

        private void toolbtnCopyGrant_Click(object sender, EventArgs e)
        {
            if (this.toolcboProduct.SelectedItem == null)
                return;
            HdpProduct hdpProduct = this.toolcboProduct.SelectedItem as HdpProduct;
            CopyRoleGrantForm form = new CopyRoleGrantForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                List<HdpRoleGrant> lstHdpRoleGrant = form.lstHdpRoleGrant;
                foreach (HdpRoleGrant hdpRoleGrant in form.lstHdpRoleGrant)
                {
                    if (hdpProduct.NAME_SHORT != hdpRoleGrant.Product)
                        continue;
                    //�ж��Ƿ��Ѵ������б���
                    if (this.CheckGrantRowData(hdpRoleGrant))
                        continue;
                    hdpRoleGrant.GrantID = hdpRoleGrant.MakeGrantID();
                    hdpRoleGrant.RoleCode = this.RoleInfo.RoleCode;
                    int rowIndex = this.dataGridView1.Rows.Add();
                    DataGridViewRow row = this.dataGridView1.Rows[rowIndex];
                    row.Tag = hdpRoleGrant;
                    row.Cells[this.colRoleRightKey.Index].Value = hdpRoleGrant.RoleRightKey;

                    row.Cells[this.colRoleRightDesc.Index].Value = hdpRoleGrant.RoleRightDesc;
                    row.Cells[this.colRoleRightCommand.Index].Value = hdpRoleGrant.RoleRightCommand;

                    this.dataGridView1.SetRowState(rowIndex, RowState.New);

                }
            }
        }
        /// <summary>
        /// ����Ƿ��Ѿ����б��д�����Ȩ��
        /// </summary>
        /// <param name="hdpRoleGrant"></param>
        private bool CheckGrantRowData(HdpRoleGrant hdpRoleGrant)
        {
            foreach (DataGridViewRow item in this.dataGridView1.Rows)
            {
                HdpRoleGrant rowHdpRoleGrant = (item.Tag as HdpRoleGrant);
                if (rowHdpRoleGrant != null && rowHdpRoleGrant.RoleRightKey == hdpRoleGrant.RoleRightKey)
                {
                    return true;
                }
            }
            return false;
        }
    }
}