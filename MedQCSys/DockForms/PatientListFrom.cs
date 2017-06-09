// ***********************************************************
// �����ʿ�ϵͳ�����б���ʾ����.
// Creator:YangMingkun  Date:2009-11-7
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;
using Heren.Common.Controls;
using Heren.Common.Controls.VirtualTreeView;
using MedDocSys.QCEngine.TimeCheck;

using MedQCSys.Dialogs;
using MedQCSys.Utility;
using MedQCSys.Controls.PatInfoList;
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Core;
using System.Linq;

namespace MedQCSys.DockForms
{
    public partial class PatientListForm : DockContentBase
    {
        //���ڼ�¼�û����һ��ѡ��Ļ��߾�����Ϣ
        private EMRDBLib.PatVisitInfo m_lastSelectedPatVisitLog = null;

        //���ڶԻ����б������ί��
        private Comparison<EMRDBLib.PatVisitInfo> m_comparison = null;

        /// <summary>
        /// ��ż������Ļ��߾�����Ϣ
        /// </summary>
        private List<EMRDBLib.PatVisitInfo> m_lstPatVisitLog = null;
        /// <summary>
        /// ������Ϣ
        /// </summary>
        private Hashtable m_htDeptInfo = null;

        //�߳�״̬����¼�����
        public delegate void UpdateThreadState(string szThrdName, List<EMRDBLib.PatVisitInfo> lstPatVisitLog, EMRDBLib.SearchThreadState threadState);

        public PatientListForm(MainForm parent)
            : base(parent)
        {
            this.InitializeComponent();
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.ShowHint = DockState.DockLeft;
            this.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;
            // Control.CheckForIllegalCrossThreadCalls = false;
        }


        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //this.patInfoList.SuspendLayout();ע�ʹ˲����߼�����࣬patInfoList��ʾ����ȷ
            this.patSearchPane1.ComboBoxSelectItemChanged += new EventHandler(PatSearchPane1_ComboBoxSelectItemChanged);
            this.patSearchPane1.PatListForm = this;
            //this.patInfoList.PerformLayout();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.patSearchPane1.SetHeight();
        }

        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.Invalidate(true);
            this.Update();
            this.patSearchPane1.InitPatSearchType();
        }
        public void ClearPatList()
        {
            if (this.patInfoList.SelectedCard != null)
                this.patInfoList.SelectedCard = null;
            this.patInfoList.ClearPatInfo();
        }

        /// <summary>
        /// װ��ָ���Ļ��߾�����Ϣ�б�
        /// </summary>
        /// <param name="lstPatVisitLogs">���߾�����Ϣ�б�</param>
        /// <param name="bClearDate">�Ƿ����ԭ������</param>
        public void LoadPatientVisitList(List<EMRDBLib.PatVisitInfo> lstPatVisitLogs, bool bClearDate)
        {
            if (bClearDate)
                ClearPatList();
            if (lstPatVisitLogs == null || lstPatVisitLogs.Count <= 0)
            {
                this.ShowStatusMessage("δ�ҵ���ػ����б�");
                return;
            }
            //���ϵͳ�и����ݴ����е�����

            if (this.patSearchPane1.SearchType != EMRDBLib.PatSearchType.Department)
            {
                if (this.m_comparison == null)
                    this.m_comparison = new Comparison<EMRDBLib.PatVisitInfo>(this.ComparePatVisitLog);
                lstPatVisitLogs.Sort(this.m_comparison);
            }

            this.ShowStatusMessage("���ڼ��ػ����б����Ժ�...");
            PatInfoCard lastSelectedPatInfoCard = null;
            this.patInfoList.SuspendLayout();
            //���²���
            this.UpdatePatientsCondition(lstPatVisitLogs);
            //��ȡ����ҽ����Ϣ
            int currentIndex = 0;
            for (int index = lstPatVisitLogs.Count - 1; index >= 0; index--)
            {
                EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLogs[index];
                this.SetPatVisitLogDeptName(patVisitLog);
                this.ShowStatusMessage(string.Format("���ڼ��ز����б�{0}/{1}�����Ժ�...", currentIndex++, lstPatVisitLogs.Count));
                //��������ӵ��б�
                PatInfoCard patInfoCard = this.patInfoList.AddPatInfo(patVisitLog);
                //��ӿ������Ʊ��
                this.AddDeptNameTag(lstPatVisitLogs, index);

                //�����ϣ��
                if (!SystemParam.Instance.PatVisitLogTable.ContainsKey(patVisitLog.PATIENT_ID))
                    SystemParam.Instance.PatVisitLogTable.Add(patVisitLog.PATIENT_ID, patInfoCard);

                //����ȱʡѡ�еĻ���
                if (lastSelectedPatInfoCard != null)
                    continue;
                if (this.m_lastSelectedPatVisitLog == null)
                    continue;
                if (patVisitLog.PATIENT_ID == this.m_lastSelectedPatVisitLog.PATIENT_ID)
                    lastSelectedPatInfoCard = patInfoCard;
                //ֻ��һ�� ��ֱ��ѡ��
                if (lstPatVisitLogs.Count == 1)
                    lastSelectedPatInfoCard = patInfoCard;

            }
            this.patInfoList.ResumeLayout(true);
            this.patInfoList.Update();
            this.ShowStatusMessage(null);

            if (lastSelectedPatInfoCard != null && !lastSelectedPatInfoCard.IsDisposed)
                this.patInfoList.SelectedCard = lastSelectedPatInfoCard;

            if (bClearDate)
                this.MainForm.ShowStatusMessage(string.Format("��������{0}������", lstPatVisitLogs.Count));
            else
                this.MainForm.ShowStatusMessage(null);
            BindDetialInfo(lstPatVisitLogs);
        }

        private void BindDetialInfo(List<PatVisitInfo> lstPatVisitLogs)
        {
            try
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                if (lstPatVisitLogs == null || lstPatVisitLogs.Count == 0)
                    return;
                if (backgroundWorker.IsBusy)
                    return;
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += M_BackgroundWorker_DoWork;
                backgroundWorker.RunWorkerCompleted += M_BackgroundWorker_RunWorkerCompleted;
                backgroundWorker.RunWorkerAsync(lstPatVisitLogs);

            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("��̨ˢ�����ݳ�������", ex);
            }
        }

        private void M_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<PatVisitInfo> lstPatVisitLogs = e.Result as List<PatVisitInfo>;
            if (lstPatVisitLogs == null)
            {
                this.ShowStatusMessage(null);
                return;
            }
            this.patInfoList.SuspendLayout();
            foreach (var item in this.patInfoList.Controls)
            {
                if (!(item is PatInfoCard))
                    continue;
                PatInfoCard card = (PatInfoCard)item;
                PatVisitInfo pat = lstPatVisitLogs.Find(
                           i => i.PATIENT_ID == card.PatVisitLog.PATIENT_ID && i.VISIT_ID == card.PatVisitLog.VISIT_ID);
                if (pat != null)
                    card.PatVisitLog = pat;
            }
            this.patInfoList.ResumeLayout(false);
            this.patInfoList.Update();

            this.ShowStatusMessage(string.Format("��������{0}������", lstPatVisitLogs.Count));
        }

        private void M_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<PatVisitInfo> lstPatVisitLogs = e.Argument as List<PatVisitInfo>;
            if (lstPatVisitLogs == null)
                return;

            //���˲���
            this.UpdatePatientsCondition(lstPatVisitLogs);
            //��ҳ��ʽ�ֲ��� ����400��sqlƴ�ӻᱨ��,ÿ��ִ�����300������
            for (int i = 0; i < lstPatVisitLogs.Count() / 100 + 1; i++)
            {
                var result = lstPatVisitLogs.Skip(i * 100).Take(100).ToList();
                //��ѯ�ʿؽ��״̬
                MedQCAccess.Instance.GetQCResultStatus(ref result);
                //��ѯ�����÷�
                MedQCAccess.Instance.GetPatQCScore(ref result);
                //��ѯ�Ƿ�����
                MedQCAccess.Instance.GetPatOperation(ref result);
                //��ѯ�����Ƿ���Ѫ
                MedQCAccess.Instance.GetPatBloodTransfusion(ref result);
            }
            e.Result = lstPatVisitLogs;
        }


        /// <summary>
        /// ��ȡ��������ҽ����Ϣ
        /// </summary>
        /// <param name="lstPatVisitLogs"></param>
        private void GetPatDoctorInfos(ref List<EMRDBLib.PatVisitInfo> lstPatVisitLogs)
        {
            if (lstPatVisitLogs == null || lstPatVisitLogs.Count == 0)
                return;
            List<EMRDBLib.PatDoctorInfo> lstPatDoctorInfos = new List<EMRDBLib.PatDoctorInfo>();
            Hashtable hashtable = new Hashtable();
            //��ȡ��Ҫ��ѯ��ʷ����ҽ����Ϣ�Ļ���
            for (int index = 0; index < lstPatVisitLogs.Count; index++)
            {
                EMRDBLib.PatVisitInfo item = lstPatVisitLogs[index];
                if (!hashtable.ContainsKey(item.PATIENT_ID + item.VISIT_ID))
                {
                    EMRDBLib.PatDoctorInfo patDoctorInfo = new EMRDBLib.PatDoctorInfo();
                    patDoctorInfo.PatientID = item.PATIENT_ID;
                    patDoctorInfo.VisitID = item.VISIT_ID;
                    hashtable.Add(patDoctorInfo.PatientID + patDoctorInfo.VisitID, patDoctorInfo);
                    lstPatDoctorInfos.Add(patDoctorInfo);
                }
            }
            //��ȡ����ҽ����Ϣ
            short shRet = PatVisitAccess.Instance.GetPatSanjiDoctors(ref lstPatDoctorInfos);
            if (shRet != SystemData.ReturnValue.OK)
                return;
            if (lstPatVisitLogs == null)
                return;
            foreach (EMRDBLib.PatVisitInfo item in lstPatVisitLogs)
            {
                EMRDBLib.PatDoctorInfo patDoctorInfo = lstPatDoctorInfos.Find(delegate (EMRDBLib.PatDoctorInfo p)
                {
                    if (p.PatientID == item.PATIENT_ID && p.VisitID == item.VISIT_ID)
                        return true;
                    else
                        return false;
                });
                if (patDoctorInfo != null)
                {
                    //item.InchargeDoctor = patDoctorInfo.RequestDoctorName;//����ҽ���Ѿ�ͨ����ͼ��ȡ�ˣ�����Ͳ����ٸ�ֵ��
                    item.AttendingDoctor = patDoctorInfo.ParentDoctorName;
                    item.SUPER_DOCTOR = patDoctorInfo.SuperDoctorName;
                }
            }
        }

        /// <summary>
        /// ��ӿ������Ʊ�ǵ������б���
        /// </summary>
        /// <param name="szDeptName">��������</param>
        private void AddDeptNameTag(List<EMRDBLib.PatVisitInfo> lstPatVisitLogs, int index)
        {
            if (lstPatVisitLogs == null || lstPatVisitLogs.Count <= 0)
                return;
            if (index < 0 || index >= lstPatVisitLogs.Count)
                return;

            //ע�⣺�����������ػ����б�ʱ��ѭ��˳�����й�ϵ��

            EMRDBLib.PatVisitInfo prevPatVisitLog = null;
            if (index > 0)
                prevPatVisitLog = lstPatVisitLogs[index - 1];
            EMRDBLib.PatVisitInfo currPatVisitLog = lstPatVisitLogs[index];

            if (index == 0 || currPatVisitLog.DEPT_CODE != prevPatVisitLog.DEPT_CODE)
            {
                Label lblDeptName = new Label();
                lblDeptName.BackColor = Color.LightSteelBlue;
                lblDeptName.ForeColor = Color.Black;
                lblDeptName.Height = 24;
                lblDeptName.Width = this.patInfoList.Width;
                lblDeptName.Dock = DockStyle.Top;
                lblDeptName.Text = currPatVisitLog.DEPT_NAME;
                lblDeptName.TextAlign = ContentAlignment.MiddleLeft;
                lblDeptName.Parent = this.patInfoList;
                lblDeptName.Font = new Font("����", 10.5f, FontStyle.Regular);
            }
        }


        /// <summary>
        /// �Բ�ѯ�����Ļ����б������������
        /// </summary>
        /// <param name="patVisitLog1">PatVisitLog����1</param>
        /// <param name="patVisitLog2">PatVisitLog����2</param>
        /// <returns>������</returns>
        private int ComparePatVisitLog(EMRDBLib.PatVisitInfo patVisitLog1, EMRDBLib.PatVisitInfo patVisitLog2)
        {
            if (patVisitLog1 == null && patVisitLog2 != null)
                return 1;
            if (patVisitLog1 != null && patVisitLog2 == null)
                return -1;
            if (patVisitLog1 == null && patVisitLog2 == null)
                return 0;
            //������Ժ�����б�����������б�,��������������
            if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Admission
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Death
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Discharge
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.InHospitalDays
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.OperationPatient
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.SeriousAndCritical
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.SpecialQC)
            {
                int nRet = string.Compare(patVisitLog1.DEPT_NAME, patVisitLog2.DEPT_NAME);
                if (nRet != 0)
                    return nRet;

                //������ͬʱ����������
                int nBedCode1 = 0;
                int.TryParse(patVisitLog1.BED_CODE, out nBedCode1);
                int nBedCode2 = 0;
                int.TryParse(patVisitLog2.BED_CODE, out nBedCode2);
                return nBedCode1.CompareTo(nBedCode2);
            }

            //����ָ��ID�Ļ����б�,��ֻ��סԺ��ʶ�Ž�����������
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.PatientID
                || this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.InHospitalID)
            {
                int nVisitID1 = 0;
                int.TryParse(patVisitLog1.VISIT_ID, out nVisitID1);
                int nVisitID2 = 0;
                int.TryParse(patVisitLog2.VISIT_ID, out nVisitID2);
                return nVisitID1.CompareTo(nVisitID2);
            }
            return 0;
        }

        /// <summary>
        /// ���ָ����PatVisitLog����,������ֿ�������Ϊ��,��ô���¿�������
        /// </summary>
        /// <param name="patVisitLog">PatVisitLog</param>
        private void SetPatVisitLogDeptName(EMRDBLib.PatVisitInfo patVisitLog)
        {
            if (patVisitLog == null || !GlobalMethods.Misc.IsEmptyString(patVisitLog.DEPT_NAME))
                return;
            DeptInfo deptInfo = null;
            if (!GlobalMethods.Misc.IsEmptyString(patVisitLog.DEPT_CODE))
            {
                DataCache.Instance.GetDeptInfo(patVisitLog.DEPT_CODE, ref deptInfo);
            }
            else if (!GlobalMethods.Misc.IsEmptyString(patVisitLog.DischargeDeptCode))
            {
                DataCache.Instance.GetDeptInfo(patVisitLog.DischargeDeptCode, ref deptInfo);
            }
            if (deptInfo != null)
                patVisitLog.DEPT_NAME = deptInfo.DEPT_NAME;
        }

        /// <summary>
        /// ˢ��ѡ�л��ߵ�״̬
        /// </summary>
        public void RefreshPatStatus(float fScore)
        {
            if (SystemParam.Instance.PatVisitLogTable == null)
                return;
            Hashtable htPatVisitLog = SystemParam.Instance.PatVisitLogTable;
            if (!htPatVisitLog.Contains(SystemParam.Instance.PatVisitInfo))
                return;
            if (this.patInfoList.SelectedCard == null)
                return;
            //��ѯ�ʿؽ��״̬
            string szQCResultStatus = GetQCResultStatus();
            SystemParam.Instance.PatVisitInfo.QCResultStatus = szQCResultStatus;
            //���ò�������
            if (fScore > 0)
                SystemParam.Instance.PatVisitInfo.TotalScore = fScore.ToString();

            this.patInfoList.SelectedCard.PatVisitLog = SystemParam.Instance.PatVisitInfo;
            this.patInfoList.SelectedCard.Refresh();
        }
        public void RefreshDocScore()
        {
            QCScore qcScore = null;
            short shRet = QcScoreAccess.Instance.GetQCScore(SystemParam.Instance.PatVisitInfo.PATIENT_ID, SystemParam.Instance.PatVisitInfo.VISIT_ID, ref qcScore);
            SystemParam.Instance.PatVisitInfo.TotalScore = qcScore.HOS_ASSESS.ToString();
            this.patInfoList.SelectedCard.PatVisitLog = SystemParam.Instance.PatVisitInfo;
            this.patInfoList.SelectedCard.Refresh();
        }
        /// <summary>
        /// ��ȡ�����ʿؽ��
        /// ˢ�»�������ʱû��ˢ���ʿؽ��
        /// RefreshPatStatusʱʵʱ��ȡ��
        /// </summary>
        /// <returns></returns>
        private string GetQCResultStatus()
        {
            string szQCResultStatus = string.Empty;
            float Score = 0.0f;
            short shRet = MedQCAccess.Instance.GetQCResultStatus(SystemParam.Instance.PatVisitInfo.PATIENT_ID, SystemParam.Instance.PatVisitInfo.VISIT_ID, ref Score, ref szQCResultStatus);

            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��ȡ�ʿؽ��״̬ʧ�ܣ�");
                return string.Empty;
            }
            return szQCResultStatus;
        }
        /// <summary>
        /// �����̵߳�״̬
        /// </summary>
        /// <param name="szThrdName">�߳�����</param>
        /// <param name="lstMedDocInfo">��ѯ�����ĵ��б�</param>
        /// <param name="st">�߳�״̬</param>
        internal void OnUpdateThreadState(string szThrdName, List<EMRDBLib.PatVisitInfo> lstPatVisitLog, EMRDBLib.SearchThreadState threadState)
        {
            if (this.m_lstPatVisitLog == null)
                this.m_lstPatVisitLog = new List<EMRDBLib.PatVisitInfo>();

            SearchThread searchThread = QueryHelper.Instance.Item(szThrdName);
            searchThread.ThreadState = threadState;
            if (threadState != EMRDBLib.SearchThreadState.finished)
                return;
            if (lstPatVisitLog != null)
                this.m_lstPatVisitLog.AddRange(lstPatVisitLog);
            if (QueryHelper.Instance.IsFinishProgress())
            {
                this.LoadPatientVisitList(this.m_lstPatVisitLog, true);
            }
            if (lstPatVisitLog == null)
            {
                this.CheckThreadProcess();
                return;
            }
            this.CheckThreadProcess();
        }

        private void CheckThreadProcess()
        {
            if (!QueryHelper.Instance.IsFinishProgress())
            {
                if (QueryHelper.Instance.StartNextThread())
                    this.MainForm.ShowStatusMessage("���ڶ�ȡ�����б����Ժ�...");
                return;
            }
            if (this.m_lstPatVisitLog == null || this.m_lstPatVisitLog.Count <= 0)
            {
                this.patInfoList.ClearPatInfo();
                this.MainForm.ShowStatusMessage("û�м�������������������");
            }
            else
            {
                this.MainForm.ShowStatusMessage(string.Format("��������{0}������", this.m_lstPatVisitLog.Count));
            }
            SystemParam.Instance.PatVisitLogList = this.m_lstPatVisitLog;
            this.MainForm.OnPatientListInfoChanged(EventArgs.Empty);
        }

        /// <summary>
        /// �������ύ����
        /// </summary>
        private void RollBackSubmitDoc()
        {
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            EMRDBLib.MrIndex mrIndex = null;
            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            short shRet = MrIndexAccess.Instance.GetMrIndex(szPatientID, szVisitID, ref mrIndex);
            if (shRet != SystemData.ReturnValue.OK || mrIndex == null)
                return;
            string status = mrIndex.MR_STATUS;//������ʷ����״̬
            mrIndex.MR_STATUS = "O";
            shRet = MrIndexAccess.Instance.UpdateMrStatus(mrIndex);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("���²���״̬ʧ�ܣ�", MessageBoxIcon.Warning);
                return;
            }
            //ѡ��֪ͨ���ĸ�ҽ��
            //string szVisitType = MedDocSys.DataLayer.SystemData.VisitType.OP;

            List<MedDocInfo> lstDocInfos = null;
            //shRet = DataAccess.GetPastDocList(szPatientID, szVisitID, ref lstDocInfos);
            shRet = EmrDocAccess.Instance.GetDocList(szPatientID, szVisitID, ref lstDocInfos);
            Dictionary<string, string> doctorDict = new Dictionary<string, string>();
            if (lstDocInfos != null)
            {
                foreach (MedDocInfo doc in lstDocInfos)
                {
                    if (!doctorDict.ContainsKey(doc.CREATOR_ID))
                    {
                        doctorDict.Add(doc.CREATOR_ID, doc.CREATOR_NAME);
                    }
                }
            }
            if (doctorDict.Count > 0)
            {
                //���ϵ���ĵ�������ֻ��һ��������ѡ������ҽ��
                if (doctorDict.Count == 1)
                {
                    mrIndex.SUBMIT_DOCTOR_ID = lstDocInfos[0].CREATOR_ID;
                }
                else
                {
                    SelectDoctorForm selForm = new SelectDoctorForm(doctorDict);
                    DialogResult result = selForm.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        mrIndex.SUBMIT_DOCTOR_ID = selForm.DoctorID;
                    }
                    else//ȡ��ѡ������ҽ������ôQCScore��ԭ
                    {
                        mrIndex.MR_STATUS = status;
                        MrIndexAccess.Instance.UpdateMrStatus(mrIndex);
                        return;
                    }
                }
            }

            shRet = PatVisitAccess.Instance.UpdateMrOnLineInfo(szPatientID, szVisitID, "0", mrIndex.SUBMIT_DOCTOR_ID
                , DateTime.Now, 1);
            //����OperFlagΪ1ʱ�������¼������������ͻ����ʧ�ܣ����������Ҫִ�и��²���
            if (shRet != EMRDBLib.SystemData.ReturnValue.OK)
            {
                shRet = PatVisitAccess.Instance.UpdateMrOnLineInfo(szPatientID, szVisitID, "0", mrIndex.SUBMIT_DOCTOR_ID
                               , DateTime.Now, 2);
            }
            if (shRet == SystemData.ReturnValue.OK)
            {
                //�¾���һ�Ų��Ժ󣬲���Ҫ����ɾ�����ĵ�״̬
                //shRet = MedQCAccess.Instance.UpdateDocStatus(szPatientID, szVisitID, "0");
                if (shRet == SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.Show("�������ύ�����ɹ���", MessageBoxIcon.Information);
                }
                else
                {
                    MessageBoxEx.Show("�������ύ����ʧ�ܣ�", MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBoxEx.Show("�������ύ����ʧ�ܣ�", MessageBoxIcon.Warning);
            }
        }

        private void patSearchPane1_StartSearch(object sender, EventArgs e)
        {
            //���ϵͳ�и����ݴ����е�����
            if (this.patInfoList.SelectedCard != null)
                this.patInfoList.SelectedCard = null;

            //�Բ�����null˵���û��Ѿ�ȡ���˻����л�
            if (this.patInfoList.SelectedCard != null)
                return;

            this.ShowStatusMessage("���ڶ�ȡ�����б����Ժ�...");
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.patInfoList.ClearPatInfo();
            if (this.m_lstPatVisitLog == null)
                this.m_lstPatVisitLog = new List<EMRDBLib.PatVisitInfo>();
            this.m_lstPatVisitLog.Clear();
            if (this.m_htDeptInfo == null)
                this.m_htDeptInfo = new Hashtable();
            this.m_htDeptInfo.Clear();
            if (SystemParam.Instance.PatVisitLogTable == null)
                SystemParam.Instance.PatVisitLogTable = new Hashtable();
            else
                SystemParam.Instance.PatVisitLogTable.Clear();

            short shRet = SystemData.ReturnValue.OK;
            List<EMRDBLib.PatVisitInfo> lstPatVisitLogs = null;
            string szDeptCode = null;
            if (!SystemParam.Instance.QCUserRight.ManageAllQC.Value)
                szDeptCode = SystemParam.Instance.UserInfo.DeptCode;

            //���Ҽ���
            if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Department)
            {
                DeptInfo deptInfo = this.patSearchPane1.DeptInfo;

                string szSelectDeptCode = string.Empty;
                if (deptInfo != null)
                {
                    szSelectDeptCode = deptInfo.DEPT_CODE;
                }
                TimeSpan timeSpan = this.patSearchPane1.AdmissionTimeEnd - this.patSearchPane1.AdmissionTimeBegin;
                if (timeSpan.Days <= 30 || patSearchPane1.PatientType == EMRDBLib.PatientType.PatInHosptial)
                {
                    shRet = PatVisitAccess.Instance.GetPatVisitList(szSelectDeptCode, this.patSearchPane1.PatientType, this.patSearchPane1.AdmissionTimeBegin
                       , this.patSearchPane1.AdmissionTimeEnd, ref lstPatVisitLogs);
                }
                else
                {
                    QueryHelper.Instance.ExecuteSearch(szSelectDeptCode, this.patSearchPane1.PatientType, this.patSearchPane1.AdmissionTimeBegin
                        , this.patSearchPane1.AdmissionTimeEnd, this, this.patSearchPane1.SearchType, string.Empty, null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
            }
            //��Ժʱ�����
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Admission)
            {
                DateTime dtAdmissionTimeBegin = this.patSearchPane1.AdmissionTimeBegin;
                DateTime dtAdmissionTimeEnd = this.patSearchPane1.AdmissionTimeEnd;
                dtAdmissionTimeBegin = this.patSearchPane1.AdmissionTimeBegin;
                dtAdmissionTimeEnd = this.patSearchPane1.AdmissionTimeEnd;
                TimeSpan timeSpan = dtAdmissionTimeEnd - dtAdmissionTimeBegin;
                if (timeSpan.Days <= 30 || this.patSearchPane1.PatientType == EMRDBLib.PatientType.PatInHosptial)
                {
                    shRet = PatVisitAccess.Instance.GetPatsListByAdmTime(dtAdmissionTimeBegin, dtAdmissionTimeEnd, this.patSearchPane1.PatientType
                        , szDeptCode, ref lstPatVisitLogs);
                }
                else
                {
                    QueryHelper.Instance.ExecuteSearch(string.Empty, this.patSearchPane1.PatientType, dtAdmissionTimeBegin
                        , dtAdmissionTimeEnd, this, this.patSearchPane1.SearchType, string.Empty, null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
            }
            //��Ժʱ�����
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Discharge)
            {
                //����ʱ�䳤����Ժ�����϶࣬
                //����л������ݳ�Ժʱ�����ʱ��ȫ���첽�߳�
                DateTime dtDischargeTimeBegin = this.patSearchPane1.DischargeTimeBegin;
                DateTime dtDischargeTimeEnd = this.patSearchPane1.DischargeTimeEnd;
                dtDischargeTimeBegin = this.patSearchPane1.DischargeTimeBegin;
                dtDischargeTimeEnd = this.patSearchPane1.DischargeTimeEnd;

                QueryHelper.Instance.ExecuteSearch(string.Empty, this.patSearchPane1.PatientType, dtDischargeTimeBegin
                    , dtDischargeTimeEnd, this, this.patSearchPane1.SearchType, string.Empty, null);
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }
            //Σ�ز�������
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.SeriousAndCritical)
            {
                DateTime dtAdmissionTimeBegin = this.patSearchPane1.SerCriAdmissionTimeBegin;
                DateTime dtAdmissionTimeEnd = this.patSearchPane1.SerCriAdmissionTimeEnd;
                TimeSpan timeSpan = dtAdmissionTimeEnd - dtAdmissionTimeBegin;

                //dtAdmissionTimeBegin = DateTime.MinValue;
                //dtAdmissionTimeEnd = DateTime.MaxValue;
                shRet = InpVisitAccess.Instance.GetPatsListBySerious(dtAdmissionTimeBegin, dtAdmissionTimeEnd, szDeptCode, ref lstPatVisitLogs);
                if (lstPatVisitLogs == null || lstPatVisitLogs.Count <= 0)
                {
                    if (SystemConfig.Instance.Get(SystemData.ConfigKey.SERIOUSPATLIST_BY_ADTLOG, false))
                        shRet = InpVisitAccess.Instance.GetPatsListSeriousByAdtLog(dtAdmissionTimeBegin, dtAdmissionTimeEnd, szDeptCode, ref lstPatVisitLogs);
                }
            }
            //������������
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Death)
            {
                DateTime dtDischargeTimeBegin = this.patSearchPane1.DischargeTimeBegin;
                DateTime dtDischargeTimeEnd = this.patSearchPane1.DischargeTimeEnd;
                TimeSpan timeSpan = dtDischargeTimeEnd - dtDischargeTimeBegin;
                if (timeSpan.Days <= 30)
                {
                    shRet = PatVisitAccess.Instance.GetPatsListByDeadTime(dtDischargeTimeBegin, dtDischargeTimeEnd, szDeptCode, ref lstPatVisitLogs);
                }
                else
                {
                    QueryHelper.Instance.ExecuteSearch(string.Empty, this.patSearchPane1.PatientType, dtDischargeTimeBegin
                        , dtDischargeTimeEnd, this, this.patSearchPane1.SearchType, string.Empty, null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
            }
            //����סԺ�ż���
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.InHospitalID)
            {
                string szInpNo = this.patSearchPane1.InHospitalID;
                if (GlobalMethods.Misc.IsEmptyString(szInpNo))
                {
                    this.ShowStatusMessage(null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
                shRet = PatVisitAccess.Instance.GetPatsListByPatient(szInpNo, false, null, szDeptCode, ref lstPatVisitLogs);
            }
            //����ID����
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.PatientID)
            {
                string szPatientID = this.patSearchPane1.PatientID;
                if (GlobalMethods.Misc.IsEmptyString(szPatientID))
                {
                    this.ShowStatusMessage(null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
                shRet = PatVisitAccess.Instance.GetPatsListByPatient(szPatientID, true, null, szDeptCode, ref lstPatVisitLogs);
            }
            //������������
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.PatientName)
            {
                string szPatientName = this.patSearchPane1.PatientName;
                if (GlobalMethods.Misc.IsEmptyString(szPatientName))
                {
                    this.ShowStatusMessage(null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
                shRet = PatVisitAccess.Instance.GetPatsListByPatient(null, false, szPatientName, szDeptCode, ref lstPatVisitLogs);
            }
            //�������߼���
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.OperationPatient)
            {
                string szPatDeptCode = string.Empty;
                DeptInfo deptInfo = this.patSearchPane1.DeptInfo;
                if (deptInfo != null)
                    szPatDeptCode = deptInfo.DEPT_CODE;
                TimeSpan timeSpan = this.patSearchPane1.AdmissionTimeEnd - this.patSearchPane1.AdmissionTimeBegin;
                if (timeSpan.Days <= 30 || patSearchPane1.PatientType == EMRDBLib.PatientType.PatInHosptial)
                {
                    shRet = PatVisitAccess.Instance.GetPatListByOperation(szPatDeptCode, this.patSearchPane1.PatientType, this.patSearchPane1.AdmissionTimeBegin
                       , this.patSearchPane1.AdmissionTimeEnd, this.patSearchPane1.OperationCode, ref lstPatVisitLogs);
                }
                else
                {
                    QueryHelper.Instance.ExecuteSearch(deptInfo.DEPT_CODE, this.patSearchPane1.PatientType, this.patSearchPane1.AdmissionTimeBegin
                        , this.patSearchPane1.AdmissionTimeEnd, this, this.patSearchPane1.SearchType, this.patSearchPane1.OperationCode, null);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    return;
                }
            }
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.InHospitalDays)
            {
                string szPatDeptCode = string.Empty;
                DeptInfo deptInfo = this.patSearchPane1.DeptInfo;
                if (deptInfo != null)
                    szPatDeptCode = deptInfo.DEPT_CODE;
                if (this.patSearchPane1.InHosptialDays <= 0)
                {
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    this.ShowStatusMessage(null);
                    return;
                }
                shRet = PatVisitAccess.Instance.GetPatListByInHospDays(szPatDeptCode, this.patSearchPane1.InHosptialDays, this.patSearchPane1.operatorType
                    , this.patSearchPane1.PatientType, ref lstPatVisitLogs);
            }//���ʼ첡����Ա��ѯ
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.CheckedDoc)
            {
                string szCheckerName = string.Empty;
                UserInfo userInfo = this.patSearchPane1.UserInfo;
                if (userInfo != null)
                    szCheckerName = userInfo.Name;

                shRet = PatVisitAccess.Instance.GetPatListByCheckedDoc(szCheckerName, this.patSearchPane1.IssusdTimeBegin, this.patSearchPane1.IssusdTimeEnd
                        , ref lstPatVisitLogs);
            }
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.SpecialQC)
            {
                if (this.patSearchPane1.QcSpecialCheck != null)
                {
                    string szConfigID = this.patSearchPane1.QcSpecialCheck.ConfigID;
                    string szUserID = this.patSearchPane1.IsSpecialAll ? "" : SystemParam.Instance.UserInfo.ID;//����ʾ����Ĳ����ٴ��û�ID
                    shRet = SpecialAccess.Instance.GetPatientList(szConfigID, szUserID, ref lstPatVisitLogs);
                    if (this.m_comparison == null)
                        this.m_comparison = new Comparison<EMRDBLib.PatVisitInfo>(this.ComparePatVisitLog);
                    if (lstPatVisitLogs != null)
                        lstPatVisitLogs.Sort(this.m_comparison);
                }
            }
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.DocType)
            {
                DocTypeInfo docTypeInfo = this.patSearchPane1.DocType;
                if (docTypeInfo == null)
                {
                    MessageBoxEx.Show("��ѡ���ĵ�����", MessageBoxIcon.Warning);
                    GlobalMethods.UI.SetCursor(this, Cursors.Default);
                    this.ShowStatusMessage(null);
                    return;
                }
                TimeSpan timeSpan = this.patSearchPane1.DocTimeEnd - this.patSearchPane1.DocTimeBegin;
                shRet = EmrDocAccess.Instance.GetPatListByDocTypeAndDocTime(docTypeInfo.DocTypeID, this.patSearchPane1.DocTimeBegin, this.patSearchPane1.DocTimeEnd, ref lstPatVisitLogs);

            }
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.MutiVisit)
            {
                TimeSpan timeSpan = this.patSearchPane1.VisitTimeEnd - this.patSearchPane1.VisitTimeBegin;
                if (timeSpan.Days < 0)
                    return;
                shRet = PatVisitAccess.Instance.GetPatListByMutiVisit(this.patSearchPane1.DocTimeBegin, this.patSearchPane1.DocTimeEnd, ref lstPatVisitLogs);
            }
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.TransferPatient)
            {
                TimeSpan timeSpan = this.patSearchPane1.VisitTimeEnd - this.patSearchPane1.VisitTimeBegin;
                if (timeSpan.Days < 0)
                    return;
                shRet = PatVisitAccess.Instance.GetPatListByTransferTime(this.patSearchPane1.DocTimeBegin, this.patSearchPane1.DocTimeEnd, ref lstPatVisitLogs);
            }
            //������
            else if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.Review)
            {
                //��ʱ�Ȱ�ת�Ʋ�ѯ
                TimeSpan timeSpan = this.patSearchPane1.VisitTimeEnd - this.patSearchPane1.VisitTimeBegin;
                if (timeSpan.Days < 0)
                    return;
                shRet = PatVisitAccess.Instance.GetPatListByDocReview(this.patSearchPane1.DocTimeBegin, this.patSearchPane1.DocTimeEnd, ref lstPatVisitLogs);
            }
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            if (shRet == EMRDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("û�л�ȡ�����������Ļ����б�!", MessageBoxIcon.Asterisk);
                return;
            }
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��ȡ�����б�ʧ�ܣ�");
                return;
            }
            this.LoadPatientVisitList(lstPatVisitLogs, true);
            SystemParam.Instance.PatVisitLogList = lstPatVisitLogs;
            this.MainForm.OnPatientListInfoChanged(EventArgs.Empty);
        }

        private void patSearchPane1_StatusMessageChanged(string szStatusMessage)
        {
            this.ShowStatusMessage(szStatusMessage);
        }

        private void PatSearchPane1_ComboBoxSelectItemChanged(object sender, EventArgs e)
        {
            this.patInfoList.ClearPatInfo();
            this.ShowStatusMessage("����");
        }

        private void mnuHistoryVisit_Click(object sender, EventArgs e)
        {

            if (this.patInfoList.SelectedCard == null)
                return;
            EMRDBLib.PatVisitInfo patVisitLog = this.patInfoList.SelectedCard.Tag as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;

            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            SystemConfig.Instance.Write(SystemData.ConfigKey.DEFAULT_PATIENT_ID, patVisitLog.PATIENT_ID);
            this.patSearchPane1.LastPatientID = patVisitLog.PATIENT_ID;
            this.patSearchPane1.SearchType = EMRDBLib.PatSearchType.PatientID;
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuPreviousView_Click(object sender, EventArgs e)
        {
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.patSearchPane1.LastSearchType != EMRDBLib.PatSearchType.Unknown)
                this.patSearchPane1.SearchType = this.patSearchPane1.LastSearchType;
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuDocumentList_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowDocumentListForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuOrderList_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowOrdersListForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuVitalSignsGraph_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowVitalSignsGraphForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        private void mnuExamList_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowExamResultListForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuTestList_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowTestResultListForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuPatientInfo_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowPatientInfoForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuDocumentTime_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowDocumentTimeForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuQuestionList_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowQuestionListForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuDiagnosisList_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowDiagnosisResultForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuPatientIndex_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowPatientIndexForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }


        private void mnuDocScore_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.MainForm.ShowDocScoreForm();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void mnuRollBackDoc_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            this.RollBackSubmitDoc();
        }

        private void cmenuPatientList_Opening(object sender, CancelEventArgs e)
        {
            this.mnuHistoryVisit.Enabled = false;
            this.mnuDocumentList.Enabled = false;
            this.mnuOrderList.Enabled = false;
            this.mnuExamList.Enabled = false;
            this.mnuTestList.Enabled = false;
            this.mnuDocumentTime.Enabled = false;
            this.mnuQuestionList.Enabled = false;
            this.mnuPreviousView.Enabled = false;
            this.mnuVitalSignsGraph.Visible = false;
            this.mnuIEDoc.Visible = false;
            if (this.patSearchPane1.LastSearchType != EMRDBLib.PatSearchType.Unknown)
                this.mnuPreviousView.Enabled = true;

            if (this.patInfoList.SelectedCard == null)
                return;

            EMRDBLib.PatVisitInfo patVisitLog = this.patInfoList.SelectedCard.PatVisitLog as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;

            if (SystemParam.Instance.PatVisitInfo != patVisitLog)
                this.MainForm.OnPatientInfoChanged(EventArgs.Empty);
            SystemParam.Instance.PatVisitInfo = patVisitLog;
            this.m_lastSelectedPatVisitLog = patVisitLog;

            if (SystemParam.Instance.QCUserRight.BrowseDocumentList.Value)
                this.mnuDocumentList.Enabled = true;
            else
                this.mnuDocumentList.Visible = false;

            if (SystemParam.Instance.QCUserRight.BrowseOrdersList.Value)
                this.mnuOrderList.Enabled = true;
            else
                this.mnuOrderList.Visible = false;

            if (SystemParam.Instance.QCUserRight.BrowseExamList.Value)
                this.mnuExamList.Enabled = true;
            else
                this.mnuExamList.Visible = false;

            if (SystemParam.Instance.QCUserRight.BrowseLabTestList.Value)
                this.mnuTestList.Enabled = true;
            else
                this.mnuTestList.Visible = false;

            if (SystemParam.Instance.QCUserRight.BrowseDocumentTime.Value)
                this.mnuDocumentTime.Enabled = true;
            else
                this.mnuDocumentTime.Visible = false;

            if (SystemParam.Instance.QCUserRight.BrowseQCQuestion.Value)
                this.mnuQuestionList.Enabled = true;
            else
                this.mnuQuestionList.Visible = false;
            if (SystemParam.Instance.QCUserRight.BrowsePatientInfo.Value)
                this.mnuPatientInfo.Enabled = true;
            else
            {
                this.mnuPatientInfo.Visible = false;
                this.toolStripSeparator2.Visible = false;
            }
            if (SystemParam.Instance.QCUserRight.BrowseDiagnosisList.Value)
                this.mnuDiagnosisList.Enabled = true;
            else
                this.mnuDiagnosisList.Visible = false;
            if (SystemParam.Instance.QCUserRight.BrowseMRScore.Value
                && SystemParam.Instance.QCUserRight.ManageAllQC.Value)
                this.mnuDocScore.Enabled = true;
            else
                this.mnuDocScore.Visible = false;
            if (SystemParam.Instance.QCUserRight.ManageRollbackSubmitDoc.Value
                && SystemParam.Instance.QCUserRight.ManageAllQC.Value)
            {
                this.mnuRollBackDoc.Enabled = true;
                if (EMRDBLib.SystemParam.Instance.LocalConfigOption.RightClickCallback)
                {
                    this.mnuRollBackDoc.Text = "�����ٻ�";
                }
            }
            else
            {
                this.toolStripSeparator3.Visible = false;
                this.mnuRollBackDoc.Visible = false;
            }

            if (this.patSearchPane1.SearchType != EMRDBLib.PatSearchType.Unknown &&
                this.patSearchPane1.SearchType != EMRDBLib.PatSearchType.PatientID &&
                this.patSearchPane1.SearchType != EMRDBLib.PatSearchType.InHospitalID)
            {
                this.mnuHistoryVisit.Enabled = true;
            }
            //�Ƿ���ʾ���µ�
            if (SystemParam.Instance.LocalConfigOption.IsShowVitalSignsGraph && SystemParam.Instance.QCUserRight.BrowsTemperatureChart.Value)
                this.mnuVitalSignsGraph.Visible = true;
            //�Ƿ���ʾ������ҳ
            if (!SystemParam.Instance.LocalConfigOption.IsShowPatientIndex)
            {
                this.mnuPatientIndex.Visible = false;
            }
            //������ʽ��ר���ʿ��ҵ�ǰ�û���ר���ʿ�Ȩ�޲���ʾɨ�財��
            if (this.patSearchPane1.SearchType == EMRDBLib.PatSearchType.SpecialQC && SystemParam.Instance.QCUserRight.IsSpecialDoc.Value)
            {
                this.mnuIEDoc.Visible = true;
            }
            //�°������ػ����б��Ҽ��Ͳ���������صĲ˵�
            if (SystemParam.Instance.LocalConfigOption.IsNewTheme)
            {
                mnuDiagnosisList.Visible = false;
                mnuDocScore.Visible = false;
                mnuDocumentList.Visible = false;
                mnuDocumentTime.Visible = false;
                mnuExamList.Visible = false;
                mnuOrderList.Visible = false;
                mnuPatientIndex.Visible = false;
                mnuPatientInfo.Visible = false;
                mnuTestList.Visible = false;
                mnuVitalSignsGraph.Visible = false;
            }
        }

        private void patInfoList_CardSelectedChanged(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;

            PatInfoCard selectedPatCard = this.patInfoList.SelectedCard;
            if (selectedPatCard == null || selectedPatCard.IsDisposed)
                return;
            this.UpdatePatientCondition(selectedPatCard.PatVisitLog);
            EMRDBLib.PatVisitInfo patVisitLog = selectedPatCard.PatVisitLog;
            if (patVisitLog == null)
                return;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.patInfoList.SelectedCard.Invalidate();
            short shRet = PatVisitAccess.Instance.GetPatVisitInfo(patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID, ref patVisitLog);
            SystemParam.Instance.PatVisitInfo = patVisitLog;
            this.MainForm.OnPatientInfoChanged(EventArgs.Empty);

            this.m_lastSelectedPatVisitLog = patVisitLog;

            if (SystemParam.Instance.LocalConfigOption.IsNewTheme)
            {
                this.MainForm.ShowPatientPageForm(patVisitLog);
            }
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// ���³�Ժ���߲���
        /// </summary>
        /// <param name="patVisitLog"></param>
        private void UpdatePatientCondition(EMRDBLib.PatVisitInfo patVisitLog)
        {
            if (patVisitLog == null)
                return;
            if (patVisitLog.DISCHARGE_TIME == patVisitLog.DefaultTime)
                return;
            if (!string.IsNullOrEmpty(patVisitLog.PATIENT_CONDITION))
                return;
            List<EMRDBLib.PatVisitInfo> lstPatVisitLog = new List<EMRDBLib.PatVisitInfo>();
            lstPatVisitLog.Add(patVisitLog);
            PatVisitAccess.Instance.GetOutPatientCondition(lstPatVisitLog);
            if (lstPatVisitLog == null)
                return;
        }

        /// <summary>
        /// ���»����б��г�Ժ���ߵĲ���
        /// </summary>
        /// <param name="lstPatVisitLog"></param>
        private void UpdatePatientsCondition(List<EMRDBLib.PatVisitInfo> lstPatVisitLog)
        {
            if (lstPatVisitLog == null || lstPatVisitLog.Count == 0)
                return;
            //��ȡ��Ժ����
            List<EMRDBLib.PatVisitInfo> lstOutPatientVisitLog = new List<EMRDBLib.PatVisitInfo>();
            foreach (EMRDBLib.PatVisitInfo item in lstPatVisitLog)
            {
                if (item.DISCHARGE_TIME == item.DefaultTime)
                    continue;
                lstOutPatientVisitLog.Add(item);
            }
            if (lstOutPatientVisitLog == null || lstOutPatientVisitLog.Count == 0)
                return;
            //���³�Ժ���߲���
            short shRet = PatVisitAccess.Instance.GetOutPatientCondition(lstOutPatientVisitLog);
            if (shRet != SystemData.ReturnValue.OK)
                return;

            foreach (EMRDBLib.PatVisitInfo item in lstOutPatientVisitLog)
            {
                EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLog.Find(delegate (EMRDBLib.PatVisitInfo p)
                {
                    if (p.VISIT_ID == item.VISIT_ID && p.PATIENT_ID == item.PATIENT_ID)
                        return true;
                    else
                        return false;
                });
                if (patVisitLog != null)
                    patVisitLog.PATIENT_CONDITION = item.PATIENT_CONDITION;
            }
        }

        private void patInfoList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            this.cmenuPatientList.Show(this.patInfoList, e.Location);
        }

        private void patInfoList_CardSelectedChanging(object sender, CancelEventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����׼���л�������Ϣ�����Ժ�...");

            CancelEventArgs cancelArgs = new CancelEventArgs();
            this.MainForm.OnPatientInfoChanging(cancelArgs);
            if (cancelArgs.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                SystemParam.Instance.PatVisitInfo = null;
            }
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }
        private void patInfoList_CardMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            PatInfoCard patInfoCard = sender as PatInfoCard;
            if (patInfoCard == null || patInfoCard.IsDisposed)
                return;
            this.cmenuPatientList.Show(patInfoCard, e.Location);
        }


    }
}