namespace SolutionName.Application.Contracts.Persistence.UoW
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the transaction.</returns>
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}

