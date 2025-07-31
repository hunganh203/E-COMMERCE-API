using Application.Constants;
using Application.Dtos.Review;
using Application.DTOs.Pagination;
using Application.Enums;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICustomerRepository _customerRepository;

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IOrderDetailRepository orderDetailRepository, IConfiguration configuration, ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _orderDetailRepository = orderDetailRepository;
            _configuration = configuration;
            _customerRepository = customerRepository;
        }

        public Task<List<ReviewDto>> Get(string keySearch, int status)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<ReviewDto>> GetAll(GetReviewsInput input)
        {
            var reviewsQuery = _reviewRepository
                .AsQueryable()
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => input.Status < 0 || x.Status == input.Status)
                .Include(x => x.Product)
                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Content.Contains(input.KeySearch.Trim()) || x.Product!.Name.Contains(input.KeySearch.Trim())));

            var reviews = await reviewsQuery.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (reviews is not { Count: > 0 })
            {
                var productResult = _mapper.Map<List<ReviewDto>>(reviews);
                return new PagedResultDto<ReviewDto>
                {
                    Items = productResult,
                    TotalCount = productResult.Count
                };
            }

            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);

            var createdByIds = reviewDtos.Select(x => x.CreatedBy);

            var createdBy = await _customerRepository.AsQueryable()
                .Select(x => new { x.Id, Name = x.FirstName + "" + x.LastName })
                .Where(x => createdByIds.Any(y => y == x.Id))
                .ToListAsync();

            reviewDtos.ForEach(rv =>
            {
                rv.Product.ImageUrl = !string.IsNullOrEmpty(rv.Product.Image)
                    ? S3Path.GetS3Url(_configuration, rv.Product.Image)
                    : "";

                rv.CreatedByName = createdBy.FirstOrDefault(x => x.Id == rv.CreatedBy)?.Name ?? "Chưa xác định";
            });

            return new PagedResultDto<ReviewDto>
            {
                Items = reviewDtos,
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && input.Status < 0
                    ? _reviewRepository.Count()
                    : await reviewsQuery.CountAsync()
            };
        }

        public async Task<PagedResultDto<ReviewDto>> GetByProduct(GetByProductInput input)
        {
            var reviewsQuery = _reviewRepository
                .AsQueryable()
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.Status == ReviewConst.ReviewStatus.Approved)
                .Where(x => x.ProductId == input.ProductId);

            var reviews = await reviewsQuery.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (reviews is not { Count: > 0 })
            {
                return new PagedResultDto<ReviewDto>
                {
                    Items = new List<ReviewDto>(),
                    TotalCount = 0
                };
            }

            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);

            var createdByIds = reviewDtos.Select(x => x.CreatedBy);

            var createdBy = await _customerRepository.AsQueryable()
                .Select(x => new { x.Id, Name = x.FirstName + "" + x.LastName, Avatar = x.Avatar })
                .Where(x => createdByIds.Any(y => y == x.Id))
                .ToListAsync();

            reviewDtos.ForEach(rv =>
            {
                var created = createdBy.FirstOrDefault(x => x.Id == rv.CreatedBy);

                rv.CreatedByName = created?.Name ?? "Chưa xác định";
                rv.CreatedByAvatarUrl = !string.IsNullOrEmpty(created?.Avatar)
                    ? S3Path.GetS3Url(_configuration, created?.Avatar ?? "", BucketType.UserAvatar)
                    : "";
                
            });

            return new PagedResultDto<ReviewDto>
            {
                Items = reviewDtos,
                TotalCount = _reviewRepository.Count()
            };
        }

        public Task<ReviewDto> GetById(int key)
        {
            throw new NotImplementedException();
        }

        public async Task<ReviewDto> GetByOrder(int orderDetailId)
        {
            var reviewDtos = await _reviewRepository.AsQueryable()
                .Where(x => x.OrderDetailId == orderDetailId)
                .ToListAsync();

            return _mapper.Map<ReviewDto>(reviewDtos);
        }

        public Task<ReviewDto> Insert(ReviewDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(int orderDetailId, int rate, string comment)
        {
            var isFinishOrder = await _orderDetailRepository.AsQueryable().Include(x => x.Order)
                .AnyAsync(x => x.Id == orderDetailId && x.Order!.IsFinish);

            if (isFinishOrder)
            {
                var review = await _reviewRepository.FirstOrDefaultAsync(x => x.OrderDetailId == orderDetailId);

                if (review == null)
                {
                    var orderDetail = await _orderDetailRepository
                        .AsQueryable()
                        .Include(x => x.Order)
                        .FirstOrDefaultAsync(x => x.Id == orderDetailId);

                    review = new Review()
                    {
                        Status = ReviewConst.ReviewStatus.PendingApprove,
                        Content = comment,
                        CreatedDate = DateTimeOffset.Now,
                        CreatedBy = orderDetail?.Order?.CustomerId ?? 0,
                        OrderDetailId = orderDetailId,
                        ProductId = orderDetail?.ProductId ?? Guid.Empty,
                        Star = rate,
                    };

                    await _reviewRepository.AddAsync(review);
                }
                else
                {
                    if (review.Star != rate || review.Content != comment)
                    {
                        review.Star = rate;
                        review.Content = comment;
                        if (review.Status == ReviewConst.ReviewStatus.Approved)
                            review.Status = ReviewConst.ReviewStatus.UpdatePendingApprove;

                        await _reviewRepository.UpdateAsync(review);
                    }
                }
            }
        }

        public async Task ReviewByCustomer(CustomerReviewInput input, int userId)
        {
            var isValidOrder = await _orderDetailRepository.AsQueryable()
                .Include(x => x.Order)
                .AnyAsync(x => x.Id == input.OrderDetailId && x.Order!.IsFinish && x.Order.CustomerId == userId);

            if (isValidOrder)
            {
                var review = await _reviewRepository.FirstOrDefaultAsync(x => x.OrderDetailId == input.OrderDetailId);

                if (review == null)
                {
                    var orderDetail = await _orderDetailRepository
                        .AsQueryable()
                        .Include(x => x.Order)
                        .FirstOrDefaultAsync(x => x.Id == input.OrderDetailId);

                    review = new Review()
                    {
                        Status = ReviewConst.ReviewStatus.PendingApprove,
                        Content = input.Content,
                        CreatedDate = DateTimeOffset.Now,
                        CreatedBy = userId,
                        OrderDetailId = input.OrderDetailId,
                        ProductId = orderDetail?.ProductId ?? Guid.Empty,
                        Star = input.Star,
                    };

                    await _reviewRepository.AddAsync(review);
                }
                else
                {
                    if (review.Star != input.Star || review.Content != input.Content)
                    {
                        review.Star = input.Star;
                        review.Content = input.Content;
                        if (review.Status == ReviewConst.ReviewStatus.Approved)
                            review.Status = ReviewConst.ReviewStatus.UpdatePendingApprove;

                        await _reviewRepository.UpdateAsync(review);
                    }
                }
            }
        }

        public Task Update(int key, ReviewDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateStatus(int id, int status)
        {
            var review = await _reviewRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (review != null)
            {
                review.Status = status;
                await _reviewRepository.UpdateAsync(review);
            }
        }
    }
}