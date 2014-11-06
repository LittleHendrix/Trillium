namespace Trillium.DAL.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Trillium.DAL.Interfaces;
    using Umbraco.Core.Persistence;

    public class PetaPocoRepository : IEditableRepository
    {
        private readonly IUnitOfWork uow;

        public PetaPocoRepository(IUnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        public T Single<T>(object primaryKey)
        {
            return this.uow.Db.Single<T>(primaryKey);
        }

        public T GetById<T>(object primaryKey)
        {
            return this.uow.Db.SingleOrDefault<T>(primaryKey);
        }

        public int Count<T>()
        {
            var pd = Database.PocoData.ForType(typeof(T));

            return this.uow.Db.ExecuteScalar<int>("SELECT Count(*) FROM @0", pd.TableInfo.TableName);
        }

        public IEnumerable<T> Query<T>()
        {
            var pd = Database.PocoData.ForType(typeof(T));

            return this.uow.Db.Query<T>("SELECT * FROM @0", pd.TableInfo.TableName);
        }

        public IEnumerable<T> Query<T>(
            string where = "",
            string orderBy = "",
            int limit = 0,
            string columns = "*",
            params object[] args)
        {
            var pd = Database.PocoData.ForType(typeof(T));
            string sql = BuildSql(pd.TableInfo.TableName, where, orderBy, limit, columns);

            return this.uow.Db.Query<T>(sql, args);
        }

        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            return this.uow.Db.Query<T>(sql, args);
        }

        public Page<T> PagedQuery<T>(long pageNumber, long itemsPerPage, string sql, params object[] args)
        {
            return this.uow.Db.Page<T>(pageNumber, itemsPerPage, sql) as Page<T>;
        }

        public int Insert(object poco)
        {
            return (int)this.uow.Db.Insert(poco);
        }

        public int Insert(string tableName, string primaryKeyName, object poco)
        {
            return (int)this.uow.Db.Insert(tableName, primaryKeyName, poco);
        }

        public int Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            return (int)this.uow.Db.Insert(tableName, primaryKeyName, autoIncrement, poco);
        }

        public int Update(object poco)
        {
            return this.uow.Db.Update(poco);
        }

        public int Update(object poco, object primaryKeyValue)
        {
            return this.uow.Db.Update(poco, primaryKeyValue);
        }

        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return this.uow.Db.Update(tableName, primaryKeyName, poco);
        }

        public int Delete(object poco)
        {
            return this.uow.Db.Delete(poco);
        }

        public int Delete<T>(object pocoOrPrimaryKey)
        {
            return this.uow.Db.Delete<T>(pocoOrPrimaryKey);
        }

        public static string BuildSql(
            string tableName,
            string where = "",
            string orderBy = "",
            int limit = 0,
            string columns = "*")
        {
            var sb = new StringBuilder(limit > 0 ? "SELECT TOP " + limit + " {0} FROM {1} " :
            "SELECT {0} FROM {1} ");

            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(
                    where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase)
                        ? where
                        : "WHERE " + where);
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sb.Append(
                    orderBy.Trim().StartsWith("order by", StringComparison.CurrentCultureIgnoreCase)
                        ? orderBy
                        : " ORDER BY " + orderBy);
            }

            return string.Format(sb.ToString(), columns, tableName);
        }
    }
}