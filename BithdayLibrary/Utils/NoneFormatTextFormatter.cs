using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Utils
{
    internal class NoneFormatTextFormatter : BaseServiceTextFormatter
    {
        public override string Format(string text, Format format, string linkText = null, string language = null)
        {
            switch (format)
            {
                case Utils.Format.Link:
                    return text + $"[{linkText}]";
                default:
                    return text;
            }
        }
    }
}
