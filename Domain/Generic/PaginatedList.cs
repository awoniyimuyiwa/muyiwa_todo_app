using System;
using System.Collections.Generic;

namespace Domain.Generic
{
    public class PaginatedList<T> : List<T>
    {
        public long Page { get; private set; }
        public long PerPage { get; private set; }
        public long From { get; private set; }
        public long To { get; private set; }
        public long Total { get; private set; }
        public long TotalPages { get; private set; }

        public PaginatedList(List<T> items, long total, long page, long perPage)
        {
            Page = page;
            PerPage = perPage;
            From = (PerPage * (Page - 1)) + 1;
            To = PerPage * Page;
            Total = total;
            TotalPages = (long)Math.Ceiling(total / (double)perPage);

            AddRange(items);
        }

        public bool HasPrevious
        {
            get
            {
                return (Page > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (Page < TotalPages);
            }
        }
    }
}
