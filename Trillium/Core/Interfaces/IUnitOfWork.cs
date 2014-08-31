namespace Trillium.Core.Interfaces
{
    using System;
    using Trillium.Models;

    public interface IUnitOfWork : IDisposable
    {
        // list all repositories
        // e.g. 
        // IRepository<ReadonlyClass> ReadonlyRepository { get; }
        IRepository<BlogComment> BlogCommentsRepository { get; } 

        void Commit();
    }
}