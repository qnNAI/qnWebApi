using Application.Common.Helpers;
using Application.Common.Interfaces.Services;
using Application.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase {
        private readonly IOrderService _service;

        public OrderController(IOrderService service) {
            this._service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id) {
            var order = await _service.GetByIdAsync(id);
            if (order is null) {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateOrderRequest request) {
            request.UserId = HttpContext.GetUserId();
            var created = await _service.AddAsync(request);
            return CreatedAtAction("GetById", new { id = created.Id }, created);
        }

        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateOrderRequest request, [FromRoute] Guid id) {
            if (request.Id != id) {
                return BadRequest("Invalid id");
            }

            var userId = HttpContext.GetUserId();
            if (userId != request.UserId) {
                return Forbid("User is not the owner of the order");
            }

            var updated = await _service.UpdateAsync(request);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid id) {
            var order = await _service.GetByIdAsync(id);
            if (order is null) {
                return NotFound();
            }

            var userId = HttpContext.GetUserId();
            if(userId != order.UserId) {
                return Forbid("User is not the owner of the order");
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }

    }
}
