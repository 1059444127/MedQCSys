// ***********************************************************
// ���ݿ���ʲ����û�������ݵķ�����.
// Creator:YangAiping  Date:2014-05-27
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Heren.Common.Libraries;
using System.Data.OleDb;
using Heren.Common.Libraries.DbAccess;

namespace EMRDBLib.DbAccess
{
    public class UserAccess : DBAccessBase
    {
        private static UserAccess m_Instance = null;

        /// <summary>
        /// ��ȡϵͳ����������ʵ��
        /// </summary>
        public static UserAccess Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new UserAccess();
                return m_Instance;
            }
        }

        /// <summary>
        /// ���ݿ��ұ�Ż�ȡ�û���Ϣ�б�
        /// </summary>
        /// <param name="szDeptCode">���ұ��</param>
        /// <param name="lstUserInfos">�û���Ϣ�б�</param>
        /// <returns>SystemData.ReturnValue</returns>
        public short GetDeptUserList(string szDeptCode, ref List<UserInfo> lstUserInfos)
        {
            if (base.DataAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            //�Ȼ�ȡ�û���Ϣ
            string szField = string.Format("{0},{1},{2},{3},{4}"
                , SystemData.UserView.USER_ID, SystemData.UserView.USER_NAME
                , SystemData.UserView.DEPT_CODE, SystemData.UserView.DEPT_NAME, SystemData.UserView.USER_PWD);
            string szTable = SystemData.DataView.USER_V;
            string szCondition = string.Format("{0}={1}", SystemData.UserView.DEPT_CODE, base.ParaHolder(SystemData.UserView.DEPT_CODE));
            string szSQL = string.Format(SystemData.SQL.SELECT_WHERE, szField, szTable, szCondition);
            DbParameter[] paras = new DbParameter[1] { new DbParameter(SystemData.UserView.DEPT_CODE, szDeptCode) };
            IDataReader dataReader = null;
            try
            {
                dataReader = base.DataAccess.ExecuteReader(szSQL, CommandType.Text, ref paras);
                if (dataReader == null || dataReader.IsClosed || !dataReader.Read())
                {
                    return SystemData.ReturnValue.RES_NO_FOUND;
                }
                if (lstUserInfos == null)
                    lstUserInfos = new List<UserInfo>();
                do
                {
                    UserInfo userInfo = new UserInfo();
                    if (!dataReader.IsDBNull(0)) userInfo.ID = dataReader.GetString(0);
                    if (!dataReader.IsDBNull(1)) userInfo.Name = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) userInfo.DeptCode = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3)) userInfo.DeptName = dataReader.GetString(3);
                    if (!dataReader.IsDBNull(4)) userInfo.Password = dataReader.GetString(4);
                    lstUserInfos.Add(userInfo);
                } while (dataReader.Read());
                return SystemData.ReturnValue.OK;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("EMRDBAccess.GetDeptUserList", new string[] { "szSQL" }, new object[] { szSQL }, ex);
                return SystemData.ReturnValue.EXCEPTION;
            }
            finally { base.DataAccess.CloseConnnection(false); }
        }

        /// <summary>
        /// �����û�����ȡָ�����û���Ϣ
        /// </summary>
        /// <param name="szUserName">�û���</param>
        /// <param name="lstUserInfo">�û���Ϣ</param>
        /// <returns>SystemData.ReturnValue</returns>
        public short GetUserInfo(string szUserName, ref List<UserInfo> lstUserInfo)
        {
            if (GlobalMethods.Misc.IsEmptyString(szUserName))
            {
                LogManager.Instance.WriteLog("EMRDBAccess.GetUserInfo", "�û���������Ϊ��!");
                return SystemData.ReturnValue.PARAM_ERROR;
            }

            if (base.DataAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            //�Ȼ�ȡ�û���Ϣ
            string szField = string.Format("{0},{1},{2},{3},{4}"
                , SystemData.UserView.USER_ID, SystemData.UserView.USER_NAME
                , SystemData.UserView.DEPT_CODE, SystemData.UserView.DEPT_NAME, SystemData.UserView.USER_PWD);
            string szCondition = string.Format("{0} like '%{1}%'", SystemData.UserView.USER_NAME, szUserName);
            string szTable = SystemData.DataView.USER_V;
            string szOrder = SystemData.UserView.DEPT_CODE;
            string szSQL = string.Format(SystemData.SQL.SELECT_DISTINCT_WHERE_ORDER_ASC, szField, szTable, szCondition, szOrder);
            IDataReader dataReader = null;
            try
            {
                dataReader = base.DataAccess.ExecuteReader(szSQL, CommandType.Text);
                if (dataReader == null || dataReader.IsClosed || !dataReader.Read())
                {
                    return SystemData.ReturnValue.RES_NO_FOUND;
                }
                if (lstUserInfo == null)
                    lstUserInfo = new List<UserInfo>();
                do
                {
                    UserInfo userInfo = new UserInfo();
                    if (!dataReader.IsDBNull(0)) userInfo.ID = dataReader.GetString(0);
                    if (!dataReader.IsDBNull(1)) userInfo.Name = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) userInfo.DeptCode = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3)) userInfo.DeptName = dataReader.GetString(3);
                    if (!dataReader.IsDBNull(4)) userInfo.Password = dataReader.GetString(4);
                    lstUserInfo.Add(userInfo);
                } while (dataReader.Read());
                return SystemData.ReturnValue.OK;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("EMRDBAccess.GetUserInfo", new string[] { "szSQL" }, new object[] { szSQL }, ex);
                return SystemData.ReturnValue.EXCEPTION;
            }
            finally { base.DataAccess.CloseConnnection(false); }
        }
        /// <summary>
        /// ��ȡȫԺ�����û��б�
        /// </summary>
        /// <param name="lstUserInfos">�û��б�</param>
        /// <returns>SystemData.ReturnValue</returns>
        public short GetAllUserInfos(ref List<UserInfo> lstUserInfos)
        {
            if (base.DataAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            //�Ȼ�ȡ�û���Ϣ
            string szField = string.Format("{0},{1},{2},{3},{4}"
                , SystemData.UserView.USER_ID, SystemData.UserView.USER_NAME
                , SystemData.UserView.DEPT_CODE, SystemData.UserView.DEPT_NAME, SystemData.UserView.USER_PWD);
            string szTable = SystemData.DataView.USER_V;
            string szOrder = SystemData.UserView.DEPT_NAME;
            string szSQL = string.Format(SystemData.SQL.SELECT_ORDER_ASC, szField, szTable, szOrder);
            IDataReader dataReader = null;
            try
            {
                dataReader = base.DataAccess.ExecuteReader(szSQL, CommandType.Text);
                if (dataReader == null || dataReader.IsClosed || !dataReader.Read())
                {
                    return SystemData.ReturnValue.RES_NO_FOUND;
                }
                if (lstUserInfos == null)
                    lstUserInfos = new List<UserInfo>();
                do
                {
                    UserInfo userInfo = new UserInfo();
                    if (!dataReader.IsDBNull(0)) userInfo.ID = dataReader.GetString(0);
                    if (!dataReader.IsDBNull(1)) userInfo.Name = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) userInfo.DeptCode = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3)) userInfo.DeptName = dataReader.GetString(3);
                    if (!dataReader.IsDBNull(4)) userInfo.Password = dataReader.GetString(4);
                    lstUserInfos.Add(userInfo);
                } while (dataReader.Read());
                return SystemData.ReturnValue.OK;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("UserAccess.GetAllUserInfos", new string[] { "szSQL" }, new object[] { szSQL }, ex);
                return SystemData.ReturnValue.EXCEPTION;
            }
            finally { base.DataAccess.CloseConnnection(false); }
        }
        /// <summary>
        /// �����û�ID��ȡָ�����û���Ϣ
        /// </summary>
        /// <param name="szUserID">�û�ID</param>
        /// <param name="userInfo">���ص��û���Ϣ</param>
        /// <returns>SystemData.ReturnValue</returns>
        public short GetUserInfo(string szUserID, ref UserInfo userInfo)
        {
            if (GlobalMethods.Misc.IsEmptyString(szUserID))
            {
                LogManager.Instance.WriteLog("EMRDBAccess.GetUserInfo", "�û�ID����Ϊ��!");
                return SystemData.ReturnValue.PARAM_ERROR;
            }

            if (base.DataAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            //�Ȼ�ȡ�û���Ϣ
            string szField = string.Format("{0},{1},{2},{3},{4},{5}"
                , SystemData.UserView.USER_ID, SystemData.UserView.USER_NAME
                , SystemData.UserView.DEPT_CODE, SystemData.UserView.DEPT_NAME, SystemData.UserView.USER_PWD, SystemData.UserView.USER_GRADE);
            string szCondition = string.Format("{0}='{1}' "
                , SystemData.UserView.USER_ID, szUserID);
            string szTable = SystemData.DataView.USER_V;
            string szSQL = string.Format(SystemData.SQL.SELECT_WHERE, szField, szTable, szCondition);
            IDataReader dataReader = null;
            try
            {
                dataReader = base.DataAccess.ExecuteReader(szSQL, CommandType.Text);
                if (dataReader == null || dataReader.IsClosed || !dataReader.Read())
                {
                    return SystemData.ReturnValue.RES_NO_FOUND;
                }
                if (userInfo == null)
                    userInfo = new UserInfo();
                if (!dataReader.IsDBNull(0)) userInfo.ID = dataReader.GetString(0);
                if (!dataReader.IsDBNull(1)) userInfo.Name = dataReader.GetString(1);
                if (!dataReader.IsDBNull(2)) userInfo.DeptCode = dataReader.GetString(2);
                if (!dataReader.IsDBNull(3)) userInfo.DeptName = dataReader.GetString(3);
                if (!dataReader.IsDBNull(4)) userInfo.Password = dataReader.GetString(4);
                if (!dataReader.IsDBNull(5)) userInfo.Grade = dataReader.GetValue(5).ToString();
                return SystemData.ReturnValue.OK;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("EMRDBAccess.GetUserInfo", new string[] { "szSQL" }, new object[] { szSQL }, ex);
                return SystemData.ReturnValue.EXCEPTION;
            }
            finally { base.DataAccess.CloseConnnection(false); }
        }
    }
}
