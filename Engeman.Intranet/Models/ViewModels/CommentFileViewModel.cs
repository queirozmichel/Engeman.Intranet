using System;

namespace Engeman.Intranet.Models.ViewModels
{
  public class CommentFileViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] BinaryData { get; set; }
    public int CommentId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
