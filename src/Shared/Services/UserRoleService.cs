using Application.Constants;
using Application.Dtos.UserRole;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;

        public UserRoleService(IUserRoleRepository userRoleRepository, IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task DeleteById(int userId, int roleId)
        {
            var entity = await _userRoleRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);
            await _userRoleRepository.DeleteAsync(entity);
        }

        public async Task<PagedResultDto<UserRoleDto>> GetAll(GetUserRoleInput input)
        {
            var query = _userRoleRepository
                .AsQueryable();

            var sources = await query.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Include(x => x.User)
                .Include(x => x.Role)
                .ToListAsync();

            if (sources is not { Count: > 0 })
            {
                return new PagedResultDto<UserRoleDto>
                {
                    Items = new List<UserRoleDto>(),
                    TotalCount = 0
                };
            }

            var productsResult = _mapper.Map<List<UserRoleDto>>(sources);

            return new PagedResultDto<UserRoleDto>
            {
                Items = productsResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch)
                    ? _userRoleRepository.Count()
                    : await query.CountAsync()
            };
        }

        public async Task<UserRoleDto> GetById(int userId, int roleId)
        {
            var entity = await _userRoleRepository.FirstOrDefaultAsync(x => x.RoleId == roleId && x.UserId == userId);
            if (entity == null)
                throw new Exception(CustomResponseMessage.UserRoleDoesNotExist);
            return _mapper.Map<UserRoleDto>(entity);
        }

        public async Task<UserRoleDto> Insert(UserRoleDto entity)
        {
            var result = await _userRoleRepository.AddAsync(new UserRole
            {
                UserId = entity.UserId,
                RoleId = entity.RoleId,
            });
            return _mapper.Map<UserRoleDto>(result);
        }

        public async Task Update(int roleId, int userId, UserRoleDto entity)
        {
            var source = await _userRoleRepository.FirstOrDefaultAsync(x => x.RoleId == roleId && x.UserId == userId);
            if (entity == null)
                throw new Exception(CustomResponseMessage.UserRoleDoesNotExist);

            source.UserId = entity.UserId;
            source.RoleId = entity.RoleId;

            await _userRoleRepository.UpdateAsync(source);
        }
    }
}