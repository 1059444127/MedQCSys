// ***********************************************************
// ���Ӳ���ϵͳ����Ԫ���Զ�����ű���������.
// Creator: YangMingkun  Date:2011-11-10
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Heren.Common.Libraries;

namespace Heren.MedQC.ScriptEngine.Script
{
    public class ScriptCompiler
    {
        private static ScriptCompiler m_Instance = null;
        /// <summary>
        /// ��ȡ�ű�����������
        /// </summary>
        public static ScriptCompiler Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ScriptCompiler();
                return m_Instance;
            }
        }
        private ScriptCompiler()
        {
        }

        private string m_workingPath = null;
        /// <summary>
        /// ��ȡ�����õ���������·��
        /// </summary>
        [Browsable(false)]
        public string WorkingPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_workingPath))
                    return this.GetWorkingPath();
                return this.m_workingPath;
            }
            set { this.m_workingPath = value; }
        }

        /// <summary>
        /// �õ�Common.ScriptEngine.dll����Ĺ���·��
        /// </summary>
        /// <returns>����·��</returns>
        private string GetWorkingPath()
        {
            string path = GlobalMethods.Misc.GetWorkingPath(this.GetType());
            try
            {
                if (!System.IO.File.Exists(path + "\\MedQC.ScriptEngine.dll"))
                    path = AppDomain.CurrentDomain.RelativeSearchPath;
            }
            catch { }
            return path;
        }

        private string m_cachePath = null;
        /// <summary>
        /// ��ȡ��������ļ����ػ���Ŀ¼
        /// </summary>
        private string CachePath
        {
            get
            {
                if (GlobalMethods.Misc.IsEmptyString(this.m_cachePath))
                    this.m_cachePath = this.WorkingPath + "\\Script\\Temp\\";
                return this.m_cachePath;
            }
        }

        /// <summary>
        /// �Զ���ӵĽű�ͷ��Imports��������
        /// </summary>
        private int m_nScriptHeaderLineCount = 0;

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <param name="outputFile">��������ļ�</param>
        /// <returns>CompilerParameters</returns>
        private CompilerParameters GetCompilerParameters(string outputFile)
        {
            CompilerParameters param = new CompilerParameters();
            param.GenerateExecutable = false;
            if (GlobalMethods.Misc.IsEmptyString(outputFile))
            {
                param.GenerateInMemory = true;
            }
            else
            {
                param.GenerateInMemory = false;
                param.OutputAssembly = outputFile;
            }

            param.WarningLevel = 0;
            param.TreatWarningsAsErrors = false;
            param.IncludeDebugInformation = false;
            param.CompilerOptions = "/platform:x86";

            param.ReferencedAssemblies.Add("System.dll");
            param.ReferencedAssemblies.Add("System.Data.dll");
            param.ReferencedAssemblies.Add("System.Xml.dll");
            param.ReferencedAssemblies.Add("System.Drawing.dll");
            param.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            param.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");

            string scriptEnginePath = this.WorkingPath + "\\MedQC.ScriptEngine.dll";
            if (!System.IO.File.Exists(scriptEnginePath))
                scriptEnginePath = GlobalMethods.Misc.GetWorkingPath() + "\\MedQC.ScriptEngine.dll";
            param.ReferencedAssemblies.Add(scriptEnginePath);

            scriptEnginePath = this.WorkingPath + "\\MedQC.Model.dll";
            if (!System.IO.File.Exists(scriptEnginePath))
                scriptEnginePath = GlobalMethods.Misc.GetWorkingPath() + "\\MedQC.Model.dll";
            param.ReferencedAssemblies.Add(scriptEnginePath);
            return param;
        }

        /// <summary>
        /// ��ȡ�ű������ļ��������ű�
        /// </summary>
        /// <param name="language">�ű�����</param>
        /// <returns>�������Խű�</returns>
        private string GetScriptAssemblyDescriptor(ScriptLanguage language)
        {
            if (language == ScriptLanguage.VBNET)
                return this.GetScriptAssemblyDescriptorVB();
            else
                return this.GetScriptAssemblyDescriptorCS();
        }

        /// <summary>
        /// ��ȡ�ű������ļ�������VB�ű�
        /// </summary>
        /// <returns>��������VB�ű�</returns>
        private string GetScriptAssemblyDescriptorVB()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Imports System.Reflection");
            builder.AppendLine("Imports System.Runtime.CompilerServices");
            builder.AppendLine("Imports System.Runtime.InteropServices");

            builder.AppendLine("<Assembly: AssemblyTitle(\"Heren.CalcScript\")>");
            builder.AppendLine("<Assembly: AssemblyDescription(\"Heren.CalcScript\")>");
            builder.AppendLine("<Assembly: AssemblyConfiguration(\"\")>");
            builder.AppendLine("<Assembly: AssemblyCompany(\"�㽭���ʿƼ�\")>");
            builder.AppendLine("<Assembly: AssemblyProduct(\"���ʵ��Ӳ���ϵͳ\")>");
            builder.AppendLine("<Assembly: AssemblyCopyright(\"��Ȩ���� (C) �㽭���ʿƼ� 2011\")>");
            builder.AppendLine("<Assembly: AssemblyTrademark(\"Heren Health\")>");
            builder.AppendLine("<Assembly: AssemblyCulture(\"\")>");

            string version = DateTime.Now.ToString("1.yy.MM.dd");
            builder.AppendLine(string.Format("<Assembly: AssemblyVersion(\"{0}\")>", version));
            builder.AppendLine(string.Format("<Assembly: AssemblyFileVersion(\"{0}\")>", version));
            return builder.ToString();
        }

        /// <summary>
        /// ��ȡ�ű������ļ�������C#�ű�
        /// </summary>
        /// <returns>��������C#�ű�</returns>
        private string GetScriptAssemblyDescriptorCS()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("using System.Runtime.CompilerServices;");
            builder.AppendLine("using System.Runtime.InteropServices;");

            builder.AppendLine("[assembly: AssemblyTitle(\"Heren.CalcScript\")]");
            builder.AppendLine("[assembly: AssemblyDescription(\"Heren.CalcScript\")]");
            builder.AppendLine("[assembly: AssemblyConfiguration(\"\")]");
            builder.AppendLine("[assembly: AssemblyCompany(\"�㽭���ʿƼ�\")]");
            builder.AppendLine("[assembly: AssemblyProduct(\"���ʵ��Ӳ���ϵͳ\")]");
            builder.AppendLine("[assembly: AssemblyCopyright(\"��Ȩ���� (C) �㽭���ʿƼ� 2011\")]");
            builder.AppendLine("[assembly: AssemblyTrademark(\"Heren Health\")]");
            builder.AppendLine("[assembly: AssemblyCulture(\"\")]");

            string version = DateTime.Now.ToString("1.yy.MM.dd");
            builder.AppendLine(string.Format("[assembly: AssemblyVersion(\"{0}\")]", version));
            builder.AppendLine(string.Format("[assembly: AssemblyFileVersion(\"{0}\")]", version));
            return builder.ToString();
        }

        /// <summary>
        /// ��ʽ�������ԭʼ�ű�,��Щ�ű����ǲ������ඨ���
        /// </summary>
        /// <param name="script">ԭʼ�ű�</param>
        /// <param name="language">�ű�����</param>
        /// <returns>��ʽ����Ľű�</returns>
        private string FormatScript(string script, ScriptLanguage language)
        {
            if (language == ScriptLanguage.VBNET)
                return this.FormatScriptVB(script);
            else
                return this.FormatScriptCS(script);
        }

        /// <summary>
        /// ��ʽ�������VBԭʼ�ű�,��Щ�ű����ǲ������ඨ���
        /// </summary>
        /// <param name="script">ԭʼVB�ű�</param>
        /// <returns>��ʽ����Ľű�</returns>
        private string FormatScriptVB(string script)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.AppendLine("Imports System");
            sbScript.AppendLine("Imports System.IO");
            sbScript.AppendLine("Imports System.Xml");
            sbScript.AppendLine("Imports System.Text");
            sbScript.AppendLine("Imports System.Collections");
            sbScript.AppendLine("Imports System.Collections.Generic");
            sbScript.AppendLine("Imports System.Drawing");
            sbScript.AppendLine("Imports System.Windows.Forms");
            sbScript.AppendLine("Imports Microsoft.VisualBasic");
            sbScript.AppendLine("Imports Heren.MedQC.ScriptEngine.Script");
            sbScript.AppendLine("Imports EMRDBLib");
            sbScript.AppendLine("Imports System.Data");
            sbScript.AppendLine();
            sbScript.AppendLine("Public Class DefaultElementCalculator");
            sbScript.AppendLine("    Inherits Heren.MedQC.ScriptEngine.Script.AbstractElementCalculator");

            //ע��:������漸��AppendLine���������б仯,�뼰ʱ�����������
            this.m_nScriptHeaderLineCount = 13;

            sbScript.AppendLine(script);
            sbScript.AppendLine("End Class");
            return sbScript.ToString();
        }

        /// <summary>
        /// ��ʽ�������C#ԭʼ�ű�,��Щ�ű����ǲ������ඨ���
        /// </summary>
        /// <param name="script">ԭʼC#�ű�</param>
        /// <returns>��ʽ����Ľű�</returns>
        private string FormatScriptCS(string script)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.AppendLine("using System;");
            sbScript.AppendLine("using System.IO;");
            sbScript.AppendLine("using System.Xml;");
            sbScript.AppendLine("using System.Text;");
            sbScript.AppendLine("using System.Collections;");
            sbScript.AppendLine("using System.Collections.Generic;");
            sbScript.AppendLine("using System.Drawing;");
            sbScript.AppendLine("using System.Windows.Forms;");
            sbScript.AppendLine("using Microsoft.VisualBasic;");
            sbScript.AppendLine("using Heren.Common.ScriptEngine.Script;");
            sbScript.AppendLine("using EMRDBLib;");
            sbScript.AppendLine("using System.Data;");
            sbScript.AppendLine();
            sbScript.AppendLine("public class DefaultElementCalculator");
            sbScript.AppendLine("    : Heren.Common.ScriptEngine.Script.AbstractElementCalculator");
            sbScript.AppendLine("{");

            //ע��:������漸��AppendLine���������б仯,�뼰ʱ�����������
            this.m_nScriptHeaderLineCount = 14;

            sbScript.AppendLine(script);
            sbScript.AppendLine("}");
            return sbScript.ToString();
        }

        /// <summary>
        /// ��.NET���õı���������ת��ΪScriptAgent�ı���������
        /// </summary>
        /// <param name="results">.NET���õı���������</param>
        /// <returns>ScriptAgent�ı���������(����Ϊnull)</returns>
        private CompileResults GetCompileResults(CompilerResults results)
        {
            CompileResults compileResults = new CompileResults();
            if (results == null)
                return compileResults;
            if (results.Errors != null && results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    if (error.IsWarning)
                        compileResults.HasWarnings = true;
                    else
                        compileResults.HasErrors = true;
                    error.Line -= this.m_nScriptHeaderLineCount;
                    compileResults.Errors.Add(new CompileError(error.FileName
                        , error.Line, error.Column, error.ErrorNumber, error.ErrorText, error.IsWarning));
                }
                return compileResults;
            }
            //�������ݵķ�ʽ���س���,��ֹ������ʱ�ļ���ռ�õ����޷����
            byte[] byteAssemblyData = AssemblyHelper.Instance.GetAssemblyData(results.PathToAssembly);
            compileResults.CompiledAssembly = AssemblyHelper.Instance.GetScriptAssembly(byteAssemblyData);
            return compileResults;
        }

        /// <summary>
        /// ����ָ���ű�Դ��,���ر�����
        /// </summary>
        /// <param name="source">Դ��</param>
        /// <param name="language">����</param>
        /// <returns>ScriptAgent�ı���������(����Ϊnull)</returns>
        public CompileResults CompileScript(string source, ScriptLanguage language)
        {
            return this.CompileScript(source, language, null);
        }

        /// <summary>
        /// ����ָ���ű�Դ��,���ر�����
        /// </summary>
        /// <param name="source">Դ��</param>
        /// <param name="language">����</param>
        /// <param name="outputFile">��������ļ�</param>
        /// <returns>ScriptAgent�ı���������(����Ϊnull)</returns>
        public CompileResults CompileScript(string source, ScriptLanguage language, string outputFile)
        {
            CodeDomProvider provider = null;
            switch (language)
            {
                case ScriptLanguage.VBNET:
                    provider = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                case ScriptLanguage.CSharp:
                    provider = new Microsoft.CSharp.CSharpCodeProvider();
                    break;
            }

            CompilerParameters param = this.GetCompilerParameters(outputFile);
            if (param == null)
                return new CompileResults();
            string szAssemblyDescriptor = this.GetScriptAssemblyDescriptor(language);
            CompilerResults results = null;
            try
            {
                source = this.FormatScript(source, language);
                results = provider.CompileAssemblyFromSource(param, szAssemblyDescriptor, source);
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog("ScriptCompiler.CompileScript", ex);
            }
            return this.GetCompileResults(results);
        }

        /// <summary>
        /// ����ָ���ű�Դ��,���ر�����
        /// </summary>
        /// <param name="scriptProperty">�ű�������Ϣ</param>
        /// <returns>ScriptAgent�ı���������(����Ϊnull)</returns>
        public CompileResults CompileScript(ScriptProperty scriptProperty)
        {
            if (scriptProperty == null)
                return new CompileResults();

            GlobalMethods.IO.CreateDirectory(this.CachePath);
            string szOutputFile = string.Format("{0}\\Calc.{1}.dll"
                , this.CachePath, Math.Abs(scriptProperty.ScriptText.GetHashCode()).ToString());

            CompileResults results = this.CompileScript(scriptProperty.ScriptText, scriptProperty.ScriptLang, szOutputFile);
            scriptProperty.ScriptData = AssemblyHelper.Instance.GetAssemblyData(szOutputFile);
            GlobalMethods.IO.DeleteFile(szOutputFile);
            return results;
        }
    }
}
