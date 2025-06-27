using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User.Queries.GetUser
{
    public class GetUserRequestParameters : RequestParameters
    {
        public bool? IsActive { get; set; }
    }
}
