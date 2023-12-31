﻿using WebAPI_tutorial.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using WebAPI_tutorial.Repository.Interfaces;

namespace WebAPI_tutorial.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ContextDB _dbContext;
        internal DbSet<T> dbSet;

        public Repository(ContextDB db)
        {
            _dbContext = db;
            this.dbSet = _dbContext.Set<T>();
        }

        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }

        public async Task<T> Get(Expression<Func<T, bool>>? filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet;

            // Agregar las propiedades a incluir al query.
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task<List<T>> GetAllIncluding(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

    }
}
