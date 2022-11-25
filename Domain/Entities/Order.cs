using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Application.Entities {

    public class Order {

        public Guid Id { get; set; }
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }

        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = null!;
    }
}
