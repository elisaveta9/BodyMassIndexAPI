﻿using BodyMassIndexAPI.Database.Context;
using BodyMassIndexAPI.Database.Entityes.Base;
using Microsoft.EntityFrameworkCore;

namespace BodyMassIndexAPI.Database
{
    internal class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly UsersDB _db;
        private readonly DbSet<T> _Set;

        public bool AutoSaveChanges { get; set; } = true;

        public Repository(UsersDB db)
        {
            _db = db;
            _Set = _db.Set<T>();
        }

        public virtual IQueryable<T> Items => _Set;

        public IEnumerable<T> GetAll() => _Set;

        public T Get(int id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<T> GetAsync(int id, CancellationToken Cancel = default) => await Items
           .SingleOrDefaultAsync(item => item.Id == id, Cancel)
           .ConfigureAwait(false);

        public T Add(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            return item;
        }

        public async Task<T> AddAsync(T item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
            return item;
        }

        public void Update(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task UpdateAsync(T item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        public void Delete(int id)
        {
            var item = _Set.Where(item => item.Id == id).FirstOrDefault();

            if (item != null)
                _db.Remove(item);

            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
