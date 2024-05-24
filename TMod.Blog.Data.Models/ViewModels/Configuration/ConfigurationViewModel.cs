using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Configuration
{
    public sealed class ConfigurationViewModel : IVersionControl, ICreate, IUpdate, IRemove, IIntKey, IKey<int>
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 配置项键
        /// </summary>
        [StringLength(50)]
        public string Key { get; set; } = null!;

        /// <summary>
        /// 配置项值
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsRemove { get; set; }

        /// <summary>
        /// 配置项版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 删除日期
        /// </summary>
        public DateTime? RemoveDate { get; set; }

        public static implicit operator TMod.Blog.Data.Models.Configuration?(ConfigurationViewModel? model)
        {
            if(model is null )
            {
                return null;
            }
            return new Models.Configuration()
            {
                Id = model.Id,
                Key = model.Key,
                Value = model.Value,
                Version = model.Version,
                CreateDate = model.CreateDate,
                UpdateDate = model.UpdateDate,
                RemoveDate = model.RemoveDate,
                IsRemove = model.IsRemove,
            };
        }

        public static implicit operator ConfigurationViewModel?(TMod.Blog.Data.Models.Configuration? model)
        {
            if ( model is null )
            {
                return null;
            }
            return new ConfigurationViewModel()
            {
                Id = model.Id,
                Key = model.Key,
                Value = model.Value,
                Version = model.Version,
                CreateDate = model.CreateDate,
                UpdateDate = model.UpdateDate,
                RemoveDate = model.RemoveDate,
                IsRemove = model.IsRemove,
            };
        }
    }
}
