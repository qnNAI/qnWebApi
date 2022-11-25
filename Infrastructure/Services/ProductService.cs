using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services;
using Application.Models.Product;
using Infrastructure.Persistence.Contexts;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services {

    public class ProductService : IProductService {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllAsync() {
            var _products = await _context.Products.AsNoTracking().ProjectToType<ProductDto>().ToListAsync();
            return _products;
        }

        public async Task<ProductDto> GetByIdAsync(Guid id) {
            var product = await _context.Products.FindAsync(id);
            return product.Adapt<ProductDto>();
        }

        public Task<ProductDto> AddAsync(CreateProductRequest request) {
            return Task.FromResult(new ProductDto {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                Description = request.Description
            });
        }
    }
}
