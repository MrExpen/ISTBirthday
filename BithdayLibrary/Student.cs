using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BithdayLibrary
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? Birthday { get; set; }
        public string Description { get; set; }
        public virtual List<Services.BaseServise> Services { get; set; } = new List<Services.BaseServise>();

        public string GetHowToRichString()
        {
            return string.Join("\n",
                Services
                .Where(s => !string.IsNullOrEmpty(s.Name) && !string.IsNullOrEmpty(s.Data))
                .Select(s => $"{s.Name}: " + (s.IsLink ? $"<a href=\"{s.Data}\">Link</a>" : $"<b>{s.Data}</b>")));
        }

        [NotMapped]
        public string FullName => LastName + ' ' + FirstName;

        public string GetFullInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(LastName))
            {
                stringBuilder.AppendLine($"Фамилия: <b>{LastName}</b>");
            }
            if (!string.IsNullOrEmpty(FirstName))
            {
                stringBuilder.AppendLine($"Имя: <b>{FirstName}</b>");
            }
            if (!string.IsNullOrEmpty(Patronymic))
            {
                stringBuilder.AppendLine($"Отчество: <b>{Patronymic}</b>");
            }
            if (Birthday.HasValue)
            {
                stringBuilder.AppendLine($"Дата рождения: <b>{Birthday?.ToShortDateString()}</b>");
                stringBuilder.AppendLine($"<b>({Birthday?.ToLongDateString()})</b>");
                stringBuilder.AppendLine($"Дней осталось: <b>{DaysLeft}</b>");
            }
            var howToRich = GetHowToRichString();
            if (!string.IsNullOrEmpty(howToRich))
            {
                stringBuilder.AppendLine(howToRich);
            }
            if (!string.IsNullOrEmpty(Description))
            {
                stringBuilder.AppendLine($"Дополнительно: <b>{Description}</b>");
            }

            return stringBuilder.ToString();
        }

        public string GetKeyWords()
        {
            return string.Join(" ",
            new[]
            {
                            FirstName?.ToLower(),
                            LastName?.ToLower(),
                            Patronymic?.ToLower(),
                            Birthday?.ToShortDateString(),
                            Birthday?.ToLongDateString(),
                            Description?.ToLower()
            }.Concat(Services.Where(s => !s.IsLink).Select(s => s.Data)).Where(x => !string.IsNullOrEmpty(x))
            );
        }

        [NotMapped]
        public int? DaysLeft
        {
            get
            {
                if (!Birthday.HasValue)
                {
                    return null;
                }
                var dtnow = DateTime.Today;
                var birthdaythisyear = new DateTime(dtnow.Year, Birthday.Value.Month, Birthday.Value.Day);
                return dtnow <= birthdaythisyear ? (birthdaythisyear - dtnow).Days : ((birthdaythisyear.AddYears(1) - dtnow).Days);
            }
        }
    }
}
