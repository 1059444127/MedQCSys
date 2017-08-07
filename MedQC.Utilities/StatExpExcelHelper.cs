// ***********************************************************
// �ʿ�Excel������.
// Creator:TANGCHENG  Date:2011-07-19
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Heren.Common.Controls;
using Heren.Common.Libraries;

using EMRDBLib;
using EMRDBLib.DbAccess;

namespace Heren.MedQC.Utilities
{
    public class StatExpExcelHelper
    {
        private static StatExpExcelHelper m_Instance = null;
        /// <summary>
        /// ��ȡ��ʵ��
        /// </summary>
        public static StatExpExcelHelper Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new StatExpExcelHelper();
                return m_Instance;
            }
        }

        private Hashtable m_htNoExportColIndex = null;
        /// <summary>
        /// ��ȡ�����ò���Ҫ�������м���
        /// </summary>
        public Hashtable HtNoExportColIndex
        {
            get { return this.m_htNoExportColIndex; }
            set { this.m_htNoExportColIndex = value; }
        }

        /// <summary>
        /// ��DataGridView�е����ݵ�����Excel��
        /// </summary>
        /// <param name="dgv">Ҫ���е�����DataGridView</param>
        /// <param name="szFileTitle">�������ݱ��ı���</param>
        public void ExportToExcel(DataGridView dgv, string szFileTitle)
        {
            string szFileName = szFileTitle + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Excel����";
            saveDialog.DefaultExt = szFileName;
            saveDialog.FileName = szFileTitle;
            saveDialog.Filter = "Excel�ļ�|*.xls|�����ļ�|*.*";
            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;
            string szFilePath = saveDialog.FileName;

            if (ExcelExporter.ExportExcelFile(dgv, false, szFilePath))
            {
                if (!GlobalMethods.Win32.Execute(szFilePath, null))
                    MessageBoxEx.ShowMessage("�ѵ������!���޷���,������û�а�װExcel���!");
            }
            else
            {
                MessageBoxEx.ShowMessage("����ʧ��!");
            }
            //this.ExportTo(dgv, szFileTitle);
        }

        /// <summary>
        /// �������¼�����е�DataGridView�е����ݵ�����Excel��
        /// </summary>
        /// <param name="dgv">Ҫ���е����ļ����¼��DataGridView</param>
        /// <param name="dgvResultList">Ҫ��������ϸ������ϢDataGridView</param>
        /// <param name="szFileTitle">�������ݱ��ı���</param>
        public void ExportTestResultToExcel(DataGridView dgv, DataGridView dgvResultList, string szFileTitle)
        {
            if (dgv == null || dgv.Rows.Count == 0)
                return;
            int curColumnIndex = 1;     //��ǰ�����е�����
            int visibleColumnCount = 0;     //DataGridView�ɼ�����
            string szFileName = szFileTitle + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            System.Drawing.Font headFont = dgv.RowHeadersDefaultCellStyle.Font;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Excel����";
            saveDialog.DefaultExt = szFileName;
            saveDialog.FileName = szFileTitle + DateTime.Now.ToString("yyyyMMddHHmmss");
            saveDialog.Filter = "Excel�ļ�|*.xls|�����ļ�|*.*";
            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;
            string szFilePath = saveDialog.FileName;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (dgv.Columns[i].Visible != true || (!(dgv.Columns[i] is DataGridViewTextBoxColumn) && !(dgv.Columns[i] is DataGridViewLinkColumn)))
                    continue;
                if (this.m_htNoExportColIndex != null && this.m_htNoExportColIndex.Contains(i))
                    continue;
                visibleColumnCount++;
            }
            try
            {
                ApplicationClass Mylxls = new ApplicationClass();
                if (Mylxls == null)
                {
                    MessageBoxEx.Show("���ݵ���ʧ�ܣ���ȷ�����Ļ�����װ��Microsoft Office Excel��", MessageBoxIcon.Information);
                    return;
                }
                Mylxls.Application.Workbooks.Add(true);
                Mylxls.Cells.Font.Size = 10.5;
                Mylxls.Caption = szFileName;       //���ñ�ͷ
                Mylxls.Cells[1, 1] = szFileName;    //��ʾ��ͷ
                Mylxls.Cells.NumberFormatLocal = "@";//������λ����Ϊ0��������ʧ
                int nSpanCount = 0;
                int rowIndex = 2;            //������
                for (int j = 0; j < dgv.Rows.Count; j++)
                {
                    curColumnIndex = 1;
                    LabResult labTestInfo = dgv.Rows[j].Tag as LabResult;
                    if (labTestInfo == null)
                        continue;

                    List<LabResult> lstResultInfo = null;
                    short shRet =LabResultAccess.Instance.GetLabResultList(labTestInfo.TEST_ID, ref lstResultInfo);

                    if (shRet != SystemData.ReturnValue.OK
                        && shRet != SystemData.ReturnValue.RES_NO_FOUND)
                        continue;

                    if (lstResultInfo == null || lstResultInfo.Count <= 0)
                        continue;

                    //��excel�ĵڶ��п�ʼд�����ݣ������Ŀ��ͷռ���У������Ŀ�б����������,���ѭ����ȥֱ��������dgv�������С�
                    if (j > 0)
                        rowIndex = rowIndex + 2;
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (dgv.Columns[i].Visible != true || (!(dgv.Columns[i] is DataGridViewTextBoxColumn) && !(dgv.Columns[i] is DataGridViewLinkColumn)))
                            continue;
                        if (this.m_htNoExportColIndex != null && this.m_htNoExportColIndex.Contains(i))
                            continue;

                        //�����鵥���ơ��걾������ҽ����Ӧ������
                        if (i < 5)
                        {
                            Mylxls.Cells[rowIndex, curColumnIndex] = dgv.Columns[i].HeaderText;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex], Mylxls.Cells[rowIndex, curColumnIndex]).Cells.Borders.LineStyle = 1;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex], Mylxls.Cells[rowIndex, curColumnIndex]).Cells.Interior.ColorIndex = 37;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex], Mylxls.Cells[rowIndex, curColumnIndex]).Font.Bold = true;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex], Mylxls.Cells[rowIndex, curColumnIndex]).Font.Size = headFont.Size;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex], Mylxls.Cells[rowIndex, curColumnIndex]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                            Mylxls.Cells[rowIndex, curColumnIndex + 1] = dgv[i, j].Value.ToString();
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex + 1], Mylxls.Cells[rowIndex, curColumnIndex + 1]).Cells.Borders.LineStyle = 1;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex + 1], Mylxls.Cells[rowIndex, curColumnIndex + 1]).Cells.Interior.ColorIndex = 37;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, curColumnIndex + 1], Mylxls.Cells[rowIndex, curColumnIndex + 1]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        }
                        //���У���䱨��״̬���������ڡ������˶�Ӧ�����ݣ�curColumnIndex���¼�����
                        else
                        {
                            if (i == 5)
                                curColumnIndex = 1;
                            Mylxls.Cells[rowIndex + 1, curColumnIndex] = dgv.Columns[i].HeaderText;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex], Mylxls.Cells[rowIndex + 1, curColumnIndex]).Cells.Interior.ColorIndex = 37;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex], Mylxls.Cells[rowIndex + 1, curColumnIndex]).Cells.Borders.LineStyle = 1;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex], Mylxls.Cells[rowIndex + 1, curColumnIndex]).Font.Bold = true;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex], Mylxls.Cells[rowIndex + 1, curColumnIndex]).Font.Size = headFont.Size;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex], Mylxls.Cells[rowIndex + 1, curColumnIndex]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                            Mylxls.Cells[rowIndex + 1, curColumnIndex + 1] = dgv[i, j].Value.ToString();
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex + 1], Mylxls.Cells[rowIndex + 1, curColumnIndex + 1]).Cells.Interior.ColorIndex = 37;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex + 1], Mylxls.Cells[rowIndex + 1, curColumnIndex + 1]).Cells.Borders.LineStyle = 1;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex + 1, curColumnIndex + 1], Mylxls.Cells[rowIndex + 1, curColumnIndex + 1]).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                        }
                        curColumnIndex = curColumnIndex + 2;
                    }
                    //�������Ŀ�б�
                    this.BindResultList(dgvResultList, lstResultInfo);
                    for (int colIndex = 0; colIndex < dgvResultList.Columns.Count + 1; colIndex++)
                    {
                        //��excel���"���"�С�
                        if (colIndex == 0)
                            Mylxls.Cells[rowIndex + 2, colIndex + 1] = "���";
                        else
                            Mylxls.Cells[rowIndex + 2, colIndex + 1] = dgvResultList.Columns[colIndex - 1].HeaderText;
                        Mylxls.get_Range(Mylxls.Cells[rowIndex + 2, colIndex + 1], Mylxls.Cells[rowIndex + 2, colIndex + 1]).Cells.Borders.LineStyle = 1;
                        Mylxls.get_Range(Mylxls.Cells[rowIndex + 2, colIndex + 1], Mylxls.Cells[rowIndex + 2, colIndex + 1]).Font.Bold = true;
                        Mylxls.get_Range(Mylxls.Cells[rowIndex + 2, colIndex + 1], Mylxls.Cells[rowIndex + 2, colIndex + 1]).Font.Size = headFont.Size;
                        Mylxls.get_Range(Mylxls.Cells[rowIndex + 2, colIndex + 1], Mylxls.Cells[rowIndex + 2, colIndex + 1]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    }

                    rowIndex = rowIndex + 3;
                    for (int ii = 0; ii < lstResultInfo.Count; ii++)
                    {
                        for (int colIndex = 0; colIndex < dgvResultList.Columns.Count + 1; colIndex++)
                        {
                            if (colIndex == 0)
                                Mylxls.Cells[rowIndex, colIndex + 1] = ii + 1;
                            else
                                Mylxls.Cells[rowIndex, colIndex + 1] = (string)dgvResultList[colIndex - 1, ii].Value;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, colIndex + 1], Mylxls.Cells[rowIndex, colIndex + 1]).Cells.Borders.LineStyle = 1;
                            Mylxls.get_Range(Mylxls.Cells[rowIndex, colIndex + 1], Mylxls.Cells[rowIndex, colIndex + 1]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        }
                        rowIndex++;
                    }
                    dgvResultList.Rows.Clear();
                    nSpanCount++;
                }
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, visibleColumnCount]).MergeCells = true;
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, 1]).RowHeight = 30;
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, 1]).Font.Name = "����";
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, 1]).Font.Size = 14;
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, visibleColumnCount]).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                Workbook workBook = Mylxls.Workbooks.get_Item(1);
                if (workBook == null)
                {
                    MessageBoxEx.Show("���ݵ����������� �����ԣ�");
                    return;
                }
                workBook.Saved = true;
                workBook.SaveCopyAs(szFilePath);
                Mylxls.Workbooks.Close();
                Mylxls.Quit();
                Mylxls = null;
                MessageBoxEx.Show("����Excel�ɹ�!", MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("���ݵ���ʧ�ܣ���ȷ�����Ļ�����װ��Microsoft Office Excel��", MessageBoxIcon.Error);
                LogManager.Instance.WriteLog("StatExpExcelHelper.ExportTo", new string[] { "dgv", "szFileTitle" }
                    , new string[] { dgv.Name.ToString(), szFileTitle }, "���ݵ���ʧ�ܣ���ȷ�����Ļ�����װ��Microsoft Office Excel!\n" + ex.ToString());
            }
        }

        /// <summary>
        /// ����ѯ���ļ����Ŀ�б�󶨵�gridview��
        /// </summary>
        /// <param name="dgvResultList">gridview</param>
        /// <param name="lstResultInfo">�����Ŀ�б�</param>
        private void BindResultList(DataGridView dgvResultList, List<LabResult> lstResultInfo)
        {
            dgvResultList.Rows.Clear();
            for (int index = lstResultInfo.Count - 1; index >= 0; index--)
            {
                LabResult resultInfo = lstResultInfo[index];
                if (resultInfo == null)
                    continue;
                int nRowIndex = dgvResultList.Rows.Add();
                DataGridViewRow row = dgvResultList.Rows[nRowIndex];
                row.Cells[0].Value = resultInfo.ITEM_NAME;
                row.Cells[1].Value = resultInfo.ITEM_RESULT;
                row.Cells[2].Value = resultInfo.ITEM_UNITS;
                row.Cells[3].Value = resultInfo.ABNORMAL_INDICATOR;
                row.Cells[4].Value = resultInfo.ITEM_REFER;
            }
        }

        public void ExportTo(DataGridView dgv, string szFileTitle)
        {
            if (dgv == null || dgv.Rows.Count == 0)
                return;

            int curColumnIndex = 1;     //��ǰ�����е�����
            int visibleColumnCount = 0;     //DataGridView�ɼ�����
            string szFileName = szFileTitle + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            System.Drawing.Font headFont = dgv.RowHeadersDefaultCellStyle.Font;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Excel����";
            saveDialog.DefaultExt = szFileName;
            saveDialog.FileName = szFileTitle + DateTime.Now.ToString("yyyyMMddHHmmss");
            saveDialog.Filter = "Excel�ļ�|*.xls|�����ļ�|*.*";
            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;
            string szFilePath = saveDialog.FileName;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (dgv.Columns[i].Visible != true || (!(dgv.Columns[i] is DataGridViewTextBoxColumn) && !(dgv.Columns[i] is DataGridViewLinkColumn)))
                    continue;
                if (this.m_htNoExportColIndex != null && this.m_htNoExportColIndex.Contains(i))
                    continue;
                visibleColumnCount++;
            }

            try
            {
                ApplicationClass Mylxls = new ApplicationClass();
                if (Mylxls == null)
                {
                    MessageBoxEx.Show("���ݵ���ʧ�ܣ���ȷ�����Ļ�����װ��Microsoft Office Excel��", MessageBoxIcon.Information);
                    return;
                }
                Mylxls.Application.Workbooks.Add(true);
                Mylxls.Cells.Font.Size = 10.5;
                Mylxls.Caption = szFileName;       //���ñ�ͷ
                Mylxls.Cells[1, 1] = szFileName;    //��ʾ��ͷ
                Mylxls.Cells.NumberFormatLocal = "@";//������λ����Ϊ0��������ʧ

                //����������ͷ
                //�ж��Ƿ���MultiMergeHeaderDataGridView���������ô��ͷ���ܶ���

                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (dgv.Columns[i].HeaderText.IndexOf("��Ժ��ڶ��첡�̼�¼") > 0)
                    {

                    }
                    if (dgv.Columns[i].Visible != true || (!(dgv.Columns[i] is DataGridViewTextBoxColumn) && !(dgv.Columns[i] is DataGridViewLinkColumn)))
                        continue;
                    if (this.m_htNoExportColIndex != null && this.m_htNoExportColIndex.Contains(i))
                        continue;
                    
                    Mylxls.Cells[2, curColumnIndex] = dgv.Columns[i].HeaderText;
                    Mylxls.get_Range(Mylxls.Cells[2, curColumnIndex], Mylxls.Cells[2, curColumnIndex]).Cells.Borders.LineStyle = 1;
                    Mylxls.get_Range(Mylxls.Cells[2, curColumnIndex], Mylxls.Cells[2, curColumnIndex]).ColumnWidth = dgv.Columns[i].Width / 8;
                    Mylxls.get_Range(Mylxls.Cells[2, curColumnIndex], Mylxls.Cells[2, curColumnIndex]).Font.Bold = true;
                    Mylxls.get_Range(Mylxls.Cells[2, curColumnIndex], Mylxls.Cells[2, curColumnIndex]).Font.Size = headFont.Size;
                    Mylxls.get_Range(Mylxls.Cells[2, curColumnIndex], Mylxls.Cells[2, curColumnIndex]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    curColumnIndex++;
                }
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, visibleColumnCount]).MergeCells = true;
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, 1]).RowHeight = 30;
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, 1]).Font.Name = "����";
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, 1]).Font.Size = 14;
                Mylxls.get_Range(Mylxls.Cells[1, 1], Mylxls.Cells[1, visibleColumnCount]).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                object[,] dataArray = new object[dgv.Rows.Count, visibleColumnCount];
                //Exc�汾��ͬ��ʱ���ʽ�������п��ܻᱻ�ĳ�С��
                //��¼����Ϊʱ���ʽ����index���ڸ�ֵ�����ݺ��޸ĸ�ʽ
                Hashtable hsDateTime = new Hashtable();//��¼��indexΪʱ���ʽ������
                Hashtable hsDate = new Hashtable();//��¼��indexΪ���ڵ�����
                Hashtable hsUnShowColumns = new Hashtable();//����Ҫ��ʾ��������
                for (int i = 0; i < dgv.Rows.Count; i++)   //ѭ���������
                {
                    curColumnIndex = 1;
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        if (dgv.Columns[j].Visible != true || (!(dgv.Columns[j] is DataGridViewTextBoxColumn) && !(dgv.Columns[j] is DataGridViewLinkColumn)))
                        {
                            if (!hsUnShowColumns.ContainsKey(j))
                                hsUnShowColumns.Add(j, j);
                            continue;
                        }
                        if (this.m_htNoExportColIndex != null && this.m_htNoExportColIndex.Contains(j))
                        {
                            if (!hsUnShowColumns.ContainsKey(j))
                                hsUnShowColumns.Add(j, j);
                            continue;
                        }
                        if (dgv[j, i].Value != null)  //�����Ԫ�����ݲ�Ϊ��
                        {
                            dataArray[i, curColumnIndex - 1] = dgv[j, i].Value.ToString();
                            //��������Ϊ���ڵ��м�¼
                            DateTime defaultTime = new DateTime();
                            bool IsTimeValue = DateTime.TryParse(dgv[j, i].Value.ToString(), out defaultTime);
                            if (IsTimeValue && !hsDateTime.ContainsKey(j - hsUnShowColumns.Count))
                            {
                                hsDateTime.Add(j - hsUnShowColumns.Count, j - hsUnShowColumns.Count);//�ų�������
                                if (defaultTime.Hour == 0 && defaultTime.Minute == 0 && defaultTime.Second == 0)
                                {
                                    hsDate.Add(j - hsUnShowColumns.Count, j - hsUnShowColumns.Count);//�ų�������
                                }
                            }
                        }
                        curColumnIndex++;
                    }
                }

                Mylxls.get_Range(Mylxls.Cells[3, 1], Mylxls.Cells[dgv.Rows.Count + 2, visibleColumnCount]).Value2 = dataArray;          //���ñ߿�
                Mylxls.get_Range(Mylxls.Cells[3, 1], Mylxls.Cells[dgv.Rows.Count + 2, visibleColumnCount]).Cells.Borders.LineStyle = 1; //���ñ߿�
                if (hsDateTime != null && hsDateTime.Count > 0)//����ʱ���ʽ
                {
                    foreach (object obj in hsDateTime.Values)
                    {
                        int i = (int)obj;
                        if (i <= 0)
                            continue;
                        if (hsDate.ContainsKey(i))//������
                        {
                            Mylxls.get_Range(Mylxls.Cells[3, i + 1], Mylxls.Cells[dgv.Rows.Count + 2, i + 1]).NumberFormat = "yyyy/M/d";
                        }
                        else//������ʱ����
                        {
                            Mylxls.get_Range(Mylxls.Cells[3, i + 1], Mylxls.Cells[dgv.Rows.Count + 2, i + 1]).NumberFormat = "yyyy/M/d HH:mm:ss";
                        }
                    }
                }

                Workbook workBook = Mylxls.Workbooks.get_Item(1);
                if (workBook == null)
                {
                    MessageBoxEx.Show("���ݵ����������� �����ԣ�");
                    return;
                }
                workBook.Saved = true;
                workBook.SaveCopyAs(szFilePath);
                Mylxls.Workbooks.Close();
                Mylxls.Quit();
                Mylxls = null;
                if (!GlobalMethods.Win32.Execute(szFilePath, null))
                    MessageBoxEx.ShowMessage("�ѵ������!���޷���,������û�а�װExcel���!");
                //MessageBoxEx.Show("����Excel�ɹ�!", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("���ݵ���ʧ�ܣ���ȷ�����Ļ�����װ��Microsoft Office Excel��", MessageBoxIcon.Error);
                LogManager.Instance.WriteLog("StatExpExcelHelper.ExportTo", new string[] { "dgv", "szFileTitle" }
                    , new string[] { dgv.Name.ToString(), szFileTitle }, "���ݵ���ʧ�ܣ���ȷ�����Ļ�����װ��Microsoft Office Excel!\n" + ex.ToString());
            }
            finally
            {

            }
        }

        /// <summary>
        /// ��ȡNodes��߲㼶
        /// </summary>
        /// <param name="tnc"></param>
        /// <param name="iNodeLevels"></param>
        /// <returns></returns>
        public int GetNodeLevels(TreeNodeCollection tnc, ref int iNodeLevels)
        {
            if (tnc == null) return 0;

            foreach (TreeNode tn in tnc)
            {
                if ((tn.Level + 1) > iNodeLevels)//tn.Level�Ǐ�0�_ʼ��
                {
                    iNodeLevels = tn.Level + 1;
                }

                if (tn.Nodes.Count > 0)
                {
                    GetNodeLevels(tn.Nodes, ref iNodeLevels);
                }
            }

            return iNodeLevels;
        }

        private int GetSubNodeCount(TreeNode node, int count)
        {
            if (node.Nodes != null)
            {
                foreach (TreeNode nd in node.Nodes)
                {
                    count += GetSubNodeCount(nd, count);
                }
            }
            else
            {
                return 1;
            }
            return count;
        }

    }
}
