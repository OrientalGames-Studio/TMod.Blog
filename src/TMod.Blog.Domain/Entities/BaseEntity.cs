using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Entities
{
    public abstract class BaseEntity<TKey>: ISoftDeleteable,IUpdateFlagable
    {
        public required TKey Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; }

        public DateTime? DeleteDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = [];
    }
}
