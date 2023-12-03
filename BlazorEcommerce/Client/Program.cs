global using BlazorEcommerce.Shared.Entities;
global using BlazorEcommerce.Shared.Models;
global using Blazored.LocalStorage;
global using Microsoft.AspNetCore.Components.Authorization;
using BlazorEcommerce.Client;
using BlazorEcommerce.Client.Services.AddressService;
using BlazorEcommerce.Client.Services.AuthService;
using BlazorEcommerce.Client.Services.CartService;
using BlazorEcommerce.Client.Services.CategoryService;
using BlazorEcommerce.Client.Services.OrderService;
using BlazorEcommerce.Client.Services.ProductService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService,  CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
