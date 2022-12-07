using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Entities;
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
            var entity = await _context.Products.FindAsync(id);
            return entity.Adapt<ProductDto>();
        }

        public async Task<ProductDto> AddAsync(CreateProductRequest request) {
            var entity = request.Adapt<Product>();
            var created = await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();

            return (created.Entity).Adapt<ProductDto>();
        }

        public async Task<ProductDto> UpdateAsync(ProductDto product) {
            var entity = product.Adapt<Product>();
            var isExists = await _context.Products.AnyAsync(x => x.Id == entity.Id);
            if (!isExists) {
                throw new NotFoundException(nameof(Product), entity.Id);
            }

            var updated = _context.Products.Update(entity);
            await _context.SaveChangesAsync();

            return (updated.Entity).Adapt<ProductDto>();
        }

        public async Task DeleteAsync(Guid id) {
            var existing = await _context.Products.FindAsync(new object[] { id });

            if (existing is null) {
                throw new NotFoundException(nameof(Product), id);
            }

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
