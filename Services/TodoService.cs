using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Data;
using TodoList.Data.DTOs;
using TodoList.Data.Entities;
using TodoList.Http.Responses;
using TodoList.Extenstions;
using TodoList.Http.Requests;

namespace TodoList.Services
{
    public class TodoService
    {
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;
        private readonly IRepository<Todo> repository;
        private readonly CategoryService categoryService;
        public TodoService(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Todo> repository,
            CategoryService categoryService)
        {
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
            this.repository = repository;
            this.categoryService = categoryService;
        }

        public async Task<ListReponse<Todo,TodoDTO>> All(int page)
        {
            var data = (await repository.Filter(t => t.UserId == GetUserId())).AsQueryable().OrderByDescending(t => t.Id).Paginate(page);
            return new ListReponse<Todo, TodoDTO>(
                data,
                mapper.Map<IList<TodoDTO>>(data.Data)
                );
        }


        public async Task<TodoResponse> Create(TodoRequest request)
        {
            var userId = GetUserId();
            if (!(await categoryService.IsCategoryBelongsToThisUser(userId, request.CategoryId)))
                return new TodoResponse
                {
                    Error = "This category does not belong to this user"
                };
            var todo = await repository.Create(new Todo
            {
                Body = request.Body,
                Completed = request.Completed,
                CategoryId = request.CategoryId,
                UserId = userId
            });
            return new TodoResponse
            {
                Todo = MapTodoEntity(todo)
            };
        }
        
        public async Task<TodoResponse> Show(int id)
        {
            var todo = await repository.Find(id);
            if (IsNotTodoAvalible(todo))
                return new TodoResponse
                {
                    Error = "this todo is not found with this id:" + id
                };
            return new TodoResponse
            {
                Todo = MapTodoEntity(todo)
            };
        }
        public async Task<TodoResponse> Update(int id, TodoRequest request)
        {
            var todo = await repository.Find(id);
            if (IsNotTodoAvalible(todo))
                return new TodoResponse
                {
                    Error = "this todo is not found with this id:" + id
                };
            if (!await categoryService.IsCategoryBelongsToThisUser(GetUserId(), request.CategoryId))
                return new TodoResponse
                {
                    Error = "this category is unauthroized to use"
                };
            todo.Body = request.Body;
            todo.CategoryId = request.CategoryId;
            todo.Completed = request.Completed;
            return new TodoResponse
            {
                Todo = MapTodoEntity(await repository.Update(todo))
            };
        }
        public async Task<bool> Delete(int id)
        {
            var todo = await repository.Find(id);
            if (IsNotTodoAvalible(todo))
                return false;
            await repository.Delete(todo);
            return true;
        }
        private bool IsNotTodoAvalible(Todo? entitiy) => 
            entitiy == null || entitiy.UserId != GetUserId();

        private TodoDTO MapTodoEntity(Todo todo) => mapper.Map<TodoDTO>(todo);
        private Guid GetUserId() => Guid.Parse(httpContext.User.FindFirst("userId").Value);
    }
}
