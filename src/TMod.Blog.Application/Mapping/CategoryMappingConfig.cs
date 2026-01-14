using Mapster;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Category;
using TMod.Blog.Domain.Entities;

namespace TMod.Blog.Application.Mapping
{
    internal class CategoryMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryDto>()
                .Map(dest => dest.ParentName, src => src.Parent == null ? "默认分类" : src.Parent.Name)
                .Map(dest => dest.Children, src => src.Children)
                .TwoWays();

            // Requests should map to DTOs only
            config.NewConfig<CreateCategoryRequest, CategoryDto>();
            config.NewConfig<UpdateCategoryRequest, CategoryDto>();
        }
    }
}
