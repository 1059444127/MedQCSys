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
using Heren.MedQC.CheckPoint;
using EMRDBLib.DbAccess;
using Heren.MedQC.Utilities;
using Heren.MedQC.Core;
using MedQCSys.DockForms;
using MedQCSys;
using Heren.MedQC.Maintenance.Dialogs;
using Heren.MedQC.ScriptEngine.Debugger;
using Heren.MedQC.ScriptEngine.Script;

namespace Heren.MedQC.Maintenance
{
    public partial class QcCheckPointForm : DockContentBase
    {
        public QcCheckPointForm(MainForm mainForm)
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
            this.LoadGridComboxItems();
            this.LoadGridViewData();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //this.dataGridView1.Font= new System.Drawing.Font("����", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }
        private void LoadGridComboxItems()
        {
            this.colHandlerCommand.PinyinFuzzyMatch = true;
            this.colHandlerCommand.Items.Clear();
            if (CommandHandler.Instance.Commands.Count > 0)
            {
                foreach (string item in CommandHandler.Instance.Commands.Keys)
                {
                    this.colHandlerCommand.Items.Add(item);
                }
            }
            this.colHandlerCommand.Items.Insert(0, "");
        }
        /// <summary>
        /// ��ȡ���ݿ�,װ�ص�ǰDataGridView��������
        /// </summary>
        private void LoadGridViewData()
        {
            this.dataGridView1.Rows.Clear();
            Dictionary<string, string> dicTimeEventName = new Dictionary<string, string>();
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


            List<QcCheckPoint> lstQcCheckPoints = null;
            result = EMRDBLib.DbAccess.QcCheckPointAccess.Instance.GetQcCheckPoints(ref lstQcCheckPoints);
            if (result != EMRDBLib.SystemData.ReturnValue.OK
                && result != EMRDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�Զ��˲�����б�����ʧ��!");
                return;
            }
            if (lstQcCheckPoints == null || lstQcCheckPoints.Count <= 0)
                return;
            for (int index = 0; index < lstQcCheckPoints.Count; index++)
            {
                QcCheckPoint qcCheckPoint = lstQcCheckPoints[index];
                if (qcCheckPoint == null)
                    continue;
                string szEventName = qcCheckPoint.EventID;
                if (!string.IsNullOrEmpty(szEventName))
                {
                    if (dicTimeEventName.ContainsKey(szEventName))
                        szEventName = dicTimeEventName[szEventName];
                }
                qcCheckPoint.EventName = szEventName;

                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                this.SetRowData(row, qcCheckPoint);
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            this.MainForm.ShowStatusMessage(string.Format("�˲����������{0}��", this.dataGridView1.Rows.Count));
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
            this.RefreshOrderValue();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            string szMessageText = null;
            if (result == EMRDBLib.SystemData.ReturnValue.FAILED)
                szMessageText = string.Format("������ֹ,�ѱ���{0}����¼!", count);
            else
                szMessageText = string.Format("����ɹ�,�ѱ���{0}����¼!", count);
            MessageBoxEx.Show(szMessageText, MessageBoxIcon.Information);
            return result == EMRDBLib.SystemData.ReturnValue.OK;
        }
        private void RefreshOrderValue()
        {
            foreach (DataTableViewRow item in this.dataGridView1.Rows)
            {
                QcCheckPoint qcCheckPoint = item.Tag as QcCheckPoint;
                qcCheckPoint.OrderValue = item.Index;
                QcCheckPointAccess.Instance.Update(qcCheckPoint);
            }
        }
        /// <summary>
        /// ����ָ������ʾ������,�Լ��󶨵�����
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="qcCheckPoint">�󶨵�����</param>
        private void SetRowData(DataTableViewRow row, QcCheckPoint qcCheckPoint)
        {
            if (row == null || row.Index < 0 || qcCheckPoint == null)
                return;
            row.Height = 27;
            row.Tag = qcCheckPoint;
            row.Cells[this.colQaEventType.Index].Value = qcCheckPoint.QaEventType;
            row.Cells[this.colCheckType.Index].Value = qcCheckPoint.CheckType;
            row.Cells[this.colCheckPointContent.Index].Value = qcCheckPoint.CheckPointContent;
            row.Cells[this.colCheckPointID.Index].Tag = qcCheckPoint.CheckPointID;
            row.Cells[this.colCheckPointID.Index].Value = qcCheckPoint.CheckPointID;
            row.Cells[this.colDocType.Index].Tag = qcCheckPoint.DocTypeID;
            row.Cells[this.colDocType.Index].Value = qcCheckPoint.DocTypeName;
            row.Cells[this.colWrittenPeriod.Index].Value = qcCheckPoint.WrittenPeriod;
            if (qcCheckPoint.Score > 0)
                row.Cells[this.colQCScore.Index].Value = qcCheckPoint.Score;
            row.Cells[this.colIsValid.Index].Value = qcCheckPoint.IsValid;
            row.Cells[this.colMsgDictMessage.Index].Value = qcCheckPoint.MsgDictMessage;
            row.Cells[this.colScriptName.Index].Value = qcCheckPoint.ScriptName;
            row.Cells[this.colHandlerCommand.Index].Value = qcCheckPoint.HandlerCommand;
            row.Cells[this.colEvent.Index].Value = qcCheckPoint.EventName;
            row.Cells[this.colEvent.Index].Tag = qcCheckPoint.EventID;
            row.Cells[this.colIsRepeat.Index].Value = qcCheckPoint.IsRepeat;
            row.Cells[this.colQaEventType.Index].Value = qcCheckPoint.QaEventType;
            row.Cells[this.col_ELEMENT_NAME.Index].Value = qcCheckPoint.ELEMENT_NAME;
            row.Cells[this.colScriptName.Index].Value = qcCheckPoint.ScriptName;
            row.Cells[this.colScriptName.Index].Tag = qcCheckPoint.ScriptID;



        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="qcCheckPoint">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref QcCheckPoint qcCheckPoint)
        {
            if (row == null || row.Index < 0)
                return false;
            qcCheckPoint = row.Tag as QcCheckPoint;
            if (qcCheckPoint == null)
                qcCheckPoint = new QcCheckPoint();
            object cellValue = row.Cells[this.colCheckPointID.Index].Value;
            if (cellValue != null)
                qcCheckPoint.CheckPointID = cellValue.ToString();

            if (this.dataGridView1.IsDeletedRow(row))
                return true;

            cellValue = row.Cells[this.colHandlerCommand.Index].Value;
            if (cellValue != null)
                qcCheckPoint.HandlerCommand = cellValue.ToString();

            cellValue = row.Cells[this.colDocType.Index].Tag;

            if (cellValue != null)
                qcCheckPoint.DocTypeID = cellValue.ToString();
            else
                qcCheckPoint.DocTypeID = string.Empty;
            cellValue = row.Cells[this.colDocType.Index].Value;

            if (cellValue != null)
                qcCheckPoint.DocTypeName = cellValue.ToString();
            else
                qcCheckPoint.DocTypeName = string.Empty;

            cellValue = row.Cells[this.colWrittenPeriod.Index].Value;
            if (cellValue != null && cellValue.ToString().Trim() != string.Empty)
            {
                qcCheckPoint.WrittenPeriod = cellValue.ToString();
            }
            cellValue = row.Cells[this.colIsValid.Index].Value;
            if (cellValue != null)
            {
                bool value = false;
                if (bool.TryParse(cellValue.ToString(), out value))
                    qcCheckPoint.IsValid = value;
            }
            cellValue = row.Cells[this.colIsRepeat.Index].Value;
            if (cellValue != null)
            {
                bool value = false;
                if (bool.TryParse(cellValue.ToString(), out value))
                    qcCheckPoint.IsRepeat = value;
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
                qcCheckPoint.Score = value;
            }
            cellValue = row.Cells[this.col_ELEMENT_NAME.Index].Value;
            if (cellValue != null && cellValue.ToString().Trim() != "")
            {
                qcCheckPoint.ELEMENT_NAME = cellValue.ToString();
            }

            qcCheckPoint.OrderValue = row.Index;

            cellValue = row.Cells[this.colCheckPointContent.Index].Value;
            if (cellValue != null)
                qcCheckPoint.CheckPointContent = cellValue.ToString();
            else
                qcCheckPoint.CheckPointContent = string.Empty;

            cellValue = row.Cells[this.colEvent.Index].Tag;
            if (cellValue == null || cellValue.ToString().Trim() == string.Empty)
            {
                cellValue = string.Empty;
            }
            qcCheckPoint.EventID = cellValue.ToString();

            cellValue = row.Cells[this.colCheckType.Index].Value;
            if (cellValue != null)
                qcCheckPoint.CheckType = cellValue.ToString();
            else
                qcCheckPoint.CheckType = string.Empty;
            cellValue = row.Cells[this.colScriptName.Index].Value;
            if (cellValue != null)
            {
                qcCheckPoint.ScriptID = row.Cells[this.colScriptName.Index].Tag as string;
                qcCheckPoint.ScriptName = row.Cells[this.colScriptName.Index].Value.ToString();
            }

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

            QcCheckPoint qcCheckPoint = row.Tag as QcCheckPoint;
            if (qcCheckPoint == null)
                return EMRDBLib.SystemData.ReturnValue.FAILED;

            string szCheckPointID = qcCheckPoint.CheckPointID;
            qcCheckPoint = null;
            if (!this.MakeRowData(row, ref qcCheckPoint))
                return EMRDBLib.SystemData.ReturnValue.FAILED;

            short result = EMRDBLib.SystemData.ReturnValue.OK;
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (!this.dataGridView1.IsNewRow(row))
                    result = EMRDBLib.DbAccess.QcCheckPointAccess.Instance.Delete(szCheckPointID);
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
                result = EMRDBLib.DbAccess.QcCheckPointAccess.Instance.Update(qcCheckPoint);
                if (result != EMRDBLib.SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼!");
                    return EMRDBLib.SystemData.ReturnValue.FAILED;
                }
                row.Tag = qcCheckPoint;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            else if (this.dataGridView1.IsNewRow(row))
            {
                result = EMRDBLib.DbAccess.QcCheckPointAccess.Instance.Insert(qcCheckPoint);
                if (result != EMRDBLib.SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼!");
                    return EMRDBLib.SystemData.ReturnValue.FAILED;
                }
                row.Tag = qcCheckPoint;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            return EMRDBLib.SystemData.ReturnValue.OK;
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
        private void ShowCheckPointContentEditForm(DataTableViewRow row)
        {
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;
            LargeTextEditForm frmRuleDescEdit = new LargeTextEditForm();
            frmRuleDescEdit.Text = "�༭�˲�����";
            DataGridViewCell cell = row.Cells[this.colCheckPointContent.Index];
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
            QcCheckPoint qcCheckPoint = null;
            DataTableViewRow currRow = this.dataGridView1.CurrentRow;
            if (currRow != null && currRow.Index >= 0)
                qcCheckPoint = currRow.Tag as QcCheckPoint;
            if (qcCheckPoint == null)
                qcCheckPoint = new QcCheckPoint();
            else
                qcCheckPoint = qcCheckPoint.Clone() as QcCheckPoint;
            qcCheckPoint.CheckPointID = qcCheckPoint.MakeCheckPointID();
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
            this.SetRowData(row, qcCheckPoint);

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
                this.SetRowData(row, row.Tag as QcCheckPoint);
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
            QcCheckPoint qcCheckPoint = row.Tag as QcCheckPoint;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (e.ColumnIndex == this.colEvent.Index)
                this.ShowTimeEventSelectForm();
            if (e.ColumnIndex == this.colDocType.Index)
                this.ShowDocTypeSelectForm(row);
            else if (e.ColumnIndex == this.colCheckPointContent.Index)
                this.ShowCheckPointContentEditForm(row);
            else if (e.ColumnIndex == this.colWrittenPeriod.Index)
                this.ShowTimeLineEditForm(row.Cells[this.colWrittenPeriod.Index]);
            else if (e.ColumnIndex == this.colMsgDictMessage.Index || e.ColumnIndex == this.colQaEventType.Index)
                this.ShowQuestionTypeForm(row);
            else if (e.ColumnIndex == this.colScriptName.Index)
            {
                if (SystemParam.Instance.PatVisitInfo == null)
                {
                    MessageBoxEx.ShowMessage("Ϊ����ɽű����ԣ����ȱ�����������Լ�ѡ����Ի���");
                    return;
                }
                this.ShowScriptEditForm(row);
            }
            else if (e.ColumnIndex == this.col_ELEMENT_NAME.Index)
            {
                SelectElementForm frm = new SelectElementForm();
                string doctypeids = row.Cells[this.colDocType.Index].Tag as string;
                if (string.IsNullOrEmpty(doctypeids))
                {
                    MessageBoxEx.ShowMessage("����ѡ���������");
                    return;
                }
                string[] arrDocTypeIDs = doctypeids.Split(';');
                frm.DocTypeInfo = new DocTypeInfo() { DocTypeID = arrDocTypeIDs[0] };
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    row.Cells[this.col_ELEMENT_NAME.Index].Value = frm.SelectedElement.ElementName;
                    qcCheckPoint.ELEMENT_NAME = frm.SelectedElement.ElementName;
                }
                else if (frm.SelectedElement == null)
                {
                    row.Cells[this.col_ELEMENT_NAME.Index].Value = string.Empty;
                    qcCheckPoint.ELEMENT_NAME = string.Empty;
                }
                if (this.dataGridView1.IsNormalRowUndeleted(row))
                    this.dataGridView1.SetRowState(row, RowState.Update);
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void ShowScriptEditForm(DataTableViewRow row)
        {
            string szScriptConfigID = row.Cells[this.colScriptName.Index].Tag as string;
            ScriptConfig qcCheckScript = null;
            if (!string.IsNullOrEmpty(szScriptConfigID))
                ScriptConfigAccess.Instance.GetScriptConfig(szScriptConfigID, ref qcCheckScript);
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;

            string szScriptID = string.Empty;
            string szScriptName = string.Empty;
            if (qcCheckScript != null)
                szScriptID = qcCheckScript.SCRIPT_ID;
            if (row.Cells[this.colScriptName.Index].Value != null)
                szScriptName = row.Cells[this.colScriptName.Index].Value.ToString();

            string szScriptText = string.Empty;
            if (!string.IsNullOrEmpty(szScriptID))
            {
                ScriptConfigAccess.Instance.GetScriptSource(szScriptID, ref szScriptText);
            }

            DebuggerForm scriptEditForm = new DebuggerForm();
            CheckPointHelper.Instance.InitPatientInfo(SystemParam.Instance.PatVisitInfo);
            scriptEditForm.PatVisitInfo = SystemParam.Instance.PatVisitInfo;
            
            scriptEditForm.QcCheckPoint = row.Tag as QcCheckPoint;
            scriptEditForm.QcCheckResult = CheckPointHelper.Instance.InitQcCheckResult(scriptEditForm.QcCheckPoint, SystemParam.Instance.PatVisitInfo);
            scriptEditForm.ScriptConfig = qcCheckScript;
            scriptEditForm.WorkingPath = GlobalMethods.Misc.GetWorkingPath();
            scriptEditForm.ScriptProperty = new ScriptProperty();
            scriptEditForm.ScriptProperty.ScriptName = szScriptName;
            scriptEditForm.ScriptProperty.ScriptText = szScriptText;
            scriptEditForm.ScriptProperty.FilePath = string.Format("{0}\\Script\\Caches\\{1}.vbs"
                , scriptEditForm.WorkingPath, szScriptID);
            scriptEditForm.MinimizeBox = false;
            scriptEditForm.ShowInTaskbar = false;
            if (scriptEditForm.ShowDialog() != DialogResult.OK)
                return;
            if (scriptEditForm.ScriptConfig != null)
            {
                row.Cells[this.colScriptName.Index].Tag = scriptEditForm.ScriptConfig.SCRIPT_ID;
                row.Cells[this.colScriptName.Index].Value = scriptEditForm.ScriptConfig.SCRIPT_NAME;
                 
            }
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);

        }

        private void ShowQuestionTypeForm(DataTableViewRow row)
        {
            if (row == null || row.Index < 0 || this.dataGridView1.IsDeletedRow(row))
                return;
            QuestionTypeForm frmQuestionType = new QuestionTypeForm();
            frmQuestionType.Text = "ѡ��ȱ�ݵ�ͷ���";
            DataGridViewCell cell = row.Cells[this.colCheckPointContent.Index];
            QcCheckPoint qcCheckPoint = row.Tag as QcCheckPoint;
            if (frmQuestionType.ShowDialog() != DialogResult.OK)
                return;
            qcCheckPoint.MsgDictCode = frmQuestionType.QcMsgCode;
            qcCheckPoint.MsgDictMessage = frmQuestionType.Message;
            qcCheckPoint.QaEventType = frmQuestionType.QaEventType;
            qcCheckPoint.Score = frmQuestionType.Score != null ? float.Parse(frmQuestionType.Score) : 0;
            row.Cells[this.colMsgDictMessage.Index].Value = qcCheckPoint.MsgDictMessage;
            row.Cells[this.colQaEventType.Index].Value = qcCheckPoint.QaEventType;
            row.Cells[this.colQCScore.Index].Value = qcCheckPoint.Score;
            row.Tag = qcCheckPoint;
            if (this.dataGridView1.IsNormalRowUndeleted(row))
                this.dataGridView1.SetRowState(row, RowState.Update);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //������༭����ID��
            if (e.ColumnIndex == this.colWrittenPeriod.Index)
                e.Cancel = true;
            else if (e.ColumnIndex == this.colCheckPointID.Index)
                e.Cancel = true;
            else if (e.ColumnIndex == this.colDocType.Index || e.ColumnIndex == this.colCheckPointContent.Index)
                e.Cancel = true;
            else if (e.ColumnIndex == this.colQaEventType.Index || e.ColumnIndex == this.colMsgDictMessage.Index)
                e.Cancel = true;
            else if (e.ColumnIndex == this.col_ELEMENT_NAME.Index)
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
            //if (e.ColumnIndex == this.colEvent.Index)
            //{
            //    TimeQCEvent timeQCEvent = editingControl.SelectedItem as TimeQCEvent;
            //    row.Cells[this.colEvent.Index].Value = timeQCEvent == null ? string.Empty : timeQCEvent.EventID;
            //    row.Cells[e.ColumnIndex].Tag = timeQCEvent;
            //}
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            if (!(column is DataGridViewButtonColumn))
                return;
            QcCheckPoint qcCheckPoint = this.dataGridView1.Rows[e.RowIndex].Tag as QcCheckPoint;
            if (string.IsNullOrEmpty(qcCheckPoint.HandlerCommand))
                return;
            Object result = null;
            PatVisitInfo patVisitLog = SystemParam.Instance.PatVisitInfo;
            if (SystemParam.Instance.PatVisitInfo == null)
            {
                MessageBoxEx.Show("���ڻ����б���ѡ����");
                return;
            }
            
            Heren.MedQC.CheckPoint.CheckPointHelper.Instance.InitPatientInfo(patVisitLog);
            CommandHandler.Instance.SendCommand(qcCheckPoint.HandlerCommand, qcCheckPoint, patVisitLog, out result);
            //������Ա�д����Ҫ��������ڰ�ť�¼��Ĳ���~
            QcCheckResult qcCheckResult = result as QcCheckResult;
            if (qcCheckResult == null)
            {
                return;
            }
            short shRet = QcCheckResultAccess.Instance.SaveQcCheckResult(qcCheckResult);
            if (qcCheckResult.QC_RESULT == 1)
            {
                MessageBoxEx.ShowMessage("����ͨ��");
                return;
            }
            MessageBox.Show(qcCheckResult.QC_EXPLAIN);

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
            StatExpExcelHelper.Instance.ExportToExcel(this.dataGridView1, "����ȱ���Զ��˲�����");
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
    }
}