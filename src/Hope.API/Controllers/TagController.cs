using Hope.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hope.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController(ITagService tagService) : ControllerBase
    {
        private readonly ITagService _tagService = tagService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<string>>> GetAll(CancellationToken ct) => Ok(await _tagService.GetAllAsync(ct));

        [HttpGet("active")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetAllActive(CancellationToken ct) => Ok(await _tagService.GetAllActiveAsync(ct));

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] string name, CancellationToken ct)
        {
            var validation = await _tagService.CreateAsync(name, ct);

            return (validation.IsValid) ? NoContent() : BadRequest(validation.ToDictionary());
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] string name, CancellationToken ct)
        {
            var result = await _tagService.DeleteAsync(name, ct);
            return (result.IsValid) ? NoContent() : BadRequest(result.ToDictionary());
        }
    }
}
