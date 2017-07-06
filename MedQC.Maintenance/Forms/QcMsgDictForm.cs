// ***********************************************************
// �����ʿ�ϵͳ�ʿ�����ģ��ά������.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using Heren.Common.Controls;
using Heren.Common.Controls.TableView;

using EMRDBLib.DbAccess;
using EMRDBLib;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Maintenance
{
    public partial class QcMsgDictForm : DockContentBase
    {
        public QcMsgDictForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
            this.dataGridView1.Font = new Font("����", 10.5f);
            //this.dataGridView1.AutoReadonly = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();
            this.OnRefreshView();
        }
        private List<QcCheckPoint> m_lstQcCheckPoint = null;
        /// <summary>
        /// ˢ�²����������������Ϣ�б�
        /// </summary>
        public override void OnRefreshView()
        {
            if (!this.SaveUncommitedChange())
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ˢ���ʿ��ʼ������ֵ䣬���Ժ�...");

            this.LoadQcMsgDictList();
            this.UpdateUIState();

            //this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// װ�ط����ʿ���Ϣ�ֵ�
        /// </summary>
        private void LoadQcMsgDictList()
        {
            this.dataGridView1.Rows.Clear();
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            List<QcMsgDict> lstQcMsgDicts = null;
            short shRet = QcMsgDictAccess.Instance.GetAllQcMsgDictList(ref lstQcMsgDicts);
            if (shRet == SystemData.ReturnValue.RES_NO_FOUND)
            {
                this.MainForm.ShowStatusMessage("δ�ҵ���¼");
                return;
            }
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��ȡ�����ʿ���Ϣ�ֵ�ʧ�ܣ�");
                return;
            }
            if (lstQcMsgDicts == null || lstQcMsgDicts.Count <= 0)
                return;
            this.RefreshQCEventTypeColumn();
            if (m_lstQcCheckPoint == null)
                m_lstQcCheckPoint = new List<QcCheckPoint>();
            shRet= QcCheckPointAccess.Instance.GetQcCheckPoints(ref m_lstQcCheckPoint);

            for (int index = 0; index < lstQcMsgDicts.Count; index++)
            {
                EMRDBLib.QcMsgDict qcMsgDict = lstQcMsgDicts[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = qcMsgDict;
                this.SetRowData(row, qcMsgDict);
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            this.MainForm.ShowStatusMessage(string.Format("��{0}����¼", lstQcMsgDicts.Count));
        }

        /// <summary>
        /// ˢ�������б������
        /// </summary>
        private void RefreshQCEventTypeColumn()
        {
            this.colQCEventType.Items.Clear();
            List<EMRDBLib.QaEventTypeDict> lstQCEventTypes = null;
            short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQCEventTypes);
            if (shRet == SystemData.ReturnValue.RES_NO_FOUND)
            {
                this.MainForm.ShowStatusMessage("δ�ҵ���¼");
                return;
            }
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                return;
            }
            if (lstQCEventTypes == null || lstQCEventTypes.Count <= 0)
                return;
            for (int index = 0; index < lstQCEventTypes.Count; index++)
            {
                if (!string.IsNullOrEmpty(lstQCEventTypes[index].PARENT_CODE))
                    continue;
                EMRDBLib.QaEventTypeDict qcEventType = lstQCEventTypes[index];
                this.colQCEventType.Items.Add(qcEventType.QA_EVENT_TYPE);
            }
            this.colQCEventType.DisplayStyle = ComboBoxStyle.DropDownList;
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
                if (this.dataGridView1.IsNormalRow(currRow) || this.dataGridView1.IsModifiedRow(currRow))
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
        public override bool HasUncommit()
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
        public override bool CommitModify()
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
        /// <param name="qcMsgDict">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataGridViewRow row, EMRDBLib.QcMsgDict qcMsgDict)
        {
            if (row == null || row.Index < 0 || qcMsgDict == null)
                return false;
            row.Tag = qcMsgDict;
            row.Cells[this.colSerialNO.Index].Value = qcMsgDict.SERIAL_NO;
            row.Cells[this.colQCEventType.Index].Value = qcMsgDict.QA_EVENT_TYPE;
            row.Cells[this.colQCMsgCode.Index].Value = qcMsgDict.QC_MSG_CODE;
            row.Cells[this.colMessage.Index].Value = qcMsgDict.MESSAGE;
            row.Cells[this.colMessageTitle.Index].Value = qcMsgDict.MESSAGE_TITLE;
            row.Cells[this.colScore.Index].Value =
                Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcMsgDict.SCORE, 0f)), 1).ToString("F1");
            row.Cells[this.col_IS_VALID.Index].Value = qcMsgDict.IS_VALID == 1 ? true : false;
            row.Cells[this.colIsVeto.Index].Value = qcMsgDict.ISVETO ? "��" : "��";
            if (m_lstQcCheckPoint == null)
                m_lstQcCheckPoint = new List<QcCheckPoint>();
            if (m_lstQcCheckPoint.Exists(m => m.MsgDictCode == qcMsgDict.QC_MSG_CODE))
            {
                row.Cells[this.colAuto.Index].Value = Properties.Resources.auto;
                row.Cells[this.colAuto.Index].ToolTipText = "��֧���Զ��ʿ�";
            }
            else
                row.Cells[this.colAuto.Index].Value = null;
            return true;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="qcMsgDict">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref EMRDBLib.QcMsgDict qcMsgDict)
        {
            if (row == null || row.Index < 0)
                return false;
            qcMsgDict = new EMRDBLib.QcMsgDict();
            EMRDBLib.QcMsgDict oldQCMessageTemplet = row.Tag as EMRDBLib.QcMsgDict;
            if (!this.dataGridView1.IsNewRow(row))
            {
                if (oldQCMessageTemplet == null)
                {
                    MessageBoxEx.Show("�ʿ��ʼ������ֵ���������ϢΪ�գ�");
                    return false;
                }
            }

            if (this.dataGridView1.IsDeletedRow(row))
            {
                qcMsgDict = oldQCMessageTemplet;
                return true;
            }
            object cellValue = row.Cells[this.colSerialNO.Index].Value;
            if (cellValue == null || GlobalMethods.Misc.IsEmptyString(cellValue.ToString()))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colSerialNO.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("������������ţ�");
                return false;
            }

            cellValue = row.Cells[this.colQCEventType.Index].Value;
            if (cellValue == null || GlobalMethods.Misc.IsEmptyString(cellValue.ToString()))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colQCEventType.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("�����������������ͣ�");
                return false;
            }

            cellValue = row.Cells[this.colQCMsgCode.Index].Value;
            if (cellValue == null || GlobalMethods.Misc.IsEmptyString(cellValue.ToString()))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colQCEventType.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("����������������룡");
                return false;
            }
            cellValue = row.Cells[this.colScore.Index].Value;

            if (cellValue == null)
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colScore.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("���������÷�����");
                return false;
            }
            string szRegexString = "^[0-9]+(.[0-9]{0,2})?$";
            Match m = Regex.Match((string)cellValue, szRegexString);
            if (!m.Success)
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colScore.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("������ķ�������ȷ��");
                return false;
            }

            if (qcMsgDict == null)
                qcMsgDict = new EMRDBLib.QcMsgDict();
            qcMsgDict.SERIAL_NO = int.Parse(row.Cells[this.colSerialNO.Index].Value.ToString());
            qcMsgDict.QA_EVENT_TYPE = (string)row.Cells[this.colQCEventType.Index].Value;
            qcMsgDict.QC_MSG_CODE = (string)row.Cells[this.colQCMsgCode.Index].Value;
            qcMsgDict.MESSAGE = (string)row.Cells[this.colMessage.Index].Value;
            qcMsgDict.MESSAGE_TITLE = row.Cells[this.colMessageTitle.Index].Value == null ?
                string.Empty : (string)row.Cells[this.colMessageTitle.Index].Value;
            qcMsgDict.SCORE = float.Parse(row.Cells[this.colScore.Index].Value.ToString());
            qcMsgDict.ISVETO = (string)row.Cells[this.colIsVeto.Index].Value == "��";
            qcMsgDict.APPLY_ENV = "MEDDOC";
            if (row.Cells[this.col_IS_VALID.Index].Value == null
                || row.Cells[this.col_IS_VALID.Index].Value.ToString().ToLower() == "false")
                qcMsgDict.IS_VALID = 0;
            else
                qcMsgDict.IS_VALID = 1;
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

            EMRDBLib.QcMsgDict qcMsgDict = row.Tag as EMRDBLib.QcMsgDict;
            if (qcMsgDict == null)
                return SystemData.ReturnValue.FAILED;
            string szQCMsgCode = qcMsgDict.QC_MSG_CODE;

            qcMsgDict = null;
            if (!this.MakeRowData(row, ref qcMsgDict))
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (!this.dataGridView1.IsNewRow(row))
                    shRet = QcMsgDictAccess.Instance.Delete(szQCMsgCode);
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
                shRet = QcMsgDictAccess.Instance.Update(qcMsgDict, szQCMsgCode);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = qcMsgDict;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            else if (this.dataGridView1.IsNewRow(row))
            {
                shRet = QcMsgDictAccess.Instance.Insert(qcMsgDict);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼��");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = qcMsgDict;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ�м�¼
        /// </summary>
        private void AddRow()
        {
            if (SystemParam.Instance.UserRight == null)
                return;
            EMRDBLib.QcMsgDict qcMessageTemplet = null;
            DataTableViewRow currRow = this.dataGridView1.CurrentRow;
            if (currRow != null && currRow.Index >= 0)
                qcMessageTemplet = currRow.Tag as EMRDBLib.QcMsgDict;
            if (qcMessageTemplet == null)
                qcMessageTemplet = new QcMsgDict();
            else
                qcMessageTemplet = qcMessageTemplet.Clone() as QcMsgDict;

            int nRowIndex = this.dataGridView1.Rows.Add();
            DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
            this.SetRowData(row, qcMessageTemplet);
            this.dataGridView1.Focus();
            this.dataGridView1.SelectRow(row);
            this.dataGridView1.SetRowState(row, RowState.New);
            this.dataGridView1.CurrentCell = row.Cells[this.colSerialNO.Index];
            this.dataGridView1.BeginEdit(true);
        }

        /// <summary>
        ///�޸�ѡ��������
        /// </summary>
        private void ModifySelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            if (SystemParam.Instance.UserRight == null)
                return;

           

            DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            if (this.dataGridView1.IsNormalRow(row))
            {
                this.dataGridView1.SetRowState(row, RowState.Update);
            }
            else if (this.dataGridView1.IsModifiedRow(row))
            {
                this.dataGridView1.SetRowState(row, RowState.Normal);
                this.SetRowData(row, row.Tag as EMRDBLib.QcMsgDict);
            }
            this.UpdateUIState();
            this.dataGridView1.CurrentCell = row.Cells[this.colSerialNO.Index];
            this.dataGridView1.BeginEdit(true);
        }

        /// <summary>
        /// ɾ��ѡ���м�¼
        /// </summary>
        private void DeleteSelectedItem()
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;

            if (SystemParam.Instance.UserRight == null)
                return;
            foreach (DataTableViewRow row in this.dataGridView1.SelectedRows)
            {
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
            //DataTableViewRow row = this.dataGridView1.SelectedRows[0];
            
        }

        private void toolbtnNew_Click(object sender, EventArgs e)
        {
            this.AddRow();
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
            if (SystemParam.Instance.UserRight == null)
                return;

           
            this.Focus();
            this.CommitModify();
        }

        private void mnuAddItem_Click(object sender, EventArgs e)
        {
            this.AddRow();
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
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            if (e.ColumnIndex == this.colMessageTitle.Index && !row.ReadOnly)
            {
                if (row.Cells[this.colQCEventType.Index].Value == null)
                {
                    MessageBoxEx.Show("ѡ����������ǰ����ѡ��������࣡", MessageBoxIcon.Warning);
                    return;
                }
                RefreshMessageTitleColumn(row);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
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

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (e.ColumnIndex == this.colQCEventType.Index)
            {
                this.ShowStatusMessage("����ˢ��������������ֵ䣬���Ժ�...");
                this.RefreshQCEventTypeColumn();
            }

            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        //ˢ����������Ӧ�����������ֵ�
        private void RefreshMessageTitleColumn(DataGridViewRow row)
        {
            string szQCEventType = row.Cells[this.colQCEventType.Index].Value.ToString();
            this.colMessageTitle.Items.Clear();
            List<EMRDBLib.QaEventTypeDict> lstQCEventTypes = null;
            short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQCEventTypes);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                return;
            }
            if (lstQCEventTypes == null || lstQCEventTypes.Count <= 0)
                return;
            EMRDBLib.QaEventTypeDict selectedQCEventType = lstQCEventTypes.Find(delegate (EMRDBLib.QaEventTypeDict item) { return item.QA_EVENT_TYPE == szQCEventType; });
            if (selectedQCEventType == null || !string.IsNullOrEmpty(selectedQCEventType.PARENT_CODE))
                return;
            for (int index = 0; index < lstQCEventTypes.Count; index++)
            {
                if (selectedQCEventType.INPUT_CODE != lstQCEventTypes[index].PARENT_CODE)
                    continue;
                EMRDBLib.QaEventTypeDict qcEventType = lstQCEventTypes[index];
                this.colMessageTitle.Items.Add(qcEventType.QA_EVENT_TYPE);
            }
            if (this.colMessageTitle.Items.Count == 0)
            {
                this.colMessageTitle.Items.Add(string.Empty);
            }
            this.colMessageTitle.DisplayStyle = ComboBoxStyle.DropDownList;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell currCell = this.dataGridView1.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colQCEventType.Index
                 || currCell.ColumnIndex == this.colMessage.Index)
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
            DataGridViewCell currCell = this.dataGridView1.CurrentCell;
            if (currCell == null)
                return;

            if (currCell.ColumnIndex == this.colQCMsgCode.Index)
            {
                if (e.KeyChar == (char)((int)Keys.Back))
                    return;
                if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
                    return;
                if (e.KeyChar.CompareTo('a') >= 0 && e.KeyChar.CompareTo('z') <= 0)
                {
                    char chRet = e.KeyChar;
                    char.TryParse(e.KeyChar.ToString().ToUpper(), out chRet);
                    e.KeyChar = chRet;
                    return;
                }
                if (e.KeyChar.CompareTo('A') >= 0 && e.KeyChar.CompareTo('Z') <= 0)
                    return;
                e.Handled = true;
            }
            else if (currCell.ColumnIndex == this.colSerialNO.Index || currCell.ColumnIndex == this.colScore.Index)
            {
                if (e.KeyChar == (char)((int)Keys.Back))
                    return;
                if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
                    return;
                if (currCell.ColumnIndex == this.colScore.Index && e.KeyChar.CompareTo('.') == 0)
                    return;
                e.Handled = true;
            }
        }

        private void tsBtnAutoSerialNo_Click(object sender, EventArgs e)
        {
            //��Ÿ��������������
            //�ֵ���������Ȱ�������ţ��ڰ�����
            List<QaEventTypeDict> lstQaEventTypeDict = null;
            short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQaEventTypeDict);
            List<QcMsgDict> lstQcMsgDict = null;
            shRet = QcMsgDictAccess.Instance.GetAllQcMsgDictList(ref lstQcMsgDict);
            WorkProcess.Instance.Initialize(this, lstQcMsgDict.Count, "���ڰ������������������ֵ����");
            foreach (QcMsgDict item in lstQcMsgDict)
            {
                if (WorkProcess.Instance.Canceled)
                {
                    WorkProcess.Instance.Close();
                    break;
                }
                WorkProcess.Instance.Show(string.Format("�����{0}��...", lstQcMsgDict.IndexOf(item)), lstQcMsgDict.IndexOf(item), true);
                QaEventTypeDict qaEventTypeDict = null;
                if (string.IsNullOrEmpty(item.MESSAGE_TITLE))
                {
                    qaEventTypeDict = lstQaEventTypeDict.Where(m => m.QA_EVENT_TYPE == item.QA_EVENT_TYPE && string.IsNullOrEmpty(m.PARENT_CODE) ).FirstOrDefault();
                }
                else
                {
                    qaEventTypeDict = lstQaEventTypeDict.Where(m => m.QA_EVENT_TYPE == item.MESSAGE_TITLE && !string.IsNullOrEmpty(m.PARENT_CODE)).FirstOrDefault();
                }
                if (qaEventTypeDict == null)
                    continue;
                item.SERIAL_NO = qaEventTypeDict.SERIAL_NO;
                shRet = QcMsgDictAccess.Instance.Update(item, item.QC_MSG_CODE);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.ShowError("����ʧ��");
                    WorkProcess.Instance.Close();
                    return;
                }
            }
            WorkProcess.Instance.Close();
            this.OnRefreshView();
        }
    }
}