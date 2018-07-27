using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JagiCore.Interfaces
{
    public interface IRepository<T> where T: IEntity
    {
        Result<T> Find(object id);
        Result<IEnumerable<T>> GetAll();

        Result<IEnumerable<T>> GetByWhere(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> include, 
            Expression<Func<T, object>> orderBy, 
            bool orderByDesc,
            bool isTracking);

        Result<IEnumerable<T>> GetByPaged(ref PageInfo pageInfo, out int totalCount,
                    Expression<Func<T, bool>> filter = null,
                    Expression<Func<T, object>> include = null,
                    Expression<Func<T, object>> orderBy = null,
                    bool orderByDesc = false,
                    bool isTracking = false);

        Result<T> GetFirst(Expression<Func<T, bool>> filter);
        void Add(T item);
        void Update(T item);
        void Remove(T item);

        /// <summary>
        /// 處理存檔工作，呼叫 DbContext.SaveChanges()
        /// 因此會將所有 DbContext 的項目一併存檔（不僅只有這個 Repository）
        /// TODO: 因應多個 repository 一般而言應該是在在 UnitOfWork 中處理
        /// 考量是否轉移到 DbContext 中進行作業
        /// </summary>
        /// <returns>變更的數量</returns>
        Result<int> Save();
    }
}
