using Application.DTOs.Pagination;

namespace Application.Dtos.UserClaim
{
    public class GetUserClaimInput : PagedInputDto
    {
        public string KeySearch { get; set; } = string.Empty;
    }
}