using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Domain.Interfaces;
using TMod.Blog.Infrastructure.Contextes;

namespace TMod.Blog.Infrastructure.Repositories
{
    internal class UnitOfWork(TMod_Blog_RW_Context _context) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is null )
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is null )
            {
                throw new InvalidOperationException("没有开启事务，无法提交");
            }
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is null )
            {
                throw new InvalidOperationException("没有开启事务，无法回退");
            }
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
