using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.DTOs.Update;
using Hope.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hope.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController(IMenuService menuService) : ControllerBase
    {
        private readonly IMenuService _menuService = menuService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MenuDto>>> GetAll(CancellationToken ct) => Ok(await _menuService.GetAllAsync(ct));

        [HttpGet("by-date")]
        public async Task<ActionResult<IReadOnlyList<MenuDto>>> GetAllByDate([FromQuery] DateOnly date, CancellationToken ct) => 
            Ok(await _menuService.GetAllByDateAsync(date, ct));

        [HttpGet("by-tags")]
        public async Task<ActionResult<IReadOnlyList<MenuDto>>> GetAllByTags([FromBody] ICollection<string> tags, CancellationToken ct) => 
            Ok(await _menuService.GetAllByTagsAsync(tags, ct));

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetById(Guid id, CancellationToken ct)
        {
            var menu = Ok(await _menuService.GetByIdAsync(id, ct));

            return (menu is null) ? NotFound() : Ok(menu);
        }

        [HttpPost]
        public async Task<ActionResult<MenuDto>> Create(MenuInsertDto dtInsert, CancellationToken ct)
        {
            var (dt, validation) = await _menuService.CreateAsync(dtInsert, ct);

            return (dt is null) ? BadRequest(validation.ToDictionary()) : CreatedAtAction(nameof(GetById), new { id =  dt!.Id }, dt);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct) => (await _menuService.DeleteAsync(id, ct)).IsValid ? NoContent() : NotFound();

        [HttpPut]
        public async Task<ActionResult<MenuDto>> Update(MenuUpdateDto updateDto, CancellationToken ct)
        {
            var (dt, validation) = await _menuService.UpdateAsync(updateDto, ct);

            return (dt is null) ? BadRequest(validation.ToDictionary()) : Ok(dt);
        }
    }
}
