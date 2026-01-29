namespace Explorer.Blog.API.Dtos;

public enum BlogSortBy
{
    CREATEDAT = 0,
    SCORE = 1,
    COMMENTCOUNT=2
}

public enum SortDirection
{
    ASC = 0, 
    DESC = 1
}
public class FilterBlogDto 
{
    public BlogQualityStatusDto? QualityStatus { get; set; }
    public long? LocationId { get; set; }
    public int? MinComments { get; set; }
    public int? MinScore { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public BlogSortBy SortBy { get; set; } = BlogSortBy.CREATEDAT;
    public SortDirection SortDirection { get; set; } = SortDirection.ASC;
}
