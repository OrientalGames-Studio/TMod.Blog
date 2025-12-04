using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Category;

namespace TMod.Blog.Application.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request,CancellationToken cancellationToken = default);

        Task<CategoryDto?> ChangeParentCategoryAsync(Guid categoryId, PatchCategoryParentRequest request, CancellationToken cancellationToken = default);

        Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);

        Task<bool> DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

        Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

        Task<PagingDto<CategoryDto>> PagingCategoriesByParentIdAsync(Guid? parentId = null, string? categoryName = null, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    }
}
