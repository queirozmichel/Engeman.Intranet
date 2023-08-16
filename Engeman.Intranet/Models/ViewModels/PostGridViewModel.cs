namespace Engeman.Intranet.Models.ViewModels
{
  public class PostGridViewModel
  {
    public int Id { get; set; }
    public bool Restricted { get; set; }
    public string Subject { get; set; }
    public char PostType { get; set; }
    public bool Revised { get; set; }
    public int UserAccountId { get; set; }
    public string Department { get; set; }
    public string UserAccountName { get; set; }
    public string ChangeDate { get; set; }
    public bool UnrevisedComments { get; set; }
    public int Status
    {
      get
      {
        if (Revised == false || UnrevisedComments == true)
        {
          return 3;// 0 = success(verde) | 1 = info(azul) | 2 = warning(amarelo) | 3 = danger(vermelho)
        }
        else
        {
          return 999;
        }
      }
      set { }
    }
  }
}