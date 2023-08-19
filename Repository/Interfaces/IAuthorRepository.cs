using WebAPI_tutorial.Models;

namespace WebAPI_tutorial.Repository.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<Author> Update(Author entity);
    }
}
