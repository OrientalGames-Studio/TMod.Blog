using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Interfaces
{
    public interface IEdit : IVersionControl
    {
        DateTime? LastEditDate { get; set; }
    }
}
