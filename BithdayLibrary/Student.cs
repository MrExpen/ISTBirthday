using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BirthdayLibrary.Utils;

namespace BirthdayLibrary
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

        public string GetHowToRichString(IServiceTextFormatter textFormatter)
        {
            return string.Join("\n",
                Services
                .Where(s => !string.IsNullOrEmpty(s.Name) && !string.IsNullOrEmpty(s.Data))
                .Select(s => $"{s.Name}: " + 
                (s.IsLink ? 
                    textFormatter.Link(s.Data, "Link") :
                    textFormatter.Bold(s.Data)))
                );
        }

        public string GetFullInfo(IServiceTextFormatter textFormatter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(LastName))
            {
                stringBuilder.AppendLine($"Фамилия: " + textFormatter.Bold(LastName));
            }
            if (!string.IsNullOrEmpty(FirstName))
            {
                stringBuilder.AppendLine($"Имя: " + textFormatter.Bold(FirstName));
            }
            if (!string.IsNullOrEmpty(Patronymic))
            {
                stringBuilder.AppendLine($"Отчество: " + textFormatter.Bold(Patronymic));
            }
            if (Birthday.HasValue)
            {
                stringBuilder.AppendLine($"Дата рождения: " + textFormatter.Bold(Birthday.Value.ToShortDateString()));
                stringBuilder.AppendLine(textFormatter.Italic($"({Birthday.Value.ToLongDateString()})"));
                stringBuilder.AppendLine($"Дней осталось: " + textFormatter.Bold(DaysLeft.Value.ToString()));
            }
            var howToRich = GetHowToRichString(textFormatter);
            if (!string.IsNullOrEmpty(howToRich))
            {
                stringBuilder.AppendLine(howToRich);
            }
            if (!string.IsNullOrEmpty(Description))
            {
                stringBuilder.AppendLine($"Дополнительно: " + textFormatter.Bold(Description));
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
        public string FullName => LastName + ' ' + FirstName;

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
