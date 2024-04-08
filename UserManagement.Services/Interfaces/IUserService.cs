using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive">The state of the user</param>
    /// <returns>The Users</returns>
    IEnumerable<User> FilterByActive(bool isActive);

    /// <summary>
    /// Returns all users
    /// </summary>
    /// <returns>The Users</returns>
    IEnumerable<User> GetAllUsers();

    /// <summary>
    /// Returns a single user
    /// </summary>
    /// <param name="id">The id of the user being fetched</param>
    /// <returns>The User</returns>
    /// <exception cref="enitiyNotFound"> thrown if there is no user by given id</exception>>
    User GetUserById(long id);

    /// <summary>
    /// Creates a new User
    /// </summary>
    /// <param name="user">The user to be created</param>
    /// <returns>A bool representing the success or failure of the operation</returns>
    bool AddUser(User user);

    /// <summary>
    /// Updates a User
    /// </summary>
    /// <param name="user">The user to be updated</param>
    /// <returns>A bool representing the success or failure of the operation</returns>
    bool Update(User user);

    /// <summary>
    /// Deletes a User by Id
    /// </summary>
    /// <param name="id">The id of the user to be deleted</param>
    /// <returns>A bool representing the success or failure of the operation</returns>
    bool DeleteUserById(long id);

    /// <summary>
    /// Returns all Logs for a given user Id
    /// </summary>
    /// <param name="id">The user Id for the logs to fetch</param>
    /// <returns>The Users Logs</returns>
    public IEnumerable<UserLog> GetLogsByUserId(long id);
}
