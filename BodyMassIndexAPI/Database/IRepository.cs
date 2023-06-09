﻿using BodyMassIndexAPI.Database.Entityes.Base;

namespace BodyMassIndexAPI.Database
{
    internal interface IRepository<T> : IDisposable where T : Entity
    {
        IEnumerable<T> GetAll();

        T Get(int id);

        T Add(T entity);

        void Update(T entity);

        void Delete(int id);
    }
}
