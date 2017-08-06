// ***********************************************************
// ���Ӳ���ϵͳ����Ԫ���Զ�����ű�������򼯸�����.
// ��Ҫ����ӳ����н������̳���Ԫ�ؼ������ӿڵĶ���
// Creator: YangMingkun Date:2011-11-10
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Heren.Common.Libraries;

namespace Heren.MedQC.ScriptEngine.Script
{
    public class AssemblyHelper
    {
        private static AssemblyHelper m_Instance = null;
        /// <summary>
        /// ��ȡ����ʵ�������൥ʵ��
        /// </summary>
        public static AssemblyHelper Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new AssemblyHelper();
                return m_Instance;
            }
        }
        private AssemblyHelper()
        {
        }

        /// <summary>
        /// ��ִ�г����ļ����س���
        /// </summary>
        /// <param name="assemblyFile">�����ļ�</param>
        /// <returns>Assembly</returns>
        public Assembly GetScriptAssembly(string assemblyFile)
        {
            try
            {
                return Assembly.LoadFile(assemblyFile);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("AssemblyHelper.GetScriptAssembly", ex);
                return null;
            }
        }

        /// <summary>
        /// ��ִ�г��������ݼ��س���
        /// </summary>
        /// <param name="assemblyRawData">����������</param>
        /// <returns>Assembly</returns>
        public Assembly GetScriptAssembly(byte[] assemblyRawData)
        {
            try
            {
                return Assembly.Load(assemblyRawData);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("AssemblyHelper.GetScriptAssembly", ex);
                return null;
            }
        }

        /// <summary>
        /// ��ȡָ�������ļ��Ķ���������
        /// </summary>
        /// <param name="assemblyFile">�����ļ�</param>
        /// <returns>byte[]</returns>
        public byte[] GetAssemblyData(string assemblyFile)
        {
            byte[] byteScriptData = null;
            if (!GlobalMethods.IO.GetFileBytes(assemblyFile, ref byteScriptData))
                return new byte[0];
            return byteScriptData;
        }

        /// <summary>
        /// ��ָ���ļ���ű������ļ��л�ȡ����ӿ��б�
        /// </summary>
        /// <param name="assemblyFile">����ű������ļ�</param>
        /// <returns>����ӿ��б�</returns>
        public List<IElementCalculator> GetElementCalculator(string assemblyFile)
        {
            return this.GetElementCalculator(this.GetScriptAssembly(assemblyFile));
        }

        /// <summary>
        /// ��ָ���ļ���ű������л�ȡ����ӿ��б�
        /// </summary>
        /// <param name="assemblyFile">����ű������ļ�</param>
        /// <returns>����ӿ��б�</returns>
        public List<IElementCalculator> GetElementCalculator(Assembly scriptAssembly)
        {
            List<IElementCalculator> elementCalculators = new List<IElementCalculator>();
            if (scriptAssembly == null)
                return elementCalculators;

            Type[] types = scriptAssembly.GetExportedTypes();
            if (types == null)
                return elementCalculators;
            foreach (Type type in types)
            {
                try
                {
                    Type interfaceClass = type.GetInterface(typeof(IElementCalculator).FullName);
                    if (interfaceClass == typeof(IElementCalculator))
                        elementCalculators.Add(scriptAssembly.CreateInstance(type.FullName) as IElementCalculator);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.WriteLog("AssemblyHelper.GetElementCalculator", ex);
                }
            }
            return elementCalculators;
        }
    }
}
