using DotNetCoreReactREST.Entities;
using DotNetCoreReactREST.ResourceParameters;
using System.Threading.Tasks;

namespace DotNetCoreReactREST.Repositories
{
    public interface ICategoryRepository
    {
        Task AddCategory(Category category);

        Task<bool> CategoryExists(int categoryId);

        void DeleteCategory(Category category);

        Task<PaginationResourceParameter<Category>> GetAllCategories(PaginationResourceParameter<Category> paginationResourceParameter);

        Task<Category> GetCategoryById(int categoryId);

        Task<bool> Save();

        void UpdateCategory(Category category);
    }
}