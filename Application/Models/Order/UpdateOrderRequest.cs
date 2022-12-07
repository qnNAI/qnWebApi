using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Order {

    public class UpdateOrderRequest {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public Guid[] ProductsId { get; set; } = null!;
    }
}
