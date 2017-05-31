// *****************************************************************************
// �����ĵ�ϵͳ���ڷ��ʾ���ϵͳ�в������������ʳ���(ͨ��fsrv.dll)
// Creator:YangMingkun  Date:2009-11-25
// Copyright:supconhealth
// *****************************************************************************
using System;
using System.Runtime.InteropServices;
using MedDocSys.Common;

namespace MedDocSys.DataLayer
{
    public class FsrvAccess
    {
        private static FsrvAccess m_Instance = null;
        private string m_szServerIP = null;
        private string m_szRootPath = null;

        [DllImport("fsrv.dll")]
        extern static int get_file(string szHostAddr, string szRemoteFile, string szLocalFile, int flag);
        [DllImport("fsrv.dll")]
        extern static int put_file(string szHostAddr, string szLocalFile, string szRemoteFile, int flag);

        /// <summary>
        /// ��ȡMedFileAccess����ʵ��
        /// </summary>
        public static FsrvAccess Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new FsrvAccess();
                return m_Instance;
            }
        }
        private FsrvAccess()
        {
        }

        private bool Initialize()
        {
            if (!GlobalMethods.AppMisc.IsEmptyString(this.m_szServerIP))
                return true;
            string szServerIP = null;
            string szRootPath = null;
            short shRet = DataAccess.GetMedFileSrvConfig(ref szServerIP, ref szRootPath);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            this.m_szServerIP = szServerIP;
            this.m_szRootPath = szRootPath;

            string szFsrvDllPath = string.Concat(GlobalMethods.AppMisc.GetWorkingPath(), "\\fsrv.dll");
            if (!System.IO.File.Exists(szFsrvDllPath))
            {
                LogManager.Instance.WriteLog("MedFileAccess.Initialize", string.Concat("δ�ҵ��ļ�:", szFsrvDllPath));
                return false;
            }
            return true;
        }

        /// <summary>
        /// ����ָ����Զ�̹����ļ���ָ���ı����ļ�
        /// </summary>
        /// <param name="szRemoteFile">Զ�̹����ļ�</param>
        /// <param name="szLocalFile">ָ���ı����ļ�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short DownloadFile(string szRemoteFile, string szLocalFile)
        {
            if (!this.Initialize())
                return SystemData.ReturnValue.FAILED;
            szRemoteFile = string.Format("{0}\\{1}", this.m_szRootPath, szRemoteFile);
            if (get_file(this.m_szServerIP, szRemoteFile, szLocalFile, 1) < 0)
            {
                LogManager.Instance.WriteLog("MedFileAccess.GetFileByFsrvdll", "get_fileִ��ʧ��!");
                return SystemData.ReturnValue.FAILED;
            }
            return SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// �ϴ�ָ����Զ�̹����ļ���ָ���ı����ļ�
        /// </summary>
        /// <param name="szLocalFile">ָ���ı����ļ�</param>
        /// <param name="szRemoteFile">Զ�̹����ļ�</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        public short UploadFile(string szLocalFile, string szRemoteFile)
        {
            if (!this.Initialize())
                return SystemData.ReturnValue.FAILED;
            szRemoteFile = string.Format("{0}\\{1}", this.m_szRootPath, szRemoteFile);
            string szCompareFile = string.Format("{0}_{1}_server", szLocalFile, SysTimeHelper.Instance.Now.ToString("yyMMddHHmmss"));
            int nPutResult = -1;
            int nRetryCount = 3;
            while (nPutResult < 0 && nRetryCount > 0)
            {
                nPutResult = put_file(this.m_szServerIP, szLocalFile, szRemoteFile, 1);
                nRetryCount--;
                if (nPutResult < 0)
                {
                    LogManager.Instance.WriteLog("MedFileAccess.PutFileByFsrvdll", "put_fileִ��ʧ��!");
                    continue;
                }
                int nGetResult = get_file(this.m_szServerIP, szRemoteFile, szCompareFile, 1);
                if (nGetResult < 0)
                {
                    nPutResult = -1;
                    LogManager.Instance.WriteLog("MedFileAccess.PutFileByFsrvdll", "get_fileִ��ʧ��!");
                    continue;
                }
                if (GlobalMethods.IO.CompareFileLength(szLocalFile, szCompareFile))
                    break;
                nPutResult = -1;
            }
            GlobalMethods.IO.DeleteFile(szCompareFile);
            return nPutResult < 0 ? SystemData.ReturnValue.FAILED : SystemData.ReturnValue.OK;
        }
    }
}
