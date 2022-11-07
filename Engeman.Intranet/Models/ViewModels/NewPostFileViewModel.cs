using System.ComponentModel.DataAnnotations;
using System;

namespace Engeman.Intranet.Models.ViewModels
{
  public class NewPostFileViewModel
  {
    public char FileType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] BinaryData { get; set; }
    public int PostId { get; set; }
  }
}