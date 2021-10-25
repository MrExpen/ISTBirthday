using System;
using BithdayLibrary;

namespace DbAdder
{
    class Program
    {
        private static ApplicationDbContext _db = new ApplicationDbContext();
        static void Main(string[] args)
        {
            _db.Students.Add(new Student(
                "Дмитрий",
                "Чибриков",
                "Сергеевич",
                new DateTime(2003, 2, 17),
                vkId: 80839391,
                telegramId: 651057170
                ));

            _db.SaveChanges();
        }
    }
}
