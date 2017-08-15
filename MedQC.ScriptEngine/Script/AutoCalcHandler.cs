/********************************************************
 * @FileName   : AutoCalcHandler.cs
 * @Description: 
 * @Author     : ������(YangMingkun)
 * @Date       : 2015-12-29 12:57:12
 * @History    : 
 * @Copyright  : ��Ȩ����(C)�㽭���ʿƼ��ɷ����޹�˾
********************************************************/
using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Heren.Common.Libraries;
using Heren.MedQC.ScriptEngine.Script;
using Heren.MedQC.Core;
using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.ScriptEngine
{
    public class AutoCalcHandler
    {
        private GetElementValueCallback m_getElementValueCallback = null;
        private SetElementValueCallback m_setElementValueCallback = null;
        private ShowElementTipCallback m_showElementTipCallback = null;
        private HideElementTipCallback m_hideElementTipCallback = null;
        private ExecuteQueryCallback m_executeQueryCallback = null;
        private ExecuteUpdateCallback m_executeUpdateCallback = null;
        private GetSystemContextCallback m_getSystemContextCallback = null;

        private static AutoCalcHandler m_Instance = null;
        public static AutoCalcHandler Instance {
            get {
                if (m_Instance == null)
                    m_Instance = new AutoCalcHandler();
                return m_Instance;
            }
        }
        public void Start()
        {
            this.m_getElementValueCallback = new GetElementValueCallback(this.GetElementValue);
            this.m_setElementValueCallback = new SetElementValueCallback(this.SetElementValue);
            this.m_showElementTipCallback = new ShowElementTipCallback(this.ShowElementTip);
            this.m_hideElementTipCallback = new HideElementTipCallback(this.HideElementTip);
            this.m_executeQueryCallback = new ExecuteQueryCallback(this.ExecuteQuery);
            this.m_executeUpdateCallback = new ExecuteUpdateCallback(this.ExecuteUpdate);
            this.m_getSystemContextCallback = new GetSystemContextCallback(this.GetSystemContext);
        }

        #region"Ԫ�ؼ���ű�����ص�"
        private bool GetElementValue(string elementName, out string elementValue)
        {
            elementValue = null;

            //if (activeDocument == null || activeDocument.IsDisposed)
            //    return false;
            //if (activeDocument.DocumentEditor == null || activeDocument.DocumentEditor.IsDisposed)
            //    return false;

            //elementValue = this.m_activeDocumentForm.DocumentEditor.GetElementText(elementName);
            //return elementValue != null;
            return true;
        }

        private bool SetElementValue(string szElementName, string szElementValue)
        {
            //if (this.m_medDocCtrl == null)
            //    return false;
            //ClinicalDocumentForm activeDocument = this.m_medDocCtrl.ActiveDocument;
            //if (activeDocument == null || activeDocument.IsDisposed)
            //    return false;
            //if (activeDocument.DocumentEditor == null || activeDocument.DocumentEditor.IsDisposed)
            //    return false;
            //string nameAndValue = string.Format("{0}={1}|", szElementName, szElementValue);
            //return activeDocument.DocumentEditor.SetElementText(nameAndValue, false);
            return true;
        }

        private bool ShowElementTip(string szTitle, string szTipText)
        {
            //ClinicalDocumentForm activeDocument = this.m_medDocCtrl.ActiveDocument;
            //if (activeDocument == null || activeDocument.IsDisposed)
            //    return false;
            //if (activeDocument.DocumentEditor == null || activeDocument.DocumentEditor.IsDisposed)
            //    return false;
            //activeDocument.DocumentEditor.ShowToolTip(szTitle, szTipText, 3000);
            return true;
        }

        private bool HideElementTip()
        {
            //ClinicalDocumentForm activeDocument = this.m_medDocCtrl.ActiveDocument;
            //if (activeDocument == null || activeDocument.IsDisposed)
            //    return false;
            //if (activeDocument.DocumentEditor == null || activeDocument.DocumentEditor.IsDisposed)
            //    return false;
            //activeDocument.DocumentEditor.HideToolTip();
            return true;
        }

        /// <summary>
        /// ִ��һ��SQL��ѯ
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>�Ƿ��ѯ�ɹ�</returns>
        private bool ExecuteQuery(string sql, out DataSet data)
        {
            short result = CommonAccess.Instance.ExecuteQuery(sql, out data);
            LogManager.Instance.WriteLog(sql);
            return result == SystemData.ReturnValue.OK;
        }

        /// <summary>
        /// ִ��ָ����һϵ�е�SQL����
        /// </summary>
        /// <param name="isProc">�����SQL�Ƿ��Ǵ洢����</param>
        /// <param name="sqlArray">SQL�������</param>
        /// <returns>�Ƿ���³ɹ�</returns>
        private bool ExecuteUpdate(bool isProc, params string[] sqlArray)
        {
            short result = CommonAccess.Instance.ExecuteUpdate(isProc, sqlArray);
            return result == SystemData.ReturnValue.OK;
        }
        public PatVisitInfo PatVisitInfo { get; set; }
        public QcCheckPoint QcCheckPoint { get; set; }
        public QcCheckResult QcCheckResult { get; set; }
        private bool GetSystemContext(string name, out object value)
        {
            value = null;
            if (name == "��ǰ����")
            {
                value = this.PatVisitInfo;
            }
            if (name == "�ʿؽ��")
            {
                value = this.QcCheckResult;
            }
            if (name == "�ʿع���")
            {
                value = this.QcCheckPoint;
            }
            //if (name == "�û�ID��")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.UserInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.UserInfo.ID;
            //}
            //else if (name == "�û�����")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.UserInfo.Name;
            //}
            //else if (name == "�û����Ҵ���")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.UserInfo.DeptCode;
            //}
            //else if (name == "�û���������")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.UserInfo.DeptName;
            //}
            //else if (name == "����ID��")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.PatientInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.PatientInfo.ID;
            //}
            //else if (name == "��������")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.PatientInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.PatientInfo.Name;
            //}
            //else if (name == "�����Ա�")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.PatientInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.PatientInfo.Gender;
            //}
            //else if (name == "��������")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.PatientInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.PatientInfo.BirthTime;
            //}
            //else if (name == "��Ժ��")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.VisitInfo.ID;
            //}
            //else if (name == "��Ժʱ��")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.VisitInfo.Time;
            //}
            //else if (name == "������Ҵ���")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.VisitInfo.DeptCode;
            //}
            //else if (name == "�����������")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.VisitInfo.DeptName;
            //}
            //else if (name == "��������")
            //{
            //    if (this.m_activeDocumentForm.MedDocCtrl.VisitInfo == null)
            //        return false;
            //    value = this.m_activeDocumentForm.MedDocCtrl.VisitInfo.Type.ToString();
            //}
            return true;
        }
        #endregion

        /// <summary>
        /// ��ȡ�����Hash�����Ԫ�ر����б�ֱ��Ӧ��Ԫ����Ϣ,�����丳ֵ
        /// </summary>
        /// <param name="htElements">Ԫ�ر����б�HashTable</param>
        /// <returns>true-��ȡ�ɹ�;false-��ȡʧ��</returns>
        private bool GetRelatedElementsValue(ref Hashtable htElements)
        {
            if (htElements == null || htElements.Count <= 0)
                return false;
           

            //StructElement element = this.m_activeDocumentForm.DocumentEditor.GetCurrentElement();
            //if (element == null)
            //    return false;

            //string szElementValue = this.m_activeDocumentForm.DocumentEditor.GetElementText();
            //if (string.IsNullOrEmpty(szElementValue))
            //    return false;

            //if (!htElements.Contains(element.ElementName))
            //    return false;
            //htElements[element.ElementName] = szElementValue;

            ////ѭ���Ӳ����н���ϣ���д�ŵ�ÿһ��������Ӧ��Ԫ����Ϣ��ȡ����
            //string[] arrKeys = new string[htElements.Count];
            //try
            //{
            //    htElements.Keys.CopyTo(arrKeys, 0);
            //}
            //catch (Exception ex)
            //{
            //    LogManager.Instance.WriteLog("CalcHelper.GetRelatedElementsValue", ex);
            //    return false;
            //}
            //for (int index = 0; index < arrKeys.Length; index++)
            //{
            //    string szElementName = arrKeys[index];
            //    if (string.IsNullOrEmpty(szElementName))
            //        continue;
            //    if (htElements[szElementName] != null)
            //        continue;

            //    string value = this.m_activeDocumentForm.DocumentEditor.GetElementText(szElementName);
            //    htElements[szElementName] = value;
            //}
            return true;
        }

        /// <summary>
        /// ���û��޸��ĵ��ڵļ�¼ʱ��ʱ,�Զ������ĵ���Ϣ�еļ�¼ʱ������
        /// </summary>
        /// <param name="szElementName">��ǰ���޸ĵ�Ԫ������</param>
        private void HandleRecordTimeElement(string szElementName)
        {
            //if (this.m_activeDocumentForm == null || this.m_activeDocumentForm.IsDisposed)
            //    return;
            //if (this.m_activeDocumentForm.Document == null || szElementName == null)
            //    return;

            //string szElementValue = this.m_activeDocumentForm.DocumentEditor.GetElementText();
            //if (string.IsNullOrEmpty(szElementValue))
            //    return;

            //szElementValue = GlobalMethods.Convert.SBCToDBC(szElementValue, null).Replace("��", " ");
            //DateTime dtRecordTime = DateTime.Now;
            //if (GlobalMethods.Convert.StringToDate(szElementValue, ref dtRecordTime))
            //{
            //    ClinicalDocument document = this.m_activeDocumentForm.Document;
            //    if (this.m_activeDocumentForm.Document.Combined)
            //        document = this.m_activeDocumentForm.GetCurrentChildDocument();
            //    if (document != null)
            //        document.ModifyRecordTime(dtRecordTime);
            //}
        }

        /// <summary>
        /// ���û��޸��ĵ��ڵ�����ʱ,�Զ�����������ݵ��ַ���Ŀ
        /// </summary>
        /// <param name="szElementName">��ǰ���޸ĵ�Ԫ������</param>
        private void HandlePatientDescElement(string szElementName)
        {
            //this.HideElementTip();
            //if (this.m_activeDocumentForm == null || this.m_activeDocumentForm.IsDisposed)
            //    return;
            //if (this.m_activeDocumentForm.Document == null || szElementName == null)
            //    return;

            //string szElementValue = this.m_activeDocumentForm.DocumentEditor.GetElementText();
            //if (string.IsNullOrEmpty(szElementValue))
            //    return;
            //szElementValue = szElementValue.Trim();
            //if (szElementValue.Length > 20)
            //{
            //    string text = "����������������Ѿ�����20���ַ�,����������һЩ!";
            //    this.m_activeDocumentForm.DocumentEditor.ShowToolTip("Υ��ҽ�Ƴ��������", text, 3000);
            //}
        }

        /// <summary>
        /// �����ĵ��е���ߺ����ؽṹ��Ԫ��ֵ�Զ�����BMIֵ
        /// </summary>
        /// <param name="szElementName">��ǰ���޸ĵ�Ԫ������</param>
        private void HandleBodyElement(string szElementName)
        {
            //if (this.m_activeDocumentForm == null || this.m_activeDocumentForm.IsDisposed)
            //    return;
            //if (GlobalMethods.Misc.IsEmptyString(szElementName))
            //    return;

            //Hashtable htElements = new Hashtable();
            //htElements.Add(SystemConsts.ElementName.BODY_HEIGHT, null);
            //htElements.Add(SystemConsts.ElementName.BODY_WEIGHT, null);
            //if (!this.GetRelatedElementsValue(ref htElements))
            //    return;

            //string szElementValue = htElements[SystemConsts.ElementName.BODY_HEIGHT] as string;
            //if (GlobalMethods.Misc.IsEmptyString(szElementValue))
            //    return;

            //double dBodyHeight = 0;
            //if (!double.TryParse(szElementValue.Trim(), out dBodyHeight))
            //    return;
            //if (dBodyHeight >= 3) //�����С��3,����Ϊ��λ��cm,��Ҫת��Ϊm
            //    dBodyHeight = dBodyHeight / 100;
            //if (dBodyHeight <= 0)
            //    return;

            //szElementValue = htElements[SystemConsts.ElementName.BODY_WEIGHT] as string;
            //if (GlobalMethods.Misc.IsEmptyString(szElementValue))
            //    return;

            //double dBodyWeight = 0;
            //if (!double.TryParse(szElementValue.Trim(), out dBodyWeight))
            //    return;

            //double dBmiValue = dBodyWeight / (dBodyHeight * dBodyHeight);
            //dBmiValue = Math.Round(dBmiValue, 2, MidpointRounding.ToEven);

            //double dAreaValue = 0.0061 * dBodyHeight * 100 + 0.0128 * dBodyWeight - 0.1529;
            //dAreaValue = Math.Round(dAreaValue, 2, MidpointRounding.ToEven);

            //string nameValueExpression = string.Format("{0}={1}|{2}={3}"
            //    , SystemConsts.ElementName.BODY_BMI, dBmiValue
            //    , SystemConsts.ElementName.BODY_AREA, dAreaValue);
            //this.m_activeDocumentForm.DocumentEditor.SetElementText(nameValueExpression, false);
        }

        /// <summary>
        /// �����ĵ��е���Ժ�ͳ�Ժʱ��ṹ��Ԫ��ֵ�Զ�������Ժ����
        /// </summary>
        /// <param name="szElementName">��ǰ���޸ĵ�Ԫ������</param>
        private void HandleInpDaysElement(string szElementName)
        {
            //if (this.m_activeDocumentForm == null || this.m_activeDocumentForm.IsDisposed)
            //    return;
            //if (GlobalMethods.Misc.IsEmptyString(szElementName))
            //    return;

            //Hashtable htElements = new Hashtable();
            //htElements.Add(SystemConsts.ElementName.ADMISSION_DATE, null);
            //htElements.Add(SystemConsts.ElementName.ADMISSION_TIME, null);
            //htElements.Add(SystemConsts.ElementName.DISCHARGE_DATE, null);
            //htElements.Add(SystemConsts.ElementName.DISCHARGE_TIME, null);
            //if (!this.GetRelatedElementsValue(ref htElements))
            //    return;

            //string szElementValue = htElements[SystemConsts.ElementName.ADMISSION_DATE] as string;
            //if (szElementValue == null)
            //    szElementValue = htElements[SystemConsts.ElementName.ADMISSION_TIME] as string;

            //if (GlobalMethods.Misc.IsEmptyString(szElementValue))
            //    return;

            //DateTime dtAdmissionDate = DateTime.Now;
            //if (!GlobalMethods.Convert.StringToDate(szElementValue, ref dtAdmissionDate))
            //    return;

            //szElementValue = htElements[SystemConsts.ElementName.DISCHARGE_DATE] as string;
            //if (szElementValue == null)
            //    szElementValue = htElements[SystemConsts.ElementName.DISCHARGE_TIME] as string;

            //if (GlobalMethods.Misc.IsEmptyString(szElementValue))
            //    return;

            //DateTime dtDischargeDate = DateTime.Now;
            //if (!GlobalMethods.Convert.StringToDate(szElementValue, ref dtDischargeDate))
            //    return;

            //long lInpDays = GlobalMethods.SysTime.GetInpDays(dtAdmissionDate, dtDischargeDate);
            //string nameValueExpression = string.Format("{0}={1}|{2}={3}"
            //    , SystemConsts.ElementName.IN_HOSPITAL_DAYS1, lInpDays
            //    , SystemConsts.ElementName.IN_HOSPITAL_DAYS2, lInpDays);
            //this.m_activeDocumentForm.DocumentEditor.SetElementText(nameValueExpression, false);
        }
        public bool CalcularTest(IElementCalculator instance,PatVisitInfo patVisitInfo,QcCheckPoint qcCheckPoint,QcCheckResult checkResult)
        {
            if (instance == null)
                return false;
            if (instance.GetElementValueCallback == null)
                instance.GetElementValueCallback = this.m_getElementValueCallback;
            if (instance.SetElementValueCallback == null)
                instance.SetElementValueCallback = this.m_setElementValueCallback;
            if (instance.ShowElementTipCallback == null)
                instance.ShowElementTipCallback = this.m_showElementTipCallback;
            if (instance.HideElementTipCallback == null)
                instance.HideElementTipCallback = this.m_hideElementTipCallback;
            if (instance.ExecuteQueryCallback == null)
                instance.ExecuteQueryCallback = this.m_executeQueryCallback;
            if (instance.ExecuteUpdateCallback == null)
                instance.ExecuteUpdateCallback = this.m_executeUpdateCallback;
            if (instance.GetSystemContextCallback == null)
                instance.GetSystemContextCallback = this.m_getSystemContextCallback;
            try
            {
                instance.Calculate(patVisitInfo, qcCheckPoint,checkResult);
                //if (!instance.Calculate(szElementName))
                //    return false;
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("Heren.MedDoc.AutoCalc.AutoCalcHandler.ExecuteElementCalculator", ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// ִ��ָ���������͵�Ԫ���Զ�����ű�DLL
        /// </summary>
        /// <param name="szExecuteTime">ִ��ʱ��</param>
        /// <param name="szDocTypeID">��������ID</param>
        /// <param name="szElementName">Ԫ�ر���</param>
        /// <returns>SystemData.ReturnValue</returns>
        public bool ExecuteElementCalculator(string szScriptID,PatVisitInfo patVisitInfo,QcCheckPoint checkPoint, QcCheckResult qcCheckResult)
        {
            List<IElementCalculator> calculatorInstances = null;
            short result = ScriptCache.Instance.GetScriptInstances(szScriptID, ref calculatorInstances);
            if (result != SystemData.ReturnValue.OK)
                return true;
            if (calculatorInstances == null)
                return true;
            for (int index = 0; index < calculatorInstances.Count; index++)
            {
                IElementCalculator instance = calculatorInstances[index];
                if (instance == null)
                    continue;
                if (instance.GetElementValueCallback == null)
                    instance.GetElementValueCallback = this.m_getElementValueCallback;
                if (instance.SetElementValueCallback == null)
                    instance.SetElementValueCallback = this.m_setElementValueCallback;
                if (instance.ShowElementTipCallback == null)
                    instance.ShowElementTipCallback = this.m_showElementTipCallback;
                if (instance.HideElementTipCallback == null)
                    instance.HideElementTipCallback = this.m_hideElementTipCallback;
                if (instance.ExecuteQueryCallback == null)
                    instance.ExecuteQueryCallback = this.m_executeQueryCallback;
                if (instance.ExecuteUpdateCallback == null)
                    instance.ExecuteUpdateCallback = this.m_executeUpdateCallback;
                if (instance.GetSystemContextCallback == null)
                    instance.GetSystemContextCallback = this.m_getSystemContextCallback;
                try
                {
                    instance.Calculate(patVisitInfo,checkPoint, qcCheckResult);
                    //if (!instance.Calculate(szElementName))
                    //    return false;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.WriteLog("Heren.MedDoc.AutoCalc.AutoCalcHandler.ExecuteElementCalculator", ex);
                    return false;
                }
            }
            return true;
        }
    }
}
