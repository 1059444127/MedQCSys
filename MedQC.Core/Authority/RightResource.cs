using System;
using System.Collections.Generic;
using System.Text;

namespace Heren.MedQC.Core
{
    public class RightResource 
    {
        /// <summary>
        /// ģ�����ڲ�Ʒ�ı�ʶ
        /// </summary>
        public static string PRODUCT_MEDQC = "MedQC";
        /// <summary>
        /// �û�����ģ��ʹ��Ȩ�޵�
        /// </summary>
        public static string MedQC_Hdp_Role = "MedQC.Hdp.Role";
        /// <summary>
        /// �û�����ģ��ʹ��Ȩ�޵�
        /// </summary>
        public static string MedQC_Hdp_User = "MedQC.Hdp.User";
        /// <summary>
        /// �û�����ģ��ʹ��Ȩ�޵�
        /// </summary>
        public static string MedQC_Hdp_Product = "MedQC.Hdp.Product";
        /// <summary>
        /// �û�����ģ��ʹ��Ȩ�޵�
        /// </summary>
        public static string MedQC_Hdp_UIConfig = "MedQC.Hdp.UIConfig";


        /// <summary>
        /// ģ�����ڲ�Ʒ�ı�ʶ
        /// </summary>
        public static string PRODUCT_MED_RECORD = "MedRecord";
        /// <summary>
        /// ֽ�ʲ������β�ѯ
        /// </summary>
        public static string MedRecord_RecMrBatch = "MedRecord.RecMrBatch";
        /// <summary>
        /// ֽ�ʲ�����ݽ���
        /// </summary>
        public static string MedRecord_ReceiveInOrder = "MedRecord_ReceiveInOrder";
        /// <summary>
        /// ֽ�ʲ��������ύ
        /// </summary>
        public static string MedRecord_RecMrBatchSend = "MedRecord_RecMrBatchSend";
        /// <summary>
        /// ����������
        /// </summary>
        public static string MedRecord_RecBrowseRequest = "MedRecord_RecBrowseRequest";


        #region AbstractRight ��Ա

        public static RightPoint[] GetRightPoints(string szProduct)
        {
            if (szProduct == PRODUCT_MEDQC)
            {
                RightPoint[] rightPoint = new RightPoint[]{
                  new RightPoint(MedQC_Hdp_Role,"��ɫ����","��ɫ����"),
                  new RightPoint(MedQC_Hdp_User,"�û�����","�û�����"),
                  new RightPoint(MedQC_Hdp_Product,"��Ʒ����","��Ʒ����"),
                  new RightPoint(MedQC_Hdp_UIConfig,"�˵�����","�˵�����"),
                };
                return rightPoint;
            }
            else if (szProduct == PRODUCT_MED_RECORD)
            {
                RightPoint[] rightPoint = new RightPoint[]{
                  new RightPoint(PRODUCT_MED_RECORD,"��������ϵͳ",""),
                  new RightPoint(MedRecord_RecMrBatch,"ֽ�ʲ������β�ѯ","ֽ�ʲ������β�ѯ"),
                  new RightPoint(MedRecord_ReceiveInOrder,"ֽ�ʲ�����ݽ���","ֽ�ʲ�����ݽ���"),
                  new RightPoint(MedRecord_RecMrBatchSend,"ֽ�ʲ��������ύ","ֽ�ʲ��������ύ"),
                  new RightPoint(MedRecord_RecBrowseRequest,"����������","����������")
                };
                return rightPoint;
            }
            return null;
        }

        public static string[] GetModuleResources(string szProduct)
        {
            return new string[] { PRODUCT_MEDQC};
        }
        #endregion
    }
}
