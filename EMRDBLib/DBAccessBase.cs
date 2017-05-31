// ***********************************************************
// ���ݿ���ʲ����ݷ��ʻ���,�����ฺ���ṩһЩ���������뷽��.
// ���ڷ�ֹ�ظ�ʵ����,���Ч��
// Creator:YangMingkun  Date:2010-11-18
// Copyright:supconhealth
// ***********************************************************
using System;
using Heren.Common.Libraries.DbAccess;
using Heren.Common.Libraries.Ftp;
using Heren.Common.Libraries;

namespace EMRDBLib.DbAccess
{
    public abstract class DBAccessBase
    {
        private DataAccess m_DbAccess = null;
        private DataAccess m_QCAccess = null;
        private DataAccess m_NursringDbAccess = null;
        private DataAccess m_HisLoginAccess = null;
        public DBAccessBase()
        {

        }

        private bool m_bAutoClearPool = true;

        /// <summary>
        /// ��ȡ�����������ݿ���ʶ���ʵ��
        /// </summary>
        protected DataAccess DataAccess
        {
            get
            {
                if (this.m_DbAccess == null)
                    this.m_DbAccess = this.GetDbAccess();
                return this.m_DbAccess;
            }
        }

        /// <summary>
        /// ��ȡ���ݿ���ʶ���
        /// </summary>
        /// <returns></returns>
        public DataAccess GetDbAccess()
        {
            if (this.m_DbAccess != null)
                return this.m_DbAccess;
            //��ȡ�����ļ������ݿ�����
            string szDBType = SystemConfig.Instance.Get(SystemData.ConfigKey.MDS_DB_TYPE, string.Empty);
            string szDBDriverType = SystemConfig.Instance.Get(SystemData.ConfigKey.MDS_PROVIDER_TYPE, string.Empty);
            string szConnectionString = SystemConfig.Instance.Get(SystemData.ConfigKey.MDS_CONN_STRING, string.Empty);
            szDBType = GlobalMethods.Security.DecryptText(szDBType, SystemData.ConfigKey.CONFIG_ENCRYPT_KEY);
            szDBDriverType = GlobalMethods.Security.DecryptText(szDBDriverType, SystemData.ConfigKey.CONFIG_ENCRYPT_KEY);
            szConnectionString = GlobalMethods.Security.DecryptText(szConnectionString,
                SystemData.ConfigKey.CONFIG_ENCRYPT_KEY);
            if (GlobalMethods.Misc.IsEmptyString(szDBType) || GlobalMethods.Misc.IsEmptyString(szDBDriverType)
                || GlobalMethods.Misc.IsEmptyString(szConnectionString))
            {
                LogManager.Instance.WriteLog("SystemParam.GetDbAccess", new string[] { "ConfigFile" }
                    , new object[] { SystemConfig.Instance.ConfigFile }, "���ݿ����ò����а����Ƿ���ֵ!");
                return null;
            }
            this.m_DbAccess = new DataAccess();
            this.m_DbAccess.ConnectionString = szConnectionString;
            this.m_DbAccess.ClearPoolEnabled = this.m_bAutoClearPool;
            this.m_DbAccess.DatabaseType = this.GetDatabaseType(szDBType);
            this.m_DbAccess.DataProvider = this.GetDataProvider(szDBDriverType);
            return this.m_DbAccess;
        }
        private StorageMode m_eStorageMode = StorageMode.Unknown;
        /// <summary>
        /// ��ȡ�ĵ��洢ģʽ
        /// </summary>
        /// <returns>StorageModeö��</returns>
        public StorageMode GetStorageMode()
        {
            if (this.m_eStorageMode != StorageMode.Unknown)
                return this.m_eStorageMode;

            if (this.m_ConfigAccess == null)
                this.m_ConfigAccess = new ConfigAccess();
            this.m_eStorageMode = this.m_ConfigAccess.GetStorageMode();
            return this.m_eStorageMode;
        }
        /// <summary>
        /// ��ȡ���ݿ���ʶ���
        /// </summary>
        /// <returns></returns>
        public DataAccess GetQCAccess()
        {
            if (this.m_QCAccess != null)
                return this.m_QCAccess;

            //��ȡ�����ļ������ݿ�����
            string szDBType = SystemConfig.Instance.Get(SystemData.ConfigKey.QC_DB_TYPE, string.Empty);
            string szDBDriverType = SystemConfig.Instance.Get(SystemData.ConfigKey.QC_PROVIDER_TYPE, string.Empty);
            string szConnectionString = SystemConfig.Instance.Get(SystemData.ConfigKey.QC_CONN_STRING, string.Empty);
            szDBType = GlobalMethods.Security.DecryptText(szDBType, SystemData.ConfigKey.CONFIG_ENCRYPT_KEY);
            szDBDriverType = GlobalMethods.Security.DecryptText(szDBDriverType, SystemData.ConfigKey.CONFIG_ENCRYPT_KEY);
            szConnectionString = GlobalMethods.Security.DecryptText(szConnectionString, SystemData.ConfigKey.CONFIG_ENCRYPT_KEY);

            if (GlobalMethods.Misc.IsEmptyString(szDBType) || GlobalMethods.Misc.IsEmptyString(szDBDriverType)
                || GlobalMethods.Misc.IsEmptyString(szConnectionString))
            {
                LogManager.Instance.WriteLog("SystemParam.GetNewQCDbAccess", new string[] { "ConfigFile" }
                    , new object[] { SystemConfig.Instance.ConfigFile }, "���ݿ����ò����а����Ƿ���ֵ!");
                return null;
            }
            this.m_QCAccess = new DataAccess();
            this.m_QCAccess.ConnectionString = szConnectionString;
            this.m_QCAccess.ClearPoolEnabled = this.m_bAutoClearPool;
            this.m_QCAccess.DatabaseType = this.GetDatabaseType(szDBType);
            this.m_QCAccess.DataProvider = this.GetDataProvider(szDBDriverType);
            return this.m_QCAccess;
        }
        /// <summary>
        /// �����ݿ������ַ���ת��Ϊ���ݿ�����ö��
        /// </summary>
        /// <param name="szDbType">���ݿ������ַ���</param>
        /// <returns>���ݿ�����ö��</returns>
        private DatabaseType GetDatabaseType(string szDbType)
        {
            if (szDbType == SystemData.DatabaseType.SQLSERVER)
            {
                return DatabaseType.SQLSERVER;
            }
            else if (szDbType == SystemData.DatabaseType.ORACLE)
            {
                return DatabaseType.ORACLE;
            }
            else
            {
                LogManager.Instance.WriteLog("SystemParam.GetDatabaseType", "��֧�ֵ����ݿ�����!");
                return DatabaseType.ORACLE;
            }
        }

        /// �����ݿ�����ṩ���������ַ���ת��Ϊ����ö��
        /// </summary>
        /// <param name="szDbDriverType">�ṩ���������ַ���</param>
        /// <returns>�ṩ��������ö��</returns>
        private DataProvider GetDataProvider(string szDbDriverType)
        {
            if (szDbDriverType == SystemData.DataProvider.ODBC)
            {
                return DataProvider.Odbc;
            }
            else if (szDbDriverType == SystemData.DataProvider.ODPNET)
            {
                return DataProvider.ODPNET;
            }
            else if (szDbDriverType == SystemData.DataProvider.ORACLE)
            {
                return DataProvider.OracleClient;
            }
            else if (szDbDriverType == SystemData.DataProvider.SQLCLIENT)
            {
                return DataProvider.SqlClient;
            }
            else if (szDbDriverType == SystemData.DataProvider.OLEDB)
            {
                return DataProvider.OleDb;
            }
            else
            {
                LogManager.Instance.WriteLog("SystemParam.GetDataProvider", "��֧�ֵ����ݿ������ṩ����!");
                return DataProvider.OleDb;
            }
        }

        /// <summary>
        /// ��ȡ�ʿ����ݿ���ʶ���ʵ��
        /// </summary>
        protected DataAccess QCAccess
        {
            get
            {
                if (this.m_QCAccess == null)
                    this.m_QCAccess = this.GetQCAccess();
                return this.m_QCAccess;
            }
        }

        public string ParaHolder(string szParaName)
        {
            switch (this.DataAccess.DataProvider)
            {
                case DataProvider.ODPNET:
                case DataProvider.Odbc:
                case DataProvider.OleDb:
                    return "?";
                case DataProvider.OracleClient:
                    return ":" + szParaName;
                case DataProvider.SqlClient:
                    return "@" + szParaName;
                default:
                    return "?";
            }
        }

        private StorageMode m_StorageMode = StorageMode.Unknown;
        /// <summary>
        /// ��ȡ������ģ��Ĵ洢ģʽ
        /// </summary>
        protected StorageMode StorageMode
        {
            get
            {
                if (this.m_StorageMode == StorageMode.Unknown)
                    this.m_StorageMode = this.GetStorageMode();
                return this.m_StorageMode;
            }
        }

        private FtpAccess m_FtpAccess = null;
        /// <summary>
        /// ��ȡFTP���������ʶ���ʵ��
        /// </summary>
        protected FtpAccess FtpAccess
        {
            get
            {
                if (this.m_FtpAccess == null)
                    this.m_FtpAccess = this.GetDocFtpAccess();
                return this.m_FtpAccess;
            }
        }

        private FtpAccess m_DocFtpAccess = null;
        private ConfigAccess m_ConfigAccess = null;
        /// <summary>
        /// ��ȡFtp������
        /// </summary>
        /// <returns>FtpAccess����</returns>
        public FtpAccess GetDocFtpAccess()
        {
            if (this.m_DocFtpAccess != null)
                return this.m_DocFtpAccess;

            if (this.m_ConfigAccess == null)
                this.m_ConfigAccess = new ConfigAccess();
            FtpConfig ftpConfig = null;
            short shRet = this.m_ConfigAccess.GetDocFtpParams(ref ftpConfig);
            if (shRet != SystemData.ReturnValue.OK)
                return null;
            if (ftpConfig == null)
                return null;
            this.m_DocFtpAccess = new FtpAccess();
            this.m_DocFtpAccess.FtpIP = ftpConfig.FtpIP;
            this.m_DocFtpAccess.FtpPort = ftpConfig.FtpPort;
            this.m_DocFtpAccess.UserName = ftpConfig.FtpUser;
            this.m_DocFtpAccess.Password = ftpConfig.FtpPwd;
            this.m_DocFtpAccess.FtpMode = ftpConfig.FtpMode;
            return this.m_DocFtpAccess;
        }


        private FtpAccess m_RecPaperFtpAccess = null;
        /// <summary>
        /// ֽ�ʲ�������ͼƬ��ŵ�ַ
        /// </summary>
        /// <returns></returns>
        public FtpAccess GetRecPaperFtpAccess()
        {
            if (this.m_RecPaperFtpAccess != null)
                return this.m_RecPaperFtpAccess;

            if (this.m_ConfigAccess == null)
                this.m_ConfigAccess = new ConfigAccess();
            FtpConfig ftpConfig = null;
            short shRet = this.m_ConfigAccess.GetRecPaperFtpParams(ref ftpConfig);
            if (shRet != SystemData.ReturnValue.OK)
                return null;
            if (ftpConfig == null)
                return null;
            this.m_RecPaperFtpAccess = new FtpAccess();
            this.m_RecPaperFtpAccess.FtpIP = ftpConfig.FtpIP;
            this.m_RecPaperFtpAccess.FtpPort = ftpConfig.FtpPort;
            this.m_RecPaperFtpAccess.UserName = ftpConfig.FtpUser;
            this.m_RecPaperFtpAccess.Password = ftpConfig.FtpPwd;
            this.m_RecPaperFtpAccess.FtpMode = ftpConfig.FtpMode;
            return this.m_RecPaperFtpAccess;
        }
        /// <summary>
        /// ��ȡ�ʿؿ�ҽ���Ƿ���Ա��没��
        /// </summary>
        /// <returns></returns>
        public bool CheckQCSaveDocEnable()
        {
            if (this.m_ConfigAccess == null)
                this.m_ConfigAccess = new ConfigAccess();
            return this.m_ConfigAccess.CheckQCCanSaveDoc();
        }
        private FtpAccess m_XDBFtpAccess = null;
        /// <summary>
        /// ��ȡXDBFtp xml�ķ��ʶ���
        /// </summary>
        protected FtpAccess XDBFtpAccess
        {
            get
            {

                if (this.m_XDBFtpAccess == null)
                    this.m_XDBFtpAccess = this.GetXDBFtpAccess();
                return this.m_XDBFtpAccess;
            }
           
        }

        /// <summary>
        /// ��ȡXDBFtp������
        /// </summary>
        /// <returns>FtpAccess����</returns>
        public FtpAccess GetXDBFtpAccess()
        {
            if (this.m_XDBFtpAccess != null)
                return this.m_XDBFtpAccess;

            if (this.m_ConfigAccess == null)
                this.m_ConfigAccess = new ConfigAccess();
            FtpConfig ftpConfig = null;
            short shRet = this.m_ConfigAccess.GetXDBFtpParams(ref ftpConfig);
            if (shRet != SystemData.ReturnValue.OK)
                return null;
            if (ftpConfig == null)
                return null;
            this.m_XDBFtpAccess = new FtpAccess();
            this.m_XDBFtpAccess.FtpIP = ftpConfig.FtpIP;
            this.m_XDBFtpAccess.FtpPort = ftpConfig.FtpPort;
            this.m_XDBFtpAccess.UserName = ftpConfig.FtpUser;
            this.m_XDBFtpAccess.Password = ftpConfig.FtpPwd;
            this.m_XDBFtpAccess.FtpMode = ftpConfig.FtpMode;
            return this.m_XDBFtpAccess;
        }
        private bool m_bQCSaveDocEnable = false;
        protected bool QCSaveDocEnable
        {
            get
            {
                this.m_bQCSaveDocEnable =this.CheckQCSaveDocEnable();
                return this.m_bQCSaveDocEnable;
            }
        }
    }
}