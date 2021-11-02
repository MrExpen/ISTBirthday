using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Utils
{
    public class TelegramHTMLTextFormatter : BaseServiceTextFormatter, IServiceTextFormatter
    {
        public override string Format(string text, Format format, string linkText = null, string language = null)
        {
            switch (format)
            {
                case Utils.Format.Bold:
                    return "<b>" + text + "</b>";
                case Utils.Format.Italic:
                    return "<i>" + text + "</i>";
                case Utils.Format.Code:
                    return "<code>" + text + "</code>";
                case Utils.Format.Strike:
                    return "<s>" + text + "</s>";
                case Utils.Format.Underline:
                    return "<u>" + text + "</u>";
                case Utils.Format.Preformat:
                    return $"<pre language=\"{language}\">" + text + "</pre>";
                case Utils.Format.Link:
                    return $"<a href=\"{text}\">{linkText}</a>";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
