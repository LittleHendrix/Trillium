namespace Trillium.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Trillium.Core.Interfaces;
    using Umbraco.Core.Persistence;

    public class PpGenericRepository<T> : IRepository<T> where T : class
    {
        private readonly Database _db;

        public PpGenericRepository(Database db)
        {
            this._db = db;
        }

        public virtual T GetById(int id)
        {
            return this._db.SingleOrDefault<T>(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this._db.Query<T>("SELECT * FROM @0", typeof(T));
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return this._db.Query<T>("SELECT * FROM @0 WHERE @1", typeof(T), predicate);
        }

        public virtual void Create(T entity)
        {
            this._db.Insert(typeof(T).ToString(), "id", entity);
        }

        public virtual void Update(T entity)
        {
            this._db.Update(typeof(T).ToString(), "id", entity);
        }

        public virtual void Delete(T entity)
        {
            this._db.Delete(typeof(T).ToString(), "id", entity);
        }
    }
}