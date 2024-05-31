using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Data.Models.DTO.Articles
{
    public sealed class AddArticleArchiveModel
    {
        private readonly Stream _contentStream;

        public string FileName { get; set; } = null!;

        public string MIMEType { get; set; } = "application/octet-stream";

        public double FileSizeMB { get; set; } = 0d;

        public AddArticleArchiveModel(Stream contentStream)
        {
            _contentStream = contentStream;
        }

        public byte[] ReadArchiveContent()
        {
            byte[] dataBytes = [];
            if ( _contentStream.CanRead )
            {
                dataBytes = new byte[_contentStream.Length ];
                _contentStream.Read(dataBytes, 0, dataBytes.Length);
            }
            return dataBytes;
        }

        public async Task<byte[]> ReadArchiveContentAsync()
        {
            byte[] dataBytes = [];
            if(_contentStream.CanRead )
            {
                dataBytes = new byte[_contentStream.Length];
                await _contentStream.ReadAsync(dataBytes, 0, dataBytes.Length);
            }
            return dataBytes;
        }
    }
}
