using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Heren.MedQC.Test
{
    class Public_class
    {
        static string sex;
        public static string Fsex(int flag)
        {

            switch (flag)
            {
                case 0:
                    sex = "δ֪";
                    break;
                case 1:
                    sex = "��";
                    break;
                case 2:
                    sex = "Ů";
                    break;
                case 9:
                    sex = "δ˵��";
                    break;
                default:
                    MessageBox.Show("�Ա����!");
                    sex = "����";
                    break;
            }
            return sex;
        }

        static string minzi;
        public static string Fminzu(int flag)
        {

            switch (flag)
            {
                case 1:
                    minzi = "��";
                    break;
                case 2:
                    minzi = "�ɹ�";
                    break;
                case 3:
                    minzi = "��";
                    break;
                case 4:
                    minzi = "��";
                    break;
                case 5:
                    minzi = "ά���";
                    break;
                case 6:
                    minzi = "��";
                    break;
                case 7:
                    minzi = "��";
                    break;
                case 8:
                    minzi = "׳";
                    break;
                case 9:
                    minzi = "����";
                    break;
                case 10:
                    minzi = "����";
                    break;
                case 11:
                    minzi = "��";
                    break;
                case 12:
                    minzi = "��";
                    break;
                case 13:
                    minzi = "��";
                    break;
                case 14:
                    minzi = "��";
                    break;
                case 15:
                    minzi = "����";
                    break;
                case 16:
                    minzi = "����";
                    break;
                case 17:
                    minzi = "������";
                    break;
                case 18:
                    minzi = "��";
                    break;
                case 19:
                    minzi = "��";
                    break;
                case 20:
                    minzi = "����";
                    break;
                case 21:
                    minzi = "��";
                    break;
                case 22:
                    minzi = "�";
                    break;
                case 23:
                    minzi = "��ɽ";
                    break;
                case 24:
                    minzi = "����";
                    break;
                case 25:
                    minzi = "ˮ";
                    break;
                case 26:
                    minzi = "����";
                    break;
                case 27:
                    minzi = "����";
                    break;
                case 28:
                    minzi = "����";
                    break;
                case 29:
                    minzi = "�¶�����";
                    break;
                case 30:
                    minzi = "��";
                    break;
                case 31:
                    minzi = "���Ӷ�";
                    break;
                case 32:
                    minzi = "����";
                    break;
                case 33:
                    minzi = "Ǽ";
                    break;
                case 34:
                    minzi = "����";
                    break;
                case 35:
                    minzi = "����";
                    break;
                case 36:
                    minzi = "ë��";
                    break;
                case 37:
                    minzi = "����";
                    break;
                case 38:
                    minzi = "����";
                    break;
                case 39:
                    minzi = "����";
                    break;
                case 40:
                    minzi = "����";
                    break;
                case 41:
                    minzi = "������";
                    break;
                case 42:
                    minzi = "ŭ";
                    break;
                case 43:
                    minzi = "���α��";
                    break;
                case 44:
                    minzi = "����˹";
                    break;
                case 45:
                    minzi = "���¿�";
                    break;
                case 46:
                    minzi = "�°�";
                    break;
                case 47:
                    minzi = "����";
                    break;
                case 48:
                    minzi = "ԣ��";
                    break;
                case 49:
                    minzi = "��";
                    break;
                case 50:
                    minzi = "������";
                    break;
                case 51:
                    minzi = "����";
                    break;
                case 52:
                    minzi = "���״�";
                    break;
                case 53:
                    minzi = "����";
                    break;
                case 54:
                    minzi = "�Ű�";
                    break;
                case 55:
                    minzi = "���";
                    break;
                case 56:
                    minzi = "��ŵ";
                    break;
                default:
                    MessageBox.Show("�������!");
                    minzi = "����";
                    break;
            }
            return minzi;
        }


        public static long Getzplength()
        {
            long bmpend = 0;
            try
            {
                using (FileStream zpFile = new FileStream("D:\\zp.bmp", FileMode.Open))
                {
                    long bmpbegin = zpFile.Seek(0, SeekOrigin.Begin);
                    bmpend = zpFile.Seek(0, SeekOrigin.End);
                    byte[] bmp = new byte[bmpend];
                    zpFile.Seek(0, SeekOrigin.Begin);

                    zpFile.Close();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return -1;
            }
            
            return bmpend;
        }
    }
}
