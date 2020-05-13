using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TodoList.Data.DTOs;

namespace TodoList.Http.Responses
{
    public class AuthenticationResponse
    {
        public enum StatusEnum
        {
            OK=1,
            Fail=2
        }

        [JsonIgnore]
        public StatusEnum Status { get; set; }
        public UserDTO UserData { get; set; }
        public string Token { get; set; }
    }
}
