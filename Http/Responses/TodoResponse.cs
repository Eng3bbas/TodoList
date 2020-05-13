using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TodoList.Data.DTOs;

namespace TodoList.Http.Responses
{
    public class TodoResponse
    {
        public enum StatusEnum
        {
            Fail = 1,
            Success = 2
        }
        [JsonIgnore]
        public StatusEnum Status
        {
            get
            {
                return string.IsNullOrEmpty(Error) ? StatusEnum.Success : StatusEnum.Fail;
            }
        }
        public TodoDTO Todo { get; set; }
        public string? Error { get; set; }
    }
}
