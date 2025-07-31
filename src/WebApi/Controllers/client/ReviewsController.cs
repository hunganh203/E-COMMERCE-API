using Application.Dtos.Review;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    public class ReviewsController : BaseNonAuthClientApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("get-by-product")]
        public async Task<AjaxResponse<PagedResultDto<ReviewDto>>> GetByProduct([FromQuery] GetByProductInput input)
        {
            var reviewDtos = await _reviewService.GetByProduct(input);
            return new AjaxResponse<PagedResultDto<ReviewDto>>(reviewDtos);
        }
    }
}