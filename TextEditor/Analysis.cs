using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TextEditorLib
{
    public class Analysis : TextEditorBase
    {
        /// <summary>
        /// Count the Number of Characters in Textbox
        /// </summary>
        /// <returns>Count of Characters in TextBox</returns>
        public static string CountCharacters(TabControl tabControl)
        {
            string tabId = GetSelectedTabId(tabControl);
            TextBox textBox = GetTextEditorTextBox(tabId, tabControl);
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
        public static string CountWords(TabControl tabControl)
        {
            string tabId = GetSelectedTabId(tabControl);
            TextBox textBox = GetTextEditorTextBox(tabId, tabControl);

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
         static string[] GetWords(string text)
        {
            if (text.Length == 0)
                return Array.Empty<string>();
            string pattern = @"[\s" + "\"?!]+|\t|\n";
            //string pattern = @"[\s.,;()\[\]'" + "\"?!]+|\t|\n";
            return Regex.Split(text, pattern);
        }
    }
}
