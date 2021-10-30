using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using BithdayLibrary;
using FuzzySharp;

namespace ISTBirthday
{
    public static class DefaultAnswers
    {
        public static async Task SendWTF(this ITelegramBotClient telegramBotClient, ChatId chatId)
        {
            await telegramBotClient._MySendMesage(chatId, "Я не могу разобрать, что вы хотите сказать.");
        }
        public static async Task SendUseCommands(this ITelegramBotClient telegramBotClient, ChatId chatId)
        {
            await telegramBotClient._MySendMesage(chatId, "Используйте команды начинающиеся с \"/\"");
        }
        public static async Task SendStart(this ITelegramBotClient telegramBotClient, ChatId chatId)
        {
            await telegramBotClient._MySendMesage(chatId, "Здравствуйте, надеюсь, я буду Вам полезен)");
        }
        public static async Task SendAllBirthdays(this ITelegramBotClient telegramBotClient, ChatId chatId, IEnumerable<Student> students)
        {
            string message = string.Join('\n', students.Select(student => $"{student.FullName} - <b>{student.DaysLeft}</b>"));
            if (string.IsNullOrEmpty(message))
            {
                await telegramBotClient._SendDbEmpty(chatId);
                return;
            }
            await telegramBotClient._MySendMesage(chatId, message);
        }
        public static async Task SendAllBirthdaysSorted(this ITelegramBotClient telegramBotClient, ChatId chatId, IEnumerable<Student> students)
            => await SendAllBirthdays(telegramBotClient, chatId, students.AsEnumerable().OrderBy(students => students.DaysLeft));
        public static async Task SendNearestBirthday(this ITelegramBotClient telegramBotClient, ChatId chatId, IEnumerable<Student> students)
        {
            if (students.Count() == 0)
            {
                await telegramBotClient._SendDbEmpty(chatId);
                return;
            }
            var immediate = students.Where(student => student.DaysLeft.HasValue).GroupBy(students => students.DaysLeft.Value).OrderBy(students => students.Key).First();

            string message = "Ближайший день рождения:\n" + string.Join('\n', immediate.Select(student => $"<b>{student.FullName}</b> - {student.Birthday.Value.ToShortDateString()}")) + $"\nДо него остолось: <b>{immediate.Key}</b>";

            await telegramBotClient._MySendMesage(chatId, message);
        }
        public static async Task SendNotify(this ITelegramBotClient telegramBotClient, ChatId chatId, bool state)
        {
            await telegramBotClient._MySendMesage(chatId, state ? "<b>Уведомления включены</b>✅" : "<b>Уведомления выключены</b>❌");
        }
        public static async Task SendAll(this ITelegramBotClient telegramBotClient, ChatId chatId, IEnumerable<Student> students)
        {
            if (students.Count() == 0)
            {
                await telegramBotClient._SendDbEmpty(chatId);
            }
            else
            {
                IEnumerable<Student> students1 = students.OrderBy(stud => stud.FullName);

                while (students1.Count() != 0)
                {
                    for (int count = students.Count(); true; count--)
                    {
                        try
                        {
                            await telegramBotClient.SendTextMessageAsync(chatId, string.Join("\n\n", students1.Take(count).Select(stud => stud.GetFullInfo())), ParseMode.Html);
                            students1 = students1.Skip(count);
                            break;
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        public static async Task SendFind(this ITelegramBotClient telegramBotClient, ChatId chatId, IEnumerable<Student> students, string keyString)
        {
            var result = students.Where(stud => Fuzz.PartialTokenSetRatio(keyString, stud.GetKeyWords()) > 75).ToArray();
            if (result.Length == 0)
            {
                await telegramBotClient.SendTextMessageAsync(chatId, $"Не удалось найти ни одного человека по ключевому слову: <b>{keyString}</b>", ParseMode.Html);
            }
            else
            {
                IEnumerable<Student> students1 = result;

                while (students1.Count() != 0)
                {
                    for (int count = students.Count(); true; count--)
                    {
                        try
                        {
                            await telegramBotClient.SendTextMessageAsync(chatId, string.Join("\n\n", students1.Take(count).Select(stud => stud.GetFullInfo())), ParseMode.Html);
                            students1 = students1.Skip(count);
                            break;
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        public static async Task SendFind(this ITelegramBotClient telegramBotClient, ChatId chatId)
        {
            await telegramBotClient.SendTextMessageAsync(chatId, "Используйте команду в паре с ключевым словом, например:\n/find <b>Дмитрий</b>", ParseMode.Html);
        }
        public static async Task Send0Days(this ITelegramBotClient telegramBotClient, ChatId chatId, Student student)
        {
            if (student.DaysLeft != 0)
            {
                return;
            }
            await telegramBotClient._MySendMesage(chatId, $"{student.FullName} сегодня празднует свой {DateTime.Today.Year - student.Birthday.Value.Year} день рождения🎂!\nНе забудьте написать ей/ему в этот замечательный день.\n{student.GetHowToRichString()}");
            
        }
        public static async Task Send1Days(this ITelegramBotClient telegramBotClient, ChatId chatId, Student student)
        {
            if (student.DaysLeft != 1)
            {
                return;
            }
            await telegramBotClient._MySendMesage(chatId, $"Завтра {student.FullName} станет на год старше🎉🎉");
        }
        public static async Task Send5Days(this ITelegramBotClient telegramBotClient, ChatId chatId, Student student)
        {
            if (student.DaysLeft != 5)
            {
                return;
            }
            await telegramBotClient._MySendMesage(chatId, $"Надеюсь ты не забыл, что <b>{student.FullName}</b> через 5 дней станет ещё старше!");
        }
        private static async Task _SendDbEmpty(this ITelegramBotClient telegramBotClient, ChatId chatId)
        {
            await telegramBotClient._MySendMesage(chatId, "<b>База данных пуста.</b>");
        }
        private static async Task _MySendMesage(this ITelegramBotClient telegramBotClient, ChatId chatId, string message)
        {
            try
            {
                await telegramBotClient.SendTextMessageAsync(chatId, message, ParseMode.Html);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
