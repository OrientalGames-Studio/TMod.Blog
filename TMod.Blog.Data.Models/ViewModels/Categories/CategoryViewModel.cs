using System.ComponentModel.DataAnnotations;

using TMod.Blog.Data.Interfaces;

namespace TMod.Blog.Data.Models.ViewModels.Categories
{
    public sealed class CategoryViewModel: IVersionControl, ICreate, IUpdate, IRemove, IIntKey, IKey<int>
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分类标签
        /// </summary>
        [StringLength(20)]
        public string Category { get; set; } = null!;

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsRemove { get; set; }

        /// <summary>
        /// 分类版本
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

        public static implicit operator Category?(CategoryViewModel? model)
        {
            if(model is null )
            {
                return null;
            }
            Category result = new Category()
            {
                Id = model.Id,
                Category1 = model.Category,
                IsRemove = model.IsRemove,
                Version = model.Version,
                CreateDate = model.CreateDate,
                UpdateDate = model.UpdateDate,
                RemoveDate = model.RemoveDate,
            };
            return result;
        }

        public static implicit operator CategoryViewModel?(Category? category)
        {
            if(category is null )
            {
                return null;
            }
            CategoryViewModel vm = new CategoryViewModel()
            {
                Id = category.Id,
                Category = category.Category1,
                IsRemove = category.IsRemove,
                Version = category.Version,
                CreateDate = category.CreateDate,
                UpdateDate = category.UpdateDate,
                RemoveDate = category.RemoveDate,
            };
            return vm;
        }
    }
}
