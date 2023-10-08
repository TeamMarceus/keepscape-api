using keepscape_api.Models.Primitives;
using keepscape_api.Models.Images;
using keepscape_api.Models.Categories;

namespace keepscape_api.Models
{
    public class Product : Base, ISoftDeletable
    {
        public Guid? SellerProfileId { get; set; }
        public Guid? PlaceCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public int Quantity { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsHidden { get; set; } = false;
        public DateTime? DateTimeDeleted { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<BaseCategory> ProductCategories { get; set; } = new List<BaseCategory>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    }
}