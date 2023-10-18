namespace keepscape_api.Dtos.Announcements
{
    public record AnnouncementResponseDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime DateTimeCreated { get; init; }
    }
    
    public record AnnouncementResponsePagedDto
    {
        public IEnumerable<AnnouncementResponseDto> Announcements { get; init; } = new List<AnnouncementResponseDto>();
        public int PageCount { get; init; }
    }
}
