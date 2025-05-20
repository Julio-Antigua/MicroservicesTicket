using MongoDB.Driver;

namespace Ticketing.Command.Domain.Abstracts
{
    public interface ISession
    {
        Task<IClientSessionHandle> BeginSessionAsync(CancellationToken cancellationToken);
        void BeginTransaction(IClientSessionHandle session);
        Task CommitTransactionAsync(IClientSessionHandle session, CancellationToken cancellationToken);
        Task RollebackTransactionAsync(IClientSessionHandle session, CancellationToken cancellationToken);
        void DisposeSession(IClientSessionHandle session);
    }
}
