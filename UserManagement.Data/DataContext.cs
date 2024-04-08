using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using System;
using static UserManagement.Models.UserLog;
using System.Collections.Generic;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    protected override void OnModelCreating(ModelBuilder model)
    {
        var users = new User[] {
            new() { Id = 1, Forename = "Peter", Surname = "Loew", DateOfBirth = new DateTime(1998, 5, 2) ,Email = "ploew@example.com", IsActive = true },
            new() { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates",  DateOfBirth = new DateTime(1967, 8, 27), Email = "bfgates@example.com", IsActive = true },
            new() { Id = 3, Forename = "Castor", Surname = "Troy", DateOfBirth = new DateTime(1972, 11, 10), Email = "ctroy@example.com", IsActive = false },
            new() { Id = 4, Forename = "Memphis", Surname = "Raines", DateOfBirth = new DateTime(1983, 9, 3), Email = "mraines@example.com", IsActive = true },
            new() { Id = 5, Forename = "Stanley", Surname = "Goodspeed", DateOfBirth = new DateTime(1950, 1, 1), Email = "sgodspeed@example.com", IsActive = true },
            new() { Id = 6, Forename = "H.I.", Surname = "McDunnough", DateOfBirth = new DateTime(2004, 12, 8), Email = "himcdunnough@example.com", IsActive = true },
            new() { Id = 7, Forename = "Cameron", Surname = "Poe", DateOfBirth = new DateTime(1992, 4, 14), Email = "cpoe@example.com", IsActive = false },
            new() { Id = 8, Forename = "Edward", Surname = "Malus", DateOfBirth = new DateTime(1977, 3, 23), Email = "emalus@example.com", IsActive = false },
            new() { Id = 9, Forename = "Damon", Surname = "Macready", DateOfBirth = new DateTime(1986, 7, 25), Email = "dmacready@example.com", IsActive = false },
            new() { Id = 10, Forename = "Johnny", Surname = "Blaze", DateOfBirth = new DateTime(1970, 1, 11), Email = "jblaze@example.com", IsActive = true },
            new() { Id = 11, Forename = "Robin", Surname = "Feld", DateOfBirth = new DateTime(2001, 7, 7), Email = "rfeld@example.com", IsActive = true },
        };

        model.Entity<User>().HasData(users);

        var userLogs = new List<UserLog>();
        int counter = 1;
        foreach (var user in users) {
            userLogs.Add(new() {Id = counter, NewState = user.ToUserRecord(), UserId = user.Id, Updated = DateTime.Now, Action = LogAction.CREATE });
            counter++;
        }

        model.Entity<UserLog>().Ignore(x => x.NewState);
        model.Entity<UserLog>().HasData(userLogs);
    }

    public DbSet<User>? Users { get; set; }
    
    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        => base.Set<TEntity>();

    public bool Create<TEntity>(TEntity entity) where TEntity : class
    {
        base.Add(entity);

        if (entity is User user)
        {
            addUserLog(user, LogAction.CREATE);
        }
        
        return SafeSaveChanges();
    }

    public new bool Update<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);

        if (entity is User user)
        {
            addUserLog(user, LogAction.UPDATE);
        }

        return SafeSaveChanges();
    }

    public bool Delete<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);

        if (entity is User user)
        {
            addUserLog(user, LogAction.DELETE);
        }

        return SafeSaveChanges();
    }

    private bool SafeSaveChanges() {
        try
        {
            SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void addUserLog(User user, LogAction action) {

        var priorState = GetAll<User>()
            .Where(x => x.Id == user.Id)
            .AsNoTracking()
            .FirstOrDefault();

        UserRecord? priorUser;
        if (priorState != null) {
            priorUser = priorState.ToUserRecord();
        }
        else {
            priorUser = null;
        }

        base.Add(new UserLog { NewState = user.ToUserRecord(), PriorState = priorUser, UserId = user.Id, Updated = DateTime.Now, Action = action });
    }
}
