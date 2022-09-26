using System;
using System.ComponentModel.DataAnnotations;

namespace Engeman.Intranet.Models
{
  public class PostFile
  {
    public int Id { get; set; }
    public char Active { get; set; }
    [Required]
    public char FileType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] BinaryData { get; set; }
    public int PostId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
