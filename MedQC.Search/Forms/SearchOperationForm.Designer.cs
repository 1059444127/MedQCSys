namespace Heren.MedQC.Search
{
    partial class SearchOperationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtPatientName = new Heren.Common.Forms.XTextBox();
            this.txtPatientID = new Heren.Common.Forms.XTextBox();
            this.cboDeptName = new Heren.Common.Controls.DictInput.FindComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dtpStatTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpStatTimeBegin = new System.Windows.Forms.DateTimePicker();
            this.colDeptName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPatientID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisitID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOperationNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOpeartionDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOperationDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHeal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAnaesthesiaMethod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOperator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWoundGrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_OPERATION_SCALE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("����", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDeptName,
            this.colPatientName,
            this.colPatientID,
            this.colVisitID,
            this.colSex,
            this.colOperationNo,
            this.colOpeartionDesc,
            this.colOperationDate,
            this.colHeal,
            this.colAnaesthesiaMethod,
            this.colOperator,
            this.colWoundGrade,
            this.col_OPERATION_SCALE});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 53);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("����", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(935, 420);
            this.dataGridView1.TabIndex = 37;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtPatientName);
            this.panel1.Controls.Add(this.txtPatientID);
            this.panel1.Controls.Add(this.cboDeptName);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnExportExcel);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnQuery);
            this.panel1.Controls.Add(this.dtpStatTimeEnd);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dtpStatTimeBegin);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(935, 53);
            this.panel1.TabIndex = 38;
            // 
            // txtPatientName
            // 
            this.txtPatientName.Font = new System.Drawing.Font("����", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPatientName.Location = new System.Drawing.Point(482, 5);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Size = new System.Drawing.Size(100, 23);
            this.txtPatientName.TabIndex = 37;
            // 
            // txtPatientID
            // 
            this.txtPatientID.Font = new System.Drawing.Font("����", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPatientID.Location = new System.Drawing.Point(293, 3);
            this.txtPatientID.Name = "txtPatientID";
            this.txtPatientID.Size = new System.Drawing.Size(100, 23);
            this.txtPatientID.TabIndex = 37;
            // 
            // cboDeptName
            // 
            this.cboDeptName.CandidateWidth = 200;
            this.cboDeptName.Font = new System.Drawing.Font("����", 10.5F);
            this.cboDeptName.Location = new System.Drawing.Point(49, 3);
            this.cboDeptName.Name = "cboDeptName";
            this.cboDeptName.Size = new System.Drawing.Size(157, 23);
            this.cboDeptName.TabIndex = 27;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("����", 10.5F);
            this.label5.Location = new System.Drawing.Point(408, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 14);
            this.label5.TabIndex = 28;
            this.label5.Text = "��������";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportExcel.Location = new System.Drawing.Point(854, 22);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(78, 28);
            this.btnExportExcel.TabIndex = 36;
            this.btnExportExcel.Text = "Excel����";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("����", 10.5F);
            this.label4.Location = new System.Drawing.Point(220, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 14);
            this.label4.TabIndex = 28;
            this.label4.Text = "����ID��";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("����", 10.5F);
            this.label3.Location = new System.Drawing.Point(11, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 28;
            this.label3.Text = "����";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Font = new System.Drawing.Font("����", 10.5F);
            this.btnQuery.Location = new System.Drawing.Point(781, 22);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(65, 28);
            this.btnQuery.TabIndex = 32;
            this.btnQuery.Text = "��ѯ(&Q)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // dtpStatTimeEnd
            // 
            this.dtpStatTimeEnd.Font = new System.Drawing.Font("����", 10.5F);
            this.dtpStatTimeEnd.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.dtpStatTimeEnd.Location = new System.Drawing.Point(232, 28);
            this.dtpStatTimeEnd.Name = "dtpStatTimeEnd";
            this.dtpStatTimeEnd.Size = new System.Drawing.Size(120, 23);
            this.dtpStatTimeEnd.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("����", 10.5F);
            this.label2.Location = new System.Drawing.Point(208, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 14);
            this.label2.TabIndex = 35;
            this.label2.Text = "��";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("����", 10.5F);
            this.label1.Location = new System.Drawing.Point(10, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 31;
            this.label1.Text = "��������";
            // 
            // dtpStatTimeBegin
            // 
            this.dtpStatTimeBegin.Font = new System.Drawing.Font("����", 10.5F);
            this.dtpStatTimeBegin.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.dtpStatTimeBegin.Location = new System.Drawing.Point(85, 28);
            this.dtpStatTimeBegin.Name = "dtpStatTimeBegin";
            this.dtpStatTimeBegin.Size = new System.Drawing.Size(121, 23);
            this.dtpStatTimeBegin.TabIndex = 29;
            // 
            // colDeptName
            // 
            this.colDeptName.FillWeight = 140F;
            this.colDeptName.HeaderText = "���ڿ���";
            this.colDeptName.Name = "colDeptName";
            this.colDeptName.ReadOnly = true;
            this.colDeptName.Width = 140;
            // 
            // colPatientName
            // 
            this.colPatientName.HeaderText = "��������";
            this.colPatientName.Name = "colPatientName";
            this.colPatientName.ReadOnly = true;
            // 
            // colPatientID
            // 
            this.colPatientID.HeaderText = "����ID��";
            this.colPatientID.Name = "colPatientID";
            this.colPatientID.ReadOnly = true;
            // 
            // colVisitID
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colVisitID.DefaultCellStyle = dataGridViewCellStyle2;
            this.colVisitID.HeaderText = "��Ժ��";
            this.colVisitID.Name = "colVisitID";
            this.colVisitID.ReadOnly = true;
            this.colVisitID.Width = 80;
            // 
            // colSex
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSex.DefaultCellStyle = dataGridViewCellStyle3;
            this.colSex.FillWeight = 70F;
            this.colSex.HeaderText = "�Ա�";
            this.colSex.Name = "colSex";
            this.colSex.ReadOnly = true;
            this.colSex.Width = 70;
            // 
            // colOperationNo
            // 
            this.colOperationNo.HeaderText = "�������";
            this.colOperationNo.Name = "colOperationNo";
            this.colOperationNo.ReadOnly = true;
            // 
            // colOpeartionDesc
            // 
            this.colOpeartionDesc.HeaderText = "��������";
            this.colOpeartionDesc.Name = "colOpeartionDesc";
            this.colOpeartionDesc.ReadOnly = true;
            // 
            // colOperationDate
            // 
            this.colOperationDate.HeaderText = "��������";
            this.colOperationDate.Name = "colOperationDate";
            this.colOperationDate.ReadOnly = true;
            // 
            // colHeal
            // 
            this.colHeal.HeaderText = "�п��������";
            this.colHeal.Name = "colHeal";
            this.colHeal.ReadOnly = true;
            this.colHeal.Width = 120;
            // 
            // colAnaesthesiaMethod
            // 
            this.colAnaesthesiaMethod.HeaderText = "������";
            this.colAnaesthesiaMethod.Name = "colAnaesthesiaMethod";
            this.colAnaesthesiaMethod.ReadOnly = true;
            // 
            // colOperator
            // 
            this.colOperator.HeaderText = "����ҽʦ";
            this.colOperator.Name = "colOperator";
            this.colOperator.ReadOnly = true;
            // 
            // colWoundGrade
            // 
            this.colWoundGrade.HeaderText = "�пڵȼ�";
            this.colWoundGrade.Name = "colWoundGrade";
            this.colWoundGrade.ReadOnly = true;
            // 
            // col_OPERATION_SCALE
            // 
            this.col_OPERATION_SCALE.HeaderText = "��������";
            this.col_OPERATION_SCALE.Name = "col_OPERATION_SCALE";
            this.col_OPERATION_SCALE.ReadOnly = true;
            this.col_OPERATION_SCALE.Width = 90;
            // 
            // SearchOperationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 473);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("����", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "SearchOperationForm";
            this.Text = "�������߲�ѯ";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnExportExcel;
        private Heren.Common.Controls.DictInput.FindComboBox cboDeptName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpStatTimeEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpStatTimeBegin;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private Heren.Common.Forms.XTextBox txtPatientName;
        private Heren.Common.Forms.XTextBox txtPatientID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeptName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPatientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPatientID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVisitID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOperationNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOpeartionDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOperationDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAnaesthesiaMethod;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOperator;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWoundGrade;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_OPERATION_SCALE;
    }
}