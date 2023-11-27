using Stripe.Checkout;

namespace BlazorEcommerce.Server.Services.PaymentService
{
    public interface IPaymentService
    {
        Session CreateCheckoutSession(List<CartItemDTO> cartItems);
    }
}
