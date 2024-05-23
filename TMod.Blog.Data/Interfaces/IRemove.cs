using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Interfaces
{
    public interface IRemove : IVersionControl
    {
        bool IsRemove { get; set; }

        DateTime? RemoveDate { get; set; }
    }
}
