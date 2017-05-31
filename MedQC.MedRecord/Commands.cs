﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heren.MedQC.Core;
using Heren.Common.DockSuite;
using MedQCSys.Dialogs;
using Heren.Common.Libraries;
using System.Windows.Forms;
using EMRDBLib;
using MedQCSys.DockForms;
using MedQCSys;
using System.IO;
using System.Diagnostics;
using Heren.MedQC.MedRecord.Dialogs;

namespace Heren.MedQC.MedRecord
{
    public class ShowSystemCommand1 : AbstractCommand
    {
        public ShowSystemCommand1()
        {
            this.m_name = "在院患者";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is PatInHospitalListForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            PatInHospitalListForm role = new PatInHospitalListForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand2 : AbstractCommand
    {
        public ShowSystemCommand2()
        {
            this.m_name = "出院患者";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is PatOutHospitalListForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            PatOutHospitalListForm role = new PatOutHospitalListForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand3 : AbstractCommand
    {
        public ShowSystemCommand3()
        {
            this.m_name = "病案归档";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is MrArchiveForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            MrArchiveForm role = new MrArchiveForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand4 : AbstractCommand
    {
        public ShowSystemCommand4()
        {
            this.m_name = "病案归档率统计";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is MrArchiveRateForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            MrArchiveRateForm role = new MrArchiveRateForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand5 : AbstractCommand
    {
        public ShowSystemCommand5()
        {
            this.m_name = "病案提交率统计";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is MrSubmitRateForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            MrSubmitRateForm role = new MrSubmitRateForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand6 : AbstractCommand
    {
        public ShowSystemCommand6()
        {
            this.m_name = "纸质病历批次查询";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecMrBatchForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecMrBatchForm role = new RecMrBatchForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand7 : AbstractCommand
    {
        public ShowSystemCommand7()
        {
            this.m_name = "纸质病历逐份接收";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is ReceiveInOrderForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            ReceiveInOrderForm role = new ReceiveInOrderForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand8 : AbstractCommand
    {
        public ShowSystemCommand8()
        {
            this.m_name = "病历打包";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecPackForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecPackForm role = new RecPackForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand9 : AbstractCommand
    {
        public ShowSystemCommand9()
        {
            this.m_name = "病历装箱";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecCaseForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecCaseForm role = new RecCaseForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand10 : AbstractCommand
    {
        public ShowSystemCommand10()
        {
            this.m_name = "病历打包查询";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecPackSearchForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecPackSearchForm role = new RecPackSearchForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand11 : AbstractCommand
    {
        public ShowSystemCommand11()
        {
            this.m_name = "缺陷率统计";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is MrDefectRateForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            MrDefectRateForm role = new MrDefectRateForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand12 : AbstractCommand
    {
        public ShowSystemCommand12()
        {
            this.m_name = "复印登记";
        }
        public override bool Execute(object param, object data)
        {
            RecPrintLogForm frm = new RecPrintLogForm();
            if (frm.ShowDialog() == DialogResult.OK)
                return true;
            return false;
        }
    }
    public class ShowSystemCommand13 : AbstractCommand
    {
        public ShowSystemCommand13()
        {
            this.m_name = "复印查询";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecPrintLogSearchForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecPrintLogSearchForm role = new RecPrintLogSearchForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand14 : AbstractCommand
    {
        public ShowSystemCommand14()
        {
            this.m_name = "复印金额统计";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecPrintLogMoneyStatisticForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecPrintLogMoneyStatisticForm role = new RecPrintLogMoneyStatisticForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand15 : AbstractCommand
    {
        public ShowSystemCommand15()
        {
            this.m_name = "病历评分工作量统计";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is MrScoreWorkloadStatisticForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            MrScoreWorkloadStatisticForm role = new MrScoreWorkloadStatisticForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand16 : AbstractCommand
    {
        public ShowSystemCommand16()
        {
            this.m_name = "病案归档一览表";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is MrArchiveTableForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            MrArchiveTableForm role = new MrArchiveTableForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand17 : AbstractCommand
    {
        public ShowSystemCommand17()
        {
            this.m_name = "病案上传";
        }
        public override bool Execute(object param, object data)
        {
            MainForm form = param as MainForm;
            if (form == null)
                return false;
            foreach (DockContentBase item in form.DockPanel.Contents)
            {
                if (item is RecUploadForm)
                {
                    item.Activate();
                    item.OnRefreshView();
                    return true;
                }
            }
            RecUploadForm role = new RecUploadForm(form);
            role.Show(form.DockPanel, DockState.Document);
            role.Activate();
            role.OnRefreshView();
            return true;
        }
    }
    public class ShowSystemCommand18 : AbstractCommand
    {
        public ShowSystemCommand18()
        {
            this.m_name = "节假日设置";
        }
        public override bool Execute(object param, object data)
        {
            HolidaySettingForm frm = new HolidaySettingForm();
            if (frm.ShowDialog() == DialogResult.OK)
                return true;
            return false;
        }
    }
}
