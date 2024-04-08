namespace UserManagement.Web.Models.Validation;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// A Validation Attribute to verify if a date is in the past
/// </summary>
public class PastDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (DateTime.TryParse(value.ToString(), out var dateValue))
        {
            return dateValue < DateTime.Now;
        }

        return false;
    }
}
