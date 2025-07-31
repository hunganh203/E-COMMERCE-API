using Application.Constants;
using Application.Dtos.Review;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    public class ReviewsController : BaseAuthAdminApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IUserService userService, IReviewService reviewService) : base(userService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("get-all")]
        public async Task<AjaxResponse<PagedResultDto<ReviewDto>>> GetAll([FromQuery] GetReviewsInput input)
        {
            var reviewDtos = await _reviewService.GetAll(input);
            return new AjaxResponse<PagedResultDto<ReviewDto>>(reviewDtos);
        }

        [Route("update-status/{id}")]
        [HttpGet]
        public async Task<AjaxResponse> UpdateStatus(int id, int status)
        {
            try
            {
                await this._reviewService.UpdateStatus(id, status);
                return new AjaxResponse(true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse(new ErrorInfo(ex.Message));
            }
        }

        [Route("review")]
        [HttpGet]
        public async Task<AjaxResponse> Review(int orderDetailId, int rate, string comment)
        {
            try
            {
                await this._reviewService.Insert(orderDetailId, rate, comment);
                return new AjaxResponse(true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse(new ErrorInfo(ex.Message));
            }
        }

        [Route("get-by-order/{orderDetailId}")]
        [HttpGet]
        public async Task<AjaxResponse> GetByOrder(int orderDetailId)
        {
            try
            {
                await this._reviewService.GetByOrder(orderDetailId);
                return new AjaxResponse(true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse(new ErrorInfo(ex.Message));
            }
        }
    }
}