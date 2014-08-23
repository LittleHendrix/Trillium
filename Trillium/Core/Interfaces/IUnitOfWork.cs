namespace Trillium.Core.Interfaces
{
    using System;

    interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
