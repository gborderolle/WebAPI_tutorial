using WebAPI_tutorial.Models;

namespace WebAPI_tutorial.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<Book> Update(Book entity);
    }
}
