﻿using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetByBuyerProfileId(Guid buyerProfileId)
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .Where(o => o.BuyerProfileId == buyerProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetBySellerProfileId(Guid sellerProfileId)
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .Where(o => o.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        public override async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .SingleOrDefaultAsync(o => o.Id == id);
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByProductId(Guid productId)
        {
            return await _dbSet
                .Include(o => o.Items)
                .Where(o => o.Items.Any(oi => oi.ProductId == productId))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Order> Orders, int PageCount)> GetForSeller(Guid sellerProfileId, OrderQuery orderQuery)
        {
            var query = _dbSet
                .Include(o => o.BuyerProfile)
                    .ThenInclude(b => b!.User)
                .Include(o => o.SellerProfile)
                    .ThenInclude(s => s!.User)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .Where(p => p.SellerProfileId == sellerProfileId)
                .AsQueryable();

            int pageCount = 1;
            if (orderQuery.ProductName != null)
            {
                query = query.Where(o => o.Items.Any(oi => oi.Product!.Name == orderQuery.ProductName));
            }
            if (orderQuery.Status != null)
            {
                if (orderQuery.Status == "Completed")
                {
                    query = query.Where(o => o.Status == OrderStatus.Cancelled || o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Refunded);
                }
                else if (orderQuery.Status == "Onhold")
                {
                    query = query.Where(o => o.Status == OrderStatus.AwaitingConfirmation || o.Status == OrderStatus.AwaitingBuyer || o.Status == OrderStatus.Reported);
                }
                else if (orderQuery.Status == "AwaitingConfirmation")
                {
                    query = query.Where(o => o.Status == OrderStatus.AwaitingConfirmation || o.Status == OrderStatus.Reported);
                }
                else
                {
                    var orderStatus = Enum.TryParse<OrderStatus>(orderQuery.Status, out var status);

                    if (orderStatus)
                    {
                        query = query.Where(o => o.Status == status);
                    }
                }     
            }
            if (!string.IsNullOrEmpty(orderQuery.Search))
            {
                query = query.Where(
                    x => x.BuyerProfile!.User!.FirstName.Contains(orderQuery.Search) ||
                    x.BuyerProfile!.User!.LastName.Contains(orderQuery.Search) ||
                    x.Items.Any(oi => oi.Product!.Name.Contains(orderQuery.Search)) ||
                    x.DateTimeCreated.ToString().Contains(orderQuery.Search));
            }
            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
            }
            if (orderQuery.Page != null && orderQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)orderQuery.PageSize);

                if (orderQuery.Page > pageCount)
                {
                    orderQuery.Page = pageCount;
                }
                else if (orderQuery.Page < 1)
                {
                    orderQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    orderQuery.Page = 1;
                }

                int skipAmount = ((int)orderQuery.Page - 1) * (int)orderQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)orderQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }

        public async Task<(IEnumerable<Order> Orders, int PageCount)> GetForAdmin(OrderReportQuery orderReportQuery)
        {
            var confirmationMaxDate = DateTime.UtcNow.AddDays(-7);
            var query = _dbSet
                .Include(o => o.BuyerProfile)
                    .ThenInclude(b => b!.User)
                .Include(o => o.SellerProfile)
                    .ThenInclude(s => s!.User)
                .Include(o => o.SellerProfile)
                    .ThenInclude(s => s!.SellerApplication)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .Where(o => (o.OrderReport != null && o.Status == OrderStatus.Reported) || 
                (o.Status == OrderStatus.AwaitingConfirmation && o.DateTimeUpdated < confirmationMaxDate))
                .AsQueryable();

            int pageCount = 1;

            if (!string.IsNullOrEmpty(orderReportQuery.SellerName))
            {
                query = query.Where(o => o.SellerProfile!.Name.ToLower().Contains(orderReportQuery.SellerName));
            }
            if (!string.IsNullOrEmpty(orderReportQuery.BuyerName))
            {
                query = query.Where(
                    x => x.BuyerProfile!.User!.FirstName.ToLower().Contains(orderReportQuery.BuyerName) ||
                    x.BuyerProfile!.User!.LastName.ToLower().Contains(orderReportQuery.BuyerName)
                    );
            }
            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
            }
            if (orderReportQuery.Page != null && orderReportQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)orderReportQuery.PageSize);

                if (orderReportQuery.Page > pageCount)
                {
                    orderReportQuery.Page = pageCount;
                }
                else if (orderReportQuery.Page < 1)
                {
                    orderReportQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    orderReportQuery.Page = 1;
                }

                int skipAmount = ((int)orderReportQuery.Page - 1) * (int)orderReportQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)orderReportQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }

        public async Task<int> GetByBuyerProfileIdCount(Guid buyerProfileId)
        {
            return await _dbSet.CountAsync(
                o => o.BuyerProfileId == buyerProfileId && 
                o.Status == OrderStatus.AwaitingBuyer
            );
        }

        public async Task<(IEnumerable<Order> Orders, int PageCount)> GetForBuyer(Guid buyerProfileId, OrderQuery orderQuery)
        {
            var query = _dbSet
                .Include(o => o.BuyerProfile)
                    .ThenInclude(b => b!.User)
                .Include(o => o.SellerProfile)
                    .ThenInclude(s => s!.User)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Gift)
                .Where(p => p.BuyerProfileId == buyerProfileId)
                .AsQueryable();

            int pageCount = 1;
            if (orderQuery.ProductName != null)
            {
                query = query.Where(o => o.Items.Any(oi => oi.Product!.Name == orderQuery.ProductName));
            }
            if (orderQuery.Status != null)
            {
                if (orderQuery.Status == "Completed")
                {
                    query = query.Where(o => o.Status == OrderStatus.Cancelled || o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Refunded);
                }
                else if (orderQuery.Status == "Onhold")
                {
                    query = query.Where(o => o.Status == OrderStatus.AwaitingConfirmation || o.Status == OrderStatus.Reported);
                }
                else
                {
                    var orderStatus = Enum.TryParse<OrderStatus>(orderQuery.Status, out var status);

                    if (orderStatus)
                    {
                        query = query.Where(o => o.Status == status);
                    }
                }
            }
            if (!string.IsNullOrEmpty(orderQuery.Search))
            {
                query = query.Where(
                    x => x.BuyerProfile!.User!.FirstName.Contains(orderQuery.Search) ||
                    x.BuyerProfile!.User!.LastName.Contains(orderQuery.Search) ||
                    x.Items.Any(oi => oi.Product!.Name.Contains(orderQuery.Search)) ||
                    x.DateTimeCreated.ToString().Contains(orderQuery.Search));
            }
            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
            }
            if (orderQuery.Page != null && orderQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)orderQuery.PageSize);

                if (orderQuery.Page > pageCount)
                {
                    orderQuery.Page = pageCount;
                }
                else if (orderQuery.Page < 1)
                {
                    orderQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    orderQuery.Page = 1;
                }

                int skipAmount = ((int)orderQuery.Page - 1) * (int)orderQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)orderQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
