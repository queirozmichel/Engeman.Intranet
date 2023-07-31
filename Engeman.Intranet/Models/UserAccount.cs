namespace Engeman.Intranet.Models
{
  public class UserAccount
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] Photo { get; set; }
    public string Description { get; set; }
    public int DepartmentId { get; set; }
    public string Permissions { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
