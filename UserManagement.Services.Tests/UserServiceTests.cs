using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using static UserManagement.Models.UserLog;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange
        var dataContext = Substitute.For<IDataContext>();
        var service = new UserService(dataContext);
        var users = SetupUsers();

        dataContext.GetAll<User>().Returns(users);

        // Act
        var result = service.GetAllUsers();

        // Assert
        Assert.Same(users, result);
    }
    [Fact]
    public void FilterByActive_WhenCalledWithTrue_ReturnsOnlyActiveUsers()
    {
        // Arrange
        var dataAccess = Substitute.For<IDataContext>();
        var service = new UserService(dataAccess);
        var users = SetupUsers().ToList(); 

        dataAccess.GetAll<User>().Returns(users.AsQueryable());

        // Act
        var result = service.FilterByActive(true);

        // Assert
        Assert.All(result, x => Assert.True(x.IsActive)); 
    }

    [Fact]
    public void FilterByActive_WhenCalledWithFalse_ReturnsOnlyInactiveUsers()
    {
        // Arrange
        var dataAccess = Substitute.For<IDataContext>();
        var service = new UserService(dataAccess);
        var users = SetupUsers().ToList(); 

        dataAccess.GetAll<User>().Returns(users.AsQueryable());

        // Act
        var result = service.FilterByActive(false);

        // Assert
        Assert.All(result, x => Assert.False(x.IsActive)); 
    }

    [Fact]
    public void GetUserById_WhenUserExists_ReturnsCorrectUser()
    {
        // Arrange
        var dataAccess = Substitute.For<IDataContext>();
        var service = new UserService(dataAccess);
        var users = SetupUsers().ToList();
        var userId = 1;

        dataAccess.GetAll<User>().Returns(users.AsQueryable());

        // Act
        var result = service.GetUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public void GetLogsByUserId_WhenLogsExistForUser_ReturnsCorrectLogs()
    {
        // Arrange
        var dataAccess = Substitute.For<IDataContext>();
        var service = new UserService(dataAccess);
        var userLogs = SetupUserLogs().ToList();
        var userId = 1;

        dataAccess.GetAll<UserLog>().Returns(userLogs.AsQueryable());

        // Act
        var result = service.GetLogsByUserId(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userLogs.Where(log => log.UserId == userId), result);
    }

        [Fact]
    public void GetLogsByUserId_WhenNoLogsExistForUser_ReturnsEmptyList()
    {
        // Arrange
        var dataAccess = Substitute.For<IDataContext>();
        var service = new UserService(dataAccess);
        var userLogs = SetupUserLogs().ToList();
        var userId = 100;

        dataAccess.GetAll<UserLog>().Returns(userLogs.AsQueryable());

        // Act
        var result = service.GetLogsByUserId(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

        private IQueryable<User> SetupUsers()
    {
        var users = new[]
        {
            new User
            {
                Id = 1,
                Forename = "Johnny",
                Surname =  "User",
                Email = "juser@example.com",
                IsActive = true
            },
            new User
            {
                Id = 2,
                Forename = "Alice",
                Surname =  "User",
                Email = "auser@example.com",
                IsActive = false
            },
        }.AsQueryable();

        return users;
    }

    private IQueryable<UserLog> SetupUserLogs()
    {
        var recordState = new UserRecord { Id = 1, Forename = "", Surname = "", DateOfBirth = DateTime.Now, Email = "", IsActive = true };

        var userLogs = new[]
        {
            new UserLog { UserId = 1, Action = LogAction.CREATE, NewState = recordState},
            new UserLog { UserId = 2, Action = LogAction.DELETE, NewState = recordState},
            new UserLog { UserId = 1, Action = LogAction.UPDATE, NewState = recordState },
            new UserLog { UserId = 3, Action = LogAction.CREATE, NewState = recordState }
        }.AsQueryable();

        return userLogs;
    }

}
