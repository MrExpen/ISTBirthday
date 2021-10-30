using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BithdayLibrary.Services
{
    public class TelegramService : IdBaseService
    {
        public override string Data 
        {
            get => base.Data ?? "tg://user?id=" + _id;
            set => base.Data = value;
        }

        public TelegramService(string link) : base("Telegram", link)
        {
        }

        public TelegramService(long id) : base("Telegram", id)
        {
        }

    }
}
