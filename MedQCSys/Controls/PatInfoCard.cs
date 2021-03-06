// ***********************************************************
// 病案质控系统用于患者列表窗口中显示患者信息的控件.
// Creator:YangMingkun  Date:2009-11-3
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Heren.Common.Libraries;
using EMRDBLib;

namespace MedQCSys.Controls.PatInfoList
{
    internal class PatInfoCard : Control
    {
        private const int CONTENT_LINE_SPACE = 6;
        private EMRDBLib.PatVisitInfo m_patVisitLog = null;

        /// <summary>
        /// 获取或设置患者就诊日志信息
        /// </summary>
        [Browsable(false)]
        public EMRDBLib.PatVisitInfo PatVisitLog
        {
            get { return this.m_patVisitLog; }
            set
            {
                this.m_patVisitLog = value;
                this.Invalidate();
            }
        }

        private bool m_bSelected = false;
        /// <summary>
        /// 获取或设置列表项是否处于选中状态
        /// </summary>
        [Browsable(false)]
        public bool Selected
        {
            get { return this.m_bSelected; }
            set
            {
                if (this.m_bSelected == value)
                    return;
                this.m_bSelected = value;
                if (this.m_bSelected)
                    this.Height = this.m_nCaptionHeight + (this.m_ContentFont.Height + CONTENT_LINE_SPACE) * 13;
                else
                    this.Height = this.m_nCaptionHeight;
                base.Invalidate();
            }
        }

        private int m_nCaptionHeight = 28;
        /// <summary>
        /// 获取或设置显示标题高度
        /// </summary>
        [Description("获取或设置显示标题高度")]
        public int CaptionHeight
        {
            get { return this.m_nCaptionHeight; }
            set
            {
                this.m_nCaptionHeight = value;
                base.Invalidate();
            }
        }
        /// <summary>
        /// 获取或设置列表项背景色
        /// </summary>
        [Description("获取或设置列表项背景色")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                base.Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置列表项的前景色
        /// </summary>
        [Description("获取或设置列表项的前景色")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                base.Invalidate();
            }
        }

        private Color m_BorderColor = SystemColors.Highlight;
        /// <summary>
        /// 获取或设置TabCapion边界色
        /// </summary>
        [Description("获取或设置TabCapion边界色")]
        public Color BorderColor
        {
            get { return this.m_BorderColor; }
            set
            {
                this.m_BorderColor = value;
                base.Invalidate();
            }
        }

        private Color m_GradientBackColor = Color.Lavender;
        /// <summary>
        /// 获取或设置TabCapion梯度渐变背景色
        /// </summary>
        [Description("获取或设置TabCapion梯度渐变背景色")]
        public Color GradientBackColor
        {
            get { return this.m_GradientBackColor; }
            set
            {
                this.m_GradientBackColor = value;
                base.Invalidate();
            }
        }

        private Color m_MouseOverBackColor = Color.White;
        /// <summary>
        /// 获取或设置光标经过时列表项的背景色
        /// </summary>
        [Description("获取或设置光标经过时列表项的背景色")]
        public Color MouseOverBackColor
        {
            get { return this.m_MouseOverBackColor; }
            set
            {
                this.m_MouseOverBackColor = value;
                base.Invalidate();
            }
        }

        private Color m_ActiveBackColor = Color.White;
        /// <summary>
        /// 获取或设置列表项活动状态下的背景色
        /// </summary>
        [Description("获取或设置列表项活动状态下的背景色")]
        public Color ActiveBackColor
        {
            get { return this.m_ActiveBackColor; }
            set
            {
                this.m_ActiveBackColor = value;
                base.Invalidate();
            }
        }

        private Color m_MouseOverForeColor = Color.Blue;
        /// <summary>
        /// 获取或设置光标经过时列表项的前景色
        /// </summary>
        [Description("获取或设置光标经过时列表项的前景色")]
        public Color MouseOverForeColor
        {
            get { return this.m_MouseOverForeColor; }
            set
            {
                this.m_MouseOverForeColor = value;
                base.Invalidate();
            }
        }

        private Color m_ActiveForeColor = Color.Blue;
        /// <summary>
        /// 获取或设置列表项活动状态下的前景色
        /// </summary>
        [Description("获取或设置列表项活动状态下的前景色")]
        public Color ActiveForeColor
        {
            get { return this.m_ActiveForeColor; }
            set
            {
                this.m_ActiveForeColor = value;
                base.Invalidate();
            }
        }

        private Font m_ContentFont = new Font("宋体", 10f, FontStyle.Regular);
        /// <summary>
        /// 获取或设置就诊卡内容字体
        /// </summary>
        [Description("获取或设置就诊卡内容字体")]
        public Font ContentFont
        {
            get { return this.m_ContentFont; }
            set
            {
                this.m_ContentFont = value;
                this.Invalidate();
            }
        }

        private Font m_MouseOverFont = new Font("宋体", 10.5f, FontStyle.Bold);
        /// <summary>
        /// 获取或设置光标经过时列表项的字体
        /// </summary>
        [Description("获取或设置光标经过时列表项的字体")]
        public Font MouseOverFont
        {
            get { return this.m_MouseOverFont; }
            set
            {
                this.m_MouseOverFont = value;
                base.Invalidate();
            }
        }

        private Font m_ActiveFont = new Font("宋体", 10.5f, FontStyle.Bold);
        /// <summary>
        /// 获取或设置列表项活动状态下的字体
        /// </summary>
        [Description("获取或设置列表项活动状态下的字体")]
        public Font ActiveFont
        {
            get { return this.m_ActiveFont; }
            set
            {
                this.m_ActiveFont = value;
                base.Invalidate();
            }
        }

        public PatInfoCard()
        {
            this.Height = this.m_nCaptionHeight;
            this.Font = new Font("宋体", 10.5f, FontStyle.Regular);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint,
                              true);
            this.UpdateStyles();
        }

        /// <summary>
        /// 绘制控件边界
        /// </summary>
        /// <param name="g">绘图对象Graphics</param>
        private void DrawBackground(Graphics g)
        {
            Rectangle ctrlRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            if (ctrlRect.Width <= 0 || ctrlRect.Height <= 0)
                return;
            if (this.m_bSelected)
            {
                SolidBrush solidBrush = new SolidBrush(this.m_ActiveBackColor);
                g.FillRectangle(solidBrush, ctrlRect);
                solidBrush.Dispose();
            }
            else if (this.m_bMouseOver)
            {
                LinearGradientBrush brush = new LinearGradientBrush(ctrlRect, this.m_GradientBackColor, this.BackColor, LinearGradientMode.Vertical);
                g.FillRectangle(brush, ctrlRect);
                brush.Dispose();
            }
            else
            {
                LinearGradientBrush brush = new LinearGradientBrush(ctrlRect, this.m_GradientBackColor, this.BackColor, LinearGradientMode.Horizontal);
                g.FillRectangle(brush, ctrlRect);
                brush.Dispose();
            }

            if (this.m_bSelected || this.m_bMouseOver)
            {
                Pen pen = new Pen(this.m_BorderColor);
                pen.Color = Color.LightGray;
                g.DrawRectangle(pen, ctrlRect);
                pen.Dispose();
            }
        }

        /// <summary>
        /// 绘制控件显示文本
        /// </summary>
        /// <param name="g">绘图对象Graphics</param>
        private void DrawCaption(Graphics g)
        {
            if (this.m_patVisitLog == null)
                return;
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;

            StringFormat stateFormat = new StringFormat();
            stateFormat.Alignment = StringAlignment.Center;
            stateFormat.LineAlignment = StringAlignment.Center;
            stateFormat.FormatFlags = StringFormatFlags.NoWrap;
            Font stateFont = new Font("宋体", 9f, FontStyle.Regular);
            SolidBrush stateBrush = new SolidBrush(Color.White);

            Font font = null;
            Color foreColor = Color.Empty;

            if (this.m_bSelected)
            {
                font = this.m_ActiveFont;
                foreColor = this.m_ActiveForeColor;
            }
            else if (this.m_bMouseOver)
            {
                font = this.m_MouseOverFont;
                foreColor = this.m_MouseOverForeColor;
            }
            else
            {
                font = this.Font;
                foreColor = this.ForeColor;
            }
            //床号
            Rectangle stBedCodeRect = new Rectangle(4, 0, 35, this.m_nCaptionHeight);
            //患者姓名
            Rectangle stPatientNameRect = new Rectangle(stBedCodeRect.Right + 5, 0, 90, this.m_nCaptionHeight);
            //质检状态
            Rectangle stQualityRect = new Rectangle(stPatientNameRect.Right + 5, 0, 20, this.m_nCaptionHeight);
            //病案评分
            Rectangle stScoreRect = new Rectangle(stQualityRect.Right + 5, 0, 35, this.m_nCaptionHeight);
            //患者性别
            Rectangle stPatientSex = new Rectangle(stScoreRect.Right + 5, 0, 40, this.m_nCaptionHeight);
            stringFormat.Alignment = StringAlignment.Near;
            SolidBrush brush = new SolidBrush(foreColor);
            //绘制床号
            if (!string.IsNullOrEmpty(this.m_patVisitLog.BED_CODE) && !stBedCodeRect.IsEmpty)
                g.DrawString(this.m_patVisitLog.BED_CODE, font, brush, stBedCodeRect, stringFormat);
            //绘制患者姓名
            if (!string.IsNullOrEmpty(this.m_patVisitLog.PATIENT_NAME) && !stPatientNameRect.IsEmpty)
            {
                if (this.m_patVisitLog.PATIENT_CONDITION == EMRDBLib.SystemData.PatientCondition.SERIOUS)
                {
                    brush.Color = Color.Green;
                }
                else if (this.m_patVisitLog.PATIENT_CONDITION == EMRDBLib.SystemData.PatientCondition.CRITICAL)
                {
                    brush.Color = Color.Red;
                }
                string szPatientName = string.Format("{0}", this.m_patVisitLog.PATIENT_NAME);
                if (SystemParam.Instance.LocalConfigOption.IsShowVisitID)
                {
                    szPatientName += string.Format("[{0}]", this.m_patVisitLog.VISIT_ID);
                }
                g.DrawString(szPatientName, font, brush, stPatientNameRect, stringFormat);
            }

            //绘制患者男女性别
            if (!string.IsNullOrEmpty(this.m_patVisitLog.PATIENT_SEX) && !stPatientSex.IsEmpty)
                g.DrawString(this.m_patVisitLog.PATIENT_SEX, font, brush, stPatientSex, stringFormat);
            if (this.m_patVisitLog.TotalScore == "100")
            {
                this.m_patVisitLog.QCResultStatus = SystemData.MedQCStatus.PASS;
            }
            //绘制质控状态
            if (!string.IsNullOrEmpty(this.m_patVisitLog.QCResultStatus) && !stQualityRect.IsEmpty)
            {
                if (this.m_patVisitLog.QCResultStatus == EMRDBLib.SystemData.MedQCStatus.PASS)
                {
                    brush.Color = Color.Lime;
                    g.DrawString("*", font, brush, stQualityRect, stringFormat);
                }
                else if (this.m_patVisitLog.QCResultStatus == EMRDBLib.SystemData.MedQCStatus.EXIST_BUG)
                {
                    brush.Color = Color.Red;
                    g.DrawString("*", font, brush, stQualityRect, stringFormat);
                }
                else if (this.m_patVisitLog.QCResultStatus == EMRDBLib.SystemData.MedQCStatus.SERIOUS_BUG)
                {
                    brush.Color = Color.Red;
                    g.DrawString("**", font, brush, stQualityRect, stringFormat);
                }
            }
            //绘制病案评分
            if (
                !string.IsNullOrEmpty(this.m_patVisitLog.TotalScore) && !stScoreRect.IsEmpty)
            {
                if (this.m_patVisitLog.QCResultStatus == EMRDBLib.SystemData.MedQCStatus.PASS)
                {
                    //brush.Color = Color.Lime;
                    g.DrawString(this.m_patVisitLog.TotalScore, font, brush, stScoreRect, stringFormat);
                }
                else if (this.m_patVisitLog.QCResultStatus == EMRDBLib.SystemData.MedQCStatus.EXIST_BUG || this.m_patVisitLog.QCResultStatus == EMRDBLib.SystemData.MedQCStatus.SERIOUS_BUG)
                {
                    //brush.Color = Color.Red;
                    g.DrawString(this.m_patVisitLog.TotalScore, font, brush, stScoreRect, stringFormat);
                }
                else
                {
                    g.DrawString(this.m_patVisitLog.TotalScore, font, brush, stScoreRect, stringFormat);
                }
            }
            if (SystemParam.Instance.LocalConfigOption.IsOpenHGYY)
            {
                long nDayIn = 0;
                if (m_patVisitLog.DISCHARGE_TIME == m_patVisitLog.DefaultTime)
                {
                    nDayIn = GlobalMethods.SysTime.GetInpDays(this.m_patVisitLog.VISIT_TIME, DateTime.Now);
                }
                else
                {
                    nDayIn = GlobalMethods.SysTime.GetInpDays(this.m_patVisitLog.VISIT_TIME, m_patVisitLog.DISCHARGE_TIME);
                }
                //在院天数
                Rectangle stnDayIn = new Rectangle(stPatientSex.Right + 5, 5, 48, 16);
                g.DrawString(nDayIn.ToString()+"天", font, brush, stnDayIn, stringFormat);

                //病人手术
                GraphicsPath path = CreateRoundedRectanglePath(stnDayIn, 4);
                Rectangle stPatientOperation = new Rectangle(stnDayIn.Right + 3, 5, 16, 16);
                if (this.m_patVisitLog.IsOperation)
                {
                    //绘制病人手术
                    path = CreateRoundedRectanglePath(stPatientOperation, 4);
                    g.FillPath(new SolidBrush(Color.Orange), path);
                    g.DrawString("术", stateFont, stateBrush, stPatientOperation, stateFormat);
                }
            }
            if (SystemParam.Instance.LocalConfigOption.IsDrawingPatientIdentification)
            {
                //病人新入
                Rectangle stPatientNew = new Rectangle(stPatientSex.Right + 5, 5, 16, 16);
                //病人病情
                Rectangle stPatientCondition = new Rectangle(stPatientNew.Right + 3, 5, 16, 16);
                //病人手术
                Rectangle stPatientOperation = new Rectangle(stPatientCondition.Right + 3, 5, 16, 16);
                //病人输血
                Rectangle stPatientBlood = new Rectangle(stPatientOperation.Right + 3, 5, 16, 16);
                GraphicsPath path = CreateRoundedRectanglePath(stPatientNew, 4);
                if (this.m_patVisitLog.VISIT_TIME > (DateTime.Now.AddDays(-3)))
                {
                    //绘制病人新入
                    g.DrawPath(new Pen(new SolidBrush(Color.Green), 1), path);
                    g.DrawString("新", stateFont, new SolidBrush(Color.Green), stPatientNew, stateFormat);
                }
                
                if (!string.IsNullOrEmpty(this.m_patVisitLog.PATIENT_CONDITION) && "危急".IndexOf(this.m_patVisitLog.PATIENT_CONDITION) >= 0)
                {
                    //绘制病人病情
                    path = CreateRoundedRectanglePath(stPatientCondition, 4);
                    if (this.m_patVisitLog.PATIENT_CONDITION.IndexOf("危")>=0)
                        g.FillPath(new SolidBrush(Color.Red), path);
                    else
                        g.FillPath(new SolidBrush(Color.Green), path);
                    g.DrawString(this.m_patVisitLog.PATIENT_CONDITION, stateFont, stateBrush, stPatientCondition, stateFormat);
                }
                if (this.m_patVisitLog.IsOperation)
                {
                    //绘制病人手术
                    path = CreateRoundedRectanglePath(stPatientOperation, 4);
                    g.FillPath(new SolidBrush(Color.Orange), path);
                    g.DrawString("术", stateFont, stateBrush, stPatientOperation, stateFormat);
                }
                if (this.m_patVisitLog.IsBlood)
                {
                    //绘制病人输血
                    path = CreateRoundedRectanglePath(stPatientBlood, 4);
                    g.FillPath(new SolidBrush(Color.Red), path);
                    g.DrawString("血", stateFont, stateBrush, stPatientBlood, stateFormat);
                }
            }
            brush.Dispose();
            stringFormat.Dispose();
        }

        /// <summary>
        /// 绘制控件显示文本
        /// </summary>
        /// <param name="g">绘图对象Graphics</param>
        private void DrawContent(Graphics g)
        {
            Font font = new Font("宋体", 10.5f, FontStyle.Regular);
            SolidBrush brush = new SolidBrush(Color.Black);
            Rectangle rect = new Rectangle(10, this.m_nCaptionHeight, this.Width, this.FontHeight + CONTENT_LINE_SPACE);

            g.DrawString(string.Format("         {0}", this.m_patVisitLog.PATIENT_ID), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("患者ID号", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.INP_NO), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("病 案 号", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.VISIT_ID), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("入 院 次", font, brush, rect.Location);


            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.BIRTH_TIME == this.m_patVisitLog.DefaultTime ? "" : GlobalMethods.SysTime.GetAgeText(this.m_patVisitLog.BIRTH_TIME, this.m_patVisitLog.VISIT_TIME)), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("年    龄", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.CHARGE_TYPE), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("费    别", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.VISIT_TIME.ToString("yyyy年M月d日 HH:mm")), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("入院时间", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.DEPT_NAME), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("入院科室", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.DIAGNOSIS), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("诊    断", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.PATIENT_CONDITION), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("病    情", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.INCHARGE_DOCTOR), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("经治医生", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.AttendingDoctor), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("上级医生", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", this.m_patVisitLog.SUPER_DOCTOR), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("主任医生", font, brush, rect.Location);

            rect.Y += rect.Height;
            g.DrawString(string.Format("         {0}", SystemData.MrStatus.GetMrStatusName(this.m_patVisitLog.MR_STATUS)), font, brush, rect.Location);
            brush.Color = Color.Black;
            g.DrawString("病案状态", font, brush, rect.Location);

            brush.Dispose();
        }

        private bool m_bMouseOver = false;
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.m_bMouseOver = true;
            base.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.m_bMouseOver = false;
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.DrawBackground(e.Graphics);
            this.DrawCaption(e.Graphics);
            if (this.m_bSelected)
                this.DrawContent(e.Graphics);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Focus();
        }

        protected GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}
