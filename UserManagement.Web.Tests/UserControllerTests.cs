using System.Collections.Generic;
using System;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using static UserManagement.Models.UserLog;
using System.Linq;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        var controller = new UsersController(userService);
        var users = SetupUsers();

        userService.GetAllUsers().Returns(users);

        // Act
        var result = controller.List(null, null, null, null);

        // Assert
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void View_WhenServiceReturnsUsersandLogs_ModelMustContainUserandLogs()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        var controller = new UsersController(userService);

        var userLogs = SetupUserLogs();
        var userId = 1;
        var user = new User { Id = userId, Forename = "John", Surname = "Doe", IsActive = true };

        userService.GetUserById(userId).Returns(user);
        userService.GetLogsByUserId(userId).Returns(userLogs);

        // Act
        var result = controller.View(userId);
        var model = result.ViewData.Model as UserAndLogViewModel;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(user.Id, model.User.Id);
        Assert.Equal(user.Forename, model.User.Forename);
        Assert.Equal(user.Surname, model.User.Surname);
        Assert.Equal(userLogs.Count(), model.Logs.Count());
    }

    [Fact]
    public void Create_WhenModelStateIsInvalid_RedirectsToAddActionWithErrorMessage()
    {
        // Arrange
        var userCreateModel = Substitute.For<UserCreateViewModel>();
        var userService = Substitute.For<IUserService>();
        var controller = new UsersController(userService);
        controller.ModelState.AddModelError("DateOfBirth", "Date of Birth must be in the past.");

        // Act
        var result = controller.Create(userCreateModel);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.RouteValues);
        Assert.Equal("Add", result.ActionName);
        Assert.Equal("Users", result.ControllerName);
        Assert.Single(result.RouteValues);
        Assert.True(result.RouteValues.ContainsKey("ErrorMessage"));
        Assert.Contains("Date of Birth must be in the past.", result.RouteValues["ErrorMessage"]!.ToString());
    }

    [Fact]
    public void Create_WhenModelStateIsValid_RedirectsToListActionWithSuccessIndicator()
    {
        // Arrange
        var userCreateModel = new UserCreateViewModel
        {
            Forename = "John",
            Surname = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-1),
            Email = "john.doe@example.com"
        };
        var userService = Substitute.For<IUserService>();
        var controller = new UsersController(userService);

        userService.AddUser(Arg.Any<User>()).Returns(true);

        // Act
        var result = controller.Create(userCreateModel);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.RouteValues);
        Assert.Equal("List", result.ActionName);
        Assert.Equal("Users", result.ControllerName);
        Assert.Single(result.RouteValues);
        Assert.True(result.RouteValues.ContainsKey("UserCreated"));
        Assert.True((bool)result.RouteValues["UserCreated"]!);
    }

    [Fact]
    public void Save_WhenModelStateIsInvalid_RedirectsToEditActionWithErrorMessage()
    {
        // Arrange
        var userCreateModel = Substitute.For<UserCreateViewModel>();
        var userService = Substitute.For<IUserService>();
        var controller = new UsersController(userService);
        long id = 1;
        controller.ModelState.AddModelError("DateOfBirth", "Date of Birth must be in the past.");

        // Act
        var result = controller.Save(id, userCreateModel);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.RouteValues);
        Assert.Equal("Edit", result.ActionName);
        Assert.Equal("Users", result.ControllerName);
        Assert.Equal(id, result.RouteValues["Id"]);
        Assert.True(result.RouteValues.ContainsKey("ErrorMessage"));
        Assert.Contains("Date of Birth must be in the past.", result.RouteValues["ErrorMessage"]!.ToString());
    }

    [Fact]
    public void Save_WhenModelStateIsValid_ToListActionWithSuccessIndicator()
    {
        // Arrange
        var userCreateModel = new UserCreateViewModel
        {
            Forename = "John",
            Surname = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-1),
            Email = "john.doe@example.com"
        };
        var userService = Substitute.For<IUserService>();
        var controller = new UsersController(userService);
        var id = 1;
        var user = new User { Id = id };

        userService.GetUserById(id).Returns(user);
        userService.Update(user).Returns(true);

        // Act
        var result = controller.Save(id, userCreateModel);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.RouteValues);
        Assert.Equal("List", result.ActionName);
        Assert.Equal("Users", result.ControllerName);
        Assert.Single(result.RouteValues);
        Assert.True(result.RouteValues.ContainsKey("UserUpdated"));
        Assert.True((bool)result.RouteValues["UserUpdated"]!);
    }

        private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        };

        return users;
    }

    private IEnumerable<UserLog> SetupUserLogs()
    {
        var recordState = new UserRecord { Id = 1, Forename = "", Surname = "", DateOfBirth = DateTime.Now, Email = "", IsActive = true };

        var userLogs = new List<UserLog>
        {
            new UserLog { Id = 1, UserId = 1, Action = LogAction.CREATE, Updated = DateTime.Now, NewState = recordState },
            new UserLog { Id = 2, UserId = 1, Action = LogAction.UPDATE, Updated = DateTime.Now, NewState = recordState }
        };

        return userLogs;
    }
}
