using API_testing3.Context;
using API_testing3.Models;
using API_testing3.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_testing3.Repository
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        /// <summary>
        /// Igual a la capa Services, pero éste hereda de interfaces (mejor)
        /// 
        /// En program.cs:
        /// builder.Services.AddScoped<IVillaRepository, VillaRepository>();
        /// </summary>
        private readonly DbContext _dbContext;

        public BookRepository(ContextDB dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Book> Update(Book entity)
        {
            entity.Update = DateTime.Now;
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
