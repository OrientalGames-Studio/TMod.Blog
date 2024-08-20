using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

        public static IRemove RemoveMetaRecord(this IRemove remove, bool updateVersion = false)
        {
            remove.RemoveDate = DateTime.Now;
            remove.IsRemove = true;
            remove.Version += updateVersion ? 1 : 0;
            return remove;
        }

        public static IEdit EditMetaRecord(this IEdit edit, bool updateVersion = false)
        {
            edit.LastEditDate = DateTime.Now;
            edit.Version += updateVersion ? 1 : 0;
            return edit;
        }

        public static ICreate CreateMetaRecord(this ICreate create)
        {
            create.CreateDate = DateTime.Now;
            return create;
        }

        public static string? GetDescription(this object? obj)
        {
            if(obj is null )
            {
                return null;
            }
            Type type = obj.GetType();
            DescriptionAttribute? description = type.GetCustomAttribute<DescriptionAttribute>();
            if ( type.IsEnum )
            {
                FieldInfo? field = type.GetFields().FirstOrDefault(p=>!p.IsSpecialName && StringComparer.InvariantCulture.Compare(p.Name,obj?.ToString()) == 0);
                if(field is null )
                {
                    return null;
                }
                description = field.GetCustomAttribute<DescriptionAttribute>();
            }
            if(description is null )
            {
                return null;
            }
            return description.Description;
        }
    }
}
