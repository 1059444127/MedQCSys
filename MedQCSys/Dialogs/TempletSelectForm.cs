/**********************************************************
 * @FileName   : TempletEditForm.cs
 * @Description: ���Ӳ������ù���ϵͳ֮ģ��ѡ�񴰿�.
 * @Author     : ������(YangMingkun)
 * @Date       : 2010-11-29 10:23
 * @History    : 
 * @Copyright  : ��Ȩ����(C)�㽭���ʿƼ��ɷ����޹�˾
***********************************************************/
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

namespace MedQCSys.Dialogs
{
    public partial class TempletSelectForm : HerenForm
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

        private string m_szDocRight = null;
        /// <summary>
        /// ��ȡ�����õ�ǰ�ĵ�����Ӧ��Ȩ��
        /// </summary>
        [Browsable(false)]
        public string DocRight
        {
            get { return this.m_szDocRight; }
            set { this.m_szDocRight = value; }
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
        /// ע��:���ж��Ĭ��ʱ,ʹ�÷ֺŷָ�
        /// </summary>
        [Browsable(false)]
        public string DefaultDocTypeID
        {
            get { return this.m_szDefaultDocTypeID; }
            set { this.m_szDefaultDocTypeID = value; }
        }

        private List<DocTypeInfo> m_lstSelectedDocTypes = null;
        /// <summary>
        /// ��ȡ�������Ѿ���ѡ�Ĳ�������
        /// </summary>
        [Browsable(false)]
        public List<DocTypeInfo> SelectedDocTypes
        {
            get { return this.m_lstSelectedDocTypes; }
        }

        public TempletSelectForm()
        {
            this.InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();
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
            this.LoadDocTypeList();
            this.SetDefaultSelectedNodes();
        }

        /// <summary>
        /// ���ز���������Ϣ�б���
        /// </summary>
        private void LoadDocTypeList()
        {
            this.treeView1.Nodes.Clear();
            if (this.m_htDocTypeNodes == null)
                this.m_htDocTypeNodes = new Hashtable();
            this.m_htDocTypeNodes.Clear();
            this.Update();

            List<DocTypeInfo> lstDocTypeInfos = null;
            short result = EMRDBLib.DbAccess.DocTypeAccess.Instance.GetDocTypeInfos(ref lstDocTypeInfos);
            if (lstDocTypeInfos == null || lstDocTypeInfos.Count <= 0)
                return;
            Hashtable htHostDocType = new Hashtable();

            //�������ĵ�����
            int index = 0;
            while (index < lstDocTypeInfos.Count)
            {
                DocTypeInfo docTypeInfo = lstDocTypeInfos[index];
                if (docTypeInfo == null || !docTypeInfo.IsHostType)
                {
                    index++;
                    continue;
                }
                if (this.m_szApplyEnv != null && this.m_szDocRight != null)
                {
                    if (docTypeInfo.ApplyEvn != this.m_szApplyEnv
                        || docTypeInfo.DocRight != this.m_szDocRight)
                    {
                        index++;
                        continue;
                    }
                }
                TreeNode docTypeNode = new TreeNode();
                docTypeNode.Text = docTypeInfo.DocTypeName;
                docTypeNode.Tag = docTypeInfo;
                if (!docTypeInfo.CanCreate)
                    docTypeNode.ForeColor = Color.Gray;
                this.treeView1.Nodes.Add(docTypeNode);

                if (!htHostDocType.Contains(docTypeInfo.DocTypeID))
                    htHostDocType.Add(docTypeInfo.DocTypeID, docTypeNode);
                if (!this.m_htDocTypeNodes.Contains(docTypeInfo.DocTypeID))
                    this.m_htDocTypeNodes.Add(docTypeInfo.DocTypeID, docTypeNode);
                lstDocTypeInfos.RemoveAt(index);
            }

            //�������ĵ�����
            for (index = 0; index < lstDocTypeInfos.Count; index++)
            {
                DocTypeInfo docTypeInfo = lstDocTypeInfos[index];
                if (docTypeInfo == null)
                    continue;
                if (this.m_szApplyEnv != null && this.m_szDocRight != null)
                {
                    if (docTypeInfo.ApplyEvn != this.m_szApplyEnv)
                        continue;
                    if (docTypeInfo.DocRight != this.m_szDocRight)
                        continue;
                }
                TreeNode docTypeNode = new TreeNode();
                docTypeNode.Tag = docTypeInfo;
                docTypeNode.Text = docTypeInfo.DocTypeName;
                if (!docTypeInfo.CanCreate)
                    docTypeNode.ForeColor = Color.Gray;

                TreeNode hostDocTypeNode = htHostDocType[docTypeInfo.HostTypeID] as TreeNode;
                if (hostDocTypeNode == null)
                    this.treeView1.Nodes.Add(docTypeNode);
                else
                    hostDocTypeNode.Nodes.Add(docTypeNode);
                if (!this.m_htDocTypeNodes.Contains(docTypeInfo.DocTypeID))
                    this.m_htDocTypeNodes.Add(docTypeInfo.DocTypeID, docTypeNode);
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
            //֧�ֶ����������
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
        private List<DocTypeInfo> GetSelectedDocTypeList()
        {
            List<DocTypeInfo> lstDocTypeInfos = new List<DocTypeInfo>();
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode selectedNode = this.treeView1.SelectedNode;
                if (selectedNode == null)
                    return lstDocTypeInfos;
                DocTypeInfo docTypeInfo = selectedNode.Tag as DocTypeInfo;
                if (docTypeInfo == null)
                    return lstDocTypeInfos;
                lstDocTypeInfos.Add(docTypeInfo);
                return lstDocTypeInfos;
            }
            for (int parentIndex = 0; parentIndex < this.treeView1.Nodes.Count; parentIndex++)
            {
                TreeNode parentNode = this.treeView1.Nodes[parentIndex];
                if (parentNode.Checked)
                    lstDocTypeInfos.Add(parentNode.Tag as DocTypeInfo);
                for (int childIndex = 0; childIndex < parentNode.Nodes.Count; childIndex++)
                {
                    TreeNode childNode = parentNode.Nodes[childIndex];
                    if (childNode.Checked)
                        lstDocTypeInfos.Add(childNode.Tag as DocTypeInfo);
                }
            }
            return lstDocTypeInfos;
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
            this.m_lstSelectedDocTypes = this.GetSelectedDocTypeList();
            this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.Default);
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
            this.m_lstSelectedDocTypes = this.GetSelectedDocTypeList();
            if (this.m_lstSelectedDocTypes != null && this.m_lstSelectedDocTypes.Count > 0)
                this.DialogResult = DialogResult.OK;
            GlobalMethods.UI.SetCursor(this.btnOK, Cursors.Default);
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            TreeNode node = e.Node;
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode item in node.Nodes)
                {
                    item.Checked = e.Node.Checked;
                }
            }

        }
    }
}