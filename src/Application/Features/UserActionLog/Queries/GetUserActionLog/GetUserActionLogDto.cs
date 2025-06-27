using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserActionLog.Queries.UserActionLog
{
    public class GetUserActionLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; }
        public string ResourceType { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
