using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _http;

        public AddressService(HttpClient http)
        {
            _http = http;
        }
        public async Task<Address> AddOrUpadteAddress(Address address)
        {
            var res = await _http.PostAsJsonAsync("api/address", address);
            return res.Content.ReadFromJsonAsync<ServiceResponse<Address>>().Result.Data;    
        }

        public async Task<Address> GetAddress()
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<Address>>("api/address");
            return res.Data;
        }
    }
}
