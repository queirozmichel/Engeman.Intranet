using Engeman.Intranet.Library;

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
  }
}