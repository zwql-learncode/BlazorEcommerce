using BlazorEcommerce.Server.Services.AuthService;

namespace BlazorEcommerce.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        public AddressService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var res = new ServiceResponse<Address>();
            var dbAddress = (await GetAddress()).Data;
            if(dbAddress == null)
            {
                address.UserId = _authService.GetUserId();
                _context.Addresses.Add(address);
                res.Data = dbAddress;
            }
            else
            {
                dbAddress.FirstName = address.FirstName; 
                dbAddress.LastName = address.LastName;
                dbAddress.Country = address.Country;
                dbAddress.City = address.City;
                dbAddress.State = address.State;
                dbAddress.Zip = address.Zip;
                dbAddress.Street = address.Street;
                res.Data = dbAddress;
            }

            await _context.SaveChangesAsync();
            return res;
        }

        public async Task<ServiceResponse<Address>> GetAddress()
        {
            int userId = _authService.GetUserId();
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
            return new ServiceResponse<Address>
            {
                Data = address,
            };
        }
    }
}
