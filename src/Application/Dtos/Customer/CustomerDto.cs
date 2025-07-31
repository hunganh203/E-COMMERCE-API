using Application.Dtos.Order;

namespace Application.Dtos.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Avatar { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; } = DateTimeOffset.MinValue;

        public List<OrderDto> Orders { get; set; } = new();
    }

    public class UpdateCustomerPasswordInput
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string RePassword { get; set; } = string.Empty;
    }
}