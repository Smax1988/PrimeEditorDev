using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace PrimeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath { get; set; } = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Open a new Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFile(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Close the Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseEditor(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
