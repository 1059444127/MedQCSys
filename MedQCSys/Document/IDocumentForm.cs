// ***********************************************************
// �ĵ����ڽӿڶ���.���ڹ淶���ܽ���ĸ��ֱ༭�ؼ��Ľӿ�.
// Creator:YangMingkun  Date:2010-10-14
// Copyright:supconhealth
// ***********************************************************
using System;
using System.Text;
using Heren.Common.Controls;
using Heren.Common.Libraries;
using EMRDBLib;

namespace MedQCSys.Document
{
    public interface IDocumentForm
    {
        #region"����"
        /// <summary>
        /// ��ȡ�������ĵ���Ϣ�б�,�����ǰ�ĵ���һϵ���ĵ��ϲ����
        /// </summary>
        MedDocList Documents { get; }

        /// <summary>
        /// ��ȡ�����õ�ǰ�ĵ���Ϣ
        /// </summary>
        MedDocInfo Document { get; }

        /// <summary>
        /// ��ȡ���������Ƿ��Ѿ����ͷ�
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// ��ȡ�ĵ��ı�������
        /// </summary>
        MedDocSys.PadWrapper.IMedEditor MedEditor { get; }

        /// <summary>
        /// ��ȡDockContent����Dock������
        /// </summary>
        Heren.Common.DockSuite.DockContentHandler DockHandler { get; }
        #endregion

        #region"����"
        /// <summary>
        /// ˢ�´��ڱ���
        /// </summary>
        /// <param name="szDocTitle">�ĵ���ʾ����</param>
        void RefreshFormTitle(string szDocTitle);

        /// <summary>
        /// ��ָ�����ĵ�
        /// </summary>
        /// <param name="document">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        short OpenDocument(MedDocInfo document);

        /// <summary>
        /// �ϲ���ָ����һϵ���ĵ�
        /// </summary>
        /// <param name="documents">�ĵ���Ϣ</param>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        short OpenDocument(MedDocList documents);

        /// <summary>
        /// �ʿ���Ա����ʷ����
        /// </summary>
        /// <returns>DataLayer.SystemData.ReturnValue</returns>
        short OpenHistoryDocument(MedicalQcMsg questionInfo);
        #endregion
    }
}