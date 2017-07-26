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
    public class NurDocCommonAccess : DBAccessBase
    {
        private static NurDocCommonAccess m_Instance = null;

        /// <summary>
        /// ��ȡϵͳ����������ʵ��
        /// </summary>
        public static NurDocCommonAccess Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new NurDocCommonAccess();
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
            if (base.NurDocAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            try
            {
                result = base.NurDocAccess.ExecuteDataSet(sql, CommandType.Text);
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
            if (base.HerenHisAccess == null)
                return SystemData.ReturnValue.PARAM_ERROR;

            if (!base.HerenHisAccess.BeginTransaction(IsolationLevel.ReadCommitted))
                return SystemData.ReturnValue.EXCEPTION;

            if (sqlarray == null)
                sqlarray = new string[0];
            foreach (string sql in sqlarray)
            {
                try
                {
                    if (!isProc)
                        base.HerenHisAccess.ExecuteNonQuery(sql, CommandType.Text);
                    else
                        base.HerenHisAccess.ExecuteNonQuery(sql, CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    base.HerenHisAccess.AbortTransaction();
                    LogManager.Instance.WriteLog("CommonAccess.ExecuteUpdate", new string[] { "sql" }, new object[] { sql }, ex);
                    return SystemData.ReturnValue.EXCEPTION;
                }
            }
            base.HerenHisAccess.CommitTransaction(true);
            return SystemData.ReturnValue.OK;
        }
    }
}
