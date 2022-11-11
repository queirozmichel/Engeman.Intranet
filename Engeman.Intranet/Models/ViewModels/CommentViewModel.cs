using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
  public class CommentViewModel
  {
    public CommentViewModel()
    {
      Files = new List<CommentFile>();
    }
    public int Id { get; set; }
    public string Description { get; set; }
    public bool Revised { get; set; }
    public List<CommentFile> Files { get; set; }
    public DateTime ChangeDate { get; set; }
    public string AuthorUsername { get; set; }
    public int AuthorId { get; set; }
    public string AuthorDepartment { get; set; }
    public byte[] AuthorPhoto { get; set; }
    public int AuthorPostsMade { get; set; }
    public int AuthorCommentsMade { get; set; }
  }
}
