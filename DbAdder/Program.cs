using System;
using BirthdayLibrary;
using BirthdayLibrary.Services;
using System.Collections.Generic;
using System.Linq;

namespace DbAdder
{
    class Program
    {
        private static ApplicationDbContext _db = new ApplicationDbContext();
        static void Main(string[] args)
        {
            foreach (var item in _db.Students.ToArray())
            {
                Console.WriteLine(item.GetFullInfo(new BirthdayLibrary.Utils.NoFormatFormatter()));
                Console.WriteLine();
            }
        }
    }
}
