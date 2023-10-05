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

        public void SaveFile()
        {

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
