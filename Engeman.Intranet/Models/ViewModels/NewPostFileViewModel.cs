using System.ComponentModel.DataAnnotations;
using System;

namespace Engeman.Intranet.Models.ViewModels
{
  public class NewPostFileViewModel
  {
    public string Name { get; set; }
    public byte[] BinaryData { get; set; }
  }
}