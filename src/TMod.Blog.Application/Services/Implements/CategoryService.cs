using MapsterMapper;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Category;
using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository
            , IMapper mapper
            , ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDto?> ChangeParentCategoryAsync(Guid categoryId, PatchCategoryParentRequest request, CancellationToken cancellationToken = default)
        {
            Category? category = await _categoryRepository.GetEntityByIdAsync(categoryId,false,cancellationToken);
            if(category is null || category.IsDeleted)
            {
                _logger.LogError("分类不存在，分类Id：{CategoryId}", categoryId);
                throw new InvalidOperationException("分类不存在，无法修改父分类");
            }
            if(request.ParentId == Guid.Empty || request.ParentId == null )
            {
                category.ParentId = null;
            }
            else
            {
                Category? parent = await _categoryRepository.GetEntityByIdAsync(request.ParentId,true,cancellationToken);
                if ( parent is null || parent.IsDeleted )
                {
                    _logger.LogError("父分类不存在，分类Id：{CategoryId}", request.ParentId);
                    throw new NotSupportedException("父分类不存在，无法绑定到父分类");
                }
                category.ParentId = request.ParentId;
            }
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            Category category = _mapper.Map<Category>(request);
            if(request.ParentId != Guid.Empty && request.ParentId != null )
            {
                Category? parent = await _categoryRepository.GetEntityByIdAsync(request.ParentId,true,cancellationToken);
                if(parent is null || parent.IsDeleted )
                {
                    _logger.LogError("父分类不存在，分类Id：{CategoryId}", request.ParentId);
                    throw new NotSupportedException("父分类不存在，无法绑定到父分类");
                }
                category.Parent = parent;
            }
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            Category? meta = await _categoryRepository.GetEntityByIdAsync(categoryId,false,cancellationToken);
            if(meta is null || meta.IsDeleted )
            {
                return true;
            }
            if(meta.Children?.Count > 0 || meta.Articles?.Count > 0 )
            {
                _logger.LogWarning("尝试删除有子分类或文章的分类，分类Id:{}", categoryId);
                throw new InvalidOperationException("无法删除有子分类或文章的分类，请先删除子分类或文章");
            }
            _categoryRepository.Delete(meta);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            Category? category = await _categoryRepository.GetEntityByIdAsync(categoryId,true,cancellationToken);
            if(category is null || category.IsDeleted )
            {
                return null;
            }
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<PagingDto<CategoryDto>> PagingCategoriesByParentIdAsync(Guid? parentId = null, string? categoryName = null, int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            pageIndex = Math.Max(1, pageIndex);
            int skip = (pageIndex - 1) * pageSize;
            var specification = CategorySpecification.CreatePagingCategoriesByParentIdWithNameFilter(parentId,categoryName, skip, pageSize, false);
            int totalCount = await _categoryRepository.CountAsync(specification, cancellationToken);
            var categories = await _categoryRepository.GetAllEntitiesAsync(specification, cancellationToken);
            int pageCount = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            pageCount = Math.Max(1, pageCount);
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories)??[];
            return new PagingDto<CategoryDto>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageCount = pageCount,
                DataCount = totalCount,
                Items = categoryDtos
            };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            Category? category = await _categoryRepository.GetEntityByIdAsync(categoryId,false,cancellationToken);
            if(category is null || category.IsDeleted )
            {
                _logger.LogError("分类不存在，分类Id:{}",categoryId);
                throw new InvalidOperationException("分类不存在，无法更新分类信息");
            }
            if ( !category.Name.Equals(request.Name) )
            {
                category.Name = request.Name;
            }
            if ( category.Description?.Equals(request.Description) == false )
            {
                category.Description = request.Description;
            }
            if(category.ParentId != request.ParentId )
            {
                if(request.ParentId == Guid.Empty || request.ParentId == null )
                {
                    category.Parent = null;
                    category.ParentId = null;
                }
                else
                {
                    Category? parent = await _categoryRepository.GetEntityByIdAsync(request.ParentId,true,cancellationToken);
                    if ( parent is null || parent.IsDeleted )
                    {
                        _logger.LogError("父分类不存在，分类Id：{CategoryId}", request.ParentId);
                        throw new NotSupportedException("父分类不存在，无法绑定到父分类");
                    }
                    category.Parent = parent;
                }
            }
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
