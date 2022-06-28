using System;

namespace Engeman.Intranet.Models
{
  public class Archive
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public char ArchiveType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] BinaryData { get; set; }
    public int PostId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
