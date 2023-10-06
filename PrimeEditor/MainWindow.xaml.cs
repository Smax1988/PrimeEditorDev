using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TextEditorLib;
using TextEditorLib.Models;

namespace PrimeEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int tabCounter = 1;
    private readonly DispatcherTimer messageTimer;
    private readonly TextEditor editor = new TextEditor();

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
    /// Opens a new file in a new tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewFile_Click(object sender, RoutedEventArgs e)
    {
        editor.CreateNewTab(tabCounter, tabControl, TextBox_TextChanged, (Style)FindResource("CloseableTabItemStyle"));
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        editor.OpenFile(tabControl, StatusMessage, messageTimer, tabCounter, TextBox_TextChanged, (Style)FindResource("CloseableTabItemStyle"));
    }

    private void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        editor.SaveFile(tabControl, StatusMessage, messageTimer);
    }

    private void SaveFileAs_Click(object sender, RoutedEventArgs e)
    {
        editor.SaveFileAs(tabControl, StatusMessage, messageTimer);
    }

    /// <summary>
    /// Close the Editor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseEditor_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Unsaved Progress will be lost. Do you wish to proceed?", string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Warning);
        switch (result)
        {
            case MessageBoxResult.Yes:
                this.Close();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Event Handler for when timer runs out
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MessageTimer_Tick(object sender, EventArgs e)
    {
        // Clear the message content
        StatusMessage.Content = string.Empty;

        // Stop the timer
        messageTimer.Stop();
    }

    #region Word and Character Counts
    /// <summary>
    /// Event Handler for when the text in TextBox changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
    {
        CharacterCount.Content = "Zeichen: " + Analysis.CountCharacters(tabControl);
        WordCount.Content = "Wörter: " + Analysis.CountWords(tabControl);
    }

    /// <summary>
    /// Event Handler: Whenever tab selection changes,
    /// reinitialize Word and Character count.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        WordCount.Content = "Words: " + Analysis.CountWords(tabControl);
        CharacterCount.Content = "Characters: " + Analysis.CountCharacters(tabControl);
    }
    #endregion

    #region Text Wrap
    /// <summary>
    /// Toggle the Text Wrap of TextBox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextWrap_Click(object sender, RoutedEventArgs e)
    {
        Edit.TextWrap(tabControl);
    }
    #endregion

    #region Tabs
    /// <summary>
    /// Event when AddNewTab Button is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddNewTab_Click(object sender, MouseButtonEventArgs e)
    {
        editor.CreateNewTab(tabCounter, tabControl, TextBox_TextChanged, (Style)FindResource("CloseableTabItemStyle"));
    }

    /// <summary>
    /// Event when Close Tab Button is clicked. Asks the user for confirmation
    /// if file has unsaved changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseTab_Click(object sender, RoutedEventArgs e)
    {
        string selectedTabId = TextEditorBase.GetSelectedTabId(tabControl);
        TextBox textBox = TextEditorBase.GetTextEditorTextBox(selectedTabId, tabControl);

        if (textBox.Text.Length == 0)
        {
            Tab.CloseTab(sender, tabControl);
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
                        Tab.CloseTab(sender, tabControl);
                        break;
                    case MessageBoxResult.No:
                        Tab.CloseTab(sender, tabControl);
                        break;
                    default:
                        break;
                }
            }
            else Tab.CloseTab(sender, tabControl);
        }
    }

    /// <summary>
    /// Creats a Tab that acts as a Create new Tab Button as last element in tabControl
    /// </summary>
    public void CreateAddNewTabButton()
    {
        TabItem tabItem = new TabItem
        {
            Header = "+",
            Tag = new TextBoxData { TabId = tabCounter++.ToString() }
        };
        tabItem.MouseLeftButtonUp += AddNewTab_Click;
        tabControl.Items.Add(tabItem);
    }
    #endregion
}
