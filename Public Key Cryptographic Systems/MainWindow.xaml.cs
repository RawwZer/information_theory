using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TI3
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

        private int addToK;
        private string selectedFilePath;
        private byte[] readedBytes;
        private bool outputPow = false;
        private int amount = 0; 

        private List<int> findPrimeDivisors(int value)
        {
            List<int> primeDivisors = new List<int>();
            for (int i = 2; i * i <= value; i++)
            {
                if (value % i == 0)
                {
                    if (isPrimar(i))
                    {
                        primeDivisors.Add(i);
                    }

                    if (isPrimar(value / i))
                    {
                        primeDivisors.Add(value / i);
                    }
                }
            }

            return primeDivisors;
        }

        public int modPow(int a, int b, int m)
        {
            if (outputPow)
            {
                TBlckPow.Text = a.ToString() + "^" + b.ToString();
            }

            int result = 1;

            while (b > 0)
            {
                while (b % 2 == 0)
                {
                    a = (a * a) % m;
                    b >>= 1;
                    if (outputPow)
                    {
                        TBlckPow.Text += " = " + a.ToString() + "^" + b.ToString() + " * " + result.ToString() + " mod " + m.ToString();
                    }
                }
                b--;
                result = (result*a) % m;
                if(outputPow)
                {
                    if (b != 0)
                    {
                        TBlckPow.Text += " = " + result.ToString() + " * " + a.ToString() + "^" + b.ToString() + " mod " + m.ToString();
                    } else
                    {
                        TBlckPow.Text += " = " + result.ToString();
                    }
                }
            }
             

            return result;
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

        private bool isPrimitiveSqrt(int value, int module, List<int> primeDivisors) 
        {
            int i = 0;
            bool isPrimitive = true;
            while (i < primeDivisors.Count && isPrimitive)
            {
                if (modPow(value, (module - 1) / primeDivisors[i], module) == 1)
                {
                    isPrimitive = false;
                }
                i++;
            }

            return isPrimitive;
        }

        private void findPrimitiveSqrt(int value)
        {
            for (int i = 2; i < value - 1; i++)
            {
                if (!GCD(value, i) || modPow(i, (value-1)/2, value) == 1)
                {
                    continue;
                }


                if (isPrimitiveSqrt(i, value, findPrimeDivisors(value-1)))
                {
                    CBPrimitiveSqrt.Items.Add(i);
                    amount++;
                };
            }
        }

        private void TBI_P_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBI_P.Text.Length != 0 && int.Parse(TBI_P.Text) > 255 && int.Parse(TBI_P.Text) < 65535) {
                if (isPrimar(int.Parse(TBI_P.Text)))
                {
                    CBPrimitiveSqrt.IsEnabled = true;
                    TBI_X.IsEnabled = true;
                    amount = 0;
                    findPrimitiveSqrt(int.Parse(TBI_P.Text));
                    L_Amount.Content = "Количество: " + amount.ToString();
                    LErrorP.Visibility = Visibility.Hidden;
                } else
                {
                    TBI_X.IsEnabled = false;
                    CBPrimitiveSqrt.IsEnabled = false;
                    CBPrimitiveSqrt.Items.Clear();
                    LErrorP.Visibility = Visibility.Visible;
                    LErrorP.Content = "Число не является простым!";
                }
            } else
            {
                TBI_X.IsEnabled = false;
                CBPrimitiveSqrt.IsEnabled = false;
                LErrorP.Visibility = Visibility.Visible;
                LErrorP.Content = "Число должно быть > 255 и < 65535!";
            }
        }

        private void TBI_P_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = "[0-9]";

            if (!Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = true;
            }
        }

        private void TBI_X_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBI_X.Text.Length != 0 && int.Parse(TBI_X.Text) > 1)
            {
                if (int.Parse(TBI_P.Text) - 1 > int.Parse(TBI_X.Text))
                {
                    TBI_K.IsEnabled = true;
                    BtnDecipher.IsEnabled = true;
                    outputPow = true;
                    L_Y.Content = modPow(int.Parse(CBPrimitiveSqrt.SelectedItem.ToString()), int.Parse(TBI_X.Text), int.Parse(TBI_P.Text));
                    outputPow = false;
                    LErrorX.Visibility = Visibility.Hidden;
                }
                else
                {   TBI_K.IsEnabled = false;
                    LErrorX.Visibility = Visibility.Visible;
                    LErrorX.Content = "Число должно быть < P-1!";
                }
            }
            else
            {   TBI_K.IsEnabled = false;
                LErrorX.Visibility = Visibility.Visible;
                LErrorX.Content = "Число должно быть > 1!";
            }
        }

        private void TBI_K_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBI_K.Text.Length != 0 && int.Parse(TBI_K.Text) > 0)
            {
                if (int.Parse(TBI_P.Text) - 1 > int.Parse(TBI_K.Text) && GCD(int.Parse(TBI_K.Text), int.Parse(TBI_P.Text) - 1))
                {
                    BtnCipher.IsEnabled = true;
                    LErrorK.Visibility = Visibility.Hidden;
                }
                else
                {
                    BtnCipher.IsEnabled = false;   
                    LErrorK.Visibility = Visibility.Visible;
                    LErrorK.Text = "Число должно быть взаимно простым с P-1 и меньше него!";
                }
            }
            else
            {
                BtnCipher.IsEnabled = false;
                LErrorK.Visibility = Visibility.Visible;
                LErrorK.Text = "Число должно быть > 0!";
            }
        }

        private byte[] ReadingByte(string fileToRead)
        {
            TBlckInput.Text = "";
            FileInfo fileInfo = new FileInfo(fileToRead);
            long fileSizeInBytes = fileInfo.Length;
            byte[] readedBytes = new byte[fileSizeInBytes];

            using (FileStream fileStream = new FileStream(fileToRead, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(readedBytes, 0, (int)fileSizeInBytes); // not such large files in this task
            }

            foreach (byte byteR in readedBytes)
            {
                TBlckInput.Text += Convert.ToString(byteR, 10);
            }

            return readedBytes;
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите файл";

            if (openFileDialog.ShowDialog() == true)
            {
                TBlckInput.Text = "";
                selectedFilePath = openFileDialog.FileName;
                readedBytes = ReadingByte(selectedFilePath);
                BtnCipher.IsEnabled = true;
            }
        }

        private string Cipher(string outputPath, int p, int g, int y, int k, byte[] source)
        {
            short[] filesBytes = new short[source.Length * 2];
            int posOutput = 0;
            int startK = k;
            for (int posPlane = 0; posPlane < source.Length; posPlane++, posOutput+=2)
            {
                filesBytes[posOutput] = (short)modPow(g, k, p);
                filesBytes[posOutput + 1] = (short)modPow(source[posPlane], 1, p);
                filesBytes[posOutput + 1] = (short)((source[posPlane] * (short)modPow(y, k, p)) % p);
                addToK = 1;
                while (!GCD(k + addToK, p - 1))
                {
                    addToK++;
                }
                k = (k + addToK) < p - 1 ? k + addToK : startK;
            }
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                foreach (short value in filesBytes)
                {
                    writer.Write(value);
                }
            }
            return string.Join(" ", filesBytes.Select(b => b.ToString())); 
        }

        private string Decipher(string outputPath, int p, int x, short[] source)
        {
            short[] filesBytes = new short[source.Length / 2];

            int posOutput = 0;
            for (int posPlane = 0; posPlane < source.Length; posPlane+=2, posOutput++)
            {
                filesBytes[posOutput] = (short)modPow(source[posPlane], p-1-x, p);
                filesBytes[posOutput] = (short)((source[posPlane+1] * filesBytes[posOutput]) % p);
            }

            byte[] shortArray = new byte[source.Length / 2];
            int k = 0;
            for (int i = 0; i < source.Length / 2; i ++)
            {

                byte[] conv = BitConverter.GetBytes(filesBytes[k]);
                shortArray[i] = conv[0];
                k++;
            }

            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
            {
                fileStream.Write(shortArray, 0, source.Length / 2); // not such large files in this task
            }
            return string.Join("", filesBytes.Select(b => b.ToString()));
        }

        private void BtnCipher_Click(object sender, RoutedEventArgs e)
        {
            TBlckOutputA.Text = "";
            TBlckOutputB.Text = "";
            TBlckOutput.Text = "";
            TBlckOutput.Visibility = Visibility.Hidden;
            string outputPath = "D:\\Uni\\Labs\\Inform\\TI3\\chiphered";
            outputPath += System.IO.Path.GetExtension(selectedFilePath);
            string[] decimalArray = Cipher(outputPath, int.Parse(TBI_P.Text), int.Parse(CBPrimitiveSqrt.SelectedItem.ToString()), int.Parse(L_Y.Content.ToString()), int.Parse(TBI_K.Text), readedBytes).Split(' ');
            for (int i = 0; i < decimalArray.Length; i++)
            {
                if (i % 2 == 0)
                    TBlckOutputA.Text += decimalArray[i] + " ";
                else
                    TBlckOutputB.Text += decimalArray[i] + " ";
            }
        }

        private void BtnDecipher_Click(object sender, RoutedEventArgs e)
        {
            if (readedBytes.Length != 0)
            {
                TBlckOutput.Visibility = Visibility.Visible;
                TBlckOutput.Text = "";
                TBlckOutputA.Text = "";
                TBlckOutputB.Text = "";
                string outputPath = "D:\\Uni\\Labs\\Inform\\TI3\\dechiphered";
                outputPath += System.IO.Path.GetExtension(selectedFilePath);

                short[] shortArray = new short[readedBytes.Length / 2];

                Buffer.BlockCopy(readedBytes, 0, shortArray, 0, readedBytes.Length);


                TBlckOutput.Text = Decipher(outputPath, int.Parse(TBI_P.Text), int.Parse(TBI_X.Text), shortArray);
            }
        }
    }
}