// ***********************************************************
// �����ʿ�ϵͳ�������Ի���.
// Creator:LiChunYing  Date:2011-09-13
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
using Heren.Common.Libraries;
using Heren.Common.Controls;
 
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace MedQCSys.Dialogs
{
    public partial class SelectQuestionForm : HerenForm
    {
        private EMRDBLib.QcMsgDict m_qcMessageTemplet = null;
        /// <summary>
        /// ��ȡ�������ʼ�����ģ����
        /// </summary>
        public EMRDBLib.QcMsgDict QCMessageTemplet
        {
            get { return this.m_qcMessageTemplet; }
            set { this.m_qcMessageTemplet = value; }
        }

        private static List<EMRDBLib.QaEventTypeDict> lstQCEventTypes = null;

        public static List<EMRDBLib.QaEventTypeDict> LstQCEventTypes
        {
            get
            {
                if (lstQCEventTypes == null)
                {
                    short shRet = QaEventTypeDictAccess.Instance.GetQCEventTypeList(ref lstQCEventTypes);
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                    }
                }
                return SelectQuestionForm.lstQCEventTypes;
            }
        }

        private static List<EMRDBLib.QcMsgDict> lstQCMessageTemplet = null;

        public static List<EMRDBLib.QcMsgDict> LstQCMessageTemplet
        {
            get
            {
                if (lstQCMessageTemplet == null)
                {
                    short shRet = QcMsgDictAccess.Instance.GetQcMsgDictList(ref lstQCMessageTemplet);
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                    }
                }
                return SelectQuestionForm.lstQCMessageTemplet;
            }
        }


        private string m_szDocTitle;
        /// <summary>
        ///��ȡ�����ò�������
        /// </summary>
        public string DocTitle
        {
            get { return this.m_szDocTitle; }
            set { this.m_szDocTitle = value; }
        }

        private DateTime m_dtQCCheckTime = DateTime.Now;
        /// <summary>
        /// ��ȡ�������ʼ�ʱ��
        /// </summary>
        public DateTime QCCheckTime
        {
            get { return this.m_dtQCCheckTime; }
            set { this.m_dtQCCheckTime = value; }
        }

        private string m_szQCAskDateTime = string.Empty;
        /// <summary>
        /// ��ȡ������ҽ��ȷ��ʱ��
        /// </summary>
        public string QCAskDateTime
        {
            get { return this.m_szQCAskDateTime; }
            set { this.m_szQCAskDateTime = value; }
        }

        private string m_szDoctorComment = string.Empty;
        /// <summary>
        /// ��ȡ������ҽ������
        /// </summary>
        public string DoctorComment
        {
            get { return this.m_szDoctorComment; }
            set { this.m_szDoctorComment = value; }
        }

        public SelectQuestionForm()
        {
            InitializeComponent();
            string caption = "��ʾ:";
            if (SystemParam.Instance.LocalConfigOption.VetoHigh > 0)
            {
                caption+= string.Format("\n{0}������������ó��Ҳ���"
                , SystemParam.Instance.LocalConfigOption.VetoHigh);
            }
            if (SystemParam.Instance.LocalConfigOption.VetoLow > 0)
            {
                caption += string.Format("\n{0}������������óɱ�����"
               , SystemParam.Instance.LocalConfigOption.VetoLow);
            }
            this.toolTip1.SetToolTip(this.picVetoDesc, caption);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.txtChecker.Text = SystemParam.Instance.UserInfo.ID;
            this.txtCheckDate.Text = this.QCCheckTime.ToString("yyyy-M-d HH:mm");
            this.txtQuestionType.Text = "<˫��ѡ��>";
            if (SystemParam.Instance.PatVisitInfo == null)
                return;
            //�½�ʱ���ݳ�Ժģʽ���ò�������
            if (String.IsNullOrEmpty(SystemParam.Instance.PatVisitInfo.DISCHARGE_MODE))
                this.rdbIn.Checked = true;
            else if (SystemParam.Instance.PatVisitInfo.DISCHARGE_MODE == "����")
                this.rdbDeath.Checked = true;
            else
                this.rdbOut.Checked = true;

            this.txtPatientID.Text = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            this.txtPatName.Text = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
            this.txtPatSex.Text = SystemParam.Instance.PatVisitInfo.PATIENT_SEX;
            this.txtDocTitle.Text = this.DocTitle;
            this.txtAskDateTime.Text = this.QCAskDateTime;
            this.txtDoctorComment.Text = this.DoctorComment;
            if (this.QCMessageTemplet != null)
            {
                this.txtQuestionType.Text = this.QCMessageTemplet.QA_EVENT_TYPE;
                this.txtMesssageTitle.Text = this.QCMessageTemplet.MESSAGE_TITLE;
                this.txtMessage.Text = this.QCMessageTemplet.MESSAGE;
                this.txtMessage.Focus();
                this.txtMessage.SelectAll();
                this.txtMessage.Tag = this.QCMessageTemplet.QC_MSG_CODE;
                //    this.txtChecker.Tag = this.QCMessageTemplet.Score;
                this.txtBoxScore.Text = this.QCMessageTemplet.SCORE.ToString();
                //�޸Ļ������
                //���ò�������
                if (this.QCMessageTemplet.QCDocType == SystemData.QCDocType.INHOSPITAL)
                {
                    this.rdbIn.Checked = true;
                }
                else if (this.QCMessageTemplet.QCDocType == SystemData.QCDocType.OUTHOSPITAL)
                {
                    this.rdbOut.Checked = true;
                }
                else
                {
                    this.rdbDeath.Checked = true;
                }

                EMRDBLib.QcMsgDict qcMessageTemplet = LstQCMessageTemplet.Find(delegate(EMRDBLib.QcMsgDict t) { return t.QC_MSG_CODE == QCMessageTemplet.QC_MSG_CODE; });
                if (qcMessageTemplet != null)
                    this.lbCurrentScoreInfo.Text = "�����׼������" + Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcMessageTemplet.SCORE, 0f)), 1).ToString("F1"); 
                this.lbCurrentScoreInfo.Tag = qcMessageTemplet;
                List<EMRDBLib.QcMsgDict> lstQCMessageTemplet = LstQCMessageTemplet;
                //short shRet = DataAccess.GetQCMessageTempletList(ref lstQCMessageTemplet);
                if (lstQCMessageTemplet == null)
                {
                    MessageBoxEx.Show("�ʿ��ʼ������ֵ���ȡʧ�ܣ�");
                }
                foreach (EMRDBLib.QcMsgDict item in lstQCMessageTemplet)
                {
                    if (item.QC_MSG_CODE == this.QCMessageTemplet.QC_MSG_CODE)
                    {
                        this.SetScoreInfos(item);
                        //���޸ķ�����ʱ�򣬼�ȥ��ǰѡ�е�
                        double scoreLevel1Count = 0.0;//�������
                        double scoreLevel2Count = 0.0;//�������
                        double currentScore = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.txtBoxScore.Text, 0f)), 1);
                        scoreLevel1Count = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.txtLevel1Score.Text, 0f)), 1);
                        scoreLevel2Count = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.txtLevel2Socre.Text, 0f)), 1);
                        scoreLevel1Count -= currentScore;
                        scoreLevel2Count -= currentScore;
                        this.txtLevel1Score.Text = scoreLevel1Count.ToString();
                        this.txtLevel2Socre.Text = scoreLevel2Count.ToString();
                        break;
                    }
                }
            }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtQuestionType.Text) || this.txtQuestionType.Text == "<˫��ѡ��>")
            {
                MessageBoxEx.Show("��˫��ѡ����������!");
                return;
            }
            if (string.IsNullOrEmpty(this.txtMessage.Text))
            {
                MessageBoxEx.Show("�������ʼ���������!");
                this.txtMessage.Focus();
                return;
            }
            bool bPassed = CheckBoxScore();
            if (!bPassed)
                return;

            if (this.QCMessageTemplet == null)
                this.QCMessageTemplet = new EMRDBLib.QcMsgDict();
            this.QCMessageTemplet.QA_EVENT_TYPE = this.txtQuestionType.Text;
            this.QCMessageTemplet.QC_MSG_CODE = (string)this.txtMessage.Tag;
            this.QCMessageTemplet.MESSAGE = (string)this.txtMessage.Text;
            this.QCMessageTemplet.MESSAGE_TITLE = (string)this.txtMesssageTitle.Text;
            this.QCMessageTemplet.SCORE =  string.IsNullOrEmpty(this.txtBoxScore.Text)?0:float.Parse(this.txtBoxScore.Text);
            //���ò�������
            if (this.rdbIn.Checked)
            {
                this.QCMessageTemplet.QCDocType = SystemData.QCDocType.INHOSPITAL;
            }
            else if (this.rdbOut.Checked)
            {
                this.QCMessageTemplet.QCDocType = SystemData.QCDocType.OUTHOSPITAL;
            }
            else
            {
                this.QCMessageTemplet.QCDocType = SystemData.QCDocType.DEATH;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void txtQuestionType_DoubleClick(object sender, EventArgs e)
        {
            if (this.txtQuestionType.Text == "<˫��ѡ��>")
                this.txtQuestionType.Clear();
            QuestionTypeForm frmQuestionType = new QuestionTypeForm();
            if (frmQuestionType.ShowDialog() != DialogResult.OK)
                return;
            this.txtQuestionType.Text = frmQuestionType.QuestionType;
            this.txtMessage.Text = string.Concat(this.txtMessage.Text, frmQuestionType.MessageTemplet);
            this.txtMesssageTitle.Text = frmQuestionType.MessageTempletTitle;
            this.txtMessage.Tag = frmQuestionType.MessageCode;
            this.lbCurrentScoreInfo.Text = "�����׼������" + Math.Round(new decimal(GlobalMethods.Convert.StringToValue(frmQuestionType.Score, 0f)), 1).ToString("F1"); ;
            this.lbCurrentScoreInfo.Tag = frmQuestionType.SelectedQCMessageTemplet;
            this.SetScoreInfos(frmQuestionType.SelectedQCMessageTemplet);
            this.txtBoxScore.Text = Math.Round(new decimal(GlobalMethods.Convert.StringToValue(frmQuestionType.Score, 0f)), 1).ToString("F1");
            this.txtBoxIsVeto.Text = frmQuestionType.SelectedQCMessageTemplet.ISVETO ? "��" : "��";
            this.txtBoxIsVeto.ForeColor= frmQuestionType.SelectedQCMessageTemplet.ISVETO ? Color.Red: Color.Black;
            this.txtMessage.Focus();
            this.txtMessage.SelectAll();
        }

        /// <summary>
        /// ���ÿ۷�������Ϣ
        /// </summary>
        /// <param name="frmQuestionType"></param>
        private void SetScoreInfos(EMRDBLib.QcMsgDict qcMessageTemplet)
        {
            string szLevel1Socre = string.Empty;
            string szLevel2Socre = string.Empty;
            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;

            List<EMRDBLib.MedicalQcMsg> lstQCQuestionInfos = null;
            short shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(szPatientID, szVisitID,  ref lstQCQuestionInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet!= SystemData.ReturnValue.RES_NO_FOUND)
            {
                MessageBoxEx.Show("�ʿ��ʼ���������ʧ�ܣ�");
                return;
            }
            if (lstQCQuestionInfos == null || lstQCQuestionInfos.Count <= 0)
            {
                szLevel1Socre = "0.0";
                szLevel2Socre = "0.0";
            }
            else
            {
                double scoreLevel1Count = 0.0;
                double scoreLevel2Count = 0.0;
                foreach (EMRDBLib.MedicalQcMsg qcQuestionInfo in lstQCQuestionInfos)
                {
                    if (qcQuestionInfo.QA_EVENT_TYPE == qcMessageTemplet.QA_EVENT_TYPE)//�������۷��ۼ�
                    {
                        scoreLevel1Count += (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcQuestionInfo.POINT, 0f)), 1);
                    }
                    EMRDBLib.QcMsgDict itemQCMessageTemplet = LstQCMessageTemplet.Find(delegate(EMRDBLib.QcMsgDict p) { return p.QC_MSG_CODE == qcQuestionInfo.QC_MSG_CODE; });
                    if (itemQCMessageTemplet == null)
                        continue;
                    //��������۷��ۼ�
                    //�����������ⰴ������������

                    if (!string.IsNullOrEmpty(qcMessageTemplet.MESSAGE_TITLE) &&
                        itemQCMessageTemplet.MESSAGE_TITLE == qcMessageTemplet.MESSAGE_TITLE)
                    {
                        scoreLevel2Count += (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcQuestionInfo.POINT, 0f)), 1);
                    } //û������������İ���ԭ������������CODE����
                    else if (string.IsNullOrEmpty(qcMessageTemplet.MESSAGE_TITLE) &&
                             itemQCMessageTemplet.QC_MSG_CODE == qcMessageTemplet.QC_MSG_CODE)
                    {
                        scoreLevel2Count += (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcQuestionInfo.POINT, 0f)), 1);
                    }

                }
                this.txtLevel1Score.Text = scoreLevel1Count.ToString();
                this.txtLevel2Socre.Text = scoreLevel2Count.ToString();
            }
            //���޷�������
            if (LstQCEventTypes != null)
            {
                double scoreLevel1Max = 0.0;
                double scoreLevel2Max = 0.0;
                foreach (EMRDBLib.QaEventTypeDict qcEventType in LstQCEventTypes)
                {
                    //��������������
                    if (qcEventType.QA_EVENT_TYPE == qcMessageTemplet.QA_EVENT_TYPE)
                    {
                        scoreLevel1Max = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcEventType.MAX_SCORE, 0f)), 1);
                        if (scoreLevel1Max == 0.0)
                        {
                            this.lblevel1MaxScore.Text = "�������޷�����������";
                        }
                        else
                        {
                            this.lblevel1MaxScore.Text = "�������޷�����" + scoreLevel1Max.ToString();
                            this.lblevel1MaxScore.Tag = scoreLevel1Max;
                        }
                    }
                    //�����������ͷ�������
                    if (!string.IsNullOrEmpty(qcEventType.PARENT_CODE) && qcEventType.QA_EVENT_TYPE == qcMessageTemplet.MESSAGE_TITLE)
                    {
                        scoreLevel2Max = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcEventType.MAX_SCORE, 0f)), 1);
                        if (scoreLevel2Max == 0.0)
                        {
                            this.lblevel2MaxScore.Text = "�������޷�����������";
                        }
                        else
                        {
                            this.lblevel2MaxScore.Text = "�������޷�����" + scoreLevel2Max.ToString();
                            this.lblevel2MaxScore.Tag = scoreLevel2Max;
                        }
                    }
                    //û��MessageTitle����������
                    if (string.IsNullOrEmpty(qcMessageTemplet.MESSAGE_TITLE))
                    {
                        this.lblevel2MaxScore.Text = "�������޷�����������";
                    }
                }
            }
            else
            {
                this.lblevel1MaxScore.Text = "�������޷�����δ֪";
                this.lblevel2MaxScore.Text = "�������޷�����δ֪";
            }
        }

        private void txtBoxScore_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //��ֹ�ո��  
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //������  
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //����Ƿ��ַ�  
                }
            }
        }

        private bool CheckBoxScore()
        {
            string szText = this.txtBoxScore.Text;
            try
            {
                if (string.IsNullOrEmpty(szText))
                    this.txtBoxScore.Text = "0.0";
                int result = 0;
                if (szText.Contains("."))
                    result = szText.Length - szText.IndexOf('.') - 1;
                if (result > 1)
                {
                    MessageBox.Show("�۷ֽ��辫ȷ��С����һλ");
                    this.txtBoxScore.Text = szText.Remove(szText.IndexOf('.') + 2, result - 1);
                    return false;
                }

                double level1ScoreCount = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.txtLevel1Score.Text, 0f)), 1);
                double level2ScoreCount = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.txtLevel2Socre.Text, 0f)), 1);
                double level1ScoreMax = this.lblevel1MaxScore.Tag == null ?
                    9999.9 : (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.lblevel1MaxScore.Tag.ToString(), 0f)), 1);
                double level2ScoreMax = this.lblevel2MaxScore.Tag == null ?
                                    9999.9 : (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(this.lblevel2MaxScore.Tag.ToString(), 0f)), 1);
                double currentScore = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(szText, 0f)), 1);
                EMRDBLib.QcMsgDict qcMessageTemplet = this.lbCurrentScoreInfo.Tag as EMRDBLib.QcMsgDict;
                double templetScore = 0.0;
                if (qcMessageTemplet == null)
                {
                    qcMessageTemplet = LstQCMessageTemplet.Find(delegate(EMRDBLib.QcMsgDict t) { return t.QC_MSG_CODE == QCMessageTemplet.QC_MSG_CODE; });
                }
                if (qcMessageTemplet == null)
                {
                    MessageBox.Show("�ʼ��׼��Ϣ��ȡʧ��");
                    return false;
                }
                else
                {
                    templetScore = (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcMessageTemplet.SCORE, 0f)), 1);
                }
                double leftLevel_1_Socre = (double)Math.Round(level1ScoreMax - level1ScoreCount - currentScore, 1);
                double leftLevel_2_Socre = (double)Math.Round(level2ScoreMax - level2ScoreCount - currentScore, 1);
                if (leftLevel_2_Socre < 0.0)
                {
                    MessageBoxEx.Show("�������ó�����������ɿ۷�����", MessageBoxIcon.Warning);
                    if ((level2ScoreMax - level2ScoreCount - templetScore) <= 0.0)
                        templetScore = level2ScoreMax - level2ScoreCount;
                    if (templetScore < 0)
                        templetScore = 0.0;
                    this.txtBoxScore.Text = templetScore.ToString("F1");
                    return false;
                }
                else if (leftLevel_1_Socre < 0.0)
                {
                    MessageBoxEx.Show("�������ó����������ɿ۷�����", MessageBoxIcon.Warning);
                    if ((level1ScoreMax - level1ScoreCount - templetScore) <= 0.0)
                        templetScore = level1ScoreMax - level1ScoreCount;
                    if (templetScore < 0)
                        templetScore = 0.0;
                    this.txtBoxScore.Text = templetScore.ToString("F1");
                    return false;
                }
                else
                {
                    this.txtBoxScore.Text = currentScore.ToString("F1");
                    return true;
                }

            }
            catch
            {

            }
            return false;
        }

        private void txtBoxScore_MouseLeave(object sender, EventArgs e)
        {
            CheckBoxScore();
        }
    }
}