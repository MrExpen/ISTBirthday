using BirthdayLibrary;
using BirthdayLibrary.Utils;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ISTBirthday
{
    public static class DefaultAnswers
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("DefaultAnswers");
        public static async Task SendWTF(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter)
        {
            await telegramBotClient._MySendMessage(chatId, textFormatter, "Я не могу разобрать, что вы хотите сказать.");
        }
        public static async Task SendUseCommands(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter)
        {
            await telegramBotClient._MySendMessage(chatId, textFormatter, "Используйте команды начинающиеся с \"" + textFormatter.Bold("/") + "\"");
        }
        public static async Task SendStart(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter)
        {
            await telegramBotClient._MySendMessage(chatId, textFormatter, "Здравствуйте, надеюсь, я буду Вам полезен)");
        }
        public static async Task SendAllBirthdays(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, IEnumerable<Student> students)
        {
            await telegramBotClient._MySendMessages(chatId, textFormatter, students.Select(student => $"{student.FullName} - " + textFormatter.Bold(student.DaysLeft.ToString())), "\n");
        }
        public static async Task SendAllBirthdaysSorted(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, IEnumerable<Student> students)
            => await SendAllBirthdays(telegramBotClient, chatId, textFormatter, students.OrderBy(students => students.DaysLeft));
        public static async Task SendNearestBirthday(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, IEnumerable<Student> students)
        {

            if (students.Count() == 0)
            {
                await telegramBotClient._SendDbEmpty(chatId, textFormatter);
                return;
            }
            var immediate = students.Where(student => student.DaysLeft.HasValue).GroupBy(students => students.DaysLeft.Value).OrderBy(students => students.Key).First();

            await telegramBotClient._MySendMessages(chatId, textFormatter, immediate.Select(student => textFormatter.Bold(student.FullName) + $" - {student.Birthday.Value.ToShortDateString()}"), "\n", textBefore: "Ближайший день рождения:\n", textAfter: "\nДо него остолось: " + textFormatter.Bold(immediate.Key.ToString()));
        }
        public static async Task SendNotify(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, bool state)
        {
            await telegramBotClient._MySendMessage(chatId, textFormatter, textFormatter.Bold(state ? "Уведомления включены✅" : "Уведомления выключены❌"));
        }
        public static async Task SendAll(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, IEnumerable<Student> students)
        {
            await telegramBotClient._MySendMessages(chatId, textFormatter, students.Select(stud => stud.GetFullInfo(textFormatter)), "\n\n");
        }
        public static async Task SendFind(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, IEnumerable<Student> students, string keyString)
        {
            var result = students.Where(stud => Fuzz.PartialTokenSetRatio(keyString, stud.GetKeyWords()) > 75).ToArray();
            if (result.Length == 0)
            {
                await telegramBotClient._MySendMessage(chatId, textFormatter, "Не удалось найти ни одного человека по ключевому слову: " + textFormatter.Bold(keyString));
            }
            else
            {
                await telegramBotClient._MySendMessages(chatId, textFormatter, students.Select(stud => stud.GetFullInfo(textFormatter)), "\n\n");
            }
        }
        public static async Task SendFind(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter)
        {
            await telegramBotClient._MySendMessage(chatId, textFormatter, "Используйте команду в паре с ключевым словом, например:\n/find " + textFormatter.Bold("Дмитрий"));
        }
        public static async Task Send0Days(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, Student student)
        {
            if (student.DaysLeft != 0)
            {
                return;
            }
            await telegramBotClient._MySendMessage(chatId, textFormatter, $"{textFormatter.Bold(student.FullName)} сегодня празднует свой {textFormatter.Bold((DateTime.Today.Year - student.Birthday.Value.Year).ToString())} день рождения🎂!\nНе забудьте написать ей/ему в этот замечательный день.\n{student.GetHowToRichString(textFormatter)}");

        }
        public static async Task Send1Days(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, Student student)
        {
            if (student.DaysLeft != 1)
            {
                return;
            }
            await telegramBotClient._MySendMessage(chatId, textFormatter, $"Завтра {textFormatter.Bold(student.FullName)} станет на год старше🎉🎉");
        }
        public static async Task Send5Days(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, Student student)
        {
            if (student.DaysLeft != 5)
            {
                return;
            }
            await telegramBotClient._MySendMessage(chatId, textFormatter, $"Надеюсь ты не забыл, что {textFormatter.Bold(student.FullName)} через 5 дней станет ещё старше!");
        }
        private static async Task _SendDbEmpty(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter)
        {
            await telegramBotClient._MySendMessage(chatId, textFormatter, textFormatter.Bold("База данных пуста."));
        }
        private static async Task _MySendMessage(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, string message, int retries = 3, int timeOut = 1000, ParseMode parseMode = ParseMode.Html)
        {
            int attempt = 0;

            while (++attempt <= retries)
            {
                try
                {
                    await telegramBotClient.SendTextMessageAsync(chatId, message, parseMode);
                    return;
                }
                catch (Exception e)
                {
                    _log.Error($"Attemt: {attempt}, {e.Message}", e);
                }
                await Task.Delay(timeOut);
            }
            _log.Error($"Failed to send a message to {chatId.Username}[{chatId.Identifier}] after {retries} attempts");
        }
        private static async Task _MySendMessages(this ITelegramBotClient telegramBotClient, ChatId chatId, IServiceTextFormatter textFormatter, IEnumerable<string> messages, string separator, string textBefore = null, string textAfter = null)
        {
            if (messages.Count() == 0)
            {
                await telegramBotClient._SendDbEmpty(chatId, textFormatter);
            }
            else
            {
                bool first = true;
                IEnumerable<string> messages1 = messages;

                while (messages1.Count() != 0)
                {
                    for (int count = messages1.Count(); true; count--)
                    {
                        string message = string.Join(separator, messages1.Take(count));
                        if (message.Length < 4096)
                        {
                            if (first)
                            {
                                message = textBefore + message;
                                first = false;
                            }
                            if (count == messages1.Count())
                            {
                                message += textAfter;
                            }
                            await telegramBotClient._MySendMessage(chatId, textFormatter, message);
                            messages1 = messages1.Skip(count);
                            break;
                        }
                    }
                }
            }
        }
    }
}
