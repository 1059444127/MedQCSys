// ***********************************************************
// �����ʿ�ϵͳ�ʿ���������ά������.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.Controls.TableView;
using Heren.Common.DockSuite;

using EMRDBLib.DbAccess;
using EMRDBLib;
using MedQCSys;
using MedQCSys.DockForms;

namespace Heren.MedQC.Maintenance
{
    public partial class QCEventTypesForm : DockContentBase
    {
        public QCEventTypesForm(MainForm parent)
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

        public override void OnRefreshView()
        {
            if (!this.SaveUncommitedChange())
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ˢ��������������ֵ䣬���Ժ�...");

            this.LoadQCEventTypeList();
            this.UpdateUIState();

            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// װ�ز����������������Ϣ
        /// </summary>
        private void LoadQCEventTypeList()
        {
            this.dataGridView1.Rows.Clear();
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
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            for (int index = 0; index < lstQCEventTypes.Count; index++)
            {
                //������������
                if (!string.IsNullOrEmpty(lstQCEventTypes[index].PARENT_CODE))
                    continue;

                EMRDBLib.QaEventTypeDict qcEventType = lstQCEventTypes[index];
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = qcEventType; //����¼��Ϣ���浽����
                this.SetRowData(row, qcEventType);
                this.dataGridView1.SetRowState(row, RowState.Normal);

                //����������������
                if (!ht.Contains(qcEventType.INPUT_CODE))
                {
                    for (int index2 = 0; index2 < lstQCEventTypes.Count; index2++)
                    {
                        EMRDBLib.QaEventTypeDict secQcEventType = lstQCEventTypes[index2];
                        if (secQcEventType.PARENT_CODE != qcEventType.INPUT_CODE)
                            continue;
                        int nRowIndex2 = this.dataGridView1.Rows.Add();
                        DataTableViewRow secRow = this.dataGridView1.Rows[nRowIndex2];
                        secRow.Tag = qcEventType; //����¼��Ϣ���浽����
                        this.SetRowData(secRow, secQcEventType);
                        this.dataGridView1.SetRowState(secRow, RowState.Normal);
                    }
                    ht.Add(qcEventType.INPUT_CODE, qcEventType.QA_EVENT_TYPE);
                }
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
        /// <param name="qcEventType">�󶨵�����</param>
        /// <returns>bool</returns>
        private bool SetRowData(DataGridViewRow row, EMRDBLib.QaEventTypeDict qcEventType)
        {
            if (row == null || row.Index < 0 || qcEventType == null)
                return false;
            row.Tag = qcEventType;
            row.Cells[this.colSerialNO.Index].Value = qcEventType.SERIAL_NO;
            row.Cells[this.colTypeDesc.Index].Value = qcEventType.QA_EVENT_TYPE;
            row.Cells[this.colInputCode.Index].Value = qcEventType.INPUT_CODE;
            row.Cells[this.colMaxScore.Index].Value = qcEventType.MAX_SCORE;
            if (!string.IsNullOrEmpty(qcEventType.PARENT_CODE))
                row.Cells[this.colSerialNO.Index].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            return true;
        }

        /// <summary>
        /// ��ȡָ���������޸ĺ������
        /// </summary>
        /// <param name="row">ָ����</param>
        /// <param name="qcEventType">�����޸ĺ������</param>
        /// <returns>bool</returns>
        private bool MakeRowData(DataTableViewRow row, ref EMRDBLib.QaEventTypeDict qcEventType)
        {
            if (row == null || row.Index < 0)
                return false;
            qcEventType = new EMRDBLib.QaEventTypeDict();
            EMRDBLib.QaEventTypeDict oldQCEventType = row.Tag as EMRDBLib.QaEventTypeDict;
            if (!this.dataGridView1.IsNewRow(row))
            {
                if (oldQCEventType == null)
                {
                    MessageBoxEx.Show("������������ֵ���������ϢΪ�գ�");
                    return false;
                }
            }

            if (this.dataGridView1.IsDeletedRow(row))
            {
                qcEventType = oldQCEventType;
                return true;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colSerialNO.Index].Value.ToString()))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colSerialNO.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("������������ţ�");
                return false;
            }
            if (GlobalMethods.Misc.IsEmptyString((string)row.Cells[this.colTypeDesc.Index].Value))
            {
                this.dataGridView1.CurrentCell = row.Cells[this.colTypeDesc.Index];
                this.dataGridView1.BeginEdit(true);
                MessageBoxEx.Show("�����������������ͣ�");
                return false;
            }
            if (qcEventType == null)
                qcEventType = new EMRDBLib.QaEventTypeDict();
            qcEventType.SERIAL_NO =int.Parse(row.Cells[this.colSerialNO.Index].Value.ToString());
            qcEventType.QA_EVENT_TYPE = (string)row.Cells[this.colTypeDesc.Index].Value;
            qcEventType.INPUT_CODE = (string)row.Cells[this.colInputCode.Index].Value;
            qcEventType.MAX_SCORE = Convert.ToDouble(row.Cells[this.colMaxScore.Index].Value);
            qcEventType.PARENT_CODE = oldQCEventType.PARENT_CODE;
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

            EMRDBLib.QaEventTypeDict qcEventType = row.Tag as EMRDBLib.QaEventTypeDict;
            if (qcEventType == null)
                return SystemData.ReturnValue.FAILED;
            string szInputCode = qcEventType.INPUT_CODE;

            qcEventType = null;
            if (!this.MakeRowData(row, ref qcEventType))
                return SystemData.ReturnValue.FAILED;

            short shRet = SystemData.ReturnValue.OK;
            if (this.dataGridView1.IsDeletedRow(row))
            {
                if (!this.dataGridView1.IsNewRow(row))
                    shRet = QaEventTypeDictAccess.Instance.Delete(szInputCode);
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
                shRet = QaEventTypeDictAccess.Instance.Update(qcEventType, szInputCode);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����µ�ǰ��¼������������ظ���");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = qcEventType;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            else if (this.dataGridView1.IsNewRow(row))
            {
                shRet = QaEventTypeDictAccess.Instance.Insert(qcEventType);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    this.dataGridView1.SelectRow(row);
                    MessageBoxEx.Show("�޷����浱ǰ��¼������������ظ���");
                    return SystemData.ReturnValue.FAILED;
                }
                row.Tag = qcEventType;
                this.dataGridView1.SetRowState(row, RowState.Normal);
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ�м�¼
        /// </summary>
        private void AddNewItem()
        {
            if (SystemParam.Instance.UserRight == null)
                return;

            
            EMRDBLib.QaEventTypeDict qcEventInfo = null;
            DataTableViewRow currRow = this.dataGridView1.CurrentRow;
            if (currRow != null && currRow.Index >= 0)
                qcEventInfo = currRow.Tag as EMRDBLib.QaEventTypeDict;
            if (qcEventInfo == null)
                qcEventInfo = new QaEventTypeDict();
            else
                qcEventInfo = qcEventInfo.Clone() as QaEventTypeDict;

            int nRowIndex = this.dataGridView1.Rows.Add();
            DataTableViewRow row = this.dataGridView1.Rows[nRowIndex];
            this.SetRowData(row, qcEventInfo);
            this.dataGridView1.Focus();
            this.dataGridView1.SelectRow(row);
            this.dataGridView1.SetRowState(row, RowState.New);
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
                this.SetRowData(row, row.Tag as EMRDBLib.QaEventTypeDict);
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
            if (SystemParam.Instance.UserRight == null)
                return;

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
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewCell currCell = this.dataGridView1.CurrentCell;
            if (currCell == null || currCell.ColumnIndex == this.colTypeDesc.Index)
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
            if (currCell == null || currCell.ColumnIndex == this.colTypeDesc.Index)
                return;

            if (currCell.ColumnIndex == this.colSerialNO.Index)
            {
                if (e.KeyChar == (char)((int)Keys.Back))
                    return;
                if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
                    return;
                e.Handled = true;
            }
            else if (currCell.ColumnIndex == this.colInputCode.Index)
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
            else if (currCell.ColumnIndex == this.colMaxScore.Index)
            {
                if (e.KeyChar == (char)((int)Keys.Back))
                    return;
                if (e.KeyChar.CompareTo('.') == 0)
                    return;
                if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
                    return;
                e.Handled = true;
            }
        }

        private void toolBtnSecNew_Click(object sender, EventArgs e)
        {
            
            this.AddSecNewItem();
        }
        /// <summary>
        /// ������������������
        /// </summary>
        private void AddSecNewItem()
        {
            if (SystemParam.Instance.UserRight == null)
                return;

           
            //�жϵ�ǰѡ�����Ƿ���һ����������
            DataTableViewRow selectedRow = this.dataGridView1.SelectedRows[0];
            if (selectedRow == null)
            {
                MessageBoxEx.Show("�����������ǰ��ѡ��������࣡", MessageBoxIcon.Warning);
                return;
            }
            EMRDBLib.QaEventTypeDict selectedQCEventType = selectedRow.Tag as EMRDBLib.QaEventTypeDict;
            if (selectedQCEventType == null || !string.IsNullOrEmpty(selectedQCEventType.PARENT_CODE))
            {
                MessageBoxEx.Show("���������²����������������࣡", MessageBoxIcon.Warning);
                return;
            }
            //��������
            EMRDBLib.QaEventTypeDict qcEventInfo = new EMRDBLib.QaEventTypeDict();
            qcEventInfo.PARENT_CODE = selectedQCEventType.INPUT_CODE;
            //������
            int index = selectedRow.Index + 1;
            DataTableViewRow row = new DataTableViewRow();
            this.dataGridView1.Rows.Insert(index, row);
            row = dataGridView1.Rows[index];
            row.Tag = qcEventInfo;

            row.Cells[this.colSerialNO.Index].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView1.SetRowState(row, RowState.New);
            this.UpdateUIState();

            this.dataGridView1.CurrentCell = row.Cells[this.colSerialNO.Index];
            this.dataGridView1.BeginEdit(true);
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
                if(WorkProcess.Instance.Canceled)
                {
                    WorkProcess.Instance.Close();
                    break;
                }
                WorkProcess.Instance.Show(string.Format("�����{0}��...",lstQcMsgDict.IndexOf(item)),lstQcMsgDict.IndexOf(item), true);
                QaEventTypeDict qaEventTypeDict = null;
                if (string.IsNullOrEmpty(item.MESSAGE_TITLE))
                {
                    qaEventTypeDict = lstQaEventTypeDict.Where(m => m.QA_EVENT_TYPE == item.QA_EVENT_TYPE && m.PARENT_CODE == null).FirstOrDefault();
                }
                else
                {
                    qaEventTypeDict = lstQaEventTypeDict.Where(m => m.QA_EVENT_TYPE == item.MESSAGE_TITLE && m.PARENT_CODE != null).FirstOrDefault();
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