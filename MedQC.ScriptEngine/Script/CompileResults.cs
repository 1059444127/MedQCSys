// ***********************************************************
// ���Ӳ���ϵͳ����Ԫ���Զ�����ű���������������
// �����Խ�����,������.NET���õı����������,��Ҫ���ǵ����밲
// ȫ,��Ҫ��ScriptAgent.dll������������������йص�.NET�����
// ��������
// Creator : YangMingkun  Date : 2011-11-29
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Heren.Common.ScriptEngine.Script
{
    public class CompileResults
    {
        private CompileErrorCollection m_compileErrors = null;
        /// <summary>
        /// ��ȡ�����ñ�������еĴ���(����Ϊnull)
        /// </summary>
        public CompileErrorCollection Errors
        {
            get { return this.m_compileErrors; }
            set
            {
                if (value == null)
                    this.m_compileErrors.Clear();
                else
                    this.m_compileErrors = value;
            }
        }

        private bool m_hasErrors = false;
        /// <summary>
        /// ��ȡ�����Ƿ��д���
        /// </summary>
        public bool HasErrors
        {
            get { return this.m_hasErrors; }
            internal set { this.m_hasErrors = value; }
        }

        private bool m_hasWarnings = false;
        /// <summary>
        /// ��ȡ�����Ƿ��о���
        /// </summary>
        public bool HasWarnings
        {
            get { return this.m_hasWarnings; }
            internal set { this.m_hasWarnings = value; }
        }

        private Assembly m_compiledAssembly = null;
        /// <summary>
        /// ��ȡ�����ñ�������ĳ���
        /// </summary>
        public Assembly CompiledAssembly
        {
            get { return this.m_compiledAssembly; }
            set { this.m_compiledAssembly = value; }
        }

        private List<IElementCalculator> m_elementCalculators = null;
        /// <summary>
        /// ��ȡ�������ɵ�Ԫ�ؼ���������(����Ϊnull)
        /// </summary>
        public List<IElementCalculator> ElementCalculators
        {
            get
            {
                if (this.m_elementCalculators == null)
                    this.m_elementCalculators = AssemblyHelper.Instance.GetElementCalculator(this.m_compiledAssembly);
                return this.m_elementCalculators == null ?
                    new List<IElementCalculator>() : this.m_elementCalculators;
            }
        }

        public CompileResults()
        {
            this.m_compileErrors = new CompileErrorCollection();
        }
    }

    /// <summary>
    /// ������������Ϣ����
    /// </summary>
    [Serializable]
    public class CompileErrorCollection : List<CompileError>
    {
    }

    /// <summary>
    /// ������������Ϣ
    /// </summary>
    [Serializable]
    public class CompileError
    {
        private string m_fileName = string.Empty;
        /// <summary>
        /// ��ȡ�����ô����Ӧ��Դ�����ļ�
        /// </summary>
        public string FileName
        {
            get { return this.m_fileName; }
            set { this.m_fileName = value; }
        }

        private int m_line = 0;
        /// <summary>
        /// ��ȡ�����ô����Ӧ���к�
        /// </summary>
        public int Line
        {
            get { return this.m_line; }
            set { this.m_line = value; }
        }

        private int m_column = 0;
        /// <summary>
        /// ��ȡ�����ô����Ӧ���к�
        /// </summary>
        public int Column
        {
            get { return this.m_column; }
            set { this.m_column = value; }
        }

        private string m_errorNumber = string.Empty;
        /// <summary>
        /// ��ȡ�����ô����Ӧ�ı��
        /// </summary>
        public string ErrorNumber
        {
            get { return this.m_errorNumber; }
            set { this.m_errorNumber = value; }
        }

        private string m_errorText = string.Empty;
        /// <summary>
        /// ��ȡ�����ô����Ӧ���ı�
        /// </summary>
        public string ErrorText
        {
            get { return this.m_errorText; }
            set { this.m_errorText = value; }
        }

        private bool m_isWarning = false;
        /// <summary>
        /// ��ȡ�������Ƿ��Ǿ������
        /// </summary>
        public bool IsWarning
        {
            get { return this.m_isWarning; }
            set { this.m_isWarning = value; }
        }

        public CompileError()
        {
        }

        public CompileError(string fileName, int line, int column, string errorNumber, string errorText)
            : this(fileName, line, column, errorNumber, errorText, false)
        {
        }

        public CompileError(string fileName, int line, int column, string errorNumber, string errorText, bool isWarning)
        {
            this.m_fileName = fileName;
            this.m_line = line;
            this.m_column = column;
            this.m_errorNumber = errorNumber;
            this.m_errorText = errorText;
            this.m_isWarning = isWarning;
        }

        public override string ToString()
        {
            return string.Format("FileName={0}, Line={1}, Column={2}, ErrorText={3} "
                , this.m_fileName, this.m_line, this.m_column, this.m_errorText);
        }
    }
}
