using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace HerenControls
{
    public class MultiMergeHeaderDataGridView : DataGridView
    {

        #region �ֶζ��x

        /// <summary>��S�И��}�Ę�Y��
        /// 
        /// </summary>
        private TreeView _ColHeaderTreeView;

        /// <summary>������Ӕ�
        /// 
        /// </summary>
        private int iNodeLevels;

        /// <summary>һ�S�И��}�ĸ߶�
        /// 
        /// </summary>
        private int iCellHeight;

        /// <summary>�����~���c
        /// 
        /// </summary>
        private IList<TreeNode> ColLists = new List<TreeNode>();

        #endregion

        #region ���Զ��x

        /// <summary>��S�И��}�Ę�Y��
        /// 
        /// </summary>
        [
          Description("��S�И��}�Ę�Y��")
        ]
        public TreeView myColHeaderTreeView
        {
            get { return _ColHeaderTreeView; }
            set { _ColHeaderTreeView = value; }
        }

        #endregion

        #region ��������

        /// <summary>�f�wӋ������Ӕ����K���������~���c
        /// 
        /// </summary>
        /// <param name="tnc"></param>
        /// <returns></returns>
        private int myGetNodeLevels(TreeNodeCollection tnc)
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
                    myGetNodeLevels(tn.Nodes);
                }
                else
                {
                    ColLists.Add(tn);//�~���c
                }
            }

            return iNodeLevels;
        }

        /// <summary>�{���f�w�����Ӕ������^����
        /// 
        /// </summary>
        public void myNodeLevels()
        {

            iNodeLevels = 1;//��ʼ��1

            ColLists.Clear();

            int iNodeDeep = myGetNodeLevels(_ColHeaderTreeView.Nodes);

            iCellHeight = this.ColumnHeadersHeight;

            this.ColumnHeadersHeight = this.ColumnHeadersHeight * iNodeDeep;//���^����=һ�S�и�*�Ӕ�

        }

        /// <summary>��úϲ������ֶεĿ��
        /// 
        /// </summary>
        /// <param name="node">�ֶνڵ�</param>
        /// <returns>�ֶο��</returns>
        private int GetUnitHeaderWidth(TreeNode node)
        {
            int uhWidth = 0;
            //�����ײ��ֶεĿ��
            if (node.Nodes == null)
                return this.Columns[GetColumnListNodeIndex(node)].Width;

            if (node.Nodes.Count == 0)
                return this.Columns[GetColumnListNodeIndex(node)].Width;

            //��÷���ײ��ֶεĿ��
            for (int i = 0; i <= node.Nodes.Count - 1; i++)
            {
                uhWidth = uhWidth + GetUnitHeaderWidth(node.Nodes[i]);
            }
            return uhWidth;
        }

        /// <summary>��õײ��ֶ�����
        /// 
        /// </summary>
        ///' <param name="node">�ײ��ֶνڵ�</param>
        /// <returns>����</returns>
        /// <remarks></remarks>
        private int GetColumnListNodeIndex(TreeNode node)
        {
            for (int i = 0; i <= ColLists.Count - 1; i++)
            {
                if (ColLists[i].Equals(node))
                    return i;
            }
            return -1;
        }

        ///<summary>
        ///���ƺϲ���ͷ
        ///</summary>
        ///<param name="node">�ϲ���ͷ�ڵ�</param>
        ///<param name="e">��ͼ������</param>
        ///<param name="level">������</param>
        ///<remarks></remarks>
        public void PaintUnitHeader(
                        TreeNode node,
                        System.Windows.Forms.DataGridViewCellPaintingEventArgs e,
                        int level)
        {
            //���ڵ�ʱ�˳��ݹ����
            if (level == 0)
                return;

            RectangleF uhRectangle;
            int uhWidth;
            SolidBrush gridBrush = new SolidBrush(this.GridColor);
            SolidBrush backColorBrush = new SolidBrush(e.CellStyle.BackColor);
            Pen gridLinePen = new Pen(gridBrush);
            StringFormat textFormat = new StringFormat();

            textFormat.Alignment = StringAlignment.Center;

            uhWidth = GetUnitHeaderWidth(node);

            //��ԭ���㷨�����������⡣
            if (node.Nodes.Count == 0)
            {
                uhRectangle = new Rectangle(e.CellBounds.Left,
                            e.CellBounds.Top + node.Level * iCellHeight,
                            uhWidth - 1,
                            iCellHeight * (iNodeLevels - node.Level) - 1);
            }
            else
            {
                uhRectangle = new Rectangle(
                            e.CellBounds.Left,
                            e.CellBounds.Top + node.Level * iCellHeight,
                            uhWidth - 1,
                            iCellHeight - 1);
            }

            //������
            e.Graphics.FillRectangle(backColorBrush, uhRectangle);

            //������
            e.Graphics.DrawLine(gridLinePen
                                , uhRectangle.Left
                                , uhRectangle.Bottom
                                , uhRectangle.Right
                                , uhRectangle.Bottom);
            //���Ҷ���
            e.Graphics.DrawLine(gridLinePen
                                , uhRectangle.Right
                                , uhRectangle.Top
                                , uhRectangle.Right
                                , uhRectangle.Bottom);

            e.Graphics.DrawString(node.Text, this.ColumnHeadersDefaultCellStyle.Font
                                    , Brushes.Black
                                    , uhRectangle.Left + uhRectangle.Width / 2 -
                                    e.Graphics.MeasureString(node.Text, this.ColumnHeadersDefaultCellStyle.Font).Width / 2 - 1
                                    , uhRectangle.Top +
                                    uhRectangle.Height / 2 - e.Graphics.MeasureString(node.Text, this.ColumnHeadersDefaultCellStyle.Font).Height / 2);

            //�ݹ����()
            if (node.PrevNode == null)
                if (node.Parent != null)
                    PaintUnitHeader(node.Parent, e, level - 1);
        }

        #endregion

        //�،����^
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                //�б��ⲻ��д
                if (e.ColumnIndex < 0)
                {
                    base.OnCellPainting(e);
                    return;
                }

                if (iNodeLevels == 1)
                {
                    base.OnCellPainting(e);
                    return;
                }

                //���Ʊ�ͷ
                if (e.RowIndex == -1)
                {
                    if (_ColHeaderTreeView != null)
                    {
                        if (iNodeLevels == 0)
                        {
                            myNodeLevels();
                        }

                        PaintUnitHeader((TreeNode)this.ColLists[e.ColumnIndex], e, iNodeLevels);

                        e.Handled = true;
                    }
                    else
                    {
                        base.OnCellPainting(e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error");
            }
        }
    }
}
