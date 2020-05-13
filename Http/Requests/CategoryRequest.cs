using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Http.Requests
{
    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
