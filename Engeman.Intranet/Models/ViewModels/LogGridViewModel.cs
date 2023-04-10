namespace Engeman.Intranet.Models.ViewModels
{
  public class LogGridViewModel
  {
    public int Id { get; set; }
    public string Operation { get; set; }
    public string RegistryType { get; set; }
    public int? RegistryId { get; set; }
    public string RegistryTable { get; set; }
    public string Username { get; set; }
    public string ChangeDate { get; set; }
  }
}