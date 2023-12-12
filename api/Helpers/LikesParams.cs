namespace API.Helpers;

public class LikesParams : PaginationParams
{
    public Guid UserId { get; set; }
    public string Predicate { get; set; }
}