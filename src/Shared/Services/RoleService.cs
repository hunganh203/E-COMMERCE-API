using Application.Constants;
using Application.Dtos.Role;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task DeleteById(int key)
        {
            var entity = await _roleRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);
            await _roleRepository.DeleteAsync(entity);
        }

        public async Task<PagedResultDto<RoleDto>> GetAll(GetRoleInput input)
        {
            var query = _roleRepository
                .AsQueryable()
                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Name.Contains(input.KeySearch.Trim())));

            var sources = await query.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (sources is not { Count: > 0 })
            {
                return new PagedResultDto<RoleDto>
                {
                    Items = new List<RoleDto>(),
                    TotalCount = 0
                };
            }

            var productsResult = _mapper.Map<List<RoleDto>>(sources);

            return new PagedResultDto<RoleDto>
            {
                Items = productsResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch)
                    ? _roleRepository.Count()
                    : await query.CountAsync()
            };
        }

        public async Task<RoleDto> GetById(int key)
        {
            var entity = await _roleRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);
            return _mapper.Map<RoleDto>(entity);
        }

        public async Task<RoleDto> Insert(RoleDto entity)
        {
            var result = await _roleRepository.AddAsync(new Role
            {
                Name = entity.Name,
                NormalizedName = entity.NormalizedName,
            });
            return _mapper.Map<RoleDto>(result);
        }

        public async Task Update(int key, RoleDto entity)
        {
            var source = await _roleRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.RoleDoesNotExist);

            source.Name = entity.Name;
            source.NormalizedName = entity.NormalizedName;

            await _roleRepository.UpdateAsync(source);
        }
    }
}