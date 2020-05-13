using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoList.Http.Requests;
using TodoList.Services;

namespace TodoList.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly TodoService service;
        public TodosController(TodoService service)
        {
            this.service = service;
        }
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> All([FromQuery]int page = 1)
        {
            return Ok(await service.All(page));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]TodoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var todo = await  service.Create(request);
            if (todo.Status == Http.Responses.TodoResponse.StatusEnum.Fail)
                return BadRequest(new { ClientError = todo.Error });
            return Created("", todo);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> Show(int id)
        {
            var todo = await service.Show(id);
            if (todo.Status == Http.Responses.TodoResponse.StatusEnum.Fail)
                return NotFound(new { ClientError = todo.Error });
            return Ok(todo);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update(int id,[FromForm]TodoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var todo = await service.Update(id, request);
            if (todo.Status == Http.Responses.TodoResponse.StatusEnum.Fail)
                return NotFound(new { ClientError = todo.Error });
            return Ok(todo);
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await service.Delete(id))
                return NotFound(new { ClientError = "No todo found" });
            return NoContent();
        }
    }
}