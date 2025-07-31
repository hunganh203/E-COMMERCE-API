using Application.Dtos;
using Application.Dtos.Article;
using Application.Dtos.Customer;
using Application.Dtos.Menu;
using Application.Dtos.Order;
using Application.Dtos.Product;
using Application.Dtos.ProductDisplay;
using Application.Dtos.Review;
using Application.Dtos.Role;
using Application.Dtos.User;
using Application.Dtos.UserClaim;
using Application.Dtos.UserRole;
using AutoMapper;
using Domain.Entities;
using Attribute = Domain.Entities.Attribute;

namespace Shared.Mappings
{
    public class CustomDtoMapper : Profile
    {
        public CustomDtoMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Attribute, AttributeDto>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductAttribute, ProductAttributeDto>().ReverseMap();
            CreateMap<ProductImage, ProductImageDto>().ReverseMap();
            CreateMap<ProductRelated, ProductRelatedDto>().ReverseMap();

            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<WebsiteDto, Website>().ReverseMap();
            CreateMap<OrderDto, Order>().ReverseMap();
            CreateMap<OrderDetailDto, OrderDetail>().ReverseMap();
            CreateMap<Website, WebsiteDto>().ReverseMap();

            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<ProductDisplayDto, ProductDisplay>().ReverseMap();

            CreateMap<UserClaim, UserClaimDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<UserRole, UserRoleDto>().ReverseMap();

            CreateMap<Article, ArticleDto>().ReverseMap();
        }
    }
}