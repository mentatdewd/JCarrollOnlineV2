﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Test.DataContexts
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    internal class FakeJCarrollOnlineV2Db<TEntity> : DbSet<TEntity>, IQueryable, IEnumerable<TEntity>, IDbAsyncEnumerable<TEntity>
        where TEntity : class
    {
        readonly ObservableCollection<TEntity> _data;
        readonly IQueryable _query;

        public FakeJCarrollOnlineV2Db()
        {
            _data = new ObservableCollection<TEntity>();
            _query = _data.AsQueryable();
        }

        public FakeJCarrollOnlineV2Db(ObservableCollection<TEntity> data, IQueryable query)
        {
            _data = data;
            _query = query;
        }

        public override TEntity Add(TEntity item)
        {
            _data.Add(item);
            return item;
        }

        public override TEntity Remove(TEntity item)
        {
            _data.Remove(item);
            return item;
        }

        public override TEntity Attach(TEntity item)
        {
            _data.Add(item);
            return item;
        }

        public override TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local => _data;

        Type IQueryable.ElementType => _query.ElementType;

        Expression IQueryable.Expression => _query.Expression;

        IQueryProvider IQueryable.Provider => new FakeJCarrollOnlineV2DbAsyncQueryProvider<TEntity>(_query.Provider);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IDbAsyncEnumerator<TEntity> IDbAsyncEnumerable<TEntity>.GetAsyncEnumerator()
        {
            return new FakeJCarrollOnlineV2DbAsyncEnumerator<TEntity>(_data.GetEnumerator());
        }

        public override DbQuery<TEntity> Include(string path)
        {
            return base.Include(path);
        }

        public override DbQuery<TEntity> AsNoTracking()
        {
            return base.AsNoTracking();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override TEntity Find(params object[] keyValues)
        {
            return base.Find(keyValues);
        }

        public override Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return base.FindAsync(cancellationToken, keyValues);
        }

        public override Task<TEntity> FindAsync(params object[] keyValues)
        {
            return base.FindAsync(keyValues);
        }

        public override IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            return base.AddRange(entities);
        }

        public override IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
        {
            return base.RemoveRange(entities);
        }

        public override DbSqlQuery<TEntity> SqlQuery(string sql, params object[] parameters)
        {
            return base.SqlQuery(sql, parameters);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }

    internal class FakeJCarrollOnlineV2DbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal FakeJCarrollOnlineV2DbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new FakeJCarrollOnlineV2DbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new FakeJCarrollOnlineV2DbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class FakeJCarrollOnlineV2DbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public FakeJCarrollOnlineV2DbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public FakeJCarrollOnlineV2DbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new FakeJCarrollOnlineV2DbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider => new FakeJCarrollOnlineV2DbAsyncQueryProvider<T>(this);
    }

    internal class FakeJCarrollOnlineV2DbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public FakeJCarrollOnlineV2DbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;

        object IDbAsyncEnumerator.Current => Current;
    }
}
