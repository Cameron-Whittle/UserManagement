﻿using System.Linq;

namespace UserManagement.Data;

public interface IDataContext
{
    /// <summary>
    /// Get a list of items
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns>A bool representing the success or failure of the operation</returns>
    bool Create<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Uodate an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns>A bool representing the success or failure of the operation</returns>
    bool Update<TEntity>(TEntity entity) where TEntity : class;

    bool Delete<TEntity>(TEntity entity) where TEntity : class;
}
