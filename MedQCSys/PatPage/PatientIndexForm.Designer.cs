namespace MedQCSys.DockForms
{
    partial class PatientIndexForm
    {
        /// <summary>
        /// ����������������
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// ������������ʹ�õ���Դ��
        /// </summary>
        /// <param name="disposing">���Ӧ�ͷ��й���Դ��Ϊ true������Ϊ false��</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows ������������ɵĴ���

        /// <summary>
        /// �����֧������ķ��� - ��Ҫ
        /// ʹ�ô���༭���޸Ĵ˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.reportDesigner1 = new Heren.Common.Report.ReportDesigner();
            this.reportDesigner2 = new Heren.Common.Report.ReportDesigner();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(793, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(48, 22);
            this.toolStripButton1.Text = "��һҳ";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(48, 22);
            this.toolStripButton2.Text = "��һҳ";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // reportDesigner1
            // 
            this.reportDesigner1.AutoScrollMinSize = new System.Drawing.Size(818, 1147);
            this.reportDesigner1.CanvasCenter = true;
            this.reportDesigner1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportDesigner1.Location = new System.Drawing.Point(0, 25);
            this.reportDesigner1.Name = "reportDesigner1";
            this.reportDesigner1.Readonly = true;
            this.reportDesigner1.Size = new System.Drawing.Size(793, 450);
            this.reportDesigner1.TabIndex = 1;
            this.reportDesigner1.Text = "reportDesigner1";
            this.reportDesigner1.QueryContext += new Heren.Common.Report.QueryContextEventHandler(this.reportDesigner1_QueryContext);
            this.reportDesigner1.ExecuteQuery += new Heren.Common.Report.ExecuteQueryEventHandler(this.reportDesigner1_ExecuteQuery);
            // 
            // reportDesigner2
            // 
            this.reportDesigner2.AutoScrollMinSize = new System.Drawing.Size(818, 1147);
            this.reportDesigner2.CanvasCenter = true;
            this.reportDesigner2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportDesigner2.Location = new System.Drawing.Point(0, 25);
            this.reportDesigner2.Name = "reportDesigner2";
            this.reportDesigner2.Readonly = true;
            this.reportDesigner2.Size = new System.Drawing.Size(793, 450);
            this.reportDesigner2.TabIndex = 2;
            this.reportDesigner2.Text = "reportDesigner2";
            this.reportDesigner2.QueryContext += new Heren.Common.Report.QueryContextEventHandler(this.reportDesigner1_QueryContext);
            this.reportDesigner2.ExecuteQuery += new Heren.Common.Report.ExecuteQueryEventHandler(this.reportDesigner1_ExecuteQuery);
            // 
            // PatProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 475);
            this.Controls.Add(this.reportDesigner2);
            this.Controls.Add(this.reportDesigner1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PatProfileForm";
            this.Text = "������ҳ";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private Heren.Common.Report.ReportDesigner reportDesigner1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private Heren.Common.Report.ReportDesigner reportDesigner2;
    }
}
