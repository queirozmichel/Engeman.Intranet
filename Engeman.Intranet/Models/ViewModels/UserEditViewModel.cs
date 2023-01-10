namespace Engeman.Intranet.Models.ViewModels
{
  public class UserEditViewModel
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] Photo { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentDescription { get; set; }
    public string Description { get; set; }
    public string UserType { get; set; }
    public int UserTypeCode { get; set; }
    public bool Novice { get; set; }
    public bool EditOwnerPost { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteOwnerPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool CreatePost { get; set; }
  }
}
