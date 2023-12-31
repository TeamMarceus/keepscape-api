﻿using keepscape_api.Models.Primitives;
using keepscape_api.Models.Categories;
using keepscape_api.Models.Checkouts.Products;

namespace keepscape_api.Models
{
    public class Product : Base, ISoftDeletable
    {
        public Guid? SellerProfileId { get; set; }
        public Guid? PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal SellerPrice { get; set; }
        public decimal BuyerPrice { get; set; }
        public decimal Rating { get; set; } = 0;
        public int Quantity { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsHidden { get; set; } = false;
        public int TotalSold { get; set; } = 0;
        public DateTime? DateTimeDeleted { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
        public virtual Place? Place { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<ProductReport> Reports { get; set; } = new List<ProductReport>();
    }
}