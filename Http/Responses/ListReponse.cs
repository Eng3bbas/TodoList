using System.Collections.Generic;
using TodoList.Helpers;

namespace TodoList.Http.Responses
{
    public class ListReponse<TEntity,TDTO>
    {
        public ListReponse(PaginatedList<TEntity> paginated,IList<TDTO> dtoList)
        {
            if(paginated.DataCount > 0)
                Pagination = new PaginationData
                {
                    CurrentPage = paginated.CurrentPage,
                    DataCount = paginated.DataCount,
                    PagesCount = paginated.PagesCount,
                    PageSize = paginated.PageSize
                };
            Data = dtoList;
        }
        public struct PaginationData
        {
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int DataCount { get; set; }
            public int PagesCount { get; set; }
        }
        public PaginationData Pagination { get; set; }
        public IList<TDTO> Data { get; set; }
    }
}