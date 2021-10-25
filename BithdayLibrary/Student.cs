using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BithdayLibrary
{
    public class Student
    {
        [Key]
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? Birthday { get; set; }
        public string Description { get; set; }

        public long? VkId { get; set; }
        public long? TelegramId { get; set; }
        [NotMapped]
        public string FullName => LastName + ' ' + FirstName;
        [NotMapped]
        public string VkLink => VkId.HasValue ? $"https://vk.com/id{VkId.Value}" : null;
        [NotMapped]
        public string VkLinkHTML => VkId.HasValue ? $"<a href=\"{VkLink}\">Профиль VK.</a>" : null;
        [NotMapped]
        public string TelegramLink => TelegramId.HasValue ? $"tg://user?id={TelegramId.Value}" : null;
        [NotMapped]
        public string TelegramLinkHtml => TelegramId.HasValue ? $"<a href=\"{TelegramLink}\">Профиль TG.</a>" : null;
        [NotMapped]
        public string FullInfo => $"Имя: <b>{FirstName}</b>\nФамилия: <b>{LastName}</b>\nОтчество: <b>{Patronymic}</b>\nДата рождения: <b>{Birthday?.ToShortDateString()}\n({Birthday?.ToLongDateString()})</b>\nДней осталось: <b>{DaysLeft}</b>\nСсылка на ВК: {VkLinkHTML}\nСсылка на телегу: {TelegramLinkHtml}\nДополнительно: {Description}";
        [NotMapped]
        public string KeyWords => string.Join(" ", 
            new[]
            {
                FirstName?.ToLower(),
                LastName?.ToLower(),
                Patronymic?.ToLower(),
                Birthday?.ToShortDateString(),
                Birthday?.ToLongDateString(),
                Description?.ToLower()
            }.Where(x => !string.IsNullOrEmpty(x))
            );

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

        public Student()
        {

        }

        public Student(string firstName, string lastName, string patronymic = null, DateTime? birthday = null, string description = null, long? vkId = null, long? telegramId = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Birthday = birthday?.Date;
            Description = description;
            VkId = vkId;
            TelegramId = telegramId;
        }
    }
}
