// ***********************************************************
// 患者列表对话框中患者检索界面的UserControl.
// Creator:YangMingkun  Date:2009-12-2
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using Heren.Common.Controls.DictInput;

using MedQCSys.Dialogs;
using System.Collections;
using EMRDBLib.DbAccess;
using EMRDBLib;
using Heren.MedQC.Core;

namespace MedQCSys.Controls
{
    public partial class PatSearchPane : UserControl
    {
        private FindComboBox m_cboDeptList = null;
        private FindComboBox m_cboDocTypeList = null;
        private ComboBox m_cboPatientType = null;
        private ComboBox m_cboOperatorType = null;
        private FindComboBox m_cboOperationList = null;
        private DateTimePicker m_dtpBeginTime = null;
        private DateTimePicker m_dtpEndTime = null;
        private TextBox m_txtPatientInfo = null;
        private TextBox m_txtInHospDays = null;
        private bool m_bSearchEnabled = true;
        public DockForms.PatientListForm PatListForm;

        //最近一次的检索类型
        private EMRDBLib.PatSearchType m_eLastSearchType = EMRDBLib.PatSearchType.Unknown;
        //当前的检索类型
        private EMRDBLib.PatSearchType m_eCurrSearchType = EMRDBLib.PatSearchType.Unknown;
        private EMRDBLib.PatientType m_PatientType = EMRDBLib.PatientType.AllPatient;
        private EMRDBLib.OperatorType m_OperatorType = EMRDBLib.OperatorType.UnKnown;

        #region"属性"
        /// <summary>
        /// 获取或设置当前的检索类型
        /// </summary>
        [Description("获取或设置当前的检索类型")]
        public PatSearchType SearchType
        {
            get
            {
                return this.m_eCurrSearchType;
            }
            set
            {
                if (value == PatSearchType.Unknown)
                    return;

                this.m_eLastSearchType = this.m_eCurrSearchType;
                this.m_eCurrSearchType = value;
                this.UpdateSearchTypeUI();

                this.SetHeight();
            }
        }

        public void SetHeight()
        {
            int height = 10;
            foreach (System.Windows.Forms.Control ctr in this.Controls)
            {
                if (ctr.GetType() == typeof(System.Windows.Forms.Panel) && ctr.Visible == true)
                {
                    height += pnlSearchType.Height;
                }
            }
            this.Height = height;
        }
        /// <summary>
        /// 初始化科室相关控件
        /// </summary>
        private void InitDeptControl()
        {
            this.m_cboDeptList = this.fdCBXDept;
            if (this.m_cboDeptList == null)
            {
                this.m_cboDeptList = new FindComboBox();
                this.m_cboDeptList.Name = "m_cboDeptList";
                this.m_cboDeptList.Font = this.cboSearchType.Font;
                this.m_cboDeptList.KeyDown += new KeyEventHandler(this.DeptListComboBox_KeyDown);
                this.m_cboDeptList.SelectedIndexChanged += new EventHandler(this.DeptListComboBox_SelectedIndexChanged);
            }
            this.InitClinicDeptList();
            if (!this.pnlDeptName.Contains(this.m_cboDeptList))
            {
                this.m_cboDeptList.Width = this.cboSearchType.Width;
                this.m_cboDeptList.Height = this.cboSearchType.Height;
                this.pnlDeptName.Controls.Add(this.m_cboDeptList);
                this.m_cboDeptList.Location = new Point(this.cboSearchType.Location.X, this.lblDeptName.Location.Y - 3);
                this.m_cboDeptList.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
            this.cboSearchType.Items.Add("专家质控");
        }
        /// <summary>
        /// 初始化手术患者相关控件
        /// </summary>
        private void InitOperatedPatientControl()
        {
            this.m_cboOperationList = this.fdCBXOperation;
            if (this.m_cboOperationList == null)
            {
                this.m_cboOperationList = new FindComboBox();
                this.m_cboOperationList.Name = "m_cboOperationList";
                this.m_cboOperationList.Font = this.cboSearchType.Font;
            }
            this.InitOperationList();
            if (!this.pnlOperationName.Contains(this.m_cboOperationList))
            {
                this.m_cboOperationList.Width = this.cboSearchType.Width;
                this.m_cboOperationList.Height = this.cboSearchType.Height;
                this.pnlOperationName.Controls.Add(this.m_cboOperationList);
                this.m_cboOperationList.Location = new Point(this.cboSearchType.Location.X, this.lblOperationName.Location.Y - 3);
                this.m_cboOperationList.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            }
        }

        /// <summary>
        /// 初始化住院天数相关控件
        /// </summary>
        private void InitInHosDaysControls()
        {
            if (this.m_txtInHospDays == null)
            {
                this.m_txtInHospDays = new TextBox();
                this.m_txtInHospDays.Name = "m_txtInHospDays";
                this.m_txtInHospDays.Font = this.cboSearchType.Font;
                this.m_txtInHospDays.MaxLength = 6;
                this.m_txtInHospDays.KeyPress += new KeyPressEventHandler(txtInHospDays_KeyPress);
            }
            if (this.m_cboOperatorType == null)
            {
                this.m_cboOperatorType = new ComboBox();
                this.m_cboOperatorType.Name = "m_cboOperatorType";
                this.m_cboOperatorType.Font = this.cboSearchType.Font;
                this.m_cboOperatorType.Items.AddRange(new string[] { "大于", "小于", "等于", "大于等于", "小于等于" });
                this.m_cboOperatorType.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            if (!this.pnlInHosDays.Contains(this.m_cboOperatorType))
            {
                this.m_cboOperatorType.Width = 100;
                this.m_cboOperatorType.Height = this.cboSearchType.Height;
                this.pnlInHosDays.Controls.Add(this.m_cboOperatorType);
                this.m_cboOperatorType.Location = new Point(this.cboSearchType.Location.X, this.lblInHosDays.Location.Y - 3);
                this.m_cboOperatorType.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            }
            if (!this.pnlInHosDays.Contains(this.m_txtInHospDays))
            {
                this.m_txtInHospDays.Width = this.cboSearchType.Width - this.m_cboOperatorType.Width - 6;
                this.m_txtInHospDays.Height = this.cboSearchType.Height;
                this.pnlInHosDays.Controls.Add(this.m_txtInHospDays);
                this.m_txtInHospDays.Location = new Point(this.cboSearchType.Location.X + this.m_cboOperatorType.Width + 3, this.lblInHosDays.Location.Y - 3);
                this.m_txtInHospDays.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
            DateTime dtStartTime = DateTime.Now;
            dtStartTime = dtStartTime.AddDays(SystemParam.Instance.LocalConfigOption.SearchTimeSpanDays);
            dtStartTime = dtStartTime.AddMonths(-SystemParam.Instance.LocalConfigOption.SearchTimeSpanMonths);
            m_dtLastAdmissionTimeBegin = dtStartTime;
            m_dtLastSerCriAdmissionTimeBegin = dtStartTime;
            m_dtLastDischargeTimeBegin = dtStartTime;
            m_dtLastDeathTimeBegin = dtStartTime;
            m_dtDeathTimeBegin = dtStartTime;
            m_dtOperBeginTime = dtStartTime;

            //入院时间起始修改
            if (SystemParam.Instance.LocalConfigOption.SpecialStartTime != 0)
            {
                m_dtLastAdmissionTimeBegin = DateTime.Now.AddDays(SystemParam.Instance.LocalConfigOption.SpecialStartTime);
                m_dtLastDischargeTimeBegin = DateTime.Now.AddDays(SystemParam.Instance.LocalConfigOption.SpecialStartTime);
            }
            //出院时间起始修改
            if (SystemParam.Instance.LocalConfigOption.SpecialEndTime != 0)
            {
                m_dtLastAdmissionTimeEnd = DateTime.Now.AddDays(SystemParam.Instance.LocalConfigOption.SpecialEndTime);
                m_dtLastDischargeTimeEnd = DateTime.Now.AddDays(SystemParam.Instance.LocalConfigOption.SpecialEndTime);
            }
        }
        /// <summary>
        /// 初始化患者病案号及姓名信息相关控件
        /// </summary>
        private void InitPatIDNameControls()
        {
            if (this.m_txtPatientInfo == null)
            {
                this.m_txtPatientInfo = new TextBox();
                this.m_txtPatientInfo.Name = "m_txtPatientInfo";
                this.m_txtPatientInfo.Font = this.cboSearchType.Font;
                this.m_txtPatientInfo.KeyDown += new KeyEventHandler(this.PatientInfoTextBox_KeyDown);
            }
            if (!this.pnlPatient.Contains(this.m_txtPatientInfo))
            {
                this.m_txtPatientInfo.Width = this.cboSearchType.Width;
                this.m_txtPatientInfo.Height = this.cboSearchType.Height;
                this.pnlPatient.Controls.Add(this.m_txtPatientInfo);
                this.m_txtPatientInfo.Location = new Point(this.cboSearchType.Location.X, this.lblPatient.Location.Y - 3);
                this.m_txtPatientInfo.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
        }

        /// <summary>
        /// 初始化患者范围（类型）相关控件
        /// </summary>
        private void InitPatientTypeControls()
        {
            if (this.m_cboPatientType == null)
            {
                this.m_cboPatientType = new ComboBox();
                this.m_cboPatientType.Name = "m_cboPatientType";
                this.m_cboPatientType.Font = this.cboSearchType.Font;
                this.m_cboPatientType.Items.AddRange(new string[] { "所有患者", "在院患者", "出院患者" });
                this.m_cboPatientType.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            if (!this.pnlPatientStatus.Contains(this.m_cboPatientType))
            {
                this.m_cboPatientType.Width = this.cboSearchType.Width;
                this.m_cboPatientType.Height = this.cboSearchType.Height;
                this.pnlPatientStatus.Controls.Add(this.m_cboPatientType);
                this.m_cboPatientType.Location = new Point(this.cboSearchType.Location.X, this.lblPatientStatus.Location.Y - 3);
                this.m_cboPatientType.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
        }
        /// <summary>
        /// 获取选择的专家病案分配信息
        /// </summary>
        public EMRDBLib.QcSpecialCheck QcSpecialCheck
        {
            get
            {
                if (this.m_cboSpecialCheckList == null || this.m_cboSpecialCheckList.IsDisposed
                    || this.m_cboSpecialCheckList.SelectedItem == null)
                    return null;

                EMRDBLib.QcSpecialCheck qcSpecialCheck = this.m_cboSpecialCheckList.SelectedItem as EMRDBLib.QcSpecialCheck;
                if (qcSpecialCheck == null)
                    return null;
                return qcSpecialCheck;
            }
        }
        /// <summary>
        /// 获取最近一次的检索类型
        /// </summary>
        [Description("获取最近一次的检索类型")]
        public EMRDBLib.PatSearchType LastSearchType
        {
            get { return this.m_eLastSearchType; }
        }

        private EMRDBLib.PatientType m_LastDeptPatientType = EMRDBLib.PatientType.PatInHosptial;
        /// <summary>
        /// 获取上一次选择的科室检索中的患者类型
        /// </summary>
        public EMRDBLib.PatientType LastDeptPatientType
        {
            get { return this.m_LastDeptPatientType; }
        }

        private EMRDBLib.PatientType m_LastAdmPatientType = EMRDBLib.PatientType.PatInHosptial;
        /// <summary>
        /// 获取上一次选择的入院日期检索的患者类型
        /// </summary>
        public EMRDBLib.PatientType LastAdmPatientType
        {
            get { return this.m_LastAdmPatientType; }
        }

        private DocTypeInfo m_DocType = null;
        /// <summary>
        /// 获取上次的文档类型
        /// </summary>
        public DocTypeInfo DocType
        {
            get { return this.m_DocType; }
        }

        private string m_szLastPatientID = string.Empty;
        /// <summary>
        /// 获取上一次选择的患者ID
        /// </summary>
        public string LastPatientID
        {
            get { return this.m_szLastPatientID; }
            set { this.m_szLastPatientID = value; }
        }

        private string m_szLastPatientName = string.Empty;
        /// <summary>
        /// 获取或设置上一次选择的患者姓名
        /// </summary>
        public string LastPatientName
        {
            get { return this.m_szLastPatientName; }
            set { this.m_szLastPatientName = value; }
        }

        private string m_szLastInpNo = string.Empty;
        /// <summary>
        /// 获取或设置上一次选择的病案号
        /// </summary>
        public string LastInpNo
        {
            get { return this.m_szLastInpNo; }
            set { this.m_szLastInpNo = value; }
        }

        private string m_szLastInHospDept = string.Empty;
        /// <summary>
        /// 获取或设置上一次选择的在院患者的科室代码
        /// </summary>
        public string LastInHospDept
        {
            get { return this.m_szLastInHospDept; }
            set { this.m_szLastInHospDept = value; }
        }

        /// <summary>
        /// 获取当前选择的科室信息
        /// </summary>
        [Browsable(false)]
        public DeptInfo DeptInfo
        {
            get
            {
                if (this.m_cboDeptList == null || this.m_cboDeptList.IsDisposed
                    || string.IsNullOrEmpty(this.m_cboDeptList.Text))
                    return null;
                return this.m_cboDeptList.SelectedItem as DeptInfo;
            }
        }
        /// <summary>
        /// 获取当前选择的质检人员信息
        /// </summary>
        [Browsable(false)]
        public UserInfo UserInfo
        {
            get
            {
                if (this.fdbCheckName == null || this.fdbCheckName.IsDisposed)
                    return null;
                if (string.IsNullOrEmpty(this.fdbCheckName.Text))
                    return null;
                return this.fdbCheckName.SelectedItem as UserInfo;
            }
        }
        private DateTime m_dtLastAdmissionTimeBegin = DateTime.Now;
        /// <summary>
        /// 获取上次选择入院起始日期
        /// </summary>
        public DateTime LastAdmissionTimeBegin
        {
            get
            {
                return this.m_dtLastAdmissionTimeBegin;
            }
        }

        private DateTime m_dtLastAdmissionTimeEnd = DateTime.Now;
        /// <summary>
        /// 获取上次选择入院截止日期
        /// </summary>
        public DateTime LastAdmissionTimeEnd
        {
            get
            {
                return this.m_dtLastAdmissionTimeEnd;
            }
        }

        /// <summary>
        /// 获取当前选择的起始入院时间
        /// </summary>
        [Browsable(false)]
        public DateTime AdmissionTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtAdmissionTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtAdmissionTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取当前选择的截止入院时间
        /// </summary>
        [Browsable(false)]
        public DateTime AdmissionTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtAdmissionTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtAdmissionTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        /// <summary>
        /// 获取当前选择的起始写病历时间
        /// </summary>
        [Browsable(false)]
        public DateTime DocTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtDocTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtDocTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取当前选择的截止写病历时间
        /// </summary>
        [Browsable(false)]
        public DateTime DocTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtDocTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtDocTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        /// <summary>
        /// 获取当前选择的起始入院时间
        /// </summary>
        [Browsable(false)]
        public DateTime VisitTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtVisitTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtVisitTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取当前选择的截止入院时间
        /// </summary>
        [Browsable(false)]
        public DateTime VisitTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtVisitTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtVisitTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        /// <summary>
        /// 获取当前选择的起始转科时间
        /// </summary>
        [Browsable(false)]
        public DateTime TransferTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtTransferTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtTransferTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取当前选择的截止转科时间
        /// </summary>
        [Browsable(false)]
        public DateTime TransferTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtTransferTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtTransferTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }
        /// <summary>
        /// 获取质检开始时间
        /// </summary>
        [Browsable(false)]
        public DateTime IssusdTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtAdmissionTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtAdmissionTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取质检结束时间
        /// </summary>
        [Browsable(false)]
        public DateTime IssusdTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtAdmissionTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtAdmissionTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        private DateTime m_dtLastSerCriAdmissionTimeBegin = DateTime.Now;
        /// <summary>
        /// 获取上次危重病历起始日期
        /// </summary>
        public DateTime LastSerCriAdmissionTimeBegin
        {
            get
            {
                return this.m_dtLastSerCriAdmissionTimeBegin;
            }
        }

        private DateTime m_dtLastSerCriAdmissionTimeEnd = DateTime.Now;
        /// <summary>
        /// 获取上次危重病历截止日期
        /// </summary>
        public DateTime LastSerCriAdmissionTimeEnd
        {
            get
            {
                return this.m_dtLastSerCriAdmissionTimeEnd;
            }
        }

        /// <summary>
        /// 获取当前选择的危重病历起始入院时间
        /// </summary>
        [Browsable(false)]
        public DateTime SerCriAdmissionTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtSerCriAdmissionTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtSerCriAdmissionTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取当前选择的危重病历截止入院时间
        /// </summary>
        [Browsable(false)]
        public DateTime SerCriAdmissionTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtSerCriAdmissionTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtSerCriAdmissionTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        private DateTime m_dtLastDischargeTimeBegin = DateTime.Now;
        /// <summary>
        /// 获取上次选择出院起始日期
        /// </summary>
        public DateTime LastDischargeTimeBegin
        {
            get
            {
                return this.m_dtLastAdmissionTimeBegin;
            }
        }

        private DateTime m_dtLastDischargeTimeEnd = DateTime.Now;
        /// <summary>
        /// 获取上次选择出院截止日期
        /// </summary>
        public DateTime LastDischargeTimeEnd
        {
            get
            {
                return this.m_dtLastDischargeTimeEnd;
            }
        }

        /// <summary>
        /// 获取当前选择的起始出院时间
        /// </summary>
        [Browsable(false)]
        public DateTime DischargeTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtDischargeTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtDischargeTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        /// <summary>
        /// 获取当前选择的截止出院时间
        /// </summary>
        [Browsable(false)]
        public DateTime DischargeTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtDischargeTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtDischargeTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        private DateTime m_dtLastDeathTimeBegin = DateTime.Now;
        /// <summary>
        /// 获取上次选择的死亡开始日期
        /// </summary>
        public DateTime LastDeathTimeBegin
        {
            get
            {
                return this.m_dtLastDeathTimeBegin;
            }
        }

        private DateTime m_dtLastDeathTimeEnd = DateTime.Now;
        /// <summary>
        /// 获取上次选择的死亡截止日期
        /// </summary>
        public DateTime LastDeathTimeEnd
        {
            get
            {
                return this.m_dtLastDeathTimeEnd;
            }
        }

        private DateTime m_dtDeathTimeBegin = DateTime.Now;
        /// <summary>
        /// 获取当前选择的死亡开始日期
        /// </summary>
        public DateTime DeathTimeBegin
        {
            get
            {
                if (this.m_dtpBeginTime == null || this.m_dtpBeginTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtDeathTimeBegin = this.m_dtpBeginTime.Value.Date;
                return DateTime.Parse(dtDeathTimeBegin.ToString("yyyy-M-d 00:00:00"));
            }
        }

        private DateTime m_dtDeathTimeEnd = DateTime.Now;
        /// <summary>
        /// 获取当前选择的死亡截止日期
        /// </summary>
        public DateTime DeathTimeEnd
        {
            get
            {
                if (this.m_dtpEndTime == null || this.m_dtpEndTime.IsDisposed)
                    return DateTime.Now;
                DateTime dtDeathTimeEnd = this.m_dtpEndTime.Value.Date;
                return DateTime.Parse(dtDeathTimeEnd.ToString("yyyy-M-d 23:59:59"));
            }
        }

        /// <summary>
        /// 获取检索患者的类型
        /// </summary>
        [Browsable(false)]
        public EMRDBLib.PatientType PatientType
        {
            get
            {
                if (this.m_cboPatientType == null || this.m_cboPatientType.IsDisposed
                    || this.m_cboPatientType.SelectedItem == null)
                {
                    this.m_PatientType = EMRDBLib.PatientType.Unknown;
                    return this.m_PatientType;
                }
                if (this.m_cboPatientType.SelectedItem.ToString() == "在院患者")
                    this.m_PatientType = EMRDBLib.PatientType.PatInHosptial;
                else if (this.m_cboPatientType.SelectedItem.ToString() == "出院患者")
                    this.m_PatientType = EMRDBLib.PatientType.PatOutHosptal;
                else
                    this.m_PatientType = EMRDBLib.PatientType.AllPatient;
                return this.m_PatientType;
            }
        }

        /// <summary>
        /// 获取操作符类型
        /// </summary>
        [Browsable(false)]
        public EMRDBLib.OperatorType operatorType
        {
            get
            {
                if (this.m_cboOperatorType == null || this.m_cboOperatorType.IsDisposed
                    || this.m_cboOperatorType.SelectedItem == null)
                {
                    this.m_OperatorType = EMRDBLib.OperatorType.UnKnown;
                    return this.m_OperatorType;
                }
                if (this.m_cboOperatorType.SelectedItem.ToString() == "大于")
                    this.m_OperatorType = EMRDBLib.OperatorType.Morethan;
                else if (this.m_cboOperatorType.SelectedItem.ToString() == "小于")
                    this.m_OperatorType = EMRDBLib.OperatorType.Lessthan;
                else if (this.m_cboOperatorType.SelectedItem.ToString() == "等于")
                    this.m_OperatorType = EMRDBLib.OperatorType.Equalthan;
                else if (this.m_cboOperatorType.SelectedItem.ToString() == "大于等于")
                    this.m_OperatorType = EMRDBLib.OperatorType.MoreEqualthan;
                else
                    this.m_OperatorType = EMRDBLib.OperatorType.LessEqualthan;
                return this.m_OperatorType;
            }
        }

        /// <summary>
        /// 获取选择的手术编码
        /// </summary>
        public string OperationCode
        {
            get
            {
                if (this.m_cboOperationList == null || this.m_cboOperationList.IsDisposed
                    || this.m_cboOperationList.SelectedItem == null)
                    return string.Empty;

                EMRDBLib.OperationDict operation = this.m_cboOperationList.SelectedItem as EMRDBLib.OperationDict;
                if (operation == null)
                    return string.Empty;
                return operation.OperationCode;
            }
        }

        private string m_szLastOperDept = string.Empty;
        /// <summary>
        /// 获取或设置上一次选择的做过手术的科室
        /// </summary>
        public string LastOperDept
        {
            get { return this.m_szLastOperDept; }
            set { this.m_szLastOperDept = value; }
        }

        private EMRDBLib.PatientType m_lastOperPatientType = EMRDBLib.PatientType.PatInHosptial;
        /// <summary>
        /// 获取或设置上一次选择的手术患者类型
        /// </summary>
        public EMRDBLib.PatientType OperPatientType
        {
            get { return this.m_lastOperPatientType; }
            set { this.m_lastOperPatientType = value; }
        }

        private EMRDBLib.PatientType m_lastInHospPatientType = EMRDBLib.PatientType.PatInHosptial;
        /// <summary>
        /// 获取或设置上一次选择的住院天数患者类型
        /// </summary>
        public EMRDBLib.PatientType LastInHospPatientType
        {
            get { return this.m_lastInHospPatientType; }
            set { this.m_lastInHospPatientType = value; }
        }

        private EMRDBLib.OperatorType m_lastOperatorType = EMRDBLib.OperatorType.UnKnown;
        /// <summary>
        /// 获取或设置上一次选择的住院天数中操作符类型
        /// </summary>
        public EMRDBLib.OperatorType LastOperatorType
        {
            get { return this.m_lastOperatorType; }
            set { this.m_lastOperatorType = value; }
        }

        private int m_nLastInHospDays = 0;
        /// <summary>
        /// 获取或设置上一次选择的住院天数
        /// </summary>
        public int LastInHospDays
        {
            get { return this.m_nLastInHospDays; }
            set { this.m_nLastInHospDays = value; }
        }
        /// <summary>
        /// 专家质检显示所有分配批次的病人
        /// </summary>
        public bool IsSpecialAll
        {
            get { return this.rdbSpecialAll.Checked; }
        }
        private DateTime m_dtOperBeginTime = DateTime.Now;
        /// <summary>
        ///  获取或设置上一次选择手术的起始日期
        /// </summary>
        public DateTime OperBeginTime
        {
            get
            {
                return this.m_dtOperBeginTime;
            }
            set { this.m_dtOperBeginTime = value; }
        }

        private DateTime m_dtOperEndTime = DateTime.Now;
        /// <summary>
        /// 获取或设置上一次选择手术的截止日期
        /// </summary>
        public DateTime OperEndTime
        {
            get
            {
                return this.m_dtOperEndTime;
            }
            set { this.m_dtOperEndTime = value; }
        }

        private EMRDBLib.OperationDict m_lastOperation = null;
        /// <summary>
        /// 获取或设置上一次选择的手术信息
        /// </summary>
        public EMRDBLib.OperationDict LastOperation
        {
            get { return this.m_lastOperation; }
            set { this.m_lastOperation = value; }
        }

        /// <summary>
        /// 获取当期输入的患者ID
        /// </summary>
        [Browsable(false)]
        public string PatientID
        {
            get
            {
                if (this.m_txtPatientInfo == null || this.m_txtPatientInfo.IsDisposed)
                    return string.Empty;

                return this.m_txtPatientInfo.Text.Trim();
            }
        }

        /// <summary>
        /// 获取当期输入的患者住院号
        /// </summary>
        [Browsable(false)]
        public string InHospitalID
        {
            get
            {
                if (this.m_txtPatientInfo == null || this.m_txtPatientInfo.IsDisposed)
                    return string.Empty;

                return this.m_txtPatientInfo.Text.Trim();
            }
        }

        /// <summary>
        /// 获取当前输入的患者姓名
        /// </summary>
        [Browsable(false)]
        public string PatientName
        {
            get
            {
                if (this.m_txtPatientInfo == null || this.m_txtPatientInfo.IsDisposed)
                    return string.Empty;

                return this.m_txtPatientInfo.Text.Trim();
            }
        }

        /// <summary>
        /// 获取当前输入的住院天数
        /// </summary>
        [Browsable(false)]
        public int InHosptialDays
        {
            get
            {
                if (this.m_txtInHospDays == null || this.m_txtInHospDays.IsDisposed)
                    return 0;

                return GlobalMethods.Convert.StringToValue(this.m_txtInHospDays.Text.Trim(), 0);
            }
        }

        /// <summary>
        /// 获取检索方式下拉框
        /// </summary>
        public ComboBox SearchTypeCombox
        {
            get { return this.cboSearchType; }
        }
        #endregion

        #region"事件"
        /// <summary>
        /// 当开始检索患者时触发
        /// </summary>
        [Description("当开始检索患者时触发")]
        public event EventHandler StartSearch;
        protected virtual void OnStartSearch(EventArgs e)
        {
            if (this.DesignMode)
                return;
            Control parent = GlobalMethods.UI.GetTopLevelParent(this);
            //GlobalMethods.UI.SetCursor(parent, Cursors.WaitCursor);
            this.SaveSearchValue();
            if (this.StartSearch == null)
            {
                GlobalMethods.UI.SetCursor(parent, Cursors.Default);
                return;
            }
            try
            {
                this.Update();
                this.StartSearch(this, e);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("PatSearchPane.OnStartSearch", ex);
            }
            //GlobalMethods.UI.SetCursor(parent, Cursors.Default);
        }


        /// <summary>
        /// 当下拉框的选项改变时触发
        /// </summary>
        [Description("当下拉框的选项改变时触发")]
        public event EventHandler ComboBoxSelectItemChanged;
        protected virtual void OnComboBoxSelectItemChanged(EventArgs e)
        {
            if (this.ComboBoxSelectItemChanged != null)
                this.ComboBoxSelectItemChanged(this, e);
        }

        /// <summary>
        /// 按下检索按钮,开始检索患者信息
        /// </summary>
        [Description("按下检索按钮,开始检索患者信息")]
        public event EventHandler ButtonSearch;
        protected virtual void OnButtonSearch(EventArgs e)
        {

            if (this.ButtonSearch != null)
                this.ButtonSearch(this, e);
        }

        public delegate void ShowStatusMessageHandler(string szStatusMessage);
        /// <summary>
        /// 当控件请求显示进度状态消息时触发
        /// </summary>
        [Description("当控件请求显示进度状态消息时触发")]
        public event ShowStatusMessageHandler StatusMessageChanged;
        protected virtual void OnStatusMessageChanged(string szStatusMessage)
        {
            if (this.StatusMessageChanged == null)
                return;
            try
            {
                this.StatusMessageChanged(szStatusMessage);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("PatSearchPane.StatusMessageChanged", ex);
            }
        }
        #endregion

        public PatSearchPane()
        {
            this.InitializeComponent();
            this.InitControls();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (SystemParam.Instance.LocalConfigOption != null)
            {
                if (SystemParam.Instance.LocalConfigOption.IsOpenComplexSearch)
                    this.btnComplex.Visible = true;
            }
        }
        /// <summary>
        /// 初始化控件
        /// </summary>
        public void InitControls()
        {
            if (SystemParam.Instance.UserInfo == null)
                return;
            WorkProcess.Instance.Initialize(this, 10, "正在初始化数据...");
            WorkProcess.Instance.Show("正在初始化数据...", 0, false);
            this.InitDeptControl();
            WorkProcess.Instance.Show("正在初始化数据...", 1, false);
            this.InitPatientTypeControls();
            WorkProcess.Instance.Show("正在初始化数据...", 2, false);
            this.InitBeginTime();
            WorkProcess.Instance.Show("正在初始化数据...", 3, false);
            this.InitEndTime();
            WorkProcess.Instance.Show("正在初始化数据...", 4, false);
            this.InitOperatedPatientControl();
            WorkProcess.Instance.Show("正在初始化数据...", 5, false);
            this.InitInHosDaysControls();
            WorkProcess.Instance.Show("正在初始化数据...", 6, false);
            this.InitPatIDNameControls();
            WorkProcess.Instance.Show("正在初始化数据...", 7, false);
            this.InitSelectedDept();
            WorkProcess.Instance.Show("正在初始化数据...", 8, false);
            this.InitCheckedDoc();
            WorkProcess.Instance.Show("正在初始化数据...", 9, false);
            this.InitDocType();
            string szToolTip = string.Format("提示：\n患者姓名颜色代表病情:红色危、绿色急、黑色一般\n绿色*代表审核通过;红色*代表存在一般问题;红色**代表存在严重问题\n");
            this.toolTip1.SetToolTip(this.pictureBox1, szToolTip);
            toolTip1.AutoPopDelay = 25000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 0;
            this.m_cboPatientType.SelectedIndexChanged += new EventHandler(m_cboPatientType_SelectedIndexChanged);
            this.chBoxShowTime.CheckedChanged += new EventHandler(chBoxShowTime_CheckedChanged);
            //触发二次变更
            //this.chBoxShowTime.Checked = true;
            //this.chBoxShowTime.Checked = false;
            WorkProcess.Instance.Close();
        }

        /// <summary>
        /// 初始化开始时间控件
        /// </summary>
        private void InitBeginTime()
        {
            if (this.m_dtpBeginTime == null)
            {
                this.m_dtpBeginTime = new DateTimePicker();
                this.m_dtpBeginTime.Name = "m_dtpBeginTime";
                this.m_dtpBeginTime.Font = this.cboSearchType.Font;
            }
            if (!this.pnlBeginDate.Contains(this.m_dtpBeginTime))
            {
                this.m_dtpBeginTime.Width = this.cboSearchType.Width;
                this.m_dtpBeginTime.Height = this.cboSearchType.Height;
                this.pnlBeginDate.Controls.Add(this.m_dtpBeginTime);
                this.m_dtpBeginTime.Location = new Point(this.cboSearchType.Location.X, this.lblBeginDate.Location.Y - 3);
                this.m_dtpBeginTime.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                this.m_dtpBeginTime.Format = DateTimePickerFormat.Custom;
                this.m_dtpBeginTime.CustomFormat = "";
            }
        }

        /// <summary>
        /// 初始化结束时间控件
        /// </summary>
        private void InitEndTime()
        {
            if (this.m_dtpEndTime == null)
            {
                this.m_dtpEndTime = new DateTimePicker();
                this.m_dtpEndTime.Name = "m_dtpEndTime";
                this.m_dtpEndTime.Font = this.cboSearchType.Font;
            }
            if (!this.pnlEndDate.Contains(this.m_dtpEndTime))
            {
                this.m_dtpEndTime.Width = this.cboSearchType.Width;
                this.m_dtpEndTime.Height = this.cboSearchType.Height;
                this.pnlEndDate.Controls.Add(this.m_dtpEndTime);
                this.m_dtpEndTime.Location = new Point(this.cboSearchType.Location.X, this.lblBeginDate.Location.Y - 3);
                this.m_dtpEndTime.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                this.m_dtpEndTime.Format = DateTimePickerFormat.Custom;
                this.m_dtpEndTime.CustomFormat = "";
            }
        }
        /// <summary>
        /// 初始化缺省检索界面
        /// </summary>
        public void InitPatSearchType()
        {
            if (this.cboSearchType.Items.Count <= 0)
                return;
            int nSearchType = SystemConfig.Instance.Get(SystemData.ConfigKey.DEFAULT_SEARCH_TYPE, 0);
            try
            {
                this.SearchType = (EMRDBLib.PatSearchType)nSearchType;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("PatSearchPane.InitPatSearchType", ex);
                this.m_bSearchEnabled = true;
                GlobalMethods.UI.SetCursor(this.Parent, Cursors.Default);
            }
        }

        /// <summary>
        /// 按照用户在检索类型列表中选择的检索类型更新检索值检索界面
        /// </summary>
        private void UpdateSearchTypeUI()
        {
            GlobalMethods.UI.SetCursor(this.Parent, Cursors.WaitCursor);
            // 科室检索下在院患者条件下需要显示时间选择复选框选择【武总质控老专家需求】
            //在院患者条件下显示复选框，全院患者和出院患者不显示，在患者状态选择后在判断是否显示
            //m_cboPatientType_SelectedIndexChanged
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Department)
            {
                this.chBoxShowTime.Visible = true;
            }
            else
            {
                this.chBoxShowTime.Visible = false;
            }
            //科室检索
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Department)
            {
                this.ShowDeptSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.OperationPatient)
            {
                this.ShowOperationSearchUI();
            }
            //入院时间、出院时间、危重病历死亡病历检索
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
            {
                this.ShowAdmissionSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Discharge)
            {
                this.ShowDischargeSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.SeriousAndCritical)
            {
                this.ShowSeriousPatSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Death)
            {
                this.ShowDeadPatSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.CheckedDoc)
            {
                this.ShowCheckDocSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.SpecialQC)
            {
                this.ShowSpecialQCSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.InHospitalDays)
            {
                this.ShowInHosptialDaysSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.DocType)
            {
                this.ShowDocTypeSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.MutiVisit)
            {
                this.ShowMutiVisitSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.TransferPatient)
            {
                this.ShowTransferPatientSearchUI();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Review)
            {
                this.ShowReviewSearchUI();
            }
            //病人ID,住院号,姓名检索
            else
            {
                this.ShowPatIDAndInpNoAndNameSearchUI();
            }

            this.OnComboBoxSelectItemChanged(EventArgs.Empty);

            GlobalMethods.UI.SetCursor(this.Parent, Cursors.Default);
        }


        /// <summary>
        /// 显示按照科室查询用户界面
        /// </summary>
        private void ShowDeptSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.cboSearchType.Items.Count > 0 && this.cboSearchType.SelectedIndex != 0)
                this.cboSearchType.SelectedIndex = 0;
            this.m_cboDeptList.BringToFront();
            this.m_cboDeptList.Focus();

            this.InitClinicDeptList();

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlDeptName", "pnlPatientStatus", "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);

            //初始化检索缺省值
            string szDeptCode = SystemConfig.Instance.Get(SystemData.ConfigKey.DEFAULT_DEPT_CODE, string.Empty);
            DeptInfo deptInfo = null;
            DataCache.Instance.GetDeptInfo(szDeptCode, ref deptInfo);
            if (deptInfo != null && this.m_cboDeptList != null)
                this.m_cboDeptList.SelectedItem = deptInfo;
            this.SetDeptBeginEndTime(chBoxShowTime.Checked);

            if (this.LastSearchType == EMRDBLib.PatSearchType.Unknown || this.m_eCurrSearchType == this.LastSearchType)
            {
                if (this.m_cboPatientType != null)
                {
                    this.m_cboPatientType.SelectedIndex = 1;
                }
                else if (this.m_cboPatientType != null)
                {
                    if (this.LastDeptPatientType == EMRDBLib.PatientType.AllPatient)
                        this.m_cboPatientType.SelectedIndex = 0;
                    else if (this.LastDeptPatientType == EMRDBLib.PatientType.PatInHosptial)
                        this.m_cboPatientType.SelectedIndex = 1;
                    else if (this.LastDeptPatientType == EMRDBLib.PatientType.PatOutHosptal)
                        this.m_cboPatientType.SelectedIndex = 2;
                }
            }
            //恢复可查询功能
            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 显示按照质检病历查询用户界面
        /// </summary>
        private void ShowCheckDocSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            this.fdbCheckName.BringToFront();
            this.fdbCheckName.Focus();

            this.InitClinicDeptList();

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlCheckedDoc", "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);
            if (this.m_dtpBeginTime != null)
                this.m_dtpBeginTime.Value = DateTime.Now.AddDays(-3);
            if (this.m_dtpEndTime != null)
                this.m_dtpEndTime.Value = DateTime.Now;
            //恢复可查询功能
            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 显示按照手术名称查询用户界面
        /// </summary>
        private void ShowOperationSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.cboSearchType.Items.Count > 8 && this.cboSearchType.SelectedIndex != 8)
                this.cboSearchType.SelectedIndex = 8;

            this.InitOperationList();
            this.InitClinicDeptList();

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlDeptName", "pnlPatientStatus", "pnlBeginDate", "pnlEndDate", "pnlOperationName" });

            this.ShowPanels(lstPanel);

            DeptInfo deptInfo = null;
            if (!string.IsNullOrEmpty(this.m_szLastOperDept))
                DataCache.Instance.GetDeptInfo(this.m_szLastOperDept, ref deptInfo);
            if (deptInfo != null)
                this.m_cboDeptList.SelectedItem = deptInfo;
            else
                this.m_cboDeptList.SelectedItem = null;
            if (this.m_lastOperPatientType == EMRDBLib.PatientType.AllPatient)
                this.m_cboPatientType.SelectedIndex = 0;
            else if (this.m_lastOperPatientType == EMRDBLib.PatientType.PatInHosptial)
                this.m_cboPatientType.SelectedIndex = 1;
            else if (this.m_lastOperPatientType == EMRDBLib.PatientType.PatOutHosptal)
                this.m_cboPatientType.SelectedIndex = 2;
            this.m_dtpBeginTime.Value = this.m_dtOperBeginTime;
            this.m_dtpEndTime.Value = this.m_dtOperEndTime;
            this.m_cboOperationList.SelectedItem = this.m_lastOperation;
            //恢复可查询功能
            this.m_bSearchEnabled = true;
        }



        /// <summary>
        /// 显示指定名称的Panel
        /// </summary>
        /// <param name="lstPanel">Panel Name List</param>
        private void ShowPanels(List<string> lstPanel)
        {
            foreach (System.Windows.Forms.Control ctl in this.Controls)
            {
                System.Type type = ctl.GetType();
                if (type == typeof(System.Windows.Forms.Panel) && lstPanel.Contains(ctl.Name))
                {
                    if (!ctl.Visible)
                    {
                        ctl.Visible = true;
                    }
                }
                else if (type == typeof(System.Windows.Forms.Panel) && ctl.Name != "pnlSearchType" && ctl.Name != "pnlSearchBtn")
                {
                    if (ctl.Visible)
                        ctl.Visible = false;
                }
            }
        }

        /// <summary>
        /// 显示病历类型检索面板
        /// </summary>
        private void ShowDocTypeSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlDocType", "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;
            this.m_dtpBeginTime.Value = this.m_dtLastDischargeTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastDischargeTimeEnd;
            this.m_bSearchEnabled = true;
        }
        /// <summary>
        /// 显示重复入院搜索面板
        /// </summary>
        private void ShowMutiVisitSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_dtpBeginTime.Value = this.m_dtLastDischargeTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastDischargeTimeEnd;

            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 显示转科患者搜索面板
        /// </summary>
        private void ShowTransferPatientSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_dtpBeginTime.Value = this.m_dtLastDischargeTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastDischargeTimeEnd;

            this.m_bSearchEnabled = true;
        }
        /// <summary>
        /// 显示转科患者搜索面板
        /// </summary>
        private void ShowReviewSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_dtpBeginTime.Value = this.m_dtLastDischargeTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastDischargeTimeEnd;

            this.m_bSearchEnabled = true;
        }

        /// <summary>
        ///  显示根据入院时间检索界面
        /// </summary>
        private void ShowAdmissionSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.cboSearchType.Items.Count > 1 && this.cboSearchType.SelectedIndex != 1)
                this.cboSearchType.SelectedIndex = 1;
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlPatientStatus", "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);

            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
                this.m_cboPatientType.SelectedIndex = 1;
            this.m_dtpBeginTime.Value = this.m_dtLastAdmissionTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastAdmissionTimeEnd;
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_cboPatientType.SelectedIndex = 1;
            this.InitClinicDeptList();
            if (this.DeptInfo != null)
            {
                this.m_cboDeptList.SelectedItem = null;
            }


            if (this.LastAdmPatientType == EMRDBLib.PatientType.AllPatient)
                this.m_cboPatientType.SelectedIndex = 0;
            else if (this.LastAdmPatientType == EMRDBLib.PatientType.PatInHosptial)
                this.m_cboPatientType.SelectedIndex = 1;
            else if (this.LastAdmPatientType == EMRDBLib.PatientType.PatOutHosptal)
                this.m_cboPatientType.SelectedIndex = 2;

            this.m_bSearchEnabled = true;
        }
        /// <summary>
        /// 显示按照出院患者检索UI
        /// </summary>
        private void ShowDischargeSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);

            if (this.cboSearchType.Items.Count > 2 && this.cboSearchType.SelectedIndex != 2)
                this.cboSearchType.SelectedIndex = 2;
            if (this.DeptInfo != null)
            {
                this.m_cboDeptList.SelectedItem = null;
            }
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_dtpBeginTime.Value = this.m_dtLastDischargeTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastDischargeTimeEnd;

            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 显示入院时间或出院时间病例检索界面
        /// </summary>
        private void ShowAdmAndDisPatSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
            {
                if (this.cboSearchType.Items.Count > 1 && this.cboSearchType.SelectedIndex != 1)
                    this.cboSearchType.SelectedIndex = 1;
            }
            else
            {
                if (this.cboSearchType.Items.Count > 2 && this.cboSearchType.SelectedIndex != 2)
                    this.cboSearchType.SelectedIndex = 2;
            }
            this.UpdateAdmAndDisAndSerAndDeathUI();
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
            {
                this.m_dtpBeginTime.Value = this.m_dtLastAdmissionTimeBegin;
                this.m_dtpEndTime.Value = this.m_dtLastAdmissionTimeEnd;
                if (this.LastAdmPatientType == EMRDBLib.PatientType.AllPatient)
                    this.m_cboPatientType.SelectedIndex = 0;
                else if (this.LastAdmPatientType == EMRDBLib.PatientType.PatInHosptial)
                    this.m_cboPatientType.SelectedIndex = 1;
                else if (this.LastAdmPatientType == EMRDBLib.PatientType.PatOutHosptal)
                    this.m_cboPatientType.SelectedIndex = 2;
            }
            else
            {
                this.m_dtpBeginTime.Value = this.m_dtLastDischargeTimeBegin;
                this.m_dtpEndTime.Value = this.m_dtLastDischargeTimeEnd;
            }
            this.m_bSearchEnabled = true;
        }
        /// <summary>
        /// 显示危重患者检索UI
        /// </summary>
        private void ShowSeriousPatSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.cboSearchType.Items.Count > 3 && this.cboSearchType.SelectedIndex != 3)
                this.cboSearchType.SelectedIndex = 3;

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);

            if (this.DeptInfo != null)
            {
                this.m_cboDeptList.SelectedItem = null;
            }
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_dtpBeginTime.Value = this.m_dtLastSerCriAdmissionTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastSerCriAdmissionTimeEnd;

            this.m_bSearchEnabled = true;
        }
        /// <summary>
        /// 显示死亡患者检索UI
        /// </summary>
        private void ShowDeadPatSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.cboSearchType.Items.Count > 4 && this.cboSearchType.SelectedIndex != 4)
                this.cboSearchType.SelectedIndex = 4;

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlBeginDate", "pnlEndDate" });
            this.ShowPanels(lstPanel);

            if (this.DeptInfo != null)
            {
                this.m_cboDeptList.SelectedItem = null;
            }
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            this.m_dtpBeginTime.Value = this.m_dtLastDeathTimeBegin;
            this.m_dtpEndTime.Value = this.m_dtLastDeathTimeEnd;
            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 更新入院时间、出院时间、危重病历和死亡病历的UI界面
        /// </summary>
        private void UpdateAdmAndDisAndSerAndDeathUI()
        {
            //如果是按入院时间检索，则显示患者类型下拉框
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
            {
                List<string> lstPanel = new List<string>();
                lstPanel.AddRange(new string[] { "pnlPatientStatus", "pnlBeginDate", "pnlEndDate" });
                this.ShowPanels(lstPanel);
            }

            if (this.DeptInfo != null)
            {
                this.m_cboDeptList.SelectedItem = null;
            }
            this.m_dtpBeginTime.BringToFront();
            this.m_dtpBeginTime.Focus();
            this.m_dtpBeginTime.ImeMode = ImeMode.Alpha;

            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
                this.m_cboPatientType.SelectedIndex = 1;
        }

        /// <summary>
        /// 显示住院天数检索界面
        /// </summary>
        private void ShowInHosptialDaysSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.InHospitalDays)
            {
                if (this.cboSearchType != null && this.cboSearchType.Items.Count > 9 && this.cboSearchType.SelectedIndex != 9)
                    this.cboSearchType.SelectedIndex = 9;
            }
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlDeptName", "pnlPatientStatus", "pnlInHosDays" });
            this.ShowPanels(lstPanel);
            this.InitClinicDeptList();
            DeptInfo deptInfo = null;
            if (!string.IsNullOrEmpty(this.m_szLastInHospDept))
                DataCache.Instance.GetDeptInfo(this.m_szLastInHospDept, ref deptInfo);
            if (this.m_cboDeptList != null)
            {
                if (deptInfo != null)
                    this.m_cboDeptList.SelectedItem = deptInfo;
                else
                    this.m_cboDeptList.SelectedItem = null;
            }
            if (this.m_cboPatientType != null)
            {
                if (this.m_lastInHospPatientType == EMRDBLib.PatientType.AllPatient)
                    this.m_cboPatientType.SelectedIndex = 0;
                else if (this.m_lastInHospPatientType == EMRDBLib.PatientType.PatInHosptial)
                    this.m_cboPatientType.SelectedIndex = 1;
                else if (this.m_lastInHospPatientType == EMRDBLib.PatientType.PatOutHosptal)
                    this.m_cboPatientType.SelectedIndex = 2;
            }
            if (this.m_cboOperatorType != null)
            {
                if (this.m_lastOperatorType == EMRDBLib.OperatorType.Morethan)
                    this.m_cboOperatorType.SelectedIndex = 0;
                else if (this.m_lastOperatorType == EMRDBLib.OperatorType.Lessthan)
                    this.m_cboOperatorType.SelectedIndex = 1;
                else if (this.m_lastOperatorType == EMRDBLib.OperatorType.Equalthan)
                    this.m_cboOperatorType.SelectedIndex = 2;
                else if (this.m_lastOperatorType == EMRDBLib.OperatorType.MoreEqualthan)
                    this.m_cboOperatorType.SelectedIndex = 3;
                else if (this.m_lastOperatorType == EMRDBLib.OperatorType.LessEqualthan)
                    this.m_cboOperatorType.SelectedIndex = 4;
            }
            if (this.m_nLastInHospDays > 0 && this.m_txtPatientInfo != null)
                this.m_txtPatientInfo.Text = this.m_nLastInHospDays.ToString();
            if (this.m_cboOperatorType != null && (this.m_OperatorType == EMRDBLib.OperatorType.UnKnown || this.m_eCurrSearchType == this.LastSearchType))
                this.m_cboOperatorType.SelectedIndex = 0;

            this.m_txtInHospDays.BringToFront();
            this.m_txtInHospDays.Focus();
            this.m_txtInHospDays.ImeMode = ImeMode.Alpha;

            this.m_bSearchEnabled = true;
        }


        /// <summary>
        /// 显示专家质检检索界面
        /// </summary>
        private void ShowSpecialQCSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;
            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlSpecial", "panSpecialSearch" });
            this.ShowPanels(lstPanel);
            this.InitSpecialCheckList();
            //恢复可查询功能
            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 显示患者ID或者病案号检索界面
        /// </summary>
        private void ShowPatIDAndInpNoAndNameSearchUI()
        {
            //切换UI的过程中关闭检索功能,待切换完成后再开启
            this.m_bSearchEnabled = false;

            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.PatientID)
            {
                if (this.cboSearchType.Items.Count > 5 && this.cboSearchType.SelectedIndex != 5)
                    this.cboSearchType.SelectedIndex = 5;
                this.lblPatient.Text = "患者ID号";
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.PatientName)
            {
                if (this.cboSearchType.Items.Count > 6 && this.cboSearchType.SelectedIndex != 6)
                    this.cboSearchType.SelectedIndex = 6;
                this.lblPatient.Text = "患者姓名";
            }
            else
            {
                if (this.cboSearchType.Items.Count > 7 && this.cboSearchType.SelectedIndex != 7)
                    this.cboSearchType.SelectedIndex = 7;
                this.lblPatient.Text = "病 案 号";
            }

            this.m_txtPatientInfo.BringToFront();
            this.m_txtPatientInfo.Focus();
            this.m_txtPatientInfo.ImeMode = ImeMode.Alpha;

            List<string> lstPanel = new List<string>();
            lstPanel.AddRange(new string[] { "pnlPatient" });
            this.ShowPanels(lstPanel);

            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.PatientID)
                this.m_txtPatientInfo.Text = this.m_szLastPatientID;
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.PatientName)
                this.m_txtPatientInfo.Text = this.m_szLastPatientName;
            else
                this.m_txtPatientInfo.Text = this.m_szLastInpNo;
            this.m_txtPatientInfo.SelectAll();
            this.m_bSearchEnabled = true;
        }

        /// <summary>
        /// 保存检索使用的条件值
        /// </summary>
        private void SaveSearchValue()
        {
            if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Department)
            {
                string szDeptCode = string.Empty;
                if (this.DeptInfo != null)
                    szDeptCode = this.DeptInfo.DEPT_CODE;
                SystemConfig.Instance.Write(SystemData.ConfigKey.DEFAULT_DEPT_CODE, szDeptCode);
                SystemConfig.Instance.Write(SystemData.ConfigKey.DEPT_DEFAULT_ADMISSION_BEGIN, this.AdmissionTimeBegin.ToString());
                SystemConfig.Instance.Write(SystemData.ConfigKey.DEPT_DEFAULT_ADMISSION_END, this.AdmissionTimeEnd.ToString());
                this.m_LastDeptPatientType = this.PatientType;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.OperationPatient)
            {
                if (this.DeptInfo != null)
                    this.m_szLastOperDept = this.DeptInfo.DEPT_CODE;
                if (this.m_cboPatientType.SelectedItem != null)
                {
                    if (this.m_cboPatientType.SelectedItem.ToString() == "在院患者")
                        this.m_lastOperPatientType = EMRDBLib.PatientType.PatInHosptial;
                    else if (this.m_cboPatientType.SelectedItem.ToString() == "出院患者")
                        this.m_lastOperPatientType = EMRDBLib.PatientType.PatOutHosptal;
                    else if (this.m_cboPatientType.SelectedItem.ToString() == "所有患者")
                        this.m_lastOperPatientType = EMRDBLib.PatientType.AllPatient;
                }
                this.m_dtOperBeginTime = this.m_dtpBeginTime.Value;
                this.m_dtOperEndTime = this.m_dtpEndTime.Value;
                if (this.m_cboOperationList.SelectedItem != null)
                    this.m_lastOperation = this.m_cboOperationList.SelectedItem as EMRDBLib.OperationDict;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Admission)
            {
                this.m_dtLastAdmissionTimeBegin = this.AdmissionTimeBegin;
                this.m_dtLastAdmissionTimeEnd = this.AdmissionTimeEnd;
                this.m_LastAdmPatientType = this.PatientType;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.DocType)
            {
                this.m_dtLastAdmissionTimeBegin = this.AdmissionTimeBegin;
                this.m_dtLastAdmissionTimeEnd = this.AdmissionTimeEnd;
                this.m_DocType = this.m_cboDocTypeList.SelectedItem as DocTypeInfo;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Discharge)
            {
                this.m_dtLastDischargeTimeBegin = this.DischargeTimeBegin;
                this.m_dtLastDischargeTimeEnd = this.DischargeTimeEnd;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.Death)
            {
                this.m_dtLastDeathTimeBegin = this.DeathTimeBegin;
                this.m_dtLastDeathTimeEnd = this.DeathTimeEnd;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.SeriousAndCritical)
            {
                this.m_dtLastSerCriAdmissionTimeBegin = this.SerCriAdmissionTimeBegin;
                this.m_dtLastSerCriAdmissionTimeEnd = this.SerCriAdmissionTimeEnd;
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.PatientID)
            {
                this.m_szLastPatientID = this.m_txtPatientInfo.Text.Trim();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.PatientName)
            {
                this.m_szLastPatientName = this.m_txtPatientInfo.Text.Trim();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.InHospitalID)
            {
                this.m_szLastInpNo = this.m_txtPatientInfo.Text.Trim();
            }
            else if (this.m_eCurrSearchType == EMRDBLib.PatSearchType.InHospitalDays)
            {
                if (this.DeptInfo != null)
                    this.m_szLastInHospDept = this.DeptInfo.DEPT_CODE;
                if (this.m_cboPatientType.SelectedItem != null)
                {
                    if (this.m_cboPatientType.SelectedItem.ToString() == "在院患者")
                        this.m_lastInHospPatientType = EMRDBLib.PatientType.PatInHosptial;
                    else if (this.m_cboPatientType.SelectedItem.ToString() == "出院患者")
                        this.m_lastInHospPatientType = EMRDBLib.PatientType.PatOutHosptal;
                    else if (this.m_cboPatientType.SelectedItem.ToString() == "所有患者")
                        this.m_lastInHospPatientType = EMRDBLib.PatientType.AllPatient;
                }
                if (this.m_cboOperatorType.SelectedItem != null)
                {
                    if (this.m_cboOperatorType.SelectedItem.ToString() == "大于")
                        this.m_lastOperatorType = EMRDBLib.OperatorType.Morethan;
                    else if (this.m_cboOperatorType.SelectedItem.ToString() == "小于")
                        this.m_lastOperatorType = EMRDBLib.OperatorType.Lessthan;
                    else if (this.m_cboOperatorType.SelectedItem.ToString() == "大于等于")
                        this.m_lastOperatorType = EMRDBLib.OperatorType.MoreEqualthan;
                    else if (this.m_cboOperatorType.SelectedItem.ToString() == "小于等于")
                        this.m_lastOperatorType = EMRDBLib.OperatorType.LessEqualthan;
                    else if (this.m_cboOperatorType.SelectedItem.ToString() == "等于")
                        this.m_lastOperatorType = EMRDBLib.OperatorType.Equalthan;
                }
                if (!GlobalMethods.Misc.IsEmptyString(this.m_txtInHospDays.Text.Trim()))
                    this.m_nLastInHospDays = GlobalMethods.Convert.StringToValue(this.m_txtInHospDays.Text.Trim(), 0);
            }
        }

        /// <summary>
        /// 按科室检索的情况下,初始化临床科室列表
        /// </summary>
        private void InitClinicDeptList()
        {
            if (this.m_cboDeptList == null || this.m_cboDeptList.IsDisposed)
                return;
            if (this.m_cboDeptList.Items.Count > 0)
                return;
            this.OnStatusMessageChanged("正在下载临床科室列表，请稍候...");
            List<DeptInfo> lstDeptInfos = null;
            if (DeptAccess.Instance.GetClinicInpDeptList(ref lstDeptInfos) != SystemData.ReturnValue.OK)
            {
                this.OnStatusMessageChanged(null);
                MessageBoxEx.Show("临床科室列表下载失败!");
                return;
            }
            if (lstDeptInfos == null || lstDeptInfos.Count <= 0)
            {
                this.OnStatusMessageChanged(null);
                return;
            }
            for (int index = 0; index < lstDeptInfos.Count; index++)
            {
                DeptInfo deptInfo = lstDeptInfos[index];
                this.m_cboDeptList.Items.Add(deptInfo);
            }
            this.OnStatusMessageChanged(null);
        }
        /// <summary>
        /// 初始化选中的科室
        /// </summary>
        private void InitSelectedDept()
        {
            //初始化默认检索方式
            if (this.cboSearchType == null || this.cboSearchType.IsDisposed)
                return;
            this.cboSearchType.SelectedIndex = -1;
            this.cboSearchType.SelectedIndex = 0;

            //初始化选中的科室
            string szDeptCode = SystemConfig.Instance.Get(SystemData.ConfigKey.DEFAULT_DEPT_CODE, string.Empty);
            DeptInfo deptInfo = null;
            DataCache.Instance.GetDeptInfo(szDeptCode, ref deptInfo);
            if (deptInfo != null && this.m_cboDeptList != null)
                this.m_cboDeptList.SelectedItem = deptInfo;
        }

        /// <summary>
        /// 初始化病历类型
        /// </summary>
        private void InitDocType()
        {
            this.m_cboDocTypeList = this.fdCBXDocType;
            if (this.m_cboDocTypeList == null || this.m_cboDocTypeList.IsDisposed)
                return;
            if (this.m_cboDocTypeList.Items.Count > 0)
                return;
            this.OnStatusMessageChanged("正在下载文书类型列表，请稍候...");
            List<DocTypeInfo> lstDocTypeInfos = null;
            short shRet = DocTypeAccess.Instance.GetDocTypeInfos(ref lstDocTypeInfos);
            if (shRet != SystemData.ReturnValue.OK)
            {
                this.OnStatusMessageChanged(null);
                MessageBoxEx.Show("文书类型列表下载失败！");
                return;
            }
            foreach (DocTypeInfo docTypeInfo in lstDocTypeInfos)
            {
                if (!docTypeInfo.CanCreate || !docTypeInfo.IsValid)
                    continue;
                this.m_cboDocTypeList.Items.Add(docTypeInfo);
            }
            this.OnStatusMessageChanged(null);
        }
        /// <summary>
        /// 初始化质检病历
        /// </summary>
        private void InitCheckedDoc()
        {
            if (this.fdbCheckName == null || this.fdbCheckName.IsDisposed)
                return;
            if (this.fdbCheckName.Items.Count > 0)
                return;
            this.OnStatusMessageChanged("正在下载质检人员列表，请稍候...");

            List<UserRightBase> lstUserRight = null;
            short shRet = RightAccess.Instance.GetUserRight(UserRightType.MedQC, ref lstUserRight);
            //获取有提交质检问题权限的人员信息
            if (shRet != SystemData.ReturnValue.OK)
            {
                this.OnStatusMessageChanged(null);
                //MessageBoxEx.Show("用户权限列表下载失败！");
                return;
            }
            if (lstUserRight == null || lstUserRight.Count == 0)
                return;

            //获取所有人员信息
            List<UserInfo> lstAllUserInfo = null;
            UserAccess.Instance.GetAllUserInfos(ref lstAllUserInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                this.OnStatusMessageChanged(null);
                //MessageBoxEx.Show("用户权限列表下载失败！");
                return;
            }
            if (lstAllUserInfo == null || lstAllUserInfo.Count == 0)
                return;
            List<UserInfo> lstUserInfoWithQCRight = new List<UserInfo>();
            foreach (QCUserRight user in lstUserRight)
            {
                //质控专家不会很，循环查询
                bool IsSpecialDoc = user.CommitQCQuestion.Value;
                if (!IsSpecialDoc)
                    continue;
                UserInfo userInfo = null;
                userInfo = lstAllUserInfo.Find(delegate (UserInfo u) { return u.USER_ID == user.UserID; });
                if (userInfo != null)
                {
                    lstUserInfoWithQCRight.Add(userInfo);
                }
            }
            if (lstUserInfoWithQCRight == null || lstUserInfoWithQCRight.Count <= 0)
            {
                this.OnStatusMessageChanged(null);
                return;
            }

            for (int index = 0; index < lstUserInfoWithQCRight.Count; index++)
            {
                UserInfo userInfo = lstUserInfoWithQCRight[index];
                string szInputCode = GlobalMethods.Convert.GetInputCode(userInfo.USER_NAME, false, 10);
                this.fdbCheckName.Items.Add(userInfo);
            }
            this.OnStatusMessageChanged(null);
        }

        /// <summary>
        /// 按手术患者检索，初始化手术名称列表
        /// </summary>
        private void InitOperationList()
        {
            if (this.m_cboOperationList == null || this.m_cboOperationList.IsDisposed)
                return;
            if (this.m_cboOperationList.Items.Count > 0)
                return;
            this.OnStatusMessageChanged("正在下载手术名称列表，请稍候...");
            List<EMRDBLib.OperationDict> lstOperationDict = null;
            short shRet = OperationDictAccess.Instance.GetOperationDict(ref lstOperationDict);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
            {
                this.OnStatusMessageChanged(null);
                return;
            }
            if (lstOperationDict == null || lstOperationDict.Count <= 0)
            {
                this.OnStatusMessageChanged(null);
                return;
            }
            for (int index = 0; index < lstOperationDict.Count; index++)
            {
                EMRDBLib.OperationDict operationDict = lstOperationDict[index];
                this.m_cboOperationList.Items.Add(operationDict);
            }
            this.OnStatusMessageChanged(null);
        }

        private void PatientInfoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter)
                return;
            if (!this.m_bSearchEnabled)
                return;
            GlobalMethods.UI.SetCursor(this.m_txtPatientInfo, Cursors.WaitCursor);
            this.OnStartSearch(EventArgs.Empty);
            GlobalMethods.UI.SetCursor(this.m_txtPatientInfo, Cursors.IBeam);
        }

        private void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.m_bSearchEnabled)
                return;
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            ////如果没有科室质控、管辖科室和全院质控权限，则只允许使用患者ID,患者姓名和住院号三种检索方式
            //if (!SystemParam.Instance.QCUserRight.ManageAllQC.Value
            //    && !SystemParam.Instance.QCUserRight.ManageDeptQC.Value
            //    && !SystemParam.Instance.QCUserRight.ManageAdminDeptsQC.Value)
            //{
            //    if (this.cboSearchType.SelectedIndex == 0)
            //        this.SearchType = EMRDBLib.PatSearchType.PatientID;
            //    else if (this.cboSearchType.SelectedIndex == 1)
            //        this.SearchType = EMRDBLib.PatSearchType.PatientName;
            //    else if (this.cboSearchType.SelectedIndex == 2)
            //        this.SearchType = EMRDBLib.PatSearchType.InHospitalID;
            //    GlobalMethods.UI.SetCursor(this, Cursors.Default);
            //    return;
            //}

            if (this.cboSearchType.SelectedIndex == 0)
                this.SearchType = EMRDBLib.PatSearchType.Department;
            else if (this.cboSearchType.SelectedIndex == 1)
                this.SearchType = EMRDBLib.PatSearchType.Admission;
            else if (this.cboSearchType.SelectedIndex == 2)
                this.SearchType = EMRDBLib.PatSearchType.Discharge;
            else if (this.cboSearchType.SelectedIndex == 3)
                this.SearchType = EMRDBLib.PatSearchType.SeriousAndCritical;
            else if (this.cboSearchType.SelectedIndex == 4)
                this.SearchType = EMRDBLib.PatSearchType.Death;
            else if (this.cboSearchType.SelectedIndex == 5)
                this.SearchType = EMRDBLib.PatSearchType.PatientID;
            else if (this.cboSearchType.SelectedIndex == 6)
                this.SearchType = EMRDBLib.PatSearchType.PatientName;
            else if (this.cboSearchType.SelectedIndex == 7)
                this.SearchType = EMRDBLib.PatSearchType.InHospitalID;
            else if (this.cboSearchType.SelectedIndex == 8)
                this.SearchType = EMRDBLib.PatSearchType.OperationPatient;
            else if (this.cboSearchType.SelectedIndex == 9)
                this.SearchType = EMRDBLib.PatSearchType.InHospitalDays;
            else if (this.cboSearchType.SelectedIndex == 10)
                this.SearchType = EMRDBLib.PatSearchType.CheckedDoc;
            else if (this.cboSearchType.SelectedItem.ToString() == "专家质控")
                this.SearchType = EMRDBLib.PatSearchType.SpecialQC;
            else if (this.cboSearchType.SelectedItem.ToString() == "病历类型")
                this.SearchType = EMRDBLib.PatSearchType.DocType;
            else if (this.cboSearchType.SelectedItem.ToString() == "重复入院")
                this.SearchType = EMRDBLib.PatSearchType.MutiVisit;
            else if (this.cboSearchType.SelectedItem.ToString() == "转科患者")
                this.SearchType = EMRDBLib.PatSearchType.TransferPatient;
            else if (this.cboSearchType.SelectedItem.ToString() == "待复审")
                this.SearchType = EMRDBLib.PatSearchType.Review;
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void DeptListComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter)
                return;
            if (!this.m_bSearchEnabled)
                return;

            this.OnComboBoxSelectItemChanged(EventArgs.Empty);
        }

        private void DeptListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.m_bSearchEnabled)
                return;

            this.OnComboBoxSelectItemChanged(EventArgs.Empty);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            StartSearchClick();
        }

        private void StartSearchClick()
        {
            if (!this.m_bSearchEnabled)
                return;
            this.OnStartSearch(EventArgs.Empty);
        }

        private void txtInHospDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)((int)Keys.Back))
                return;
            if (e.KeyChar.CompareTo('0') >= 0 && e.KeyChar.CompareTo('9') <= 0)
                return;
            e.Handled = true;
        }

        /// <summary>
        /// 按专家质控检索，初始化抽检批次名称列表
        /// </summary>
        private void InitSpecialCheckList()
        {
            if (this.m_cboSpecialCheckList == null || this.m_cboSpecialCheckList.IsDisposed)
                return;
            this.m_cboSpecialCheckList.Items.Clear();
            this.OnStatusMessageChanged("正在下载病案抽检批次列表，请稍候...");
            List<EMRDBLib.QcSpecialCheck> lstQcSpecialCheck = null;
            DateTime dtStartTime = DateTime.MinValue;
            DateTime dtEndTime = DateTime.Now;
            short shRet = SpecialAccess.Instance.GetQCSpecialCheckList(dtStartTime, dtEndTime, ref lstQcSpecialCheck);
            if (shRet != SystemData.ReturnValue.OK)
            {
                this.OnStatusMessageChanged(null);
                return;
            }
            foreach (EMRDBLib.QcSpecialCheck item in lstQcSpecialCheck)
            {
                if (!item.Checked)
                    this.m_cboSpecialCheckList.Items.Add(item.Name, item);
            }
            this.OnStatusMessageChanged(null);
        }

        private void chBoxShowTime_CheckedChanged(object sender, EventArgs e)
        {

            SetDeptBeginEndTime(chBoxShowTime.Checked);
        }

        /// <summary>
        /// 根据选择是否显示时间复选框设置起始
        /// </summary>
        /// <param name="bShow">是否显示</param>
        private void SetDeptBeginEndTime(bool bShow)
        {
            if (!this.chBoxShowTime.Checked)
            {
                this.pnlBeginDate.Visible = false;
                this.pnlEndDate.Visible = false;
                this.m_dtpBeginTime.Value = Convert.ToDateTime("1990 1/1 00:00:00");
                this.m_dtpEndTime.Value = DateTime.Now;
            }
            else
            {
                this.pnlBeginDate.Visible = true;
                this.pnlEndDate.Visible = true;
                this.m_dtpEndTime.Value = DateTime.Now;
                DateTime dtStartTime = DateTime.Now;
                dtStartTime = dtStartTime.AddDays(-SystemParam.Instance.LocalConfigOption.SearchTimeSpanDays);
                dtStartTime = dtStartTime.AddMonths(-SystemParam.Instance.LocalConfigOption.SearchTimeSpanMonths);
                this.m_dtpBeginTime.Value = dtStartTime;
            }
            this.SetHeight();
        }

        void m_cboPatientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SearchType != EMRDBLib.PatSearchType.Department)
                return;
            bool bShow = false;
            if (this.m_cboPatientType.SelectedIndex == 1)//在院患者
            {
                if (this.chBoxShowTime.Visible == false)
                    this.chBoxShowTime.Visible = true;
                bShow = this.chBoxShowTime.Checked;

                this.chBoxShowTime.Enabled = true;
            }
            else
            {
                this.chBoxShowTime.Checked = true;
                this.chBoxShowTime.Enabled = false;
                bShow = true;
                //SetDeptBeginEndTime(bShow);
            }
            SetDeptBeginEndTime(bShow);
        }


        private void rdbSpecialMine_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbSpecialMine.Checked)
                StartSearchClick();
        }

        private void rdbSpecialAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbSpecialAll.Checked)
                StartSearchClick();
        }

        private void PatSearchPane_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void fdCBXDept_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartSearchClick();
            }
        }
        private void fdCBXOperation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartSearchClick();
            }
        }
        private ComplexSearchForm m_frmComplexSearch;

        private void btnComplex_Click(object sender, EventArgs e)
        {
            if (this.m_frmComplexSearch == null)
                this.m_frmComplexSearch = new ComplexSearchForm();
            if (this.m_frmComplexSearch.ShowDialog() == DialogResult.OK)
            {
                List<PatVisitInfo> lstPatVisitLog = this.m_frmComplexSearch.LstPatVisitLog;
                if (this.PatListForm == null)
                    return;
                if (SystemParam.Instance.PatVisitLogTable == null)
                    SystemParam.Instance.PatVisitLogTable = new Hashtable();
                else
                    SystemParam.Instance.PatVisitLogTable.Clear();
                //清空系统中各数据窗口中的数据
                this.PatListForm.ClearPatList();
                this.PatListForm.LoadPatientVisitList(lstPatVisitLog, true);
                SystemParam.Instance.PatVisitLogList = lstPatVisitLog;
                return;
            }
            return;
        }
    }
}
