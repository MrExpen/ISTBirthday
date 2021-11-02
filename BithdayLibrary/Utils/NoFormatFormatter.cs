using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Utils
{
    public class NoFormatFormatter : BaseServiceTextFormatter, IServiceTextFormatter
    {
        public override string Format(string text, Format format, string linkText = null, string language = null)
        {
            switch (format)
            {
                case Utils.Format.Link:
                    return $"{linkText} ({text})";
                default:
                    return text;
            }
        }
    }
}
