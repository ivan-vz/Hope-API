using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hope.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UsertDto>>> GetAll(CancellationToken ct) => Ok(await _userService.GetAllAsync(ct));

        [HttpGet("{id}")]
        public async Task<ActionResult<UsertDto?>> GetById(Guid id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            return (user is null) ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UsertDto>> Create(UserInsertDto dtInsert, CancellationToken ct)
        {
            var (dt, validation) = await _userService.CreateAsync(dtInsert, ct);

            return (dt is null) ? BadRequest(validation.ToDictionary()) : CreatedAtAction(nameof(GetById), new { id = dt!.Id }, dt);
        }
    }
}
