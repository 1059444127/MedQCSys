// ***********************************************************
// �����ʿ�ϵͳ�û������޸ĶԻ���.
// Creator:YangMingkun  Date:2009-11-13
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Heren.Common.Controls;
using Heren.Common.DockSuite;
using Heren.Common.Libraries;

using EMRDBLib.DbAccess;
using EMRDBLib;

namespace Heren.MedQC.Maintenance
{
    public partial class ModifyPwdForm : HerenForm
    {
        public ModifyPwdForm()
        {
            this.InitializeComponent();
            this.Icon =Heren.MedQC.Maintenance.Properties.Resources.LoginIcon;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (SystemParam.Instance.UserInfo == null)
            {
                MessageBoxEx.Show("����û�е�¼ϵͳ,�����޸Ŀ���!");
                return;
            }

            string szOldPwd = this.txtOldPwd.Text;
            string szNewPwd = this.txtNewPwd.Text;
            string szConfirmPwd = this.txtConfirmPwd.Text;
            if (szNewPwd.CompareTo(szConfirmPwd) != 0)
            {
                MessageBoxEx.Show("�������ȷ�Ͽ����ȷ!");
                this.txtConfirmPwd.Focus();
                this.txtConfirmPwd.SelectAll();
                return;
            }

            string szUserID = SystemParam.Instance.UserInfo.ID;

            GlobalMethods.UI.SetCursor(this, Cursors.WaitCursor);
            short shRet = RightAccess.Instance.ModifyUserPwd(szUserID, szOldPwd, szNewPwd);

            GlobalMethods.UI.SetCursor(this, Cursors.Default);
            if (shRet == SystemData.ReturnValue.CANCEL)
            {
                MessageBoxEx.Show("��û��Ȩ���޸Ŀ���!");
                return;
            }
            if (shRet == SystemData.ReturnValue.FAILED)
            {
                MessageBoxEx.Show("�������ԭʼ�����ȷ!");
                this.txtOldPwd.Focus();
                this.txtOldPwd.SelectAll();
                return;
            }
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.Show("��¼�����޸�ʧ��,�����ڲ�����!");
            }
            else
            {
                MessageBoxEx.Show("��¼�����޸ĳɹ�!", MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //
            Rectangle rect = new Rectangle(0, 0, this.Width, 72);
            Color beginColor = Color.RoyalBlue;
            Color endColor = Color.CornflowerBlue;
            LinearGradientBrush brush = new LinearGradientBrush(rect, beginColor, endColor, LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(brush, rect);
            brush.Dispose();
            //
            Image img =Heren.MedQC.Maintenance.Properties.Resources.LoginBgImage;
            e.Graphics.DrawImage(img, 24, 18);
            //
            Font font = new Font("����", 9, FontStyle.Bold);
            SolidBrush solidBrush = new SolidBrush(Color.White);
            e.Graphics.DrawString("����������ԭʼ������¿���...", font, solidBrush, 68, 32);
            solidBrush.Dispose();
            font.Dispose();
        }
    }
}