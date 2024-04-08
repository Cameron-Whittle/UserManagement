using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService(IDataContext dataAccess) : IUserService
{
    private readonly IDataContext _dataAccess = dataAccess;

    /// <inheritdoc />
    public IEnumerable<User> FilterByActive(bool isActive) 
        => _dataAccess.GetAll<User>().Where(x => x.IsActive == isActive);

    /// <inheritdoc />
    public IEnumerable<User> GetAllUsers()
        => _dataAccess.GetAll<User>();

    /// <inheritdoc />
    public User GetUserById(long id)
        => _dataAccess.GetAll<User>().Single(x => x.Id == id);

    public bool Update(User user) => _dataAccess.Update(user);

    /// <inheritdoc />
    public bool AddUser(User user)
        => _dataAccess.Create(user);

    /// <inheritdoc />
    public bool DeleteUserById(long id)
        => _dataAccess.Delete(GetUserById(id));

    /// <inheritdoc />
    public IEnumerable<UserLog> GetLogsByUserId(long id)
        => _dataAccess.GetAll<UserLog>().Where(x => x.UserId == id);
}
