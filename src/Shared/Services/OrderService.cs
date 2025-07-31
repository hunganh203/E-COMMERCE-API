using Application.Constants;
using Application.Dtos;
using Application.Dtos.Order;
using Application.Dtos.Review;
using Application.DTOs.Pagination;
using Application.Extensions;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Interfaces.Service.Invoice;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHandleInvoiceService _handleInvoiceService;
        private readonly IReviewRepository _reviewRepository;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IMapper mapper, IConfiguration configuration, IHandleInvoiceService handleInvoiceService, IReviewRepository reviewRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _configuration = configuration;
            _handleInvoiceService = handleInvoiceService;
            _reviewRepository = reviewRepository;
        }

        public async Task<PagedResultDto<OrderDto>> GetOrders(GetOrdersInput input)
        {
            var query = _orderRepository.AsQueryable();

            if (input.SystemStatus > 0)
            {
                switch (input.SystemStatus)
                {
                    case OrderConst.OrderSystemStatus.Finished:
                        query = query.Where(o => o.IsFinish && !o.IsCancel);
                        break;

                    case OrderConst.OrderSystemStatus.Canceled:
                        query = query.Where(o => !o.IsFinish && o.IsCancel);
                        break;

                    default:
                        query = query.Where(o => o.SystemStatus == input.SystemStatus && !o.IsFinish && !o.IsCancel);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(input.KeySearch))
                query = query.Where(x => x.Code.Contains(input.KeySearch) || x.CustomerName.Contains(input.KeySearch));

            if (input.Status > 0)
                query = query.Where(x => input.Status < 0 || x.Status == input.Status);

            if (input.CustomerId.HasValue)
                query = query.Where(x => x.CustomerId == input.CustomerId);

            if (input.StartDate.HasValue && input.EndDate.HasValue)
            {
                var startDate = input.StartDate.Value.Date;
                var endDate = input.EndDate.Value.AddDays(1).Date;
                query = query.Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);
            }

            var result = await query.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var ordersResult = _mapper.Map<List<OrderDto>>(result);

            return new PagedResultDto<OrderDto>
            {
                Items = ordersResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && input.Status < 0 && input.SystemStatus < 0 && !input.StartDate.HasValue && !input.EndDate.HasValue && !input.CustomerId.HasValue
                    ? _orderRepository.Count()
                    : query.Count()
            };
        }

        public async Task<PagedResultDto<OrderDto>> GetOrdersForCustomer(GetOrdersByCustomerInput input, int customerId)
        {
            return await this.GetOrders(new GetOrdersInput
            {
                SystemStatus = -1,
                Status = -1,

                SkipCount = input.SkipCount,
                MaxResultCount = input.MaxResultCount,

                CustomerId = customerId,
                KeySearch = input.KeySearch,
            });
        }

        public async Task<OrderDto> GetById(int key)
        {
            var order = await _orderRepository
                .AsQueryable()
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            var orderDetail = _orderDetailRepository
                .FindByIncludes(x => x.OrderId == order.Id, o => o.Product!).ToList();

            order.OrderDetails = orderDetail;
            var orderDto = _mapper.Map<OrderDto>(order);
            orderDto.OrderDetails.ForEach(od =>
            {
                // todo: Remove test data
                if (od.ProductImage.Contains("http"))
                {
                    od.ProductImageUrl = od.ProductImage;
                    return;
                }

                od.ProductImageUrl = !string.IsNullOrEmpty(od.ProductImage)
                    ? S3Path.GetS3Url(_configuration, od.ProductImage)
                    : "";
            });
            return orderDto;
        }

        public async Task<OrderDto> GetByIdForCustomer(int key, int customerId)
        {
            var order = await _orderRepository
                .AsQueryable()
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == key && x.CustomerId == customerId);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            var orderDetail = _orderDetailRepository
                .FindByIncludes(x => x.OrderId == order.Id, o => o.Product!).ToList();

            order.OrderDetails = orderDetail;
            var orderDto = _mapper.Map<OrderDto>(order);

            var orderDetailIds = orderDto.OrderDetails.Select(x => x.Id);

            var reviews = await _reviewRepository
                .AsQueryable()
                .Where(x => x.CreatedBy == customerId && x.OrderDetailId.HasValue && orderDetailIds.Any(y => y == x.OrderDetailId.GetValueOrDefault()))
                .ToListAsync();

            orderDto.OrderDetails.ForEach(od =>
            {
                // todo: Remove test data
                if (od.ProductImage.Contains("http"))
                {
                    od.ProductImageUrl = od.ProductImage;
                    return;
                }

                od.ProductImageUrl = !string.IsNullOrEmpty(od.ProductImage)
                    ? S3Path.GetS3Url(_configuration, od.ProductImage)
                    : "";
                od.Reviews = _mapper.Map<List<ReviewDto>>(reviews.Where(x => x.OrderDetailId == od.Id));
            });
            return orderDto;
        }

        public async Task<OrderDto> ChangeStatus(int key, int systemStatus, int status)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            switch (systemStatus)
            {
                case OrderConst.OrderSystemStatus.Canceled:
                    order.IsCancel = true;
                    break;

                default:
                    order.SystemStatus = systemStatus;
                    order.Status = status;
                    order.IsFinish = false;
                    order.IsCancel = false;
                    break;
            }

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> Insert(OrderDto entity, int userId)
        {
            var order = new Order()
            {
                Address = entity.Address,
                Code = Guid.NewGuid().ToString("N").Truncate(8).ToUpper(),
                PhoneNumber = entity.PhoneNumber,
                CustomerName = entity.CustomerName,
                CustomerEmail = entity.CustomerEmail,
                CustomerId = entity.CustomerId,
                Note = entity.Note,

                Deposit = entity.Deposit,
                DiscountPrice = entity.DiscountPrice,
                SystemStatus = OrderConst.OrderSystemStatus.Process,
                Status = OrderConst.OrderStatus.WaitingWarehouseConfirm,

                SourceFrom = string.IsNullOrEmpty(entity.SourceFrom) ? OrderConst.SourceFrom.SaleCreate : entity.SourceFrom,
                CreatedDate = entity.CreatedDate ?? DateTimeOffset.Now,
                CreatedBy = entity.CreatedBy ?? userId,

                UpdatedDate = DateTimeOffset.UtcNow,
                UpdatedBy = userId,
            };
            if (entity.OrderDetails.Count > 0)
            {
                var orderDetails = new List<OrderDetail>();

                foreach (var item in entity.OrderDetails)
                {
                    orderDetails.Add(new OrderDetail()
                    {
                        ProductId = item.ProductId,
                        Attribute = item.Attribute,
                        ProductDiscountPrice = item.ProductDiscountPrice,
                        ProductImage = item.ProductImage,
                        ProductName = item.ProductName,
                        ProductPrice = item.ProductPrice,
                        Quantity = item.Quantity,

                        Status = OrderConst.InProcessStatus.ConfirmImportProduct,
                    });
                }

                order.OrderDetails = orderDetails;
            }

            order.TotalPrice = CalculateTotalPriceOrder(order);
            await _orderRepository.AddAsync(order);

            return entity;
        }

        public async Task Update(int key, OrderDto entity, int userId)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");
            order.Note = entity.Note;
            order.Address = entity.Address;
            order.PhoneNumber = entity.PhoneNumber;
            order.PhoneNumber = entity.PhoneNumber;
            await Task.WhenAll(_orderDetailRepository.DeleteRangeAsync(order.OrderDetails));
            if (entity.OrderDetails.Count > 0)
            {
                var orderDetails = new List<OrderDetail>();

                foreach (var item in entity.OrderDetails)
                {
                    orderDetails.Add(new OrderDetail()
                    {
                        OrderId = key,
                        ProductId = item.ProductId,
                        Attribute = item.Attribute,
                        ProductDiscountPrice = item.ProductDiscountPrice,
                        ProductImage = item.ProductImage,
                        ProductName = item.ProductName,
                        ProductPrice = item.ProductPrice,
                        Quantity = item.Quantity,
                    });
                }
                await _orderDetailRepository.AddRangeAsync(orderDetails);
            }

            order.TotalPrice = CalculateTotalPriceOrder(order);

            order = UpdateAudit(order, userId);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task Delete(int key)
        {
            var order = await _orderRepository
                .AsQueryable()
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            var orderDetailIds = order.OrderDetails.Select(x => x.Id);

            var reviews = await _reviewRepository
                .AsQueryable()
                .Where(x => x.OrderDetailId.HasValue && orderDetailIds.Any(y => y == x.OrderDetailId.GetValueOrDefault()))
                .ToListAsync();

            if (reviews.Count > 0)
                await this._reviewRepository.DeleteRangeAsync(reviews);

            if (order.OrderDetails.Count > 0)
                await this._orderDetailRepository.DeleteRangeAsync(order.OrderDetails);

            await this._orderRepository.DeleteAsync(order);
        }

        public async Task<OrderDto> UpdateDeposit(int key, double value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            //if (order.Deposit > order.TotalPrice)
            //    throw new Exception("DEPOSIT_PRICE_INVALID");

            order.Deposit = value;
            order.TotalPrice = CalculateTotalPriceOrder(order);

            order = UpdateAudit(order, userId);

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateDiscountPrice(int key, double value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            //if (value > order.OrderDetails.Sum(orderOrderDetail => (orderOrderDetail.Quantity ?? 0) * (orderOrderDetail.ProductDiscountPrice ?? 0)))
            //    throw new Exception("DISCOUNT_PRICE_INVALID");

            order.DiscountPrice = value;
            order.TotalPrice = CalculateTotalPriceOrder(order);

            order = UpdateAudit(order, userId);
            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateAddress(int key, string value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            order.Address = value;
            order = UpdateAudit(order, userId);
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdatePhoneNumber(int key, string value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            order.PhoneNumber = value;
            order = UpdateAudit(order, userId);
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateShippingInfo(int key, OrderShippingInfo value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            order.PhoneNumber = value.Phone;
            order.CustomerName = value.Name;
            order.CustomerEmail = value.Email;
            order.Address = value.Address;

            order = UpdateAudit(order, userId);
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateNote(int key, string value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            order.Note = value;

            order = UpdateAudit(order, userId);

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateStockNote(int key, string value, int userId)
        {
            var order = await _orderRepository.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == key);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");

            order.StockNote = value;

            order = UpdateAudit(order, userId);
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<FileStorageDto> GenerateInvoiceOfOrder(int orderId)
        {
            var order = await _orderRepository
                .AsQueryable()
                .Include(x => x.Customer)
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                throw new Exception("ORDER_DOES_NOT_EXIST");
            var result = await _handleInvoiceService.GenerateInvoiceOfOrder(order);

            if (result == null)
                throw new Exception("FILE_NOT_EXIST");

            result.PathUrl = S3Path.GetS3Url(_configuration, result.Path);

            return result;
        }

        private double CalculateTotalPriceOrder(Order order)
        {
            var result = order.OrderDetails.Where(x => x.IsUsed).Sum(orderOrderDetail => (orderOrderDetail.Quantity ?? 0) * (orderOrderDetail.ProductDiscountPrice ?? 0));
            result -= order.DiscountPrice ?? 0;
            return result < 0 ? 0 : result;
        }

        private Order UpdateAudit(Order order, int userId)
        {
            order.UpdatedDate = DateTimeOffset.Now;
            order.UpdatedBy = userId;

            return order;
        }
    }
}