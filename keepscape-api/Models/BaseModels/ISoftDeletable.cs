namespace keepscape_api.Models.BaseModels
{
    public interface ISoftDeletable
    {
        public DateTime? DateTimeDeleted { get; set; }
    }
}
