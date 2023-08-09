namespace Engeman.Intranet.Models.ViewModels
{
  public class NotificationViewModel
  {
    public int RegistryId { get; set; }
    public string RegistryCode { get; set; }
    public string Username { get; set; }
    public byte[] UserPhoto { get; set; }
    public string RegistryDescription { get; set; }
    public int PastDays { get; set; }
  }
}
