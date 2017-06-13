// ***********************************************************
// �����ʿ�ϵͳ������״̬���ؼ�.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using EMRDBLib.Entity;
using Heren.Common.Libraries;
using Timer = System.Windows.Forms.Timer;
using EMRDBLib.DbAccess;
using EMRDBLib;
using MedDocSys.QCEngine.BugCheck;
using MedDocSys.QCEngine.TimeCheck;

namespace Heren.MedQC.TimeCheckGener.Forms
{
    internal class MainStatusStrip : StatusStrip
    {
        //����״̬
        public enum RunState
        {
            Normal,
            Running,
            Pause,
            Stop
        }

        private List<PatVisitInfo> m_ListPatVisitInfos;


        private BugCheckEngine m_bugCheckEngine;
        private RunState m_BugState = RunState.Normal;

        private DateTime m_LastBugRunTime = SystemParam.Instance.DefaultTime;
        //���һ��ʱЧִ�е�ʱ��
        private DateTime m_LastQCRunTime = SystemParam.Instance.DefaultTime;


        private global::MedQC.TimeCheckGener.MainForm m_parent;
        private RunState m_QCState = RunState.Normal;
        private Thread m_QCThread;

        public MainStatusStrip()
            : this(null)
        {
            m_ListPatVisitInfos = new List<PatVisitInfo>();
        }

        public MainStatusStrip(global::MedQC.TimeCheckGener.MainForm parent)
        {
            m_parent = parent;
            InitializeComponent();
            CommonTimer.Stop();
        }

        public virtual global::MedQC.TimeCheckGener.MainForm MainForm
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public void ShowStatusMessage(string szMessage)
        {
            if (szMessage == null || szMessage.Trim() == string.Empty)
                szMessage = "����";
            statuslblSystemInfo.Text = szMessage;
            Update();
        }

        public void Stop()
        {
            CommonTimer.Stop();
            if (m_QCThread != null)
                m_QCThread.Abort();

        }

        public void Start()
        {
            CommonTimer.Start();

            m_ListPatVisitInfos.Clear();
        }

        private void CommonTick_Tick(object sender, EventArgs e)
        {
            statuslblTime.Text = DateTime.Now.ToString("yyyy��M��d�� HH:mm:ss dddd");
            //ʱЧ���
            if (m_QCState != RunState.Running
                && DateTime.Now.ToString("H:m") == MainForm.QCStartTime
               ) //�������й��˲�������
            {
                m_QCState = RunState.Running;
                m_QCThread = new Thread(DoTimeCheckGener);
                m_QCThread.Start(); //�����߳�

            }
            //���ݼ��
            if (m_BugState != RunState.Running
                && DateTime.Now.ToString("H:m") == MainForm.BugStartTime
               )
            {

                //m_BugState = RunState.Running;
                //m_BugThread = new Thread(BugCheckGener);
                //m_BugThread.Start(); //�����߳�
                BugCheckGener();

            }
        }

        private void DoTimeCheckGener()
        {
            if (!this.MainForm.bTimeCheckRunning)
                return;
            int iCurrIndex = 0;
            LogManager.Instance.WriteLog(string.Format("{0} ִ��ʱЧ��¼����", DateTime.Now), null, LogType.Information);
            List<EMRDBLib.PatVisitInfo> lstPatVisitLog = null;
            short shRet = QcTimeRecordAccess.Instance.GetPatsListByInHosptial(ref lstPatVisitLog);
            if (shRet != SystemData.ReturnValue.OK && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                LogManager.Instance.WriteLog("δ��ѯ����Ժ���˻�ʧ��");
                m_QCState = RunState.Normal;
                return;
            }
            int nudDisCharge = int.Parse(m_parent.DischargeDays);
            DateTime now = DateTime.Now;
            m_LastQCRunTime = now;
            DateTime dtDischargeBeginTime = DateTime.Parse(now.AddDays(-nudDisCharge).ToString("yyyy-MM-dd 00:00:00"));
            DateTime dtDischargeEndTime = DateTime.Parse(now.ToString("yyyy-MM-dd 23:59:59"));

            shRet = QcTimeRecordAccess.Instance.GetPatsListByOutHosptial(dtDischargeBeginTime, dtDischargeEndTime,
                ref lstPatVisitLog);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND
                && lstPatVisitLog.Count <= 0)
            {
                LogManager.Instance.WriteLog("δ��ѯ����Ժ����");
                m_QCState = RunState.Normal;
                return;
            }
            if (lstPatVisitLog != null && lstPatVisitLog.Count > 0)
            {
                LogManager.Instance.WriteLog(
                    string.Format("ʱЧ�汾�ţ�{1}������ʱЧ���в�������{0} ��ʼִ�� ", lstPatVisitLog.Count,
                        Assembly.GetExecutingAssembly().GetName().Version), null, LogType.Information);
                iCurrIndex = 0;
                StartRun(lstPatVisitLog.Count, CheckType.TimeCheck);
                int nError = 0;
                int nSuccess = 0;
                foreach (EMRDBLib.PatVisitInfo item in lstPatVisitLog)
                {
                    shRet = TimeCheckHelper.Instance.GenerateTimeRecord(item,now);
                    if (shRet == SystemData.ReturnValue.EXCEPTION)
                    {
                        m_QCState = RunState.Normal;
                        nError++;
                        continue;
                    }
                    if (shRet == SystemData.ReturnValue.RES_NO_FOUND)
                    {
                        continue;
                    }
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        nError++;
                    }
                    iCurrIndex++;
                    string szMessage = string.Format("ʱЧ�ʿز������� {0}/{1}"
                        , iCurrIndex, lstPatVisitLog.Count);
                    ShowMessage(szMessage, iCurrIndex, CheckType.TimeCheck);
                    nSuccess++;
                }
                //ִ�гɹ���д����־
                LogManager.Instance.WriteLog(string.Format("���н���������ִ�гɹ���{0}��ʧ����{1}", nSuccess, nError), null,
                    LogType.Information);
                //����ͳ�Ƶ����˵�ʱЧ���
                shRet = QcTimeRecordAccess.Instance.SaveTimeRecordStatByPatient(now);


                if (shRet != SystemData.ReturnValue.OK)
                {
                    LogManager.Instance.WriteLog("ͳ�Ƶ����˵Ľ��ʱЧ�Խ������ʧ��");
                }
                //����ͳ�Ƶ����ҵ�ʱЧ��� 
                shRet = QcTimeRecordAccess.Instance.SaveTimeRecordStatByDept(now);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    LogManager.Instance.WriteLog("ͳ�Ƶ����ҵĽ��ʱЧ�Խ������ʧ��");
                }
            }
            m_QCState = RunState.Normal;


            EndRun(iCurrIndex, CheckType.TimeCheck);
        }

        private void BugCheckGener()
        {
            if (!this.MainForm.ContentRecordRunning)
                return;
            int iCurrIndex = 0;
            LogManager.Instance.WriteLog(string.Format("{0} ִ�����ݼ���¼����", DateTime.Now), null, LogType.Information);
            List<EMRDBLib.MedDocInfo> lstDocInfos = null;
            int nDays = m_parent.DocContentDay;
            short shRet = EmrDocAccess.Instance.GetDocInfoByModifyTime(DateTime.Now, nDays, ref lstDocInfos);
            if (shRet != SystemData.ReturnValue.OK || lstDocInfos == null)
            {
                LogManager.Instance.WriteLog("δ��ѯ�������޸Ĳ�����Ϣ");
                m_BugState = RunState.Normal;
                return;
            }
            StartRun(lstDocInfos.Count, CheckType.BugCheck);
            //�����ø����ؼ�
            Editor.Instance.ParentControl = Parent;
            for (int index = 0; index < lstDocInfos.Count; index++)
            {
                string szMessage = string.Format("�����ʿز������� {0}/{1}"
                   , index + 1, lstDocInfos.Count);
                ShowMessage(szMessage, index + 1, CheckType.BugCheck);
                string szTextData = Editor.Instance.GetDocText(lstDocInfos[index]);
                if (string.IsNullOrEmpty(szTextData))
                    continue;
                //��ȡ���˻�����Ϣ
                InitPatientInfo(lstDocInfos[index].PATIENT_ID, lstDocInfos[index].VISIT_ID);

                //��ʼ���ʿ�����
                if (m_bugCheckEngine == null)
                    m_bugCheckEngine = new BugCheckEngine();
                m_bugCheckEngine.UserInfo = GetUserInfo();
                m_bugCheckEngine.PatientInfo = GetPatientInfo(lstDocInfos[index].PATIENT_ID, lstDocInfos[index].VISIT_ID);
                m_bugCheckEngine.VisitInfo = GetVisitInfo(lstDocInfos[index].PATIENT_ID, lstDocInfos[index].VISIT_ID);
                m_bugCheckEngine.DocType = lstDocInfos[index].DOC_TITLE;
                m_bugCheckEngine.DocText = szTextData;
                //m_bugCheckEngine.SectionInfos = new List<DocumentSectionInfo>();
                shRet = m_bugCheckEngine.InitializeEngine();
                if (shRet != SystemData.ReturnValue.OK)
                {
                    LogManager.Instance.WriteLog(string.Format("{0} �����ʿ������ʼ��ʧ��,�޷��Բ��������Զ��ʿأ�", DateTime.Now), null,
                        LogType.Information);
                    continue;
                }

                //����ĵ�����ȱ��
                List<DocuemntBugInfo> lstDocuemntBugList = null;
                if (shRet == SystemData.ReturnValue.OK)
                {
                    lstDocuemntBugList = m_bugCheckEngine.PerformBugCheck();
                    m_bugCheckEngine.DocText = null;
                }

                //����ĵ�Ԫ��ȱ��
                List<ElementBugInfo> lstElementBugList = null;
                Editor.Instance.CheckElementBugs(lstDocInfos[index].EMR_TYPE, ref lstElementBugList);
                if ((lstDocuemntBugList == null || lstDocuemntBugList.Count <= 0)
                    && (lstElementBugList == null || lstElementBugList.Count <= 0))
                {
                    continue;
                }

                //�������DocID��Ӧ����ɾ��������
                QcContentRecordAccess.Instance.DeleteContentRecord(lstDocInfos[index].DOC_SETID);
                //����Bug
                PatVisitInfo patVisitInfo =
                    m_ListPatVisitInfos.Find(
                        item =>
                            item.PATIENT_ID == lstDocInfos[index].PATIENT_ID && item.VISIT_ID == lstDocInfos[index].VISIT_ID);
                SaveContentRecord(patVisitInfo, lstDocInfos[index], lstDocuemntBugList, lstElementBugList);
            }

            m_BugState = RunState.Normal;
            EndRun(iCurrIndex, CheckType.BugCheck);
        }

        private void SaveContentRecord(PatVisitInfo patVisitInfo, MedDocInfo docInfo, List<DocuemntBugInfo> lstDocuemntBugList, List<ElementBugInfo> lstElementBugList)
        {
            if (patVisitInfo == null || docInfo == null)
                return;
            List<EMRDBLib.Entity.QcContentRecord> lstQcContentRecord = new List<EMRDBLib.Entity.QcContentRecord>();
            //���ݴ���ֵ
            if (lstDocuemntBugList != null)
                foreach (var item in lstDocuemntBugList)
                {
                    QcContentRecord record = CreateQcContentRecord(patVisitInfo, docInfo);
                    record.BugClass = (int)item.BugLevel;
                    record.BugType = "1";
                    //ȥ�� ��˫����λ��ȱ�ݣ�����˫����λ������
                    record.QCExplain = item.BugDesc.Replace("��˫����λ��ȱ�ݣ�", "").Replace("��˫����λ������", "");

                    lstQcContentRecord.Add(record);
                }
            //Ԫ�ش���ֵ
            if (lstElementBugList != null)
                foreach (var item in lstElementBugList)
                {
                    QcContentRecord record = CreateQcContentRecord(patVisitInfo, docInfo);
                    record.BugClass = item.IsFatalBug ? 1 : 0;
                    record.BugType = "2";
                    record.QCExplain = item.BugDesc;
                    lstQcContentRecord.Add(record);
                }
            //����
            QcContentRecordAccess.Instance.SaveQCContentRecord(lstQcContentRecord);
        }

        /// <summary>
        /// ������Ϣ��
        /// </summary>
        /// <param name="patVisitInfo"></param>
        /// <param name="docInfo"></param>
        /// <returns></returns>
        private static QcContentRecord CreateQcContentRecord(PatVisitInfo patVisitInfo, EMRDBLib.MedDocInfo docInfo)
        {
            EMRDBLib.Entity.QcContentRecord record = new EMRDBLib.Entity.QcContentRecord();
            record.PatientID = patVisitInfo.PATIENT_ID;
            record.PatientName = patVisitInfo.PATIENT_NAME;
            record.VisitID = patVisitInfo.VISIT_ID;
            record.DocTypeID = docInfo.DOC_TYPE;
            record.Point = 0.0f;
            record.DocSetID = docInfo.DOC_SETID;
            record.DocTitle = docInfo.DOC_TITLE;
            record.ModifyTime = docInfo.MODIFY_TIME;
            record.BugCreateTime = DateTime.Now;
            record.CreateID = docInfo.CREATOR_ID;
            record.CreateName = docInfo.CREATOR_NAME;
            record.DocTime = docInfo.DOC_TIME;
            record.DocIncharge = patVisitInfo.INCHARGE_DOCTOR;
            record.DeptIncharge = patVisitInfo.DEPT_NAME;
            record.DeptCode = patVisitInfo.DEPT_CODE;
            return record;
        }

        //��ȡ������Ϣ�����浽HashTable��
        private void InitPatientInfo(string patientId, string visitId)
        {
            if (m_ListPatVisitInfos == null)
                m_ListPatVisitInfos = new List<PatVisitInfo>();
            if (!m_ListPatVisitInfos.Exists(item => item.PATIENT_ID == patientId && item.VISIT_ID == visitId))
            {
                PatVisitInfo patVisitInfo = null;
                short shRet = PatVisitAccess.Instance.GetPatVisitInfo(patientId, visitId, ref patVisitInfo);
                if (shRet != SystemData.ReturnValue.OK || patVisitInfo == null)
                {
                    LogManager.Instance.WriteLog(string.Format("{0} ��ȡ������ϢPid:{1},Vid:{2}ʧ�ܣ�", DateTime.Now, patientId, visitId), null,
                        LogType.Error);
                    return;
                }
                m_ListPatVisitInfos.Add(patVisitInfo);
            }
        }

        private VisitInfo GetVisitInfo(string patientId, string visitId)
        {
            if (m_ListPatVisitInfos == null)
                return null;
            PatVisitInfo patVisitInfo =
              m_ListPatVisitInfos.Find(item => item.PATIENT_ID == patientId && item.VISIT_ID == visitId);
            if (patVisitInfo == null)
                return null;
            VisitInfo clientVisitInfo = new VisitInfo();
            clientVisitInfo.ID = visitId;
            clientVisitInfo.InpID = patVisitInfo.INP_NO;
            clientVisitInfo.Time = patVisitInfo.VISIT_TIME;
            clientVisitInfo.WardCode = patVisitInfo.WARD_CODE;
            clientVisitInfo.WardName = patVisitInfo.WardName;
            clientVisitInfo.BedCode = patVisitInfo.BED_CODE;
            clientVisitInfo.Type = VisitType.IP;
            return clientVisitInfo;
        }

        private PatientInfo GetPatientInfo(string patientId, string visitId)
        {
            PatVisitInfo patVisitInfo =
               m_ListPatVisitInfos.Find(item => item.PATIENT_ID == patientId && item.VISIT_ID == visitId);
            if (patVisitInfo == null)
                return null;

            PatientInfo patInfo = new PatientInfo();
            patInfo.ID = patientId;
            patInfo.Name = patVisitInfo.PATIENT_NAME;
            patInfo.Gender = patVisitInfo.PATIENT_SEX;
            patInfo.BirthTime = patVisitInfo.BIRTH_TIME;
            return patInfo;
        }

        /// <summary>
        ///     ����ϵͳ����
        /// </summary>
        /// <returns></returns>
        private UserInfo GetUserInfo()
        {
            UserInfo clientUserInfo = new UserInfo();
            clientUserInfo.ID = "System";
            clientUserInfo.Name = "System";
            return clientUserInfo;
        }

        /// <summary>
        ///     ί�����߳���ʾ��ǰ����������Ϣ
        /// </summary>
        /// <param name="info">��ʾ����Ϣ</param>
        private void ShowMessage(string info, int index, string szCheckType)
        {
            if (m_parent == null || m_parent.IsDisposed)
                return;
            try
            {
                ShowMessageHandler handler = m_parent.ShowStatusMessage;
                m_parent.Invoke(handler, index, info, szCheckType);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     ��ʼ���г�ʼ�����߳̽���������
        /// </summary>
        /// <param name="info">��ʾ����Ϣ</param>
        private void StartRun(int nPatCount, string szChectType)
        {
            if (m_parent == null || m_parent.IsDisposed)
                return;
            try
            {
                RunHandler handler = m_parent.HandStartRun;
                m_parent.Invoke(handler, nPatCount, szChectType);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     ֪ͨ���̲߳����ѽ�������
        /// </summary>
        /// <param name="info">��ʾ����Ϣ</param>
        private void EndRun(int nPatCount, string szCheckType)
        {
            if (m_parent == null || m_parent.IsDisposed)
                return;
            try
            {
                RunHandler handler = m_parent.EndStartRun;
                m_parent.Invoke(handler, nPatCount, szCheckType);
            }
            catch
            {
            }
        }

        private delegate void ShowMessageHandler(int index, string info, string szCheckType);

        private delegate void RunHandler(int nPatCount, string szCheckType);

        #region"��״̬����ʼ��"

        private ToolStripStatusLabel statuslblSystemInfo;
        private ToolStripStatusLabel statuslblTime;
        private IContainer components;
        private Timer CommonTimer;

        private void InitializeComponent()
        {
            components = new Container();
            statuslblSystemInfo = new ToolStripStatusLabel();
            statuslblTime = new ToolStripStatusLabel();
            CommonTimer = new Timer(components);
            SuspendLayout();
            // 
            // statuslblSystemInfo
            // 
            statuslblSystemInfo.Name = "statuslblSystemInfo";
            statuslblSystemInfo.Size = new Size(608, 17);
            statuslblSystemInfo.Spring = true;
            statuslblSystemInfo.Text = "����";
            statuslblSystemInfo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statuslblTime
            // 
            statuslblTime.AutoSize = false;
            statuslblTime.Name = "statuslblTime";
            statuslblTime.Size = new Size(200, 17);
            statuslblTime.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // CommonTimer
            // 
            CommonTimer.Enabled = true;
            CommonTimer.Interval = 500;
            CommonTimer.Tick += CommonTick_Tick;
            // 
            // MainStatusStrip
            // 
            Items.AddRange(new ToolStripItem[]
            {
                statuslblSystemInfo,
                statuslblTime
            });
            Location = new Point(0, 498);
            Size = new Size(823, 22);
            ResumeLayout(false);
        }

        #endregion
    }
}