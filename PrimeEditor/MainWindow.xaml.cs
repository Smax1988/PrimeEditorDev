using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrimeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int tabCounter = 1;


        public MainWindow()
        {
            InitializeComponent();
            CreateNewTab();

            // Set Main Window Icon
            Icon = new BitmapImage(new Uri("../../../Images/edit_text_icon.png", UriKind.RelativeOrAbsolute));
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

        /// <summary>
        /// Open a txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs\"";
            if (openFileDialog.ShowDialog() == true)
            {
                PrimeEditorFile file = new PrimeEditorFile();
                file.FilePath = openFileDialog.FileName;
                file.Content = File.ReadAllText(file.FilePath);
                
                var newTextBox = CreateNewTab();

                newTextBox.Text = file.Content;
                ((TextBoxData)newTextBox.Tag).FilePath = file.FilePath;

                SetSelectedTabItemHeader(file.FileName);

                // Status Message
                StatusMessage.Content = $"Datei '{file.FileName}' wurde geöffnet.";
            }
        }

        /// <summary>
        /// Saves an already opened 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = GetTextEditorTextBox();
            PrimeEditorFile file = new PrimeEditorFile();
            file.FilePath = ((TextBoxData)textBox.Tag).FilePath;
            file.Content = textBox.Text;

            if (File.Exists(file.FilePath))
            {
                File.WriteAllText(file.FilePath, file.Content);
                StatusMessage.Content = $"Datei '{file.FileName}' wurde gespeichert.";
            }
            else
            {
                SaveFileAs_Click(sender, e);
            }
        }

        /// <summary>
        /// Save the text as .txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file(*.cs)|*.cs";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                TextBox textBox = GetTextEditorTextBox();

                PrimeEditorFile file = new PrimeEditorFile();
                file.FilePath = saveFileDialog.FileName;
                file.Content = textBox.Text;

                File.WriteAllText(file.FilePath, file.Content);
                SetSelectedTabItemHeader(file.FileName);

                StatusMessage.Content = $"Datei '{file.FileName}' wurde gespeichert.";
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
            TextBox textBox = GetTextEditorTextBox();
            return textBox.Text.Length.ToString();
        }


        /// <summary>
        /// Count the number of words
        /// </summary>
        /// <returns></returns>
        private string CountWords()
        {
            TextBox textBox = GetTextEditorTextBox();
            string text = textBox.Text.Trim();
            string[] words = GetWords(text);

            return words.Length.ToString();
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
            string pattern = @"[\s.,;()\[\]'" + "\"?!]+|\t|\n";
            return Regex.Split(text, pattern);
        }

        /// <summary>
        /// Close the Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseEditor_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Change the Text Wrap of TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextWrap_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = GetTextEditorTextBox();
            PrimeEditorFile file = new PrimeEditorFile();
            file.FilePath = textBox.Name;
            file.Content = textBox.Text;

            if (file.TextWrapping)
            {
                textBox.TextWrapping = TextWrapping.NoWrap;
                file.TextWrapping = false;
                TextWrap.Header = "Enable Text Wrapping";
            }
            else
            {
                textBox.TextWrapping = TextWrapping.Wrap;
                file.TextWrapping = true;
                TextWrap.Header = "Disable Text Wrapping";
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


            TextBoxData data = new TextBoxData{ TabId = tabCounter++.ToString()};
            TextBox textBox = CreatePrimeEditorTextBox(data.TabId);
            textBox.TextChanged += TextBox_TextChanged;

            newTab.Content = textBox;
            newTab.Style = (Style)FindResource("CloseableTabItemStyle");

            tabControl.Items.Insert((tabControl.Items.Count - 1), newTab);

            tabControl.SelectedItem = newTab;
            
            return textBox;
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.Items.Count > 2) 
            {
                string selectedTabId = GetSelectedTabId();
                TextBox textBox = GetTextEditorTextBox();

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

        private static DependencyObject FindChild(DependencyObject parent, Type childType, int childIndex)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            if (childCount == 0)
                return null;
            while (parent != null)
            {
                var child = VisualTreeHelper.GetChild(parent, childIndex);
                if (child != null && child.GetType() == childType)
                    return child;
                parent = child;
            }
            return null;
        }

        private TextBox GetTextEditorTextBox()
        {
            TextBox textBox = (TextBox)FindChild(tabControl, typeof(TextBox), 0);
            return textBox;
        }


        private string GetSelectedTabId()
        {
            TabItem selectedTab = (TabItem)tabControl.SelectedItem;
            return ((TextBoxData)selectedTab.Tag).TabId;
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
                Padding = new Thickness(5, 10, 0, 5),
                Background = Brushes.Black,
                Foreground = Brushes.White,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
        }
    }
}
