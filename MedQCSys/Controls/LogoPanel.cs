using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using MedQCSys.Dialogs;
using EMRDBLib;

namespace MedQCSys.Controls
{
    public class LogoPanel : ToolStripPanel
    {
        private Image m_Image = null;
        /// <summary>
        /// ��ȡ�����ñ���Logo
        /// </summary>
        [Description("��ȡ�����ñ���Logo")]
        [DefaultValue(null)]
        public Image LogoImage
        {
            get { return this.m_Image; }
            set { this.m_Image = value; }
        }

        private Color m_GradientBeginColor = ProfessionalColors.ToolStripGradientEnd;
        /// <summary>
        /// ��ȡ�����ñ���������ʼɫ
        /// </summary>
        [DefaultValue(typeof(ProfessionalColors), "ToolStripGradientEnd")]
        [Description("��ȡ�����ñ���������ʼɫ")]
        public Color GradientBeginColor
        {
            get { return this.m_GradientBeginColor; }
            set { this.m_GradientBeginColor = value; }
        }

        private Color m_GradientEndColor = ProfessionalColors.ToolStripGradientBegin;
        /// <summary>
        /// ��ȡ�����ñ����������ɫ
        /// </summary>
        [DefaultValue(typeof(ProfessionalColors), "ToolStripGradientBegin")]
        [Description("��ȡ�����ñ����������ɫ")]
        public Color GradientEndColor
        {
            get { return this.m_GradientEndColor; }
            set { this.m_GradientEndColor = value; }
        }

        private LinearGradientMode m_eLinearGradientMode = LinearGradientMode.Horizontal;
        /// <summary>
        /// ��ȡ�����ñ�������ģʽ
        /// </summary>
        [Description("��ȡ�����ñ�������ģʽ")]
        [DefaultValue(LinearGradientMode.Horizontal)]
        public LinearGradientMode LinearGradientMode
        {
            get { return this.m_eLinearGradientMode; }
            set { this.m_eLinearGradientMode = value; }
        }

        public LogoPanel()
        {
            this.RowMargin = new Padding(0, 0, 0, 4);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
            int index = 0;
            while (index < this.Controls.Count)
            {
                Control ctrl = this.Controls[index++];
                if (ctrl != null && !ctrl.IsDisposed)
                    ctrl.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.PaintBackground(e.Graphics);
            //LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, this.m_GradientBeginColor
            //    , this.m_GradientEndColor, this.m_eLinearGradientMode);
            //e.Graphics.FillRectangle(brush, this.ClientRectangle);
            //brush.Dispose();

            if (this.m_Image != null)
            {
                int nLeft = this.ClientSize.Width - this.m_Image.Width - 8;
                e.Graphics.DrawImage(this.m_Image, nLeft, this.ClientSize.Height - this.m_Image.Height);
            }
            if (SystemParam.Instance.LocalConfigOption == null)
                return;
            if (string.IsNullOrEmpty(SystemParam.Instance.LocalConfigOption.HOSPITAL_NAME))
                return;
            SolidBrush brush = new SolidBrush(Color.DarkBlue);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            RectangleF rect = new RectangleF(0, 6, this.Width - 8, 24);
            e.Graphics.DrawString(SystemParam.Instance.LocalConfigOption.HOSPITAL_NAME, this.Font, brush, rect, format);
            brush.Dispose();
            format.Dispose();

        }

        private void PaintBackground(Graphics graphics)
        {
            Rectangle clipRect = this.ClientRectangle;
            Color beginColor = Color.FromArgb(123, 164, 224); 
            Color endColor = Color.FromArgb(226, 238, 255);
            LinearGradientMode gradientMode = LinearGradientMode.Vertical;
            LinearGradientBrush brush = new LinearGradientBrush(clipRect, beginColor, endColor, gradientMode);
            graphics.FillRectangle(brush, clipRect);
            brush.Dispose();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (this.m_Image == null)
                return;
            Rectangle rectLogo = Rectangle.Empty;
            rectLogo.Size = this.m_Image.Size;
            rectLogo.Location = new Point(this.ClientSize.Width - rectLogo.Width - 8, this.ClientSize.Height - rectLogo.Height);
            if (rectLogo.Contains(e.Location))
                (new SystemAboutForm()).ShowDialog();
        }
    }
}