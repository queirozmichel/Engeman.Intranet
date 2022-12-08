using System;

namespace Engeman.Intranet.Models
{
  public class PostFile
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; }
    public byte[] BinaryData { get; set; }
    public int PostId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}