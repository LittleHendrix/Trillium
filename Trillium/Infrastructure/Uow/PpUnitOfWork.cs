namespace Trillium.Infrastructure.Uow
{
    using Trillium.Core.Interfaces;
    using Trillium.Infrastructure.Repositories;
    using Trillium.Models;
    using Umbraco.Core;
    using Umbraco.Core.Persistence;

    public class PpUnitOfWork : IUnitOfWork
    {
        private readonly PpGenericRepository<BlogComment> _blogCommentsRepository;

        private readonly Database _db;

        private readonly Transaction _petaTransaction;

        public PpUnitOfWork()
        {
            this._db = ApplicationContext.Current.DatabaseContext.Database;
            this._petaTransaction = new Transaction(this._db);

            this._blogCommentsRepository = new PpGenericRepository<BlogComment>(this._db);
        }

        public IRepository<BlogComment> BlogCommentsRepository
        {
            get { return this._blogCommentsRepository; }
        }

        public void Commit()
        {
            this._petaTransaction.Complete();
        }

        public void Dispose()
        {
            this._petaTransaction.Dispose();
        }
    }
}