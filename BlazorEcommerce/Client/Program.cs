global using BlazorEcommerce.Shared.Entities;
global using BlazorEcommerce.Shared.Models;
using BlazorEcommerce.Client;
using BlazorEcommerce.Client.Services.CategoryService;
using BlazorEcommerce.Client.Services.ProductService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService,  CategoryService>();

await builder.Build().RunAsync();
