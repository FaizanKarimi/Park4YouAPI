using System;
using System.Data;

namespace Repository.Provider
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; set; }
        IDbTransaction Transaction { get; set; }

        /// <summary>
        /// Open up the database connection
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the database connection
        /// </summary>
        void Close();

        /// <summary>
        /// Begin the transaction
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// Roll back the transaction
        /// </summary>
        void RollBackTransaction();

        /// <summary>
        /// Commit the transaction into database
        /// </summary>
        /// <returns></returns>
        void CommitTransaction();
    }
}