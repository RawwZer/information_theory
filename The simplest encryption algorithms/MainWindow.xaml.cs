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

namespace TI1
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

        private static bool IsSquareCipher;
        private static string outputPath = "D:\\Uni\\Labs\\Inform\\TI1\\output.txt";

        static string ReadAndRemoveFirstLine(ref string content)
        {
            int newlineIndex = content.IndexOf('\r');
            string firstLine = content.Substring(0, newlineIndex);
            content = content.Substring(newlineIndex + 2);
            return firstLine;
        }

        private string ReadFromFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        private void CipherSquare(string planeText, string key, bool isEncryption)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                int turnNum = 0;
                if (isEncryption)
                {
                    writer.WriteLine(key);
                }

                key = key.Replace("-", string.Empty);
                int[,] startCoord = new int[4, 2];
                for (int i = 0, row = 0; i < key.Length; i += 2, row++)
                {
                    startCoord[row, 0] = int.Parse(key[i].ToString());
                    startCoord[row, 1] = int.Parse(key[i + 1].ToString());
                }

                StringBuilder forWrite = new StringBuilder("                ");
                for (global::System.Int32 currPos = 0; currPos < planeText.Length;)
                {
                    int positionI = 0;
                    switch (turnNum)
                    {
                        case 0:
                            for (int i = 0, row = 0, wInd = 0, currPosPT = 0; i < key.Length & currPos < planeText.Length; i += 2, row++, wInd++, currPos++)
                            {
                                positionI = startCoord[row, 0] * 4 + startCoord[row, 1];
                                if (!isEncryption)
                                {
                                    currPosPT = currPos > 15 ? positionI + 16 : positionI;
                                    positionI = wInd + turnNum * 4;
                                }
                                else { currPosPT = currPos; }
                                forWrite[positionI] = planeText[currPosPT];
                            }
                            break;

                        case 1:
                            (startCoord[1, 0], startCoord[3, 0]) = (startCoord[3, 0], startCoord[1, 0]);
                            (startCoord[1, 1], startCoord[3, 1]) = (startCoord[3, 1], startCoord[1, 1]);
                            for (int i = 0, row = 0, wInd = 0, currPosPT = 0; i < key.Length & currPos < planeText.Length; i += 2, row++, wInd++, currPos++)
                            {
                                positionI = startCoord[row, 1] * 4 + (3 - startCoord[row, 0]); // Первый поворот: Xn = Ys; Yn = 3 - Xs
                                if (!isEncryption)
                                {
                                    currPosPT = currPos > 15 ? positionI + 16 : positionI;
                                    positionI = wInd + turnNum * 4;
                                }
                                else { currPosPT = currPos; }
                                forWrite[positionI] = planeText[currPosPT];
                            }
                            break;

                        case 2:
                            (startCoord[1, 0], startCoord[3, 0]) = (startCoord[3, 0], startCoord[1, 0]);
                            (startCoord[1, 1], startCoord[3, 1]) = (startCoord[3, 1], startCoord[1, 1]);
                            for (int i = 0, row = 3, wInd = 0, currPosPT = 0; i < key.Length & currPos < planeText.Length; i += 2, row--, wInd++, currPos++)
                            {
                                positionI = (3 - startCoord[row, 0]) * 4 + (3 - startCoord[row, 1]); // Второй поворот: Xn = 3 - Xs; Yn = 3- Ys
                                if (!isEncryption)
                                {
                                    currPosPT = currPos > 15 ? positionI + 16 : positionI;
                                    positionI = wInd + turnNum * 4;
                                }
                                else { currPosPT = currPos; }
                                forWrite[positionI] = planeText[currPosPT];
                            }
                            break;

                        case 3:
                            (startCoord[1, 0], startCoord[3, 0]) = (startCoord[3, 0], startCoord[1, 0]);
                            (startCoord[1, 1], startCoord[3, 1]) = (startCoord[3, 1], startCoord[1, 1]);
                            for (int i = 0, row = 3, wInd = 0, currPosPT = 0; i < key.Length & currPos < planeText.Length; i += 2, row--, wInd++, currPos++)
                            {
                                positionI = (3 - startCoord[row, 1]) * 4 + startCoord[row, 0]; // Третий поворот: Xn = 3 - Ys; Yn = Xs
                                if (!isEncryption)
                                {
                                    currPosPT = currPos > 15 ? positionI + 16 : positionI;
                                    positionI = wInd + turnNum * 4;
                                }
                                else { currPosPT = currPos; }
                                forWrite[positionI] = planeText[currPosPT];
                            }
                            break;
                    }

                    turnNum = (turnNum + 1) % 4;

                    if (turnNum == 0 || currPos >= planeText.Length)
                    {
                        string forWriteS = forWrite.ToString();
                        resultText.Text += forWriteS;
                        writer.Write(forWriteS);
                        forWrite.Clear();
                        forWrite.Append("                ");
                        (startCoord[1, 0], startCoord[3, 0]) = (startCoord[3, 0], startCoord[1, 0]);
                        (startCoord[1, 1], startCoord[3, 1]) = (startCoord[3, 1], startCoord[1, 1]);
                    }
                }
            }
        }

        private void CipherVi(string planeText, string key, bool isEncryption)
        {
            const string RusAlph = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                int posKey = 0;
                int indOff = 0;
                StringBuilder forWrite = new StringBuilder();
                StringBuilder keyW = new StringBuilder();
                for (global::System.Int32 posText = 0; posText < planeText.Length; posText++, posKey++)
                {
                    int indC = RusAlph.IndexOf(planeText[posText]);
                    int resC;
                    
                    if (isEncryption)
                    {
                        int indK = (RusAlph.IndexOf(key[posKey]) + indOff) % RusAlph.Length;
                        resC = (indK + indC) % RusAlph.Length;
                     //   keyW.Append(RusAlph[indK]);
                    }
                    else
                    {
                        int indK = (RusAlph.IndexOf(key[posKey]) + indOff) % RusAlph.Length;
                        //   resC = ((RusAlph.Length - RusAlph.IndexOf(key[posKey])) + indC) % RusAlph.Length;
                        resC = ((RusAlph.Length - indK) + indC) % RusAlph.Length;
                    }

                    resultText.Text += RusAlph[resC];
                    forWrite.Append(RusAlph[resC]);


                    if (posKey + 1 >= key.Length)
                    {
                        posKey = -1;
                        indOff++;
                    }
                }
                // writer.WriteLine(keyW);
                writer.WriteLine(key);
                writer.Write(forWrite);
            }
        }

        private void wayOfInp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            if (combobox.SelectedIndex == 1)
            {
                BTNopenFile.IsEnabled = true;
            }
        }

        private void BTNopenFile_Click(object sender, RoutedEventArgs e)
        {
            string pattern;
            if (!IsSquareCipher)
            {
                pattern = "[a-zA-Z0-9]";
            }
            else
            {
                pattern = "[а-яА-ЯёЁ0-9]";
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.Title = "Выберите файл";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string planeTextFromFile = ReadFromFile(selectedFilePath);
                planeText.Text = Regex.Replace(planeTextFromFile, pattern, "");
                BTNdecoding.IsEnabled = true;
            }
        }

        private void BTNcoding_Click(object sender, RoutedEventArgs e)
        {
            resultText.Text = string.Empty;
            if (IsSquareCipher)
            {
                CipherSquare(planeText.Text.Replace(" ", string.Empty).Replace("\r\n", string.Empty), keyInp.Text, true);
            }
            else
            {
                CipherVi(planeText.Text.Replace(" ", string.Empty).Replace("\r\n", string.Empty).ToLower(), keyInp.Text.ToLower(), true);
            }
        }

        private void BTNdecoding_Click(object sender, RoutedEventArgs e)
        {
            resultText.Text = string.Empty;
            string planeTextV = planeText.Text;
            string key;
            if (keyInp.Text == "Введите ключ..." || keyInp.Text == "")
            {
                key = ReadAndRemoveFirstLine(ref planeTextV);
                keyInp.Text = key;
            }
            else
            {
                key = keyInp.Text;
            }
            planeText.Text = planeTextV;

            if (IsSquareCipher)
            {
                CipherSquare(planeTextV.Replace("\r\n", string.Empty), key, false);
            }
            else
            {
                CipherVi(planeTextV.Replace("\r\n", string.Empty), key, false);
            }
        }

        private void planeText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern;
            if (IsSquareCipher)
            {
                pattern = @"^[a-zA-Z]+$";
            }
            else
            {
                pattern = @"^[а-яё]+$";
            }

            if (!Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = true;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsSquareCipher = true;
            keyInp.IsEnabled = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            IsSquareCipher = false;
            keyInp.IsEnabled = true;
        }

        private void keyInp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern;
            if (IsSquareCipher)
            {
                pattern = @"^[0-9-]+$";
            }
            else
            {
                pattern = @"^[а-яё]+$";
            }

            if (!Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = true;
            }
        }

        private void keyInp_TextInput(object sender, TextChangedEventArgs e)
        {
            if (keyInp.Text != "Введите ключ..." & keyInp.Text != "")
            {
                BTNcoding.IsEnabled = true;
            }
        }
    }
}