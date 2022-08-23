using System;

namespace Engeman.Intranet.Models
{
  public class PostCommentFile
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public char FileType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] BinaryData { get; set; }
    public int PostCommentId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
