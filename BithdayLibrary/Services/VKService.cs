using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Services
{
    public class VKService : IdBaseService
    {
        public override string Data 
        {
            get => base.Data ?? "https://vk.com/id" + _id; 
            set => base.Data = value; 
        }


        public VKService(string link) : base("VK", link)
        {
        }
        public VKService(long id) : base("VK", id)
        {
        }
        public VKService() : base("VK")
        {

        }
    }
}
