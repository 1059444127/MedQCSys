// ***********************************************************
// �����ʿ�ϵͳͳ�������˵��ؼ�.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EMRDBLib;
using MedQCSys.Utility;
using Heren.MedQC.Core;

namespace MedQCSys.MenuBars
{
    internal class StatisticMenu : MqsMenuItemBase
    {
        private MainForm m_mainForm = null;
        public virtual MainForm MainForm
        {
            get { return this.m_mainForm; }
            set { this.m_mainForm = value; }
        }

        #region"ͳ�Ʋ˵���ʼ��"
        private MqsMenuItemBase mnuStatByBugs;
        private MqsMenuItemBase mnuStatByBugType;
        private MqsMenuItemBase mnuStatByDept;
        private MqsMenuItemBase mnuStatByChecker;
        private MqsMenuItemBase mnuStatByWorkload;
        private MqsMenuItemBase mnuStatByQuestion;
        private MqsMenuItemBase mnuStatByModifyDocTimes;
        private MqsMenuItemBase mnuStatDocScores;
        private MqsMenuItemBase mnuStatByDeptWordload;
        private MqsMenuItemBase mnuStatByDocScoreCompare;
        private MqsMenuItemBase mnuStatByHospitalTimeCheck;
        private MqsMenuItemBase mnuStatByTimeCheck;
        private MqsMenuItemBase mnuStatByQcCheckResult;
        private MqsMenuItemBase mnuStatByYunxingQcCheckResult;
        private MqsMenuItemBase mnuStatByYunxingQcCheckResultDetail;
        private MqsMenuItemBase mnuStatByContentCheck;
        private MqsMenuItemBase mnuStatJobDescription;
        private void InitializeComponent()
        {
            this.mnuStatByBugs = new MqsMenuItemBase();
            this.mnuStatByBugType = new MqsMenuItemBase();
            this.mnuStatByDept = new MqsMenuItemBase();
            this.mnuStatByChecker = new MqsMenuItemBase();
            this.mnuStatByWorkload = new MqsMenuItemBase();
            this.mnuStatByQuestion = new MqsMenuItemBase();
            this.mnuStatByModifyDocTimes = new MqsMenuItemBase();
            this.mnuStatDocScores = new MqsMenuItemBase();
            this.mnuStatByDeptWordload = new MqsMenuItemBase();
            this.mnuStatByDocScoreCompare = new MqsMenuItemBase();
            this.mnuStatByHospitalTimeCheck = new MqsMenuItemBase();
            this.mnuStatByTimeCheck = new MqsMenuItemBase();
            this.mnuStatByQcCheckResult = new MqsMenuItemBase();
            this.mnuStatByYunxingQcCheckResult = new MqsMenuItemBase();
            this.mnuStatByYunxingQcCheckResultDetail = new MqsMenuItemBase();
            this.mnuStatByContentCheck = new MqsMenuItemBase();
            this.mnuStatJobDescription = new MqsMenuItemBase();
            // 
            // mnuStatByBugs
            // 
            this.mnuStatByBugs.Name = "mnuStatByBugs";
            this.mnuStatByBugs.ShortcutKeys = Keys.F3;
            this.mnuStatByBugs.Text = "��������嵥";
            this.mnuStatByBugs.Click += new EventHandler(this.mnuStatByBugs_Click);
            // 
            // mnuStatByBugType
            // 
            this.mnuStatByBugType.Name = "mnuStatByBugType";
            this.mnuStatByBugType.Text = "����������ͳ��";
            this.mnuStatByBugType.Click += new EventHandler(this.mnuStatByBugType_Click);
            // 
            // mnuStatByDept
            // 
            this.mnuStatByDept.Name = "mnuStatByDept";
            this.mnuStatByDept.Text = "������ͳ��";
            this.mnuStatByDept.Click += new EventHandler(this.mnuStatByDept_Click);
            // 
            // mnuStatByChecker
            // 
            this.mnuStatByChecker.Name = "mnuStatByChecker";
            this.mnuStatByChecker.Text = "�������ͳ��";
            this.mnuStatByChecker.Click += new EventHandler(this.mnuStatByChecker_Click);
            // 
            // mnuStatByWorkload
            // 
            this.mnuStatByWorkload.Name = "mnuStatByWorkload";
            this.mnuStatByWorkload.Text = "������ͳ��";
            this.mnuStatByWorkload.Click += new EventHandler(this.mnuStatByWorkload_Click);
            //
            //mnuStatJobDescription
            //
            this.mnuStatJobDescription.Name = "mnuStatJobDescription";
            this.mnuStatJobDescription.Text = "������ϸ����ͳ��";
            this.mnuStatJobDescription.Click += new EventHandler(this.mnuStatJobDescription_Click);
            // 
            // mnuStatByDeptWordload
            // 
            this.mnuStatByDeptWordload.Name = "mnuStatByDeptWordload";
            this.mnuStatByDeptWordload.Text = "���Ҳ���������ͳ��";
            this.mnuStatByDeptWordload.Click += new EventHandler(this.mnuStatByDeptWordload_Click);
            //
            // mnuStatByQuestion
            //
            this.mnuStatByQuestion.Name = "mnuStatByQuestion";
            this.mnuStatByQuestion.Text = "���ʼ�����ͳ��";
            this.mnuStatByQuestion.Click += new EventHandler(this.mnuStatByQuestion_Click);
            //
            //mnuStatByModifyDocTimes
            //
            this.mnuStatByModifyDocTimes.Name = "mnuStatByModifyDocTimes";
            this.mnuStatByModifyDocTimes.Text = "�������޸Ĵ���ͳ��";
            this.mnuStatByModifyDocTimes.Click += new EventHandler(this.mnuStatByModifyDocTimes_Click);
            //
            //mnuStatDocScores
            //
            this.mnuStatDocScores.Name = "mnuStatDocScores";
            this.mnuStatDocScores.Text = "��������һ����";
            this.mnuStatDocScores.Click += new EventHandler(this.mnuStatDocScores_Click);
            
            //
            //ϵͳʱЧ�������
            //
            this.mnuStatByHospitalTimeCheck.Name = "mnuStatByHospitalTimeCheck";
            this.mnuStatByHospitalTimeCheck.Text = "ϵͳʱЧ�������";
            this.mnuStatByHospitalTimeCheck.Click += new EventHandler(mnuStatByHospitalTimeCheck_Click);
            //
            //���ڲ����Զ��˲�ȱ��ͳ�Ʊ�
            //
            this.mnuStatByQcCheckResult.Name = "mnuStatByQcCheckResult";
            this.mnuStatByQcCheckResult.Text = "����ȱ�������嵥";
            this.mnuStatByQcCheckResult.Click += new EventHandler(mnuStatByQcCheckResult_Click);
            //
            //����ȱ��ͳ��
            //
            this.mnuStatByYunxingQcCheckResult.Name = "mnuStatByYunxingQcCheckResult";
            this.mnuStatByYunxingQcCheckResult.Text = "����ȱ�ݷ���ͳ��";
            this.mnuStatByYunxingQcCheckResult.Click += new EventHandler(mnuStatByYunxingQcCheckResult_Click);
            //
            //����ȱ��ͳ��
            //
            this.mnuStatByYunxingQcCheckResultDetail.Name = "mnuStatByYunxingQcCheckResultDetail";
            this.mnuStatByYunxingQcCheckResultDetail.Text = "����ȱ�ݿ���ͳ��";
            this.mnuStatByYunxingQcCheckResultDetail.Click += new EventHandler(mnuStatByYunxingQcCheckResultDetail_Click);
            //
            //ϵͳʱЧ��ʱͳ��
            //
            this.mnuStatByTimeCheck.Name = "mnuStatByTimeCheck";
            this.mnuStatByTimeCheck.Text = "ϵͳʱЧ��ʱͳ��";
            this.mnuStatByTimeCheck.Click += new EventHandler(mnuStatByTimeCheck_Click);
            //
            //ϵͳ�������ݼ������
            //
            this.mnuStatByContentCheck.Name = "mnuStatByContentCheck";
            this.mnuStatByContentCheck.Text = "ϵͳ�������ݼ������";
            this.mnuStatByContentCheck.Click += MnuStatByContentCheck_Click;
          
            //
            //��������ͳ��
            //
            this.mnuStatByDocScoreCompare.Name = "mnuStatByDocScoreCompare";
            this.mnuStatByDocScoreCompare.Text = "��������ͳ��";
            this.mnuStatByDocScoreCompare.Click += new EventHandler(this.mnuStatByDocScoreCompare_Click);
           
            // 
            // menuStatistic
            // 
            this.Name = "menuStatistic";
            this.Text = "ͳ��(&T)";
            this.DropDownItems.AddRange(new ToolStripItem[] {
                this.mnuStatByBugs,
                this.mnuStatByBugType,
                this.mnuStatByDept,
                this.mnuStatByChecker,
                this.mnuStatByWorkload,
                this.mnuStatJobDescription,
                this.mnuStatByDeptWordload,
                this.mnuStatByQuestion,
                this.mnuStatDocScores,
                this.mnuStatByDocScoreCompare,
                this.mnuStatByModifyDocTimes,
                this.mnuStatByHospitalTimeCheck,
                this.mnuStatByTimeCheck,
                this.mnuStatByContentCheck,
                this.mnuStatByYunxingQcCheckResult,
                this.mnuStatByYunxingQcCheckResultDetail,
                this.mnuStatByQcCheckResult,
                new ToolStripSeparator(),
                this.mnuStatByHospitalTimeCheck,
                this.mnuStatByTimeCheck,
                this.mnuStatByContentCheck,
                this.mnuStatByYunxingQcCheckResult,
                this.mnuStatByYunxingQcCheckResultDetail,
                this.mnuStatByQcCheckResult
            });
        }
        #endregion

        public StatisticMenu(MainForm parent)
        {
            this.m_mainForm = parent;

            this.InitializeComponent();
        }

        private void mnuStatByBugs_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��������嵥", this.MainForm, null);
        }

        private void mnuStatByBugType_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��������ͳ��", this.MainForm, null);
        }

        private void mnuStatByDept_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("������ͳ��", this.MainForm, null);
        }

        private void mnuStatByChecker_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�������ͳ��", this.MainForm, null);
        }

        private void mnuStatByWorkload_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("������ͳ��", this.MainForm, null);
        }

        private void mnuStatJobDescription_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("������ϸ����ͳ��", this.MainForm, null);
        }
        private void mnuStatByDeptWordload_Click(object sender, System.EventArgs e)
        {
            CommandHandler.Instance.SendCommand("���Ҳ���������ͳ��", this.MainForm, null);
        }

        private void mnuStatByQuestion_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("���ʼ�����ͳ��", this.MainForm, null);
        }
        private void mnuStatByModifyDocTimes_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("�����޸Ĵ���ͳ��", this.MainForm, null);
        }

        private void mnuStatDocScores_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��������һ����", this.MainForm, null);
        }
        
        private void mnuStatByDocScoreCompare_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��������ͳ��", this.MainForm, null);
        }
        private void MnuStatByContentCheck_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("��������ͳ��", this.MainForm, null);
        }
        
        void mnuStatByHospitalTimeCheck_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("ϵͳʱЧ�������", this.MainForm, null);
        }
        void mnuStatByTimeCheck_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("ϵͳʱЧ��ʱͳ��", this.MainForm, null);
        }
        void mnuStatByQcCheckResult_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("����ȱ�������嵥", this.MainForm, null);
        }
        void mnuStatByYunxingQcCheckResult_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("����ȱ�ݿ���ͳ��", this.MainForm, null);
        }
        void mnuStatByYunxingQcCheckResultDetail_Click(object sender, EventArgs e)
        {
            CommandHandler.Instance.SendCommand("����ȱ�ݷ���ͳ��", this.MainForm, null);
        }
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (this.m_mainForm == null || this.m_mainForm.IsDisposed)
                return;
            this.mnuStatByHospitalTimeCheck.Visible = true;
            this.mnuStatByTimeCheck.Visible = true;
            this.mnuStatByModifyDocTimes.Visible = false;

            if (!SystemParam.Instance.LocalConfigOption.IsShowQCContentRecord)
            {
                this.mnuStatByContentCheck.Visible = false;
            }
            if (!SystemParam.Instance.LocalConfigOption.IsShowHospitalTimeCheckStatic)
            {
                this.mnuStatByTimeCheck.Visible = false;
                this.mnuStatByHospitalTimeCheck.Visible = false;
            }

            if (!SystemParam.Instance.LocalConfigOption.IsCheckPoint)
            {
                this.mnuStatByQcCheckResult.Visible = false;
                this.mnuStatByYunxingQcCheckResult.Visible = false;
                this.mnuStatByYunxingQcCheckResultDetail.Visible = false;
            }
        }
    }
}
