using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Http.Requests
{
    public class TodoRequest
    {
       
        [Required]
        public string Body { get; set; }
        public bool Completed { get; set; } = false;
        [Required]
        public int CategoryId { get; set; }
    }
}
