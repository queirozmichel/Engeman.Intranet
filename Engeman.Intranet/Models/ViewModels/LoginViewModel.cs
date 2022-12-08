using System.ComponentModel.DataAnnotations;

namespace Engeman.Intranet.Models.ViewModels
{
  public class LoginViewModel
  {
    [Required]
    public string DomainAccount { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
