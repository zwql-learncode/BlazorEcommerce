using BlazorEcommerce.Shared.Entities;
using Stripe;
using Stripe.Checkout;

namespace BlazorEcommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        public PaymentService()
        {
            StripeConfiguration.ApiKey = "sk_test_51OH0owBCD48lcuKVPhjuxGTCdyZiZ3FL1ub1jZudAU1rvfpt3ZqRkt6xa47aqVmw2212CkeSSXt67IcNCTWGlQZC00r9ZDKhH7";
        }
        public Session CreateCheckoutSession(List<CartItemDTO> cartItems)
        {
            var lineItems = new List<SessionLineItemOptions>();
            cartItems.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.ProductTitle,
                        Images = new List<string> { product.ImageUrl }
                    }
                },
                Quantity = product.Quantity
            }));
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7276/order-success",
                CancelUrl = "https://localhost:7276/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;

        }
    }
}
