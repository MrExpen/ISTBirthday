using System;
using BithdayLibrary;
using BithdayLibrary.Services;
using System.Collections.Generic;
using System.Linq;

namespace DbAdder
{
    class Program
    {
        private static ApplicationDbContext _db = new ApplicationDbContext("server=mrexpen.ru;user=mrexpen;password=m{V8[W?THnf@GHVckkv3'7=Rbm/2P=QC._L8br*^Dk;database=_11BBirthdays;");
        static void Main(string[] args)
        {
            _db.SaveChanges();
        }
    }
}
