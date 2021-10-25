using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BithdayLibrary
{
    public static class DbUtils
    {
        public static async Task<User> FindUserOrCreate(this DbSet<User> db, long id)
        {
            User user = await db.FindAsync(id);
            if (user == null)
            {
                await db.AddAsync(new User(id));
                user = await db.FindAsync(id);
            }
            return user;
        }
    }
}
