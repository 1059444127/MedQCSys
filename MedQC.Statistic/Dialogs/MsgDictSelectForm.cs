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
using EMRDBLib.DbAccess;
using System.Linq;

namespace Heren.MedQC.Statistic.Dialogs
{
    public partial class MsgDictSelectForm : HerenForm
    {
        private Hashtable m_htMsgDictNodes = null;

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

        private string m_szDefaultMsgCode = null;
        /// <summary>
        /// ��ȡ�����õ�ǰĬ��ѡ�е��ĵ�����.
        /// ע��:���ж��Ĭ��ʱ,ʹ�÷ֺŷָ�
        /// </summary>
        [Browsable(false)]
        public string DefaultMsgCode
        {
            get { return this.m_szDefaultMsgCode; }
            set { this.m_szDefaultMsgCode = value; }
        }

        private List<QcMsgDict> m_lstSelectedQcMsgDicts = null;
        /// <summary>
        /// ��ȡ�������Ѿ���ѡ�Ĳ�������
        /// </summary>
        [Browsable(false)]
        public List<QcMsgDict> SelectedQcMsgDicts
        {
            get { return this.m_lstSelectedQcMsgDicts; }
        }

        public MsgDictSelectForm()
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
            this.LoadQcMsgDictList();
            this.SetDefaultSelectedNodes();
        }

        /// <summary>
        /// ���ز���������Ϣ�б���
        /// </summary>
        private void LoadQcMsgDictList()
        {
            this.treeView1.Nodes.Clear();
            if (this.m_htMsgDictNodes == null)
                this.m_htMsgDictNodes = new Hashtable();
            this.m_htMsgDictNodes.Clear();
            this.Update();
            List<QaEventTypeDict> lstQaEventTypeDict = null;
            short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQaEventTypeDict);
            List<QcMsgDict> lstQcMsgDicts = null;
            short result = QcMsgDictAccess.Instance.GetQcMsgDictList(ref lstQcMsgDicts);
            if (lstQcMsgDicts == null || lstQcMsgDicts.Count <= 0)
                return;
            Hashtable htHostDocType = new Hashtable();

            List<QaEventTypeDict> lstFirst = lstQaEventTypeDict.Where(m => string.IsNullOrEmpty(m.PARENT_CODE)).ToList();
            //������������
            foreach (var first in lstFirst)
            {
                TreeNode node = new TreeNode();
                node.Text = first.QA_EVENT_TYPE;
                node.Tag = first;
                this.treeView1.Nodes.Add(node);
                //���ض�������
                List<QaEventTypeDict> lstSecond = lstQaEventTypeDict.Where(m => m.PARENT_CODE == first.INPUT_CODE).ToList();
                if (lstSecond != null)
                {
                    foreach (var secord in lstSecond)
                    {
                        TreeNode node2 = new TreeNode();
                        node2.Text = secord.QA_EVENT_TYPE;
                        node2.Tag = secord;
                        node.Nodes.Add(node2);
                        var msgDicts = lstQcMsgDicts.Where(m => m.QA_EVENT_TYPE == first.QA_EVENT_TYPE && m.MESSAGE_TITLE == secord.QA_EVENT_TYPE).ToList();
                        if (msgDicts == null || msgDicts.Count <= 0)
                            continue;
                        foreach (var item in msgDicts)
                        {
                            TreeNode node3 = new TreeNode();
                            node3.Text = item.MESSAGE;
                            node3.Tag = item;
                            node2.Nodes.Add(node3);

                            if (!this.m_htMsgDictNodes.Contains(item.QC_MSG_CODE))
                                this.m_htMsgDictNodes.Add(item.QC_MSG_CODE, node3);
                        }
                    }
                }
                var msgDicts2 = lstQcMsgDicts.Where(m => m.QA_EVENT_TYPE == first.QA_EVENT_TYPE && m.MESSAGE_TITLE == string.Empty).ToList();
                if (msgDicts2 == null || msgDicts2.Count <= 0)
                {
                    continue;
                }
                //һ�������µ������ֵ�
                foreach (var item in msgDicts2)
                {
                    TreeNode node3 = new TreeNode();
                    node3.Text = item.MESSAGE;
                    node3.Tag = item;
                    node.Nodes.Add(node3);
                    if (!this.m_htMsgDictNodes.Contains(item.QC_MSG_CODE))
                        this.m_htMsgDictNodes.Add(item.QC_MSG_CODE, node3);
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
            if (this.m_htMsgDictNodes == null)
                return;
            if (GlobalMethods.Misc.IsEmptyString(this.m_szDefaultMsgCode))
                return;
            //Ĭ��ѡ������
            if (this.m_szDefaultMsgCode.IndexOf("����") >= 0)
            {
                foreach (DictionaryEntry dic in this.m_htMsgDictNodes)
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
            string[] lstMsgCodes = this.m_szDefaultMsgCode.Split(';');
            if (lstMsgCodes == null || lstMsgCodes.Length == 0)
                return;
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode defaultSelectedNode = null;
                defaultSelectedNode = this.m_htMsgDictNodes[lstMsgCodes[0].Trim()] as TreeNode;
                if (defaultSelectedNode == null)
                    return;
                defaultSelectedNode.EnsureVisible();
                this.treeView1.SelectedNode = defaultSelectedNode;
            }
            for (int index = 0; index < lstMsgCodes.Length; index++)
            {
                string szMsgCode = lstMsgCodes[index];
                if (!this.m_htMsgDictNodes.Contains(szMsgCode))
                    continue;
                TreeNode defaultSelectedNode = null;
                defaultSelectedNode = this.m_htMsgDictNodes[szMsgCode] as TreeNode;
                if (defaultSelectedNode == null)
                    return;
                defaultSelectedNode.Checked = true;
            }
            this.m_htMsgDictNodes.Clear();
        }

        /// <summary>
        /// ��ȡ��ǰTreeView�����ѹ�ѡ�Ĳ���������Ϣ�б�
        /// </summary>
        /// <returns>����������Ϣ�б�</returns>
        private List<QcMsgDict> GetSelectedQcMsgDictList()
        {
            List<QcMsgDict> lstQcMsgDicts = new List<QcMsgDict>();
            if (!this.treeView1.CheckBoxes)
            {
                TreeNode selectedNode = this.treeView1.SelectedNode;
                if (selectedNode == null)
                    return lstQcMsgDicts;
                QcMsgDict qcMsgDict = selectedNode.Tag as QcMsgDict;
                if (qcMsgDict == null)
                    return lstQcMsgDicts;
                lstQcMsgDicts.Add(qcMsgDict);
                return lstQcMsgDicts;
            }
            for (int parentIndex = 0; parentIndex < this.treeView1.Nodes.Count; parentIndex++)
            {
                TreeNode parentNode = this.treeView1.Nodes[parentIndex];
                for (int childIndex = 0; childIndex < parentNode.Nodes.Count; childIndex++)
                {
                    TreeNode childNode = parentNode.Nodes[childIndex];
                    if (childNode.Tag is QaEventTypeDict)
                    {
                        foreach (TreeNode item in childNode.Nodes)
                        {
                            if (item.Tag is QcMsgDict && item.Checked)
                            {
                                lstQcMsgDicts.Add(item.Tag as QcMsgDict);
                            }
                        }
                    }
                    else if (childNode.Checked)
                        lstQcMsgDicts.Add(childNode.Tag as QcMsgDict);
                }
            }
            return lstQcMsgDicts;
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
            this.m_lstSelectedQcMsgDicts = this.GetSelectedQcMsgDictList();
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
            this.m_lstSelectedQcMsgDicts = this.GetSelectedQcMsgDictList();
            if (this.m_lstSelectedQcMsgDicts != null && this.m_lstSelectedQcMsgDicts.Count > 0)
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