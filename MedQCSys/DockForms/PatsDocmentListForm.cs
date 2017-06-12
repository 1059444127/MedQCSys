// ***********************************************************
// �����ʿ�ϵͳ���в��̼�¼��ʾ����(��ͬһ����������ʾ������ߵĲ��̼�¼).
// Creator:LiChunYing  Date:2012-3-12
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.Controls.VirtualTreeView;
using Heren.Common.DockSuite;
 
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace MedQCSys.DockForms
{
    public partial class PatsDocmentListForm : DockContentBase
    {
        /// <summary>
        /// �����Ķ���Ϣ��ϣ��
        /// </summary>
        private Hashtable m_htQCActionLog = null;

        /// <summary>
        /// ���в����б��кϲ��ĵ��ڵ��ʶ
        /// </summary>
        private const string COMBIN_NODE_TAG = "COMBIN_NODE";
        /// <summary>
        /// ҽ��д�Ĳ���
        /// </summary>
        private const string DOCTOR_NODE_TAG = "DOC_NODE";
        /// <summary>
        /// �������ƽڵ�
        /// </summary>
        public const string DEPT_NODE_TAG = "DEPT_NODE";
        /// <summary>
        /// ���������ڵ�
        /// </summary>
        public const String PAT_NODE_TAG = "PAT_NODE";
        /// <summary>
        /// ��ʿд�Ĳ���
        /// </summary>
        private const string NURSE_NODE_TAG = "NUR_NODE";
        /// <summary>
        /// δ֪���͵Ĳ���
        /// </summary>
        private const string UNKNOWN_NODE_TAG = "UNKNOWN_NODE";
        /// <summary>
        /// ������Ϣ��ϣ��
        /// </summary>
        private Hashtable m_htDeptInfo = null;

        /// <summary>
        /// ҽ���Ĳ���
        /// </summary>
        private VirtualNode m_DoctorNode = null;
        /// <summary>
        /// ��ʿ�Ĳ���
        /// </summary>
        private VirtualNode m_NurseNode = null;

        private VirtualNode m_SelectedNode;
        /// <summary>
        /// ��ȡ��ǰѡ�еĽڵ�
        /// </summary>
        public VirtualNode SelectedNode
        {
            get { return this.m_SelectedNode; }
            set { this.m_SelectedNode = value; }
        }

        public PatsDocmentListForm(MainForm parent)
            : base(parent)
        {
            InitializeComponent();
            this.CloseButtonVisible = false;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
        }

        /// <summary>
        /// �����б���Ϣ�ı�ʱˢ������
        /// </summary>
        protected override void OnPatientListInfoChanged()
        {
            if (this.Pane == null || this.Pane.IsDisposed)
                return;

            if (this.Pane.ActiveContent == this && this.NeedRefreshView)
                this.OnRefreshView();
        }

        /// <summary>
        /// ���л���ĵ�ʱˢ������
        /// </summary>
        protected override void OnActiveContentChanged()
        {
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this && this.NeedRefreshView)
                this.OnRefreshView();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.virtualTree1.SuspendLayout();
            this.virtualTree1.Columns.Add(new VirtualColumn("��������", 250));
            this.virtualTree1.Columns.Add(new VirtualColumn("����ʱ��", 150, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("������", 80, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("�޸�ʱ��", 150, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("�޸���", 80, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("�ʿض�", 70, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("�ʼ�ʱ��", 150, ContentAlignment.MiddleCenter));
            this.virtualTree1.ImageList.Images.Add(global::MedQCSys.Properties.Resources.Share);
            this.virtualTree1.ImageList.Images.Add(global::MedQCSys.Properties.Resources.CombinDoc);
            this.virtualTree1.ImageList.Images.Add(global::MedQCSys.Properties.Resources.Male);
            this.virtualTree1.ImageList.Images.Add(global::MedQCSys.Properties.Resources.Female);
            this.virtualTree1.PerformLayout();
        }

        /// <summary>
        /// ˢ�»��ߵ��ξ�������Ĳ��̼�¼
        /// </summary>
        public override void OnRefreshView()
        {
            base.OnRefreshView();
            this.Update();
            if (SystemParam.Instance.PatVisitLogList == null)
                return;

            List<EMRDBLib.PatVisitInfo> lstPatVisitLog = SystemParam.Instance.PatVisitLogList;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("����ˢ�²��̼�¼�����Ժ�...");
            if (this.m_htQCActionLog == null)
                this.m_htQCActionLog = new Hashtable();
            else
                this.m_htQCActionLog.Clear();
            this.GetQCActionLogInfo();
            this.virtualTree1.SuspendLayout();
            this.virtualTree1.Tag = null;
            this.virtualTree1.Nodes.Clear();
            this.virtualTree1.PerformLayout();
            this.m_SelectedNode = null;

            this.virtualTree1.SuspendLayout();
            VirtualNode deptNode = null;
            VirtualNode patientNode = null;
            if (this.m_htDeptInfo == null)
                this.m_htDeptInfo = new Hashtable();
            this.m_htDeptInfo.Clear();
            for (int index = 0; index < lstPatVisitLog.Count; index++)
            {
                EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLog[index];
                if (patVisitLog == null)
                    continue;

                if (!this.m_htDeptInfo.ContainsKey(patVisitLog.DEPT_CODE))
                {
                    deptNode = new VirtualNode(patVisitLog.DEPT_NAME);
                    deptNode.Font = new Font("����", 11f, FontStyle.Regular);
                    deptNode.ImageIndex = 0;
                    deptNode.ForeColor = Color.Blue;
                    deptNode.Expand();
                    this.virtualTree1.Nodes.Add(deptNode);
                    this.m_htDeptInfo.Add(patVisitLog.DEPT_CODE, deptNode);
                }
                else
                {
                    deptNode = this.m_htDeptInfo[patVisitLog.DEPT_CODE] as VirtualNode;
                    if (deptNode == null)
                        continue;
                }

                string szPatName = patVisitLog.PATIENT_NAME;
                string szBedNo = null;
                if (!GlobalMethods.Misc.IsEmptyString(patVisitLog.BED_CODE))
                {
                    szBedNo = patVisitLog.BED_CODE;
                    if (szBedNo.Length < 3)
                        szBedNo = szBedNo.PadLeft(2, ' ');
                    szPatName = string.Format("{0} {1}", szBedNo, szPatName);
                }
                patientNode = new VirtualNode(szPatName);
                patientNode.Font = new Font("����", 10.5f, FontStyle.Regular);
                patientNode.Data = PAT_NODE_TAG;
                patientNode.ForeColor = Color.Blue;
                if (patVisitLog.PATIENT_SEX == "��")
                    patientNode.ImageIndex = 2;
                else
                    patientNode.ImageIndex = 3;
                this.LoadMedDocList(patVisitLog, patientNode);
                //this.LoadPastDocList(patVisitLog, patientNode);
                deptNode.Nodes.Add(patientNode);
            }
            this.virtualTree1.ExpandAll();
            this.virtualTree1.PerformLayout();
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void LoadMedDocList(EMRDBLib.PatVisitInfo patVisitLog, VirtualNode patientNode)
        {
            if (patVisitLog == null)
                return;

            string szPatientID = patVisitLog.PATIENT_ID;
            string szVisitID = patVisitLog.VISIT_ID;
            if (GlobalMethods.Misc.IsEmptyString(szPatientID) || GlobalMethods.Misc.IsEmptyString(szVisitID))
                return;

            string szVisitType = MedDocSys.DataLayer.SystemData.VisitType.IP;
            MedDocList lstDocInfos = null;
            short shRet = EmrDocAccess.Instance.GetDocInfos(szPatientID, szVisitID, szVisitType, DateTime.Now, string.Empty, ref lstDocInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet!= SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ�¸�ʽ���̼�¼ʧ�ܣ�");
                return;
            }
            if (lstDocInfos == null || lstDocInfos.Count <= 0)
                return;

            lstDocInfos.Sort();

            //�ĵ�ʱ����ʾ��ʽ
            string szDocTimeFormat = "yyyy-MM-dd HH:mm";
            this.virtualTree1.SuspendLayout();
            this.virtualTree1.Tag = lstDocInfos;

            VirtualNode lastDocRootNode = new VirtualNode();
            lastDocRootNode.Text = "ҽ���Ĳ���";
            lastDocRootNode.ImageIndex = 0;
            lastDocRootNode.Font = new Font("����", 10.5f, FontStyle.Regular);
            lastDocRootNode.Data = DOCTOR_NODE_TAG;
            lastDocRootNode.Expand();

            VirtualNode LastNurRootNode = new VirtualNode();
            LastNurRootNode.Text = "��ʿ�Ĳ���";
            LastNurRootNode.ImageIndex = 0;
            LastNurRootNode.Font = new Font("����", 10.5f, FontStyle.Regular);
            LastNurRootNode.Data = NURSE_NODE_TAG;
            LastNurRootNode.CollapseAll();

            VirtualNode otherDocRootNode = new VirtualNode("δ������Ĳ���");
            otherDocRootNode.ImageIndex = 1;
            otherDocRootNode.Font = new Font("����", 10.5f, FontStyle.Regular);
            otherDocRootNode.Data = UNKNOWN_NODE_TAG;
            otherDocRootNode.Expand();

            Hashtable htDocType = new Hashtable();

            //���������ĵ��б�ָ��������
            for (int index = 0; index < lstDocInfos.Count; index++)
            {
                MedDocInfo docInfo = lstDocInfos[index];
                if (docInfo == null)
                    continue;

                VirtualNode docInfoNode = new VirtualNode(docInfo.DOC_TITLE);
                docInfoNode.Data = docInfo;
                docInfoNode.Tag = patVisitLog;
                docInfoNode.ForeColor = Color.Black;
                docInfoNode.Font = new Font("����", 10.5f, FontStyle.Regular);

                VirtualSubItem subItem = null;
                DateTime dtDocTime = docInfo.DOC_TIME;
                dtDocTime = docInfo.DOC_TIME;
                subItem = new VirtualSubItem(dtDocTime.ToString(szDocTimeFormat));
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(docInfo.CREATOR_NAME);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(docInfo.MODIFY_TIME.ToString(szDocTimeFormat));
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(docInfo.MODIFIER_NAME);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                string szReadTime = string.Empty;
                if (this.m_htQCActionLog.Contains(docInfo.DOC_SETID))
                    szReadTime = this.m_htQCActionLog[docInfo.DOC_SETID].ToString();

                subItem = new VirtualSubItem();
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                if (!string.IsNullOrEmpty(szReadTime))
                    subItem.Text = "��";
                else
                    subItem.Text = "��";
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(szReadTime);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);
                //����������Ķ������������������ɫ��Ϊ��ɫ
                if (!string.IsNullOrEmpty(szReadTime))
                {

                    docInfoNode.ForeColor = Color.Green;
                    for (int nIndex = 0; nIndex < docInfoNode.SubItems.Count; nIndex++)
                    {
                        docInfoNode.SubItems[nIndex].ForeColor = Color.Green;
                    }
                }
                 DocTypeInfo currDocType = null;
                DocTypeAccess.Instance.GetDocTypeInfo(docInfo.DOC_TYPE, ref currDocType);
                if (currDocType == null)
                {
                    otherDocRootNode.Nodes.Add(docInfoNode);
                    continue;
                }
                 DocTypeInfo hostDocType = null;
                DocTypeAccess.Instance.GetDocTypeInfo(currDocType.HostTypeID, ref hostDocType);
                if (hostDocType == null)
                {
                    otherDocRootNode.Nodes.Add(docInfoNode);
                    continue;
                }

                VirtualNode hostDocRootNode = null;
                if (!htDocType.Contains(hostDocType.DocTypeID))
                {
                    hostDocRootNode = new VirtualNode();
                    hostDocRootNode.Text = hostDocType.DocTypeName;
                    hostDocRootNode.Tag = hostDocType.DocTypeName;
                    hostDocRootNode.Data = COMBIN_NODE_TAG;
                    hostDocRootNode.HitExpand = HitExpandMode.Click;
                    hostDocRootNode.Expand();
                    hostDocRootNode.Font = new Font("����", 10.5f, FontStyle.Regular);
                    hostDocRootNode.ImageIndex = 1;
                    if (hostDocType.DocRight != MedDocSys.DataLayer.SystemData.UserType.NURSE)
                    {
                        lastDocRootNode.Nodes.Add(hostDocRootNode);
                    }
                    else if (hostDocType.DocRight == MedDocSys.DataLayer.SystemData.UserType.NURSE)
                    {
                        LastNurRootNode.Nodes.Add(hostDocRootNode);
                    }
                    else
                    {
                        this.virtualTree1.Nodes.Add(hostDocRootNode);
                    }
                    htDocType.Add(hostDocType.DocTypeID, hostDocRootNode);
                }
                else
                {
                    hostDocRootNode = htDocType[hostDocType.DocTypeID] as VirtualNode;
                }
                hostDocRootNode.Nodes.Add(docInfoNode);
            }
            htDocType.Clear();
            this.m_DoctorNode = lastDocRootNode;
            this.m_NurseNode = LastNurRootNode;
            if (otherDocRootNode.Nodes.Count > 0)
                patientNode.Nodes.Add(otherDocRootNode);
            if (lastDocRootNode.Nodes.Count > 0)
                patientNode.Nodes.Add(lastDocRootNode);
            if (LastNurRootNode.Nodes.Count > 0)
                patientNode.Nodes.Add(LastNurRootNode);
        }

        private void LoadPastDocList(EMRDBLib.PatVisitInfo patVisitLog, VirtualNode patientNode)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed || patVisitLog == null)
                return;

            string szPatientID = patVisitLog.PATIENT_ID;
            string szVisitID = patVisitLog.VISIT_ID;
            if (GlobalMethods.Misc.IsEmptyString(szPatientID) || GlobalMethods.Misc.IsEmptyString(szVisitID))
                return;

            MedDocList lstPastDocInfos = null;
            short shRet = EMRDBAccess.Instance.GetPastDocList(szPatientID, szVisitID, ref lstPastDocInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet!=SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡ�ɲ��̼�¼�б�ʧ�ܣ�");
                return;
            }
            if (lstPastDocInfos == null || lstPastDocInfos.Count <= 0)
                return;
            lstPastDocInfos.Sort();

            //�ĵ�ʱ����ʾ��ʽ
            string szDocTimeFormat = "yyyy-MM-dd HH:mm";
            VirtualNode oldDocRootNode = new VirtualNode("�����ɲ���");
            oldDocRootNode.Data = lstPastDocInfos;
            oldDocRootNode.ImageIndex = 1;
            oldDocRootNode.Font = new Font("����", 10.5f, FontStyle.Regular);
            oldDocRootNode.Expand();

            Hashtable htDocType = new Hashtable();

            //���������ĵ��б�ָ��������
            for (int index = 0; index < lstPastDocInfos.Count; index++)
            {
                 MedDocInfo pastDocInfo = lstPastDocInfos[index];
                if (pastDocInfo == null)
                    continue;

                string szDocTitle = "������";
                if (!GlobalMethods.Misc.IsEmptyString(pastDocInfo.DOC_TITLE))
                    szDocTitle = pastDocInfo.DOC_TITLE;
                VirtualNode docInfoNode = new VirtualNode(szDocTitle);
                docInfoNode.Font = new Font("����", 10.5f, FontStyle.Regular);
                docInfoNode.Data = pastDocInfo;
                docInfoNode.Tag = patVisitLog;
                docInfoNode.ForeColor = Color.Black;

                VirtualSubItem subItem = null;
                subItem = new VirtualSubItem(pastDocInfo.DOC_TIME.ToString(szDocTimeFormat));
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(pastDocInfo.CREATOR_NAME);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(pastDocInfo.MODIFY_TIME.ToString(szDocTimeFormat));
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(pastDocInfo.MODIFIER_NAME);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);

                string szReadTime = string.Empty;
                if (this.m_htQCActionLog.Contains(pastDocInfo.DOC_SETID))
                    szReadTime = this.m_htQCActionLog[pastDocInfo.DOC_SETID].ToString();
                subItem = new VirtualSubItem();
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                if (!string.IsNullOrEmpty(szReadTime))
                    subItem.Text = "��";
                else
                    subItem.Text = "��";
                docInfoNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(szReadTime);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                docInfoNode.SubItems.Add(subItem);
                //����������Ķ������������������ɫ��Ϊ��ɫ
                if (!string.IsNullOrEmpty(szReadTime))
                {
                    docInfoNode.ForeColor = Color.Green;
                    for (int nIndex = 0; nIndex < docInfoNode.SubItems.Count; nIndex++)
                    {
                        docInfoNode.SubItems[nIndex].ForeColor = Color.Green;
                    }
                }
                oldDocRootNode.Nodes.Add(docInfoNode);
            }
            patientNode.Nodes.Add(oldDocRootNode);
        }

        /// <summary>
        /// ��ȡѡ�нڵ�Ĳ����ĵ��ı�����ĵ���ID
        /// </summary>
        /// <param name="selectedNode">��ǰ�ڵ�</param>
        /// <param name="szDocSetID">�ĵ���ID</param>
        /// <param name="szDocTitle">�ĵ�����</param>
        private void GetSelectedNodeInfo(VirtualNode selectedNode, ref string szDocTitle, ref string szDocSetID
            , ref string szCreatorName,ref string szDeptCode,ref byte[] byteDocData)
        {
             MedDocInfo docInfo = selectedNode.Data as  MedDocInfo;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;

            if (docInfo != null)
            {
                szDocTitle = docInfo.DOC_TITLE;
                if (docInfo.EMR_TYPE != "BAODIAN" && docInfo.EMR_TYPE != "CHENPAD" 
                    && docInfo.EMR_TYPE != "HEREN")
                    szDocSetID = string.Empty;
                else
                    szDocSetID = docInfo.DOC_SETID;
                szCreatorName = docInfo.CREATOR_NAME;
                szDeptCode = docInfo.DEPT_CODE;
            }
            else if (selectedNode.Data.Equals(COMBIN_NODE_TAG))
                szDocTitle = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�Ĳ���");
            else if (selectedNode.Data.Equals(DOCTOR_NODE_TAG))
                szDocTitle = "ҽ��д�Ĳ���";
            else if (selectedNode.Data.Equals(NURSE_NODE_TAG))
                szDocTitle = "��ʿд�Ĳ���";
            else if (selectedNode.Data.Equals(UNKNOWN_NODE_TAG))
                szDocTitle = "δ������Ĳ���";
            else
                szDocTitle = string.Concat(SystemParam.Instance.PatVisitInfo.PATIENT_NAME, "�������ɲ���");
            EmrDocAccess.Instance.GetDocByID(docInfo.DOC_ID, ref byteDocData);
        }

        ///<summary>
        /// ��¼�ʿ����Ķ�������־��Ϣ
        /// </summary>
        /// <param name="lstDocInfo">�����ĵ���Ϣ�б�</param>
        /// <param name="dtCheckTime">���ʱ��</param>
        /// <param name="node">��ǰ�ڵ�</param>
        private void SaveReadLogInfo( MedDocInfo docInfo, DateTime dtCheckTime, VirtualNode node)
        {
            if (string.IsNullOrEmpty(docInfo.DOC_SETID))
                return;
            EMRDBLib.PatVisitInfo patVisitLog = node.Tag as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;

            SystemParam.Instance.PatVisitInfo = patVisitLog;

            EMRDBLib.MedicalQcLog qcActionLog = new EMRDBLib.MedicalQcLog();
            qcActionLog.PATIENT_ID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            qcActionLog.VISIT_ID = SystemParam.Instance.PatVisitInfo.VISIT_ID;
            qcActionLog.DOC_SETID = docInfo.DOC_SETID;
            qcActionLog.CHECKED_BY = SystemParam.Instance.UserInfo.Name;
            qcActionLog.CHECKED_ID = SystemParam.Instance.UserInfo.ID;
            qcActionLog.DEPT_CODE = SystemParam.Instance.UserInfo.DeptCode;
            qcActionLog.DEPT_NAME = SystemParam.Instance.UserInfo.DeptName;
            qcActionLog.CHECK_TYPE = 0;
            qcActionLog.CHECK_DATE = dtCheckTime;
            qcActionLog.LOG_TYPE = 1;
            qcActionLog.LOG_DESC = "�ʿ����Ķ�����";
            qcActionLog.AddQCQuestion = false;
            short shRet = MedicalQcLogAccess.Instance.Insert(qcActionLog);
            if (shRet != SystemData.ReturnValue.OK)
                return;

            node.SubItems[4].Text = "��";
            node.SubItems[5].Text = dtCheckTime.ToString("yyyy-MM-dd HH:mm:ss");
            node.ForeColor = Color.Green;
            for (int index = 0; index < node.SubItems.Count; index++)
            {
                node.SubItems[index].ForeColor = Color.Green;
            }
        }

        /// <summary>
        /// ��ȡ��ǰ���߱��ξ������һ�εĲ����Ķ���־��Ϣ
        /// </summary>
        private void GetQCActionLogInfo()
        {
            if (SystemParam.Instance.PatVisitLogList == null)
                return;

            List<EMRDBLib.PatVisitInfo> lstPatVisitLog = SystemParam.Instance.PatVisitLogList;
            short shRet = SystemData.ReturnValue.OK;
            int nLogType = 1;
            int nOperateType = 0;
            for (int index = 0; index < lstPatVisitLog.Count; index++)
            {
                EMRDBLib.PatVisitInfo patVisitLog = lstPatVisitLog[index];
                if (patVisitLog == null)
                    continue;

                List<EMRDBLib.MedicalQcLog> lstQCActionLog = null;
                shRet = MedicalQcLogAccess.Instance.GetQCLogInfo(patVisitLog.PATIENT_ID, patVisitLog.VISIT_ID, nLogType, nOperateType, ref lstQCActionLog);
                if (shRet != SystemData.ReturnValue.OK)
                    continue;

                if (lstQCActionLog == null || lstQCActionLog.Count <= 0)
                    continue;

                for (int nIndex = 0; nIndex < lstQCActionLog.Count; nIndex++)
                {
                    if (string.IsNullOrEmpty(lstQCActionLog[nIndex].DOC_SETID))
                        continue;

                    if (!this.m_htQCActionLog.ContainsKey(lstQCActionLog[nIndex].DOC_SETID))
                        this.m_htQCActionLog.Add(lstQCActionLog[nIndex].DOC_SETID, lstQCActionLog[nIndex].CHECK_DATE);
                }
            }
        }

        private void virtualTree1_NodeMouseClick(object sender, VirtualTreeEventArgs e)
        {
            this.m_SelectedNode = e.Node;
            EMRDBLib.PatVisitInfo patVisitLog = e.Node.Tag as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;

            if (SystemParam.Instance.PatVisitInfo != patVisitLog)
            {
                SystemParam.Instance.PatVisitInfo = patVisitLog;
                this.MainForm.OnPatientInfoChanged(EventArgs.Empty);
            }
        }

        private void virtualTree1_NodeMouseDoubleClick(object sender, VirtualTreeEventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            if (e.Node == null || e.Type == HitTestType.None)
                return;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("�������ز��򿪲��������Ժ�...");

             MedDocInfo docInfo = e.Node.Data as  MedDocInfo;
            DateTime dtCheckTime = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
            if (docInfo != null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                this.Update();
                this.MainForm.OpenDocument(docInfo);
                this.SaveReadLogInfo(docInfo, dtCheckTime, e.Node);
                this.ShowStatusMessage(null);
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                return;
            }

            //�򿪺ϲ�����
            if (COMBIN_NODE_TAG.Equals(e.Node.Data))
            {
                int count = e.Node.Nodes.Count;
                if (count <= 0)
                    return;
                GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
                 MedDocList lstDocInfos = new  MedDocList();
                for (int index = 0; index < count; index++)
                {
                    docInfo = e.Node.Nodes[index].Data as  MedDocInfo;
                    if (docInfo == null)
                        continue;

                    if (index > 0)
                        dtCheckTime = dtCheckTime.AddSeconds(1);
                    this.SaveReadLogInfo(docInfo, dtCheckTime, e.Node.Nodes[index]);
                    lstDocInfos.Add(docInfo);
                }
                this.Update();
                this.MainForm.OpenDocument(lstDocInfos);
                this.ShowStatusMessage(null);
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
            }
            else
            {
                if (DOCTOR_NODE_TAG.Equals(e.Node.Data))
                    this.OpenAllDocList(e.Node);
                else if (NURSE_NODE_TAG.Equals(e.Node.Data))
                    this.OpenAllDocList(e.Node);
                this.ShowStatusMessage(null);
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
            }
        }

        /// <summary>
        /// ��ҽ����ʿ���������в���
        /// <param name="parentNode">˫���Ľڵ�</param>
        /// </summary>
        private void OpenAllDocList(VirtualNode parentNode)
        {
            if (parentNode == null || parentNode.Nodes.Count <= 0)
                return;

             MedDocList lstDocInfos = new  MedDocList();
            DateTime dtCheckTime = MedDocSys.DataLayer.SysTimeHelper.Instance.Now;
            for (int index = 0; index < parentNode.Nodes.Count; index++)
            {
                if (!parentNode.Nodes[index].Data.Equals(COMBIN_NODE_TAG))
                    continue;

                if (index > 0)
                    dtCheckTime = dtCheckTime.AddSeconds(1);
                for (int ii = 0; ii < parentNode.Nodes[index].Nodes.Count; ii++)
                {
                    VirtualNode childNode = parentNode.Nodes[index].Nodes[ii];
                     MedDocInfo docInfo = childNode.Data as  MedDocInfo;
                    if (docInfo == null)
                        continue;

                    if (ii > 0)
                        dtCheckTime = dtCheckTime.AddSeconds(1);
                    this.SaveReadLogInfo(docInfo, dtCheckTime, childNode);
                    lstDocInfos.Add(docInfo);
                }
            }
            this.Update();
            this.MainForm.OpenDocument(lstDocInfos);
        }

        private void mnuAddFeedInfo_Click(object sender, EventArgs e)
        {
            if (this.MainForm == null || this.MainForm.IsDisposed)
                return;
            
            VirtualNode selectedNode = this.virtualTree1.SelectedNode;
            if (selectedNode == null)
                return;
            EMRDBLib.PatVisitInfo patVisitLog = selectedNode.Tag as EMRDBLib.PatVisitInfo;
            if (patVisitLog == null)
                return;
            SystemParam.Instance.PatVisitInfo = patVisitLog;
            string szDocTitle = string.Empty;
            string szDocSetID = string.Empty;
            string szDocCreator = string.Empty;
            string szDeptCode = string.Empty;
            byte[] byteDocData = null;
            this.GetSelectedNodeInfo(selectedNode, ref szDocTitle, ref szDocSetID, ref szDocCreator,ref szDeptCode, ref byteDocData);
            this.MainForm.AddFeedBackInfo(szDocTitle, szDocSetID, szDocCreator,szDeptCode, byteDocData);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

    }
}