namespace Engeman.Intranet.Models.ViewModels
{
  public class LogGridViewModel
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Operation { get; set; }
    public string Description { get; set; }
    public int? ReferenceId { get; set; }
    public string ReferenceTable { get; set; }
    public string ChangeDate { get; set; }
  }
}