using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities {

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
