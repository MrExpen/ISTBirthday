using System;
using BirthdayLibrary;
using BirthdayLibrary.Services;
using System.Collections.Generic;
using System.Linq;

namespace DbAdder
{
    class Program
    {
        private static ApplicationDbContext _db = new ApplicationDbContext("server=mrexpen.ru;user=mrexpen;password=;database=ISTBirthdaysV2;");
        static void Main(string[] args)
        {
            foreach (var item in _db.Students.ToArray())
            {
                Console.WriteLine(item.GetFullInfo(new BirthdayLibrary.Utils.NoFormatFormatter()));
            }
        }
    }
}
