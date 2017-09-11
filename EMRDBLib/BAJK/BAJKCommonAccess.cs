// ***********************************************************
// ���ݿ���ʲ�ͨ�ò����йص����ݷ��ʽӿ�.
// Creator:YangMingkun  Date:2010-11-18
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Heren.Common.Libraries;
using Heren.Common.Libraries.DbAccess;
using EMRDBLib.DbAccess;

namespace EMRDBLib
{
    public class BAJKCommonAccess : DBAccessBase
    {
        private static BAJKCommonAccess m_Instance = null;

        /// <summary>
        /// ��ȡϵͳ����������ʵ��
        /// </summary>
        public static BAJKCommonAccess Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new BAJKCommonAccess();
                return m_Instance;
            }
        }

        /// <summary>
        /// ִ��ָ����SQL����ѯ
        /// </summary>
        /// <param name="sql">��ѯ��SQL���</param>
        /// <param name="result">��ѯ���صĽ����</param>
        /// <returns>ServerData.ExecuteResult</returns>
        public short ExecuteQuery(string sql, out DataSet result)
        {
            result = null;
            if (base.BAJKDataAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;
            if (string.IsNullOrEmpty(sql))
                return SystemData.ReturnValue.PARAM_ERROR;
            try
            {
                result = base.BAJKDataAccess.ExecuteDataSet(sql, CommandType.Text);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("CommonAccess.ExecuteQuery", new string[] { "sql" }, new object[] { sql }, ex);
                return SystemData.ReturnValue.EXCEPTION;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ִ��ָ����һϵ�е�SQL������
        /// </summary>
        /// <param name="isProc">�����SQL�Ƿ��Ǵ洢����</param>
        /// <param name="sqlArray">��ѯ��SQL��伯��</param>
        /// <returns>ServerData.ExecuteResult</returns>
        public short ExecuteUpdate(bool isProc, params string[] sqlarray)
        {
            if (base.BAJKDataAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            if (!base.BAJKDataAccess.BeginTransaction(IsolationLevel.ReadCommitted))
                return SystemData.ReturnValue.EXCEPTION;

            if (sqlarray == null)
                sqlarray = new string[0];
            foreach (string sql in sqlarray)
            {
                try
                {
                    if (!isProc)
                        base.BAJKDataAccess.ExecuteNonQuery(sql, CommandType.Text);
                    else
                        base.BAJKDataAccess.ExecuteNonQuery(sql, CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    base.BAJKDataAccess.AbortTransaction();
                    LogManager.Instance.WriteLog("CommonAccess.ExecuteUpdate", new string[] { "sql" }, new object[] { sql }, ex);
                    return SystemData.ReturnValue.EXCEPTION;
                }
            }
            base.BAJKDataAccess.CommitTransaction(true);
            return SystemData.ReturnValue.OK;
        }
    }
}
