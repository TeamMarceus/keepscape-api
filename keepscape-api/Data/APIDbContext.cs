using keepscape_api.Models;
using keepscape_api.Models.Primitives;
using Microsoft.EntityFrameworkCore;
using keepscape_api.Models.Checkouts.Products;
using keepscape_api.Models.Categories;
using keepscape_api.Models.Users.Finances;
using keepscape_api.Models.Checkouts.Orders;

namespace keepscape_api.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) { }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderDeliveryLog> OrderDeliveryLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<BuyerCategoryPreference> BuyerCategoryPreferences { get; set; }
        public DbSet<BuyerProfile> BuyerProfiles { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<BalanceLog> BalanceLogs { get; set; }
        public DbSet<BalanceWithdrawal> BalanceWithdrawals { get; set; }
        public DbSet<SellerApplication> SellerApplications { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductReport> ProductReports { get; set; }
        public DbSet<OrderReport> OrderReports { get; set; }

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
            modelBuilder.Entity<BuyerProfile>()
                .HasOne(b => b.Cart)
                .WithOne(c => c.BuyerProfile)
                .HasForeignKey<Cart>(c => c.BuyerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BuyerProfile>()
                .HasMany(b => b.Orders)
                .WithOne(o => o.BuyerProfile)
                .HasForeignKey(o => o.BuyerProfileId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<BuyerProfile>()
                .HasMany(b => b.BuyerCategoryPreferences)
                .WithOne(bcp => bcp.BuyerProfile)
                .HasForeignKey(bcp => bcp.BuyerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BuyerProfile>()
                .HasMany(b => b.Orders)
                .WithOne(o => o.BuyerProfile)
                .HasForeignKey(o => o.BuyerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BuyerCategoryPreference>()
                .HasOne(bcp => bcp.Category)
                .WithMany()
                .HasForeignKey(bcp => bcp.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
            modelBuilder.Entity<SellerProfile>()
                .HasOne(s => s.User)
                .WithOne(u => u.SellerProfile)
                .HasForeignKey<SellerProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SellerApplication>()
                .Property(e => e.Status)
                .HasConversion<string>();
            modelBuilder.Entity<SellerProfile>()
                .HasMany(s => s.Products)
                .WithOne(p => p.SellerProfile)
                .HasForeignKey(p => p.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
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
            modelBuilder.Entity<BalanceLog>()
                .Property(bh => bh.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<BalanceWithdrawal>()
                .Property(bw => bw.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<BalanceWithdrawal>()
                .Property(bw => bw.PaymentMethod)
                .HasConversion<string>();
            modelBuilder.Entity<BalanceWithdrawal>()
                .Property(bw => bw.Status)
                .HasConversion<string>();

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
                .HasMany(p => p.Images)
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
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
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
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.SellerProfile)
                .WithMany()
                .HasForeignKey(o => o.SellerProfileId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.DeliveryLogs)
                .WithOne(odl => odl.Order)
                .HasForeignKey(odl => odl.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

            // SellerApplication
            modelBuilder.Entity<SellerApplication>()
                .Property(e => e.Status)
                .HasConversion<string>();
        }
    }
}
