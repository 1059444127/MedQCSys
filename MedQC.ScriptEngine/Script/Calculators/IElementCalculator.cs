// ***********************************************************
// ���Ӳ���ϵͳ����Ԫ���Զ�����ű��ӿ�.
// Creator: YangMingkun  Date:2011-11-10
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using EMRDBLib;

namespace Heren.MedQC.ScriptEngine.Script
{
    public interface IElementCalculator : IDisposable
    {
        /// <summary>
        /// ��ȡ��������ʾԪ����ʾ�Ļص�ί��
        /// </summary>
        ShowElementTipCallback ShowElementTipCallback { get; set; }
        /// <summary>
        /// ��ȡ����������Ԫ����ʾ�Ļص�ί��
        /// </summary>
        HideElementTipCallback HideElementTipCallback { get; set; }
        /// <summary>
        /// ��ȡ�����û�ȡԪ��ֵ�Ļص�ί��
        /// </summary>
        GetElementValueCallback GetElementValueCallback { get; set; }
        /// <summary>
        /// ��ȡ����������Ԫ��ֵ�Ļص�ί��
        /// </summary>
        SetElementValueCallback SetElementValueCallback { get; set; }
        /// <summary>
        /// ��ȡ������ί�����̵߳��õĻص�ί��
        /// </summary>
        ThreadInvokeCallback ThreadInvokeCallback { get; set; }
        /// <summary>
        /// ��ȡ�����û�ȡϵͳ�������������ݵĻص�ί��
        /// </summary>
        GetSystemContextCallback GetSystemContextCallback { get; set; }
        /// <summary>
        /// ��ȡ������ִ��ָ����SQL��ѯ�Ļص�ί��
        /// </summary>
        ExecuteQueryCallback ExecuteQueryCallback { get; set; }
        /// <summary>
        /// ��ȡ������ָ��ָ����SQL���µĻص�ί��
        /// </summary>
        ExecuteUpdateCallback ExecuteUpdateCallback { get; set; }
        /// <summary>
        /// ��ȡ�������Զ����¼��Ļص�ί��
        /// </summary>
        CustomEventCallback CustomEventCallback { get; set; }
        /// <summary>
        /// ��ȡ�����ö�λ��ָ��Ԫ��λ�õĻص�ί��
        /// </summary>
        LocateToElementCallback LocateToElementCallback { get; set; }
        /// <summary>
        /// �ṩ��������д�����㷽��
        /// </summary>
        /// <param name="szElementName">Ԫ������</param>
        void Calculate(object patVisitInfo, object checkPoint, object checkresult);
        /// <summary>
        /// �ṩ��������д�����㷽��
        /// </summary>
        /// <param name="param">��������</param>
        /// <param name="data">��������</param>
        bool Calculate(string param, object data);
        /// <summary>
        /// �ṩ��������д�����㷽��
        /// </summary>
        /// <param name="param">��������</param>
        /// <param name="data">��������</param>
        bool Calculate(string param);

    }

    /// <summary>
    /// ���ص�ǰԪ����ʾ�Ļص�ί��
    /// </summary>
    /// <returns>ִ�н��</returns>
    public delegate bool HideElementTipCallback();
    /// <summary>
    /// ��ʾ��ǰԪ����ʾ�Ļص�ί��
    /// </summary>
    /// <param name="szTitle">��ʾ����</param>
    /// <param name="szTipText">��ʾ�ı�</param>
    /// <returns>ִ�н��</returns>
    public delegate bool ShowElementTipCallback(string szTitle, string szTipText);
    /// <summary>
    /// ��ȡָ��Ԫ�ص�ֵ�Ļص�ί��
    /// </summary>
    /// <param name="szName">Ԫ������</param>
    /// <param name="szValue">���ص�Ԫ��ֵ</param>
    /// <returns>ִ�н��</returns>
    public delegate bool GetElementValueCallback(QcCheckPoint qcCheckPoint,PatVisitInfo patVisitInfo, string szElementName, out string szElementValue);
    /// <summary>
    /// ����ָ��Ԫ�ص�ֵ�Ļص�ί��
    /// </summary>
    /// <param name="szName">Ԫ������</param>
    /// <param name="szValue">Ԫ��ֵ</param>
    /// <returns>ִ�н��</returns>
    public delegate bool SetElementValueCallback(string szElementName, string szElementValue);
    /// <summary>
    /// ��궨λ��ָ��Ԫ��λ�ûص�ί��
    /// </summary>
    ///<param name="elementName">Ԫ������</param>
    public delegate bool LocateToElementCallback(string elementName);
    /// <summary>
    /// ί�����̵߳��õ�ί��
    /// </summary>
    /// <param name="method">ί�з���</param>
    /// <param name="args">����</param>
    /// <returns>���ز���</returns>
    public delegate object ThreadInvokeCallback(Delegate method, params object[] args);
    /// <summary>
    /// ִ��ָ����SQL��ѯ
    /// </summary>
    /// <param name="sql">SQL���</param>
    /// <returns>��ѯ�����</returns>
    public delegate bool ExecuteQueryCallback(string sql, out DataSet data);
    /// <summary>
    /// ִ��ָ����SQL����
    /// </summary>
    /// <param name="isProc">�����SQL�Ƿ��Ǵ洢����</param>
    /// <param name="sql">SQL���</param>
    /// <returns>����������</returns>
    public delegate bool ExecuteUpdateCallback(bool isProc, params string[] sql);
    /// <summary>
    /// ��ȡϵͳ�������������ݵĻص�ί��
    /// </summary>
    /// <param name="szContextName">����������</param>
    /// <param name="objContextValue">����������</param>
    /// <returns>ִ�н��</returns>
    public delegate bool GetSystemContextCallback(string szContextName, out object objContextValue);
    /// <summary>
    /// �Զ����¼��ص�ί��
    /// </summary>
    /// <param name="sender">������</param>
    /// <param name="name">�¼�����</param>
    /// <param name="param">�¼�����</param>
    /// <param name="data">�¼�����</param>
    /// <param name="result">�¼����</param>
    public delegate bool CustomEventCallback(object sender, string name, object param, object data, ref object result);
}
