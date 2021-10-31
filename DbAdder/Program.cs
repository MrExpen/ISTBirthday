using System;
using BithdayLibrary;
using BithdayLibrary.Services;
using System.Collections.Generic;
using System.Linq;

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
