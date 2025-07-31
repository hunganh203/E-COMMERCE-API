using Application.Constants;
using Application.Dtos;
using Application.Dtos.Menu;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public MenuService(IMenuRepository menuRepository, IMapper mapper, IProductRepository productRepository)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Xóa danh mục menu
        /// </summary>
        /// <param name="menuId"></param>
        public async Task DeleteById(int menuId)
        {
            var menu = await _menuRepository.FirstOrDefaultAsync(x => x.Id == menuId);
            if (menu == null)
            {
                throw new Exception(CustomResponseMessage.MenuDoesNotExist);
            }

            var isExistPMenu = await _menuRepository.AnyAsync(x => x.PMenuId == menuId);
            if (isExistPMenu)
            {
                throw new Exception(CustomResponseMessage.CategoryRemoveParentFirst);
            }

            var isExist = await _productRepository.AnyAsync(x => x.MenuId == menuId);
            if (isExist)
            {
                throw new Exception(CustomResponseMessage.CategoryHasBeenUsed);
            }

            await this._menuRepository.DeleteAsync(menu);
        }

        /// <summary>
        /// Get tất cả menu
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<MenuDto>> GetAll(GetMenuInput input)
        {
            var menus = await _menuRepository
               .AsQueryable()
               .OrderByDescending(x => x.Id)
               .Where(x => !input.PMenuId.HasValue || x.PMenuId == input.PMenuId)
               .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Name.Contains(input.KeySearch.Trim())))
               .Skip(input.SkipCount)
               .Take(input.MaxResultCount)
               .Include(x => x.PMenu)

               .ToListAsync();

            if (menus is not { Count: > 0 })
            {
                var menuDtos = _mapper.Map<List<MenuDto>>(menus);
                return new PagedResultDto<MenuDto>
                {
                    Items = menuDtos,
                    TotalCount = menuDtos.Count
                };
            }

            var items = _mapper.Map<List<MenuDto>>(menus);

            return new PagedResultDto<MenuDto>
            {
                Items = items,
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && !input.PMenuId.HasValue && !input.PMenuId.HasValue
                    ? _menuRepository.Count()
                    : _menuRepository
                        .Count(x => (!input.PMenuId.HasValue || x.PMenuId == input.PMenuId)
                            && (string.IsNullOrEmpty(input.KeySearch) || x.Name.Contains(input.KeySearch.Trim())))
            };
        }

        /// <summary>
        /// Get danh sách menu chính theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public async Task<List<MenuDto>> GetMainMenu(string? keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            var menus = await _menuRepository.AsQueryable()
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Where(x => x.Group == "main")
                .OrderBy(x => x.Index)
                .Include(x => x.PMenu)
                .ToListAsync();

            return _mapper.Map<List<MenuDto>>(menus);
        }

        public async Task<List<MenuDto>> GetSubMenu(string? keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            var menus = await _menuRepository.AsQueryable()
                .OrderBy(x => x.Index)
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Where(x => x.Group == "sub")
                .ToListAsync();

            return _mapper.Map<List<MenuDto>>(menus);
        }

        public async Task<List<MenuDto>> GetMainMenuActive()
        {
            var menus = (await _menuRepository.GetAllAsync())
                .Where(x => x.Active)
                .Where(x => x.Group == "main")
                .OrderBy(x => x.Index);

            var menuDtos = _mapper.Map<List<MenuDto>>(menus);

            var parentMenu = menuDtos.FindAll(x => x.PMenu == null);
            parentMenu.ForEach(x =>
            {
                x.SubMenus = menuDtos.FindAll(y => y.PMenuId == x.Id);
            });
            return parentMenu;
        }

        public async Task<List<MenuDto>> GetSubMenuActive()
        {
            var menus = (await _menuRepository.GetAllAsync())
                .Where(x => x.Active)
                .Where(x => x.Group == "sub")
                .OrderBy(x => x.Index);

            var menuDtos = _mapper.Map<List<MenuDto>>(menus);

            var parentMenu = menuDtos.FindAll(x => x.PMenu == null);
            parentMenu.ForEach(x =>
            {
                x.SubMenus = menuDtos.FindAll(y => y.PMenuId == x.Id);
            });
            return parentMenu;
        }

        public async Task<List<MenuDto>> GetParentMainMenu()
        {
            var menus = await _menuRepository.AsQueryable()
                    .Where(x => x.PMenu == null)
                    .Where(x => x.Group == "main")
                    .OrderBy(x => x.Index)
                    .ToListAsync();

            return _mapper.Map<List<MenuDto>>(menus);
        }

        public async Task<List<MenuDto>> GetParentSubMenu()
        {
            var menus = (await _menuRepository.GetAllAsync())
                .Where(x => x.PMenuId == null)
                .Where(x => x.Group == "sub")
                .OrderBy(x => x.Index);

            return _mapper.Map<List<MenuDto>>(menus);
        }

        /// <summary>
        /// Get danh sách menu theo loại
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public async Task<List<MenuDto>> GetByType(List<string> types)
        {
            var menus = (await _menuRepository.GetAllAsync())
                .Where(x => types.Any(y => y == x.Type))
                .OrderBy(x => x.Index);

            return _mapper.Map<List<MenuDto>>(menus);
        }

        public async Task<List<MenuDto>> GetByTypeForAdmin(List<string> types)
        {
            var allMenus = await _menuRepository.AsQueryable()
                .Where(x => types.Any(y => y == x.Type))
                .OrderBy(x => x.Index)
                .ToListAsync();

            var menuDtos = new List<MenuDto>();

            foreach (var parentMenu in allMenus.Where(x => !x.PMenuId.HasValue))
            {
                var menuDto = _mapper.Map<MenuDto>(parentMenu);
                menuDto.SubMenus = _mapper.Map<List<MenuDto>>(allMenus.Where(x => x.PMenuId == parentMenu.Id).ToList());
                menuDtos.Add(menuDto);
            }

            return menuDtos;
        }

        /// <summary>
        /// Get tất cả menu hiển thị trang chủ
        /// </summary>
        /// <returns></returns>
        public async Task<List<MenuDto>> GetAllShowHomePage()
        {
            var menuDtos = (await _menuRepository.GetAllAsync())
                .Where(x => x.Active && x.ShowHomePage == true)
                .OrderBy(x => x.Index)
                .Select(x => new MenuDto()
                {
                    Alias = x.Alias,
                    Name = x.Name,
                    //Products = x.Products
                    //    .Where(y => y.Status == 10)
                    //    .OrderBy(y => y.Index)
                    //    .Select(y => new ProductDto()
                    //    {
                    //        Id = y.Id,
                    //        Alias = y.Alias,
                    //        Name = y.Name,
                    //        Price = y.Price,
                    //        DiscountPrice = y.DiscountPrice,
                    //        Image = y.Image
                    //    })
                    //    .Take(10)
                    //    .ToList()
                }).ToList();

            return menuDtos;
        }

        public async Task<MenuDto> GetById(int key)
        {
            var menu = await _menuRepository
                .AsQueryable()
                .Include(x => x.PMenu)
                .FirstOrDefaultAsync(x => x.Id == key);
            return _mapper.Map<MenuDto>(menu);
        }

        /// <summary>
        /// Get menu theo alias
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task<MenuDto> GetByAlias(string alias)
        {
            return (await _menuRepository.GetAllAsync())
                .Where(x => x.Alias == alias)
                .Select(x => new MenuDto()
                {
                    Alias = x.Alias,
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefault() ?? new MenuDto();
        }

        public async Task<MenuDto> Insert(MenuDto entity)
        {
            var menu = new Menu()
            {
                Active = entity.Active,
                Alias = "",
                Index = entity.Index,
                Group = entity.Group,
                Name = entity.Name,
                PMenuId = entity.PMenuId,
                ShowHomePage = entity.ShowHomePage,
                Type = entity.Type,
            };

            await _menuRepository.AddAsync(menu);

            return entity;
        }

        public async Task Update(int key, MenuDto entity)
        {
            var menu = await _menuRepository.FirstOrDefaultAsync(x => x.Id == key);

            menu.PMenuId = entity.PMenuId;
            menu.Name = entity.Name;
            if (menu.Alias != entity.Alias)
                menu.Alias = entity.Alias + "-" + key;
            menu.Index = entity.Index;
            menu.ShowHomePage = entity.ShowHomePage;
            menu.Type = entity.Type;
            menu.Active = entity.Active;

            await _menuRepository.UpdateAsync(menu);
        }
    }
}