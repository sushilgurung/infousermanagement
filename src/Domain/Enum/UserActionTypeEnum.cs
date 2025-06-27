using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum UserActionTypeEnum
    {
        // CRUD Operations
        Created = 1,
        Viewed = 2,
        Updated = 3,
        Deleted = 4,

        // Authentication
        LoginSuccess = 9,
        LoginFailed = 10,
        Logout = 11,
        PasswordChanged = 12,
        PasswordResetRequested = 13,
        PasswordResetCompleted = 14,

    }

}
