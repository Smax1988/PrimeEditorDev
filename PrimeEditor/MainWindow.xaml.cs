﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PrimeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int tabCounter = 1;
        private readonly DispatcherTimer messageTimer;

        public MainWindow()
        {
            InitializeComponent();
            CreateAddNewTabButton();

            // Set Main Window Icon
            Icon = new BitmapImage(new Uri("../../../Images/edit_text_icon_helix.png", UriKind.RelativeOrAbsolute));
            
            // Init timer for StausMessage disappearance
            messageTimer = new DispatcherTimer();
            messageTimer.Interval = TimeSpan.FromSeconds(3);
            messageTimer.Tick += MessageTimer_Tick!;
        }

        /// <summary>
        /// Open a new Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }

        private bool ItemIsOpened(string fileName)
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

        /// <summary>
        /// Open a .txt or .cs file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs\"";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName.Split('\\').Last();

                if (ItemIsOpened(fileName))
                {
                    MessageBox.Show($"The file '{fileName}' is already opened.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }


                PrimeEditorFile file = new PrimeEditorFile();
                file.FilePath = openFileDialog.FileName;
                file.Content = File.ReadAllText(file.FilePath);

                var newTextBox = CreateNewTab();

                newTextBox.Text = file.Content;
                ((TextBoxData)newTextBox.Tag).FilePath = file.FilePath;

                SetSelectedTabItemHeader(file.FileName);

                // Status Message
                SetStatusMessage($"File '{file.FileName}' opened.");

            }
        }

        private void SetStatusMessage(string message)
        {
            StatusMessage.Content = message;
            messageTimer.Start();
        }

        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            // Clear the message content
            StatusMessage.Content = string.Empty;

            // Stop the timer
            messageTimer.Stop();
        }

        /// <summary>
        /// Saves an already opened file or navigates to "Save As" method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            string tabId = GetSelectedTabId();
            TextBox textBox = GetTextEditorTextBox(tabId);
            PrimeEditorFile file = new PrimeEditorFile();

            if (textBox != null)
            {
                string filePath = ((TextBoxData)textBox.Tag).FilePath;
                AddFilePathToTab(filePath);

                file.FilePath = filePath;
                file.Content = textBox.Text;

                if (File.Exists(file.FilePath))
                {
                    File.WriteAllText(file.FilePath, file.Content);
                    SetStatusMessage($"File '{file.FileName}' saved.");
                }
                else
                {
                    SaveFileAs_Click(sender, e);
                }
            }
        }

        /// <summary>
        /// Save the text as .txt or .cs file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            string tabId = GetSelectedTabId();
            TextBox textBox = GetTextEditorTextBox(tabId);
            if (textBox != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    AddFilePathToTab(filePath);

                    PrimeEditorFile file = new PrimeEditorFile();
                    file.FilePath = filePath;
                    file.Content = textBox.Text;

                    File.WriteAllText(file.FilePath, file.Content);
                    SetSelectedTabItemHeader(file.FileName);

                    SetStatusMessage($"Datei '{file.FileName}' wurde gespeichert.");
                }
            }
        }

        private void AddFilePathToTab(string filePath)
        {
            TabItem selectedTab = (TabItem)tabControl.SelectedItem;

            if (selectedTab != null)
            {
                ((TextBoxData)selectedTab.Tag).FilePath = filePath;
            }
        }


        /// <summary>
        /// Event Handler for when the text in TextBox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            CharacterCount.Content = "Zeichen: " + CountCharacters();
            WordCount.Content = "Wörter: " + CountWords();
        }


        /// <summary>
        /// Count the Number of Characters
        /// </summary>
        /// <returns></returns>
        private string CountCharacters()
        {
            string tabId = GetSelectedTabId();
            TextBox textBox = GetTextEditorTextBox(tabId);
            if (textBox != null)
            {
                return textBox.Text.Length.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Count the number of words
        /// </summary>
        /// <returns></returns>
        private string CountWords()
        {
            string tabId = GetSelectedTabId();
            TextBox textBox = GetTextEditorTextBox(tabId);

            if (textBox != null)
            {
                string text = textBox.Text.Trim();
                string[] words = GetWords(text);

                return words.Length.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the words from text
        /// </summary>
        /// <param name="text"></param>
        /// <returns>array of all words</returns>
        private static string[] GetWords(string text)
        {
            if (text.Length == 0)
                return Array.Empty<string>();
            string pattern = @"[\s" + "\"?!]+|\t|\n";
            //string pattern = @"[\s.,;()\[\]'" + "\"?!]+|\t|\n";
            return Regex.Split(text, pattern);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WordCount.Content = "Words: " + CountWords();
            CharacterCount.Content = "Characters: " + CountCharacters();
        }

        /// <summary>
        /// Close the Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseEditor_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Unsaved Progress will be lost. Do you wish to proceed?", string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    this.Close();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Change the Text Wrap of TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextWrap_Click(object sender, RoutedEventArgs e)
        {
            //TODO: buggy
            string tabId = GetSelectedTabId();
            TextBox textBox = GetTextEditorTextBox(tabId);

            if (textBox != null)
            {
                if (((TextBoxData)textBox.Tag).TextWrapping)
                {
                    textBox.TextWrapping = TextWrapping.NoWrap;
                    ((TextBoxData)textBox.Tag).TextWrapping = false;
                    TextWrap.Header = "Enable Text Wrapping";
                }
                else
                {
                    textBox.TextWrapping = TextWrapping.Wrap;
                    ((TextBoxData)textBox.Tag).TextWrapping = true;
                    TextWrap.Header = "Disable Text Wrapping";
                }
            }
        }

        private void AddNewTab_Click(object sender, MouseButtonEventArgs e)
        {
            CreateNewTab();
        }

        public TextBox CreateNewTab(string tabName = "New Tab")
        {
            TabItem newTab = new TabItem();
            newTab.Header = tabName;


            TextBoxData data = new TextBoxData { TabId = tabCounter++.ToString() };
            TextBox textBox = CreatePrimeEditorTextBox(data.TabId);
            textBox.TextChanged += TextBox_TextChanged;

            newTab.Content = textBox;
            newTab.Tag = data;
            newTab.Style = (Style)FindResource("CloseableTabItemStyle");

            tabControl.Items.Insert((tabControl.Items.Count - 1), newTab);

            tabControl.SelectedItem = newTab;

            return textBox;
        }

        public void CreateAddNewTabButton()
        {
            TabItem tabItem = new TabItem
            {
                Header = "+",
                Style = (Style)FindResource("CustomTabItemStyle"),
                Tag = new TextBoxData { TabId = tabCounter++.ToString() }
            };
            tabItem.MouseLeftButtonUp += AddNewTab_Click;
            tabControl.Items.Add(tabItem);
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            string selectedTabId = GetSelectedTabId();
            TextBox textBox = GetTextEditorTextBox(selectedTabId);

            if (textBox.Text.Length == 0)
            {
                CloseTab(sender);
            }
            else
            {
                PrimeEditorFile file = new PrimeEditorFile();
                file.FilePath = ((TextBoxData)textBox.Tag).FilePath;
                file.Content = textBox.Text;

                string messageBoxText = "You have unsaved changes. Do you wish to save the file?";
                string caption = "Save";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Question;
                MessageBoxResult result;

                string savedText = string.Empty;

                if (file.FilePath != "notSaved" && file.FilePath != "0")
                {
                    savedText = File.ReadAllText(((TextBoxData)textBox.Tag).FilePath);
                }

                if (file.Content != savedText)
                {
                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            SaveFile_Click(sender, e);
                            CloseTab(sender);
                            break;
                        case MessageBoxResult.No:
                            CloseTab(sender);
                            break;
                        default:
                            break;
                    }
                }
                else CloseTab(sender);
            }
        }

        private void CloseTab(object sender)
        {
            if (sender is Button closeButton)
            {
                // Get the TabItem associated with the close button
                TabItem tabItem = (TabItem)FindParent(closeButton, typeof(TabItem));

                // Remove the tab from the TabControl
                tabControl.Items.Remove(tabItem);
                tabControl.SelectedItem = tabControl.Items[0];
            }
        }

        private static DependencyObject FindParent(DependencyObject child, Type parentType)
        {
            while (child != null)
            {
                var parent = VisualTreeHelper.GetParent(child);
                if (parent != null && parent.GetType() == parentType)
                    return parent;
                child = parent;
            }
            return null;
        }

        private TextBox GetTextEditorTextBox(string tabId)
        {
            foreach (TabItem tabItem in tabControl.Items)
            {
                if (((TextBoxData)(tabItem.Tag)).TabId == tabId)
                {
                    if (tabItem.Content is TextBox textBox)
                    {
                        return textBox;
                    }
                }
            }
            return null!; // TextBox not found
        }

        private string GetSelectedTabId()
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                if (((TextBoxData)selectedTab.Tag).TabId is string tabId)
                {
                    return tabId;
                }
            }
            return null; // No tab selected or tab without a valid tag
        }

        private void SetSelectedTabItemHeader(string header)
        {
            TabItem tab = (TabItem)tabControl.SelectedItem;
            tab.Header = header;
        }

        private static TextBox CreatePrimeEditorTextBox(string tabId)
        {
            return new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 16,
                Tag = new TextBoxData
                {
                    FilePath = "notSaved",
                    TabId = tabId
                },
                Padding = new Thickness(10, 10, 0, 10),
                Background = Brushes.Black,
                Foreground = Brushes.White,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
        }
    }
}
