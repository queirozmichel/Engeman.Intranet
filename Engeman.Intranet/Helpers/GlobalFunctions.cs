using Engeman.Intranet.Library;
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
   /// <param name="registry_type">POS = Postagem COM = Comentário USU = Usuário.</param>
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
      var nodes = htmlDocument.DocumentNode.SelectNodes("*");
      var last = nodes.Last();

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
}