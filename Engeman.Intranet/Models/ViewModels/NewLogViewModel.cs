using System.ComponentModel;

namespace Engeman.Intranet.Models.ViewModels
{
  public class NewLogViewModel
  {
    /// <summary>
    /// Informar os parâmetros que irão compor o Log.
    /// </summary>
    /// <param name="username">nome de usuário atual. Ex: michel.queiroz</param>
    /// <param name="operation">o valor do decorador colocado acima do elemento do enumerador onde se especifica qual operação será realizada. Ex: Operation.Exclusion.GetEnumDescription()</param>
    /// <param name="referenceId">id do registro relacionado ao log. Pode ser passado o valor null. Ex: 1 ou null</param>
    /// <param name="referenceTable">o valor do decorador colocado acima do elemento do enumerador onde se especifica o nome da tabela a qual pertence o registro relacionado ao log. Pode ser passado o valor null. Ex: ReferenceTable.Post.GetEnumDescription() ou null</param>
    public NewLogViewModel(string username, string operation, int? referenceId, string referenceTable)
    {
      Username = username;
      Operation = operation;
      ReferenceId = referenceId;
      ReferenceTable = referenceTable;
    }

    public string Username { get; set; }
    public string Operation { get; set; }
    public string Description { get; set; }
    public int? ReferenceId { get; set; }
    public string ReferenceTable { get; set; }
  }
  public enum Operation
  {
    [Description("Inclusão")]
    Inclusion,
    [Description("Alteração")]
    Alteration,
    [Description("Exclusão")]
    Exclusion,
    [Description("Aprovação")]
    Approval
  }

  public enum ReferenceTable
  {
    [Description("COMMENT")]
    Comment,
    [Description("POST")]
    Post,
  }
}