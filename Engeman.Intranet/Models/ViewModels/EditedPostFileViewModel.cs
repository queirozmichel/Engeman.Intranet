using System.ComponentModel.DataAnnotations;
using System;

namespace Engeman.Intranet.Models.ViewModels
{
  public class EditedPostFileViewModel
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public char FileType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] BinaryData { get; set; }
    public int PostId { get; set; }
  }
}
