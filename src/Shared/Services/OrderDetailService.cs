using Application.Constants;
using Application.Dtos.Order;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IProductRepository productRepository, IMapper mapper, IConfiguration configuration, IOrderRepository orderRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _configuration = configuration;
            _orderRepository = orderRepository;
        }

        public async Task DeleteById(int key)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == key);

            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");

            await _orderDetailRepository.DeleteAsync(orderDetail);

            var order = await _orderRepository.AsQueryable()
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == orderDetail.OrderId);

            if (order == null)
                throw new Exception(CustomResponseMessage.OrderDoesNotExist);

            order.Status = HandleStatusOrder(order);
            order.TotalPrice = this.CalculateTotalPriceOrder(order);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderDetailDto> GetById(int key)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == key);
            var orderDetailDto = _mapper.Map<OrderDetailDto>(orderDetail);
            if (orderDetailDto == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");

            orderDetailDto.ProductImageUrl = !string.IsNullOrEmpty(orderDetailDto.ProductImage)
                ? S3Path.GetS3Url(_configuration, orderDetailDto.ProductImage)
                : "";

            return orderDetailDto;
        }

        public async Task<OrderDetailDto> Insert(OrderDetailDto entity)
        {
            var product = await _productRepository.FirstOrDefaultAsync(x => x.Id == entity.ProductId);

            if (product == null)
                throw new Exception("PRODUCT_DOES_NOT_EXIST");

            var orderDetail = new OrderDetail()
            {
                Attribute = entity.Attribute,
                ProductId = product.Id,
                OrderId = entity.OrderId,
                ProductDiscountPrice = product.DiscountPrice,
                ProductImage = product.Image,
                ProductName = product.Name,
                ProductPrice = product.Price,
                Quantity = entity.Quantity,
                Status = OrderConst.InProcessStatus.ConfirmImportProduct,
                IsUsed = true,
                RefOrderDetailId = entity.RefOrderDetailId,
            };
            var result = await _orderDetailRepository.AddAsync(orderDetail);

            if (result == null)
                throw new Exception(CustomResponseMessage.InsertErr);

            var order = await _orderRepository.AsQueryable()
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == orderDetail.OrderId);

            if (order == null)
                throw new Exception(CustomResponseMessage.OrderDoesNotExist);

            order.Status = HandleStatusOrder(order);
            order.TotalPrice = this.CalculateTotalPriceOrder(order);
            await _orderRepository.UpdateAsync(order);

            var orderDetailDto = _mapper.Map<OrderDetailDto>(result);
            orderDetailDto.ProductImageUrl = !string.IsNullOrEmpty(orderDetailDto.ProductImage)
                ? S3Path.GetS3Url(_configuration, orderDetailDto.ProductImage)
                : "";

            return orderDetailDto;
        }

        public async Task Update(int key, OrderDetailDto entity)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.Quantity = entity.Quantity;
            await _orderDetailRepository.UpdateAsync(orderDetail);
        }

        public async Task<bool> UpdateQuantity(int orderDetailId, int quantity)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.Quantity = quantity;
            await _orderDetailRepository.UpdateAsync(orderDetail);

            var order = await _orderRepository.AsQueryable()
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == orderDetail.OrderId);

            if (order == null)
                throw new Exception(CustomResponseMessage.OrderDoesNotExist);

            order.TotalPrice = this.CalculateTotalPriceOrder(order);
            await _orderRepository.UpdateAsync(order);

            return true;
        }

        public async Task<bool> UpdatePriceStockReceiving(int orderDetailId, double priceStockReceiving)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.PriceStockReceiving = priceStockReceiving;
            await _orderDetailRepository.UpdateAsync(orderDetail);
            return true;
        }

        public async Task<bool> UpdateStockReceivingName(int orderDetailId, string stockReceivingName)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.StockReceivingName = stockReceivingName;
            await _orderDetailRepository.UpdateAsync(orderDetail);
            return true;
        }

        public async Task<UpdateOrderDetailStatusOutput> UpdateStatus(int orderDetailId, int status)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.Status = status;

            if (status is OrderConst.InProcessStatus.Return or OrderConst.InProcessStatus.Renew)
            {
                orderDetail.IsUsed = false;
            }

            await _orderDetailRepository.UpdateAsync(orderDetail);

            var order = await _orderRepository.AsQueryable()
                     .Include(x => x.OrderDetails)
                     .FirstOrDefaultAsync(x => x.Id == orderDetail.OrderId);

            if (order == null)
                throw new Exception(CustomResponseMessage.OrderDoesNotExist);

            if (order.SystemStatus == OrderConst.OrderSystemStatus.Finished)
            {
                order.SystemStatus = OrderConst.OrderSystemStatus.InProcess;
                order.IsFinish = false;
            }

            order.Status = HandleStatusOrder(order);

            if (order.OrderDetails.Count(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.CustomerHasReceived)
                == order.OrderDetails.Count(od => od.IsUsed))
            {
                order.IsFinish = true;
                order.Status = OrderConst.OrderStatus.Finished;
                order.SystemStatus = OrderConst.OrderSystemStatus.Finished;
            }

            await _orderRepository.UpdateAsync(order);

            return new UpdateOrderDetailStatusOutput
            {
                OrderId = order.Id,
                OrderStatus = order.Status,
                OrderSystemStatus = order.SystemStatus,

                OrderDetailId = orderDetail.Id,
                OrderDetailStatus = orderDetail.Status,
            };
        }

        public async Task<bool> UpdateNote(int orderDetailId, string note)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.Note = note;
            await _orderDetailRepository.UpdateAsync(orderDetail);
            return true;
        }

        public async Task<bool> UpdateCheckingCode(int orderDetailId, string checkingCode)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.CheckingCode = checkingCode;
            await _orderDetailRepository.UpdateAsync(orderDetail);
            return true;
        }

        public async Task<bool> UpdatePriceProduct(int orderDetailId, double productPrice, double productDiscountPrice)
        {
            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(x => x.Id == orderDetailId);
            if (orderDetail == null)
                throw new Exception("ORDER_DETAIL_DOES_NOT_EXIST");
            orderDetail.ProductPrice = productPrice;
            orderDetail.ProductDiscountPrice = productDiscountPrice;

            await _orderDetailRepository.UpdateAsync(orderDetail);

            var order = await _orderRepository.AsQueryable()
            .Include(x => x.OrderDetails)
            .FirstOrDefaultAsync(x => x.Id == orderDetail.OrderId);

            if (order == null)
                throw new Exception(CustomResponseMessage.OrderDoesNotExist);

            order.TotalPrice = this.CalculateTotalPriceOrder(order);
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private double CalculateTotalPriceOrder(Order order)
        {
            var result = order.OrderDetails.Where(x => x.IsUsed).Sum(orderOrderDetail => (orderOrderDetail.Quantity ?? 0) * (orderOrderDetail.ProductDiscountPrice ?? 0));
            result -= order.DiscountPrice ?? 0;
            return result < 0 ? 0 : result;
        }

        private int HandleStatusOrder(Order? order)
        {
            if (order?.OrderDetails == null || order.OrderDetails.Count <= 0)
            {
                return -2;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.Process
                && order.OrderDetails.Any(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.PendingConfirmImportProduct))
            {
                return OrderConst.OrderStatus.WaitingImportProduct;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.InProcess
                && order.OrderDetails.Any(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.ConfirmImportProduct))
            {
                return OrderConst.OrderStatus.WaitingImportProduct;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.InProcess
              && (order.OrderDetails.Any(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.WaitCustomerApprove)
                  || order.OrderDetails.Any(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.ImportingProduct)))
            {
                return OrderConst.OrderStatus.ImportingProduct;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.InProcess
              && order.OrderDetails.Any(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.PendingSend))
            {
                return OrderConst.OrderStatus.PendingSend;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.InProcess
              && order.OrderDetails.Any(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.PendingSend))
            {
                return OrderConst.OrderStatus.PendingSend;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.InProcess &&
              order.OrderDetails.Count(od => od.IsUsed && od.Status == OrderConst.InProcessStatus.CustomerApproved)
              == order.OrderDetails.Count(od => od.IsUsed))
            {
                return OrderConst.OrderStatus.FinishImportProduct;
            }

            if (order.SystemStatus == OrderConst.OrderSystemStatus.InProcess &&
              order.OrderDetails.Any(od => od.IsUsed &&
                (
                  od.Status == OrderConst.InProcessStatus.ReceivingToVietNam
                  || od.Status == OrderConst.InProcessStatus.ReceivingToCustomer
                )
              ))
            {
                return OrderConst.OrderStatus.Delivering;
            }

            return -2;
        }
    }
}