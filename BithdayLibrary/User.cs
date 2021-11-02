using System.ComponentModel.DataAnnotations;

namespace BirthdayLibrary
{
    public class User
    {
        [Key]
        public long Id { get; private set; }
        public bool Notify { get; set; }

        public User(long id, bool notify = false)
        {
            Id = id;
            Notify = notify;
        }
    }
}
