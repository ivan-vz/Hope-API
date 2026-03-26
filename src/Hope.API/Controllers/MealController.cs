using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.DTOs.Update;
using Hope.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hope.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController(IMealService mealService) : ControllerBase
    {
        private readonly IMealService _mealService = mealService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MealDto>>> GetAll(CancellationToken ct) => Ok(await _mealService.GetAllAsync(ct));

        [HttpGet("{id}")]
        public async Task<ActionResult<MealDto>> GetById(Guid id, CancellationToken ct)
        {
            var meal = await _mealService.GetByIdAsync(id, ct);

            return (meal is null) ? NotFound() : Ok(meal);
        }

        [HttpGet("by-tags")]
        public async Task<ActionResult<IReadOnlyList<MealDto>>> GetByTags([FromBody] ICollection<string> tags, CancellationToken ct) => 
            Ok(await _mealService.GetByTagsAsync(tags, ct));

        [HttpPost]
        public async Task<ActionResult<MealDto>> Create(MealInsertDto dtInser, CancellationToken ct)
        {
            var (dt, validation) = await _mealService.CreateAsync(dtInser, ct);

            return (dt is null) ? BadRequest(validation.ToDictionary()) : CreatedAtAction(nameof(GetById), new { id = dt!.Id }, dt);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var validation = await _mealService.DeleteAsync(id, ct);

            return (validation.IsValid) ? NoContent() : BadRequest(validation.ToDictionary());
        }

        [HttpPut]
        public async Task<ActionResult<MealDto>> Update(MealUpdateDto dtUpdate, CancellationToken ct)
        {
            var (dt, validation) = await _mealService.UpdateAsync(dtUpdate, ct);

            return (dt is null) ? BadRequest(validation.ToDictionary()) : Ok(dt);
        }
    }
}
