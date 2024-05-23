using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Interfaces
{
    public interface INotifiable
    {
        bool NotifitionWhenReply { get; set; }

        string? CreateUserName { get; set; }

        string? CreateUserEmail { get; set; }
    }
}
