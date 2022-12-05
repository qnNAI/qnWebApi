using Application.Common.Interfaces.Services;
using Application.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase {
        private readonly IProductService _service;

        public ProductController(IProductService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id) {
            var product = await _service.GetByIdAsync(id);
            if (product is null) {
                return NotFound();
            }
            return Ok(product);
        }
             
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateProductRequest request) {
            var created = await _service.AddAsync(request);
            return CreatedAtAction("GetById", new { id = created.Id }, created);
        }

        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ProductDto product, [FromRoute] Guid id) {
            if (product.Id != id) {
                return BadRequest("Invalid id");
            }

            var updated = await _service.UpdateAsync(product);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
