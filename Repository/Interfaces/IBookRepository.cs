using API_testing3.Models;

namespace API_testing3.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<Book> Update(Book entity);
    }
}
