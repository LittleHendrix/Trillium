namespace Trillium.DAL.Infrastructure
{
    using Trillium.DAL.Interfaces;

    public class PetaPocoUnitOfWorkProvider : IUnitOfWorkProvider
    {
        public IUnitOfWork GetUnitOfWork()
        {
            return new PetaPocoUnitOfWork();
        }
    }
}