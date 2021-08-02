using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class ListResult<T>
    {
        public List<T> Data { get; set; }

        public DateTime ServerDateTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public int? From
        {
            get
            {
                int? from = null;
                if (PageNumber.HasValue && PageSize.HasValue)
                {
                    from = (PageNumber - 1) * PageSize + 1;
                }
                if (from <= TotalCount)
                {
                    return from;
                }
                return null;
            }
        }

        public int? To
        {
            get
            {
                int? to = null;
                if (From.HasValue)
                {
                    to = From + PageSize - 1;
                }
                if (to <= TotalCount)
                {
                    return to;
                }
                return null;
            }
        }

        public long TotalCount { get; set; }

        public ListResult()
        {
            Data = new List<T>();
        }

        public bool HasData
        {
            get
            {
                return Data.Count > 0;
            }
        }

        public ListResult<TOut> CopyFrom<TOut, TIn>(ListResult<TIn> source, Func<TIn, TOut> projector)
        {
            var target = new ListResult<TOut>
            {
                PageNumber = source.PageNumber,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                Data = new List<TOut>()
            };
            foreach (var item in source.Data)
            {
                target.Data.Add(projector(item));
            }
            return target;
        }
        
        public void CopyMetaDataFrom<TOut, TIn>(ListResult<TIn> source, ListResult<TOut> dest)
        {
            dest.PageNumber = source.PageNumber;
            dest.PageSize = source.PageSize;
            dest.TotalCount = source.TotalCount;
            dest.TotalCount = source.TotalCount;
        }
    }
}
