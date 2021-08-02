using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Sepidar.Framework
{
    public interface IDocumentHelper
    {
        IDocumentHelper Create(Stream stream);

        IDocumentHelper Create(byte[] bytes);

        string GetText();

        IDocumentHelper Highlight(List<string> words);

        IDocumentHelper Highlight(Dictionary<string, Color> wordsAndColors);

        IDocumentHelper Save(string path);

        IDocumentHelper Replace(string text, string replacement);

        Stream GetStream();
    }
}
