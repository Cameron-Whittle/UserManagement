using System.Linq;
using UserManagement.Models;
using static UserManagement.Models.UserLog;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void Create_User_ShouldCreateLogEntity()
    {
        // Arrange
        var context = CreateContext();
        var user = new User
        {
            Forename = "create",
            Surname = "test",
            Email = "create@example.com"
        };

        // Act
        context.Create(user);
        var addedLog = context.GetAll<UserLog>().LastOrDefault();
        var addedUser = context.GetAll<User>().SingleOrDefault(x => x.Email == user.Email);

        // Assert
        Assert.NotNull(addedLog);
        Assert.NotNull(addedUser);
        Assert.Equal(addedUser.Id, addedLog.UserId);
        Assert.Equal(addedUser.Forename, addedLog.NewState.Forename);
        Assert.Equal(LogAction.CREATE, addedLog.Action);
        Assert.NotNull(addedLog.Updated);
    }

    [Fact]
    public void Update_User_ShouldCreateLogEntity()
    {
        // Arrange
        var context = CreateContext();
        var user = new User
        {
            Forename = "Update",
            Surname = "Test",
            Email = "updatetest@example.com"
        };

        // Add user to context
        context.Create(user);

        // Act
        user.Forename = "I'm updated!";
        context.Update(user);
        var addedLog = context.GetAll<UserLog>().LastOrDefault();
        var updatedUser = context.GetAll<User>().SingleOrDefault(x => x.Email == user.Email);

        // Assert
        Assert.NotNull(addedLog);
        Assert.NotNull(updatedUser);
        Assert.Equal(updatedUser.Id, addedLog.UserId);
        Assert.Equal("Update", addedLog.PriorState!.Forename);
        Assert.Equal("I'm updated!", addedLog.NewState.Forename);
        Assert.Equal(LogAction.UPDATE, addedLog.Action);
        Assert.NotNull(addedLog.Updated);
    }

    private DataContext CreateContext() => new();
}
