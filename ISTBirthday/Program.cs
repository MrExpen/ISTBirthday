using BirthdayLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BirthdayLibrary.Utils;
using log4net;

namespace ISTBirthday
{
    class Program
    {
        private static readonly ILog Log;
        private static readonly TelegramBotClient Bot;
        private static readonly string ConnectionString;
        private static readonly string ConfigFile = "log4net.config";
        private static readonly IServiceTextFormatter TextFormatter;
        static Program()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("RU");

            var botToken = Environment.GetEnvironmentVariable("TOKEN");
            ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (new[] { "-h", "--help" }.Contains(args[i]))
                    {
                        Console.WriteLine("-h, --help - show this message.");
                        Console.WriteLine("-tf --token-file - set a file name for bot token.");
                        Console.WriteLine("-t --token - set a bot token.");
                        Console.WriteLine("-c --connection-string - set a db connection string.");
                        Console.WriteLine("--log-config - set a xml config for log4net.");
                        Environment.Exit(0);
                    }
                    else if (new[] { "-t", "--token" }.Contains(args[i]))
                    {
                        botToken = args[++i];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Check your parameters!");
                    throw;
                }
            }

            log4net.Config.XmlConfigurator.Configure(new FileInfo(ConfigFile));
            Log = LogManager.GetLogger("Program");

            TextFormatter = new TelegramHTMLTextFormatter();

            Log = LogManager.GetLogger("Program");

            Bot = new TelegramBotClient(botToken);
        }

        static async Task Main()
        {
            var me = await Bot.GetMeAsync();
            Log.Info($"{me.Username}[{me.Id}]");

            using var cts = new CancellationTokenSource();

            Bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            await NotifyLoop();
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is HttpRequestException)
            {
                return Task.CompletedTask;
            }
            Log.Fatal(exception.Message, exception);

            return Task.CompletedTask;
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
                        Log.Info($"{update.Message.From.Username}[{update.Message.From.Id}]: {update.Message.Text}");
                        if (update.Message.Text.StartsWith("/"))
                        {
                            if (update.Message.Text == "/start")
                            {
                                await Bot.SendStart(update.Message.Chat.Id, TextFormatter);
                            }
                            else if (update.Message.Text == "/allbirthdays")
                            {
                                await Bot.SendAllBirthdays(update.Message.Chat.Id, TextFormatter, db.Students.ToArray().OrderBy(stud => stud.FullName));
                            }
                            else if (update.Message.Text == "/allbirthdayssorted")
                            {
                                await Bot.SendAllBirthdaysSorted(update.Message.Chat.Id, TextFormatter, db.Students.ToArray());
                            }
                            else if (update.Message.Text == "/nearestbirthday")
                            {
                                await Bot.SendNearestBirthday(update.Message.Chat.Id, TextFormatter, db.Students);
                            }
                            else if (update.Message.Text == "/notificate")
                            {
                                var user = await db.Users.FindUserOrCreate(update.Message.Chat.Id);
                                user.Notify = !user.Notify;
                                await Bot.SendNotify(update.Message.Chat.Id, TextFormatter, user.Notify);
                            }
                            else if (update.Message.Text == "/find")
                            {
                                await Bot.SendFind(update.Message.Chat.Id, TextFormatter);
                            }
                            else if (update.Message.Text == "/all")
                            {
                                await Bot.SendAll(update.Message.Chat.Id, TextFormatter, db.Students.ToArray().OrderBy(x => x.FullName));
                            }
                            else if (update.Message.Text.StartsWith("/find "))
                            {
                                await Bot.SendFind(update.Message.Chat.Id, TextFormatter, db.Students.ToArray(), update.Message.Text.Substring(6).ToLower().Trim());
                            }
                        }
                        else
                        {
                            await Bot.SendUseCommands(update.Message.Chat.Id, TextFormatter);
                        }
                        await db.SaveChangesAsync(cancellationToken);
                    }
                }
                else
                {
                    await Bot.SendWTF(update.Message.Chat.Id, TextFormatter);
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
                    var users = db.Users.Where(u => u.Notify).ToArray();
                    var gropedStudents = db.Students.ToArray().GroupBy(stud => stud.DaysLeft).ToArray();
                    foreach (var group in gropedStudents)
                    {
                        switch (group.Key)
                        {
                            case 10:
                            {
                                tasks.AddRange(from user in users from student in @group select Bot.Send10Days(user.Id, TextFormatter, student));

                                break;
                            }
                            case 5:
                            {
                                tasks.AddRange(from user in users from student in @group select Bot.Send5Days(user.Id, TextFormatter, student));

                                break;
                            }
                            case 1:
                            {
                                tasks.AddRange(from user in users from student in @group select Bot.Send1Days(user.Id, TextFormatter, student));

                                break;
                            }
                            case 0:
                            {
                                tasks.AddRange(from user in users from student in @group select Bot.Send0Days(user.Id, TextFormatter, student));

                                break;
                            }
                            default:
                                continue;
                        }
                    }
                    await Task.WhenAll(tasks);
                    await Task.Delay(DateTime.Today.AddDays(1).AddHours(6) - DateTime.Now);
                }
            }
        }
    }
}
