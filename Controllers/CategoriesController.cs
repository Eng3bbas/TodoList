using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Http.Requests;
using TodoList.Services;


namespace TodoList.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService service;
        public CategoriesController(CategoryService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> All([FromQuery]int page = 1)
            => Ok(await service.All(page));
        [Route("{id:int}")]
        public async Task<IActionResult> Show(int id)
        {
            var category = await service.Show(id);
            if (category.Status == Http.Responses.CategoryReponse.StatusEnum.Fail)
                return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var category = await service.Create(request);
            return Created("", category);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromForm] CategoryRequest request , int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var category = await service.Update(request, id);
            if (category.Status == Http.Responses.CategoryReponse.StatusEnum.Fail)
                return NotFound();
            return Ok(category);
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await service.Delete(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
