using Application.Models.Order;

namespace Application.Common.Interfaces.Services {

    public interface IOrderService {

        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto> GetByIdAsync(Guid id);
        Task<OrderDto> AddAsync(CreateOrderRequest request);
        Task<OrderDto> UpdateAsync(UpdateOrderRequest request);
        Task DeleteAsync(Guid id);
    }
}
