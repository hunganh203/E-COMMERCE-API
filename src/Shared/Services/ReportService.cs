using Application.Constants;
using Application.Dtos.Order;
using Application.Dtos.Product;
using Application.Dtos.Report;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IConfiguration _configuration;

        public ReportService(IOrderRepository orderRepository, IMapper mapper, IProductRepository productRepository, IOrderDetailRepository orderDetailRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _orderDetailRepository = orderDetailRepository;
            _configuration = configuration;
        }

        public async Task<ReportHighlight> GetHighlight(DateTime? date)
        {
            date ??= DateTime.Now;

            var highlight = new ReportHighlight
            {
                TotalNewOrder = _orderRepository
                    .AsQueryable()
                    .Where(x => x.CreatedDate.Day == date.Value.Day &&
                                x.CreatedDate.Month == date.Value.Month &&
                                x.CreatedDate.Year == date.Value.Year)
                    .Count(),
                DailySales = _orderRepository
                    .AsQueryable()
                    .Where(x => x.CreatedDate.Day == date.Value.Day &&
                                x.CreatedDate.Month == date.Value.Month &&
                                x.CreatedDate.Year == date.Value.Year)
                    .Sum(x => x.TotalPrice.GetValueOrDefault()),
                TotalOrder = _orderRepository
                    .AsQueryable()
                    .Where(x => x.CreatedDate.Month == date.Value.Month &&
                                x.CreatedDate.Year == date.Value.Year)
                    .Count(),
                SalesRevenue = _orderRepository
                    .AsQueryable()
                    .Where(x => x.CreatedDate.Month == date.Value.Month &&
                                x.CreatedDate.Year == date.Value.Year)
                    .Sum(x => x.TotalPrice.GetValueOrDefault()),
                OrderQty = new List<int>(),
                OrderQtyByStatus = new List<int>(),
                Revenues = new List<double>()
            };

            var dateNow = DateTime.Now;

            for (var i = 1; i < 13; i++)
            {
                var totalOrder = _orderRepository
                    .AsQueryable()
                    .Where(x => x.CreatedDate.Month == i && x.CreatedDate.Year == date.Value.Year)
                    .Count();

                var totalAmount = _orderRepository
                    .AsQueryable()
                    .Where(x => x.CreatedDate.Month == i && x.CreatedDate.Year == date.Value.Year)
                            .Sum(x => x.TotalPrice.GetValueOrDefault());

                if ((date.Value.Year == dateNow.Year && dateNow.Month >= i) || date.Value.Year != dateNow.Year)
                {
                    highlight.OrderQty.Add(totalOrder);
                }
                highlight.Revenues.Add(totalAmount);
            }

            new List<int>{
                OrderConst.OrderSystemStatus.Process,
                OrderConst.OrderSystemStatus.InProcess, 
                OrderConst.OrderSystemStatus.Finished,
                OrderConst.OrderSystemStatus.Canceled,
            }.ForEach(status =>
            {
                highlight.OrderQtyByStatus.Add(_orderRepository
                    .AsQueryable()
                    .Where(x => (status == OrderConst.OrderSystemStatus.Finished || status == OrderConst.OrderSystemStatus.Canceled) ?
                    (status == OrderConst.OrderSystemStatus.Finished ? x.IsFinish : x.IsCancel)
                    : x.SystemStatus == status)
                    .Where(x => x.CreatedDate.Month == date.Value.Month && x.CreatedDate.Year == date.Value.Year)
                    .Count());
            });

            return await Task.Run(() => highlight);
        }

        public async Task<List<OrderDto>> GetGeneralReport(string keySearch, int status, DateTime? fDate, DateTime? tDate)
        {
            var query = _orderRepository
                .AsQueryable()
                .Include(x => x.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keySearch))
                query = query.Where(x => x.Id.ToString().Contains(keySearch) || (x.Customer!.FirstName + " " + x.Customer.LastName).Contains(keySearch));

            if (status > 0)
                query = query.Where(x => status < 0 || x.Status == status);

            if (fDate.HasValue && tDate.HasValue)
            {
                var fd = fDate.Value.Date;
                var td = tDate.Value.AddDays(1).Date;
                query = query.Where(x => x.CreatedDate >= fd && x.CreatedDate < td);
            }

            var orders = await query
                .OrderByDescending(x => x.CreatedDate).ToListAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<PagedResultDto<ProductDto>> GetProductReport(GetProductReport input)
        {
            if (string.IsNullOrWhiteSpace(input.KeySearch))
                input.KeySearch = null;

            var productsQuery = _productRepository
                .AsQueryable()
                .Where(x => !input.CategoryId.HasValue || x.MenuId == input.CategoryId)
                .Where(x => !input.Status.HasValue || x.Status == input.Status)

                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Name.Contains(input.KeySearch.Trim()) || x.Code.Contains(input.KeySearch.Trim())));

            //foreach (var product in productsDto)
            //{
            //    product.ImageUrl = !string.IsNullOrEmpty(product.Image)
            //        ? S3Path.GetS3Url(_configuration, product.Image)
            //        : "";
            //}

            var products = await productsQuery.Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (products is not { Count: > 0 })
            {
                var productResult = _mapper.Map<List<ProductDto>>(products);
                return new PagedResultDto<ProductDto>
                {
                    Items = productResult,
                    TotalCount = productResult.Count
                };
            }

            var productsDto = _mapper.Map<List<ProductDto>>(products);

            foreach (var item in productsDto)
            {
                var query = _orderDetailRepository.AsQueryable()
                    .Where(x => x.ProductId == item.Id);

                item.ImageUrl = !string.IsNullOrEmpty(item.Image)
                    ? S3Path.GetS3Url(_configuration, item.Image)
                    : "";

                if (input.StartDate.HasValue && input.EndDate.HasValue)
                {
                    var starDate = input.StartDate.Value.Date;
                    var endDate = input.EndDate.Value.AddDays(1).Date;
                    query = query.Where(x => x.Order!.CreatedDate >= starDate && x.Order.CreatedDate < endDate);
                }

                item.TotalQty = query.Sum(x => x.Quantity);
                item.TotalAmount = query.Sum(x => x.Quantity.GetValueOrDefault() * x.ProductDiscountPrice);
            }

            var productsResult = _mapper.Map<List<ProductDto>>(productsDto);

            return new PagedResultDto<ProductDto>
            {
                Items = productsResult.OrderByDescending(x => x.TotalAmount)
                    .ThenByDescending(x => x.TotalQty).ToList(),
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && !input.CategoryId.HasValue && !input.Status.HasValue
                    ? _productRepository.Count()
                    : _productRepository
                        .Count(x => (!input.CategoryId.HasValue || x.MenuId == input.CategoryId)
                                    && (string.IsNullOrEmpty(input.KeySearch) || x.Name.Contains(input.KeySearch.Trim()))
                                    && (!input.Status.HasValue || x.Status == input.Status))
            };
        }

        public async Task<GetRevenueReportOutput> GetRevenueReport(DateTime? date)
        {
            date ??= DateTime.Now;

            var revenues = new List<double>();
            var orderQty = new List<int>();

            for (var i = 1; i < 13; i++)
            {
                var totalOrder = _orderRepository.AsQueryable()
                    .Where(x => x.CreatedDate.Month == i &&
                                x.CreatedDate.Year == date.Value.Year)
                    .Count();

                var totalAmount = _orderRepository.AsQueryable()
                    .Where(x => x.CreatedDate.Month == i &&
                                x.CreatedDate.Year == date.Value.Year)
                    .Sum(x => x.TotalPrice.GetValueOrDefault());

                orderQty.Add(totalOrder);
                revenues.Add(totalAmount);
            }

            return await Task.Run(() => new GetRevenueReportOutput
            {
                Revenues = revenues,
                OrderQty = orderQty
            });
        }
    }
}