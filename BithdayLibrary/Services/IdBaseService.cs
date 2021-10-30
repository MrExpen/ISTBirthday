using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BithdayLibrary.Services
{
    public abstract class IdBaseService : BaseServise
    {
        public override string Data
        {
            get => _link;
            set => _link = value;
        }

        public virtual long? _id { get; set; }
        protected string _link { get; set; }



        public IdBaseService(string name, string link) : base(name, link, true)
        {
            Data = link;
        }
        public IdBaseService(string name, long id) : base(name, null, true)
        {
            _id = id;
        }
    }
}
