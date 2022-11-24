using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities {

    public class Order {

        public Guid Id { get; set; }
        public decimal Total { get; set; }

        public ICollection<Product> Products { get; set; } = null!;
    }
}
