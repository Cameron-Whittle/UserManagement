using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

public class UserLog
{
    public enum LogAction
    {
        CREATE,
        READ,
        UPDATE,
        DELETE
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public required long UserId { get; set; }
    public UserRecord? PriorState { get; set; }
    public required UserRecord NewState { get; set; }
    public DateTime Updated { get; set; }
    public required LogAction Action { get; set; }

    public class UserRecord {
        public long Id { get; set; }
        public required string Forename { get; set; }
        public required string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
