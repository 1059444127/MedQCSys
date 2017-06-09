// ***********************************************************
// ���ݿ���ʲ�ϵͳ���в���������,���ڵ��ò����޸����в���.
// Creator:YangMingkun  Date:2010-11-18
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.IO;
using EMRDBLib.DbAccess;
using Heren.Common.Libraries;
using Heren.Common.Libraries.DbAccess;
using Heren.Common.Libraries.Ftp;
using System.Text;
using System.Xml;
using EMRDBLib.Entity;
using System.Collections;

namespace EMRDBLib
{
    public partial class SystemParam
    {
        private static SystemParam m_Instance = null;

        /// <summary>
        /// ��ȡSystemParam����ʵ��
        /// </summary>
        public static SystemParam Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SystemParam();
                return m_Instance;
            }
        }

        private SystemParam()
        {
        }

        /// <summary>
        /// ��ȡ������Ĭ��ʱ��
        /// </summary>
        public DateTime DefaultTime
        {
            get { return DateTime.Parse("1900-1-1"); }
        }
        private string m_szWorkPath = string.Empty;

        /// <summary>
        /// ��ȡ�����ó�����·��
        /// </summary>
        public string WorkPath
        {
            set { this.m_szWorkPath = value; }
            get
            {
                if (GlobalMethods.Misc.IsEmptyString(this.m_szWorkPath))
                    this.m_szWorkPath = GlobalMethods.Misc.GetWorkingPath();
                return this.m_szWorkPath;
            }
        }


        /// <summary>
        /// ��ȡָ�����ĵ���Ϣ��֯ҽ���ĵ�FTP·��
        /// </summary>
        /// <param name="docInfo">�ĵ���Ϣ</param>
        /// <param name="extension">�ĵ���չ��</param>
        /// <returns>�ĵ�FTP·��</returns>
        public string GetFtpDocPath(MedDocInfo docInfo, string extension)
        {
            //���Ӳ��˸�Ŀ¼
            StringBuilder ftpPath = new StringBuilder();
            ftpPath.Append("/MEDDOC");

            if (docInfo == null || docInfo.PATIENT_ID == null)
                return ftpPath.ToString();

            string szPatientID = docInfo.PATIENT_ID.PadLeft(10, '0');
            for (int index = 0; index < 10; index += 2)
            {
                ftpPath.Append("/");
                ftpPath.Append(szPatientID.Substring(index, 2));
            }

            //���Ӿ���Ŀ¼
            ftpPath.Append("/");
            ftpPath.Append(docInfo.VISIT_TYPE);
            if (docInfo.VISIT_TYPE == SystemData.VisitType.OP)
            {
                ftpPath.Append("_");
                ftpPath.Append(docInfo.VISIT_TIME.ToString("yyyyMMddHHmmss"));
            }
            ftpPath.Append("_");
            ftpPath.Append(docInfo.VISIT_ID);
            ftpPath.Append("/");
            if (docInfo.EMR_TYPE == "HEREN")
            {
                ftpPath.Append(string.Format("{0}.{1}", docInfo.DOC_SETID, extension));
            }
            else
                ftpPath.Append(string.Format("{0}.{1}", docInfo.DOC_ID, extension));
            return ftpPath.ToString();
        }


        private QChatArgs m_QChatArgs = null;

        /// <summary>
        /// ��ͨƽ̨�ڲ�����
        /// </summary>
        public QChatArgs QChatArgs
        {
            get
            {
                if (m_QChatArgs == null)
                    m_QChatArgs = new QChatArgs();
                return this.m_QChatArgs;
            }
            set { this.m_QChatArgs = value; }
        }


        private UserInfo m_userInfo = null;

        /// <summary>
        /// ��ȡ�������û���Ϣ
        /// </summary>
        public UserInfo UserInfo
        {
            get { return this.m_userInfo; }
            set { this.m_userInfo = value; }
        }

        private List<DeptInfo> m_UserAdminDepts = null;

        /// <summary>
        /// ��ǰ�û�����Ͻ�Ŀ���
        /// </summary>
        public List<DeptInfo> UserAdminDepts
        {
            get { return m_UserAdminDepts; }
            set { m_UserAdminDepts = value; }
        }

        private EMRDBLib.PatVisitInfo m_patVisitLog = null;

        /// <summary>
        /// ��ȡ�����ò��˼�����������Ϣ
        /// </summary>
        public EMRDBLib.PatVisitInfo PatVisitInfo
        {
            get { return this.m_patVisitLog; }
            set
            {
                if (this.m_patVisitLog != value)
                    this.m_patVisitLog = value;
            }
        }

        private List<EMRDBLib.PatVisitInfo> m_lstPatVisitLog = null;

        /// <summary>
        /// ��ȡ�����ò��˾�����Ϣ�б�
        /// </summary>
        public List<EMRDBLib.PatVisitInfo> PatVisitLogList
        {
            get { return this.m_lstPatVisitLog; }
            set
            {
                if (this.m_lstPatVisitLog != value)
                    this.m_lstPatVisitLog = value;
            }
        }

        private Hashtable m_htPatVisitLogTable = null;

        /// <summary>
        /// ��ȡ�����ò��˾�����Ϣ��ϣ��
        /// </summary>
        public Hashtable PatVisitLogTable
        {
            get
            {
                if (this.m_htPatVisitLogTable == null)
                    this.m_htPatVisitLogTable = new Hashtable();
                return this.m_htPatVisitLogTable;
            }
            set
            {
                if (this.m_htPatVisitLogTable != value)
                    this.m_htPatVisitLogTable = value;
            }
        }

        private static DateTime m_dtBeginTime = DateTime.MinValue;

        /// <summary>
        /// ��ѯ��ʼʱ��
        /// </summary>
        public static DateTime BeginTime
        {
            get { return m_dtBeginTime; }
            set { m_dtBeginTime = value; }
        }

        private static DateTime m_dtEndTime = DateTime.MinValue;

        /// <summary>
        /// ��ѯ����ʱ��
        /// </summary>
        public static DateTime EndTime
        {
            get { return m_dtEndTime; }
            set { m_dtEndTime = value; }
        }

        private UserRight m_userRight = null;

        /// <summary>
        /// ��ȡ��ǰ�û���Ȩ����Ϣ(�ö��󲻻�Ϊ��)
        /// </summary>
        public UserRight UserRight
        {
            get
            {
                if (this.m_userInfo == null)
                    return new UserRight();

                //�û�Ȩ��Ϊ��ʱ���û�ID�Ѹı�ʱ,���»�ȡ
                if (this.m_userRight == null || this.m_userRight.UserID != this.m_userInfo.ID)
                {
                    UserRightBase userRight = null;
                    RightAccess.Instance.GetUserRight(this.m_userInfo.ID, UserRightType.MedDoc,
                        ref userRight);
                    this.m_userRight = userRight as UserRight;
                }
                if (this.m_userRight == null)
                    return new UserRight();
                return this.m_userRight;
            }
        }

        private QCUserRight m_QCUserRight = null;

        /// <summary>
        /// ��ȡ��ǰ�ʿ��û���Ȩ����Ϣ(�ö��󲻻�Ϊ��)
        /// </summary>
        public QCUserRight QCUserRight
        {
            get
            {
                if (this.m_userInfo == null)
                    return new QCUserRight();

                //�û�Ȩ��Ϊ��ʱ���û�ID�Ѹı�ʱ,���»�ȡ
                if (this.m_QCUserRight == null || this.m_QCUserRight.UserID != this.m_userInfo.ID)
                {
                    UserRightBase userRight = null;
                    RightAccess.Instance.GetUserRight(this.m_userInfo.ID, UserRightType.MedQC,
                        ref userRight);
                    this.m_QCUserRight = userRight as QCUserRight;
                }
                if (this.m_QCUserRight == null)
                    return new QCUserRight();
                return this.m_QCUserRight;
            }
        }
        private Hashtable m_htDMLB = null;
        public Hashtable HtDMLB
        {
            get {
                if (this.m_htDMLB == null)
                {
                    this.m_htDMLB = new Hashtable();
                    m_htDMLB.Add("�Ա�", "1");
                    m_htDMLB.Add("����", "7");
                    m_htDMLB.Add("ְҵ", "3");
                    m_htDMLB.Add("����", "4");
                    m_htDMLB.Add("����/סַ/������", "6");
                    m_htDMLB.Add("����", "8");
                    m_htDMLB.Add("��ϵ", "2");
                    m_htDMLB.Add("��������", "21");
                    m_htDMLB.Add("��Ժ���", "20");
                    m_htDMLB.Add("������ϱ�־", "");
                    m_htDMLB.Add("�ų����ϱ�־", "");
                    m_htDMLB.Add("�Ƿ�ʬ��", "");
                    //m_htDMLB.Add("���������", "");
                    m_htDMLB.Add("����M����", "");
                    m_htDMLB.Add("����E����", "");
                    m_htDMLB.Add("���Ҵ���", "");
                    m_htDMLB.Add("ABOѪ��", "30");
                    m_htDMLB.Add("RHѪ��", "RHѪ��");
                    m_htDMLB.Add("��Ժ��ʽ", "23");
                    m_htDMLB.Add("��Ժ��ʽ", "");
                    m_htDMLB.Add("������", "");
                    m_htDMLB.Add("ת�����/���ƽ��", "28");
                }
                return m_htDMLB;
            }
        }

        private Hashtable m_htBaseCodeDict = null;
        public Hashtable HtBaseCodeDict
        {
            get
            {
                if (this.m_htBaseCodeDict == null)
                {
                    this.m_htBaseCodeDict = new Hashtable();
                    m_htBaseCodeDict.Add("�Ա�", "SEX_DICT");
                    m_htBaseCodeDict.Add("����", "MARITAL_STATUS_DICT");
                    m_htBaseCodeDict.Add("ְҵ", "OCCUPATION_DICT");
                    m_htBaseCodeDict.Add("����", "COUNTRY_DICT");
                    m_htBaseCodeDict.Add("����/סַ/������", "AREA_DICT");
                    m_htBaseCodeDict.Add("����", "NATION_DICT");
                    m_htBaseCodeDict.Add("��ϵ", "RELATIONSHIP_DICT");
                    m_htBaseCodeDict.Add("��������", "MR_QUALITY_DICT");
                    m_htBaseCodeDict.Add("��Ժ���", "PAT_ADM_CONDITION_DICT");
                    m_htBaseCodeDict.Add("������ϱ�־", "");
                    m_htBaseCodeDict.Add("ABOѪ��", "BLOOD_ABO_TYPE_DICT");
                    m_htBaseCodeDict.Add("RHѪ��", "BLOOD_RH_TYPE_DICT");
                    m_htBaseCodeDict.Add("��Ժ��ʽ", "PATIENT_CLASS_DICT");
                    m_htBaseCodeDict.Add("��Ժ��ʽ", "DISCHARGE_DISPOSITION_DICT");
                    m_htBaseCodeDict.Add("������", "DIAGNOSIS_TYPE_DICT");
                    m_htBaseCodeDict.Add("ת�����/���ƽ��", "TREATING_RESULT_DICT");
                }
                return m_htBaseCodeDict;
            }
        }

        /// <summary>
        /// ��ȡ�ʿ�ϵͳ�����ļ�ȫ·��
        /// </summary>
        public string ConfigFile
        {
            get
            {
                return string.Format(@"{0}\MedQCSys.xml"
                    , GlobalMethods.Misc.GetWorkingPath());
            }
        }

        /// <summary>
        /// ��ȡָ�����ĵ���Ϣ��֯ҽ���ĵ�FTP·��
        /// </summary>
        /// <returns>�ĵ�FTP·��</returns>
        public string GetQCFtpDocPath(EMRDBLib.MedicalQcMsg questionInfo, string szFileExt)
        {
            //���Ӳ��˸�Ŀ¼
            StringBuilder sbDocPath = new StringBuilder();
            sbDocPath.Append("/MEDDOC/�ʿز���");
            if (string.IsNullOrEmpty(questionInfo.ISSUED_BY))
                return sbDocPath.ToString();
            sbDocPath.AppendFormat("/{0}", questionInfo.ISSUED_BY);
            if (string.IsNullOrEmpty(questionInfo.PATIENT_ID))
                return sbDocPath.ToString();
            string szPatientID = questionInfo.PATIENT_ID.PadLeft(10, '0');
            for (int index = 0; index < 10; index += 2)
            {
                sbDocPath.Append("/");
                sbDocPath.Append(szPatientID.Substring(index, 2));
            }
            sbDocPath.Append("/");
            sbDocPath.Append(string.Format("{0}_{1}.{2}", questionInfo.TOPIC_ID,
                questionInfo.ISSUED_DATE_TIME.ToString("yyyyMMddHHmmss"), szFileExt));
            return sbDocPath.ToString();
        }

        /// <summary>
        /// ��ǰ�û��Ƿ������Ȩ�� 
        /// </summary>
        public bool CurrentUserHasQCCheckRight
        {
            get
            {
                bool bRight = false;
                if (!QCUserRight.ManageAllQC.Value)
                {
                    if (QCUserRight.ManageAdminDeptsQC.Value)
                    {
                        //�ж��Ƿ��û���Ͻ���Ұ������˿���
                        if (UserAdminDepts != null && UserAdminDepts.Count > 0)
                        {
                            foreach (DeptInfo deptInfo in UserAdminDepts)
                            {
                                if (deptInfo.DEPT_CODE == PatVisitInfo.DEPT_CODE)
                                {
                                    bRight = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (QCUserRight.ManageDeptQC.Value)
                    {
                        //�ж��Ƿ��û������벡�˿���һ��
                        if (UserInfo.DeptCode == PatVisitInfo.DEPT_CODE)
                        {
                            bRight = true;
                        }
                    }
                }
                else
                {
                    bRight = true;
                }
                return bRight;
            }
        }


    }
}
