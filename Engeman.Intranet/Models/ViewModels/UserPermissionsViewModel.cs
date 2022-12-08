namespace Engeman.Intranet.Models.ViewModels
{
  public class UserPermissionsViewModel
  {
    public bool CreatePost { get; set; }
    public bool EditOwnerPost { get; set; }
    public bool DeleteOwnerPost { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool Moderator { get; set; }
    public bool NoviceUser { get; set; }
  }
}
