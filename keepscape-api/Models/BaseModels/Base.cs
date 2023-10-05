using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Models.BaseModels
{
    public class Base
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeUpdated { get; set; }
    }
}
