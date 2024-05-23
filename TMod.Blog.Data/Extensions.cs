using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data
{
    public static class Extensions
    {
        public static IVersionControl UpdateVersionRecord(this IVersionControl versionControl)
        {
            versionControl.Version += 1;
            return versionControl;
        }

        public static IUpdate UpdateMetaRecord(this IUpdate update, bool updateVersion = false)
        {
            update.UpdateDate = DateTime.Now;
            update.Version += updateVersion ? 1 : 0;
            return update;
        }

        public static IRemove RemoveMetaRecord(this IRemove remove, bool removeVersion = false)
        {
            remove.RemoveDate = DateTime.Now;
            remove.IsRemove = true;
            remove.Version += removeVersion ? 1 : 0;
            return remove;
        }

        public static IEdit EditMetaRecord(this IEdit edit, bool editVersion = false)
        {
            edit.LastEditDate = DateTime.Now;
            edit.Version += editVersion ? 1 : 0;
            return edit;
        }

        public static ICreate CreateMetaRecord(this ICreate create)
        {
            create.CreateDate = DateTime.Now;
            return create;
        }
    }
}
