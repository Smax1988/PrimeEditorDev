using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PrimeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath { get; set; } = string.Empty;
        private bool TextWrapping { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();

            // Set Main Window Icon
            Icon = new BitmapImage(new Uri("../../../Images/edit_text_icon.png", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Open a new Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFile(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "You have unsaved changes. Do you wish to save the file?";
            string caption = "Save";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result;

            string editorText = PrimeEditorText.Text;
            string fileText = string.Empty;
            
            if (FilePath != string.Empty)
                fileText = File.ReadAllText(FilePath);

            if (editorText != fileText)
            {
                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveFile(sender, e);
                        ResetEditor();
                        break;
                    case MessageBoxResult.No:
                        ResetEditor();
                        break;
                    default:
                        break;
                }
            }
            else ResetEditor();
        }

        private void ResetEditor()
        {
            PrimeEditorText.Text = "";
            FilePath = string.Empty;
        }

        /// <summary>
        /// Open a txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs\"";
            if (openFileDialog.ShowDialog() == true)
            {
                PrimeEditorText.Text = File.ReadAllText(openFileDialog.FileName);
                // save filename to local variable
                FilePath = openFileDialog.FileName;

                // Status Message
                StatusMessage.Content = $"Datei '{FilePath.Split('\\').Last()}' wurde geöffnet.";
            }
        }

        /// <summary>
        /// Saves an already opened 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, PrimeEditorText.Text);
                StatusMessage.Content = $"Datei '{FilePath.Split('\\').Last()}' wurde gespeichert.";
            }
            else
            {
                SaveFileAs(sender, e);
            }
        }

        /// <summary>
        /// Save the text as .txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, PrimeEditorText.Text);
                // save filename to local variable
                FilePath = saveFileDialog.FileName;

                StatusMessage.Content = $"Datei '{FilePath.Split('\\').Last()}' wurde gespeichert.";
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            CharacterCount.Content = "Zeichen: " + CountCharacters();
            WordCount.Content = "Wörter: " + CountWords();
        }

        private string CountCharacters()
        {
            return PrimeEditorText.Text.Length.ToString();
        }

        private string CountWords()
        {
            string text = PrimeEditorText.Text.Trim();
            if (text.Length == 0)
                return "0";
            string pattern = @"[\s.,;()\[\]'" + "\"?!]+|\t|\n";
            string[] words = Regex.Split(text, pattern);

            return words.Length.ToString();
        }

        /// <summary>
        /// Close the Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseEditor(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void New_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }
    }
}
