// ***********************************************************
// �����ʿ�ϵͳ�ĵ�ʱЧ�Զ����������.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using MedDocSys.QCEngine.TimeCheck;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace Heren.MedQC.TimeCheckGener
{
    public partial class DocumentTimeForm : Form
    {
        public DocumentTimeForm()
        {
            this.InitializeComponent();

            this.dataGridView1.Font = new Font("����", 10.5f);
        }

        /// <summary>
        /// ������Ϣ�ı䷽����д
        /// </summary>
        //protected override void OnPatientInfoChanged()
        //{
        //    this.dataGridView1.Rows.Clear();
        //    if (this.IsHidden)
        //        return;
        //    if (this.Pane == null || this.Pane.IsDisposed)
        //        return;
        //    if (this.Pane.ActiveContent == this)
        //        this.OnRefreshView();
        //}

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

        }





        private void ShowStatusMessage(string szMessage)
        {
          
        }

        /// <summary>
        ///  ��ֹʱ��Ƚ���
        /// </summary>
        /// <param name="result1"></param>
        /// <param name="result2"></param>
        /// <returns></returns>
        public int Compare(TimeCheckResult result1, TimeCheckResult result2)
        {
            if (result1 == null)
            {
                if (result2 == null)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (result2 == null)
            {
                return 1;
            }
            else
            {
                if (result1.EndTime.CompareTo(DateTime.Now) > 0)
                {
                    if (result2.EndTime.CompareTo(DateTime.Now) > 0)
                    {
                        return result1.EndTime.CompareTo(result2.EndTime);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (result2.EndTime.CompareTo(DateTime.Now) > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


        /// <summary>
        /// ���ش�ӡģ��
        /// </summary>
        private byte[] GetReportFileData(string szReportName)
        {
            if (GlobalMethods.Misc.IsEmptyString(szReportName))
                szReportName = string.Format("{0}\\Templet\\{1}.hrdt", GlobalMethods.Misc.GetWorkingPath(), "����ʱЧ����嵥");
            if (!System.IO.File.Exists(szReportName))
            {
                MessageBoxEx.ShowError("����ʱЧ����嵥����û������!");
                return null;
            }

            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szReportName, ref byteTempletData))
            {
                MessageBoxEx.ShowError("����ʱЧ����嵥������������ʧ��!");
                return null;
            }
            return byteTempletData;
        }



        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column != this.colCreateTime && e.Column != this.colEndTime)
            {
                return;
            }
            if (e.CellValue1 == null || e.CellValue2 == null)
                return;
            e.Handled = true;
            DateTime dtDateValue1 = DateTime.Now;
            DateTime dtDateValue2 = DateTime.Now;
            DateTime.TryParse(e.CellValue1.ToString(), out dtDateValue1);
            DateTime.TryParse(e.CellValue2.ToString(), out dtDateValue2);
            e.SortResult = DateTime.Compare(dtDateValue1, dtDateValue2);
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (e.RowIndex < 0 || e.RowIndex >= this.dataGridView1.RowCount)
                return;
            if (e.ColumnIndex < 0 || e.ColumnIndex >= this.dataGridView1.ColumnCount)
                return;
            this.dataGridView1.Rows[e.RowIndex].Selected = true;
            Point ptMousePos = this.dataGridView1.PointToClient(Control.MousePosition);
            this.cmenuDocTime.Show(this.dataGridView1, ptMousePos);
        }

        private void mnuCopyCheckBasis_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count <= 0)
                return;
            DataGridViewRow row = this.dataGridView1.SelectedRows[0];
            try
            {
                if (row.Cells[this.colCheckBasis.Index].Value == null)
                    Clipboard.Clear();
                else
                    Clipboard.SetDataObject(row.Cells[this.colCheckBasis.Index].Value.ToString());
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("DocumentTimeForm.mnuCopyCheckBasis_Click", ex);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
           
        }




        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.colPatientName.Visible = true;
            this.dataGridView1.Rows.Clear();
            string szPatientID = txtPatientID.Text.Trim();
            string szVisitID = txtVisitID.Text.Trim();
            if (string.IsNullOrEmpty(szPatientID) || string.IsNullOrEmpty(szVisitID))
            {
                MessageBoxEx.Show("�����벡���ź;����");
                return;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            PatVisitInfo patVisitLog = new PatVisitInfo();
            patVisitLog.PATIENT_ID = szPatientID;
            patVisitLog.VISIT_ID = szVisitID;
            short shRet = QcTimeRecordAccess.Instance.GetPatVisitLog( szPatientID, szVisitID, ref patVisitLog);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("������ѯʧ��");
                return;
            }
            if (string.IsNullOrEmpty(patVisitLog.INCHARGE_DOCTOR))
            {
                string doctorInCharge = string.Empty;
                shRet =EmrDocAccess.Instance.GetDoctorInChargeByEmrDoc(szPatientID, szVisitID, ref doctorInCharge);
                if (shRet == SystemData.ReturnValue.OK)
                    patVisitLog.INCHARGE_DOCTOR = doctorInCharge;
            }
            List<TimeCheckResult> lstCheckResults = new List<TimeCheckResult>();
            TimeCheckQuery timeCheckQuery = new TimeCheckQuery();

            timeCheckQuery.PatientID = patVisitLog.PATIENT_ID;
            timeCheckQuery.PatientName = patVisitLog.PATIENT_NAME;
            //timeCheckQuery.VisitID = patVisitLog.VISIT_ID;
            //���ݵ��Ӳ�������VISIT_ID=VISIT_NO
            timeCheckQuery.VisitID = patVisitLog.VISIT_NO;
            timeCheckQuery.VisitNO = patVisitLog.VISIT_NO;
            TimeCheckEngine.Instance.PerformTimeCheck(timeCheckQuery);
            lstCheckResults.AddRange(TimeCheckEngine.Instance.TimeCheckResults);
            lstCheckResults.Sort(new Comparison<TimeCheckResult>(this.Compare));
            if (lstCheckResults == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage(null);
                return;
            }
            DateTime now = DateTime.Now;
            for (int index = 0; index < lstCheckResults.Count; index++)
            {
                TimeCheckResult resultInfo = lstCheckResults[index];
                resultInfo.VisitID = patVisitLog.VISIT_ID;
                int nRowIndex = this.dataGridView1.Rows.Add();
                DataGridViewRow row = this.dataGridView1.Rows[nRowIndex];
                row.Tag = resultInfo;
                row.Cells[this.colPatientName.Index].Value = resultInfo.PatientName;
                row.Cells[this.colDocName.Index].Value = resultInfo.DocTitle;
                row.Cells[this.colDoctorInCharge.Index].Value = patVisitLog.INCHARGE_DOCTOR;
                if (resultInfo.WrittenState == WrittenState.Timeout)
                {
                    row.Cells[this.colStatus.Index].Value = "��д��ʱ";
                }
                else if (resultInfo.WrittenState == WrittenState.Unwrite)
                {
                    row.Cells[this.colStatus.Index].Value = "δ��д";
                }
                else if (resultInfo.WrittenState == WrittenState.Early)
                {
                    row.Cells[this.colStatus.Index].Value = "��д��ǰ";
                }
                else if (resultInfo.WrittenState == WrittenState.Normal)
                {
                    row.Cells[this.colStatus.Index].Value = "����";
                }
                if (resultInfo.WrittenState != WrittenState.Unwrite)
                {
                    row.Cells[this.colCreateTime.Index].Value = resultInfo.DocTime.ToString("yyyy-M-d HH:mm");
                    row.Cells[this.colCreator.Index].Value = resultInfo.CreatorName;
                }
                row.Cells[this.colEndTime.Index].Value = resultInfo.EndTime.ToString("yyyy-M-d HH:mm");
                if (now.CompareTo(resultInfo.EndTime) < 0)
                {
                    row.Cells[this.colLeave.Index].Value = Math.Round((resultInfo.EndTime - now).TotalHours, 0, MidpointRounding.ToEven);
                }
                else
                {
                    row.Cells[this.colLeave.Index].Value = "�ѳ�ʱ";
                }
                if (!resultInfo.IsRepeat)
                {
                    row.Cells[this.colCheckBasis.Index].Value = string.Format("����{0}{1},{2}����д{3}"
                        , resultInfo.EventTime.ToString("yyyy-M-d HH:mm")
                        , resultInfo.EventName, resultInfo.WrittenPeriod, resultInfo.DocTypeName);
                }
                else
                {
                    row.Cells[this.colCheckBasis.Index].Value = string.Format("����{0}{1},ÿ{2}��дһ��{3}"
                        , resultInfo.EventTime.ToString("yyyy-M-d HH:mm")
                        , resultInfo.EventName, resultInfo.WrittenPeriod, resultInfo.DocTypeName);
                }
                row.Cells[this.colDocTypeID.Index].Value = resultInfo.DocTypeID;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            this.ShowStatusMessage(null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBoxEx.ShowMessage("��¼Ϊ��");
                return;
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            PatVisitInfo patVisitLog = new PatVisitInfo();
            patVisitLog.PATIENT_ID = this.txtPatientID.Text;
            patVisitLog.VISIT_ID = this.txtVisitID.Text;
            short shRet = QcTimeRecordAccess.Instance.GetPatVisitLog(this.txtPatientID.Text, this.txtVisitID.Text, ref patVisitLog);
            DateTime now = DateTime.Now;
            List<QcTimeRecord> lstExitQCTimeRecord = new List<QcTimeRecord>();
            QcTimeRecordAccess.Instance.GetQcTimeRecords(this.txtPatientID.Text,this.txtVisitID.Text, ref lstExitQCTimeRecord);
            List<QcTimeRecord> lstQcTimeRecord = new List<QcTimeRecord>();
            foreach (DataGridViewRow item in this.dataGridView1.Rows)
            {
                TimeCheckResult resultInfo = item.Tag as TimeCheckResult;
                if (resultInfo == null)
                    continue;
                QcTimeRecord qcTimeRecord = new QcTimeRecord();
                
                qcTimeRecord.BeginDate = resultInfo.StartTime;
                qcTimeRecord.CheckDate = now;
                qcTimeRecord.CheckName = "ϵͳ�Զ�";
                qcTimeRecord.CreateID = resultInfo.CreatorID;
                qcTimeRecord.CreateName = resultInfo.CreatorName;
                qcTimeRecord.DeptInCharge = patVisitLog.DEPT_CODE; 
                qcTimeRecord.DeptStayed = patVisitLog.DEPT_NAME;
                qcTimeRecord.DocID = resultInfo.DocID;
                qcTimeRecord.DocTitle = resultInfo.DocTitle;
                qcTimeRecord.DoctorInCharge = patVisitLog.INCHARGE_DOCTOR;
                qcTimeRecord.DocTypeID = resultInfo.DocTypeID;
                qcTimeRecord.DocTypeName = resultInfo.DocTypeName;
                qcTimeRecord.EndDate = resultInfo.EndTime;
                qcTimeRecord.EventID = resultInfo.EventID;
                qcTimeRecord.EventName = resultInfo.EventName;
                qcTimeRecord.EventTime = resultInfo.EventTime;
                qcTimeRecord.PatientID = patVisitLog.PATIENT_ID;
                qcTimeRecord.PatientName = patVisitLog.PATIENT_NAME;
                qcTimeRecord.Point = resultInfo.QCScore;
                qcTimeRecord.DischargeTime = patVisitLog.DISCHARGE_TIME;
                qcTimeRecord.QcExplain = resultInfo.ResultDesc;
                qcTimeRecord.IsVeto = resultInfo.IsVeto;
                if (resultInfo.WrittenState == WrittenState.Early)
                    qcTimeRecord.QcResult = SystemData.WrittenState.Early;
                if (resultInfo.WrittenState == WrittenState.Normal)
                    qcTimeRecord.QcResult = SystemData.WrittenState.Normal;
                if (resultInfo.WrittenState == WrittenState.Timeout)
                    qcTimeRecord.QcResult = SystemData.WrittenState.Timeout;
                if (resultInfo.WrittenState == WrittenState.Unwrite)
                {
                    //���ݺ������󣬽�δ��д������ֳ�����״̬��һ����δ��д����һ��������δ��д����һ�ִ����Ѿ���ʱ�ˣ���Ҫ���п۷֡�
                    if (resultInfo.EndTime < now)
                    {
                        //����δ��д������ֹʱ�䣬״̬Ϊδ��д��ʱ
                        qcTimeRecord.QcResult = SystemData.WrittenState.Unwrite;
                    }
                    else
                    {
                        qcTimeRecord.QcResult = SystemData.WrittenState.UnwriteNormal;
                    }
                }
                // qcTimeRecord.QcResult = SystemData.WrittenState.Unwrite;
                qcTimeRecord.RecNo = "0"; //δ������
                qcTimeRecord.RecordTime = resultInfo.RecordTime;
                qcTimeRecord.DocTime = resultInfo.DocTime;
                qcTimeRecord.VisitID = resultInfo.VisitID;
                if (!resultInfo.IsRepeat)
                {
                    qcTimeRecord.QcExplain = string.Format("����{0}{1},{2}����д{3}"
                        , resultInfo.EventTime.ToString("yyyy-M-d HH:mm")
                        , resultInfo.EventName, resultInfo.WrittenPeriod, resultInfo.DocTypeName);
                }
                else
                {
                    qcTimeRecord.QcExplain = string.Format("����{0}{1},ÿ{2}��дһ��{3}"
                        , resultInfo.EventTime.ToString("yyyy-M-d HH:mm")
                        , resultInfo.EventName, resultInfo.WrittenPeriod, resultInfo.DocTypeName);
                }

                //�ж��Ƿ��Ѷ���֪ͨ����������ʱЧ��¼�Ѵ��ڵĻ�д�����ͳ�Ʊ����ж���֪ͨ������������ͬ���Ѵ����ݽ����Ϊ����֪ͨ��ͨ�����ֶα���д��ͳ�Ƶ����˵ı�Ӷ�����ظ����ŷ��͵����⡣
                if (lstExitQCTimeRecord != null && lstExitQCTimeRecord.Count > 0)
                {
                    foreach (QcTimeRecord exitTimeRecord in lstExitQCTimeRecord)
                    {
                        //QCTimeRecord����ΪPatientId,VisitID,DocTypeID,BeginDate,EndDate,QcResult,EventTime
                        if (exitTimeRecord.PatientID == qcTimeRecord.PatientID
                            && exitTimeRecord.VisitID == qcTimeRecord.VisitID
                            && exitTimeRecord.DocTypeID == qcTimeRecord.DocTypeID
                            && exitTimeRecord.BeginDate == qcTimeRecord.BeginDate
                            && exitTimeRecord.EndDate == qcTimeRecord.EndDate
                            && exitTimeRecord.QcResult == qcTimeRecord.QcResult
                            && exitTimeRecord.EventTime == qcTimeRecord.EventTime)
                        {
                            //�Ѵ��ڵļ�¼Ϊ����ģ������֪ͨ�Ծ�Ϊδ֪ͨ,��Ҫ�ǲ����л��ڵ���������������
                            //ʵ�������ÿ��ֻ������һ��
                            if (qcTimeRecord.CheckDate.ToShortDateString() !=
                                exitTimeRecord.CheckDate.ToShortDateString())
                                qcTimeRecord.MessageNotify = true;
                        }
                    }
                }
                lstQcTimeRecord.Add(qcTimeRecord);
            }
            shRet = QcTimeRecordAccess.Instance.SavePatientQcTimeRecord(patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID, lstQcTimeRecord);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog(string.Format("����{0} ʱЧ�ʿؼ�¼����ʧ��", patVisitLog.PATIENT_NAME));
            }
            else
            {
                MessageBoxEx.ShowMessage("����ɹ�");
                LogManager.Instance.WriteLog(string.Format("����{0} ʱЧ�ʿؼ�¼���ɳɹ�", patVisitLog.PATIENT_NAME));
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            this.ShowStatusMessage(null);
        }
    }
}