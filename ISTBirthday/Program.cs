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
        private static readonly ILog _log;
        private static readonly TelegramBotClient _bot;
        private static readonly string _connectionString;
        private static readonly string _botToken;
        private const string _connectionStringFileName = "connectionString.txt";
        private const string _botTokenFileName = "token.txt";
        private static readonly IServiceTextFormatter _textFormatter;
        static Program()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("RU");
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (new[] { "-h", "--help" }.Contains(args[i]))
                    {
                        Console.WriteLine("-h, --help - show this message.");
                        Console.WriteLine("-tf --token-file - set a file name for bot token.");
                        Console.WriteLine("-cf --connection-string-file - set a file name for db connection string.");
                        Console.WriteLine("-t --tokene - set a bot token.");
                        Console.WriteLine("-c --connection-string - set a db connection string.");
                        Environment.Exit(0);
                    }
                    else if (new[] { "-tf", "--token-file" }.Contains(args[i]))
                    {
                        try
                        {
                            _botToken = System.IO.File.ReadAllText(args[i + 1]);
                        }
                        catch (FileNotFoundException)
                        {
                            _log.Fatal($"File {args[i + 1]} Not Found!");
                            throw;
                        }
                    }
                    else if (new[] { "-cf", "--connection-string-file" }.Contains(args[i]))
                    {
                        try
                        {
                            _connectionString = System.IO.File.ReadAllText(args[i + 1]);
                        }
                        catch (FileNotFoundException)
                        {
                            _log.Fatal($"File {args[i + 1]} Not Found!");
                            throw;
                        }
                    }
                    else if (new[] { "-t", "--token" }.Contains(args[i]))
                    {
                        _botToken = args[i + 1];
                    }
                    else if (new[] { "-c", "--connection-string" }.Contains(args[i]))
                    {
                        _connectionString = args[i + 1];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    _log.Fatal("Check your parametrs!");
                    throw;
                }

            }
            if (string.IsNullOrEmpty(_botToken))
            {
                try
                {
                    _botToken = System.IO.File.ReadAllText(_botTokenFileName);
                }
                catch (FileNotFoundException)
                {
                    _log.Fatal($"File {_botTokenFileName} Not Found!");
                    throw;
                }
            }
            if (string.IsNullOrEmpty(_connectionString))
            {
                try
                {
                    _connectionString = System.IO.File.ReadAllText(_connectionStringFileName);
                }
                catch (FileNotFoundException)
                {
                    _log.Fatal($"File {_connectionStringFileName} Not Found!");
                    throw;
                }
            }

            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            _log = LogManager.GetLogger("Program");
            _textFormatter = new TelegramHTMLTextFormatter();

            _bot = new TelegramBotClient(_botToken);
        }

        static async Task Main(string[] args)
        {
            var me = await _bot.GetMeAsync();
            _log.Info($"{me.Username}[{me.Id}]");

            using var cts = new CancellationTokenSource();

            _bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            await NotifyLoop();
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is HttpRequestException)
            {
                return Task.CompletedTask;
            }
            _log.Fatal(exception.Message, exception);

            return Task.CompletedTask;
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.Message &&
                update.Message.Chat.Type == ChatType.Private)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    using (var db = new ApplicationDbContext(_connectionString))
                    {
                        _log.Info($"{update.Message.From.Username}[{update.Message.From.Id}]: {update.Message.Text}");
                        if (update.Message.Text.StartsWith("/"))
                        {
                            if (update.Message.Text == "/start")
                            {
                                await _bot.SendStart(update.Message.Chat.Id, _textFormatter);
                            }
                            else if (update.Message.Text == "/allbirthdays")
                            {
                                await _bot.SendAllBirthdays(update.Message.Chat.Id, _textFormatter, db.Students.ToArray().OrderBy(stud => stud.FullName));
                            }
                            else if (update.Message.Text == "/allbirthdayssorted")
                            {
                                await _bot.SendAllBirthdaysSorted(update.Message.Chat.Id, _textFormatter, db.Students.ToArray());
                            }
                            else if (update.Message.Text == "/nearestbirthday")
                            {
                                await _bot.SendNearestBirthday(update.Message.Chat.Id, _textFormatter, db.Students);
                            }
                            else if (update.Message.Text == "/notificate")
                            {
                                var user = await db.Users.FindUserOrCreate(update.Message.Chat.Id);
                                user.Notify = !user.Notify;
                                await _bot.SendNotify(update.Message.Chat.Id, _textFormatter, user.Notify);
                            }
                            else if (update.Message.Text == "/find")
                            {
                                await _bot.SendFind(update.Message.Chat.Id, _textFormatter);
                            }
                            else if (update.Message.Text == "/all")
                            {
                                await _bot.SendAll(update.Message.Chat.Id, _textFormatter, db.Students.ToArray().OrderBy(x => x.FullName));
                            }
                            else if (update.Message.Text.StartsWith("/find "))
                            {
                                await _bot.SendFind(update.Message.Chat.Id, _textFormatter, db.Students.ToArray(), update.Message.Text.Substring(6).ToLower().Trim());
                            }
                        }
                        else
                        {
                            await _bot.SendUseCommands(update.Message.Chat.Id, _textFormatter);
                        }
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    await _bot.SendWTF(update.Message.Chat.Id, _textFormatter);
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

                using (var db = new ApplicationDbContext(_connectionString))
                {
                    List<Task> tasks = new List<Task>();
                    var users = db.Users.ToArray();
                    var gropedStudents = db.Students.ToArray().GroupBy(stud => stud.DaysLeft).ToArray();
                    foreach (var group in gropedStudents)
                    {
                        if (!group.Key.HasValue)
                        {
                            continue;
                        }
                        if (group.Key == 5)
                        {
                            foreach (var user in users)
                            {
                                foreach (var student in group)
                                {
                                    tasks.Add(_bot.Send5Days(user.Id, _textFormatter, student));
                                }
                            }
                        }
                        else if (group.Key == 1)
                        {
                            foreach (var user in users)
                            {
                                foreach (var student in group)
                                {
                                    tasks.Add(_bot.Send1Days(user.Id, _textFormatter, student));
                                }
                            }
                        }
                        else if (group.Key == 0)
                        {
                            foreach (var user in users)
                            {
                                foreach (var student in group)
                                {
                                    tasks.Add(_bot.Send0Days(user.Id, _textFormatter, student));
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
