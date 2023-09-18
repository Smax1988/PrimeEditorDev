using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string Filename { get; set; } = string.Empty;

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
            Filename = string.Empty;
        }

        /// <summary>
        /// Open a txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                PrimeEditorText.Text = File.ReadAllText(openFileDialog.FileName);
                // save filename to local variable
                Filename = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Saves an already opened 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if(File.Exists(Filename))
            {
                File.WriteAllText(Filename, PrimeEditorText.Text);
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
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, PrimeEditorText.Text);
                // save filename to local variable
                Filename = saveFileDialog.FileName;
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
