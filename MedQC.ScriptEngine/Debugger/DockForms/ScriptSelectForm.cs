// ***********************************************************
// ���������ù���ϵͳ,��ģ��ѡ��Ի���.
// ��Ҫ���ڵ����͵����ģ�幦����,ѡ�񵼳�������Щģ��
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

namespace Heren.MedQC.ScriptEngine.Debugger
{
    internal partial class ScriptSelectForm : HerenForm
    {
        private Hashtable m_htDocTypeNodes = null;
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

        private string m_szApplyEnv = null;

        /// <summary>
        /// ��ȡ�����õ�ǰ�ĵ�����Ӧ�û���
        /// </summary>
        [Browsable(false)]
        public string ApplyEnv
        {
            get { return this.m_szApplyEnv; }
            set { this.m_szApplyEnv = value; }
        }

        private string m_szDefaultDocTypeID = null;

        /// <summary>
        /// ��ȡ�����õ�ǰĬ��ѡ�е��ĵ�����.
        /// ע�⵱�ж��Ĭ��ʱ,����ʹ�÷ֺŷָ�
        /// </summary>
        [Browsable(false)]
        public string DefaultDocTypeID
        {
            get { return this.m_szDefaultDocTypeID; }
            set { this.m_szDefaultDocTypeID = value; }
        }

        private List<ScriptConfig> m_lstSelectedScriptConfigs = null;

        /// <summary>
        /// ��ȡ�������Ѿ���ѡ�Ĳ�������
        /// </summary>
        [Browsable(false)]
        public List<ScriptConfig> SelectedScriptConfigs
        {
            get { return this.m_lstSelectedScriptConfigs; }
        }

        public ScriptSelectForm()
        {
            this.InitializeComponent();
            
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
                this.LoadDocTypeList(this.m_szApplyEnv);
            this.SetDefaultSelectedNodes();
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// ���ز���������Ϣ�б���
        /// </summary>
        private void LoadDocTypeList(string szApplyEnv)
        {
            this.treeView1.Nodes.Clear();
            if (this.m_htDocTypeNodes == null)
                this.m_htDocTypeNodes = new Hashtable();
            this.m_htDocTypeNodes.Clear();
            this.Update();

            List<ScriptConfig> lstDocTypeInfos = null;
            short shRet = ScriptConfigAccess.Instance.GetScriptConfigs(ref lstDocTypeInfos);
            if (shRet != SystemData.ReturnValue.OK
                &&shRet!=SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.ShowError("������ģ���б�����ʧ��!");
                return;
            }
            if (lstDocTypeInfos == null)
                return;

            //�ȴ����ڵ��ϣ�б�
            Dictionary<string, TreeNode> nodeTable = new Dictionary<string, TreeNode>();
            foreach (ScriptConfig docTypeInfo in lstDocTypeInfos)
            {
                TreeNode docTypeNode = new TreeNode();
                docTypeNode.Tag = docTypeInfo;
                docTypeNode.Text = docTypeInfo.SCRIPT_NAME;
                

                if (docTypeInfo.IS_FOLDER==0)
                    docTypeNode.ImageIndex = 2;
                else
                    docTypeNode.ImageIndex = 0;
                docTypeNode.SelectedImageIndex = docTypeNode.ImageIndex;

                if (!nodeTable.ContainsKey(docTypeInfo.SCRIPT_ID))
                    nodeTable.Add(docTypeInfo.SCRIPT_ID, docTypeNode);
                if (!this.m_htDocTypeNodes.Contains(docTypeInfo.SCRIPT_ID))
                    this.m_htDocTypeNodes.Add(docTypeInfo.SCRIPT_ID, docTypeNode);
            }

            //���ڵ���������,��ӵ�����
            foreach (KeyValuePair<string, TreeNode> pair in nodeTable)
            {
                if (string.IsNullOrEmpty(pair.Key) || pair.Value == null)
                    continue;

                ScriptConfig docTypeInfo = pair.Value.Tag as ScriptConfig;
                if (docTypeInfo == null)
                    continue;

                if (string.IsNullOrEmpty(docTypeInfo.PARENT_ID))
                {
                    this.treeView1.Nodes.Add(pair.Value);
                }
                else
                {
                    TreeNodeCollection nodes = null;
                    if (!nodeTable.ContainsKey(docTypeInfo.PARENT_ID))
                        nodes = this.treeView1.Nodes;

                    TreeNode parentNode = nodeTable[docTypeInfo.PARENT_ID];
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
            if (this.m_htDocTypeNodes == null)
                return;
            if (GlobalMethods.Misc.IsEmptyString(this.m_szDefaultDocTypeID))
                return;
            //Ĭ��ѡ������
            if (this.m_szDefaultDocTypeID.IndexOf("����") >= 0)
            {
                foreach (DictionaryEntry dic in this.m_htDocTypeNodes)
                {
                    TreeNode defaultSelectedNode = null;
                    defaultSelectedNode = dic.Value as TreeNode;
                    if (defaultSelectedNode == null)
                        return;
                    defaultSelectedNode.Checked = true;
                }
                return;
            }
            string[] lstDocTypes = this.m_szDefaultDocTypeID.Split(';');
            if (lstDocTypes == null || lstDocTypes.Length == 0)
                return;
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode defaultSelectedNode = null;
                defaultSelectedNode = this.m_htDocTypeNodes[lstDocTypes[0].Trim()] as TreeNode;
                if (defaultSelectedNode == null)
                    return;
                defaultSelectedNode.EnsureVisible();
                this.treeView1.SelectedNode = defaultSelectedNode;
            }
            for (int index = 0; index < lstDocTypes.Length; index++)
            {
                string szDocTypeID = lstDocTypes[index];
                if (!this.m_htDocTypeNodes.Contains(szDocTypeID))
                    continue;
                TreeNode defaultSelectedNode = null;
                defaultSelectedNode = this.m_htDocTypeNodes[szDocTypeID] as TreeNode;
                if (defaultSelectedNode == null)
                    return;
                defaultSelectedNode.Checked = true;
            }
            this.m_htDocTypeNodes.Clear();
        }

        /// <summary>
        /// ��ȡ��ǰTreeView�����ѹ�ѡ�Ĳ���������Ϣ�б�
        /// </summary>
        /// <returns>����������Ϣ�б�</returns>
        private List<ScriptConfig> GetSelectedDocTypeList()
        {
            List<ScriptConfig> lstDocTypeInfos = new List<ScriptConfig>();
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode selectedNode = this.treeView1.SelectedNode;
                if (selectedNode == null)
                    return lstDocTypeInfos;
                ScriptConfig docTypeInfo = selectedNode.Tag as ScriptConfig;
                if (docTypeInfo == null)
                    return lstDocTypeInfos;
                lstDocTypeInfos.Add(docTypeInfo);
                return lstDocTypeInfos;
            }
            for (int parentIndex = 0; parentIndex < this.treeView1.Nodes.Count; parentIndex++)
            {
                TreeNode parentNode = this.treeView1.Nodes[parentIndex];
                if (parentNode.Checked)
                    lstDocTypeInfos.Add(parentNode.Tag as ScriptConfig);
                if (parentNode.Nodes.Count > 0)
                    this.GetSelectedDocTypeList(parentNode, ref lstDocTypeInfos);
            }
            return lstDocTypeInfos;
        }

        private void GetSelectedDocTypeList(TreeNode parentNode, ref List<ScriptConfig> lstDocTypeInfos)
        {
            if (lstDocTypeInfos == null)
            {
                lstDocTypeInfos = new List<ScriptConfig>();
            }
            for (int nodeIndex = 0; nodeIndex < parentNode.Nodes.Count; nodeIndex++)
            {
                TreeNode tempNode = parentNode.Nodes[nodeIndex];
                if (tempNode.Checked)
                    lstDocTypeInfos.Add(tempNode.Tag as ScriptConfig);
                if (tempNode.Nodes.Count > 0)
                {
                    this.GetSelectedDocTypeList(tempNode, ref lstDocTypeInfos);
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
            this.m_lstSelectedScriptConfigs = this.GetSelectedDocTypeList();
            this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.Default);
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
            this.m_lstSelectedScriptConfigs = this.GetSelectedDocTypeList();
            if (this.m_lstSelectedScriptConfigs != null && this.m_lstSelectedScriptConfigs.Count > 0)
                this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.Default);
        }
    }
}