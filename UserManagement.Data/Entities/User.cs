using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static UserManagement.Models.UserLog;

namespace UserManagement.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }

    public UserRecord ToUserRecord()
        => new()
        { Id = Id, Forename = Forename, Surname = Surname, DateOfBirth = DateOfBirth, Email = Email, IsActive = IsActive };
}
