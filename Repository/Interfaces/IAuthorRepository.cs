using API_testing3.Models;

namespace API_testing3.Repository.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<Author> Update(Author entity);
    }
}
