namespace Engeman.Intranet.Models.ViewModels
{
  public class PostEditViewModel
  {
    public PostEditViewModel()
    {
      Files = new List<PostFile>();
      PostTypeDictionary = new Dictionary<string, char>
      {
        {"Informativa", 'I'},
        {"Pergunta", 'Q'},
        {"Documento", 'D'},
        {"Manual", 'M'}
      };
    }

    private char _postType;
    private Dictionary<string, char> _postTypeDictionary;

    public int Id { get; set; }
    public bool Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public List<Keyword> KeywordsList { get; set; }
    public bool Revised { get; set; }
    public List<PostFile> Files { get; set; }
    public List<int> DepartmentsList { get; set; }
    public string PostTypeDescription { get; set; }
    public Dictionary<string, char> PostTypeDictionary { get; set; }
    public char PostType
    {
      get => _postType;
      set
      {
        _postType = value;
        if (value == 'I') PostTypeDescription = "Informativa";
        else if (value == 'Q') PostTypeDescription = "Pergunta";
        else if (value == 'M') PostTypeDescription = "Manual";
        else if (value == 'D') PostTypeDescription = "Documento";
        else PostTypeDescription = null;
      }
    }
  }
}
