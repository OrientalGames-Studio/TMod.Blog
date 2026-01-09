using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Application.Common.Results
{
    public record Result
    {
        public bool IsSuccess { get; set; }
        
        public string? Message { get; set; }

        public object? Data { get; set; }

        public static Result Ok(object? data, string? message = null)
        {
            Result result = new()
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
            return result;
        }

        public static Result Fail(object? data,string? message = null)
        {
            Result result = new()
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
            return result;
        }

        public static Result Paging(int dataCount, int pageCount,int pageIndex = 1,int pageSize = 20,object? data = null,string? message = null,bool isSuccess = true)
        {
            Result result = new()
            {
                IsSuccess = isSuccess,
                Message = message,
                Data = new
                {
                    PageIndex = Math.Max(1,pageIndex),
                    PageSize = Math.Max(1,pageSize),
                    DataCount = Math.Max(0,dataCount),
                    PageCount = Math.Max(1,pageCount),
                    Items = data
                }
            };
            return result;
        }
    }
}
