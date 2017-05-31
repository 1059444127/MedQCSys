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
using Heren.MedQC.Hdp.Properties;

namespace Heren.MedQC.Hdp
{
    public partial class ProductManageForm : DockContentBase
    {
        public ProductManageForm(MainForm form) : base(form)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dgvProductList.Font = new Font("����", 10.5f);
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
            this.LoadHdpProductList();
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
        private void LoadHdpProductList()
        {
            this.dgvProductList.Rows.Clear();
            List<HdpProduct> lstHdpProducts = null;
            short shRet = HdpProductAccess.Instance.GetHdpProductList(ref lstHdpProducts);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                return;
            }
            if (lstHdpProducts == null || lstHdpProducts.Count <= 0)
                return;
            for (int index = 0; index < lstHdpProducts.Count; index++)
            {
                HdpProduct hdpProduct = lstHdpProducts[index];

                int nRowIndex = this.dgvProductList.Rows.Add();
                DataTableViewRow row = this.dgvProductList.Rows[nRowIndex];
                row.Tag = hdpProduct; //����¼��Ϣ���浽����
                this.SetRowData(row, hdpProduct);
                this.dgvProductList.SetRowState(row, RowState.Normal);
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

            if (this.dgvProductList.SelectedRows.Count <= 0)
                return;
            DataTableViewRow currRow = this.dgvProductList.SelectedRows[0];
            if (!this.dgvProductList.IsDeletedRow(currRow))
            {
                if (this.dgvProductList.IsNormalRow(currRow)
                    || this.dgvProductList.IsModifiedRow(currRow))
                {
                    this.toolbtnModify.Enabled = true;
                    this.mnuModifyItem.Enabled = true;
                  
                }
            }

            this.toolbtnDelete.Text = "ɾ��";
            this.mnuDeleteItem.Text = "ɾ��";
            this.toolbtnModify.Text = "�޸�";
            this.mnuModifyItem.Text = "�޸�";
            if (this.dgvProductList.IsDeletedRow(currRow))
            {
                this.toolbtnDelete.Text = "ȡ��ɾ��";
                this.mnuDeleteItem.Text = "ȡ��ɾ��";
            }
            else if (this.dgvProductList.IsModifiedRow(currRow))
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
            if (this.dgvProductList.Rows.Count <= 0)
                return false;
            foreach (DataTableViewRow row in this.dgvProductList.Rows)
            {
                if (this.dgvProductList.IsDeletedRow(row))
                    return true;
                if (this.dgvProductList.IsNewRow(row))
                    return true;
                if (this.dgvProductList.IsModifiedRow(row))
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
            if (this.dgvProductList.Rows.Count <= 0) return true;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            int index = 0;
            int count = 0;
            short shRet = SystemData.ReturnValue.OK;
            this.dgvProductList.EndEdit();
            while (index < this.dgvProductList.Rows.Count)
            {
                DataTableViewRow row = this.dgvProductList.Rows[index];
                bool bIsDeletedRow = this.dgvProductList.IsDeletedRow(row);
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
        /// <param name="hdpProduct">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataGridViewRow row, HdpProduct hdpProduct)
        {
            if (row == null || row.Index < 0 || hdpProduct == null)
                return false;
            row.Tag = hdpProduct;
            row.Cells[this.colSerialNO.Index].Value = hdpProduct.SERIAL_NO;
            row.Cells[this.colCnName.Index].Value = hdpProduct.CN_NAME;
            row.Cells[this.colEnName.Index].Value = hdpProduct.EN_NAME;
            row.Cells[this.colNameShort.Index].Value = hdpProduct.NAME_SHORT;
            row.Cells[this.colProductBak.Index].Value = hdpProduct.PRODUCT_BAK;

            return true;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="hdpProduct">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref HdpProduct hdpProduct)
        {
            if (row == null || row.Index < 0)
                return false;
            hdpProduct = new HdpProduct();
            HdpProduct oldHdpProduct = row.Tag as HdpProduct;
            if (!this.dgvProductList.IsNewRow(row))
            {
                if (oldHdpProduct == null)
                {
                    MessageBoxEx.Show("������������ֵ���������ϢΪ�գ�");
                    return false;
                }
            }

            if (this.dgvProductList.IsDeletedRow(row))
            {
                hdpProduct = oldHdpProduct;
                return true;
            }
            if (GlobalMethods.Misc.IsEmptyString(row.Cells[this.colSerialNO.Index].Value.ToString()))
            {
                this.dgvProductList.CurrentCell = row.Cells[this.colSerialNO.Index];
                this.dgvProductList.BeginEdit(true);
                MessageBoxEx.Show("������������ţ�");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colNameShort.Index].Value))
            {
                this.dgvProductList.CurrentCell = row.Cells[this.colNameShort.Index];
                this.dgvProductList.BeginEdit(true);
                MessageBoxEx.Show("���������ò�Ʒ��д��");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colEnName.Index].Value))
            {
                this.dgvProductList.CurrentCell = row.Cells[this.colEnName.Index];
                this.dgvProductList.BeginEdit(true);
                MessageBoxEx.Show("����������Ӣ�����ƣ�");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colCnName.Index].Value))
            {
                MessageBoxEx.Show("�����������������ƣ�");
                return false;
            }
            if (hdpProduct == null) hdpProduct = new HdpProduct();
            hdpProduct.SERIAL_NO = row.Cells[this.colSerialNO.Index].Value.ToString();
            hdpProduct.NAME_SHORT = (string)row.Cells[this.colNameShort.Index].Value;
            hdpProduct.CN_NAME = (string)row.Cells[this.colCnName.Index].Value;
            hdpProduct.EN_NAME = (string)row.Cells[this.colEnName.Index].Value;
            hdpProduct.PRODUCT_BAK = row.Cells[this.colProductBak.Index].Value == null ? "" : (string)row.Cells[this.colProductBak.Index].Value;
            return true;
        }

        /// <summary>
        /// ����ָ���е����ݵ�Զ�����ݱ�,��Ҫע����ǣ��е�ɾ��״̬��������״̬����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <returns>SystemData.ReturnValue</returns>
        private short SaveRowData(DataTableViewRow row)
        {
            if (row == null || row.Index < 0) return SystemData.ReturnValue.FAILED;
            if (this.dgvProductList.IsNormalRow(row) || this.dgvProductList.IsUnknownRow(row))
            {
                if (!this.dgvProductList.IsDeletedRow(row)) return SystemData.ReturnValue.CANCEL;
            }

            HdpProduct hdpProduct = row.Tag as HdpProduct;
            if (hdpProduct == null) return SystemData.ReturnValue.FAILED;
            string szNameShort = hdpProduct.NAME_SHORT;

            hdpProduct = null;
            if (!this.MakeRowData(row, ref hdpProduct)) return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            if (this.dgvProductList.IsDeletedRow(row))
            {
                if (!this.dgvProductList.IsNewRow(row)) shRet = HdpProductAccess.Instance.DeleteHdpProduct(szNameShort);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dgvProductList.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                this.dgvProductList.Rows.Remove(row);
            }
            else if (this.dgvProductList.IsModifiedRow(row))
            {

                shRet = HdpProductAccess.Instance.ModifyHdpProduct(hdpProduct, szNameShort);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dgvProductList.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpProduct;
                this.dgvProductList.SetRowState(row, RowState.Normal);
            }
            else if (this.dgvProductList.IsNewRow(row))
            {

                shRet = HdpProductAccess.Instance.SaveHdpProduct(hdpProduct);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dgvProductList.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpProduct;
                this.dgvProductList.SetRowState(row, RowState.Normal);
            }
    
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ�м�¼
        /// </summary>
        private void AddNewItem()
        {
            //��������
            HdpProduct qcEventInfo = new HdpProduct();
            //������
            int maxNo = 0;
            foreach (DataTableViewRow oneRow in dgvProductList.Rows)
            {
                string szSerialNO = oneRow.Cells[this.colSerialNO.Index].Value.ToString();
                if (!string.IsNullOrEmpty(szSerialNO) && maxNo < int.Parse(szSerialNO)) maxNo = int.Parse(szSerialNO);
            }
            int index = this.dgvProductList.Rows.Add();
            DataTableViewRow row = this.dgvProductList.Rows[index];
            row.Cells[this.colSerialNO.Index].Value = maxNo + 1;
            row.Tag = qcEventInfo;
            this.dgvProductList.SetRowState(row, RowState.New);
            this.UpdateUIState();
            this.dgvProductList.CurrentCell = row.Cells[this.colNameShort.Index];
            this.dgvProductList.BeginEdit(true);
        }

        /// <summary>
        ///�޸�ѡ��������
        /// </summary>
        private void ModifySelectedItem()
        {
            if (this.dgvProductList.SelectedRows.Count <= 0)
                return;


            DataTableViewRow row = this.dgvProductList.SelectedRows[0];
          
            if (this.dgvProductList.IsNormalRow(row))
            {
                this.dgvProductList.SetRowState(row, RowState.Update);
            }
            else if (this.dgvProductList.IsModifiedRow(row))
            {
                this.dgvProductList.SetRowState(row, RowState.Normal);
                this.SetRowData(row, row.Tag as HdpProduct);
            }
            this.UpdateUIState();
            this.dgvProductList.CurrentCell = row.Cells[this.colNameShort.Index];
            this.dgvProductList.BeginEdit(true);
        }

        /// <summary>
        /// ɾ��ѡ���м�¼
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.dgvProductList.SelectedRows.Count <= 0)
                return;


            DataTableViewRow row = this.dgvProductList.SelectedRows[0];
            if (this.dgvProductList.IsDeletedRow(row))
            {
                if (this.dgvProductList.IsNewRow(row))
                    this.dgvProductList.SetRowState(row, RowState.New);
                else if (this.dgvProductList.IsModifiedRow(row))
                    this.dgvProductList.SetRowState(row, RowState.Update);
                else if (this.dgvProductList.IsNormalRow(row))
                    this.dgvProductList.SetRowState(row, RowState.Normal);
                else
                    this.dgvProductList.SetRowState(row, RowState.Unknown);
            }
            else
            {
                this.dgvProductList.SetRowState(row, RowState.Delete);
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
            if (this.dgvProductList.CurrentCell != null)
                this.dgvProductList.BeginEdit(true);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.UpdateUIState();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            //�������޸���ɾ����
            DataTableViewRow row = this.dgvProductList.Rows[e.RowIndex];
            if (this.dgvProductList.IsDeletedRow(row))
            {
                e.Cancel = true;
                return;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell currCell = this.dgvProductList.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colNameShort.Index)
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
            DataGridViewCell currCell = this.dgvProductList.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colNameShort.Index)
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.ModifySelectedItem();
        }

        private void tsbSetDefault_Click(object sender, EventArgs e)
        {
            if (this.dgvProductList.SelectedRows.Count <= 0)
                return;
            DataGridViewRow selectedRow = this.dgvProductList.SelectedRows[0];
            if (selectedRow == null)
                return;
        }
    }
}