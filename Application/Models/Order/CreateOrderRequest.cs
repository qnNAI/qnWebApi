using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Product;

namespace Application.Models.Order {

    public class CreateOrderRequest {
        public string? UserId { get; set; }
        public Guid[] ProductsId { get; set; } = null!;
    }
}
