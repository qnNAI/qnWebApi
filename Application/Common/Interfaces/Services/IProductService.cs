using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Product;

namespace Application.Common.Interfaces.Services {

    public interface IProductService {

        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<ProductDto> AddAsync(CreateProductRequest request);
    }
}
