namespace BlazorEcommerce.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
        {
            category.Editing = category.IsNew = false;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return await GetAdminCategories();
        }

        public async Task<ServiceResponse<List<Category>>> GetAdminCategories()
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            return new ServiceResponse<List<Category>>
            {
                Data = categories,
                Message = "Thành công"
            };
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted && c.Visible)
                .ToListAsync();
            return new ServiceResponse<List<Category>>
            {
                Data = categories,
                Message = "Thành công"
            };
        }

        public async Task<ServiceResponse<List<Category>>> SoftDeleteCategory(int categoryId)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            category.IsDeleted = true;
            await _context.SaveChangesAsync();

            return await GetAdminCategories();
        }

        public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
        {
            var dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if(dbCategory == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            dbCategory.Name = category.Name;
            dbCategory.Url = category.Url;
            dbCategory.Visible = category.Visible;

            await _context.SaveChangesAsync();

            return await GetAdminCategories();

        }
    }
}
