namespace Engeman.Intranet.Models.ViewModels
{
  public class UserGridViewModel
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Department { get; set; }
    public bool Novice { get; set; }
    public bool Moderator { get; set; }
  }
}