using Engeman.Intranet.Library;
using Engeman.Intranet.Models.ViewModels;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Engeman.Intranet.Helpers
{
  public class GlobalFunctions

  {/// <summary>
   /// Procedure para inserir um registro de log na tabela LOG.<br/>
   /// Caso precise adicionar mais valores para 'operation' e 'registry_type' buscar pela procedure 'NewLog'
   /// </summary>
   /// <param name="operation">I = Inserção D = Exclusão U = Atualização A = Aprovação.</param>
   /// <param name="registry_type">POS = Postagem COM = Comentário USU = Usuário TER = Termo.</param>
   /// <param name="registry_id">ID do registro que foi inserido, modificado ou excluído.</param>
   /// <param name="registry_table">Nome da tabela do registro que modificado, inserido ou excluído.</param>
   /// <param name="username">Nome de usuário corrente que fez a modificação, inserção ou exclusão.</param>
    public static void NewLog(char operation, string registry_type, int registry_id, string registry_table, string username)
    {
      var query = $"EXEC NEW_LOG '{operation}','{registry_type}',{registry_id},'{registry_table}','{username}'";
      using StaticQuery sq = new();

      sq.ExecuteCommand(query);
    }

    /// <summary>
    /// Remove caracteres especiais, espaços extras e espaços inicial e final utlizando Regex.
    /// </summary>
    /// <param name="text">cadeia de caracteres a ser limpa</param>
    /// <returns>uma string com o texto limpo</returns>
    public static string CleanText(string text)
    {
      string cleanedText = string.Empty;

      cleanedText = Regex.Replace(text, @"[^0-9a-zA-Zà-úÀ-Ú\s]+", " ");   //Remove todos os caracteres especiais
      cleanedText = Regex.Replace(cleanedText, @"\s{2,}", " ");                  //Remove espaços maiores ou iguais a 2
      cleanedText = Regex.Replace(cleanedText, @"(^\s+)|(\s+$)", string.Empty);  //Remove os espaços inicial e final 

      return cleanedText;
    }

    /// <summary>
    /// Converte uma cadeia de caracteres contendo tags HTML para texto puro
    /// </summary>
    /// <param name="htmlText">cadeia de caracteres contendo tags HTML</param>
    /// <returns></returns>
    public static string HTMLToTextConvert(string htmlText)
    {
      var htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(htmlText);
      var pureText = "";
      HtmlNode last;
      var nodes = htmlDocument.DocumentNode.SelectNodes("*");

      if (nodes == null)
      {
        return htmlText;
      }
      else
      {
        last = nodes.Last();

        foreach (var node in nodes)
        {
          if (node.Name == "table")
          {
            var tbody = node.ChildNodes[0];
            var aux = 0;

            while (aux < tbody.ChildNodes.Count)
            {
              var elements = tbody.ChildNodes[aux];
              foreach (var element in elements.ChildNodes)
              {
                pureText += element.InnerText + " ";
              }
              aux++;
            }
            continue;
          }

          pureText += node.InnerText;

          if (!node.InnerText.EndsWith(" ") && !node.Equals(last))
          {
            pureText += " ";
          }
        }
        pureText = Regex.Replace(pureText, Constants.EmojisPattern, " ");    //Remove todos os Emojis
        pureText = Regex.Replace(pureText, @"(&nbsp;)|(&lt;)|(&gt;)", " ");  // Remove as tags '&nbsp;' '&lt;' e '&gt;'

        return pureText;
      }
    }

    /// <summary>
    /// Testa se um usuário é considerado moderador.
    /// Para ser considerado moderador, todas os itens devem estar com o valor 1 exceto "RequiresModeration" que deve obrigatoriamente estar com o valor 0.
    /// Qualquer combinação diferente desta, é tratado como um usuário comum.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Retorna "True" caso seja moderador e "False" caso usuário comum.</returns>
    public static bool IsModerator(int userId)
    {
      var query = $"SELECT PERMISSIONS FROM USERACCOUNT WHERE ID = {userId}";
      using StaticQuery sq = new();
      var result = sq.GetDataToObject(query).ToString();

      if (string.IsNullOrEmpty(result) == true) return false;
      else
      {
        UserPermissionsViewModel permissions = JsonSerializer.Deserialize<UserPermissionsViewModel>(result);

        if (permissions.PostType.Informative.CanPost == 1 && permissions.PostType.Informative.CanComment == 1 && permissions.PostType.Informative.EditAnyPost == 1
          && permissions.PostType.Informative.DeleteAnyPost == 1 && permissions.PostType.Informative.RequiresModeration == 0 &&
          permissions.PostType.Question.CanPost == 1 && permissions.PostType.Question.CanComment == 1 && permissions.PostType.Question.EditAnyPost == 1
          && permissions.PostType.Question.DeleteAnyPost == 1 && permissions.PostType.Question.RequiresModeration == 0 &&
          permissions.PostType.Manual.CanPost == 1 && permissions.PostType.Manual.CanComment == 1 && permissions.PostType.Manual.EditAnyPost == 1
          && permissions.PostType.Manual.DeleteAnyPost == 1 && permissions.PostType.Manual.RequiresModeration == 0 &&
          permissions.PostType.Document.CanPost == 1 && permissions.PostType.Document.CanComment == 1 && permissions.PostType.Document.EditAnyPost == 1
          && permissions.PostType.Document.DeleteAnyPost == 1 && permissions.PostType.Document.RequiresModeration == 0)
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Testa se uma postagem ou comentários necessita de moderação/revisão.
    /// Se sim, é retornado TRUE, caso contrário, FALSE.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="postType">Tipo de postagem que está sendo verificada, se for um comentário, deve-se passar o tipo da postagem na qual o comentário está sendo feito. <br/> I = Informativa, Q = Pergunta D = Documento M = Manual.</param>
    /// <returns></returns>
    public static bool RequiresModeration(int userId, char postType)
    {
      var query = $"SELECT PERMISSIONS FROM USERACCOUNT WHERE ID = {userId}";
      using StaticQuery sq = new();
      var result = sq.GetDataToObject(query).ToString();

      if (string.IsNullOrEmpty(result) == true) return true;
      else
      {
        UserPermissionsViewModel permissions = JsonSerializer.Deserialize<UserPermissionsViewModel>(result);

        if (IsModerator(userId) == true)
        {
          return false;
        }
        else
        {
          if (postType == 'I')
          {
            if (permissions.PostType.Informative.RequiresModeration == 1)
            {
              return true;
            }
          }
          else if (postType == 'Q')
          {
            if (permissions.PostType.Question.RequiresModeration == 1)
            {
              return true;
            }
          }
          else if (postType == 'M')
          {
            if (permissions.PostType.Manual.RequiresModeration == 1)
            {
              return true;
            }
          }
          else if (postType == 'D')
          {
            if (permissions.PostType.Document.RequiresModeration == 1)
            {
              return true;
            }
          }
        }
        return false;
      }
    }
  }
}