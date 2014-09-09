namespace Trillium.DAL.Interfaces
{
    using System.Collections.Generic;
    using Umbraco.Core.Persistence;

    public interface IRepository
    {
        T Single<T>(object primaryKey);

        T GetById<T>(object primaryKey);

        IEnumerable<T> Query<T>();

        int Count<T>();

        Page<T> PagedQuery<T>(long pageNumber, long itemsPerPage, string sql, params object[] args);
    }

    public interface IEditableRepository : IRepository
    {
        int Insert(object obj);

        int Delete(object obj);

        int Update(object obj, object primaryKeyValue);
    }
}
