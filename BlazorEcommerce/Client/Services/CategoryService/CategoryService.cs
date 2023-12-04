using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _http;

        public CategoryService(HttpClient http)
        {
            _http = http;
        }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Category> AdminCategories { get; set; }

        public event Action OnChange;

        public async Task AddCategory(Category category)
        {
            var result = await _http.PostAsJsonAsync("api/category/admin", category);
            AdminCategories = (await result.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public Category CreateNewCategory()
        {
            var newCategory = new Category 
            {
                IsNew = true,
                Editing = true,
            };
            AdminCategories.Add(newCategory);   
            OnChange.Invoke();
            return newCategory;
        }

        public async Task DeleteCategory(int categoryId)
        {
            var result = await _http.DeleteAsync($"api/category/admin/{categoryId}");
            AdminCategories = (await result.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public async Task GetAdminCategories()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category/admin");
            if (result != null && result.Data != null) 
            {
                AdminCategories = result.Data;
            }
        }

        public async Task GetCategories()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");
            if(result != null && result.Data != null)
            {
                Categories = result.Data;
            }
        }

        public async Task UpdateCategory(Category category)
        {
            var result = await _http.PutAsJsonAsync("api/category/admin", category);
            AdminCategories = (await result.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }
    }
}
