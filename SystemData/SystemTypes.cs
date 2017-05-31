// ***********************************************************
// ��װ�ͻ��˽�������ڹ�������ͼ���
// Creator:YangMingkun  Date:2009-6-22
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Text;
using MedDocSys.Common;

namespace MedDocSys.DataLayer
{
    #region "enum"
    /// <summary>
    /// �û���ɫö��
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// סԺҽ��(0)IP doctor
        /// </summary>
        InDoctor = 0,
        /// <summary>
        /// ����ҽ��(1)OP doctor
        /// </summary>
        OutDoctor = 1,
        /// <summary>
        /// ��ʿ(2)
        /// </summary>
        Nurse = 2,
        /// <summary>
        /// ������Ա(3)
        /// </summary>
        Other = 3
    }

    /// <summary>
    /// �û��ȼ�ö��
    /// </summary>
    public enum UserLevel
    {
        /// <summary>
        /// ����,����ҽʦ
        /// </summary>
        Normal = 0,
        /// <summary>
        /// �ϼ�ҽʦ
        /// </summary>
        Higher = 1,
        /// <summary>
        /// ����ҽʦ
        /// </summary>
        Chief = 2
    }

    /// <summary>
    /// �û�Ȩ��ö��
    /// </summary>
    public enum UserPower
    {
        /// <summary>
        /// ���ɼ�(0)
        /// </summary>
        Invisible = 0,
        /// <summary>
        /// ֻ��(1)
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// ֻ���ʹ�ӡ(2)
        /// </summary>
        ReadPrint = 2,
        /// <summary>
        /// ֻ����ӡ�͵���(3)
        /// </summary>
        ReadPrintExport = 3,
        /// <summary>
        /// �ɱ༭(4)
        /// </summary>
        Editable = 4
    }

    /// <summary>
    /// ��������ö��
    /// </summary>
    public enum VisitType
    {
        /// <summary>
        /// ����(0)
        /// </summary>
        OP = 0,
        /// <summary>
        /// ����(1)
        /// </summary>
        EP = 1,
        /// <summary>
        /// סԺ(2)
        /// </summary>
        IP = 2,
        /// <summary>
        /// ����(3)
        /// </summary>
        Other = 3
    }

    /// <summary>
    /// ���ݿ��ĵ�״̬
    /// </summary>
    public enum ServerDocState
    {
        /// <summary>
        /// δ֪״̬(0)
        /// </summary>
        None = 0,
        /// <summary>
        /// ����(1)
        /// </summary>
        Normal = 1,
        /// <summary>
        /// ����(2)
        /// </summary>
        Locked = 2,
        /// <summary>
        /// �鵵(3)
        /// </summary>
        Archived = 3,
        /// <summary>
        /// ����(4)
        /// </summary>
        Canceled = 4
    }

    /// <summary>
    /// �ͻ����ĵ�״̬
    /// </summary>
    public enum DocumentState
    {
        /// <summary>
        /// δ֪״̬(0)
        /// </summary>
        None = 0,
        /// <summary>
        /// �½�״̬(1)
        /// </summary>
        New = 1,
        /// <summary>
        /// �༭״̬(2)
        /// </summary>
        Edit = 2,
        /// <summary>
        /// �޶�״̬(3)
        /// </summary>
        Revise = 3,
        /// <summary>
        /// ���״̬(4)
        /// </summary>
        View = 4
    }

    /// <summary>
    /// ҳ�沼����ʽ
    /// </summary>
    public enum PageLayout
    {
        /// <summary>
        /// ��ͨ
        /// </summary>
        Normal = 0,
        /// <summary>
        /// װ�����ڶ���
        /// </summary>
        TopMirrored = 1,
        /// <summary>
        /// װ���������
        /// </summary>
        LeftMirrored = 2
    }

    /// <summary>
    /// ҳ���ı�������ʽ
    /// </summary>
    public enum PageFooterAlign
    {
        /// <summary>
        /// ����
        /// </summary>
        Middle = 0,
        /// <summary>
        /// �����
        /// </summary>
        Left = 1,
        /// <summary>
        /// ���Ҳ�
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// �ĵ���ʷ�汾
    /// </summary>
    public enum HistoryVersion
    {
        /// <summary>
        /// ��һ���汾
        /// </summary>
        First,
        /// <summary>
        /// ǰһ���汾
        /// </summary>
        Prevision,
        /// <summary>
        /// ��һ���汾
        /// </summary>
        Next,
        /// <summary>
        /// ���°汾
        /// </summary>
        Latest
    }
    #endregion

    #region "class"
    /// <summary>
    /// �����С�ṹ
    /// </summary>
    public class FontSize
    {
        public string m_szName = "����";
        public float m_fSize = 14f;
        /// <summary>
        /// ��ȡ�������ֺ�����
        /// </summary>
        public string Name
        {
            get { return this.m_szName; }
            set { this.m_szName = value; }
        }
        /// <summary>
        /// ��ȡ�������ֺŸ����С
        /// </summary>
        public float Size
        {
            get { return this.m_fSize; }
            set { this.m_fSize = value; }
        }

        public FontSize(string name, float size)
        {
            Name = name;
            Size = size;
        }

        public override string ToString()
        {
            return this.m_szName;
        }
    }

    /// <summary>
    /// ϵͳ�û���Ϣ
    /// </summary>
    public class UserInfo : ICloneable
    {
        protected string m_szID = string.Empty;         //�û�ID
        protected string m_szName = string.Empty;       //�û���
        protected string m_szDeptCode = string.Empty;   //���Ҵ���
        protected int m_nDeptCode = 0;                  //��ϣֵ��ʾ�Ŀ��Ҵ���
        protected string m_szDeptName = string.Empty;   //��������
        protected string m_szPwd = string.Empty;        //�û�����
        protected UserRole m_eRole = UserRole.Other;    //�û���ɫ
        protected UserLevel m_eLevel = UserLevel.Normal;//�û��ȼ�
        protected UserPower m_ePower = UserPower.Invisible;  //�û�Ȩ��

        /// <summary>
        /// ��ȡ�������û�ID
        /// </summary>
        public string ID
        {
            get { return this.m_szID; }
            set { this.m_szID = value; }
        }
        /// <summary>
        /// ��ȡ�������û���
        /// </summary>
        public string Name
        {
            get { return this.m_szName; }
            set { this.m_szName = value; }
        }
        /// <summary>
        /// ��ȡ�������û��Ŀ��Ҵ���
        /// </summary>
        public string DeptCode
        {
            get { return this.m_szDeptCode; }
            set
            {
                this.m_nDeptCode = 0;
                this.m_szDeptCode = value;
            }
        }
        /// <summary>
        /// ��ȡ�������û��Ŀ�������
        /// </summary>
        public string DeptName
        {
            get { return this.m_szDeptName; }
            set { this.m_szDeptName = value; }
        }
        /// <summary>
        /// ��ȡ�������û�����
        /// </summary>
        public string Password
        {
            get { return this.m_szPwd; }
            set { this.m_szPwd = value; }
        }
        /// <summary>
        /// ��ȡ�������û���ɫ
        /// </summary>
        public UserRole Role
        {
            get { return this.m_eRole; }
            set { this.m_eRole = value; }
        }
        /// <summary>
        /// ��ȡ�������û��ȼ�
        /// </summary>
        public UserLevel Level
        {
            get { return this.m_eLevel; }
            set { this.m_eLevel = value; }
        }
        /// <summary>
        /// ��ȡ�������û�Ȩ��
        /// </summary>
        public UserPower Power
        {
            get { return this.m_ePower; }
            set { this.m_ePower = value; }
        }

        public UserInfo()
        {
            this.Initialize();
        }

        public object Clone()
        {
            UserInfo userInfo = new UserInfo();
            userInfo.ID = this.m_szID;
            userInfo.Name = this.m_szName;
            userInfo.DeptCode = this.m_szDeptCode;
            userInfo.DeptName = this.m_szDeptName;
            userInfo.Password = this.m_szPwd;
            userInfo.Role = this.m_eRole;
            userInfo.Level = this.m_eLevel;
            userInfo.Power = this.m_ePower;
            return userInfo;
        }

        /// <summary>
        /// ��ʼ��Ϊȱʡֵ
        /// </summary>
        public void Initialize()
        {
            this.m_szID = string.Empty;
            this.m_szName = string.Empty;
            this.m_szPwd = string.Empty;

            this.m_ePower = UserPower.Invisible;
            this.m_eRole = UserRole.Other;
            this.m_eLevel = UserLevel.Normal;

            this.m_nDeptCode = 0;
            this.m_szDeptCode = string.Empty;
            this.m_szDeptName = string.Empty;
        }

        /// <summary>
        /// �Ƚ�����UserInfo����
        /// </summary>
        /// <param name="clsUserInfo">�Ƚϵ�UserInfo����</param>
        /// <returns>bool</returns>
        public bool Equals(UserInfo clsUserInfo)
        {
            if (clsUserInfo == null)
                return false;

            return ((this.m_szID == clsUserInfo.ID)
                && (this.m_szName == clsUserInfo.Name)
                && (this.m_szDeptCode == clsUserInfo.DeptCode)
                && (this.m_szDeptName == clsUserInfo.DeptName)
                && (this.m_szPwd == clsUserInfo.Password)
                && (this.m_eRole == clsUserInfo.Role)
                && (this.m_eLevel == clsUserInfo.Level)
                && (this.m_ePower == clsUserInfo.Power));
        }

        /// <summary>
        /// ��ȡ�ĵ�����Ȩ������
        /// </summary>
        /// <returns></returns>
        public static string GetDocRight(UserInfo userInfo)
        {
            if (userInfo == null)
                return SystemData.UserType.OTHER;
            if (userInfo.Role == UserRole.InDoctor)
                return SystemData.UserType.IP_DOCTOR;
            else if (userInfo.Role == UserRole.OutDoctor)
                return SystemData.UserType.OP_DOCTOR;
            else if (userInfo.Role == UserRole.Nurse)
                return SystemData.UserType.NURSE;
            return SystemData.UserType.OTHER;
        }

        /// <summary>
        /// �����û���ɫ�����ȡ��ɫö��
        /// </summary>
        /// <param name="szRole">�û���ɫ����</param>
        /// <param name="eUserRole">���ص��û���ɫö��</param>
        /// <returns>bool</returns>
        public static bool GetRoleEnum(string szRole, ref UserRole eUserRole)
        {
            if (GlobalMethods.AppMisc.IsEmptyString(szRole))
                return false;
            try
            {
                short shRole = short.Parse(szRole);
                eUserRole = (UserRole)shRole;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// �����û��ȼ������ȡ�ȼ�ö��
        /// </summary>
        /// <param name="szLevel">�û��ȼ�����</param>
        /// <param name="eUserLevel">���ص��û��ȼ�ö��</param>
        /// <returns>bool</returns>
        public static bool GetLevelEnum(string szLevel, ref UserLevel eUserLevel)
        {
            if (GlobalMethods.AppMisc.IsEmptyString(szLevel))
                return false;
            try
            {
                short shLevel = short.Parse(szLevel);
                eUserLevel = (UserLevel)shLevel;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ����Ȩ�޴����ȡȨ��ö��
        /// </summary>
        /// <param name="shType">�û�Ȩ�޴���</param>
        /// <returns>bool</returns>
        public static bool GetPowerEnum(string szPower, ref UserPower eUserPower)
        {
            if (GlobalMethods.AppMisc.IsEmptyString(szPower))
                return false;
            try
            {
                short shPower = short.Parse(szPower);
                eUserPower = (UserPower)shPower;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��֤UserInfo���ݵĺϷ���(�׳��쳣��Ϣ)
        /// </summary>
        public static void Validate(UserInfo userInfo)
        {
            if (userInfo == null)
                throw new Exception("�û���Ϣ���ݲ���Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(userInfo.ID))
                throw new Exception("�û�ID��������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(userInfo.Name))
                throw new Exception("�û�������������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(userInfo.DeptCode))
                throw new Exception("�û��������Ҵ����������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(userInfo.DeptName))
                throw new Exception("�û������������Ʋ�������Ϊ��!");
        }

        /// <summary>
        /// ��ȡ���Ҵ����Ӧ�Ĺ�ϣֵ
        /// </summary>
        /// <returns>��ϣֵ</returns>
        public int GetHashDeptCode()
        {
            if (string.IsNullOrEmpty(this.m_szDeptCode))
                return 0;
            if (this.m_nDeptCode == 0)
                this.m_nDeptCode = Math.Abs(this.m_szDeptCode.GetHashCode());
            return this.m_nDeptCode;
        }

        /// <summary>
        /// ����д
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.m_szName;
        }

        /// <summary>
        /// ���û���Ϣ�������Ϊ�ַ�����ʽ
        /// </summary>
        /// <param name="userInfo">�û���Ϣ����</param>
        /// <param name="szSplitChar">�ָ���</param>
        /// <returns>�û���Ϣ�ַ���</returns>
        public static string GetStrFromUserInfo(UserInfo userInfo, string szSplitChar)
        {
            if (userInfo == null)
                userInfo = new UserInfo();

            StringBuilder sbUserInfo = new StringBuilder();
            sbUserInfo.Append(userInfo.ID);
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(userInfo.Name);
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(userInfo.Password);
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(userInfo.DeptCode);
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(userInfo.DeptName);
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(((int)userInfo.Role));
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(((int)userInfo.Level));
            sbUserInfo.Append(szSplitChar);
            sbUserInfo.Append(((int)userInfo.Power));
            sbUserInfo.Append(szSplitChar);

            return sbUserInfo.ToString();
        }

        /// <summary>
        /// ���ַ��������û���Ϣ����Ϊ�û���Ϣ����
        /// </summary>
        /// <param name="szUserInfoData">�ַ��������û���Ϣ</param>
        /// <param name="szSplitChar">�ָ���</param>
        /// <returns>UserInfo</returns>
        public static UserInfo GetUserInfoFromStr(string szUserInfoData, string szSplitChar)
        {
            if (GlobalMethods.AppMisc.IsEmptyString(szUserInfoData))
                throw new Exception("�û���Ϣ���ݲ���Ϊ��!");

            UserInfo userInfo = new UserInfo();

            string[] arrUserData = szUserInfoData.Split(new string[] { szSplitChar }, StringSplitOptions.None);

            if (arrUserData.Length > 0)
                userInfo.ID = arrUserData[0];

            if (arrUserData.Length > 1)
                userInfo.Name = arrUserData[1];

            if (arrUserData.Length > 2)
                userInfo.Password = arrUserData[2];

            if (arrUserData.Length > 3)
                userInfo.DeptCode = arrUserData[3];

            if (arrUserData.Length > 4)
                userInfo.DeptName = arrUserData[4];

            if (arrUserData.Length > 5)
            {
                UserRole eRole = UserRole.InDoctor;
                if (!UserInfo.GetRoleEnum(arrUserData[5], ref eRole))
                {
                    throw new Exception("�û���ɫ��������Ƿ�!");
                }
                userInfo.Role = eRole;
            }

            if (arrUserData.Length > 6)
            {
                UserLevel eLevel = UserLevel.Normal;
                if (!UserInfo.GetLevelEnum(arrUserData[6], ref eLevel))
                {
                    throw new Exception("�û��ȼ���������Ƿ�!");
                }
                userInfo.Level = eLevel;
            }

            if (arrUserData.Length > 7)
            {
                UserPower ePower = UserPower.Invisible;
                if (!UserInfo.GetPowerEnum(arrUserData[7], ref ePower))
                {
                    throw new Exception("�û�Ȩ�޴�������Ƿ�!");
                }
                userInfo.Power = ePower;
            }
            return userInfo;
        }
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class PatientInfo : ICloneable
    {
        private string m_szID = string.Empty; //���˺�
        private string m_szName = string.Empty;  //��������

        private string m_szGenderCode = string.Empty; //�Ա���
        private string m_szGender = string.Empty; //�Ա���ʾ��

        private DateTime m_dtBirthTime = DateTime.Now; //����ʱ��

        private string m_szMaritalCode = string.Empty; //����״����
        private string m_szMarital = string.Empty; //����״����ʾ��

        private string m_szBirthPlace = string.Empty; //������
        private string m_szFamilyAddr = string.Empty; //��ͥסַ
        private string m_szDepartment = string.Empty; //������λ(����)

        private string m_szOccupationCode = string.Empty; //ְҵ����
        private string m_szOccupation = string.Empty; //ְҵ

        private string m_szRaceCode = string.Empty; //�������
        private string m_szRaceName = string.Empty; //����

        private string m_szConfidCode = string.Empty; //�����Դ���

        /// <summary>
        /// ��ȡ�����ò���ID
        /// </summary>
        public string ID
        {
            get { return this.m_szID; }
            set { this.m_szID = value; }
        }
        /// <summary>
        /// ��ȡ�����ò�������
        /// </summary>
        public string Name
        {
            get { return this.m_szName; }
            set { this.m_szName = value; }
        }
        /// <summary>
        /// ��ȡ�������Ա���
        /// </summary>
        public string GenderCode
        {
            get { return this.m_szGenderCode; }
            set { this.m_szGenderCode = value; }
        }
        /// <summary>
        /// ��ȡ�������Ա���ʾ��
        /// </summary>
        public string Gender
        {
            get { return this.m_szGender; }
            set { this.m_szGender = value; }
        }
        /// <summary>
        /// ��ȡ�����ó���ʱ��
        /// </summary>
        public DateTime BirthTime
        {
            get { return this.m_dtBirthTime; }
            set { this.m_dtBirthTime = value; }
        }
        /// <summary>
        /// ��ȡ����������ֵ
        /// </summary>
        public int AgeValue
        {
            get
            {
                return (int)GlobalMethods.SysTime.DateDiff(DateInterval.Year, this.m_dtBirthTime, SysTimeHelper.Instance.Now);
            }
        }
        /// <summary>
        /// ��ȡ�����û���״����
        /// </summary>
        public string MaritalCode
        {
            get { return this.m_szMaritalCode; }
            set { this.m_szMaritalCode = value; }
        }
        /// <summary>
        /// ��ȡ�����û���״��
        /// </summary>
        public string Marital
        {
            get { return this.m_szMarital; }
            set { this.m_szMarital = value; }
        }
        /// <summary>
        /// ��ȡ�����ó�����
        /// </summary>
        public string BirthPlace
        {
            get { return this.m_szBirthPlace; }
            set { this.m_szBirthPlace = value; }
        }
        /// <summary>
        /// ��ȡ�����ü�ͥסַ
        /// </summary>
        public string FamilyAddr
        {
            get { return this.m_szFamilyAddr; }
            set { this.m_szFamilyAddr = value; }
        }
        /// <summary>
        /// ��ȡ�����ù�����λ
        /// </summary>
        public string Department
        {
            get { return this.m_szDepartment; }
            set { this.m_szDepartment = value; }
        }
        /// <summary>
        /// ��ȡ������ְҵ����
        /// </summary>
        public string OccupationCode
        {
            get { return this.m_szOccupationCode; }
            set { this.m_szOccupationCode = value; }
        }
        /// <summary>
        /// ��ȡ������ְҵ
        /// </summary>
        public string Occupation
        {
            get { return this.m_szOccupation; }
            set { this.m_szOccupation = value; }
        }
        /// <summary>
        /// ��ȡ������������
        /// </summary>
        public string RaceCode
        {
            get { return this.m_szRaceCode; }
            set { this.m_szRaceCode = value; }
        }
        /// <summary>
        /// ��ȡ����������
        /// </summary>
        public string RaceName
        {
            get { return this.m_szRaceName; }
            set { this.m_szRaceName = value; }
        }
        /// <summary>
        /// ��ȡ�����ò��˻����Դ���
        /// </summary>
        public string ConfidCode
        {
            get { return this.m_szConfidCode; }
            set { this.m_szConfidCode = value; }
        }

        public PatientInfo()
        {
        }

        public object Clone()
        {
            PatientInfo patientInfo = new PatientInfo();
            patientInfo.ID = this.m_szID;
            patientInfo.Name = this.m_szName;
            patientInfo.GenderCode = this.m_szGenderCode;
            patientInfo.Gender = this.m_szGender;
            patientInfo.BirthTime = this.m_dtBirthTime;
            patientInfo.MaritalCode = this.m_szMaritalCode;
            patientInfo.Marital = this.m_szMarital;
            patientInfo.BirthPlace = this.m_szBirthPlace;
            patientInfo.FamilyAddr = this.m_szFamilyAddr;
            patientInfo.Department = this.m_szDepartment;
            patientInfo.OccupationCode = this.m_szOccupationCode;
            patientInfo.Occupation = this.m_szOccupation;
            patientInfo.RaceCode = this.m_szRaceCode;
            patientInfo.RaceName = this.m_szRaceName;
            patientInfo.ConfidCode = this.m_szConfidCode;
            return patientInfo;
        }

        /// <summary>
        /// ��ʼ��Ϊȱʡֵ
        /// </summary>
        public void Initialize()
        {
            this.m_szID = string.Empty;
            this.m_szName = string.Empty;
            this.m_szGenderCode = string.Empty;
            this.m_dtBirthTime = DateTime.Now;
            this.m_szMaritalCode = string.Empty;
            this.m_szMarital = string.Empty;
            this.m_szBirthPlace = string.Empty;
            this.m_szFamilyAddr = string.Empty;
            this.m_szDepartment = string.Empty;
            this.m_szOccupation = string.Empty;
            this.m_szRaceCode = string.Empty;
            this.m_szRaceName = string.Empty;
            this.m_szConfidCode = string.Empty;
        }
        /// <summary>
        /// �Ƚ�����PatientInfo����
        /// </summary>
        /// <param name="clsPatientInfo">�Ƚϵ�PatientInfo����</param>
        /// <returns>bool</returns>
        public bool Equals(PatientInfo clsPatientInfo)
        {
            if (clsPatientInfo == null)
                return false;

            return ((this.m_szID == clsPatientInfo.ID)
                && (this.m_szName == clsPatientInfo.Name)
                && (this.m_szGenderCode == clsPatientInfo.GenderCode)
                && (DateTime.Compare(this.m_dtBirthTime, clsPatientInfo.BirthTime) == 0)
                && (this.m_szMaritalCode == clsPatientInfo.MaritalCode)
                && (this.m_szBirthPlace == clsPatientInfo.BirthPlace)
                && (this.m_szFamilyAddr == clsPatientInfo.FamilyAddr)
                && (this.m_szDepartment == clsPatientInfo.Department)
                && (this.m_szOccupation == clsPatientInfo.Occupation)
                && (this.m_szRaceCode == clsPatientInfo.RaceCode)
                && (this.m_szRaceName == clsPatientInfo.RaceName)
                && (this.m_szConfidCode == clsPatientInfo.ConfidCode));
        }
        /// <summary>
        /// ��֤PatientInfo���ݵĺϷ���(�׳��쳣��Ϣ)
        /// </summary>
        public static void Validate(PatientInfo patientInfo)
        {
            if (patientInfo == null)
                throw new Exception("������Ϣ��������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(patientInfo.ID))
                throw new Exception("����ID��������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(patientInfo.Name))
                throw new Exception("����������������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(patientInfo.Gender))
                throw new Exception("�����Ա��������Ϊ��!");
        }
        /// <summary>
        /// ����д
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.m_szName;
        }
        /// <summary>
        /// �Ѳ�����Ϣ�������Ϊһ���ַ���
        /// </summary>
        /// <param name="patientInfo">������Ϣ����</param>
        /// <param name="szSplitChar">�ָ���</param>
        /// <returns>string</returns>
        public static string GetStrFromPatientInfo(PatientInfo patientInfo, string szSplitChar)
        {
            if (patientInfo == null)
                patientInfo = new PatientInfo();

            StringBuilder sbPatientInfo = new StringBuilder();
            sbPatientInfo.Append(patientInfo.ID);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.Name);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.GenderCode);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.Gender);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.BirthTime);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.BirthPlace);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.FamilyAddr);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.MaritalCode);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.Marital);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.Department);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.OccupationCode);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.Occupation);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.RaceCode);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.RaceName);
            sbPatientInfo.Append(szSplitChar);
            sbPatientInfo.Append(patientInfo.ConfidCode);
            sbPatientInfo.Append(szSplitChar);

            return sbPatientInfo.ToString();
        }
        /// <summary>
        /// ���ַ������Ĳ�����Ϣ����ת���ɲ�����Ϣ����
        /// </summary>
        /// <param name="szPatientData">������Ϣ�ַ���</param>
        /// <param name="szSplitChar">�ָ���</param>
        /// <returns>PatientInfo</returns>
        public static PatientInfo GetPatientInfoFromStr(string szPatientData, string szSplitChar)
        {
            PatientInfo patientInfo = new PatientInfo();

            string[] arrPatientData = szPatientData.Split(new string[] { szSplitChar }, StringSplitOptions.None);

            if (arrPatientData.Length > 0)
                patientInfo.ID = arrPatientData[0];

            if (arrPatientData.Length > 1)
                patientInfo.Name = arrPatientData[1];

            if (arrPatientData.Length > 2)
                patientInfo.GenderCode = arrPatientData[2];

            if (arrPatientData.Length > 3)
                patientInfo.Gender = arrPatientData[3];

            if (arrPatientData.Length > 4)
            {
                try
                {
                    patientInfo.BirthTime = DateTime.Parse(arrPatientData[4]);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.WriteLog("PatientInfo.GetPatientInfoFromStr", new string[] { "szPatientData" }
                        , new object[] { szPatientData }, ex);
                    throw new Exception("���˳������ڲ����Ƿ�!");
                }
            }

            if (arrPatientData.Length > 5)
                patientInfo.BirthPlace = arrPatientData[5];

            if (arrPatientData.Length > 6)
                patientInfo.FamilyAddr = arrPatientData[6];

            if (arrPatientData.Length > 7)
                patientInfo.MaritalCode = arrPatientData[7];

            if (arrPatientData.Length > 8)
                patientInfo.Marital = arrPatientData[8];

            if (arrPatientData.Length > 9)
                patientInfo.Department = arrPatientData[9];

            if (arrPatientData.Length > 10)
                patientInfo.OccupationCode = arrPatientData[10];

            if (arrPatientData.Length > 11)
                patientInfo.Occupation = arrPatientData[11];

            if (arrPatientData.Length > 12)
                patientInfo.RaceCode = arrPatientData[12];

            if (arrPatientData.Length > 13)
                patientInfo.RaceName = arrPatientData[13];

            if (arrPatientData.Length > 14)
                patientInfo.ConfidCode = arrPatientData[14];

            return patientInfo;
        }
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class VisitInfo : ICloneable
    {
        private string m_szID = string.Empty;       //�����
        private string m_szInpID = string.Empty;     //סԺ��
        private VisitType m_eType = VisitType.OP;   //�������ͣ�0���� 1���� 2��Ժ
        private DateTime m_dtTime = DateTime.Now;   //����ʱ��
        private string m_szDeptCode = string.Empty; //���������
        private string m_szDeptName = string.Empty; //�����������
        private string m_szWardCode = string.Empty; //���ﲡ����
        private string m_szWardName = string.Empty; //���ﲡ������
        private string m_szCareCode = string.Empty; //���ﻤ��Ԫ��
        private string m_szCareName = string.Empty; //���ﻤ��Ԫ����
        private string m_szBedCode = string.Empty;  //��λ��
        /// <summary>
        /// ��ȡ�����þ����(����סԺ������ڼ�����Ժ;������������������)
        /// </summary>
        public string ID
        {
            get { return this.m_szID; }
            set { this.m_szID = value; }
        }
        /// <summary>
        /// ��ȡ������סԺ��
        /// </summary>
        public string InpID
        {
            get { return this.m_szInpID; }
            set { this.m_szInpID = value; }
        }
        /// <summary>
        /// ��ȡ�����þ�������
        /// </summary>
        public VisitType Type
        {
            get { return this.m_eType; }
            set { this.m_eType = value; }
        }
        /// <summary>
        /// ��ȡ�����þ���ʱ��
        /// </summary>
        public DateTime Time
        {
            get { return this.m_dtTime; }
            set { this.m_dtTime = value; }
        }
        /// <summary>
        /// ��ȡ�����þ��������
        /// </summary>
        public string DeptCode
        {
            get { return this.m_szDeptCode; }
            set { this.m_szDeptCode = value; }
        }
        /// <summary>
        /// ��ȡ�����þ����������
        /// </summary>
        public string DeptName
        {
            get { return this.m_szDeptName; }
            set { this.m_szDeptName = value; }
        }
        /// <summary>
        /// ��ȡ�����þ��ﲡ����
        /// </summary>
        public string WardCode
        {
            get { return this.m_szWardCode; }
            set { this.m_szWardCode = value; }
        }
        /// <summary>
        /// ��ȡ�����þ��ﲡ������
        /// </summary>
        public string WardName
        {
            get { return this.m_szWardName; }
            set { this.m_szWardName = value; }
        }
        /// <summary>
        /// ��ȡ�����ô�λ��������Ward���Ҫ
        /// </summary>
        public string BedCode
        {
            get { return this.m_szBedCode; }
            set { this.m_szBedCode = value; }
        }
        /// <summary>
        /// ��ȡ�����þ��ﻤ��Ԫ��
        /// </summary>
        public string CareCode
        {
            get { return this.m_szCareCode; }
            set { this.m_szCareCode = value; }
        }
        /// <summary>
        /// ��ȡ�����þ��ﻤ��Ԫ����
        /// </summary>
        public string CareName
        {
            get { return this.m_szCareName; }
            set { this.m_szCareName = value; }
        }

        public VisitInfo()
        {
            this.Initialize();
        }

        public object Clone()
        {
            VisitInfo visitInfo = new VisitInfo();
            visitInfo.ID = this.m_szID;
            visitInfo.InpID = this.m_szInpID;
            visitInfo.Type = this.m_eType;
            visitInfo.Time = this.m_dtTime;
            visitInfo.DeptCode = this.m_szDeptCode;
            visitInfo.DeptName = this.m_szDeptName;
            visitInfo.WardCode = this.m_szWardCode;
            visitInfo.WardName = this.m_szWardName;
            visitInfo.CareCode = this.m_szCareCode;
            visitInfo.CareName = this.m_szCareName;
            visitInfo.BedCode = this.m_szBedCode;
            return visitInfo;
        }

        /// <summary>
        /// ��ʼ��Ϊȱʡֵ
        /// </summary>
        public void Initialize()
        {
            this.m_szID = string.Empty;
            this.m_szInpID = string.Empty;
            this.m_eType = VisitType.OP;
            this.m_dtTime = DateTime.Now;
            this.m_szDeptCode = string.Empty;
            this.m_szDeptName = string.Empty;
            this.m_szWardCode = string.Empty;
            this.m_szWardName = string.Empty;
            this.m_szBedCode = string.Empty;
            this.m_szCareCode = string.Empty;
            this.m_szCareName = string.Empty;
        }
        /// <summary>
        /// ��֤VisitInfo���ݵĺϷ���(�׳��쳣��Ϣ)
        /// </summary>
        public static void Validate(VisitInfo visitInfo)
        {
            if (visitInfo == null)
                throw new Exception("���˾�����Ϣ���ݲ���Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(visitInfo.ID))
                throw new Exception("���˾���ID��������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(visitInfo.DeptCode))
                throw new Exception("���˾�����Ҵ����������Ϊ��!");
            if (GlobalMethods.AppMisc.IsEmptyString(visitInfo.DeptName))
                throw new Exception("���˾���������Ʋ�������Ϊ��!");
        }
        /// <summary>
        /// �Ƚ�����VisitInfo����
        /// </summary>
        /// <param name="clsVisitInfo">�Ƚϵ�VisitInfo����</param>
        /// <returns>bool</returns>
        public bool Equals(VisitInfo clsVisitInfo)
        {
            if (clsVisitInfo == null)
                return false;

            return ((this.m_szID == clsVisitInfo.ID)
                && (this.m_szInpID == clsVisitInfo.InpID)
                && (this.m_eType == clsVisitInfo.Type)
                && (DateTime.Compare(this.m_dtTime, clsVisitInfo.Time) == 0)
                && (this.m_szDeptCode == clsVisitInfo.DeptCode)
                && (this.m_szDeptName == clsVisitInfo.DeptName)
                && (this.m_szWardCode == clsVisitInfo.WardCode)
                && (this.m_szWardName == clsVisitInfo.WardName)
                && (this.m_szBedCode == clsVisitInfo.BedCode)
                && (this.m_szCareCode == clsVisitInfo.CareCode)
                && (this.m_szCareName == clsVisitInfo.CareName));
        }
        /// <summary>
        /// ���ݾ������ʹ����ȡ��������ö��
        /// </summary>
        /// <param name="shType">�������ʹ���</param>
        /// <returns>bool</returns>
        public static bool GetTypeEnum(string szType, ref VisitType eVisitType)
        {
            if (GlobalMethods.AppMisc.IsEmptyString(szType))
                return false;
            try
            {
                short shType = short.Parse(szType);
                eVisitType = (VisitType)shType;
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// ��ȡ�������Ͷ�Ӧ����д
        /// </summary>
        /// <returns></returns>
        public static string GetVisitTypeStr(VisitInfo visitInfo)
        {
            if (visitInfo == null)
                return SystemData.VisitType.IP;
            if (visitInfo.Type == VisitType.OP || visitInfo.Type == VisitType.EP)
                return SystemData.VisitType.OP;
            else
                return SystemData.VisitType.IP;
        }
        /// <summary>
        /// ����д
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.m_szID;
        }
        /// <summary>
        /// �Ѿ�����Ϣ����ת���ɾ�����Ϣ�ַ���
        /// </summary>
        /// <param name="visitInfo">������Ϣ����</param>
        /// <param name="szSplitChar">������Ϣ�ַ���</param>
        /// <returns>string</returns>
        public static string GetStrFromVisitInfo(VisitInfo visitInfo, string szSplitChar)
        {
            if (visitInfo == null)
                visitInfo = new VisitInfo();

            StringBuilder sbVisitInfo = new StringBuilder();
            sbVisitInfo.Append(visitInfo.ID);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.InpID);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(((int)visitInfo.Type).ToString());
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.Time);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.DeptCode);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.DeptName);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.WardCode);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.WardName);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.CareCode);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.CareName);
            sbVisitInfo.Append(szSplitChar);
            sbVisitInfo.Append(visitInfo.BedCode);
            sbVisitInfo.Append(szSplitChar);

            return sbVisitInfo.ToString();
        }
        /// <summary>
        /// ���ַ������ľ�����Ϣת��Ϊ������Ϣ����
        /// </summary>
        /// <param name="szVisitData">�ַ������ľ�����Ϣ</param>
        /// <param name="szSplitChar">�ָ���</param>
        /// <returns>������Ϣ����</returns>
        public static VisitInfo GetVisitInfoFromStr(string szVisitData, string szSplitChar)
        {
            VisitInfo visitInfo = new VisitInfo();

            string[] arrVisitData = szVisitData.Split(new string[] { szSplitChar }, StringSplitOptions.None);

            if (arrVisitData.Length > 0)
                visitInfo.ID = arrVisitData[0];

            if (arrVisitData.Length > 1)
                visitInfo.InpID = arrVisitData[1];

            if (arrVisitData.Length > 2)
            {
                VisitType eType = VisitType.OP;
                if (!VisitInfo.GetTypeEnum(arrVisitData[2], ref eType))
                {
                    throw new Exception("�������Ͳ����Ƿ�!");
                }
                visitInfo.Type = eType;
            }

            if (arrVisitData.Length > 3)
            {
                try
                {
                    visitInfo.Time = DateTime.Parse(arrVisitData[3]);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.WriteLog("VisitInfo.GetVisitInfoFromStr", new string[] { "szVisitData" }
                        , new object[] { szVisitData }, ex);
                    throw new Exception("����ʱ������Ƿ�!");
                }
            }

            if (arrVisitData.Length > 4)
                visitInfo.DeptCode = arrVisitData[4];

            if (arrVisitData.Length > 5)
                visitInfo.DeptName = arrVisitData[5];

            if (arrVisitData.Length > 6)
                visitInfo.WardCode = arrVisitData[6];

            if (arrVisitData.Length > 7)
                visitInfo.WardName = arrVisitData[7];

            if (arrVisitData.Length > 8)
                visitInfo.CareCode = arrVisitData[8];

            if (arrVisitData.Length > 9)
                visitInfo.CareName = arrVisitData[9];

            if (arrVisitData.Length > 10)
                visitInfo.BedCode = arrVisitData[10];

            return visitInfo;
        }
    }

    /// <summary>
    /// ҳ���ֽ����Ϣ��
    /// </summary>
    public class PageSettings : ICloneable
    {
        private int m_nWidth = 210;
        private int m_nHeight = 297;
        private bool m_bLandscape = false;
        private int m_nLeftMargin = 15;
        private int m_nRightMargin = 15;
        private int m_nTopMargin = 20;
        private int m_nBottomMargin = 20;
        private int m_nStartPageNo = 1;
        private int m_nOffsetTop = 0;
        private PageLayout m_ePageLayout = PageLayout.Normal;
        private string m_szFooterText = string.Empty;
        private bool m_bFooterLine = false;
        private PageFooterAlign m_eFooterAlign = PageFooterAlign.Middle;

        /// <summary>
        /// ��ȡ������ֽ�ſ��
        /// </summary>
        public int Width
        {
            get { return this.m_nWidth; }
            set { this.m_nWidth = value; }
        }

        /// <summary>
        /// ��ȡ������ֽ�ſ��
        /// </summary>
        public int Height
        {
            get { return this.m_nHeight; }
            set { this.m_nHeight = value; }
        }

        /// <summary>
        /// ��ȡ������ֽ���Ƿ��Ǻ����
        /// </summary>
        public bool Landscape
        {
            get { return this.m_bLandscape; }
            set { this.m_bLandscape = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ����߾�
        /// </summary>
        public int LeftMargin
        {
            get { return this.m_nLeftMargin; }
            set { this.m_nLeftMargin = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ���ұ߾�
        /// </summary>
        public int RightMargin
        {
            get { return this.m_nRightMargin; }
            set { this.m_nRightMargin = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ���ϱ߾�
        /// </summary>
        public int TopMargin
        {
            get { return this.m_nTopMargin; }
            set { this.m_nTopMargin = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ���±߾�
        /// </summary>
        public int BottomMargin
        {
            get { return this.m_nBottomMargin; }
            set { this.m_nBottomMargin = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ�沼��
        /// </summary>
        public PageLayout PageLayout
        {
            get { return this.m_ePageLayout; }
            set { this.m_ePageLayout = value; }
        }

        /// <summary>
        /// ��ȡ�������ĵ���ʼҳ��
        /// </summary>
        public int StartPageNo
        {
            get { return this.m_nStartPageNo; }
            set { this.m_nStartPageNo = value; }
        }

        /// <summary>
        /// ��ȡ�����ö��˴�ӡƫ����
        /// </summary>
        public int OffsetTop
        {
            get { return this.m_nOffsetTop; }
            set { this.m_nOffsetTop = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ�����Ƿ���ʾһ������
        /// </summary>
        public bool FooterLine
        {
            get { return this.m_bFooterLine; }
            set { this.m_bFooterLine = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ����ʾ���ı�
        /// </summary>
        public string FooterText
        {
            get { return this.m_szFooterText; }
            set { this.m_szFooterText = value; }
        }

        /// <summary>
        /// ��ȡ������ҳ���ı��Ķ�����ʽ
        /// </summary>
        public PageFooterAlign FooterAlign
        {
            get { return this.m_eFooterAlign; }
            set { this.m_eFooterAlign = value; }
        }

        public void Initialize()
        {
            this.m_nWidth = 210;
            this.m_nHeight = 297;
            this.m_bLandscape = false;
            this.m_nLeftMargin = 25;
            this.m_nRightMargin = 10;
            this.m_nTopMargin = 16;
            this.m_nBottomMargin = 16;
            this.m_nStartPageNo = 1;
            this.m_nOffsetTop = 0;
            this.m_ePageLayout = PageLayout.Normal;
            this.m_eFooterAlign = PageFooterAlign.Middle;
            this.m_bFooterLine = false;
            this.m_szFooterText = string.Empty;
        }

        public object Clone()
        {
            PageSettings pageSettings = new PageSettings();
            pageSettings.Width = this.m_nWidth;
            pageSettings.Height = this.m_nHeight;
            pageSettings.Landscape = this.m_bLandscape;
            pageSettings.PageLayout = this.m_ePageLayout;
            pageSettings.LeftMargin = this.m_nLeftMargin;
            pageSettings.RightMargin = this.m_nRightMargin;
            pageSettings.TopMargin = this.m_nTopMargin;
            pageSettings.BottomMargin = this.m_nBottomMargin;
            pageSettings.OffsetTop = this.m_nOffsetTop;
            pageSettings.StartPageNo = this.m_nStartPageNo;
            pageSettings.FooterAlign = this.m_eFooterAlign;
            pageSettings.FooterLine = this.m_bFooterLine;
            pageSettings.FooterText = this.m_szFooterText;
            return pageSettings;
        }
    }

    /// <summary>
    /// ֽ����Ϣ��
    /// </summary>
    public class PaperInfo : ICloneable
    {
        private string m_szName = string.Empty;
        private float m_fWidth = 0f;
        private float m_fHeight = 0f;
        private string m_szPrinter = string.Empty;
        /// <summary>
        /// ��ȡ������ֽ������
        /// </summary>
        public string Name
        {
            get { return this.m_szName; }
            set { this.m_szName = value; }
        }
        /// <summary>
        /// ��ȡ������ֽ�ſ�
        /// </summary>
        public float Width
        {
            get { return this.m_fWidth; }
            set { this.m_fWidth = value; }
        }
        /// <summary>
        /// ��ȡ������ֽ�Ÿ�
        /// </summary>
        public float Height
        {
            get { return this.m_fHeight; }
            set { this.m_fHeight = value; }
        }
        /// <summary>
        /// ��ȡ�����ø�ֽ�Ŷ�Ӧ�Ĵ�ӡ��
        /// </summary>
        public string Printer
        {
            get { return this.m_szPrinter; }
            set { this.m_szPrinter = value; }
        }

        public PaperInfo()
        {
        }
        /// <summary>
        /// ʵ����ֽ����Ϣ
        /// </summary>
        /// <param name="name">ֽ������</param>
        /// <param name="width">ֽ�ſ�</param>
        /// <param name="height">ֽ�Ÿ�</param>
        public PaperInfo(string name, float width, float height)
        {
            this.m_szName = name;
            this.m_fWidth = width;
            this.m_fHeight = height;
        }
        /// <summary>
        /// ʵ����ֽ����Ϣ
        /// </summary>
        /// <param name="name">ֽ������</param>
        /// <param name="width">ֽ�ſ�</param>
        /// <param name="height">ֽ�Ÿ�</param>
        /// <param name="printer">ֽ�Ŷ�Ӧ�Ĵ�ӡ��</param>
        public PaperInfo(string name, float width, float height, string printer)
            : this (name, width, height)
        {
            this.m_szPrinter = printer;
        }
        /// <summary>
        /// ����ֽ������
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.m_szName;
        }

        public object Clone()
        {
            return new PaperInfo(this.m_szName, this.m_fWidth, this.m_fHeight, this.m_szPrinter);
        }
    }

    /// <summary>
    /// �ĵ�����Ŀ��Ϣ
    /// </summary>
    public class DocumentSectionInfo : ICloneable
    {
        private string m_szID = null;
        /// <summary>
        /// ��ȡ�������ĵ�����ĿID
        /// </summary>
        public string ID
        {
            get { return this.m_szID; }
            set { this.m_szID = value; }
        }

        private string m_szName = null;
        /// <summary>
        /// ��ȡ�������ĵ�����Ŀ����
        /// </summary>
        public string Name
        {
            get { return this.m_szName; }
            set { this.m_szName = value; }
        }

        private string m_szText = null;
        /// <summary>
        /// ��ȡ�������ĵ�����Ŀ���û�������ı�
        /// </summary>
        public string Text
        {
            get { return this.m_szText; }
            set { this.m_szText = value; }
        }

        public DocumentSectionInfo()
        {
        }

        public DocumentSectionInfo(string szSectionID, string szSectionName)
        {
            this.m_szID = szSectionID;
            this.m_szName = szSectionName;
        }

        public object Clone()
        {
            DocumentSectionInfo sectionInfo = new DocumentSectionInfo();
            sectionInfo.ID = this.m_szID;
            sectionInfo.Name = this.m_szName;
            sectionInfo.Text = this.m_szText;
            return sectionInfo;
        }
    }
    #endregion
}
