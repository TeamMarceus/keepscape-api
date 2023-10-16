using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Models.Primitives
{
    public abstract class Base
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeUpdated { get; set; }
    }
}
