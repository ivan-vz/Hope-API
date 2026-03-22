using FluentValidation.Results;
using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;

namespace Hope.Application.Interfaces
{
    public interface IUserService
    {
        public Task<(UsertDto? dt, ValidationResult validation)> CreateAsync(UserInsertDto dtInsert, CancellationToken ct);
        public Task<IReadOnlyList<UsertDto>> GetAllAsync ();
        public Task<UsertDto?> GetByIdAsync(Guid id);
    }
}
