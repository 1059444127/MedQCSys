// ***********************************************************
// �����ʿ�ϵͳ�ʿ����������,���������������ʾ\�޸�\ɾ��
// �Լ�����������.
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
using Heren.Common.Libraries;
using System.Threading;
using System.IO;
using Heren.MedQC.QuestionChat.Utilities;
using System.Drawing.Imaging;
using EMRDBLib.Entity;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace Heren.MedQC.QuestionChat.Forms
{
    public partial class QCQuestionChatForm : Form
    {
        private string m_szPatientID = string.Empty;
        public string PatientID
        {
            get { return this.m_szPatientID; }
            set { this.m_szPatientID = value; }
        }
        private string m_szVisitID = string.Empty;
        public string VisitID
        {
            get { return this.m_szVisitID; }
            set { this.m_szVisitID = value; }
        }
        private string m_szSender = string.Empty;
        public string Sender
        {
            get { return this.m_szSender; }
            set { this.m_szSender = value; }
        }
        private string m_szListener = string.Empty;
        public string Listener
        {
            get { return this.m_szListener; }
            set { this.m_szListener = value; }
        }
        public string UserType
        {
            get { return SystemParam.Instance.QChatArgs.UserType; }
            set { SystemParam.Instance.QChatArgs.UserType = value; }
        }

        private string m_MsgID = string.Empty;
        /// <summary>
        /// �ʼ���ϢID
        /// </summary>
        public string MsgID
        {
            get { return m_MsgID; }
            set { m_MsgID = value; }
        }

        public QCQuestionChatForm()
        {
            InitializeComponent();

            if (SystemParam.Instance.QChatArgs == null)
                SystemParam.Instance.QChatArgs = new QChatArgs();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetQCMsgChatLog();
            SetQCMsg();
            PatVisitInfo patVisitLog = null;
           
            short shRet =InpVisitAccess.Instance.GetInpPatVisitInfo(this.m_szPatientID, this.m_szVisitID, ref patVisitLog);
            if (patVisitLog == null)
                return;
            //�����ʿ���Ա����
            this.Text = string.Format("�ʿ����⹵ͨƽ̨-��ǰ�û���{0}���Է���{1}�����ˣ�{2},�����ţ�{3},����ţ�{4}"
           , this.m_szSender
           , SystemParam.Instance.QChatArgs.UserType == "0" ? "�ʿؿ�" : this.m_szListener
           , patVisitLog.PatientName
           , this.m_szPatientID
           , this.m_szVisitID);
            this.txtPatName.Text = patVisitLog.PatientName;
            this.txtPatientID.Text = this.m_szPatientID;
        }

        private static EMRDBLib.DbAccess.MedQCAccess m_MedQCAccess = new EMRDBLib.DbAccess.MedQCAccess();
        private static List<EMRDBLib.QcMsgDict> m_lstQCMessageTemplets;
        /// <summary>
        /// �����ʼ��ֵ��
        /// </summary>
        private List<EMRDBLib.QcMsgDict> ListQCMessageTemplets
        {
            get
            {
                if (m_lstQCMessageTemplets == null)
                {
                    QcMsgDictAccess.Instance.GetQcMsgDictList(ref m_lstQCMessageTemplets);
                }
                return m_lstQCMessageTemplets;
            }
        }
        /// <summary>
        ///����˲��˵�MSGID���������������Ϣ
        /// </summary>
        private System.Collections.Hashtable m_htMSGIDs = new System.Collections.Hashtable();
        /// <summary>
        /// �󶨹���MSG_ID���ʼ�����
        /// </summary>
        private void SetQCMsg()
        {
            EMRDBLib.MedicalQcMsg qCQuestionInfo = null;
            short shRet =MedicalQcMsgAccess.Instance.GetMedicalQcMsg(this.MsgID, ref qCQuestionInfo);
            if (shRet != SystemData.ReturnValue.OK || qCQuestionInfo == null)
                return;

            this.txtDocTitle.Text = qCQuestionInfo.TOPIC;
            this.txtQuestionType.Text = qCQuestionInfo.QA_EVENT_TYPE;
            EMRDBLib.QcMsgDict qcMessageTemplet = ListQCMessageTemplets.Find(
                delegate(EMRDBLib.QcMsgDict q)
                {
                    return q.QC_MSG_CODE == qCQuestionInfo.QC_MSG_CODE;
                }
                );
            this.txtMesssageTitle.Text = qcMessageTemplet != null ? qcMessageTemplet.MESSAGE : "";
            this.txtContent.Text = qCQuestionInfo.MESSAGE;
            this.txtBoxScore.Text = qCQuestionInfo.POINT.ToString();
            this.txtMessage.Focus();
        }
        /// <summary>
        /// ���ù�ͨ��Ϣ��¼
        /// </summary>
        private void SetQCMsgChatLog()
        {
            List<QcMsgChatLog> lstQcMsgChatLog = null;
            short shRet = QcMsgChatAccess.Instance.GetQCMsgChatLogList(this.m_szPatientID, this.m_szVisitID, ref lstQcMsgChatLog);
            if (shRet != SystemData.ReturnValue.OK)
                return;
            //��ղ���MSGID
            m_htMSGIDs.Clear();
            foreach (QcMsgChatLog qcMsgChatLog in lstQcMsgChatLog)
            {
                if (!m_htMSGIDs.ContainsKey(qcMsgChatLog.MsgID))
                    m_htMSGIDs.Add(qcMsgChatLog.MsgID, qcMsgChatLog.MsgID);
                //������Ǻ�MSGID��ص���Ϣ������ʾ�����촰��
                if (this.MsgID != qcMsgChatLog.MsgID)
                    continue;

                SetChatContentToForm(qcMsgChatLog);
                //����Ϣ����Ϊ�Ѷ�//������Ӧ��Ϊ��ǰ������Ϣ����
                if (!qcMsgChatLog.IsRead && qcMsgChatLog.Listener == this.m_szSender)
                {
                    qcMsgChatLog.IsRead = true;
                    shRet = QcMsgChatAccess.Instance.UpdateQCMsgChatLog(qcMsgChatLog);
                }
            }
            this.richTextBox1.Focus();
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            this.timer1.Enabled = true; //����������ʱ��
        }

        private void SetChatContentToForm(QcMsgChatLog qcMsgChatLog)
        {
            short shRet = SystemData.ReturnValue.OK;

            richTextBox1.SelectionFont = new Font(Font, FontStyle.Bold);
            bool bIsSend = false;
            if (SystemParam.Instance.QChatArgs == null)
                SystemParam.Instance.QChatArgs = new QChatArgs();
            if (qcMsgChatLog.Sender == this.m_szListener)
            {
                //�����ʿؿ���Ա����
                if (SystemParam.Instance.QChatArgs.UserType == "0")
                    bIsSend = true;
            }
            else
            {
                richTextBox1.SelectionColor = Color.Blue;
            }
            this.richTextBox1.AppendText(string.Format("  {0}|{1} {2}    "
                , bIsSend ? "�ʿؿ�" : qcMsgChatLog.Sender
                        , qcMsgChatLog.ChatSendDate
                        , System.Environment.NewLine));
            richTextBox1.SelectionFont = new Font(Font, FontStyle.Regular);
            richTextBox1.SelectionColor = Color.Black;
            if (qcMsgChatLog.MsgChatDataType == SystemData.MsgChatDataType.Image)
            {
                byte[] byteImage = null;
                //����ͼƬ
                shRet = QcMsgChatAccess.Instance.GetQCMsgChatInfoImage(qcMsgChatLog.ChatID, ref byteImage);
                if (byteImage != null && byteImage.Length > 0)
                {
                    Clipboard.Clear();
                    Bitmap image = ImageAccess.Instance.BufferToImage(byteImage);
                    if (image != null)
                    {
                        this.richTextBox1.ReadOnly = false;
                        Clipboard.SetImage(image);//��ͼƬ��ӵ�������
                        this.richTextBox1.Paste();
                        Clipboard.Clear();
                        this.richTextBox1.ReadOnly = true;
                    }
                }
            }
            else
            {
                this.richTextBox1.AppendText(string.Format("{0}"
                    , qcMsgChatLog.ChatContent));
            }
            this.richTextBox1.AppendText(string.Format("{0}{1}"
                           , System.Environment.NewLine
                           , System.Environment.NewLine));

        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtMessage.Text.Trim()))
                return;
            QcMsgChatLog qcMsgChatLog = new QcMsgChatLog();
            qcMsgChatLog.ChatID = qcMsgChatLog.MakeChatID();
            qcMsgChatLog.ChatContent = this.txtMessage.Text.Trim();
            qcMsgChatLog.ChatSendDate = DateTime.Now;
            qcMsgChatLog.Sender = this.m_szSender;
            qcMsgChatLog.PatientID = this.m_szPatientID;
            qcMsgChatLog.VisitID = this.m_szVisitID;
            qcMsgChatLog.Listener = this.m_szListener;
            qcMsgChatLog.MsgID = this.MsgID;
            byte[] byteChatImage = null;
            short shRet = QcMsgChatAccess.Instance.SaveQCMsgChatLog(qcMsgChatLog, byteChatImage);
            if (shRet != SystemData.ReturnValue.OK)
            {
                MessageBoxEx.ShowError("��Ϣ����ʧ��");
                return;
            }

            SetChatContentToForm(qcMsgChatLog);

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            this.txtMessage.Focus();
            this.txtMessage.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //���������ǰ�û���δ����Ϣ
            List<QcMsgChatLog> lstQcMsgChatLog = null;
            //������Ӧ��Ϊ��ǰ������Ϣ����
            short shRet = QcMsgChatAccess.Instance.GetQCMsgChatLogList(this.m_szPatientID, this.m_szVisitID, this.m_szSender, ref lstQcMsgChatLog);
            if (shRet != SystemData.ReturnValue.OK)
                return;
            this.richTextBox1.Focus();

            foreach (QcMsgChatLog qcMsgChatLog in lstQcMsgChatLog)
            {
                //������Ǻ�MSGID��ص���Ϣ������ʾ�����촰��
                if (this.MsgID != qcMsgChatLog.MsgID)
                    continue;
                SetChatContentToForm(qcMsgChatLog);
                //����Ϣ����Ϊ�Ѷ�
                if (!qcMsgChatLog.IsRead && qcMsgChatLog.Listener == this.m_szSender)
                {
                    qcMsgChatLog.IsRead = true;
                    shRet = QcMsgChatAccess.Instance.UpdateQCMsgChatLog(qcMsgChatLog);
                }
            }
            this.richTextBox1.Focus();
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void QCQuestionChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SystemParam.Instance.QChatArgs == null)
            {
                return;
            }
            this.Dispose();
            if (!string.IsNullOrEmpty(SystemParam.Instance.QChatArgs.Listener)
                && SystemParam.Instance.QChatArgs.ArgType != "1")
            {
                //EMRS���ùر���Ϣ���Ҫ������ص�
                this.ExitMedTask();
            }

        }

        public bool ExitMedTask()
        {
            IntPtr hMainFormHandle = GlobalMethods.Win32.GetSystemHandle(SystemData.MappingName.QUESTION_CHAT_SYS);
            if (!NativeMethods.User32.IsWindow(hMainFormHandle))
                return true;
            NativeMethods.User32.SetForegroundWindow(hMainFormHandle);
            NativeMethods.User32.SetActiveWindow(hMainFormHandle);
            NativeMethods.User32.SendMessage(hMainFormHandle, 0x10, IntPtr.Zero, IntPtr.Zero);
            return !NativeMethods.User32.IsWindow(hMainFormHandle);
        }

        private void btnScreenshot_Click(object sender, EventArgs e)
        {
            this.Screenshot();
        }

        private void Screenshot()
        {
            ScreenSnapForm form = new ScreenSnapForm(this);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.image == null)
                    return;
                QcMsgChatLog qcMsgChatLog = new QcMsgChatLog();
                qcMsgChatLog.ChatID = qcMsgChatLog.MakeChatID();
                qcMsgChatLog.ChatContent = this.txtMessage.Text.Trim();
                qcMsgChatLog.ChatSendDate = DateTime.Now;
                qcMsgChatLog.Sender = this.m_szSender;
                qcMsgChatLog.PatientID = this.m_szPatientID;
                qcMsgChatLog.VisitID = this.m_szVisitID;
                qcMsgChatLog.Listener =  this.m_szListener;
                qcMsgChatLog.MsgID = this.MsgID;
                qcMsgChatLog.MsgChatDataType = SystemData.MsgChatDataType.Image;

                byte[] byteChatImage = ImageAccess.Instance.ImageToBuffer(form.image, ImageFormat.Bmp);
                short shRet = QcMsgChatAccess.Instance.SaveQCMsgChatLog(qcMsgChatLog, byteChatImage);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.ShowError("��ͼ����ʧ��");
                    return;
                }
                SetChatContentToForm(qcMsgChatLog);
                this.richTextBox1.Focus();
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }

        }

        private void btnSelectImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "ͼƬ�ļ�(*.PNG)|*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                Bitmap image = Bitmap.FromFile(dialog.FileName) as Bitmap;
                QcMsgChatLog qcMsgChatLog = new QcMsgChatLog();
                qcMsgChatLog.ChatID = qcMsgChatLog.MakeChatID();
                qcMsgChatLog.ChatContent = this.txtMessage.Text.Trim();
                qcMsgChatLog.ChatSendDate = DateTime.Now;
                qcMsgChatLog.Sender = this.m_szListener;
                qcMsgChatLog.PatientID = this.m_szPatientID;
                qcMsgChatLog.VisitID = this.m_szVisitID;
                qcMsgChatLog.Listener = this.m_szSender;
                qcMsgChatLog.MsgChatDataType = SystemData.MsgChatDataType.Image;

                byte[] byteChatImage = ImageAccess.Instance.ImageToBuffer(image, ImageFormat.Bmp);
                short shRet = QcMsgChatAccess.Instance.SaveQCMsgChatLog(qcMsgChatLog, byteChatImage);
                if (shRet != SystemData.ReturnValue.OK)
                {
                    MessageBoxEx.ShowError("��ͼ����ʧ��");
                    return;
                }
                SetChatContentToForm(qcMsgChatLog);
                this.richTextBox1.Focus();
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}