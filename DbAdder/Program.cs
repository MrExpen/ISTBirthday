using System;
using BithdayLibrary;
using System.Collections.Generic;
using BithdayLibrary.Services;

namespace DbAdder
{
    class Program
    {
        private static ApplicationDbContext _db = new ApplicationDbContext();
        static void Main(string[] args)
        {
            _db.SaveChanges();
        }
    }
}
