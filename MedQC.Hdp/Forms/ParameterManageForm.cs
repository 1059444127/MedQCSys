// ***********************************************************
// �����༭�����ù���ϵͳ��̨�����ֵ�����ù�����.
// Creator:YangMingkun  Date:2010-11-29
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;
using Heren.Common.Controls.TableView;
using MedQCSys;
using MedQCSys.DockForms;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.Hdp
{
    internal partial class ParameterManageForm : DockContentBase
    {
        public ParameterManageForm(MainForm form):base(form)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
        }
        

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();
            this.OnRefreshView();
            //�������������������б�
            FieldInfo[] fieldInfos = typeof(SystemData.ConfigKey).GetFields();
            if (fieldInfos == null || fieldInfos.Length <= 0)
                return;
            for (int index = 0; index < fieldInfos.Length; index++)
            {
                FieldInfo fieldInfo = fieldInfos[index];
                if (fieldInfo == null)
                    continue;
                object value = fieldInfo.GetValue(null);
                if (value == null)
                    continue;
                string szValue = value.ToString();
                if (!this.colConfigName.Items.Contains(szValue))
                    this.colConfigName.Items.Add(szValue);
                if (!this.colConfigGroup.Items.Contains(szValue))
                    this.colConfigGroup.Items.Add(szValue);
            }
            if (this.colConfigName.Items.Count > 0) this.colConfigName.Items.Sort();
            if (this.colConfigGroup.Items.Count > 0) this.colConfigGroup.Items.Sort();
        }

        /// <summary>
        /// ˢ�µ�ǰ���ڵ�������ʾ
        /// </summary>
        public override void OnRefreshView()
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (!this.CheckModifiedData())
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.LoadParameterList();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void LoadParameterList()
        {
            this.dataGridView1.Rows.Clear();
          
         
            List<HdpParameter> lstHdpParameter = null;

            short shRet =HdpParameterAccess.Instance.GetHdpParameters(null, null, null, ref lstHdpParameter);
            if (shRet == SystemData.ReturnValue.RES_NO_FOUND)
                return;
            if (shRet != SystemData.ReturnValue.OK)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                MessageBoxEx.Show("�����ֵ�����ʧ��!");
            }
            if (lstHdpParameter == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }

            for (int index = 0; index < lstHdpParameter.Count; index++)
            {
                HdpParameter hdpParameter = lstHdpParameter[index];
                if (hdpParameter == null)
                    continue;
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                this.SetRowData(row, hdpParameter);
                this.dataGridView1.SetRowState(row, RowState.Normal);

                if (nRowIndex <= 0)
                    continue;

                DataTableViewRow prevRow = this.dataGridView1.Rows[nRowIndex - 1];
                if (!string.Equals(row.Cells[this.colConfigGroup.Index].Value
                    , prevRow.Cells[this.colConfigGroup.Index].Value))
                {
                    if (prevRow.DefaultCellStyle.BackColor == Color.Gainsboro)
                        row.DefaultCellStyle.BackColor = Color.White;
                    else
                        row.DefaultCellStyle.BackColor = Color.Gainsboro;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = prevRow.DefaultCellStyle.BackColor;
                }
            }
        }
        /// <summary>
        /// ����Ƿ���δ���������
        /// </summary>
        /// <returns>bool</returns>
        public override bool HasUncommit()
        {
            int nCount = this.dataGridView1.Rows.Count;
            if (nCount <= 0)
                return false;
            for (int index = 0; index < nCount; index++)
            {
                DataTableViewRow row = this.dataGridView1.Rows[index];
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
        /// ���浱ǰ���ڵ������޸�
        /// </summary>
        /// <returns>bool</returns>
        public override bool CommitModify()
        {
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
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            string szMessageText = null;
            if (shRet == SystemData.ReturnValue.FAILED)
                szMessageText = string.Format("������ֹ,�ѱ���{0}����¼!", count);
            else
                szMessageText = string.Format("����ɹ�,�ѱ���{0}����¼!", count);
            if (count > 0)
                szMessageText += "\r\nϵͳ�����ѱ�����,�������µ�¼����Ч!";
            this.UpdateUIState();
            MessageBoxEx.Show(szMessageText, MessageBoxIcon.Information);
            return shRet == SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="configInfo">�󶨵�����</param>
        private bool SetRowData(DataTableViewRow row, HdpParameter parameter)
        {
            if (row == null || row.Index < 0 || parameter == null)
                return false;
            row.Tag = parameter;
            row.Cells[this.colConfigGroup.Index].Value = parameter.GROUP_NAME;
            row.Cells[this.colConfigName.Index].Value = parameter.CONFIG_NAME;
            row.Cells[this.colConfigValue.Index].Value = parameter.CONFIG_VALUE;
            row.Cells[this.colConfigDesc.Index].Value = parameter.CONFIG_DESC;
            return true;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="configInfo">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref HdpParameter hdpParameter)
        {
            
            if (row == null || row.Index < 0)
                return false;
            hdpParameter = new HdpParameter();

            object cellValue = row.Cells[this.colConfigGroup.Index].Value;
            if (GlobalMethods.Misc.IsEmptyString(cellValue))
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("��������������������!");
                return false;
            }
            hdpParameter.GROUP_NAME = cellValue.ToString();
           
            cellValue = row.Cells[this.colConfigName.Index].Value;
            if (GlobalMethods.Misc.IsEmptyString(cellValue))
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("��������������������!");
                return false;
            }
            hdpParameter.CONFIG_NAME = cellValue.ToString();

            if (row.Cells[this.colConfigValue.Index].Value != null)
                hdpParameter.CONFIG_VALUE = row.Cells[this.colConfigValue.Index].Value.ToString();
            if (row.Cells[this.colConfigDesc.Index].Value != null)
                hdpParameter.CONFIG_DESC = row.Cells[this.colConfigDesc.Index].Value.ToString();
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

            HdpParameter hdpParameter = row.Tag as HdpParameter;
            if (hdpParameter == null)
                return SystemData.ReturnValue.FAILED;
            string szGroupName = hdpParameter.GROUP_NAME;
            string szConfigName = hdpParameter.CONFIG_NAME;
            hdpParameter = null;
            if (!this.MakeRowData(row, ref hdpParameter))
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (!this.dataGridView1.IsNewRow(row))
                    shRet =HdpParameterAccess.Instance.DeleteHdpParameter(szGroupName, szConfigName);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼!");
                    return SystemData.ReturnValue.FAILED;
                }
                this.dataGridView1.Rows.Remove(row);
            }
            else if (this.dataGridView1.IsModifiedRow(row))
            {
                shRet =HdpParameterAccess.Instance.UpdateHdpParameter(szGroupName, szConfigName, hdpParameter);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼!");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpParameter;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            else if (this.dataGridView1.IsNewRow(row))
            {
                shRet =HdpParameterAccess.Instance.SaveHdpParameter(hdpParameter);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼!");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = hdpParameter;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ʾ��ѯ������öԻ���
        /// </summary>
        /// <param name="row">ָ����</param>
        private void ShowConfigValueEditForm(DataTableViewRow row)
        {
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;
            if (this.dataGridView1.EditingControl != null)
                this.dataGridView1.EndEdit();
            LargeTextEditForm frmConfigValueEdit = new LargeTextEditForm();
            frmConfigValueEdit.Text = "�༭��������";
            DataGridViewCell cell = row.Cells[this.colConfigValue.Index];
            if (cell.Value != null)
                frmConfigValueEdit.LargeText = cell.Value.ToString();
            if (frmConfigValueEdit.ShowDialog() != DialogResult.OK)
                return;
            string szConfigValue = frmConfigValueEdit.LargeText;
            if (szConfigValue.Equals(cell.Value))
                return;
            cell.Value = szConfigValue;
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);
        }

        /// <summary>
        /// ��ʾ���������ı��༭�Ի���
        /// </summary>
        /// <param name="row">ָ����</param>
        private void ShowConfigDescEditForm(DataTableViewRow row)
        {
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;
            if (this.dataGridView1.EditingControl != null)
                this.dataGridView1.EndEdit();
            LargeTextEditForm frmConfigDescEdit = new LargeTextEditForm();
            frmConfigDescEdit.Text = "�༭��������";
            DataGridViewCell cell = row.Cells[this.colConfigDesc.Index];
            if (cell.Value != null)
                frmConfigDescEdit.LargeText = cell.Value.ToString();
            if (frmConfigDescEdit.ShowDialog() != DialogResult.OK)
                return;
            string szConfigDesc = frmConfigDescEdit.LargeText.Trim();
            if (szConfigDesc.Equals(cell.Value))
                return;
            cell.Value = szConfigDesc;
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);
        }

        private void mnnCancelModify_Click(object sender, EventArgs e)
        {
            int nCount = this.dataGridView1.SelectedRows.Count;
            if (nCount <= 0)
                return;
            for (int index = 0; index < nCount; index++)
            {
                DataTableViewRow row = this.dataGridView1.SelectedRows[index];
                if (!this.dataGridView1.IsModifiedRow(row))
                    continue;
                this.SetRowData(row, row.Tag as HdpParameter);
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
        }

        private void mnuCancelDelete_Click(object sender, EventArgs e)
        {
            int nCount = this.dataGridView1.SelectedRows.Count;
            if (nCount <= 0)
                return;
            for (int index = 0; index < nCount; index++)
            {
                DataTableViewRow row = this.dataGridView1.SelectedRows[index];
                if (!this.dataGridView1.IsDeletedRow(row))
                    continue;
                if (this.dataGridView1.IsNewRow(row))
                    this.dataGridView1.SetRowState(row, RowState.New);
                else if (this.dataGridView1.IsModifiedRow(row))
                    this.dataGridView1.SetRowState(row, RowState.Update);
                else
                    this.dataGridView1.SetRowState(row, RowState.Normal);
            }
        }

        private void mnuDeleteRecord_Click(object sender, EventArgs e)
        {
            int nCount = this.dataGridView1.SelectedRows.Count;
            if (nCount <= 0)
                return;
            for (int index = 0; index < nCount; index++)
            {
                DataTableViewRow row = this.dataGridView1.SelectedRows[index];
                this.dataGridView1.SetRowState(row, RowState.Delete);
            }
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            this.CommitModify();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            DataTableViewRow row = this.dataGridView1.Rows[e.RowIndex];
            if (e.ColumnIndex == this.colConfigValue.Index)
                this.ShowConfigValueEditForm(row);
            else if (e.ColumnIndex == this.colConfigDesc.Index)
                this.ShowConfigDescEditForm(row);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void toolbtnNew_Click(object sender, EventArgs e)
        {
            this.AddNewItem();
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
        /// ����һ�м�¼
        /// </summary>
        private void AddNewItem()
        {
            
            //��������
            HdpParameter hdpParameter = new HdpParameter();
            //������
            int index = this.dataGridView1.Rows.Add();
            DataTableViewRow row = this.dataGridView1.Rows[index];
            row.Tag = hdpParameter;
            this.dataGridView1.SetRowState(row, RowState.New);
            this.UpdateUIState();

            this.dataGridView1.CurrentCell = row.Cells[this.colConfigGroup.Index];
            this.dataGridView1.BeginEdit(true);
        }

        private void mnuAddItem_Click(object sender, EventArgs e)
        {
            this.AddNewItem();
        }

        private void mnuModifyItem_Click(object sender, EventArgs e)
        {
            this.ModifySelectedItem();
        }

        private void toolbtnModify_Click(object sender, EventArgs e)
        {
            this.ModifySelectedItem();
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
                this.SetRowData(row, row.Tag as HdpParameter);
            }
            this.UpdateUIState();
            this.dataGridView1.CurrentCell = row.Cells[this.colConfigGroup.Index];
            this.dataGridView1.BeginEdit(true);
        }

        private void toolbtnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedItem();
        }

        private void mnuDeleteItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedItem();
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

        private void mnuSaveItems_Click(object sender, EventArgs e)
        {
            this.CommitModify();
        }
        private void toolbtnSave_Click(object sender, EventArgs e)
        {
            this.Focus();
            this.CommitModify();
        }

        private void toolcboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadParameterList();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.UpdateUIState();
        }
    }
}