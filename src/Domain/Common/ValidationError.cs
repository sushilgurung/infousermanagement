using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class ValidationError
    {
        public ValidationError(string name, string reason)
        {
            Name = name != string.Empty ? name : null;
            Reason = reason;
        }

        public string Name { get; }
        public string Reason { get; }
    }
}
