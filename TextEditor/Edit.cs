using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace TextEditorLib
{
    public class Edit : TextEditorBase
    {
        public static void TextWrap(TabControl tabControl)
        {
            string tabId = GetSelectedTabId(tabControl);
            TextBox textBox = GetTextEditorTextBox(tabId, tabControl);

            if (textBox != null)
            {
                if (textBox.TextWrapping == TextWrapping.Wrap)
                    textBox.TextWrapping = TextWrapping.NoWrap;
                else
                    textBox.TextWrapping = TextWrapping.Wrap;
            }
        }
    }
}
