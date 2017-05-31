/************************************************************
 * @FileName   : Startup.cs
 * @Description: ���Ӳ���ϵͳ֮ϵͳ�Զ���������������
 * @Author     : ������(YangMingkun)
 * @Date       : 2015-12-7 12:56:09
 * @History    : 
 * @Copyright  : ��Ȩ����(C)�㽭���ʿƼ��ɷ����޹�˾
************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Heren.Common.Libraries;
using EMRDBLib;

namespace Heren.MedQC.Upgrade
{
    public class Startup
    {
        [STAThread]
        public static void Main(string[] args)
        {
            LogManager.Instance.TextLogOnly = true;
            SystemConfig.Instance.ConfigFile = SystemParam.Instance.ConfigFile;
            MessageBoxEx.Caption = "�����ʿ�ϵͳ";

            FileMapping fileMapping = new FileMapping(SystemData.MappingName.UPGRADE_SYS);
            if (fileMapping.IsFirstInstance)
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();
                WorkProcess.Instance.Initialize(null, 10, "����׼��������������ϵͳ�����Ժ�...");
                (new UpgradeHandler()).BeginUpgradeSystem(args.Length > 0 ? args[0] : null);
                fileMapping.Dispose(true);
            }
        }
    }
}
