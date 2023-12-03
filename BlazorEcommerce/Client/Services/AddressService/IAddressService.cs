namespace BlazorEcommerce.Client.Services.AddressService
{
    public interface IAddressService
    {
        Task<Address> GetAddress();
        Task<Address> AddOrUpadteAddress(Address address);
    }
}
