using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Heren.Common.Controls;
using Heren.Common.Libraries;
using Heren.Common.DockSuite;

using Heren.Common.Report;
using Heren.Common.VectorEditor;
using EMRDBLib.DbAccess;
using EMRDBLib.Entity;
using EMRDBLib;
using System.IO;
using Heren.MedQC.Core;

namespace MedQCSys.DockForms
{
    public partial class PatientIndexForm : DockContentBase
    {
        private int m_nPageIndex = 1;
        public PatientIndexForm(MainForm parent)
            : base(parent)
        {
            InitializeComponent();
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.ShowHint = DockState.Document;
            this.DockAreas = DockAreas.Document | DockAreas.DockBottom | DockAreas.DockLeft
                | DockAreas.DockRight | DockAreas.DockTop;
        }

        public PatientIndexForm(MainForm parent, PatPage.PatientPageControl patientPageControl)
            : base(parent, patientPageControl)
        {
            this.InitializeComponent();
            this.ShowHint = DockState.Document;
            this.HideOnClose = true;
            this.CloseButtonVisible = true;
            this.DockAreas = DockAreas.Document | DockAreas.DockBottom | DockAreas.DockLeft
                | DockAreas.DockRight | DockAreas.DockTop;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.OnRefreshView();
            // ShowPageView();
        }
        private delegate void DispMSGDelegate();
        private void Thread_DisplayMSG()
        {
            DispMsg();
        }
        private void DispMsg()
        {
            if (this.InvokeRequired == false)                      //������øú������̺߳Ϳؼ�lstMainλ��ͬһ���߳���
            {
                //ֱ�ӽ�������ӵ�����Ŀؼ���
                this.ShowPageView();
            }
            else
            {
                //ͨ��ʹ��Invoke�ķ����������̸߳��ߴ����߳��������Ӧ�Ŀؼ�����
                DispMSGDelegate DMSGD = new DispMSGDelegate(DispMsg);

                //ʹ�ÿؼ�lstMain��Invoke����ִ��DMSGD����(��������DispMSGDelegate)
                this.Invoke(DMSGD);
            }
        }
      
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                ReportType reportTypeInfo = null;
                if (this.m_nPageIndex==1)
                    ReportTypeAccess.Instance.GetReportType(this.m_PatientIndex1.ReportTypeID, ref reportTypeInfo);
                else
                    ReportTypeAccess.Instance.GetReportType(this.m_PatientIndex2.ReportTypeID, ref reportTypeInfo);
                this.Update();
                if (reportTypeInfo == null)
                {
                    MessageBoxEx.ShowError("���µ�ģ�廹û������!");
                    return true;
                }
                byte[] byteTempletData = null;
                if (!ReportCache.Instance.GetReportTemplet(reportTypeInfo, ref byteTempletData))
                {
                    MessageBoxEx.ShowError("���µ�ģ����������ʧ��!");
                    return true;
                }
                if (this.m_nPageIndex == 1)
                    this.reportDesigner1.OpenDocument(byteTempletData);
                else
                    this.reportDesigner2.OpenDocument(byteTempletData);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private bool m_bIsShow = false;
        private ReportType m_PatientIndex1;
        private ReportType m_PatientIndex2;
        private void ShowPageView()
        {
            if (this.m_bIsShow)
                return;
            List<ReportType> lstReportTypes = ReportCache.Instance.GetReportTypeList(SystemData.ReportTypeApplyEnv.PATIENT_INDEX);
            if (lstReportTypes == null || lstReportTypes.Count <= 0)
                return;
            foreach (var item in lstReportTypes)
            {
                if (item.IsFolder || !item.IsValid)
                    continue;
                if (item.ReportTypeName == "������ҳ1"
                    && this.m_PatientIndex1 == null)
                {
                    byte[] byteTempletData = null;
                    bool result = ReportCache.Instance.GetReportTemplet(item.ReportTypeID, ref byteTempletData);
                    if (result)
                        this.reportDesigner1.OpenDocument(byteTempletData);
                    this.m_PatientIndex1 = item;
                }
                else if (item.ReportTypeName == "������ҳ2"
                    && this.m_PatientIndex2 == null)
                {
                    byte[] byteTempletData = null;
                    bool result = ReportCache.Instance.GetReportTemplet(item.ReportTypeID, ref byteTempletData);
                    if (result)
                        this.reportDesigner2.OpenDocument(byteTempletData);
                    this.m_PatientIndex2 = item;
                }
            }
            this.m_bIsShow = true;

            ShowReportDesignerIndex(this.m_nPageIndex);
        }

        public override void OnRefreshView()
        {
            this.m_bIsShow = false;
            base.OnRefreshView();
            //Thread thread = new Thread(new ThreadStart(Thread_DisplayMSG));
            //thread.Start();
            this.ShowPageView();
            this.Update();
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

        protected override void OnPatientInfoChanged()
        {
            base.OnPatientInfoChanged();
            if (this.Pane == null || this.Pane.IsDisposed)
                return;
            if (this.Pane.ActiveContent == this && this.NeedRefreshView)
                this.OnRefreshView();
        }

        private void ShowReportDesignerIndex(int pageIndex)
        {
            if (pageIndex == 1)
            {
                this.reportDesigner1.Visible = true;
                this.reportDesigner1.PerformLayout();
                this.reportDesigner2.Visible = false;
            }
            else
            {
                this.reportDesigner1.Visible = false;
                this.reportDesigner2.Visible = true;
                this.reportDesigner2.PerformLayout();
            }
        }

        private void LoadPatProfileTemplet(string szTempletPath, int pageIndex)
        {
            byte[] byteTempletData = null;
            if (!GlobalMethods.IO.GetFileBytes(szTempletPath, ref byteTempletData))
            {
                MessageBoxEx.Show("��ҳģ�����ʧ��");
                return;
            }
            if (pageIndex == 1)
            {
                this.reportDesigner1.OpenDocument(byteTempletData);
            }
            else
            {
                this.reportDesigner2.OpenDocument(byteTempletData);
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.m_nPageIndex == 1)
                this.m_nPageIndex = 2;
            else
                this.m_nPageIndex = 1;
            this.ShowReportDesignerIndex(this.m_nPageIndex);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (this.m_nPageIndex == 1)
                this.m_nPageIndex = 2;
            else
                this.m_nPageIndex = 1;
            this.ShowReportDesignerIndex(this.m_nPageIndex);
        }

        private void reportDesigner1_ExecuteQuery(object sender, Heren.Common.Report.ExecuteQueryEventArgs e)
        {
            DataSet result = null;
            if (CommonAccess.Instance.ExecuteQuery(e.SQL, out result) == SystemData.ReturnValue.OK)
            {
                e.Success = true;
                e.Result = result;
            }
        }

        private void reportDesigner1_QueryContext(object sender, Heren.Common.Report.QueryContextEventArgs e)
        {
            object value = e.Value;
            e.Success = GetSystemContext(e.Name, ref value);
            if (e.Success) e.Value = value;
        }

        private bool GetSystemContext(string name, ref object value)
        {
            if (name == "����ID��" || name == "���߱��")
            {
                if (SystemParam.Instance.PatVisitInfo == null)
                    return false;
                value = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
                return true;
            }
            else if (name == "����ID��" || name == "��Ժ��" || name == "��Ժ��")
            {
                if (SystemParam.Instance.PatVisitInfo == null)
                    return false;
                value = SystemParam.Instance.PatVisitInfo.VISIT_ID;
                return true;
            }
            else if (name.ToUpper() == "DBLINK")
            {
                value = SystemParam.Instance.LocalConfigOption.DBLINK;
                return true;
            }
            else if (name == "ҽԺ����")
            {
                value = SystemParam.Instance.LocalConfigOption.HOSPITAL_NAME;
                return true;
            }
            return false;
        }
    }
}

