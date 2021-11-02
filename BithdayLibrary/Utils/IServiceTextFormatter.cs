using BirthdayLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Utils
{
    public interface IServiceTextFormatter
    {
        string Format(string text, Format format, string linkText = null, string language = null);
        string Bold(string text);
        string Italic(string text);
        string Code(string text);
        string Strike(string text);
        string Underline(string text);
        string Preformat(string text, string language);
        string Link(string text, string linkText);
    }
}
