using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using TextEditorLib.Models;

namespace TextEditorLib;

abstract class TextEditorBase
{
    /// <summary>
    /// Gets the TextBox by Id of the currently selected tab. Every tab and its textbox
    /// have the same id.
    /// </summary>
    /// <param name="tabId">The Id of the currently selected tab</param>
    /// <returns>Found TextBox</returns>
    protected static TextBox GetTextEditorTextBox(string tabId, TabControl tabControl)
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

    /// <summary>
    /// Gets the currently selected TabItem's Id
    /// </summary>
    /// <returns>Selected TabItem Id</returns>
    protected static string GetSelectedTabId(TabControl tabControl)
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
    protected static void SetStatusMessage(string message, Label StatusMessage, DispatcherTimer messageTimer)
    {
        StatusMessage.Content = message;
        messageTimer.Start();
    }
}
