using System;
using Telegram.Bot;
using System.IO;
using System.Linq;
using Telegram.Bot.Extensions.Polling;
using System.Threading.Tasks;
using System.Threading;
using BithdayLibrary;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Collections.Generic;
using System.Net.Http;

namespace ISTBirthday
{
    class Program
    {
        private static readonly DirectoryInfo LogsPath;
        private static readonly TelegramBotClient _bot;

        static Program()
        {
            _bot = new TelegramBotClient("1927440684:AAGtyHOzzwxeYkR0f0dtKLI8lFs1FFz6udM");
            LogsPath = new DirectoryInfo("logs");
            if (!LogsPath.Exists)
            {
                LogsPath.Create();
            }
        }

        static async Task Main(string[] args)
        {
            var me = await _bot.GetMeAsync();
            Console.WriteLine($"{me.Username}[{me.Id}]");

            using var cts = new CancellationTokenSource();

            _bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            await NotifyLoop();
        }

        private static async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is HttpRequestException)
            {
                return;
            }
            await System.IO.File.AppendAllTextAsync(Path.Combine(LogsPath.FullName, "exceptions.log"), $"{DateTime.Now}|{exception}\n\n");
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.Message &&
                update.Message.Chat.Type == ChatType.Private)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        await System.IO.File.AppendAllTextAsync(Path.Combine(LogsPath.FullName, "messages.log"), $"{DateTime.Now}|{update.Message.From.Username}[{update.Message.From.Id}]: {update.Message.Text}\n");
                        Console.WriteLine($"{update.Message.From.Username}[{update.Message.From.Id}]: {update.Message.Text}");
                        if (update.Message.Text.StartsWith("/"))
                        {
                            if (update.Message.Text == "/start")
                            {
                                await _bot.SendStart(update.Message.Chat.Id);
                            }
                            else if (update.Message.Text == "/allbirthdays")
                            {
                                await _bot.SendAllBirthdays(update.Message.Chat.Id, db.Students.AsEnumerable().OrderBy(stud => stud.FullInfo));
                            }
                            else if (update.Message.Text == "/allbirthdayssorted")
                            {
                                await _bot.SendAllBirthdaysSorted(update.Message.Chat.Id, db.Students);
                            }
                            else if (update.Message.Text == "/nearestbirthday")
                            {
                                await _bot.SendNearestBirthday(update.Message.Chat.Id, db.Students);
                            }
                            else if (update.Message.Text == "/notificate")
                            {
                                var user = await db.Users.FindUserOrCreate(update.Message.Chat.Id);
                                user.Notify = !user.Notify;
                                await _bot.SendNotify(update.Message.Chat.Id, user.Notify);
                            }
                            else if (update.Message.Text == "/find")
                            {
                                await _bot.SendFind(update.Message.Chat.Id);
                            }
                            else if (update.Message.Text == "/all")
                            {
                                await _bot.SendAll(update.Message.Chat.Id, db.Students);
                            }
                            else if (update.Message.Text.StartsWith("/find "))
                            {
                                await _bot.SendFind(update.Message.Chat.Id, db.Students, update.Message.Text.Substring(6).ToLower().Trim());
                            }
                        }
                        else
                        {
                            await _bot.SendUseCommands(update.Message.Chat.Id);
                        }
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    await _bot.SendWTF(update.Message.Chat.Id);
                }
            }
        }

        private static async Task NotifyLoop()
        {
            if (DateTime.Now < DateTime.Today.AddHours(6))
            {
                await Task.Delay(DateTime.Today.AddHours(6) - DateTime.Now);
            }
            while (true)
            {
                using (var db = new ApplicationDbContext())
                {
                    List<Task> tasks = new List<Task>();
                    var a = db.Students.ToArray().GroupBy(stud => stud.DaysLeft);
                    foreach (var group in a)
                    {
                        if (!group.Key.HasValue)
                        {
                            continue;
                        }
                        if (group.Key == 5)
                        {
                            foreach (var user in db.Users)
                            {
                                foreach (var student in group)
                                {
                                    tasks.Add(_bot.Send5Days(user.Id, student));
                                }
                            }
                        }
                        else if (group.Key == 1)
                        {
                            foreach (var user in db.Users)
                            {
                                foreach (var student in group)
                                {
                                    tasks.Add(_bot.Send1Days(user.Id, student));
                                }
                            }
                        }
                        else if (group.Key == 0)
                        {
                            foreach (var user in db.Users)
                            {
                                foreach (var student in group)
                                {
                                    tasks.Add(_bot.Send0Days(user.Id, student));
                                }
                            }
                        }

                    }
                    await Task.WhenAll(tasks);
                    await Task.Delay(DateTime.Today.AddDays(1).AddHours(6) - DateTime.Now);
                }
            }
        }
    }
}
