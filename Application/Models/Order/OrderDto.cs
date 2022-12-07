using Application.Models.Product;

namespace Application.Models.Order {

    public class OrderDto {
        public Guid Id { get; set; }
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }

        public string UserId { get; set; } = null!;
        public ICollection<ProductDto> Products { get; set; } = null!;
    }
}
