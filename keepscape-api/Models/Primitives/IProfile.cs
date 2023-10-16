namespace keepscape_api.Models.Primitives
{
    public interface IProfile
    {
        Guid UserId { get; set; }
        User? User { get; set; }
    }
}
