using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User.Commands.CreateUser
{
    public class CreateUserDto
    {
        public string ForeName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }
}
