using Engeman.Intranet.Attributes;
using Engeman.Intranet.Extensions;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models.ViewModels;
using HtmlAgilityPack;
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
    /// Para ser considerado moderador, todas os itens devem estar com o valor true exceto "RequiresModeration", que deve obrigatoriamente estar com o valor false.
    /// Qualquer combinação diferente desta, é tratado como um usuário comum.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Retorna "true" caso seja moderador e "false" caso usuário comum.</returns>
    public static bool IsModerator(int userId)
    {
      var query = $"SELECT PERMISSIONS FROM USERACCOUNT WHERE ID = {userId}";
      using StaticQuery sq = new();
      var result = sq.GetDataToObject(query).ToString();

      if (string.IsNullOrEmpty(result) == true) return false;
      else
      {
        UserPermissionsViewModel permissions = result.DeserializeAndConvertIntToBool<UserPermissionsViewModel>();

        if (permissions.PostType.Informative.CanPost == true && permissions.PostType.Informative.CanComment == true && permissions.PostType.Informative.EditAnyPost == true
          && permissions.PostType.Informative.DeleteAnyPost == true && permissions.PostType.Informative.RequiresModeration == false &&
          permissions.PostType.Question.CanPost == true && permissions.PostType.Question.CanComment == true && permissions.PostType.Question.EditAnyPost == true
          && permissions.PostType.Question.DeleteAnyPost == true && permissions.PostType.Question.RequiresModeration == false &&
          permissions.PostType.Manual.CanPost == true && permissions.PostType.Manual.CanComment == true && permissions.PostType.Manual.EditAnyPost == true
          && permissions.PostType.Manual.DeleteAnyPost == true && permissions.PostType.Manual.RequiresModeration == false &&
          permissions.PostType.Document.CanPost == true && permissions.PostType.Document.CanComment == true && permissions.PostType.Document.EditAnyPost == true
          && permissions.PostType.Document.DeleteAnyPost == true && permissions.PostType.Document.RequiresModeration == false)
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Testa se uma postagem ou comentários necessita de moderação/revisão.
    /// Se sim, é retornado true, caso contrário, false.
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
        UserPermissionsViewModel permissions = result.DeserializeAndConvertIntToBool<UserPermissionsViewModel>();

        if (IsModerator(userId) == true)
        {
          return false;
        }
        else
        {
          if (postType == 'I')
          {
            if (permissions.PostType.Informative.RequiresModeration == true)
            {
              return true;
            }
          }
          else if (postType == 'Q')
          {
            if (permissions.PostType.Question.RequiresModeration == true)
            {
              return true;
            }
          }
          else if (postType == 'M')
          {
            if (permissions.PostType.Manual.RequiresModeration == true)
            {
              return true;
            }
          }
          else if (postType == 'D')
          {
            if (permissions.PostType.Document.RequiresModeration == true)
            {
              return true;
            }
          }
        }
        return false;
      }
    }

    /// <summary>
    /// Calcula quantos dias se passaram a partir de uma data, e retorna um inteiro correspondente à quantidade de dias.
    /// </summary>
    /// <param name="date"></param>
    /// <returns>-1 = antes de meia noite, porém não um dia completo <br></br> Qualquer outro valor = quantidade de dias</returns>
    public static int DaysUntilToday(DateTime date)
    {
      var days = (DateTime.Now - date).Days;

      if (days == 0)
      {
        TimeSpan timeSpanPast = date.TimeOfDay;
        TimeSpan timeSpanPresent = DateTime.Now.TimeOfDay;
        if (timeSpanPast > timeSpanPresent)
        {
          return -1;
        }
      }
      return days;
    }

    /// <summary>
    /// Obtém a data e hora da última compilação.
    /// </summary>
    /// <returns></returns>
    public static DateTime? GetAssemblyBuildDateTime()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var attr = Attribute.GetCustomAttribute(assembly, typeof(BuildDateTimeAttribute)) as BuildDateTimeAttribute;
      if (DateTime.TryParse(attr?.DateTime, out DateTime dateTime))
        return dateTime;
      else
        return null;
    }
  }
}