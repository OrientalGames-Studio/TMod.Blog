using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Web.Models
{
	public class PagingResult
	{
		public static PagingResult Empty { get; } = new PagingResult([]);
		private int _pageIndex;
		private int _pageSize;
		private int _dataCount;
		private int _pageCount;

		public int PageIndex
		{
			get => Math.Max(1, Math.Min(_pageIndex, _pageCount));
			set
			{
				if ( _pageIndex != value )
				{
					_pageIndex = Math.Max(1, Math.Min(value, _pageCount));
				}
			}
		}

		public int PageSize
		{
			get => Math.Max(1, _pageSize);
			set
			{
				if(_pageSize != value )
				{
					_pageSize = Math.Max(1,value);
				}
			}
		}

		public int DataCount
		{
			get => Math.Max(0, _dataCount);
			set
			{
				if ( _dataCount != value )
				{
					_dataCount = Math.Max(0, value);
				}
			}
		}

		public int PageCount
		{
			get => Math.Max(1, _pageCount);
			set
			{
				if ( _pageCount != value )
				{
					_pageCount = Math.Max(1,value);
				}
			}
		}

		public IEnumerable<object> Data { get; set; } = [];

		public PagingResult()
		{

		}

		public PagingResult(IEnumerable<object> data)
		{
			Data = data;
		}

		public PagingResult(int pageIndex, int pageSize,int dataCount,int pageCount,IEnumerable<object> data)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			DataCount = dataCount;
			PageCount = pageCount;
			Data = data;
		}
	}

	public class PagingResult<T> : PagingResult
	{
		public static new PagingResult<T> Empty { get; } = new PagingResult<T>([]);
		public new IEnumerable<T> Data { get; set; } = [];

		public PagingResult()
		{

		}

		public PagingResult(IEnumerable<T> data) : base([])
		{
			Data = data;
		}

		public PagingResult(int pageIndex, int pageSize, int dataCount, int pageCount, IEnumerable<T> data) : base(pageIndex, pageSize, dataCount, pageCount, [])
		{
			Data = data;
		}
	}
}
