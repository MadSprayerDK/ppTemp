using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PpApi.Database
{
    public class BaseRepository<TDatatype, TContext>
        where TDatatype : class
        where TContext : DbContext
    {
        protected TContext _dbContext;
        private DbSet<TDatatype> _dbSet;

        public BaseRepository(Func<TContext, DbSet<TDatatype>> dbToUse, TContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbToUse(_dbContext);
        }

        protected virtual TDatatype CreateDbObj(TDatatype obj)
        {
            var returnObj = _dbSet.Add(obj);
            _dbContext.SaveChanges();

            return returnObj;
        }

        protected virtual ICollection<TDatatype> GetDbObjs(Func<TDatatype, bool> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        protected IQueryable<TDatatype> GetDbObjQueryable()
        {
            return _dbSet;
        }

        protected virtual void UpdateDbObjects()
        {
            _dbContext.SaveChanges();
        }

        protected virtual void DeleteDbObj(TDatatype obj)
        {
            _dbSet.Remove(obj);
            _dbContext.SaveChanges();
        }

        protected virtual void RemoveAllEntitiesFromDatabase()
        {
            _dbSet.RemoveRange(_dbSet);
            _dbContext.SaveChanges();
        }
    }
}