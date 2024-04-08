using System;
using System.ComponentModel.DataAnnotations;
using UserManagement.Web.Models.Validation;

namespace UserManagement.Web.Models.Users;

public class UserCreateViewModel
{
    [Required(ErrorMessage = "Forename is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Surname must be between 2 and 50 characters.")]
    public required string Forename { get; set; }

    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Surname must be between 2 and 50 characters.")]
    public required string Surname { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    [PastDate(ErrorMessage = "Date of Birth must be in the past.")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public required string Email { get; set; }
}
