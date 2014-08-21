namespace Trillium.Core.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    interface IRepository<T> where T : class
    {
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        T GetById(int id);
    }

    interface IEditableRepository<T> : IRepository<T> where T : class
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
