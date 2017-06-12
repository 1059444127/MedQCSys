﻿using EMRDBLib;
using EMRDBLib.DbAccess;
using EMRDBLib.HerenHis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heren.Common.Libraries;
using EMRDBLib.BAJK;
namespace Heren.MedQC.Core.Services
{
    /// <summary>
    /// 病案上传服务
    /// </summary>
    public class RecUploadService
    {
        private static RecUploadService m_instance = null;
        /// <summary>
        /// 病案上传服务
        /// </summary>
        public static RecUploadService Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new RecUploadService();
                return m_instance;
            }
        }

        private RecUploadService()
        {

        }
        /// <summary>
        /// 性别对照字典集合
        /// </summary>
        private List<RecCodeCompasion> SexDict = null;
        /// <summary>
        /// 婚姻对照字典集合
        /// </summary>
        private List<RecCodeCompasion> MaritalStatusDict = null;
        /// <summary>
        /// 职业
        /// </summary>
        private List<RecCodeCompasion> OccupationDict = null;
        /// <summary>
        /// 行政区域/籍贯/住址等字典对照集合
        /// </summary>
        private List<RecCodeCompasion> AreaDict = null;
        /// <summary>
        /// 民族字典对照集合
        /// </summary>
        private List<RecCodeCompasion> NationDict = null;
        /// <summary>
        /// 关系字典对照集合
        /// </summary>
        private List<RecCodeCompasion> RelationShipDict = null;
        /// <summary>
        /// ABO血型字典对照集合
        /// </summary>
        private List<RecCodeCompasion> BloodABOTypeDict = null;
        /// <summary>
        /// RH血型字典对照集合
        /// </summary>
        private List<RecCodeCompasion> BloodRHTypeDict = null;
        /// <summary>
        /// 病案质量
        /// </summary>
        private List<RecCodeCompasion> MrQualityDict = null;
        /// <summary>
        /// 入院方式
        /// </summary>
        private List<RecCodeCompasion> PatientClassDict = null;
        /// <summary>
        /// 国籍
        /// </summary>
        private List<RecCodeCompasion> CountryDict = null;
        /// <summary>
        /// 出院方式
        /// </summary>
        private List<RecCodeCompasion> DischargeDisPositionDict = null;
        /// <summary>
        /// 诊断类别
        /// </summary>
        private List<RecCodeCompasion> DiagnosisTypeDict = null;
        /// <summary>
        /// 治疗结果
        /// </summary>
        private List<RecCodeCompasion> TreatingResultDict = null;

        public bool InitializeDict()
        {
            short shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("SEX_DICT", ref this.SexDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("MARITAL_STATUS_DICT", ref this.MaritalStatusDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("OCCUPATION_DICT", ref this.OccupationDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("AREA_DICT", ref this.AreaDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("NATION_DICT", ref this.NationDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("RELATIONSHIP_DICT", ref this.RelationShipDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("BLOOD_ABO_TYPE_DICT", ref this.BloodABOTypeDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("BLOOD_RH_TYPE_DICT", ref this.BloodRHTypeDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("MR_QUALITY_DICT", ref this.MrQualityDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("PATIENT_CLASS_DICT", ref this.PatientClassDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("COUNTRY_DICT", ref this.CountryDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("DISCHARGE_DISPOSITION_DICT", ref this.DischargeDisPositionDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("DIAGNOSIS_TYPE_DICT", ref this.DiagnosisTypeDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;
            shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions("TREATING_RESULT_DICT", ref this.TreatingResultDict);
            if (shRet != SystemData.ReturnValue.OK)
                return false;

            return true;
        }
        public bool Upload(string szPatientID, string szVisitID)
        {
            try
            {

                StringBuilder sbField = new StringBuilder();
                InpVisit inpVisit = null;
                PatMasterIndex patMasterIndex = null;
                short shRet = EMRDBLib.DbAccess.HerenHis.InpVisitAccess.Instance.GetInpVisit(szPatientID, szVisitID, ref inpVisit);
                shRet = PatMasterIndexAccess.Instance.GetModel(szPatientID, ref patMasterIndex);
                if (inpVisit == null || patMasterIndex == null)
                    return false;
                string patientID = inpVisit.PATIENT_ID;
                string visitNo = inpVisit.VISIT_NO;

                List<RecCodeCompasion> lstRecCodeCompasion = null;
                shRet = RecCodeCompasionAccess.Instance.GetRecCodeCompasions(null, ref lstRecCodeCompasion);
                if (inpVisit == null)
                    return false;
                EMRDBLib.BAJK.BAJK08 bajk08 = null;
                shRet = BAJK08Access.Instance.GetBAJK08s(szPatientID, szVisitID, ref bajk08);
                if (bajk08 == null)
                    bajk08 = new EMRDBLib.BAJK.BAJK08();
                bajk08.COL0801 = szPatientID;
                if (string.IsNullOrEmpty(inpVisit.NAME))
                    bajk08.COL0802 = patMasterIndex.NAME;
                else
                    bajk08.COL0802 = inpVisit.NAME;
                if (SexDict != null)
                {
                    var result = SexDict.Where(m => m.CODE_NAME == patMasterIndex.SEX).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0803 = decimal.Parse(result.DM);
                    }
                }
                bajk08.COL0804 = inpVisit.VISIT_ID;
                bajk08.COL0805 = patMasterIndex.DATE_OF_BIRTH;
                if (MaritalStatusDict != null && !string.IsNullOrEmpty(inpVisit.MARITAL_STATUS))
                {
                    var result = MaritalStatusDict.Find(m => m.CODE_NAME == inpVisit.MARITAL_STATUS);
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0806 = decimal.Parse(result.DM);
                    }
                }
                if (OccupationDict != null && !string.IsNullOrEmpty(inpVisit.OCCUPATION))
                {
                    var result = OccupationDict.Where(m => m.CODE_ID == inpVisit.OCCUPATION).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0807 = decimal.Parse(result.DM);
                    }
                }
                if (this.AreaDict != null && !string.IsNullOrEmpty(patMasterIndex.NATIVE_PLACE))
                {
                    var result = this.AreaDict.Where(m => m.CODE_ID == patMasterIndex.NATIVE_PLACE).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0808 = decimal.Parse(result.DM);
                    }
                }

                if (this.NationDict != null && !string.IsNullOrEmpty(patMasterIndex.NATION))
                {
                    var result = this.NationDict.Where(m => m.CODE_NAME == patMasterIndex.NATION).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0809 = decimal.Parse(result.DM);
                    }
                }

                bajk08.COL0810 = patMasterIndex.ID_NO;
                bajk08.COL0811 = patMasterIndex.WORKING_ADDRESS;
                bajk08.COL0812 = patMasterIndex.PHONE_NUMBER_BUSINESS;
                bajk08.COL0813 = patMasterIndex.WORKING_ADDRESS_ZIPCODE;
                bajk08.COL0814 = patMasterIndex.PRESENT_ADDRESS_COUNTY + patMasterIndex.PRESENT_ADDRESS_OTHERS;//县+街道门牌
                bajk08.COL0815 = patMasterIndex.PRESENT_ADDRESS_ZIPCODE;
                bajk08.COL0816 = patMasterIndex.NEXT_OF_KIN;

                if (this.RelationShipDict != null && !string.IsNullOrEmpty(patMasterIndex.RELATIONSHIP))
                {
                    var result = this.RelationShipDict.Where(m => m.CODE_ID == patMasterIndex.RELATIONSHIP).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0817 = decimal.Parse(result.DM);
                    }
                }
                bajk08.COL0818 = patMasterIndex.NEXT_OF_KIN_ADDR;
                bajk08.COL0819 = patMasterIndex.NEXT_OF_KIN_PHONE;
                if (this.BloodABOTypeDict != null && !string.IsNullOrEmpty(inpVisit.BLOOD_TYPE))
                {
                    var result = this.BloodABOTypeDict.Where(m => m.CODE_NAME == inpVisit.BLOOD_TYPE).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0820 = decimal.Parse(result.DM);
                    }
                }
                if (this.BloodRHTypeDict != null && !string.IsNullOrEmpty(inpVisit.BLOOD_TYPE_RH))
                {
                    var result = this.BloodRHTypeDict.Where(m => m.CODE_ID == inpVisit.BLOOD_TYPE_RH).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0821 = decimal.Parse(result.DM);
                    }
                }
                bajk08.COL0822 = inpVisit.EMER_TREAT_TIMES;
                bajk08.COL0823 = inpVisit.ESC_EMER_TIMES;

                if (this.MrQualityDict != null && !string.IsNullOrEmpty(inpVisit.MR_QUALITY))
                {
                    var result = this.MrQualityDict.Where(m => m.CODE_NAME == inpVisit.MR_QUALITY).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0827 = decimal.Parse(result.DM);
                    }
                }

                bajk08.COL0828 = inpVisit.DOCTOR_OF_CONTROL_QUALITY;
                bajk08.COL0829 = inpVisit.NURSE_OF_CONTROL_QUALITY;
                //bajk08.COL0830 治疗类别没有

                if (this.PatientClassDict != null && !string.IsNullOrEmpty(inpVisit.PATIENT_CLASS))
                {
                    var result = this.MrQualityDict.Where(m => m.CODE_ID == inpVisit.PATIENT_CLASS).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0831 = decimal.Parse(result.DM);
                    }
                }
                if (inpVisit.BLOOD_TRAN_TIMES > 0 && inpVisit.BLOOD_TRAN_REACT_TIMES == 0)
                {
                    bajk08.COL0832 = 1;
                }
                else if (inpVisit.BLOOD_TRAN_TIMES > 0 && inpVisit.BLOOD_TRAN_REACT_TIMES > 0)
                {
                    bajk08.COL0832 = 2;
                }
                else if (inpVisit.BLOOD_TRAN_TIMES == 0)
                {
                    bajk08.COL0832 = 3;
                }
                bajk08.COL0834 = inpVisit.DIRECTOR_ID;
                bajk08.COL0835 = inpVisit.CHIEF_DOCTOR_ID;
                bajk08.COL0836 = inpVisit.ATTENDING_DOCTOR_ID;
                bajk08.COL0837 = inpVisit.DOCTOR_IN_CHARGE_ID;
                //bajk08.COL0838 = inpVisit.ADVANCED_STUDIES_DOCTOR;//编号未获取
                //bajk08.COL0839 = inpVisit.PRACTICE_DOCTOR;//编号未获取
                if (inpVisit.CATALOG_DATE != inpVisit.DefaultTime2)
                    bajk08.COL0841 = inpVisit.CATALOG_DATE;
                bajk08.COL0842 = inpVisit.CATALOGER;
                //bajk08.COL0843 手术标志未获取
                bajk08.COL0844 = inpVisit.DEPT_ADMISSION_TO;//未对照科室代码
                if (inpVisit.ADMISSION_DATE_TIME != inpVisit.DefaultTime2)
                    bajk08.COL0845 = inpVisit.ADMISSION_DATE_TIME;
                bajk08.COL0847 = inpVisit.DEPT_DISCHARGE_FROM;//未对照出院科室代码
                if (inpVisit.DISCHARGE_DATE_TIME != inpVisit.DefaultTime2)
                    bajk08.COL0848 = inpVisit.DISCHARGE_DATE_TIME;
                //bajk08.COL0849= 确诊天数未获取
                //bajk08.COL0850 确诊日期未获取
                if (inpVisit.DISCHARGE_DATE_TIME != inpVisit.DefaultTime2
                    && inpVisit.ADMISSION_DATE_TIME != inpVisit.DefaultTime2)
                    bajk08.COL0851 = GlobalMethods.SysTime.GetInpDays(inpVisit.ADMISSION_DATE_TIME, inpVisit.DISCHARGE_DATE_TIME);
                //bajk08.COL0852=其他方式未获取
                bajk08.COL0853 = inpVisit.TOTAL_COSTS;
                //bajk08.COL0854=疾病序号未获取，不确定首页改用哪个诊断
                //bajk08.COL0855 转归情况未获取，不确定首页改用哪个诊断
                //bajk08.COL0856 入出符合标志未获取
                //bajk08.COL0857 门出符合标志未获取
                //bajk08.COL0858 E序号未获取，搞不清含义
                //bajk08.COL0859 M序号未获取，
                bajk08.COL0866 = inpVisit.AUTOPSY_INDICATOR; //是否尸检
                bajk08.COL0871 = inpVisit.VISIT_NO;//唯一号设为就诊流水号
                //bajk08.COL0873=
                if (this.CountryDict != null && !string.IsNullOrEmpty(patMasterIndex.CITIZENSHIP))
                {
                    var result = this.CountryDict.Where(m => m.CODE_ID == patMasterIndex.CITIZENSHIP).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0873 = decimal.Parse(result.DM);
                    }
                }
                bajk08.COL0882 = inpVisit.DEPT_ADMISSION_TO;
                bajk08.COL0883 = inpVisit.DEPT_DISCHARGE_FROM;
                //bajk08.COL0884 临床与病理未获取
                //bajk08.COL0885 放射与病理未获取
                //bajk08.COL0887 主要诊断未获取

                //bajk08.COL0888 结帐标识未获取
                //bajk08.COL0889 术前与术后
                //bajk08.COL0890 自动出院
                bajk08.COL0891 = inpVisit.SECURITY_NO;
                //bajk08.COL0892 新生儿体重未获取
                //bajk08.COL0893 新生儿入院体重未获取
                if (this.AreaDict != null && !string.IsNullOrEmpty(patMasterIndex.BIRTH_PLACE_CODE))
                {
                    var result = this.AreaDict.Where(m => m.CODE_ID == patMasterIndex.BIRTH_PLACE_CODE).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0894 = decimal.Parse(result.DM);
                    }
                }
                if (this.AreaDict != null && !string.IsNullOrEmpty(patMasterIndex.PRESENT_ADDRESS_CODE))
                {
                    var result = this.AreaDict.Where(m => m.CODE_ID == patMasterIndex.PRESENT_ADDRESS_CODE).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0895 = patMasterIndex.PRESENT_ADDRESS_COUNTY
+ patMasterIndex.PRESENT_ADDRESS_OTHERS;
                    }
                }
                bajk08.COL0896 = patMasterIndex.PHONE_NUMBER;
                bajk08.COL0897 = patMasterIndex.PRESENT_ADDRESS_CODE;
                if (inpVisit.DATE_OF_CONTROL_QUALITY != inpVisit.DefaultTime2)
                    bajk08.COL0898 = inpVisit.DATE_OF_CONTROL_QUALITY;
                //bajk08.COL0899 出院方式/离院方式
                if (this.DischargeDisPositionDict != null && !string.IsNullOrEmpty(inpVisit.DISCHARGE_DISPOSITION))
                {
                    var result = this.AreaDict.Where(m => m.CODE_ID == inpVisit.DISCHARGE_DISPOSITION).FirstOrDefault();
                    if (result != null && !string.IsNullOrEmpty(result.DM))
                    {
                        bajk08.COL0899 = decimal.Parse(result.DM);
                    }
                }
                //bajk08.COLni0900= 拟接收医疗机构名称1未获取
                //bajk08.COL0902再住院计划未获取
                //bajk08.COL0903再住院目的未获取
                //bajk08.COL0904入院前昏迷时间
                //bajk08.COL0905入院后昏迷时间
                //bajk08.COL0906入院病情(主诊断)
                //bajk08.COL0907 药物过敏标志
                //bajk08.COL0908 责任护士编号
                //bajk08.COL0914
                //bajk08.COL0915
                //bajk08.COL0916
                //bajk08.COL0917
                //bajk08.COL0918
                //bajk08.COL0919
                //bajk08.COL0920
                //bajk08.COL0921
                //bajk08.COL0922
                //bajk08.COL0923
                //bajk08.COL0924
                //bajk08.COL0925
                //bajk08.COL0926
                //bajk08.COL0927
                //bajk08.COL0928
                //bajk08.COL0929
                //bajk08.COL0930
                //bajk08.COL0931
                //bajk08.COL0932
                //bajk08.COL0933
                //bajk08.COL0934
                //bajk08.COL0935
                //bajk08.COL0936
                if (bajk08.KEY0801 == 0)
                {
                    bajk08.KEY0801 = BAJK08Access.Instance.MakeKey0801();
                    shRet = BAJK08Access.Instance.Insert(bajk08);
                    if (shRet != SystemData.ReturnValue.OK)
                        return false;
                }
                else
                {
                    shRet = BAJK08Access.Instance.Update(bajk08);
                }
                //上传诊断情况
                //1、获取和仁his患者诊断信息
                List<Diagnosis> lstDiagnosis = null;
                shRet = DiagnosisAccess.Instance.GetList(patientID, visitNo, ref lstDiagnosis);
                //2、获取联众已经上传的诊断情况
                List<EMRDBLib.BAJK.BAJK09> lstBajk09 = null;
                shRet = BAJK09Access.Instance.GetBAJK09s(bajk08.KEY0801, ref lstBajk09);
                //3、清理已经上传的诊断情况
                if (lstBajk09 != null && lstBajk09.Count > 0)
                {
                    shRet = BAJK09Access.Instance.Delete(bajk08.KEY0801);
                }
                //4、上传诊断情况
                if (lstDiagnosis != null)
                {
                    foreach (var item in lstDiagnosis)
                    {
                        string jbmc = item.DIAG_DESC;
                        DiagnosisDict diagnosisDict = new DiagnosisDict();
                        shRet = DiagnosisDictAccess.Instance.GetModel(item.DIAG_DESC, ref diagnosisDict);
                        if (diagnosisDict == null)//首页诊断不标准，不上传
                            continue;
                        //查询联众疾病代码库疾病序号
                        BaIcdDM baicdDM = null;
                        shRet = BaIcdDMAccess.Instance.GetModel(diagnosisDict.DIAGNOSIS_CODE, ref baicdDM);
                        if (baicdDM == null)//代码库中未找到，则不上传（暂定）
                            continue;
                        EMRDBLib.BAJK.BAJK09 bajk09 = new EMRDBLib.BAJK.BAJK09();
                        bajk09.COL0901 = baicdDM.JBXH;
                        if (this.TreatingResultDict != null && !string.IsNullOrEmpty(item.TREAT_RESULT))
                        {
                            var result = this.TreatingResultDict.Where(m => m.CODE_NAME == item.TREAT_RESULT).FirstOrDefault();
                            if (result != null && !string.IsNullOrEmpty(result.DM))
                            {
                                bajk09.COL0902 = decimal.Parse(result.DM);
                            }
                        }
                        bajk09.COL0903 = item.DIAG_DESC;
                        //bajk09.COL0904 入院病情未获取
                        bajk09.KEY0901 = bajk08.KEY0801;
                        if (this.DiagnosisTypeDict != null && !string.IsNullOrEmpty(item.DIAG_TYPE))
                        {
                            var result = this.DiagnosisTypeDict.Where(m => m.CODE_ID == item.DIAG_TYPE).FirstOrDefault();
                            if (result != null && !string.IsNullOrEmpty(result.DM))
                            {
                                bajk09.KEY0902 = decimal.Parse(result.DM);
                            }
                        }
                        bajk09.KEY0903 = item.DIAG_NO;
                        shRet = BAJK09Access.Instance.Insert(bajk09);
                    }
                }
                //获取和仁His手术情况
                List<OperationMaster> lstOperationMasters = null;
                shRet = OperationMasterAccess.Instance.GetOperationMasters(patientID, szVisitID, ref lstOperationMasters);
                if (lstOperationMasters != null)
                {
                    foreach (var item in lstOperationMasters)
                    {
                        BAJK11 bajk11 = new BAJK11();
                        OperationName operationName = null;
                        shRet = OperationNameAccess.Instance.GetModel(item.OPER_NO, ref operationName);
                        if (operationName != null)
                        {
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("RecUploadSercvice.Upload", new string[] { "szPatientID", "szVisitID" }, new object[] { szPatientID, szVisitID }, "病案上传出错", ex);
                return false;
            }
            return true;
        }

    }
}