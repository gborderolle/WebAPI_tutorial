using API_testing3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API_testing3.Context
{
    public class ContextDB :DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options)
        {
        }

        public DbSet<Author> Autor { get; set; }
        public DbSet<Book> Book { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .HasData(
                new Author()
                {
                    Id = 1,
                    Name = "Gonzalo"
                },
                new Author()
                {
                    Id = 2,
                    Name = "Miranda"
                }
                );
        }

        internal async Task<List<Author>> GetAuthors()
        {
            return await Autor.ToListAsync();
        }

        internal async Task<Author> GetAuthor(int id)
        {
            return await Autor.AsNoTracking().FirstAsync(x => x.Id == id);
        }

        internal async Task<Author> CreateAuthors(Author autor)
        {
            EntityEntry<Author> response = await Autor.AddAsync(autor);
            await SaveChangesAsync();
            return await GetAuthor(response.Entity.Id);
        }


    }
}
