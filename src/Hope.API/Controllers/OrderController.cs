using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.DTOs.Update;
using Hope.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hope.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAllByUser(Guid userId, CancellationToken ct) => 
            Ok(await _orderService.GetAllByUserAsync(userId, ct));

        [HttpGet("by-date")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAllByDate([FromBody] DateOnly date, CancellationToken ct) => 
            Ok(await _orderService.GetAllByDateAsync(date, ct));

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
        {
            var order = await _orderService.GetByIdAsync(id, ct);
            return (order is null) ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(OrderInsertDto insertDto, CancellationToken ct)
        {
            var (dt, validation) = await _orderService.CreateAsync(insertDto, ct);
            return (dt is null) ? BadRequest(validation.ToDictionary()) : CreatedAtAction(nameof(GetById), new { id = dt!.Id }, dt);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct) => (await _orderService.DeleteAsync(id, ct)).IsValid ? NoContent() : NotFound();

        [HttpPut]
        public async Task<ActionResult<OrderDto>> Update(OrderUpdateDto updateDto, CancellationToken ct)
        {
            var (dt, validation) = await _orderService.UpdateAsync(updateDto, ct);
            return (dt is null) ? BadRequest(validation.ToDictionary()) : Ok(dt);
        }
    }
}
