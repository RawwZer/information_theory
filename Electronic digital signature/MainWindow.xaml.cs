using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TI4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string selectedFilePath;
        private byte[] readedBytes;
        private int valueR;
        private int valueD;
        private int valueS;
        private int valeuNS;
        private int valueHash;
        private const int H0 = 100;

        private byte[] ReadingByte(string fileToRead)
        {
            FileInfo fileInfo = new FileInfo(fileToRead);
            long fileSizeInBytes = fileInfo.Length;
            byte[] readedBytes = new byte[fileSizeInBytes];

            using (FileStream fileStream = new FileStream(fileToRead, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(readedBytes, 0, (int)fileSizeInBytes); // not such large files in this task
            }

            return readedBytes;
        }

        private bool isPrimar(int value)
        {
            if (value % 2 == 0 || value % 3 == 0)
                return false;
            int i = 5;
            while (i * i <= value)
            {
                if (value % i == 0 || value % (i + 2) == 0)
                    return false;
                i += 6;
            }
            return true;
        }
        private bool GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a == 1;
        }

        public int ExtendedEuclidean(int a, int b)
        {
            int d0 = a;
            int d1 = b;
            int x0 = 1;
            int x1 = 0;
            int y0 = 0;
            int y1 = 1;

            while (d1 > 1)
            {
                int q = d0 / d1;
                int d2 = d0 % d1;
                int x2 = x0 - q * x1;
                int y2 = y0 - q * y1;
                d0 = d1;
                d1 = d2;
                x0 = x1;
                x1 = x2;
                y0 = y1;
                y1 = y2;
            }

            if (y1 < 0)
            {
                y1 += a;
            }

            return y1;
        }

        public int modPow(int a, int b, int m)
        {
            int result = 1;

            while (b > 0)
            {
                while (b % 2 == 0)
                {
                    a = (a * a) % m;
                    b >>= 1;
                }
                b--;
                result = (result * a) % m;
            }

            return result;
        }

        private int CountS(byte[] bytes)
        {

            if (bytes.Length == 0)
            {
                LError_S.Content = "Файл пуст!";
                LHash.Content = "Значение хэш-образа = " + H0.ToString();
                return H0;
            } else
            {
                LError_S.Content = "";
                int count = ((H0 + bytes[0]) * (H0 + bytes[0])) % valueR;

                for (int i = 1; i < bytes.Length; i++)
                {
                    count = ((count + bytes[i]) * (count + bytes[i])) % valueR;
                }
                LHash.Content = "Значение хэш-образа = " + count.ToString();
                return count;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите файл";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                readedBytes = ReadingByte(selectedFilePath);
                BtnCheck.IsEnabled = true;
                BtnWrite.IsEnabled = true;
                LError_S.Content = "";
            }
        }

        private void TB_P_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = "[0-9]";

            if (!Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = true;
            }
        }

        private void TB_P_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_P.Text.Length != 0 && int.Parse(TB_P.Text) < 65535)
            {
                if (isPrimar(int.Parse(TB_P.Text)))
                {
                    LError_P.Visibility = Visibility.Hidden;
                    TB_Q.IsEnabled = true;
                }
                else
                {
                    TB_Q.IsEnabled = false;
                    LError_P.Visibility = Visibility.Visible;
                    LError_P.Content = "Число не является простым!";
                }
            }
            else
            {
                TB_Q.IsEnabled = false;
                LError_P.Visibility = Visibility.Visible;
                LError_P.Content = "Число должно быть < 65535!";
            }
        }

        private void TB_Q_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Q.Text.Length != 0 && int.Parse(TB_Q.Text) < 65535)
            {
                if (isPrimar(int.Parse(TB_Q.Text)))
                {
                    LError_Q.Visibility = Visibility.Hidden;
                    valueR = int.Parse(TB_Q.Text) * int.Parse(TB_P.Text);
                    LValue_R.Content = "Значение r = P * Q = " + valueR.ToString();
                    LValue_FR.Content = "Значение ф(r) = (P - 1) * (Q - 1) = " + ((int.Parse(TB_Q.Text)-1) * (int.Parse(TB_P.Text)-1)).ToString();
                }
                else
                {
                    LError_Q.Visibility = Visibility.Visible;
                    LError_Q.Content = "Число не является простым!";
                    LValue_R.Content = "Значение r = P * Q = ";
                    LValue_FR.Content = "Значение ф(r) = (P - 1) * (Q - 1) = ";
                }
            }
            else
            {
                LError_Q.Visibility = Visibility.Visible;
                LError_Q.Content = "Число должно быть < 65535!";
                LValue_R.Content = "Значение r = P * Q = ";
                LValue_FR.Content = "Значение ф(r) = (P - 1) * (Q - 1) = ";
            }
        }

        private void TB_E_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_E.Text.Length != 0 && GCD(int.Parse(TB_E.Text), ((int.Parse(TB_Q.Text) - 1) * (int.Parse(TB_P.Text) - 1))))
            {
                LError_E.Visibility = Visibility.Hidden;
                valueD = ExtendedEuclidean(((int.Parse(TB_Q.Text) - 1) * (int.Parse(TB_P.Text) - 1)), int.Parse(TB_E.Text));
                LValue_D.Content = "Значение открытой экспоненты E = " + valueD.ToString();
                valueS = modPow(CountS(readedBytes), int.Parse(TB_E.Text), valueR);
                valueHash = CountS(readedBytes);
                LValue_S.Content = "S = M^D mod R = " + valueS.ToString();
                BtnRemember.IsEnabled = true;
            }
            else
            {
                BtnRemember.IsEnabled = false;
                LError_E.Visibility = Visibility.Visible;
                LError_E.Content = "Число не является взаимно простым с ф(R)!";
                LValue_D.Content = "Значение закрытой экспоненты D = ";
            }
        }

        private void BtnRemember_Click(object sender, RoutedEventArgs e)
        {
            LValue_NS.Content = "Фиксированное значение ЭЦП S = " + valueS.ToString();
            valeuNS = valueS;
        }

        private void BtnWrite_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(selectedFilePath, true))
            {
                writer.WriteLine("\n"+valeuNS.ToString());
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = File.ReadAllLines(selectedFilePath);
            byte[] bytesArray;
            if (lines.Length > 1)
            {
                string concatenatedString = string.Join(Environment.NewLine, lines.Take(lines.Length - 1));
                bytesArray = Encoding.Default.GetBytes(concatenatedString);
            } else
            {
                string concatenatedString = string.Join(Environment.NewLine, lines);
                bytesArray = Encoding.Default.GetBytes(concatenatedString);
            }

                    if (CountS(bytesArray) != valueHash)
                    {
                        LError_S.Content = "Подписи НЕ совпали! \n" + "Хеш-образ открытого файла: " + CountS(bytesArray).ToString() + "\n Хеш-образ необходимый: " + valueHash.ToString();
                    }
                    else
                    {
                        LError_S.Content = "Подписи совпали!";
                    }
        }
    }
}