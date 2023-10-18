﻿using keepscape_api.Models;
using keepscape_api.Models.Primitives;
using Microsoft.EntityFrameworkCore;
using keepscape_api.Models.Checkouts.Products;
using keepscape_api.Models.Categories;

namespace keepscape_api.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) { }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDeliveryLog> OrderDeliveryLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<BuyerCategoryPreference> BuyerCategoryPreferences { get; set; }
        public DbSet<BuyerProfile> BuyerProfiles { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<BalanceHistory> BalanceHistories { get; set; }
        public DbSet<SellerApplication> SellerApplications { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductReport> ProductReports { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                
                if (entry.Entity is ISoftDeletable && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    ((ISoftDeletable)entry.Entity).DateTimeDeleted = DateTime.UtcNow;
                }
                if (entry.Entity is Base && entry.State == EntityState.Added)
                {
                    ((Base)entry.Entity).DateTimeCreated = DateTime.UtcNow;
                }
                if (entry.Entity is Base && entry.State == EntityState.Modified)
                {
                    ((Base)entry.Entity).DateTimeUpdated = DateTime.UtcNow;
                }
            }
        }

        private void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ISoftDeletable
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(entity => entity.DateTimeDeleted == null);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplySoftDeleteFilter<Product>(modelBuilder);
            modelBuilder.Entity<CartItem>().HasQueryFilter(ci => !ci.Product!.DateTimeDeleted.HasValue);
            modelBuilder.Entity<ProductReview>().HasQueryFilter(pr => !pr.Product!.DateTimeDeleted.HasValue);
            modelBuilder.Entity<ProductReport>().HasQueryFilter(pi => !pi.Product!.DateTimeDeleted.HasValue);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(e => e.UserType)
                .HasConversion<string>();
            
            // Profiles
            modelBuilder.Entity<BuyerProfile>()
                .HasOne(b => b.User)
                .WithOne(u => u.BuyerProfile)
                .HasForeignKey<BuyerProfile>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SellerProfile>()
                .HasOne(s => s.User)
                .WithOne(u => u.SellerProfile)
                .HasForeignKey<SellerProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SellerApplication>()
                .Property(e => e.Status)
                .HasConversion<string>();
            modelBuilder.Entity<SellerApplication>()
                .HasOne(sa => sa.SellerProfile)
                .WithOne(s => s.SellerApplication)
                .HasForeignKey<SellerApplication>(sa => sa.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Balance
            modelBuilder.Entity<Balance>()
                .HasOne(b => b.User)
                .WithOne(u => u.Balance)
                .HasForeignKey<Balance>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Balance>()
                .HasMany(b => b.BalanceHistories)
                .WithOne(bh => bh.Balance)
                .HasForeignKey(bh => bh.BalanceId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Balance>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<BalanceHistory>()
                .Property(bh => bh.Amount)
                .HasPrecision(18, 2);


            // Token and Code
            modelBuilder.Entity<Token>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
            modelBuilder.Entity<ConfirmationCode>()
                .HasOne(c => c.User)
                .WithMany(u => u.ConfirmationCodes)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            // Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.SellerProfile)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SellerProfileId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Categories)
                .WithMany(pc => pc.Products);
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ImageUrls)
                .WithOne();
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Place)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.PlaceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(pr => pr.Product)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Product>()
                .Property(p => p.Rating)
                .HasPrecision(18, 2);
            modelBuilder.Entity<ProductReport>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
            modelBuilder.Entity<ProductReport>()
                .HasOne(pr => pr.Product)
                .WithMany()
                .HasForeignKey(pr => pr.ProductGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true); 

            // Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.BuyerProfile)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.BuyerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(c => c.Cart)
                .HasForeignKey(c => c.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            // Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.BuyerProfile)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BuyerProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDeliveryLogs)
                .WithOne(odl => odl.Order)
                .HasForeignKey(odl => odl.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            // SellerApplication
            modelBuilder.Entity<SellerApplication>()
                .Property(e => e.Status)
                .HasConversion<string>();
        }
    }
}
