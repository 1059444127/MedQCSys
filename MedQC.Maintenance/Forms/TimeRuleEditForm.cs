// ***********************************************************
// �����༭�����ù���ϵͳʱЧ�ʿع���༭����.
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;
using Heren.Common.Controls.TableView;
using EMRDBLib.Entity;
using MedQCSys.Dialogs;
using EMRDBLib;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Maintenance
{
    public partial class TimeRuleEditForm : DockContentBase
    {
        public TimeRuleEditForm(MainForm mainForm)
            : base(mainForm)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.btnMoveUp.Image = Properties.Resources.MoveUp;
            this.btnMoveDown.Image = Properties.Resources.MoveDown;
            this.OnRefreshView();
        }

        /// <summary>
        /// ˢ�µ�ǰ���ڵ�������ʾ
        /// </summary>
        public override void OnRefreshView()
        {
            base.OnRefreshView();

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (!this.CheckModifiedData())
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            this.Update();
            this.LoadGridViewData();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// ��ȡ���ݿ�,װ�ص�ǰDataGridView��������
        /// </summary>
        private void LoadGridViewData()
        {
            this.dataGridView1.Rows.Clear();

            //����ʱЧ�¼������ֵ��,�Ա�����ʹ��
            Dictionary<string, string> dicTimeEventName = null;
            dicTimeEventName = new Dictionary<string, string>();
            List<TimeQCEvent> lstTimeQCEvents = null;
            short result = EMRDBLib.DbAccess.TimeQCEventAccess.Instance.GetTimeQCEvents(ref lstTimeQCEvents);
            if (result != EMRDBLib.SystemData.ReturnValue.OK
                && result != EMRDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.ShowError("����ʱЧ�¼��б�����ʧ��!");
                return;
            }
            for (int index = 0; lstTimeQCEvents != null; index++)
            {
                if (index >= lstTimeQCEvents.Count)
                    break;
                if (lstTimeQCEvents[index] == null)
                    continue;
                string szEventID = lstTimeQCEvents[index].EventID;
                if (string.IsNullOrEmpty(szEventID))
                    continue;
                string szEventName = lstTimeQCEvents[index].EventName;
                if (string.IsNullOrEmpty(szEventName))
                    continue;
                dicTimeEventName.Add(szEventID, szEventName);
            }

            //����ʱЧ�¼��б�
            List<TimeQCRule> lstTimeQCRules = null;
            result = EMRDBLib.DbAccess.TimeQCRuleAccess.Instance.GetTimeQCRules(ref lstTimeQCRules);
            if (result != EMRDBLib.SystemData.ReturnValue.OK
                && result != EMRDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("����ʱЧ�����б�����ʧ��!");
                return;
            }
            if (lstTimeQCRules == null || lstTimeQCRules.Count <= 0)
                return;
            for (int index = 0; index < lstTimeQCRules.Count; index++)
            {
                TimeQCRule timeQCRule = lstTimeQCRules[index];
                if (timeQCRule == null)
                    continue;
                string szEventName = timeQCRule.EventID;
                if (!string.IsNullOrEmpty(szEventName))
                {
                    if (dicTimeEventName.ContainsKey(szEventName))
                        szEventName = dicTimeEventName[szEventName];
                }
                timeQCRule.EventName = szEventName;

                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                this.SetRowData(row, timeQCRule);
                this.dataGridView1.SetRowState(row, RowState.Normal);
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
            short result = EMRDBLib.SystemData.ReturnValue.OK;
            while (index < this.dataGridView1.Rows.Count)
            {
                DataTableViewRow row = this.dataGridView1.Rows[index];
                bool bIsDeletedRow = this.dataGridView1.IsDeletedRow(row);
                result = this.SaveRowData(row);
                if (result == EMRDBLib.SystemData.ReturnValue.OK)
                    count++;
                else if (result == EMRDBLib.SystemData.ReturnValue.FAILED)
                    break;
                if (!bIsDeletedRow) index++;
            }

            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            string szMessageText = null;
            if (result == EMRDBLib.SystemData.ReturnValue.FAILED)
                szMessageText = string.Format("������ֹ,�ѱ���{0}����¼!", count);
            else
                szMessageText = string.Format("����ɹ�,�ѱ���{0}����¼!", count);
            MessageBoxEx.Show(szMessageText, MessageBoxIcon.Information);
            return result == EMRDBLib.SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="timeQCRule">�󶨵�����</param>
        private void SetRowData(DataTableViewRow row, TimeQCRule timeQCRule)
        {
            if (row == null || row.Index < 0 || timeQCRule == null)
                return;
            row.Tag = timeQCRule;
            row.Cells[this.colRuleID.Index].Value = timeQCRule.RuleID;
            row.Cells[this.colEvent.Index].Tag = timeQCRule.EventID;
            row.Cells[this.colEvent.Index].Value = timeQCRule.EventName;
            row.Cells[this.colDocType.Index].Tag = timeQCRule.DocTypeID;
            row.Cells[this.colDocType.Index].Value = timeQCRule.DocTypeName;
            row.Cells[this.colDocTypeAlias.Index].Value = timeQCRule.DocTypeAlias;
            row.Cells[this.colWrittenPeriod.Index].Value = timeQCRule.WrittenPeriod;
            row.Cells[this.colIsRepeat.Index].Value = timeQCRule.IsRepeat;
            if (timeQCRule.QCScore > 0)
                row.Cells[this.colQCScore.Index].Value = timeQCRule.QCScore;
            row.Cells[this.colIsValid.Index].Value = timeQCRule.IsValid;
            row.Cells[this.colRuleDesc.Index].Value = timeQCRule.RuleDesc;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="timeQCRule">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref TimeQCRule timeQCRule)
        {
            if (row == null || row.Index < 0)
                return false;
            timeQCRule = new TimeQCRule();

            object cellValue = row.Cells[this.colRuleID.Index].Value;
            if (cellValue != null)
                timeQCRule.RuleID = cellValue.ToString();

            if (this.dataGridView1.IsDeletedRow(row))
                return true;

            cellValue = row.Cells[this.colEvent.Index].Tag;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("�����������ѡ��һ���¼�!");
                return false;
            }
            timeQCRule.EventID = cellValue.ToString();

            cellValue = row.Cells[this.colEvent.Index].Value;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("�����������ѡ��һ���¼�!");
                return false;
            }
            timeQCRule.EventName = cellValue.ToString();

            cellValue = row.Cells[this.colDocType.Index].Tag;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("������ѡ��������!");
                return false;
            }
            timeQCRule.DocTypeID = cellValue.ToString();

            cellValue = row.Cells[this.colDocType.Index].Value;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("������ѡ��������!");
                return false;
            }
            timeQCRule.DocTypeName = cellValue.ToString();

            cellValue = row.Cells[this.colDocTypeAlias.Index].Value;
            if (cellValue != null)
                timeQCRule.DocTypeAlias = cellValue.ToString();

            cellValue = row.Cells[this.colWrittenPeriod.Index].Value;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
            {
                this.dataGridView1.SelectRow(row);
                MessageBoxEx.Show("������������дʱ��!");
                return false;
            }
            timeQCRule.WrittenPeriod = cellValue.ToString();

            cellValue = row.Cells[this.colIsRepeat.Index].Value;
            if (cellValue != null)
            {
                bool value = false;
                if (bool.TryParse(cellValue.ToString(), out value))
                    timeQCRule.IsRepeat = value;
            }

            cellValue = row.Cells[this.colIsValid.Index].Value;
            if (cellValue != null)
            {
                bool value = false;
                if (bool.TryParse(cellValue.ToString(), out value))
                    timeQCRule.IsValid = value;
            }

            cellValue = row.Cells[this.colQCScore.Index].Value;
            if (cellValue != null && cellValue.ToString().Trim() != "")
            {
                float value = 0f;
                if (!float.TryParse(cellValue.ToString(), out value)
                    || value < 0 || value >= 100)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("����ȷ�����ʿؿ۷�!");
                    return false;
                }
                timeQCRule.QCScore = value;
            }

            timeQCRule.OrderValue = row.Index;

            cellValue = row.Cells[this.colRuleDesc.Index].Value;
            if (cellValue != null)
                timeQCRule.RuleDesc = cellValue.ToString();
            return true;
        }

        /// <summary>
        /// ����ָ���е����ݵ�Զ�����ݱ�,��Ҫע����ǣ��е�ɾ��״̬��������״̬����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <returns>SystemConsts.ReturnValue</returns>
        private short SaveRowData(DataTableViewRow row)
        {
            if (row == null || row.Index < 0)
                return EMRDBLib.SystemData.ReturnValue.FAILED;
            if (this.dataGridView1.IsNormalRow(row) || this.dataGridView1.IsUnknownRow(row))
            {
                if (!this.dataGridView1.IsDeletedRow(row))
                    return EMRDBLib.SystemData.ReturnValue.CANCEL;
            }

            TimeQCRule timeQCRule = row.Tag as TimeQCRule;
            if (timeQCRule == null)
                return EMRDBLib.SystemData.ReturnValue.FAILED;

            string szRuleID = timeQCRule.RuleID;
            timeQCRule = null;
            if (!this.MakeRowData(row, ref timeQCRule))
                return EMRDBLib.SystemData.ReturnValue.FAILED;

            short result = EMRDBLib.SystemData.ReturnValue.OK;
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (!this.dataGridView1.IsNewRow(row))
                    result = EMRDBLib.DbAccess.TimeQCRuleAccess.Instance.DeleteTimeQCRule(szRuleID);
                if (result != EMRDBLib.SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷�ɾ����ǰ��¼!");
                    return EMRDBLib.SystemData.ReturnValue.FAILED;
                }
                this.dataGridView1.Rows.Remove(row);
            }
            else if (this.dataGridView1.IsModifiedRow(row))
            {
                result = EMRDBLib.DbAccess.TimeQCRuleAccess.Instance.UpdateTimeQCRule(timeQCRule);
                if (result != EMRDBLib.SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼!");
                    return EMRDBLib.SystemData.ReturnValue.FAILED;
                }
                row.Tag = timeQCRule;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            else if (this.dataGridView1.IsNewRow(row))
            {
                result = EMRDBLib.DbAccess.TimeQCRuleAccess.Instance.SaveTimeQCRule(timeQCRule);
                if (result != EMRDBLib.SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼!");
                    return EMRDBLib.SystemData.ReturnValue.FAILED;
                }
                row.Tag = timeQCRule;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            //ˢ��orderValue
            EMRDBLib.DbAccess.TimeQCRuleAccess.Instance.UpdateTimeQCRule(timeQCRule);
            return EMRDBLib.SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ʾʱЧ�¼�ѡ��Ի���
        /// </summary>
        private void ShowTimeEventSelectForm()
        {
            TimeEventSelectForm frmTimeEventSelect = new TimeEventSelectForm();

            //����Ĭ��ѡ�е�ʱЧ�¼�
            DataGridViewCell currCell = this.dataGridView1.CurrentCell;
            if (currCell != null)
            {
                TimeQCEvent timeQCEvent = new TimeQCEvent();
                if (currCell.Tag != null)
                    timeQCEvent.EventID = currCell.Tag.ToString();
                if (currCell.Value != null)
                    timeQCEvent.EventName = currCell.Value.ToString();
                frmTimeEventSelect.SelectedEvent = timeQCEvent;
            }

            //����ѡ�������е�ʱЧ�¼�
            if (frmTimeEventSelect.ShowDialog() == DialogResult.OK)
            {
                TimeQCEvent timeQCEvent = frmTimeEventSelect.SelectedEvent;
                if (timeQCEvent == null)
                    timeQCEvent = new TimeQCEvent();
                for (int index = 0; index < this.dataGridView1.SelectedRows.Count; index++)
                {
                    DataTableViewRow row = this.dataGridView1.SelectedRows[index];
                    if (row == null || row.Index < 0)
                        continue;
                    row.Cells[this.colEvent.Index].Tag = timeQCEvent.EventID;
                    row.Cells[this.colEvent.Index].Value = timeQCEvent.EventName;
                    if (!this.dataGridView1.IsNormalRowUndeleted(row))
                        continue;
                    this.dataGridView1.SetRowState(row, RowState.Update);
                }
            }
        }

        /// <summary>
        /// ��ʾ�ĵ��������öԻ���
        /// </summary>
        /// <param name="row">ָ����</param>
        private void ShowDocTypeSelectForm(DataTableViewRow row)
        {
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;

            TempletSelectForm templetSelectForm = new TempletSelectForm();
            DataGridViewCell cell = row.Cells[this.colDocType.Index];
            if (cell.Tag != null)
                templetSelectForm.DefaultDocTypeID = cell.Tag.ToString();
            templetSelectForm.MultiSelect = true;
            templetSelectForm.Text = "ѡ��������";
            templetSelectForm.Description = "��ѡ��Ӧ��д�Ĳ������ͣ�";
            if (templetSelectForm.ShowDialog() != DialogResult.OK)
                return;

            List<DocTypeInfo> lstDocTypeInfos = templetSelectForm.SelectedDocTypes;
            if (lstDocTypeInfos == null || lstDocTypeInfos.Count <= 0)
            {
                row.Cells[this.colDocType.Index].Tag = null;
                row.Cells[this.colDocType.Index].Value = null;
                if (this.dataGridView1.IsNormalRowUndeleted(row))
                    this.dataGridView1.SetRowState(row, RowState.Update);
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
            row.Cells[this.colDocType.Index].Tag = sbDocTypeIDList.ToString();
            row.Cells[this.colDocType.Index].Value = sbDocTypeNameList.ToString();
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);
        }

        /// <summary>
        /// ��ʾʱ�����ô���Ի���
        /// </summary>
        /// <param name="cell">ָ����Ԫ��</param>
        private void ShowTimeLineEditForm(DataGridViewCell cell)
        {
            if (cell == null || cell.RowIndex < 0)
                return;

            TimeLineEditForm frmTimeLineEdit = new TimeLineEditForm();
            if (cell.Value != null)
                frmTimeLineEdit.TimeLine = cell.Value.ToString();
            if (frmTimeLineEdit.ShowDialog() != DialogResult.OK)
                return;
            cell.Value = frmTimeLineEdit.TimeLine;

            DataTableViewRow row = this.dataGridView1.Rows[cell.RowIndex];
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);
        }

        /// <summary>
        /// ��ʾ���������༭�Ի���
        /// </summary>
        /// <param name="row">ָ����</param>
        private void ShowRuleDescEditForm(DataTableViewRow row)
        {
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;
            LargeTextEditForm frmRuleDescEdit = new LargeTextEditForm();
            frmRuleDescEdit.Text = "�༭��������";
            DataGridViewCell cell = row.Cells[this.colRuleDesc.Index];
            if (cell.Value != null)
                frmRuleDescEdit.LargeText = cell.Value.ToString();
            if (frmRuleDescEdit.ShowDialog() != DialogResult.OK)
                return;
            string szRuleDesc = frmRuleDescEdit.LargeText.Trim();
            if (szRuleDesc.Equals(cell.Value))
                return;
            cell.Value = szRuleDesc;
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);
        }

        /// <summary>
        /// �����ƶ�ѡ����
        /// </summary>
        private void MoveSelectedRowUp()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataTableViewRow selectedRow = this.dataGridView1.SelectedRows[0];
            if (selectedRow.Index <= 0)
                return;
            DataTableViewRow targetRow = this.dataGridView1.Rows[selectedRow.Index - 1];
            this.dataGridView1.SwapRow(selectedRow, targetRow);
            if (!this.dataGridView1.IsNewRow(selectedRow))
                this.dataGridView1.SetRowState(selectedRow, RowState.Update);
            if (!this.dataGridView1.IsNewRow(targetRow))
                this.dataGridView1.SetRowState(targetRow, RowState.Update);
        }

        /// <summary>
        /// �����ƶ�ѡ����
        /// </summary>
        private void MoveSelectedRowDown()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataTableViewRow selectedRow = this.dataGridView1.SelectedRows[0];
            if (selectedRow.Index >= this.dataGridView1.Rows.Count - 1)
                return;
            DataTableViewRow targetRow = this.dataGridView1.Rows[selectedRow.Index + 1];
            this.dataGridView1.SwapRow(selectedRow, targetRow);
            if (!this.dataGridView1.IsNewRow(selectedRow))
                this.dataGridView1.SetRowState(selectedRow, RowState.Update);
            if (!this.dataGridView1.IsNewRow(targetRow))
                this.dataGridView1.SetRowState(targetRow, RowState.Update);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TimeQCRule timeQCRule = null;
            DataTableViewRow currRow = this.dataGridView1.CurrentRow;
            if (currRow != null && currRow.Index >= 0)
                timeQCRule = currRow.Tag as TimeQCRule;
            if (timeQCRule == null)
                timeQCRule = new TimeQCRule();
            else
                timeQCRule = timeQCRule.Clone() as TimeQCRule;
            timeQCRule.RuleID = timeQCRule.MakeRuleID();
            int nRowIndex = 0;
            if (currRow != null)
            {
                nRowIndex = currRow.Index + 1;
                DataTableViewRow dataTableViewRow = new DataTableViewRow();
                this.dataGridView1.Rows.Insert(nRowIndex, dataTableViewRow);
            }
            else
                nRowIndex = this.dataGridView1.Rows.Add();
            DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
            this.SetRowData(row, timeQCRule);
            this.dataGridView1.Focus();
            this.dataGridView1.SelectRow(row);
            this.dataGridView1.SetRowState(row, RowState.New);
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            this.CommitModify();
        }

        private void mnuMoveUp_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MoveSelectedRowUp();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuMoveDown_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MoveSelectedRowDown();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
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
                this.SetRowData(row, row.Tag as TimeQCRule);
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

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MoveSelectedRowUp();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MoveSelectedRowDown();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            DataTableViewRow row = this.dataGridView1.Rows[e.RowIndex];
            if (this.dataGridView1.IsDeletedRow(row))
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (e.ColumnIndex == this.colEvent.Index)
                this.ShowTimeEventSelectForm();
            else if (e.ColumnIndex == this.colDocType.Index)
                this.ShowDocTypeSelectForm(row);
            else if (e.ColumnIndex == this.colRuleDesc.Index)
                this.ShowRuleDescEditForm(row);
            else if (e.ColumnIndex == this.colWrittenPeriod.Index)
                this.ShowTimeLineEditForm(row.Cells[this.colWrittenPeriod.Index]);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //������༭����ID��
            if (e.ColumnIndex == this.colWrittenPeriod.Index)
                e.Cancel = true;
            else if (e.ColumnIndex == this.colRuleID.Index || e.ColumnIndex == this.colEvent.Index)
                e.Cancel = true;
            else if (e.ColumnIndex == this.colDocType.Index || e.ColumnIndex == this.colRuleDesc.Index)
                e.Cancel = true;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.EditingControl == null)
                return;
            ComboBoxEditingControl editingControl = this.dataGridView1.EditingControl as ComboBoxEditingControl;
            if (editingControl == null)
                return;
            DataTableViewRow row = this.dataGridView1.Rows[e.RowIndex];
            //���û��������б���ѡ�еķ����¼�ID����Cell��Tag�д洢����,�Ա㱣��ʱ������¼���IDд�����ݱ�
            if (e.ColumnIndex == this.colEvent.Index)
            {
                TimeQCEvent timeQCEvent = editingControl.SelectedItem as TimeQCEvent;
                row.Cells[this.colEvent.Index].Value = timeQCEvent == null ? string.Empty : timeQCEvent.EventID;
                row.Cells[e.ColumnIndex].Tag = timeQCEvent;
            }
        }
    }
}