using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces.Repositories;
using TMod.Blog.Domain.Interfaces.Services;
using TMod.Blog.Infrastructure.Utils;

namespace TMod.Blog.Infrastructure.Services
{
    internal class ShortCodeService : IShortCodeService
    {
        private readonly SnowflakeIdGenerator _snowflakeIdGenerator;
        private readonly ISiteConfigurationCacheService _configurationCacheService;
        private readonly ILogger<ShortCodeService> _logger;
        private readonly byte[] _mask;
        private readonly int _minLength;
        private readonly int _maxLength;

        public ShortCodeService(ISiteConfigurationCacheService configurationCacheService
            , ILogger<ShortCodeService> logger)
        {
            _configurationCacheService = configurationCacheService;
            _logger = logger;
            string secret = _configurationCacheService.GetConfiguration(SITE_SHORT_CODE_SECRET_KEY)!;
            if(!int.TryParse(_configurationCacheService.GetConfiguration(SITE_SHORT_CODE_WORKER_ID), out int workerId) )
            {
                workerId = 1;
            }
            if(!DateTime.TryParse(_configurationCacheService.GetConfiguration(SITE_SHORT_CODE_EPOCH),out DateTime epoch) )
            {
                epoch = DateTime.Parse("2025-01-01");
            }
            _snowflakeIdGenerator = new SnowflakeIdGenerator(workerId,epoch);
            _mask = DeriveMask(secret);
            if(!int.TryParse(_configurationCacheService.GetConfiguration(SITE_SHORT_CODE_MIN_LENGTH),out _minLength) )
            {
                _minLength = 4;
            }
            if(!int.TryParse(_configurationCacheService.GetConfiguration(SITE_SHORT_CODE_MAX_LENGTH),out _maxLength) )
            {
                _maxLength = 20;
            }
            _minLength = Math.Max(4, _minLength);
            _maxLength = Math.Max(_minLength, _maxLength);
        }

        private static byte[] DeriveMask(string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var digest = hmac.ComputeHash(Encoding.UTF8.GetBytes("shortcode-mask-v1"));
            var mask = new byte[8];
            Array.Copy(digest, mask, 8);
            return mask;
        }

        private ulong ApplyMask(ulong v)
        {
            var bytes = BitConverter.GetBytes(v); // little-endian
            for ( int i = 0; i < 8; i++ ) bytes[i] ^= _mask[i];
            return BitConverter.ToUInt64(bytes, 0);
        }

        public (DateTime createDate, int workerId, long sequence)? DecodeShortCode(string shortCode)
        {
            if ( string.IsNullOrWhiteSpace(shortCode) )
            {
                return null;
            }
            if(!Base62.TryDecode(shortCode,out var code) )
            {
                return null;
            }
            // 取消 mask
            var bytes = BitConverter.GetBytes(code);
            for ( int i = 0; i < 8; i++ ) bytes[i] ^= _mask[i];
            var original = BitConverter.ToUInt64(bytes, 0);

            // parse snowflake
            try
            {
                var parsed = _snowflakeIdGenerator.Parse(original);
                return parsed;
            }
            catch
            {
                return null;
            }
        }

        public Task<string> GenerateShortCodeAsync(int length = 8, CancellationToken cancellationToken = default)
        {
            length = Math.Min(Math.Max(_minLength, length), _maxLength);
            var id = _snowflakeIdGenerator.NextId();
            var obfuscated = ApplyMask(id);
            var encoded = Base62.Encode(obfuscated);
            return Task.FromResult(encoded);
        }
    }
}
