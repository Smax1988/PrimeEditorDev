using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using TextEditorLib.Models;
using System.Windows.Media;

namespace TextEditorLib
{
    internal class Tab : TextEditorBase
    {
        /// <summary>
        /// Creates a new Tab and sets the Tag property to TextBoxData for data binding
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns>the created TextBox</returns>
        public static TextBox CreateNewTab(int tabCounter, TabControl tabControl, TextChangedEventHandler textChangedEventHandler, Style closableTabItemStyle, string tabName = "New Tab")
        {
            TabItem newTab = new TabItem();
            newTab.Header = tabName;

            TextBoxData data = new TextBoxData { TabId = tabCounter++.ToString() };
            TextBox textBox = CreatePrimeEditorTextBox(data.TabId);
            textBox.TextChanged += textChangedEventHandler;

            newTab.Content = textBox;
            newTab.Tag = data;
            newTab.Style = closableTabItemStyle;
            //newTab.Style = (Style)FindResource("CloseableTabItemStyle");

            tabControl.Items.Insert((tabControl.Items.Count - 1), newTab);

            tabControl.SelectedItem = newTab;

            return textBox;
        }

        /// <summary>
        /// Adds the FilePath to the TextBoxData Object
        /// </summary>
        /// <param name="filePath"></param>
        public static void AddFilePathToTab(string filePath, TabControl tabControl)
        {
            TabItem selectedTab = (TabItem)tabControl.SelectedItem;

            if (selectedTab != null)
            {
                ((TextBoxData)selectedTab.Tag).FilePath = filePath;
            }
        }

        /// <summary>
        /// Closes a Tab by removing it from tabControl
        /// </summary>
        /// <param name="sender"></param>
        public static void CloseTab(object sender, TabControl tabControl)
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

        /// <summary>
        /// Helper for CloseTab() to get the tab from where the close button was clicked
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
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
    }
}
