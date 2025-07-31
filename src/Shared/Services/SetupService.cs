using Application.Constants;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility;
using Domain.Entities;

namespace Shared.Services
{
    public class SetupService : ISetupService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public SetupService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<bool> SeedDataAdmin()
        {
            User admin;
            var role = new Role();

            var adminExist = await _userRepository.AnyAsync(x => x.UserName == AppConsts.AdminUserName);
            if (!adminExist)
            {
                admin = await _userRepository.AddAsync(new User
                {
                    UserName = "admin",
                    NormalizedEmail = AppConsts.AdminUserName,
                    Email = "admin@gmail.com",
                    FirstName = "Quản",
                    LastName = "Trị",
                    Avatar = "",
                    PhoneNumber = "",
                    NormalizedUserName = AppConsts.AdminUserName,
                    PasswordHash = Utils.ComputeHash(AppConsts.AdminPassword)
                });
            }
            else
            {
                admin = await _userRepository.FirstOrDefaultAsync(x => x.UserName == AppConsts.AdminUserName);
            }

            var roles = new List<Role>(new[]
            {
                new Role {  Name = UserRoleKey.RoleConst.AdminName, NormalizedName = UserRoleKey.RoleConst.Admin},
                new Role
                {
                     Name = UserRoleKey.RoleConst.InventoryManagementName,
                    NormalizedName = UserRoleKey.RoleConst.InventoryManagement
                },
                new Role {  Name = UserRoleKey.RoleConst.SaleName, NormalizedName = UserRoleKey.RoleConst.Sale},
                new Role {  Name = UserRoleKey.RoleConst.ShipperName, NormalizedName = UserRoleKey.RoleConst.Shipper}
            });

            foreach (var r in roles)
            {
                if (r.NormalizedName == UserRoleKey.RoleConst.Admin)
                {
                    var roleExist = await _roleRepository.AnyAsync(x => x.NormalizedName == r.NormalizedName);
                    if (!roleExist)
                    {
                        role = await _roleRepository.AddAsync(r);
                    }
                    else
                    {
                        role = await _roleRepository.FirstOrDefaultAsync(x => x.NormalizedName == r.NormalizedName);
                    }
                }
                else
                {
                    var roleExist = await _roleRepository.AnyAsync(x => x.NormalizedName == r.NormalizedName);
                    if (!roleExist)
                    {
                        await _roleRepository.AddAsync(r);
                    }
                }
            }

            // seed user role

            var userRoleExist = await _userRoleRepository.AnyAsync(x => x.UserId == admin.Id && x.RoleId == role.Id);

            if (!userRoleExist)
            {
                await _userRoleRepository.AddAsync(new UserRole
                {
                    UserId = admin.Id,
                    RoleId = role.Id,
                });
            }

            return true;
        }
    }
}