using static UserManagement.Models.UserLog;
using System;

namespace UserManagement.Web.Models.Users;

public class UserAndLogViewModel
{
    public UserListItemViewModel User { get; set; } = new();

    public List<UserLogViewModel> Logs { get; set; } = new();

    public class UserLogViewModel {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime Updated { get; set; }
        public LogAction Action { get; set; }
    }
}
