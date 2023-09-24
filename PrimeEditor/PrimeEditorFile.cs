using PrimeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrimeEditor
{
    public class PrimeEditorFile
    {
        public string FilePath { get; set; } = string.Empty;

        public string FileName
        {
            get { return FilePath.Split('\\').Last(); }
        }

        public bool TextWrapping { get; set; } = false;

        public string Content { get; set; } = string.Empty;
    }
}