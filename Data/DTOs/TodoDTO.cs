using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Data.DTOs
{
    public class TodoDTO
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public bool Completed { get; set; } = false;
        public Guid UserId { get; set; }
        public UserDTO User { get; set; }
        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
