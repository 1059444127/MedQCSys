// ***********************************************************
// �����ĵ�ϵͳ���ݷ��ʲ�ӿڷ�װ��.
// Creator:YangMingkun  Date:2009-6-22
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using MedDocSys.Common;

namespace MedDocSys.DataLayer
{
    public class DataAccess
    {
        private static MDSDBLib.DbAccess.CommonAccess m_CommonAccess = null;
        private static MDSDBLib.DbAccess.ConfigAccess m_ConfigAccess = null;
        private static MDSDBLib.DbAccess.DocTypeAccess m_DocTypeAccess = null;
        private static MDSDBLib.DbAccess.MedDocAccess m_MedDocAccess = null;
        private static MDSDBLib.DbAccess.EMRDBAccess m_EMRDBAccess = null;
        private static MDSDBLib.DbAccess.MedQCAccess m_MedQCAccess = null;
        private static MDSDBLib.DbAccess.TempletAccess m_TempletAccess = null;

        protected static MDSDBLib.DbAccess.ConfigAccess ConfigAccess
        {
            get
            {
                if (m_ConfigAccess == null)
                    m_ConfigAccess = new MDSDBLib.DbAccess.ConfigAccess();
                return m_ConfigAccess;
            }
        }

        protected static MDSDBLib.DbAccess.CommonAccess CommonAccess
        {
            get
            {
                if (m_CommonAccess == null)
                    m_CommonAccess = new MDSDBLib.DbAccess.CommonAccess();
                return m_CommonAccess;
            }
        }

        protected static MDSDBLib.DbAccess.DocTypeAccess DocTypeAccess
        {
            get
            {
                if (m_DocTypeAccess == null)
                    m_DocTypeAccess = new MDSDBLib.DbAccess.DocTypeAccess();
                return m_DocTypeAccess;
            }
        }

        protected static MDSDBLib.DbAccess.MedDocAccess MedDocAccess
        {
            get
            {
                if (m_MedDocAccess == null)
                    m_MedDocAccess = new MDSDBLib.DbAccess.MedDocAccess();
                return m_MedDocAccess;
            }
        }

        protected static MDSDBLib.DbAccess.EMRDBAccess EMRDBAccess
        {
            get
            {
                if (m_EMRDBAccess == null)
                    m_EMRDBAccess = new MDSDBLib.DbAccess.EMRDBAccess();
                return m_EMRDBAccess;
            }
        }

        protected static MDSDBLib.DbAccess.MedQCAccess MedQCAccess
        {
            get
            {
                if (m_MedQCAccess == null)
                    m_MedQCAccess = new MDSDBLib.DbAccess.MedQCAccess();
                return m_MedQCAccess;
            }
        }

        protected static MDSDBLib.DbAccess.TempletAccess TempletAccess
        {
            get
            {
                if (m_TempletAccess == null)
                    m_TempletAccess = new MDSDBLib.DbAccess.TempletAccess();
                return m_TempletAccess;
            }
        }

        /// <summary>
        /// ��ȡMDSDBAccess�쳣��������Ӧ�Ĵ�����Ϣ
        /// </summary>
        /// <param name="errorCode">�������</param>
        /// <returns>������Ϣ</returns>
        protected static string GetDBAccessError(short errorCode)
        {
            string szErrorInfo = "����δ֪�쳣����!";
            switch (errorCode)
            {
                case 1:
                    { szErrorInfo = "�����������!"; break; }
                case 2:
                    { szErrorInfo = "���ݿ�����쳣!"; break; }
                case 3:
                    { szErrorInfo = "�ӿ��ڲ��쳣!"; break; }
                case 4:
                    { szErrorInfo = "��Դδ����!"; break; }
                case 5:
                    { szErrorInfo = "��Դ�Ѿ�����!"; break; }
                case 6:
                    { szErrorInfo = "�����¼ʧ��!"; break; }
            }
            return szErrorInfo;
        }

        private static string m_OcxConnectionString = string.Empty;
        /// <summary>
        /// ��ȡ�������ݿ����Ӵ�
        /// </summary>
        public static string OcxConnectionString
        {
            get
            {
                if (DataAccess.ConfigAccess == null)
                    return string.Empty;
                if (GlobalMethods.AppMisc.IsEmptyString(m_OcxConnectionString))
                    m_OcxConnectionString = ConfigAccess.GetBaodianConnectionString();
                return m_OcxConnectionString;
            }
        }

        /// <summary>
        /// ��ȡ���ݿ������ʱ��
        /// </summary>
        /// <param name="dtSysDate">���ݿ������ʱ��</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetServerTime(ref DateTime dtSysDate)
        {
            dtSysDate = DateTime.Now;

            if (DataAccess.CommonAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = CommonAccess.GetServerTime(ref dtSysDate);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetServerTime", DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡOCX�ؼ���֤���������ʲ���
        /// </summary>
        /// <param name="szIP">��֤������IP</param>
        /// <param name="nPort">��֤����˿�</param>
        /// <param name="nMode">��֤ģʽ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetBaodianConfig(ref string szIP, ref int nPort, ref int nMode)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = ConfigAccess.GetBaodianConfig(ref szIP, ref nPort, ref nMode);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetOcxAuthenSrv", DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����ĵ�������Ϣ(����ҽ����סԺҽ������ʿ��������)����ȡ��Ӧ���������ĵ�������Ϣ�б�
        /// </summary>
        /// <param name="szDocRight">�ĵ�����Ȩ�����</param>
        /// <param name="szApplyEvn">Ӧ�û���</param>
        /// <param name="lstDocTypeInfos">�ĵ�������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetHostDocTypeInfos(string szDocRight, string szApplyEvn, ref List<MDSDBLib.DocTypeInfo> lstDocTypeInfos)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.GetHostDocTypeInfos(szDocRight, szApplyEvn, ref lstDocTypeInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetHostDocTypeInfos", new string[] { "szDocRight", "szApplyEvn" }
                    , new object[] { szDocRight, szApplyEvn }, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �������ĵ�����Code����ȡ���ĵ�������Ϣ�б�
        /// </summary>
        /// <param name="szTypeCode">���ĵ����ʹ���</param>
        /// <param name="lstDocTypeInfos">�ĵ�������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetSubDocTypeInfos(string szTypeCode, ref List<MDSDBLib.DocTypeInfo> lstDocTypeInfos)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.GetSubDocTypeInfos(szTypeCode, ref lstDocTypeInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetSubDocTypeInfos", new string[] { "szTypeCode" }
                    , new object[] { szTypeCode }, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ĵ����ʹ����ȡ�ĵ�������Ϣ
        /// </summary>
        /// <param name="szDocTypeID">�ĵ����ʹ���</param>
        /// <param name="docTypeInfo">�ĵ�������Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocTypeInfo(string szDocTypeID, ref MDSDBLib.DocTypeInfo docTypeInfo)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.GetDocTypeInfo(szDocTypeID, ref docTypeInfo);
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocTypeInfo", new string[] { "szDocTypeID" }
                    , new object[] { szDocTypeID }, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ���õ��ĵ�������Ϣ�б�
        /// </summary>
        /// <param name="lstDocTypeInfos">�ĵ�������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocTypeInfos(ref List<MDSDBLib.DocTypeInfo> lstDocTypeInfos)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.GetDocTypeInfos(ref lstDocTypeInfos);
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocTypeInfos", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����ĵ�����Code���û�ID����ȡ�û��Զ�����ĵ�ģ����Ϣ�б�
        /// </summary>
        /// <param name="szDeptCode">���Ҵ���,Ϊ��ʱ�������п���</param>
        /// <param name="szTypeCode">�ĵ����ʹ���,Ϊ��ʱ�����������͵�ģ��</param>
        /// <param name="szUserID">�û�ID,Ϊ��ʱ���������û���ģ��</param>
        /// <param name="lstTempletInfos">�ĵ�ģ����Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetUserTempletInfos(string szDeptCode, string szTypeCode, string szUserID, ref List<MDSDBLib.TempletInfo> lstTempletInfos)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            if (lstTempletInfos == null)
                lstTempletInfos = new List<MDSDBLib.TempletInfo>();

            short shRet = TempletAccess.GetUserTempletInfos(szDeptCode, szTypeCode, szUserID, ref lstTempletInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserTempletInfos", new string[] { "szDeptCode", "szTypeCode", "szUserID" }
                    , new object[] { szDeptCode, szTypeCode, szUserID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����ĵ�ģ��ID��ȡ�ĵ�ģ����Ϣ
        /// </summary>
        /// <param name="szTempletID">�ĵ�ģ��ID</param>
        /// <param name="templetInfo">�ĵ�ģ����Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetUserTempletInfo(string szTempletID, ref MDSDBLib.TempletInfo templetInfo)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetUserTempletInfo(szTempletID, ref templetInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserTempletInfo", new string[] { "szTempletID" }
                    , new object[] { szTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ�����ָ�����ˣ�ָ�������������ĵ���ϸ��Ϣ�б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">������</param>
        /// <param name="szVisitType">��������</param>
        /// <param name="dtVisitTime">����ʱ��</param>
        /// <param name="szDocTypeID">�ĵ����ʹ���</param>
        /// <param name="lstDocInfos">�ĵ���Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocInfos(string szPatientID, string szVisitID, string szVisitType, DateTime dtVisitTime, string szDocTypeID, ref List<MDSDBLib.MedDocInfo> lstDocInfos)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetDocInfos(szPatientID, szVisitID, szVisitType, dtVisitTime, szDocTypeID, ref lstDocInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocInfos", new string[] { "szPatientID", "szVisitID", "szVisitType", "dtVisitTime", "szDocTypeID" }
                    , new object[] { szPatientID, szVisitID, szVisitType, dtVisitTime, szDocTypeID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����ĵ�ID����ȡ�ĵ�������Ϣ
        /// </summary>
        /// <param name="szDocID">�ĵ����</param>
        /// <param name="clsDocInfo">�ĵ���Ϣ��</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocInfo(string szDocID, ref MDSDBLib.MedDocInfo clsDocInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetDocInfo(szDocID, ref clsDocInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocInfo", new string[] { "szDocID" }
                    , new object[] { szDocID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�ĵ�ժҪ��Ϣ
        /// </summary>
        /// <param name="szDocID">�ĵ����</param>
        /// <param name="szSummary">�ĵ�ժҪ��Ϣ��</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocSummary(string szDocID, ref string szSummary)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetDocSummary(szDocID, ref szSummary);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocSummary", new string[] { "szDocID" }
                    , new object[] { szDocID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ�����ָ�����ˣ�ָ�������������ĵ���ϸ��Ϣ�б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">������</param>
        /// <param name="szVisitType">��������</param>
        /// <param name="dtVisitTime">����ʱ��</param>
        /// <param name="szDocTypeID">�ĵ����ʹ���</param>
        /// <param name="lstDocInfos">�ĵ���Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetActiveDocInfo(string szPatientID, string szVisitID, string szVisitType, DateTime dtVisitTime, string szDocTypeID, ref MDSDBLib.MedDocInfo docInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetActiveDocInfo(szPatientID, szVisitID, szVisitType, dtVisitTime, szDocTypeID, ref docInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetActiveDocInfo", new string[] { "szPatientID", "szVisitID", "szVisitType", "dtVisitTime", "szDocTypeID" }
                    , new object[] { szPatientID, szVisitID, szVisitType, dtVisitTime, szDocTypeID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ���ĵ��������µ��ĵ��汾��Ϣ
        /// </summary>
        /// <param name="szDocSetID">�ĵ���ID</param>
        /// <param name="docInfo">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetLatestDocID(string szDocSetID, ref MDSDBLib.MedDocInfo docInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetLatestDocID(szDocSetID, ref docInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetLatestDocID", new string[] { "szDocSetID" }, new object[] { szDocSetID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡϵͳ�Դ���ָ���ĵ����͵��ĵ�ģ������
        /// </summary>
        /// <param name="szDocTypeID">�ĵ����ʹ���</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetSystemTemplet(string szDocTypeID, ref byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetSystemTemplet(szDocTypeID, ref byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetSystemTemplet", new string[] { "szDocTypeID" }
                    , new object[] { szDocTypeID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�û��Զ�����ĵ�ģ������
        /// </summary>
        /// <param name="szTempletID">�ĵ�ģ��ID</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetUserTemplet(string szTempletID, ref byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetUserTemplet(szTempletID, ref byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserTemplet", new string[] { "szTempletID" }
                    , new object[] { szTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����û��Զ�����ĵ�ģ������
        /// </summary>
        /// <param name="templetInfo">�ĵ�ģ����Ϣ</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveUserTemplet(MDSDBLib.TempletInfo templetInfo, byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.SaveUserTemplet(templetInfo, byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveUserTemplet", new string[] { "templetInfo" }
                    , new object[] { templetInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ϵͳĬ��ģ��
        /// </summary>
        /// <param name="szDocTypeID">�ĵ����ʹ���</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveSystemTemplet(string szDocTypeID, byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.SaveSystemTemplet(szDocTypeID, byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveSystemTemplet", new string[] { "szDocTypeID" }
                    , new object[] { szDocTypeID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ĵ����ʹ�������ĵ�������Ϣ
        /// </summary>
        /// <param name="docTypeInfo">�ĵ�������Ϣ</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short ModifyDocTypeInfo(MDSDBLib.DocTypeInfo docTypeInfo)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.ModifyDocTypeInfo(docTypeInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyDocTypeInfo", new string[] { "docTypeInfo" }
                    , new object[] { docTypeInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸��û�ģ�干��ȼ�
        /// </summary>
        /// <param name="szTempletID">�û�ģ��ID</param>
        /// <param name="szShareLevel">ģ���µĹ�����</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyUserTempletShareLevel(string szTempletID, string szShareLevel)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifyUserTempletShareLevel(szTempletID, szShareLevel);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyUserTempletShareLevel", new string[] { "szTempletID", "szShareLevel" }
                    , new object[] { szTempletID, szShareLevel }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸��û�ģ������
        /// </summary>
        /// <param name="szTempletID">�û�ģ��ID</param>
        /// <param name="szTempletName">ģ���µ�����</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyUserTempletName(string szTempletID, string szTempletName)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifyUserTempletName(szTempletID, szTempletName);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyUserTempletName", new string[] { "szTempletID", "szTempletName" }
                    , new object[] { szTempletID, szTempletName }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸��û�ģ�常Ŀ¼
        /// </summary>
        /// <param name="szTempletID">�û�ģ��ID</param>
        /// <param name="szTempletName">ģ���µĸ�Ŀ¼ID</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyUserTempletParentID(string szTempletID, string szParentID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifyUserTempletParentID(szTempletID, szParentID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyUserTempletParentID", new string[] { "szTempletID", "szParentID" }
                    , new object[] { szTempletID, szParentID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ɾ��ָ����һ���û�ģ��
        /// </summary>
        /// <param name="szTempletID">�û�ģ��ID</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteUserTemplet(string szTempletID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.DeleteUserTemplet(szTempletID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteUserTemplet", new string[] { "szTempletID" }
                    , new object[] { szTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ɾ��ָ����һϵ���û�ģ��
        /// </summary>
        /// <param name="lstTempletID">�û�ģ��ID�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteUserTemplet(List<string> lstTempletID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.DeleteUserTemplet(lstTempletID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteUserTemplet", new string[] { "lstTempletID" }
                    , new object[] { lstTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����ĵ�ID��ȡ�ĵ�����
        /// </summary>
        /// <param name="szDocID">�ĵ����</param>
        /// <param name="byteDocData">�ĵ�����������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocByID(string szDocID, ref byte[] byteDocData)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetDocByID(szDocID, ref byteDocData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocByID", new string[] { "szDocID" }
                    , new object[] { szDocID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �������ĵ�
        /// </summary>
        /// <param name="oDocInfo">�ĵ���Ϣ</param>
        /// <param name="byteDocData">�ĵ�����������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveDoc(MDSDBLib.MedDocInfo oDocInfo, byte[] byteDocData)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.SaveDoc(oDocInfo, byteDocData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveDoc", new string[] { "oDocInfo" }
                    , new object[] { oDocInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���������ĵ�
        /// </summary>
        /// <param name="szOldDocID">�������ĵ����</param>
        /// <param name="oNewDocInfo">���ĵ���Ϣ��</param>
        /// <param name="byteDocData">�ĵ�����������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short UpdateDoc(string szOldDocID, MDSDBLib.MedDocInfo oNewDocInfo, byte[] byteDocData)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.UpdateDoc(szOldDocID, oNewDocInfo, byteDocData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.UpdateDoc", new string[] { "szOldDocID", "oNewDocInfo" }
                    , new object[] { szOldDocID, oNewDocInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸Ĳ�������
        /// </summary>
        /// <param name="szDocID">���޸ĵĲ������</param>
        /// <param name="szDocTitle">�µĲ�������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyDocTitle(string szDocID, string szDocTitle)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.ModifyDocTitle(szDocID, szDocTitle);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyDocTitle", new string[] { "szDocID", "szDocTitle" }
                    , new object[] { szDocID, szDocTitle }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���������ĵ�
        /// </summary>
        /// <param name="szDocID">�ĵ����</param>
        /// <param name="szModifierID">�ĵ��޸��߱��</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short CancelDoc(ref MDSDBLib.DocStatusInfo docStatusInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.SetDocStatusInfo(ref docStatusInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.CancelDoc", new string[] { "docStatusInfo" }
                    , new object[] { docStatusInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ���ĵ�״̬��Ϣ
        /// </summary>
        /// <param name="szDocID">�ĵ����</param>
        /// <param name="szModifierID">�ĵ��޸��߱��</param>
        /// <param name="szModifierName">�ĵ��޸�������</param>
        /// <param name="szDeptCode">�ĵ��޸������ڿ��ұ��</param>
        /// <param name="szDeptName">�ĵ��޸������ڿ��Ҵ���</param>
        /// <param name="szDocStatus">�ĵ�״̬</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocStatusInfo(string szDocID, ref MDSDBLib.DocStatusInfo docStatusInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetDocStatusInfo(szDocID, ref docStatusInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocStatusInfo", new string[] { "szDocID" }
                    , new object[] { szDocID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ĵ�״̬��Ϣ
        /// </summary>
        /// <param name="szDocID">�ĵ����</param>
        /// <param name="szModifierID">�ĵ��޸��߱��</param>
        /// <param name="szModifierName">�ĵ��޸�������</param>
        /// <param name="szDeptCode">�ĵ��޸������ڿ��ұ��</param>
        /// <param name="szDeptName">�ĵ��޸������ڿ��Ҵ���</param>
        /// <param name="szDocStatus">�ĵ�״̬</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SetDocStatusInfo(ref MDSDBLib.DocStatusInfo docStatusInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.SetDocStatusInfo(ref docStatusInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SetDocStatusInfo", new string[] { "docStatusInfo" }
                    , new object[] { docStatusInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ������ָ�������µ��������̼�¼
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">����ID</param>
        /// <param name="lstPastDocInfos">�������̼�¼</param>
        /// <returns>MedDocSys.Common.CommonData.ReturnValue</returns>
        public static short GetPastDocList(string szPatientID, string szVisitID, ref List<MDSDBLib.MedDocInfo> lstPastDocInfos)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetPastDocList(szPatientID, szVisitID, ref lstPastDocInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetPastDocList", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ�����˵ľ����б�
        /// </summary>
        /// <param name="szPatientID">���˱��</param>
        /// <param name="lstVisitInfo">������Ϣ�б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetPatVisitList(string szPatientID, ref List<MDSDBLib.VisitInfo> lstVisitInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetPatVisitList(szPatientID, ref lstVisitInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetVisitList", new string[] { "szPatientID" }
                    , new object[] { szPatientID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���ݲ���ID�;���ţ���ȡ�ô�סԺ�ļ�����Ϣ�б�
        /// </summary>
        /// <param name="szPatientID">���˱��</param>
        /// <param name="szVisitID">�����</param>
        /// <param name="lstLabTestInfo">������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetInpLabTestList(string szPatientID, string szVisitID, ref List<MDSDBLib.LabTestInfo> lstLabTestInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetInpLabTestList(szPatientID, szVisitID, ref lstLabTestInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetInpLabTestList", new string[] { "szPatientID", "szVisitID", }
                    , new object[] { szPatientID, szVisitID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        ///  ���ݲ���ID�;���ʱ�����䣬��ȡ�ô�����ļ�����Ϣ�б�
        /// </summary>
        /// <param name="szPatientID">���˱��</param>
        /// <param name="dtVisitDateTime">�ôξ���ʱ��</param>
        /// <param name="dtNextVisitDateTime">�´ξ���ʱ��</param>
        /// <param name="lstLabTestInfo">������Ϣ�б�</param>
        /// <returns>Common.CommonData.ReturnValu</returns>
        public static short GetClinicLabTestList(string szPatientID, DateTime dtVisitTime, DateTime dtNextVisitTime, ref List<MDSDBLib.LabTestInfo> lstLabTestInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetClinicLabTestList(szPatientID, dtVisitTime, dtNextVisitTime, ref lstLabTestInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetClinicLabTestList", new string[] { "szPatientID", "dtVisitDateTime", "dtNextVisitDateTime" }
                    , new object[] { szPatientID, dtVisitTime, dtNextVisitTime }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ����������ţ���ȡ�������б�
        /// </summary>
        /// <param name="szTestNo">�������</param>
        /// <param name="lstTestResultInfo">��������Ϣ��</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetTestResultList(string szTestNo, ref List<MDSDBLib.TestResultInfo> lstTestResultInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetTestResultList(szTestNo, ref lstTestResultInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetTestResultList", new string[] { "szTestNo" }
                    , new object[] { szTestNo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ľ�����Ϣ����ȡ�ôξ���ʱ�������Ĵ����б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">������</param>
        /// <param name="szVisitType">��������</param>
        /// <param name="dtVisitTime">����ʱ��</param>
        /// <param name="lstRxInfo">������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetClinicRxList(string szPatientID, ref List<MDSDBLib.MedRxInfo> lstRxInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetClinicRxList(szPatientID, ref lstRxInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetVisitRxList", new string[] { "szPatientID" }
                    , new object[] { szPatientID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���Ĳ��˺;�����Ϣ����ȡ�ôξ���ʱ������������������Ϣ�б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">����ID</param>
        /// <param name="lstVitalSigns">���������б���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetVitalSignsList(string szPatientID, string szVisitID, ref List<MDSDBLib.VitalSignsInfo> lstVitalSigns)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetVitalSignsList(szPatientID, szVisitID, ref lstVitalSigns);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetVitalSignsList", new string[] { "szPatientID", "szVisitID", }
                    , new object[] { szPatientID, szVisitID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ľ�����Ϣ����ȡ�ôξ���ʱ��������ҽ�������б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">������</param>
        /// <param name="lstOrderInfo">ҽ����Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetInpOrderList(string szPatientID, string szVisitID, ref List<MDSDBLib.MedOrderInfo> lstOrderInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetInpOrderList(szPatientID, szVisitID, ref lstOrderInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetVisitOrderList", new string[] { "szPatientID", "szVisitID" }
                    , new object[] { szPatientID, szVisitID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ľ�����Ϣ����ȡ�ô�סԺ�����ļ�������б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">������</param>
        /// <param name="lstOrderInfo">�����Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetClinicExamList(string szPatientID, DateTime dtVisitTime, DateTime dtNextVisitTime, ref List<MDSDBLib.ExamInfo> lstExamInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetClinicExamList(szPatientID, dtVisitTime, dtNextVisitTime, ref lstExamInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetClinicExamList", new string[] { "szPatientID", "dtVisitTime", "dtNextVisitTime" }
                    , new object[] { szPatientID, dtVisitTime, dtNextVisitTime }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ľ�����Ϣ����ȡ�ô���������ļ�������б�
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="dtVisitTime">�ôξ���ʱ��</param>
        /// <param name="dtNextDateTime">�´ξ���ʱ��</param>
        /// <param name="lstOrderInfo">�����Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetInpExamList(string szPatientID, string szVisitID, ref List<MDSDBLib.ExamInfo> lstExamInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetInpExamList(szPatientID, szVisitID, ref lstExamInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.CANCEL;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetInpExamList", new string[] { "szPatientID", "szVisitID" }
                    , new object[] { szPatientID, szVisitID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ָ���ľ�����Ϣ����ȡ�ôξ���ʱ�������ļ�鱨����ϸ��Ϣ
        /// </summary>
        /// <param name="szExamNo">�������</param>
        /// <param name="examReportInfo">��鱨����Ϣ</param>
        /// <returns>MedDocSys.Common.CommonData.ReturnValue</returns>
        public static short GetExamResultInfo(string szExamNo, ref MDSDBLib.ExamResultInfo examReportInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetExamResultInfo(szExamNo, ref examReportInfo);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MqsDbAccess.GetExamReportDetialInfo", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���ĵ��������л�ȡָ�����˵ľ�����Ϣ�б�
        /// </summary>
        /// <param name="szPatientID">���˱��</param>
        /// <param name="lstVisitInfos">������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetDocVisitInfoList(string szPatientID, ref List<MDSDBLib.VisitInfo> lstVisitInfos)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.GetDocVisitInfoList(szPatientID, ref lstVisitInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDocVisitInfoList", new string[] { "szPatientID" }
                    , new object[] { szPatientID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�Զ���ҽѧ���Ｏ�ϡ�<br/>
        /// ���UserIDΪ��,��ô����DeptCodeָ���Ŀ��ҹ��������<br/>
        /// ������߶�Ϊ��,��ô����ȫԺ���������
        /// </summary>
        /// <param name="szUserID">����ָ���û��������б�</param>
        /// <param name="szDeptCode">����ָ�����ҹ���ȼ��������б�</param>
        /// <param name="lstMedTerms">ҽѧ������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetUserMedTerms(string szUserID, string szDeptCode, ref List<MDSDBLib.MedTermInfo> lstMedTerms)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetUserMedTerms(szUserID, szDeptCode, ref lstMedTerms);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserMedTerms", new string[] { "szUserID", "szDeptCode" }
                    , new object[] { szUserID, szDeptCode }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ�����������
        /// </summary>
        /// <param name="szTermID">����ID</param>
        /// <param name="szTermText">��������</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetUserMedTermText(string szTermID, ref string szTermText)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetUserMedTermText(szTermID, ref szTermText);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserMedTermText", new string[] { "szTermID" }
                    , new object[] { szTermID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��������IDɾ��һ������
        /// </summary>
        /// <param name="szTermID">����ID</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteUserMedTerm(string szTermID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.DeleteUserMedTerm(szTermID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteUserMedTerm", new string[] { "szTermID" }
                    , new object[] { szTermID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ���û�����
        /// </summary>
        /// <param name="medTermInfo">������Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveUserMedTerm(MDSDBLib.MedTermInfo medTermInfo)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.SaveUserMedTerm(medTermInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveUserMedTerm", new string[] { "medTermInfo" }
                    , new object[] { medTermInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ���û�����
        /// </summary>
        /// <param name="medTermInfo">������Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyUserMedTerm(MDSDBLib.MedTermInfo medTermInfo)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifyUserMedTerm(medTermInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveUserMedTerm", new string[] { "medTermInfo" }
                    , new object[] { medTermInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ָ���ṹ������SQL���,�����ĵ�,�������ĵ��б�
        /// </summary>
        /// <param name="queryInfo">�ṹ������������Ϣ</param>
        /// <param name="lstDocInfos">�ĵ���Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SearchStructDocument(MDSDBLib.StructQueryInfo queryInfo, ref List<MDSDBLib.MedDocInfo> lstDocInfos)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            //short shRet = EMRDBAccess.SearchStructDocument(queryInfo, ref lstDocInfos);
            //if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            //    return SystemData.ReturnValue.OK;
            //if (shRet != SystemData.ReturnValue.OK)
            //{
            //    LogManager.Instance.WriteLog("DataAccess.GetStructDocInfos", new string[] { "queryInfo" }
            //        , new object[] { queryInfo }, DataAccess.GetDBAccessError(shRet));
            //    return shRet;
            //}
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�ĵ��ʿع���Entry�б�
        /// </summary>
        /// <param name="lstQCRules">Entry�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetQCEntryList(ref List<MDSDBLib.MedQCEntry> lstQCEntrys)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.GetQCEntryList(ref lstQCEntrys);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetQCEntryList", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�ĵ��ʿع����б�
        /// </summary>
        /// <param name="lstQCRules">�����б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetQCRuleList(ref List<MDSDBLib.MedQCRule> lstQCRules)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.GetQCRuleList(ref lstQCRules);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetQCRuleList", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ��ԭ�ӹ���
        /// </summary>
        /// <param name="qcEntry"></param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveQCEntry(MDSDBLib.MedQCEntry qcEntry, MDSDBLib.MedQCRule qcRule)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.SaveQCEntry(qcEntry, qcRule);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveQCEntry", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����һ�����Ϲ���
        /// </summary>
        /// <param name="qcRule">������Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveQCRule(MDSDBLib.MedQCRule qcRule)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.SaveQCRule(qcRule);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveQCRule", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸�һ��ԭ�ӹ���
        /// </summary>
        /// <param name="qcEntry">ԭ�ӹ�����Ϣ</param>
        /// <param name="qcRule">ԭ�ӹ����Ӧ�Ĺ�����Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyQCEntry(MDSDBLib.MedQCEntry qcEntry, MDSDBLib.MedQCRule qcRule)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.ModifyQCEntry(qcEntry, qcRule);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyQCEntry", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸�һ�����Ϲ���
        /// </summary>
        /// <param name="qcRule">������Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifyQCRule(MDSDBLib.MedQCRule qcRule)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.ModifyQCRule(qcRule);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifyQCRule", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ɾ��һ��ԭ�ӹ���
        /// </summary>
        /// <param name="szEntryID">��QC_Entry����ID</param>
        /// <param name="szRuleID">��QC_Rule����ID</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteQCEntry(string szEntryID, string szRuleID)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.DeleteQCEntry(szEntryID, szRuleID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteQCEntry", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ɾ��һ�����Ϲ���
        /// </summary>
        /// <param name="szRuleID"></param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteQCRule(string szRuleID)
        {
            if (DataAccess.MedQCAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedQCAccess.DeleteQCRule(szRuleID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteQCRule", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���ĵ����ͻ�ȡ�����ݲ�ѯ����б�
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <param name="lstStructBindConfigInfos">���ݲ�ѯ����б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetElementBindConfig(string szDocTypeID, ref List<MDSDBLib.StructBindConfigInfo> lstStructBindConfigInfos)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = ConfigAccess.GetElementBindConfig(szDocTypeID, ref lstStructBindConfigInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetStructBindConfig", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ִ���ĵ������ݲ�ѯ���
        /// </summary>
        /// <param name="szQuerySQL">�ĵ������ݲ�ѯ���</param>
        /// <param name="lstAliasText">������ֵ�б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetStructBindData(string szQuerySQL, ref String[] arrBindData)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            //short shRet = m_MdsDbAccess.GetStructBindData(szQuerySQL, ref arrBindData);
            //if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
            //    return SystemData.ReturnValue.OK;
            //if (shRet != SystemData.ReturnValue.OK)
            //{
            //    LogManager.Instance.WriteLog("DataAccess.GetStructBindData", null, null, DataAccess.GetDBAccessError(shRet));
            //    return shRet;
            //}
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡWord��ʽ�Ĳ����������ķ��ʲ���
        /// </summary>
        /// <param name="szServerIP">Word����������IP��ַ</param>
        /// <param name="szRootPath">Word������������Ŀ¼</param>
        /// <returns>MedDocSys.Common.CommonData.ReturnValue</returns>
        public static short GetMedFileSrvConfig(ref string szServerIP, ref string szRootPath)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = ConfigAccess.GetMedFileSrvConfig(ref szServerIP, ref szRootPath);
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetWordEmrConfig", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ָ������������ĵ���״̬����Ϊ�ѹ鵵״̬
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">�����</param>
        /// <param name="dtVisitTime">����ʱ��</param>
        /// <param name="szVisitType">��������</param>
        /// <param name="szOperatorID">������ID</param>
        /// <param name="szOperatorName">����������</param>
        /// <param name="szExceptionInfo">�����쳣��Ϣ</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short ArchiveDocument(string szPatientID, string szVisitID, DateTime dtVisitTime, string szVisitType, string szOperatorID, string szOperatorName, ref string szExceptionInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.ArchiveDocument(szPatientID, szVisitID, dtVisitTime, szVisitType, szOperatorID, szOperatorName, ref szExceptionInfo);
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ArchiveDocument", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ָ������������ĵ���״̬����Ϊ����״̬
        /// </summary>
        /// <param name="szPatientID">����ID</param>
        /// <param name="szVisitID">�����</param>
        /// <param name="dtVisitTime">����ʱ��</param>
        /// <param name="szVisitType">��������</param>
        /// <param name="szOperatorID">������ID</param>
        /// <param name="szOperatorName">����������</param>
        /// <param name="szExceptionInfo">�����쳣��Ϣ</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short RollbackDocument(string szPatientID, string szVisitID, DateTime dtVisitTime, string szVisitType, string szOperatorID, string szOperatorName, ref string szExceptionInfo)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.RollbackDocument(szPatientID, szVisitID, dtVisitTime, szVisitType, szOperatorID, szOperatorName, ref szExceptionInfo);
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.RollbackDocument", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ���Ľṹ��Сģ�������
        /// </summary>
        /// <param name="szTempletID">ģ��ID</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetSmallTemplet(string szTempletID, ref byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetSmallTemplet(szTempletID, ref byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserTemplet", new string[] { "szTempletID" }
                    , new object[] { szTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ṹ��Сģ����Ϣ������
        /// </summary>
        /// <param name="templetInfo">ģ����Ϣ</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short SaveSmallTemplet(MDSDBLib.SmallTempletInfo templetInfo, byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.SaveSmallTemplet(templetInfo, byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveSmallTemplet", new string[] { "templetInfo" }
                    , new object[] { templetInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ���½ṹ��Сģ����Ϣ������
        /// </summary>
        /// <param name="templetInfo">ģ����Ϣ</param>
        /// <param name="byteTempletData">ģ������</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short UpdateSmallTemplet(MDSDBLib.SmallTempletInfo templetInfo, byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.UpdateSmallTemplet(templetInfo, byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.UpdateSmallTemplet", new string[] { "templetInfo" }
                    , new object[] { templetInfo }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸�ָ���Ľṹ��Сģ�干��ȼ�
        /// </summary>
        /// <param name="szTempletID">�û�ģ��ID</param>
        /// <param name="szShareLevel">ģ���µĹ�����</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifySmallTempletShareLevel(string szTempletID, string szShareLevel)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifySmallTempletShareLevel(szTempletID, szShareLevel);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifySmallTempletShareLevel", new string[] { "szTempletID", "szShareLevel" }
                    , new object[] { szTempletID, szShareLevel }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸�ָ���Ľṹ��Сģ������
        /// </summary>
        /// <param name="szTempletID">ģ��ID</param>
        /// <param name="szTempletName">ģ���µ�����</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifySmallTempletName(string szTempletID, string szTempletName)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifySmallTempletName(szTempletID, szTempletName);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifySmallTempletName", new string[] { "szTempletID", "szTempletName" }
                    , new object[] { szTempletID, szTempletName }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸�ָ���Ľṹ��Сģ��ĸ�Ŀ¼
        /// </summary>
        /// <param name="szTempletID">ģ��ID</param>
        /// <param name="szTempletName">ģ���µĸ�Ŀ¼ID</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short ModifySmallTempletParentID(string szTempletID, string szParentID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifySmallTempletParentID(szTempletID, szParentID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.ModifySmallTempletParentID", new string[] { "szTempletID", "szParentID" }
                    , new object[] { szTempletID, szParentID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ɾ��ָ����һ���ṹ��Сģ��
        /// </summary>
        /// <param name="szTempletID">Сģ��ID</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteSmallTemplet(string szTempletID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.DeleteSmallTemplet(szTempletID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteSmallTemplet", new string[] { "szTempletID" }
                    , new object[] { szTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ɾ��ָ����һϵ�нṹ��Сģ��
        /// </summary>
        /// <param name="lstTempletID">Сģ��ID�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short DeleteSmallTemplet(List<string> lstTempletID)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.DeleteSmallTemplet(lstTempletID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteSmallTemplet", new string[] { "lstTempletID" }
                    , new object[] { lstTempletID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ���û�ID��Сģ���б�
        /// </summary>
        /// <param name="szUserID">�û�ID</param>
        /// <param name="lstTempletInfos">Сģ���б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetPersonalSmallTempletInfos(string szUserID, ref List<MDSDBLib.SmallTempletInfo> lstTempletInfos)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetPersonalSmallTempletInfos(szUserID, ref lstTempletInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetPersonalSmallTempletInfos", new string[] { "szUserID" }
                    , new object[] { szUserID }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ�����ҵ�Сģ���б�
        /// </summary>
        /// <param name="szDeptCode">���Ҵ���</param>
        /// <param name="bOnlyDeptShare">�Ƿ�����ر��Ϊ���ҹ����Сģ��</param>
        /// <param name="lstTempletInfos">Сģ���б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetDeptSmallTempletInfos(string szDeptCode, bool bOnlyDeptShare, ref List<MDSDBLib.SmallTempletInfo> lstTempletInfos)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetDeptSmallTempletInfos(szDeptCode, bOnlyDeptShare, ref lstTempletInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetDeptSmallTempletInfos", new string[] { "szDeptCode", "bOnlyDeptShare" }
                    , new object[] { szDeptCode, bOnlyDeptShare }, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡȫԺСģ���б�
        /// </summary>
        /// <param name="lstTempletInfos">Сģ���б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetHospitalSmallTempletInfos(ref List<MDSDBLib.SmallTempletInfo> lstTempletInfos)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetHospitalSmallTempletInfos(ref lstTempletInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetHospitalSmallTempletInfos", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡϵͳ�Դ���Сģ���б�
        /// </summary>
        /// <param name="lstTempletInfos">Сģ���б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetSystemSmallTempletInfos(ref List<MDSDBLib.SmallTempletInfo> lstTempletInfos)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetSystemSmallTempletInfos(ref lstTempletInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetSystemSmallTempletInfos", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ѭ��ָ�����ĵ�ID�б���˳�������Щ�ĵ���¼��ORDER_VALUE�ֶ�
        /// </summary>
        /// <param name="lstDocIDList">�ĵ�ID�б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short UpdateDocOrder(List<string> lstDocIDList)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = MedDocAccess.UpdateDocOrder(lstDocIDList);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.UpdateDocOrder", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�����ֵ�����й�Ԫ�ر����������б�
        /// </summary>
        /// <param name="lstConfigInfo">�й�Ԫ�ر����������б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetElementAliasList(ref List<MDSDBLib.ConfigInfo> lstConfigInfo)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = ConfigAccess.GetElementAliasList(ref lstConfigInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetElementAliasList", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�����ֵ���мκͱ༭���ؼ���һ��������ò���
        /// </summary>
        /// <param name="szRegCode">ע����</param>
        /// <param name="szOcxTag1">�༭����ʶ1</param>
        /// <param name="szOcxTag2">�༭����ʶ2</param>
        /// <param name="szUserCode">ע���û�����</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetEPRPad2Config(ref string szRegCode, ref string szOcxTag1, ref string szOcxTag2, ref string szUserCode)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = ConfigAccess.GetEPRPad2Config(ref szRegCode, ref szOcxTag1, ref szOcxTag2, ref szUserCode);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetEMRPad3Config", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�����ֵ���мκͱ༭���ؼ��ڶ���������ò���
        /// </summary>
        /// <param name="szRegCode">ע����</param>
        /// <param name="szOcxTag1">�༭����ʶ1</param>
        /// <param name="szOcxTag2">�༭����ʶ2</param>
        /// <param name="szUserCode">ע���û�����</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetEMRPad3Config(ref string szRegCode, ref string szOcxTag1, ref string szOcxTag2, ref string szUserCode)
        {
            if (DataAccess.ConfigAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = ConfigAccess.GetEMRPad3Config(ref szRegCode, ref szOcxTag1, ref szOcxTag2, ref szUserCode);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetEMRPad3Config", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ����ĳ���û��ĳ��ò���������Ϣ
        /// </summary>
        /// <param name="szDocTypeID">�û�ID</param>
        /// <param name="docTypeInfo">������Ϣ</param>
        /// <returns></returns>
        public static short SaveCommonUseTemplet(string szUserID, string szDocTypeID)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.SaveCommonUseTemplet(szUserID, szDocTypeID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.SaveCommonUseTemplet", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡĳ���û����õĲ��������б�
        /// </summary>
        /// <param name="userID">�û�ID</param>
        /// <param name="lstSubDocTypeInfo">�����б�</param>
        /// <returns></returns>
        public static short GetCommonDocInfosByUserID(string szUserID, ref List<MDSDBLib.DocTypeInfo> lstSubDocTypeInfo)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.GetCommonDocInfosByUserID(szUserID, ref lstSubDocTypeInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetCommonDocInfosByUserID", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����ĵ�����ID���û�IDɾ�����ò���
        /// </summary>
        /// <param name="szDocTypeID">�ĵ�����ID</param>
        /// <param name="szUserID">�û�ID</param>
        /// <returns></returns>
        public static short DeleteCommonUseTempletByDocTypeID(string szDocTypeID, string szUserID)
        {
            if (DataAccess.DocTypeAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = DocTypeAccess.DeleteCommonUseTempletByDocTypeID(szDocTypeID, szUserID);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.DeleteCommonUseTempletByDocTypeID", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��xml��ʽ�Ĳ����ı����浽XDB���ݿ���
        /// </summary>
        /// <param name="szDocID">�ɵ��ĵ�ID</param>
        /// <param name="szLocalFile">�ĵ���Ϣ</param>
        /// <param name="szLocalFile">xml�����ַ���</param>
        /// <returns></returns>
        public static short SaveToXDB(string szOldDocID, MDSDBLib.MedDocInfo szDocInfo, string szXmlData)
        {
            if (DataAccess.MedDocAccess == null)
                return SystemData.ReturnValue.FAILED;

            //short shRet = m_MdsDbAccess.SaveToXDB(szOldDocID, szDocInfo, szXmlData);
            //if (shRet != SystemData.ReturnValue.OK)
            //{
            //    LogManager.Instance.WriteLog("DataAccess.SaveToXDB", null, null, DataAccess.GetDBAccessError(shRet));
            //    return shRet;
            //}
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡָ�����û���Ϣ
        /// </summary>
        /// <param name="szUserID">�û�ID</param>
        /// <param name="userInfo">���ص��û���Ϣ</param>
        /// <returns>SystemData.ReturnValue</returns>
        public static short GetUserInfo(string szUserID, ref UserInfo userInfo)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            MDSDBLib.UserInfo dbUserInfo = null;
            short shRet = EMRDBAccess.GetUserInfo(szUserID, ref dbUserInfo);

            //û�в�ѯ��
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.FAILED;

            //��ѯ���쳣
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetUserInfo", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.EXCEPTION;
            }

            //ת���û���ϢΪ�ͻ��˶���
            if (dbUserInfo != null)
            {
                if (userInfo == null) userInfo = new UserInfo();
                userInfo.ID = dbUserInfo.ID;
                userInfo.Name = dbUserInfo.Name;
                userInfo.DeptCode = dbUserInfo.DeptCode;
                userInfo.DeptName = dbUserInfo.DeptName;
                userInfo.Password = dbUserInfo.Password;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// У���û�������,У��ɹ��󷵻��û���Ϣ
        /// </summary>
        /// <param name="szUserID">�û�ID</param>
        /// <param name="szUserPwd">����</param>
        /// <returns>MedDocSys.Common.GlobalData.ReturnValue</returns>
        public static short UserVerify(string szUserID, string szUserPwd)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.VerifyUser(szUserID, szUserPwd);
            if (shRet != MDSDBLib.SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.UserVerify", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �޸��û�ģ����������Ϣ
        /// </summary>
        /// <param name="templetInfo">�û�ģ����Ϣ</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short ModifyTempletCheckInfo(MDSDBLib.TempletInfo templetInfo)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.ModifyUserTempletCheckInfo(templetInfo);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MtsDbAccess.ModifyTempletCheckInfo", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �����û�ģ���ļ�
        /// </summary>
        /// <param name="templetInfo">�û�ģ����Ϣ</param>
        /// <param name="byteTempletData">�û�ģ���ļ�����</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short UpdateUserTemplet(MDSDBLib.TempletInfo templetInfo, byte[] byteTempletData)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.UpdateUserTemplet(templetInfo, byteTempletData);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MtsDbAccess.UpdateUserTemplet", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ�ٴ�������Ϣ�б�
        /// </summary>
        /// <param name="lstDeptInfos">������Ϣ�б�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public static short GetClinicDeptList(ref List<MDSDBLib.DeptInfo> lstDeptInfos)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetClinicDeptList(ref lstDeptInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("DataAccess.GetClinicDeptList", null, null, DataAccess.GetDBAccessError(shRet));
                return shRet;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡסԺ�����б�
        /// </summary>
        /// <param name="lstDeptInfos">סԺ�����б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetWardDeptList(ref List<MDSDBLib.DeptInfo> lstDeptInfos)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetWardDeptList(ref lstDeptInfos);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MtsDbAccess.GetInpDeptList", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ��������б�
        /// </summary>
        /// <param name="lstDeptInfos">��������б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetOutPDeptList(ref List<MDSDBLib.DeptInfo> lstDeptInfos)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetOutPDeptList(ref lstDeptInfos);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MtsDbAccess.GetOutpDeptList", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ����Ԫ�����б�
        /// </summary>
        /// <param name="lstDeptInfos">����Ԫ�����б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetNurseDeptList(ref List<MDSDBLib.DeptInfo> lstDeptInfos)
        {
            if (DataAccess.EMRDBAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = EMRDBAccess.GetNurseDeptList(ref lstDeptInfos);
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MtsDbAccess.GetNurseDeptList", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ��ȡ������ָ��ʱ����ڴ���������ģ����Ϣ
        /// </summary>
        /// <param name="szDeptCode">ָ�����Ҵ���</param>
        /// <param name="dtBeginTime">ָ��������ʼʱ��</param>
        /// <param name="dtEndTime">ָ����������ʱ��</param>
        /// <param name="lstTempletInfos">�ĵ�ģ����Ϣ�б�</param>
        /// <returns>CommonConst.ReturnValue</returns>
        public static short GetUserTempletInfos(string szDeptCode, DateTime dtBeginTime, DateTime dtEndTime, ref List<MDSDBLib.TempletInfo> lstTempletInfos)
        {
            if (DataAccess.TempletAccess == null)
                return SystemData.ReturnValue.FAILED;

            short shRet = TempletAccess.GetUserTempletInfos(szDeptCode, dtBeginTime, dtEndTime, ref lstTempletInfos);
            if (shRet == MDSDBLib.SystemData.ReturnValue.RES_NO_FOUND)
                return SystemData.ReturnValue.OK;
            if (shRet != SystemData.ReturnValue.OK)
            {
                LogManager.Instance.WriteLog("MtsDbAccess.GetTempletInfos", null, null, DataAccess.GetDBAccessError(shRet));
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }
    }
}