using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TextEditorLib.Models;

namespace TextEditorLib
{
    public class TextEditor : TextEditorBase
    {
        /// <summary>
        /// Open a .txt or .cs file. File only opened, if not already opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenFile(TabControl tabControl, Label messageContainer, DispatcherTimer messageTimer, int tabCounter, TextChangedEventHandler textChangedEventHandler, Style closableTabItemStyle)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs\"";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName.Split('\\').Last();

                if (ItemIsOpened(fileName, tabControl))
                {
                    MessageBox.Show($"The file '{fileName}' is already opened.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                PrimeEditorFile file = new PrimeEditorFile();
                file.FilePath = openFileDialog.FileName;
                file.Content = File.ReadAllText(file.FilePath);

                var newTextBox = CreateNewTab(tabCounter, tabControl, textChangedEventHandler, closableTabItemStyle);

                newTextBox.Text = file.Content;
                ((TextBoxData)newTextBox.Tag).FilePath = file.FilePath;

                SetSelectedTabItemHeader(file.FileName, tabControl);

                // Status Message
                SetStatusMessage($"File '{file.FileName}' opened.", messageContainer, messageTimer);
            }
        }

        /// <summary>
        /// Saves an already opened file or navigates to "Save As" method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveFile(TabControl tabControl, Label messageContainer, DispatcherTimer messageTimer)
        {
            string tabId = GetSelectedTabId(tabControl);
            TextBox textBox = GetTextEditorTextBox(tabId, tabControl);
            PrimeEditorFile file = new PrimeEditorFile();

            if (textBox != null)
            {
                string filePath = ((TextBoxData)textBox.Tag).FilePath;
                Tab.AddFilePathToTab(filePath, tabControl);

                file.FilePath = filePath;
                file.Content = textBox.Text;

                if (File.Exists(file.FilePath))
                {
                    File.WriteAllText(file.FilePath, file.Content);
                    SetStatusMessage($"File '{file.FileName}' saved.", messageContainer, messageTimer);
                }
                else
                    SaveFileAs(tabControl, messageContainer, messageTimer);
            }
        }

        /// <summary>
        /// Save the text as .txt or .cs file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveFileAs(TabControl tabControl, Label messageContainer, DispatcherTimer messageTimer)
        {
            string tabId = GetSelectedTabId(tabControl);
            TextBox textBox = GetTextEditorTextBox(tabId, tabControl);
            if (textBox != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    Tab.AddFilePathToTab(filePath, tabControl);

                    PrimeEditorFile file = new PrimeEditorFile();
                    file.FilePath = filePath;
                    file.Content = textBox.Text;

                    File.WriteAllText(file.FilePath, file.Content);
                    SetSelectedTabItemHeader(file.FileName, tabControl);

                    SetStatusMessage($"Datei '{file.FileName}' wurde gespeichert.", messageContainer, messageTimer);
                }
            }
        }

        public TextBox CreateNewTab(int tabCounter, TabControl tabControl, TextChangedEventHandler textChangedEventHandler, Style closableTabItemStyle, string tabName = "New Tab")
        {
            return Tab.CreateNewTab(tabCounter, tabControl, textChangedEventHandler, closableTabItemStyle);
        }

        /// <summary>
        /// Determines if a file is already opened
        /// </summary>
        /// <param name="fileName">Filename to check</param>
        /// <returns>True if item is already opened, false otherwise</returns>
        private bool ItemIsOpened(string fileName, TabControl tabControl)
        {
            ItemCollection tabControlItems = tabControl.Items;
            foreach (var item in tabControlItems)
            {
                if ((string)((TabItem)item).Header == fileName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
