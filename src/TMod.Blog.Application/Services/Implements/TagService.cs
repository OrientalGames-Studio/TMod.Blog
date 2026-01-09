using MapsterMapper;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Dtos;
using TMod.Blog.Application.Requests.Tag;
using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces.UnitOfWorks;
using TMod.Blog.Infrastructure.Specifications;

namespace TMod.Blog.Application.Services.Implements
{
    internal class TagService : ITagService
    {
        private readonly ILogger<TagService> _logger;
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagService(ILogger<TagService> logger, IApplicationUnitOfWork unitOfWork,IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TagDto> AcquireTagAsync(CreateTagRequest request, CancellationToken token = default)
        {
            Tag? tag = await _unitOfWork.TagRepository.GetByNameAsync(request.Name,token);
            if(tag is null )
            {
                TagDto tagDto = _mapper.Map<TagDto>(request);
                tag = _mapper.Map<Tag>(tagDto);
                await _unitOfWork.TagRepository.AddAsync(tag, token);
                await _unitOfWork.SaveChangesAsync(token);
            }
            return _mapper.Map<TagDto>(tag);
        }

        public async Task<IReadOnlyList<TagDto>> AcquireTagsAsync(IEnumerable<CreateTagRequest> requests, CancellationToken token = default)
        {
            List<TagDto> tagDtos = new List<TagDto>();
            foreach (var request in requests)
            {
                TagDto tagDto = await AcquireTagAsync(request, token);
                tagDtos.Add(tagDto);
            }
            return tagDtos;
        }

        public async Task<bool> DeleteTagAsync(Guid tagId, CancellationToken token = default)
        {
            Tag? tag = await _unitOfWork.TagRepository.GetEntityByIdAsync(tagId,false,token);
            if(tag is null )
            {
                return false;
            }
            if ( tag.Articles.Any() )
            {
                _logger.LogWarning("删除 Tag 时，还有文章关联到这个 Tag，不允许删除");
                return false;
            }
            _unitOfWork.TagRepository.Delete(tag);
            await _unitOfWork.SaveChangesAsync(token);
            return true;
        }

        public async Task<IReadOnlyList<TagDto>> GetArticleTagsAsync(Guid articleId, CancellationToken token = default)
        {
            Article? article = await _unitOfWork.ArticleRepository.GetEntityByIdAsync(articleId,true,token);
            if(article is null )
            {
                return [];
            }
            var specification = TagSpecification.CreateGetAllByArticle(articleId);
            var tags = await _unitOfWork.TagRepository.GetAllEntitiesAsync(specification, token);
            return _mapper.Map<IReadOnlyList<TagDto>>(tags);
        }

        public async Task<PagingDto<TagDto>> PagingTagsAsync(string? keyword = null, int pageIndex = 1, int pageSize = 20, CancellationToken token = default)
        {
            pageIndex = Math.Max(1, pageIndex);
            int skip = (pageIndex - 1) * pageSize;
            var specification = TagSpecification.CreatePaging(keyword,skip,pageSize);
            int totalCount = await _unitOfWork.TagRepository.CountAsync(specification,token);
            var tags = await _unitOfWork.TagRepository.GetAllEntitiesAsync(specification,token);
            int pageCount = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            pageCount = Math.Max(1, pageCount);
            var tagDtos = _mapper.Map<IReadOnlyList<TagDto>>(tags);
            return new PagingDto<TagDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageCount = pageCount,
                DataCount = totalCount,
                Items = tagDtos
            };
        }

        public async Task<TagDto?> UpdateTagAsync(Guid tagId, UpdateTagRequest request, CancellationToken token = default)
        {
            Tag? tag = await _unitOfWork.TagRepository.GetEntityByIdAsync(tagId,false,token);
            if(tag is null || tag.IsDeleted)
            {
                return null;
            }
            TagDto tagDto = _mapper.Map<TagDto>(request);
            tag = _mapper.Map<Tag>(tagDto);
            _unitOfWork.TagRepository.Update(tag);
            await _unitOfWork.SaveChangesAsync(token);
            return _mapper.Map<TagDto>(tag);
        }
    }
}
