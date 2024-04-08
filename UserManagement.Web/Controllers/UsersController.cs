using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using static UserManagement.Web.Models.Users.UserAndLogViewModel;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    private readonly Random _rand = new();

    [HttpGet]
    public ViewResult List(bool? isActive, bool? userCreated, bool? userUpdated, bool? userDeleted) {

        ViewBag.UserCreated = userCreated;
        ViewBag.UserUpdated = userUpdated;
        ViewBag.UserDeleted = userDeleted;

        if (isActive is null)
        {
            return ToUserListView(_userService.GetAllUsers());
        }
        else
        {
            return ToUserListView(_userService.FilterByActive((bool)isActive));
        }
    }

    [HttpGet]
    [Route("Users/Add")]
    public ViewResult Add(string? errorMessage)
    {
        ViewBag.ErrorMessage = errorMessage;
        return View();
    }

    [HttpGet]
    [Route("Users/View")]
    public ViewResult View(long id)
    {
        var user = _userService.GetUserById(id);

        var userModel = ToUserItemViewModel(user);

        var userLogs = _userService.GetLogsByUserId(id);

        var logs = userLogs.Select(x =>new UserLogViewModel()
        {
            Id= x.Id,
            UserId= x.UserId,
            Action= x.Action,
            Updated= x.Updated
        });

        var model = new UserAndLogViewModel()
        {
            User = userModel,
            Logs = logs.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("Users/Edit")]
    public ViewResult Edit(long id, string? errorMessage)
    {
        ViewBag.ErrorMessage = errorMessage;

        var item = _userService.GetUserById(id);
        var model = ToUserItemViewModel(item);

        return View(model);
    }

    [HttpPost]
    [Route("Users/Create")]
    public RedirectToActionResult Create(UserCreateViewModel userCreateModel)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Add", "Users", new { ErrorMessage = FormatError(ModelState.Values) });
        }

        var user = new User
        {
            Forename = userCreateModel.Forename,
            Surname = userCreateModel.Surname,
            DateOfBirth = userCreateModel.DateOfBirth,
            Email = userCreateModel.Email,
            IsActive = false
        };

        var success = _userService.AddUser(user);

        return RedirectToAction("List", "Users", new { UserCreated = success });
    }

    [HttpPost]
    [Route("Users/Save")]
    public RedirectToActionResult Save(long id, UserCreateViewModel userModel)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Edit", "Users", new {Id = id, ErrorMessage = FormatError(ModelState.Values) });
        }

        var user = _userService.GetUserById(id);

        user.Forename = userModel.Forename;
        user.Surname = userModel.Surname;
        user.DateOfBirth = userModel.DateOfBirth;
        user.Email = userModel.Email;

        ViewBag.UserUpdated = _userService.Update(user);

        return RedirectToAction("List", "Users", new {UserUpdated = true });
    }

    [HttpGet]
    [Route("Users/Delete")]
    public RedirectToActionResult Delete(long id)
    {
        var success = _userService.DeleteUserById(id);

        return RedirectToAction("List", "Users", new { UserDeleted = success });
    }

    private static string FormatError(ModelStateDictionary.ValueEnumerable values) {
        return string.Join("<br/>", values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
    }

    private ViewResult ToUserListView(IEnumerable<User> users)
    {
        var items = users.Select(_ => ToUserItemViewModel(_));

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    private static UserListItemViewModel ToUserItemViewModel(User user) => new()
    {
        Id = user.Id,
        Forename = user.Forename,
        Surname = user.Surname,
        DateOfBirth = user.DateOfBirth.Date,
        Email = user.Email,
        IsActive = user.IsActive
    };
}
