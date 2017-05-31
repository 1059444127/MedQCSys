using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;

namespace MedQCSys.Dialogs
{
    public partial class InsertCommentForm : Form
    {
        private string m_szUserComment = string.Empty;
        /// <summary>
        /// ��ȡ�������û���ע
        /// </summary>
        public string UserComment
        {
            get { return this.m_szUserComment; }
            set { this.m_szUserComment = value; }
        }

        public InsertCommentForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtComment.Text.Trim()))
            {
                MessageBoxEx.Show("��������ע��", MessageBoxIcon.Warning);
                return;
            }
            this.m_szUserComment = this.txtComment.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }
    }
}