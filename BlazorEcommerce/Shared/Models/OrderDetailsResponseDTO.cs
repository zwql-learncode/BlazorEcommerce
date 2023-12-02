using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared.Models
{
    public class OrderDetailsResponseDTO
    {
        public DateTime OrderDate { get; set; } 
        public decimal TotalPrice { get; set; }
        public List<OrderDetailsProductResponseDTO> Products { get; set; }
    }
}
