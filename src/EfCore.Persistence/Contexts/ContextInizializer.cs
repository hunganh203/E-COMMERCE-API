using Application.Constants;
using Application.Utility;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Persistence.Contexts
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var role = new Role
            { Id = 1, Name = UserRoleKey.RoleConst.Admin, NormalizedName = UserRoleKey.RoleConst.Admin };
            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { Id = 1, Name = UserRoleKey.RoleConst.Admin, NormalizedName = UserRoleKey.RoleConst.Admin },
                    new Role { Id = 2, Name = UserRoleKey.RoleConst.InventoryManagement, NormalizedName = UserRoleKey.RoleConst.InventoryManagement },
                    new Role { Id = 3, Name = UserRoleKey.RoleConst.Sale, NormalizedName = UserRoleKey.RoleConst.Sale },
                    new Role { Id = 4, Name = UserRoleKey.RoleConst.Shipper, NormalizedName = UserRoleKey.RoleConst.Shipper });

            var user = new User
            {
                Id = 1,
                UserName = "admin",
                NormalizedEmail = "admin",
                Email = "admin@gmail.com",
                FirstName = "Quan",
                LastName = "Tri",
                Avatar = "",
                PhoneNumber = "",
                NormalizedUserName = "admin",
                PasswordHash = Utils.ComputeHash("123qwe")
            };

            modelBuilder.Entity<User>()
               .HasData(user);

            modelBuilder.Entity<UserRole>()
                .HasData(
                    new UserRole
                    {
                        UserId = 1,
                        User = user,
                        RoleId = 1,
                        Role = role,
                    });
        }
    }
}