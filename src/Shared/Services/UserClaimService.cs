using Application.Constants;
using Application.Dtos.UserClaim;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.Services
{
    public class UserClaimService : IUserClaimService
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;

        public UserClaimService(IUserClaimRepository userClaimRepository, IMapper mapper)
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
        }

        public async Task DeleteById(int key)
        {
            var entity = await _userClaimRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);
            await _userClaimRepository.DeleteAsync(entity);
        }

        public async Task<PagedResultDto<UserClaimDto>> GetAll(GetUserClaimInput input)
        {
            var query = _userClaimRepository
                .AsQueryable()
                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.ClaimValue.Contains(input.KeySearch.Trim()) || x.ClaimType.Contains(input.KeySearch.Trim())));

            var sources = await query.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (sources is not { Count: > 0 })
            {
                return new PagedResultDto<UserClaimDto>
                {
                    Items = new List<UserClaimDto>(),
                    TotalCount = 0
                };
            }

            var productsResult = _mapper.Map<List<UserClaimDto>>(sources);

            return new PagedResultDto<UserClaimDto>
            {
                Items = productsResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch)
                    ? _userClaimRepository.Count()
                    : await query.CountAsync()
            };
        }

        public async Task<UserClaimDto> GetById(int key)
        {
            var entity = await _userClaimRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);
            return _mapper.Map<UserClaimDto>(entity);
        }

        public async Task<UserClaimDto> Insert(UserClaimDto entity)
        {
            var result = await _userClaimRepository.AddAsync(new UserClaim
            {
                ClaimValue = entity.ClaimValue,
                ClaimType = entity.ClaimType
            });
            return _mapper.Map<UserClaimDto>(result);
        }

        public async Task Update(int key, UserClaimDto entity)
        {
            var source = await _userClaimRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);

            source.ClaimValue = entity.ClaimValue;
            source.ClaimType = entity.ClaimType;

            await _userClaimRepository.UpdateAsync(source);
        }
    }
}