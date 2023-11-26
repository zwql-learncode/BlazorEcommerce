using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared.Models
{
    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductTypeName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
