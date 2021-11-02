using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Utils
{
    public abstract class BaseServiceTextFormatter : IServiceTextFormatter
    {
        public abstract string Format(string text, Format format, string linkText = null, string language = null);

        public string Bold(string text) => Format(text, Utils.Format.Bold);

        public string Code(string text) => Format(text, Utils.Format.Code);

        public string Italic(string text) => Format(text, Utils.Format.Italic);

        public string Preformat(string text, string language) => Format(text, Utils.Format.Code, language: language);

        public string Strike(string text) => Format(text, Utils.Format.Strike);

        public string Underline(string text) => Format(text, Utils.Format.Underline);

        public string Link(string text, string linkText) => Format(text, Utils.Format.Link, linkText: linkText);
    }
}
