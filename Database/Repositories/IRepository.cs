using System;
using System.Collections.Generic;
using System.Data;

namespace AutoSalon.Database.Repositories
{
    public interface IRepository<T>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
} 