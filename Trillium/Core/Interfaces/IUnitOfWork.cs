namespace Trillium.Core.Interfaces
{
    using System;

    interface IUnitOfWork : IDisposable
    {
        // list all repositories
        // e.g. 
        // IEditableRepository<BlogComments> BlogCommentsRepository { get; }
        // IRepository<ReadonlyClass> ReadonlyRepository { get; }

        void Commit();
        void Rollback();
        
    }
}
