using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TextEditorLib.Models;

namespace TextEditorLib
{
    public abstract class TextEditorBase
    {
        /// <summary>
        /// Gets the TextBox by Id of the currently selected tab. Every tab and its textbox
        /// have the same id.
        /// </summary>
        /// <param name="tabId">The Id of the currently selected tab</param>
        /// <returns>Found TextBox</returns>
        public static TextBox GetTextEditorTextBox(string tabId, TabControl tabControl)
        {
            foreach (TabItem tabItem in tabControl.Items)
            {
                if (((TextBoxData)tabItem.Tag).TabId == tabId)
                {
                    if (tabItem.Content is TextBox textBox)
                    {
                        return textBox;
                    }
                }
            }
            return null!; // TextBox not found
        }

        /// <summary>
        /// Gets the currently selected TabItem's Id
        /// </summary>
        /// <returns>Selected TabItem Id</returns>
        public static string GetSelectedTabId(TabControl tabControl)
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

        /// <summary>
        /// Sets the text of the StatusMessage element at the bottom of the UI
        /// </summary>
        /// <param name="message">Message to be displayed in the Statusbar</param>
        protected void SetStatusMessage(string message, Label messageContainer, DispatcherTimer messageTimer)
        {
            messageContainer.Content = message;
            messageTimer.Start();
        }

        /// <summary>
        /// Set the Header of the selectedTabItem
        /// </summary>
        /// <param name="header">Header of the TabItem </param>
        protected void SetSelectedTabItemHeader(string header, TabControl tabControl)
        {
            TabItem tab = (TabItem)tabControl.SelectedItem;
            tab.Header = header;
        }

        /// <summary>
        /// Creates a TextBox with TextBoxData Object as Tag with all necessary
        /// properties.
        /// </summary>
        /// <param name="tabId">Textbox will belong to the tab with this Id</param>
        /// <returns>TextBox</returns>
        protected static TextBox CreatePrimeEditorTextBox(string tabId)
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
