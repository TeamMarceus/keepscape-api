namespace keepscape_api.Models.Primitives
{
    public interface ISoftDeletable
    {
        public DateTime? DateTimeDeleted { get; set; }
    }
}
