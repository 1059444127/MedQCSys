// ***********************************************************
// �����ʿ�ϵͳ���в��̼�¼��ʾ����.
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
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.Controls.VirtualTreeView;
using Heren.Common.DockSuite;

using EMRDBLib;
using EMRDBLib.DbAccess;
using MedQCSys.DockForms;
using MedQCSys;

namespace Heren.MedQC.Systems
{
    internal partial class TempletStatForm : DockContentBase
    {
        private Hashtable m_htDocTypeList = null;
        public TempletStatForm(MainForm parent)
            : base(parent)
        {
            InitializeComponent();
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.virtualTree1.SuspendLayout();
            this.virtualTree1.Columns.Add(new VirtualColumn("ģ������", 260, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("��������", 150, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("����", 120, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("�޸�ʱ��", 180, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("����", 100, ContentAlignment.MiddleCenter));
            this.virtualTree1.Columns.Add(new VirtualColumn("���", 100, ContentAlignment.MiddleCenter));
            this.virtualTree1.PerformLayout();
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

        /// <summary>
        /// 
        /// </summary>
        public override void OnRefreshView()
        {

            base.OnRefreshView();        
            this.Update();
            if (this.virtualTree1.Nodes.Count > 0)
                this.virtualTree1.Nodes.Clear();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            this.ShowStatusMessage("���ڼ����ĵ������б����Ժ�...");
            this.LoadDocTypeList();
            this.ShowStatusMessage("���ڼ��ش����ģ���б����Ժ�...");
            List< TempletInfo> lstTempletInfo = null;
            short shRet =TempletAccess.Instance.GetUserTempletInfos("0", ref lstTempletInfo);
            if (shRet != SystemData.ReturnValue.OK )
            {
                MessageBoxEx.Show("��ȡ�����ģ���б�ʧ��");
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage(null);
                return;
            }
            if (lstTempletInfo == null)
            {
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage("δ�ҵ�����˵�ģ��");
                return;
            }
            //��ѯҽ��������ȷ�ϵ�ģ���б�
            List< TempletInfo> lstModifyTempletInfos = null;
            shRet =TempletAccess.Instance.GetUserTempletInfos("2", ref lstModifyTempletInfos);
            if (shRet != SystemData.ReturnValue.OK && shRet !=  SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("��ȡҽ����ȷ�����������ģ���б�ʧ��");
                GlobalMethods.UI.SetCursor(this, Cursors.Default);
                this.ShowStatusMessage(null);
                return;
            }
            if (lstModifyTempletInfos != null)
                lstTempletInfo.AddRange(lstModifyTempletInfos);
            lstTempletInfo.Sort(new Comparison< TempletInfo>(this.Compare));
            //ģ��ʱ����ʾ��ʽ
            string szDocTimeFormat = "yyyy-MM-dd HH:mm";
            string szDeptCode = string.Empty;
            VirtualNode deptNode = null;
            for (int index = 0; index < lstTempletInfo.Count; index++)
            {
                 TempletInfo templetInfo = lstTempletInfo[index];
                if (templetInfo == null)
                    continue;
                if (templetInfo.IsFolder)
                    continue;
                //��ӿ���������ʾ��
                if (templetInfo.DeptCode != szDeptCode)
                {
                    deptNode = new VirtualNode(templetInfo.DeptName);
                    deptNode.ForeColor = Color.Blue;
                    deptNode.Font = new Font("����", 10.5f, FontStyle.Regular);
                    this.virtualTree1.Nodes.Add(deptNode);
                    deptNode.Expand();
                }
                szDeptCode = templetInfo.DeptCode;
                VirtualNode templetNode = new VirtualNode(templetInfo.TempletName);
                templetNode.Data = templetInfo;
                templetNode.ForeColor = Color.Black;
                templetNode.Font = new Font("����", 10.5f, FontStyle.Regular);

                string szDocTypeName = string.Empty;
                 DocTypeInfo docTypeInfo = this.m_htDocTypeList[templetInfo.DocTypeID] as  DocTypeInfo;
                if (docTypeInfo == null)
                    szDocTypeName = templetInfo.DocTypeID;
                else
                    szDocTypeName = docTypeInfo.DocTypeName;
                VirtualSubItem subItem = null;
                subItem = new VirtualSubItem(szDocTypeName);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                templetNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(templetInfo.CreatorName);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                templetNode.SubItems.Add(subItem);

                subItem = new VirtualSubItem(templetInfo.ModifyTime.ToString(szDocTimeFormat));
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                templetNode.SubItems.Add(subItem);

                string szShareLevel = string.Empty;
                if (templetInfo.ShareLevel ==  SystemData.ShareLevel.HOSPITAL)
                    szShareLevel = "ȫԺ";
                else if (templetInfo.ShareLevel ==  SystemData.ShareLevel.DEPART)
                    szShareLevel = "����";
                else
                    szShareLevel = "����";
                subItem = new VirtualSubItem(szShareLevel);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                templetNode.SubItems.Add(subItem);

                string szCheckStatus = string.Empty;
                if (templetInfo.CheckStatus ==  TempletCheckStatus.None)
                    szCheckStatus = "δ���";
                else if (templetInfo.CheckStatus ==  TempletCheckStatus.Affirm)
                    szCheckStatus = "��ȷ��";
                subItem = new VirtualSubItem(szCheckStatus);
                subItem.Font = new Font("����", 10.5f, FontStyle.Regular);
                subItem.Alignment = Alignment.Middle;
                templetNode.SubItems.Add(subItem);
                deptNode.Nodes.Add(templetNode);
            }
            this.ShowStatusMessage(null);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        /// <summary>
        /// ��ָ��������ģ����Ϣ�����������
        /// </summary>
        /// <param name="templetInfo1">ģ����Ϣ����1</param>
        /// <param name="templetInfo2">ģ����Ϣ����2</param>
        /// <returns>int</returns>
        private int Compare( TempletInfo templetInfo1,  TempletInfo templetInfo2)
        {
            if (templetInfo1 == null && templetInfo2 != null)
                return -1;
            if (templetInfo1 != null && templetInfo2 == null)
                return 1;
            if (templetInfo1 == null && templetInfo2 == null)
                return 0;
            return string.Compare(templetInfo1.DeptCode, templetInfo2.DeptCode);
        }

        /// <summary>
        /// װ���ĵ������б�
        /// </summary>
        private void LoadDocTypeList()
        {
            if (this.m_htDocTypeList == null)
                this.m_htDocTypeList = new Hashtable();
            this.m_htDocTypeList.Clear();

            List< DocTypeInfo> lstDocTypeInfo = null;
            short shRet = DocTypeAccess.Instance.GetDocTypeInfos(ref lstDocTypeInfo);
            if (lstDocTypeInfo == null)
                return;

            //�����ĵ������б�
            for (int index = 0; index < lstDocTypeInfo.Count; index++)
            {
                 DocTypeInfo docTypeInfo = lstDocTypeInfo[index];
                if (!docTypeInfo.CanCreate)
                    continue;

                if (!this.m_htDocTypeList.Contains(docTypeInfo.DocTypeID))
                    this.m_htDocTypeList.Add(docTypeInfo.DocTypeID, docTypeInfo);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.OnRefreshView();
        }
    }
}