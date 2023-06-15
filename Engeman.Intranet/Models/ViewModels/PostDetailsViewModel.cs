namespace Engeman.Intranet.Models.ViewModels
{
  public class PostDetailsViewModel
  {
    public PostDetailsViewModel()
    {
      Files = new List<PostFile>();
    }
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }
    public bool Revised { get; set; }
    public DateTime ChangeDate { get; set; }
    public List<PostFile> Files { get; set; }
    public int PostedDaysAgo { get; set; }
    public int AuthorId { get; set; }
    public int AuthorPostsMade { get; set; }
    public int AuthorCommentsMade { get; set; }
    public string AuthorUsername { get; set; }
    public string AuthorDepartment { get; set; }
    public byte[] AuthorPhoto { get; set; }
  }
}

