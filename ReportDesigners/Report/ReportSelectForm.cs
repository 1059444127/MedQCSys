// ***********************************************************
// ���������ù���ϵͳ,����ѡ��Ի���.
// ��Ҫ���ڵ����͵��뱨��ģ�幦����,ѡ�񵼳�������Щģ��
// Author : YangMingkun, Date : 2012-6-10
// Copyright : Heren Health Services Co.,Ltd.
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Designers.Report
{
    internal partial class ReportSelectForm : HerenForm
    {
        private string m_szDescription = "��ѡ��ģ�壺";

        /// <summary>
        /// ��ȡ�����õ�ǰ�Ի���������Ϣ
        /// </summary>
        [DefaultValue("��ѡ��ģ�壺")]
        public string Description
        {
            get { return this.m_szDescription; }
            set { this.m_szDescription = value; }
        }

        private bool m_bMultiSelect = true;

        /// <summary>
        /// ��ȡ�����õ�ǰ�Ƿ������ѡ
        /// </summary>
        [DefaultValue(true)]
        public bool MultiSelect
        {
            get { return this.m_bMultiSelect; }
            set { this.m_bMultiSelect = value; }
        }

        private string m_szReportType = null;

        /// <summary>
        /// ��ȡ�����õ�ǰ���صı����б�����
        /// </summary>
        [Browsable(false)]
        public string ReportType
        {
            get { return this.m_szReportType; }
            set { this.m_szReportType = value; }
        }

        private string m_szDefaultTempletID = null;

        /// <summary>
        /// ��ȡ�����õ�ǰĬ��ѡ�е�ģ����.
        /// ע�⵱�ж��Ĭ��ʱ,����ʹ�÷ֺŷָ�
        /// </summary>
        [Browsable(false)]
        public string DefaultTempletID
        {
            get { return this.m_szDefaultTempletID; }
            set { this.m_szDefaultTempletID = value; }
        }

        private List<ReportType> m_lstSelectedTemplets = null;

        /// <summary>
        /// ��ȡ�������Ѿ���ѡ�Ĳ�������
        /// </summary>
        [Browsable(false)]
        public List<ReportType> SelectedTemplets
        {
            get { return this.m_lstSelectedTemplets; }
        }

        private Hashtable m_htTempletNodes = null;

        public ReportSelectForm()
        {
            this.InitializeComponent();

            string[] templetTypes = SystemData.ReportTypeApplyEnv.GetTypeNames();
            this.cboReportType.Items.AddRange(templetTypes);
            this.cboReportType.Items.Add("<���б�������>");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_bMultiSelect)
            {
                this.treeView1.CheckBoxes = true;
                this.treeView1.FullRowSelect = false;
                this.chkCheckAll.Visible = true;
            }
            else
            {
                this.treeView1.CheckBoxes = false;
                this.treeView1.FullRowSelect = true;
                this.chkCheckAll.Visible = false;
            }
            this.lblTipInfo.Text = this.m_szDescription;
            if (!GlobalMethods.Misc.IsEmptyString(this.m_szReportType))
                this.LoadReportTempletList(this.m_szReportType);
            this.treeView1.Focus();

            this.SetDefaultSelectedNodes();

            this.cboReportType.Text =
                SystemData.ReportTypeApplyEnv.GetTypeName(this.m_szReportType);
            this.cboReportType.SelectedIndexChanged +=
                new EventHandler(this.cboReportType_SelectedIndexChanged);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// ���ز���������Ϣ�б���
        /// </summary>
        private void LoadReportTempletList(string szReportType)
        {
            this.treeView1.Nodes.Clear();
            if (this.m_htTempletNodes == null)
                this.m_htTempletNodes = new Hashtable();
            this.m_htTempletNodes.Clear();
            this.Update();

            List<ReportType> lstReportTypeInfos = null;
            short shRet =ReportTypeAccess.Instance.GetReportTypes(szReportType, ref lstReportTypeInfos);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowError("����ģ���б�����ʧ��!");
                return;
            }
            if (lstReportTypeInfos == null)
                return;

            //�ȴ����ڵ��ϣ�б�
            Dictionary<string, TreeNode> nodeTable = new Dictionary<string, TreeNode>();
            foreach (ReportType reportTypeInfo in lstReportTypeInfos)
            {
                TreeNode docTypeNode = new TreeNode();
                docTypeNode.Tag = reportTypeInfo;
                docTypeNode.Text = reportTypeInfo.ReportTypeName;

                if (!reportTypeInfo.IsValid)
                    docTypeNode.ForeColor = Color.Silver;

                if (!reportTypeInfo.IsFolder)
                    docTypeNode.ImageIndex = 2;
                else
                    docTypeNode.ImageIndex = 0;
                docTypeNode.SelectedImageIndex = docTypeNode.ImageIndex;

                if (!nodeTable.ContainsKey(reportTypeInfo.ReportTypeID))
                    nodeTable.Add(reportTypeInfo.ReportTypeID, docTypeNode);
                if (!this.m_htTempletNodes.Contains(reportTypeInfo.ReportTypeID))
                    this.m_htTempletNodes.Add(reportTypeInfo.ReportTypeID, docTypeNode);
            }

            //���ڵ���������,��ӵ�����
            foreach (KeyValuePair<string, TreeNode> pair in nodeTable)
            {
                if (string.IsNullOrEmpty(pair.Key) || pair.Value == null)
                    continue;

                ReportType reportTypeInfo = pair.Value.Tag as ReportType;
                if (reportTypeInfo == null)
                    continue;

                if (string.IsNullOrEmpty(reportTypeInfo.ParentID))
                {
                    this.treeView1.Nodes.Add(pair.Value);
                }
                else
                {
                    TreeNodeCollection nodes = null;
                    if (!nodeTable.ContainsKey(reportTypeInfo.ParentID))
                        nodes = this.treeView1.Nodes;

                    TreeNode parentNode = nodeTable[reportTypeInfo.ParentID];
                    if (parentNode != null)
                        parentNode.Nodes.Add(pair.Value);
                    else
                        this.treeView1.Nodes.Add(pair.Value);
                }
            }
            this.treeView1.ExpandAll();
            if (this.treeView1.Nodes.Count > 0) this.treeView1.Nodes[0].EnsureVisible();
        }

        /// <summary>
        /// ��ʾĬ��ѡ�еĽڵ�
        /// </summary>
        private void SetDefaultSelectedNodes()
        {
            if (this.m_htTempletNodes == null)
                return;
            if (GlobalMethods.Misc.IsEmptyString(this.m_szDefaultTempletID))
                return;
            //Ĭ��ѡ������
            if (this.m_szDefaultTempletID.IndexOf("����") >= 0)
            {
                foreach (DictionaryEntry dic in this.m_htTempletNodes)
                {
                    TreeNode defaultSelectedNode = null;
                    defaultSelectedNode = dic.Value as TreeNode;
                    if (defaultSelectedNode == null)
                        return;
                    defaultSelectedNode.Checked = true;
                }
                return;
            }
            string[] lstTempletIDs = this.m_szDefaultTempletID.Split(';');
            if (lstTempletIDs == null || lstTempletIDs.Length == 0)
                return;
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode defaultSelectedNode = null;
                defaultSelectedNode = this.m_htTempletNodes[lstTempletIDs[0].Trim()] as TreeNode;
                if (defaultSelectedNode == null)
                    return;
                defaultSelectedNode.EnsureVisible();
                this.treeView1.SelectedNode = defaultSelectedNode;
            }
            for (int index = 0; index < lstTempletIDs.Length; index++)
            {
                string szTempletID = lstTempletIDs[index];
                if (!this.m_htTempletNodes.Contains(szTempletID))
                    continue;
                TreeNode defaultSelectedNode = null;
                defaultSelectedNode = this.m_htTempletNodes[szTempletID] as TreeNode;
                if (defaultSelectedNode == null)
                    return;
                defaultSelectedNode.Checked = true;
            }
            this.m_htTempletNodes.Clear();
        }

        /// <summary>
        /// ��ȡ��ǰTreeView�����ѹ�ѡ�ı���ģ����Ϣ�б�
        /// </summary>
        /// <returns>����ģ����Ϣ�б�</returns>
        private List<ReportType> GetSelectedTempletList()
        {
            List<ReportType> lstReportTemplets = new List<ReportType>();
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode selectedNode = this.treeView1.SelectedNode;
                if (selectedNode == null)
                    return lstReportTemplets;
                ReportType reportTemplet = selectedNode.Tag as ReportType;
                if (reportTemplet == null)
                    return lstReportTemplets;
                lstReportTemplets.Add(reportTemplet);
                return lstReportTemplets;
            }
            for (int parentIndex = 0; parentIndex < this.treeView1.Nodes.Count; parentIndex++)
            {
                TreeNode parentNode = this.treeView1.Nodes[parentIndex];
                if (parentNode.Checked)
                    lstReportTemplets.Add(parentNode.Tag as ReportType);
                if (parentNode.Nodes.Count > 0)
                    this.GetSelectedTempletList(parentNode, ref lstReportTemplets);
            }
            return lstReportTemplets;
        }

        private void GetSelectedTempletList(TreeNode parentNode, ref List<ReportType> lstReportTemplets)
        {
            if (lstReportTemplets == null)
            {
                lstReportTemplets = new List<ReportType>();
            }
            for (int nodeIndex = 0; nodeIndex < parentNode.Nodes.Count; nodeIndex++)
            {
                TreeNode tempNode = parentNode.Nodes[nodeIndex];
                if (tempNode.Checked)
                    lstReportTemplets.Add(tempNode.Tag as ReportType);
                if (tempNode.Nodes.Count > 0)
                {
                    this.GetSelectedTempletList(tempNode, ref lstReportTemplets);
                }
            }
        }

        private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int parentIndex = 0; parentIndex < this.treeView1.Nodes.Count; parentIndex++)
            {
                TreeNode parentNode = this.treeView1.Nodes[parentIndex];
                parentNode.Checked = this.chkCheckAll.Checked;
                for (int childIndex = 0; childIndex < parentNode.Nodes.Count; childIndex++)
                {
                    parentNode.Nodes[childIndex].Checked = this.chkCheckAll.Checked;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.WaitCursor);
            this.m_lstSelectedTemplets = this.GetSelectedTempletList();
            this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.Default);
        }

        private void cboReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string szReportType = this.cboReportType.Text;
            szReportType = SystemData.ReportTypeApplyEnv.GetTypeCode(szReportType);
            this.LoadReportTempletList(szReportType);
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            foreach (TreeNode node in e.Node.Nodes)
                node.Checked = e.Node.Checked;
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.treeView1.CheckBoxes)
                return;
            TreeNode node = this.treeView1.GetNodeAt(e.X, e.Y);
            if (node == null || !node.Bounds.Contains(e.Location))
                this.treeView1.SelectedNode = null;
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.m_bMultiSelect)
                return;
            TreeNode node = this.treeView1.GetNodeAt(e.X, e.Y);
            if (node == null)
                return;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.WaitCursor);
            this.m_lstSelectedTemplets = this.GetSelectedTempletList();
            if (this.m_lstSelectedTemplets != null && this.m_lstSelectedTemplets.Count > 0)
                this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.Default);
        }
    }
}