using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Heren.Common.Libraries;
using System.Drawing.Imaging;

namespace Heren.MedQC.QuestionChat.Utilities
{
    public class ImageAccess
    {
        public Image ResourceImage;
        private int ImageWidth;
        private int ImageHeight;
        public string ErrMessage;

        /// <summary>
        /// ��Ĺ��캯��
        /// </summary>
        /// <param name="ImageFileName">ͼƬ�ļ���ȫ·������</param>
        public ImageAccess()
        {
            
        }
        private static ImageAccess m_Instance = null;
        public static ImageAccess Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ImageAccess();
                }
                return m_Instance;
            }
        }
        /// <summary>
        /// ��Ĺ��캯��
        /// </summary>
        /// <param name="ImageFileName">ͼƬ�ļ���ȫ·������</param>
        public ImageAccess(string ImageFileName)
        {
            ResourceImage = Image.FromFile(ImageFileName);
            ErrMessage = "";
        }
        public bool ThumbnailCallback()
        {
            return false;
        }
        /// <summary>
        /// ��������ͼ���ط���1����������ͼ��Image����
        /// </summary>
        /// <param name="Width">����ͼ�Ŀ��</param>
        /// <param name="Height">����ͼ�ĸ߶�</param>
        /// <returns>����ͼ��Image����</returns>
        public Image GetReducedImage(int Width, int Height)
        {
            try
            {
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);

                return ReducedImage;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return null;
            }
        }
        /// <summary>
        /// ��������ͼ���ط���2��������ͼ�ļ����浽ָ����·��
        /// </summary>
        /// <param name="Width">����ͼ�Ŀ��</param>
        /// <param name="Height">����ͼ�ĸ߶�</param>
        /// <param name="targetFilePath">����ͼ�����ȫ�ļ�����(��·��)��������ʽ��D:\Images\filename.jpg</param>
        /// <returns>�ɹ�����true�����򷵻�false</returns>
        public bool GetReducedImage(int Width, int Height, string targetFilePath)
        {
            try
            {
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);
                ReducedImage.Dispose();

                return true;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return false;
            }
        }
        /// <summary>
        /// ��������ͼ���ط���3����������ͼ��Image����
        /// </summary>
        /// <param name="Percent">����ͼ�Ŀ�Ȱٷֱ� �磺��Ҫ�ٷ�֮80������0.8</param>  
        /// <returns>����ͼ��Image����</returns>
        public Image GetReducedImage(double Percent)
        {
            try
            {
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ImageWidth = Convert.ToInt32(ResourceImage.Width * Percent);
                ImageHeight = Convert.ToInt32(ResourceImage.Width * Percent);

                ReducedImage = ResourceImage.GetThumbnailImage(ImageWidth, ImageHeight, callb, IntPtr.Zero);

                return ReducedImage;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return null;
            }
        }
        /// <summary>
        /// ��������ͼ���ط���4����������ͼ��Image����
        /// </summary>
        /// <param name="Percent">����ͼ�Ŀ�Ȱٷֱ� �磺��Ҫ�ٷ�֮80������0.8</param>  
        /// <param name="targetFilePath">����ͼ�����ȫ�ļ�����(��·��)��������ʽ��D:\Images\filename.jpg</param>
        /// <returns>�ɹ�����true,���򷵻�false</returns>
        public bool GetReducedImage(double Percent, string targetFilePath)
        {
            try
            {
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ImageWidth = Convert.ToInt32(ResourceImage.Width * Percent);
                ImageHeight = Convert.ToInt32(ResourceImage.Width * Percent);

                ReducedImage = ResourceImage.GetThumbnailImage(ImageWidth, ImageHeight, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);

                ReducedImage.Dispose();

                return true;
            }
            catch (Exception e)
            {
                ErrMessage = e.Message;
                return false;
            }
        }
        /// <summary>
        /// byte[]תImage
        /// </summary>
        /// <param name="Buffer">��Ҫת����byte[]</param>
        /// <returns>Image�ļ�</returns>
        public Bitmap BufferToImage(byte[] Buffer)
        {
            if (Buffer == null || Buffer.Length == 0) { return null; }
            byte[] data = null;
            Image oImage = null;
            Bitmap oBitmap = null;
            data = (byte[])Buffer.Clone();
            try
            {
                MemoryStream oMemoryStream = new MemoryStream(Buffer);
                oMemoryStream.Position = 0;

                oImage = System.Drawing.Image.FromStream(oMemoryStream);
                oBitmap = new Bitmap(oImage);
            }
            catch (Exception err)
            {
                MessageBoxEx.Show("ͼƬ��ʽת�������г���");
            }
            return oBitmap;
        }

        /// <summary>
        /// Imageתbyte[]
        /// </summary>
        /// <param name="Image">��Ҫת����Image</param>
        /// <param name="imageFormat">��Ҫת����Image�ļ���ʽ</param>
        /// <returns>byte[]</returns>
        public byte[] ImageToBuffer(Bitmap Image, System.Drawing.Imaging.ImageFormat imageFormat)
        {

            if (Image == null) { return null; }

            byte[] data = null;

            using (MemoryStream oMemoryStream = new MemoryStream())
            {
                using (Bitmap oBitmap = new Bitmap(Image))
                {
                    oBitmap.Save(oMemoryStream, imageFormat);
                    oMemoryStream.Position = 0;
                    data = new byte[oMemoryStream.Length];
                    oMemoryStream.Read(data, 0, Convert.ToInt32(oMemoryStream.Length));
                    oMemoryStream.Flush();
                }
            }
            return data;
        }


    }
}
