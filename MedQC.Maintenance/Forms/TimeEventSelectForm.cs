// ***********************************************************
// �����༭�����ù���ϵͳʱЧ�¼�ѡ�񴰿�.
// Creator:YangMingkun  Date:2012-1-3
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using Heren.Common.Controls;
using MedQCSys.DockForms;
using EMRDBLib.Entity;

namespace Heren.MedQC.Maintenance
{
    internal partial class TimeEventSelectForm :  DockContentBase
    {
        private List<TimeQCEvent> m_lstTimeQCEvents = null;
        /// <summary>
        /// ��ȡ�����õ�ǰ�����¼��б�
        /// </summary>
        public List<TimeQCEvent> TimeQCEvents
        {
            get { return this.m_lstTimeQCEvents; }
            set
            {
                if (value == null)
                    this.m_lstTimeQCEvents.Clear();
                else
                    this.m_lstTimeQCEvents = value;
            }
        }

        private TimeQCEvent m_selectedEvent = null;
        /// <summary>
        /// ��ȡ�����õ�ǰѡ�е������¼�����
        /// </summary>
        public TimeQCEvent SelectedEvent
        {
            get { return this.m_selectedEvent; }
            set { this.m_selectedEvent = value; }
        }

        public TimeEventSelectForm()
        {
            this.InitializeComponent();
            this.m_lstTimeQCEvents = new List<TimeQCEvent>();
        }

        /// <summary>
        /// �����ݿ����ʱЧ�¼��б�
        /// </summary>
        private void LoadTimeEventList()
        {
            List<TimeQCEvent> lstTimeQCEvents = null;
            short result = EMRDBLib.DbAccess.TimeQCEventAccess.Instance.GetTimeQCEvents(ref lstTimeQCEvents);
            if (result !=EMRDBLib.SystemData.ReturnValue.OK
                && result!=EMRDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.ShowError("ʱЧ�¼��б�����ʧ��!");
                return;
            }
            this.LoadTimeEventList(lstTimeQCEvents);
        }

        /// <summary>
        /// ���ⲿ���õ�ʱЧ�¼��б����
        /// </summary>
        /// <param name="lstTimeQCEvents">�ⲿ���õ�ʱЧ�¼��б�</param>
        private void LoadTimeEventList(List<TimeQCEvent> lstTimeQCEvents)
        {
            this.listView1.Items.Clear();
            if (lstTimeQCEvents == null || lstTimeQCEvents.Count <= 0)
                return;
            for (int index = 0; index < lstTimeQCEvents.Count; index++)
            {
                TimeQCEvent timeQCEvent = lstTimeQCEvents[index];
                if (timeQCEvent == null)
                    continue;
                if (string.IsNullOrEmpty(timeQCEvent.EventID))
                    continue;
                if (string.IsNullOrEmpty(timeQCEvent.EventName))
                    continue;
                ListViewItem item = new ListViewItem(timeQCEvent.EventName);
                item.Tag = timeQCEvent;
                this.listView1.Items.Add(item);

                if (this.m_selectedEvent != null
                    && this.m_selectedEvent.EventID == timeQCEvent.EventID)
                    item.Selected = true;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Update();
            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            if (this.m_lstTimeQCEvents.Count <= 0)
                this.LoadTimeEventList();
            else
                this.LoadTimeEventList(this.m_lstTimeQCEvents);
            GlobalMethods.UI.SetCursor(this, Cursors.Default);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count <= 0)
                return;
            this.m_selectedEvent = this.listView1.SelectedItems[0].Tag as TimeQCEvent;
            if (this.m_selectedEvent != null)
                this.DialogResult = DialogResult.OK;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.m_selectedEvent = null;
            if (this.listView1.SelectedItems.Count > 0)
                this.m_selectedEvent = this.listView1.SelectedItems[0].Tag as TimeQCEvent;
            this.DialogResult = DialogResult.OK;
        }
    }
}