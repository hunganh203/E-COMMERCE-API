using Application.Dtos.Review;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> Get(string keySearch, int status);

        Task UpdateStatus(int id, int status);

        Task<PagedResultDto<ReviewDto>> GetAll(GetReviewsInput input);

        Task<PagedResultDto<ReviewDto>> GetByProduct(GetByProductInput input);

        Task<ReviewDto> GetById(int key);

        Task<ReviewDto> GetByOrder(int orderDetailId);

        Task ReviewByCustomer(CustomerReviewInput input, int userId);

        Task Insert(int orderDetailId, int rate, string comment);

        Task Update(int key, ReviewDto entity);
    }
}