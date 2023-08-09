namespace Engeman.Intranet.Models
{
  public class Notification
  {
    public int Id { get; set; }
    public bool Viewed { get; set; }
    public int RegistryId { get; set; }
    public int ByUserId { get; set; }
    public int ToUserId { get; set; }
    public int NotifcationTypeId { get; set; }
    public string Description { get; set; }
    public bool Revised { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
