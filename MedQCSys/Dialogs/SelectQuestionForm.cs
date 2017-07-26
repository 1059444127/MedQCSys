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
using System.Linq;
using EMRDBLib.DbAccess;
using EMRDBLib;

namespace MedQCSys.Dialogs
{
    public partial class SelectQuestionForm : HerenForm
    {
        private MedicalQcMsg m_MedicalQcMsg = null;
        /// <summary>
        /// ��ȡ�������ʼ�����ģ����
        /// </summary>
        public MedicalQcMsg MedicalQcMsg
        {
            get { return this.m_MedicalQcMsg; }
            set { this.m_MedicalQcMsg = value; }
        }

        private static List<QaEventTypeDict> lstQCEventTypes = null;

        public static List<QaEventTypeDict> LstQCEventTypes
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

        private static List<QcMsgDict> m_lstQcMsgDicts = null;

        public static List<QcMsgDict> ListQcMsgDict
        {
            get
            {
                if (m_lstQcMsgDicts == null)
                {
                    short shRet = QcMsgDictAccess.Instance.GetQcMsgDictList(ref m_lstQcMsgDicts);
                    if (shRet != SystemData.ReturnValue.OK)
                    {
                        MessageBoxEx.Show("��ȡ����������������ֵ�ʧ�ܣ�");
                    }
                }
                return SelectQuestionForm.m_lstQcMsgDicts;
            }
        }

        public SelectQuestionForm()
        {
            InitializeComponent();
            string caption = "��ʾ:";
            if (SystemParam.Instance.LocalConfigOption.VetoHigh > 0)
            {
                caption += string.Format("\n{0}������������ó��Ҳ���"
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
            this.txtChecker.Text = SystemParam.Instance.UserInfo.USER_ID;
            if (SystemParam.Instance.PatVisitInfo == null)
                return;
            if (String.IsNullOrEmpty(SystemParam.Instance.PatVisitInfo.DISCHARGE_MODE))
                this.rdbIn.Checked = true;
            else if (SystemParam.Instance.PatVisitInfo.DISCHARGE_MODE == "����")
                this.rdbDeath.Checked = true;
            else
                this.rdbOut.Checked = true;
            this.txtPatientID.Text = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            this.txtPatName.Text = SystemParam.Instance.PatVisitInfo.PATIENT_NAME;
            this.txtPatSex.Text = SystemParam.Instance.PatVisitInfo.PATIENT_SEX;
            this.txtDocTitle.Text = this.m_MedicalQcMsg.TOPIC;
            this.txtQuestionType.Text = "<˫��ѡ��>";
            this.txt_ISSUED_DATE_TIME.Text = this.m_MedicalQcMsg.ISSUED_DATE_TIME.ToString("yyyy-M-d HH:mm");
            if (this.MedicalQcMsg != null && !string.IsNullOrEmpty(this.m_MedicalQcMsg.QC_MSG_CODE))
            {
                if (this.m_MedicalQcMsg.ASK_DATE_TIME != this.m_MedicalQcMsg.DefaultTime)
                    this.txtAskDateTime.Text = this.m_MedicalQcMsg.ASK_DATE_TIME.ToString();
                this.txtDoctorComment.Text = this.m_MedicalQcMsg.DOCTOR_COMMENT;
                this.txtQuestionType.Text = this.MedicalQcMsg.QA_EVENT_TYPE;
                this.txtMessage.Text = this.MedicalQcMsg.MESSAGE;
                this.txtMessage.Focus();
                this.txtMessage.SelectAll();
                this.txtMessage.Tag = this.MedicalQcMsg.QC_MSG_CODE;
                this.txtBoxScore.Text = this.MedicalQcMsg.POINT.ToString();
                //�޸Ļ������
                //���ò�������
                if (this.MedicalQcMsg.QCDOC_TYPE == SystemData.QCDocType.INHOSPITAL)
                {
                    this.rdbIn.Checked = true;
                }
                else if (this.MedicalQcMsg.QCDOC_TYPE == SystemData.QCDocType.OUTHOSPITAL)
                {
                    this.rdbOut.Checked = true;
                }
                else
                {
                    this.rdbDeath.Checked = true;
                }
                if (!string.IsNullOrEmpty(this.m_MedicalQcMsg.QC_MSG_CODE))
                {
                    QcMsgDict qcMsgDict = ListQcMsgDict.Where(m => m.QC_MSG_CODE == this.m_MedicalQcMsg.QC_MSG_CODE).FirstOrDefault();
                    this.txtMesssageTitle.Text = qcMsgDict.MESSAGE_TITLE;
                    if (qcMsgDict != null)
                        this.lbCurrentScoreInfo.Text = "�����׼������" + Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcMsgDict.SCORE, 0f)), 1).ToString("F1");
                    this.lbCurrentScoreInfo.Tag = qcMsgDict;
                    List<QcMsgDict> lstQCMessageTemplet = ListQcMsgDict;
                    if (lstQCMessageTemplet == null)
                    {
                        MessageBoxEx.Show("�ʿ��ʼ������ֵ���ȡʧ�ܣ�");
                    }
                    var item = lstQCMessageTemplet.Where(m => m.QC_MSG_CODE == this.m_MedicalQcMsg.QC_MSG_CODE).FirstOrDefault();
                    if(item!=null)
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
                     
                    }
                }
            }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(MedicalQcMsg.QC_MSG_CODE))
            {
                MessageBoxEx.Show("��˫��ѡ����������!");
                return;
            }
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
            this.MedicalQcMsg.MESSAGE = (string)this.txtMessage.Text;
            this.MedicalQcMsg.POINT = string.IsNullOrEmpty(this.txtBoxScore.Text) ? 0 : float.Parse(this.txtBoxScore.Text);
            //���ò�������
            if (this.rdbIn.Checked)
            {
                this.MedicalQcMsg.QCDOC_TYPE = SystemData.QCDocType.INHOSPITAL;
            }
            else if (this.rdbOut.Checked)
            {
                this.MedicalQcMsg.QCDOC_TYPE = SystemData.QCDocType.OUTHOSPITAL;
            }
            else
            {
                this.MedicalQcMsg.QCDOC_TYPE = SystemData.QCDocType.DEATH;
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
            this.m_MedicalQcMsg.QC_MSG_CODE = frmQuestionType.QcMsgCode;
            this.m_MedicalQcMsg.QA_EVENT_TYPE = frmQuestionType.QaEventType;
            this.m_MedicalQcMsg.MESSAGE = frmQuestionType.Message;
            this.txtQuestionType.Text = frmQuestionType.QaEventType;
            this.txtMessage.Text = string.Concat(this.txtMessage.Text, frmQuestionType.Message);
            this.txtMesssageTitle.Text = frmQuestionType.MessageTitle;
            this.txtMessage.Tag = frmQuestionType.QcMsgCode;
            this.lbCurrentScoreInfo.Text = "�����׼������" + Math.Round(new decimal(GlobalMethods.Convert.StringToValue(frmQuestionType.Score, 0f)), 1).ToString("F1"); ;
            this.lbCurrentScoreInfo.Tag = frmQuestionType.SelectedQCMessageTemplet;
            this.SetScoreInfos(frmQuestionType.SelectedQCMessageTemplet);
            this.txtBoxScore.Text = Math.Round(new decimal(GlobalMethods.Convert.StringToValue(frmQuestionType.Score, 0f)), 1).ToString("F1");
            this.txtBoxIsVeto.Text = frmQuestionType.SelectedQCMessageTemplet.ISVETO ? "��" : "��";
            this.txtBoxIsVeto.ForeColor = frmQuestionType.SelectedQCMessageTemplet.ISVETO ? Color.Red : Color.Black;
            this.txtMessage.Focus();
            this.txtMessage.SelectAll();
        }

        /// <summary>
        /// ���ÿ۷�������Ϣ
        /// </summary>
        /// <param name="frmQuestionType"></param>
        private void SetScoreInfos(QcMsgDict qcMessageTemplet)
        {
            string szLevel1Socre = string.Empty;
            string szLevel2Socre = string.Empty;
            string szPatientID = SystemParam.Instance.PatVisitInfo.PATIENT_ID;
            string szVisitID = SystemParam.Instance.PatVisitInfo.VISIT_ID;

            List<MedicalQcMsg> lstQCQuestionInfos = null;
            short shRet = MedicalQcMsgAccess.Instance.GetMedicalQcMsgList(szPatientID, szVisitID, ref lstQCQuestionInfos);
            if (shRet != SystemData.ReturnValue.OK
                && shRet != SystemData.ReturnValue.RES_NO_FOUND)
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
                foreach (MedicalQcMsg qcQuestionInfo in lstQCQuestionInfos)
                {
                    if (qcQuestionInfo.QA_EVENT_TYPE == qcMessageTemplet.QA_EVENT_TYPE)//�������۷��ۼ�
                    {
                        scoreLevel1Count += (double)Math.Round(new decimal(GlobalMethods.Convert.StringToValue(qcQuestionInfo.POINT, 0f)), 1);
                    }
                    QcMsgDict itemQCMessageTemplet = ListQcMsgDict.Find(delegate (QcMsgDict p) { return p.QC_MSG_CODE == qcQuestionInfo.QC_MSG_CODE; });
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
                foreach (QaEventTypeDict qcEventType in LstQCEventTypes)
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
            if (string.IsNullOrEmpty(MedicalQcMsg.QC_MSG_CODE))
            {
                return false;
            }
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
                QcMsgDict qcMessageTemplet = this.lbCurrentScoreInfo.Tag as QcMsgDict;
                double templetScore = 0.0;
                if (qcMessageTemplet == null)
                {
                    qcMessageTemplet = ListQcMsgDict.Find(delegate (QcMsgDict t) { return t.QC_MSG_CODE == MedicalQcMsg.QC_MSG_CODE; });
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