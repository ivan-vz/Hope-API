using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.DTOs.Login;
using Hope.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hope.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UsertDto>>> GetAll(CancellationToken ct) => Ok(await _userService.GetAllAsync(ct));

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsertDto?>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return (user is null) ? NotFound() : Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<LoggedDto>> Create(UserInsertDto dtInsert, CancellationToken ct)
        {
            var (dt, validation) = await _userService.CreateAsync(dtInsert, ct);

            return (dt is null) ? BadRequest(validation.ToDictionary()) : CreatedAtAction(nameof(GetById), new { id = dt!.Id }, dt);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoggedDto>> Login(LoginRequest login)
        {
            var (dt, validation) = await _userService.Login(login);
            return (dt is null) ? Unauthorized(validation.ToDictionary()) : Ok(dt);
        }
    }
}
