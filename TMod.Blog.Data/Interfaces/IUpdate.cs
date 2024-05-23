using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Interfaces
{
    public interface IUpdate:IVersionControl
    {
        DateTime? UpdateDate { get; set; }
    }
}
