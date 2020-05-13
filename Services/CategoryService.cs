using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Data;
using TodoList.Data.Entities;
using TodoList.Helpers;
using TodoList.Extenstions;
using Microsoft.EntityFrameworkCore;
using TodoList.Http.Responses;
using AutoMapper;
using TodoList.Data.DTOs;
using TodoList.Http.Requests;

namespace TodoList.Services
{
    public class CategoryService
    {
        private readonly IRepository<Category> repository;
        private readonly HttpContext httpContext;
        private readonly IMapper mapper;
        public CategoryService(IRepository<Category> repository,
                               IHttpContextAccessor httpContext,
                               IMapper mapper)
        {
            this.repository = repository;
            this.httpContext = httpContext.HttpContext;
            this.mapper = mapper;
        }

        public async Task<ListReponse<Category,CategoryDTO>> All(int page = 1)
        {
            var data = (await repository.Filter(cat => cat.UserId == GetUserId()))
                .AsQueryable()
                .Include(cat => cat.User)
                .Paginate(page,20);
            return new ListReponse<Category,CategoryDTO>(data,mapper.Map<IList<CategoryDTO>>(data.Data));
        }

        public async Task<CategoryReponse> Show(int id)
        {
           
            var category = await repository.Find(id);
            if (category == null || category.UserId != GetUserId())
                return new CategoryReponse
                {
                    Error = "Category Not found"
                };
            return new CategoryReponse {
                Category = mapper.Map<CategoryDTO>(category)
            };
        }
        public async Task<CategoryReponse> Create(CategoryRequest request)
        {
            var category = await repository.Create(new Category
            {
                Name = request.Name,
                UserId = GetUserId()
            });
            return new CategoryReponse
            {
                Category = mapper.Map<CategoryDTO>(category)
            };
        }

        public async Task<CategoryReponse> Update(CategoryRequest request,int id)
        {

            var category = await repository.Find(id);
            if (category == null || category.UserId != GetUserId())
                return new CategoryReponse { Error = "Category Not found" };
            category.Name = request.Name;
            return new CategoryReponse
            {
                Category = mapper.Map<CategoryDTO>(await repository.Update(category))
            };
        }

        public async Task<bool> Delete(int id)
        {
            var category = await repository.Find(id);
            if (category == null || category.UserId != GetUserId())
                return false;
            
            await repository.Delete(category);
            return true;
        }
        public async Task<bool> IsCategoryBelongsToThisUser(Guid userId, int id)
        {
            return (await repository.Filter(c => c.Id == id && c.UserId == userId)).Any();
        }
        private Guid GetUserId() => Guid.Parse(httpContext.User.FindFirst("userId").Value);
    }
}
