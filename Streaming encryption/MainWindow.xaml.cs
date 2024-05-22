using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Reflection.Metadata;
using System.Drawing;
using System.Collections;
using System.Diagnostics.Metrics;

namespace TI2
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

        static string selectedFilePath;
        static string noimg = "D:\\Uni\\Labs\\Inform\\TI2\\file-broken.png";
        static int stateKey = 1;

        private void KeyInp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = "[0-1]";

            if (!Regex.IsMatch(e.Text, pattern) || KeyInp.Text.Length == 29)
            {
                e.Handled = true;
            }
        }
        private void KeyInp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (KeyInp.Text.Length == 29)
            {
                BtnCipher.IsEnabled = true;
                BtnDecipher.IsEnabled = true;
                TBlckRegister.Text += (stateKey).ToString() + ". " + KeyInp.Text + "\n";
            }
            else
            {
                BtnCipher.IsEnabled = false;
                BtnDecipher.IsEnabled = false;
                if (TBlckRegister != null) { TBlckRegister.Text = "";  stateKey = 1; };
            }
        }

        private void openInView(string filepath)
        {
            if (System.IO.Path.GetExtension(filepath) == ".txt")
            {
                TBlckFile.Text = File.ReadAllText(filepath);
                TBlckFile.Visibility = Visibility.Visible;
                ImgFile.Visibility = Visibility.Hidden;
            }
            else
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(filepath);
                    bitmapImage.EndInit();
                    ImgFile.Source = bitmapImage;
                    TBlckFile.Visibility = Visibility.Hidden;
                    ImgFile.Visibility = Visibility.Visible;
                } catch {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(noimg);
                    bitmapImage.EndInit();
                    ImgFile.Source = bitmapImage;
                    TBlckFile.Visibility = Visibility.Hidden;
                    ImgFile.Visibility = Visibility.Visible;
                };
            }
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите файл";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;

                openInView(selectedFilePath);
            }
        }

        private byte[] ReadingByte(string fileToRead)
        {
            FileInfo fileInfo = new FileInfo(fileToRead); 
            long fileSizeInBytes = fileInfo.Length;
            byte[] readedBytes = new byte[fileSizeInBytes];

            using (FileStream fileStream = new FileStream(fileToRead, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(readedBytes, 0, (int)fileSizeInBytes); // not such large files in this task
            }

            foreach (byte byteR in readedBytes)
            {
                TBlckInput.Text += Convert.ToString(byteR, 2).PadLeft(8, '0'); 
            }

            return readedBytes;
        }

        private void GeneratingKey(ref int startKey)
        {
            stateKey++;
            bool secondBit = (startKey & 0b0_0000_0000_0000_0000_0000_0000_0010) != 0;
            bool lastBit = (startKey & 0b1_0000_0000_0000_0000_0000_0000_0000) != 0;
            startKey = (startKey << 1) | ((secondBit ^ lastBit) ? 1 : 0);
            startKey = startKey & 0b0001_1111_1111_1111_1111_1111_1111_1111;
            TBlckRegister.Text += stateKey.ToString() + ". " + Convert.ToString(startKey, 2).PadLeft(29, '0') + "\n";
        }

        private void Chiphering(string planeFile, int key, string outputPath)
        {
            int posKey = 0;
            byte[] filesBytes = ReadingByte(planeFile);
            FileInfo fileInfo = new FileInfo(planeFile);
            int fileSizeInBytes = (int)fileInfo.Length;

            for (int posPlane = 0; posPlane < fileSizeInBytes; posPlane++)
            {
                int newKey = 0;
                if (posKey + 8 > 29)
                {
                    int oldKey = (key >> (29 - posKey));
                    GeneratingKey(ref key);
                    newKey = (key << (29 - posKey)) | oldKey;
                    posKey = 8 - (29 - posKey);
                }
                else
                {
                    newKey = key >> posKey;
                    posKey += 8;
                }
                filesBytes[posPlane] = (byte)(filesBytes[posPlane] ^ newKey);
                TBlckOutput.Text += Convert.ToString(filesBytes[posPlane], 2).PadLeft(8, '0');

            }

            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
            {
                fileStream.Write(filesBytes, 0, fileSizeInBytes);
            }

        }

        private void BtnCipher_Click(object sender, RoutedEventArgs e)
        {
            TBlckInput.Text = "";
            TBlckOutput.Text = "";
            stateKey = 1;
            TBlckRegister.Text = (stateKey).ToString() + ". " + KeyInp.Text + "\n";
            string outputPath = "D:\\Uni\\Labs\\Inform\\TI2\\chiphered.txt";
/*            outputPath += System.IO.Path.GetExtension(selectedFilePath);*/
            Chiphering(selectedFilePath, Convert.ToInt32(KeyInp.Text.ToString(), 2), outputPath);
            openInView(outputPath);
            using (StreamWriter fileStream = new StreamWriter("D:\\Uni\\Labs\\Inform\\TI2\\keys.txt"))
            {
                fileStream.Write(TBlckRegister.Text, 0, TBlckRegister.Text.Length);
            }
        }

        private void BtnDecipher_Click(object sender, RoutedEventArgs e)
        {
            TBlckInput.Text = "";
            TBlckOutput.Text = "";
            stateKey = 1;
            TBlckRegister.Text = (stateKey).ToString() + ". " + KeyInp.Text + "\n";
            string outputPath = "D:\\Uni\\Labs\\Inform\\TI2\\dechiphered";
            outputPath += System.IO.Path.GetExtension(selectedFilePath);
            Chiphering(selectedFilePath, Convert.ToInt32(KeyInp.Text.ToString(), 2), outputPath);
            openInView(outputPath);
        }
    }
}