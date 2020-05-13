using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Helpers
{
    public class PaginatedList<T>
    {
        public IList<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int DataCount { get; set; }
        public int PagesCount { get; set; }
    }
}
