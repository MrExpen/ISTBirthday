using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayLibrary.Services
{
    public interface IService
    {
        string Name { get; }
        string Data { get; }
        bool IsLink { get; }
    }
}
