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
    internal abstract class UnitOfWork(TMod_Blog_RW_Context _context) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
        private bool _disposedValue;

        protected TMod_Blog_RW_Context context = _context;

        public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is null )
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is null )
            {
                throw new InvalidOperationException("没有开启事务，无法提交");
            }
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }



        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(_transaction is null )
            {
                throw new InvalidOperationException("没有开启事务，无法回退");
            }
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposedValue )
            {
                if ( disposing )
                {
                    // TODO: 释放托管状态(托管对象)
                    _transaction?.Dispose();
                    _context.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~UnitOfWork()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
