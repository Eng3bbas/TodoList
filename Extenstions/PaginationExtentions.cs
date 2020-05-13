using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Extenstions
{
    public static class PaginationExtentions
    {
        public static PaginatedList<T> Paginate<T>(this IQueryable<T>  query,int page = 1 , int recoredsPerPage = 15)
        {
            int skipRows = (page - 1) * recoredsPerPage;
            return new PaginatedList<T>
            {
                Data = query.Skip(skipRows).Take(recoredsPerPage).ToList(),
                CurrentPage = page,
                PageSize = recoredsPerPage,
                DataCount = query.Count(),
                PagesCount = (int) Math.Ceiling(query.Count() / (double)recoredsPerPage)
            };
        }
    }
}
