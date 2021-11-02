using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Services
{
    public class BaseServise : IService
    {
        [Key]
        public long DbId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Data { get; set; }

        public virtual bool IsLink { get; protected set; }
        public BaseServise(string name, string data, bool isLink) : this()
        {
            Name = name;
            Data = data;
            IsLink = isLink;
        }
        public BaseServise()
        {

        }
    }
}
