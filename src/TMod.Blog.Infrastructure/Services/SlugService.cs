using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TMod.Blog.Domain.Entities;
using TMod.Blog.Domain.Interfaces;
using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.Services;

namespace TMod.Blog.Infrastructure.Services
{
    internal class SlugService : ISlugService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICharacterService _characterService;
        private readonly ILogger<SlugService> _logger;

        public SlugService(IArticleRepository articleRepository
            , ICharacterService characterService
            , ILogger<SlugService> logger)
        {
            _articleRepository = articleRepository;
            _characterService = characterService;
            _logger = logger;
        }

        public async Task<string> GenerateSlugAsync(string title,int? maxLength = null, CancellationToken cancellationToken = default)
        {
            if ( string.IsNullOrWhiteSpace(title) )
            {
                return "";
            }
            string spell = await _characterService.ParseCharacterToSpellAsync(title,Domain.Common.ChineseCharacterToSpellOptions.Default,cancellationToken);
            string normalizedTitle = spell.Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();
            string noSpecialCharsTitle = Regex.Replace(normalizedTitle,@"[^\p{L}\p{N}\s-]", " ");
            string hyphenatedTitle = Regex.Replace(noSpecialCharsTitle, @"\s+", "-");
            string noDuplicateHyphensTitle = Regex.Replace(hyphenatedTitle, @"-+", "-");
            string trimmedTitle = noDuplicateHyphensTitle.Trim('-');
            trimmedTitle = TrimSlug(trimmedTitle, maxLength.GetValueOrDefault());
            int slugUseCount = await _articleRepository.CountSlugAsync(trimmedTitle);
            if ( slugUseCount > 0 )
            {
                // 加上 8 位短码（只包含 a-z0-9）
                string randomSuffix = GenerateRandomSuffix(8);
                trimmedTitle = TrimSlug($"{trimmedTitle}-{randomSuffix}", maxLength.GetValueOrDefault());
            }
            return trimmedTitle;
        }

        private static string GenerateRandomSuffix(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = RandomNumberGenerator.Create();
            var buffer = new byte[length];
            random.GetBytes(buffer);

            var sb = new StringBuilder(length);
            foreach ( var b in buffer )
                sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }

        private static string TrimSlug(string input,int maxLength)
        {
            if ( maxLength > 0 && input.Length > maxLength )
            {
                // 确保截断后不以连字号结尾
                int length = maxLength;
                while ( length > 0 && input[^1] == '-' )
                {
                    length--;
                }
                input = input[..Math.Max(length, 0)];
            }
            return input;
        }
    }
}
