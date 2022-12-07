using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using Application.Models.Order;
using Infrastructure.Persistence.Contexts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure.Services {

    public class OrderService : IOrderService {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<List<OrderDto>> GetAllAsync() {
            var _orders = await _context.Orders.AsNoTracking()
                .Include(x => x.Products)
                .ProjectToType<OrderDto>()
                .ToListAsync();
            return _orders;
        }

        public async Task<OrderDto> GetByIdAsync(Guid id) {
            var entity = await _context.Orders.FindAsync(id);
            return entity.Adapt<OrderDto>();
        }

        public async Task<OrderDto> AddAsync(CreateOrderRequest request) {
            var entity = request.Adapt<Order>();

            var products = await GetProductsAsync(request.ProductsId);

            entity.Total = products.Sum(x => x.Price);
            entity.DateTime = DateTime.Now;

            var created = (await _context.AddAsync(entity)).Entity;
            created.Products = products;
            await _context.SaveChangesAsync();

            return created.Adapt<OrderDto>();
        }

        

        public async Task<OrderDto> UpdateAsync(UpdateOrderRequest request) {
            var entity = request.Adapt<Order>();
            var isExists = await _context.Orders.AnyAsync(x => x.Id == entity.Id);
            if(!isExists) {
                throw new NotFoundException(nameof(Order), entity.Id);
            }

            var products = await GetProductsAsync(request.ProductsId);

            entity.Total = products.Sum(x => x.Price);
            var updated = _context.Orders.Update(entity).Entity;

            var orderProducts = await _context.OrderProduct.Where(x => x.OrderId == entity.Id).ToArrayAsync();
            _context.OrderProduct.RemoveRange(orderProducts.Where(x => !request.ProductsId.Any(id => id == x.ProductId)));

            var newOrderProducts = products.Where(p => !orderProducts.Any(x => x.ProductId == p.Id))
                .Select(p => new OrderProduct { OrderId = entity.Id, ProductId = p.Id });
            await _context.OrderProduct.AddRangeAsync(newOrderProducts);

            await _context.SaveChangesAsync();
            updated.Products = products;
            return updated.Adapt<OrderDto>();
        }

        public async Task DeleteAsync(Guid id) {
            var existing = await _context.Orders.FindAsync(new object[] { id });

            if(existing is null) {
                throw new NotFoundException(nameof(Order), id);
            }

            _context.Orders.Remove(existing);
            await _context.SaveChangesAsync();
        }

        private async Task<List<Product>> GetProductsAsync(Guid[] productsId) {
            var products = new List<Product>();
            foreach(var productId in productsId) {
                var product = await _context.Products.FindAsync(new object[] { productId });
                if(product is not null)
                    products.Add(product);
            }

            if(products.Count == 0) {
                throw new OrderProductsEmptyException();
            }

            return products;
        }
    }
}
