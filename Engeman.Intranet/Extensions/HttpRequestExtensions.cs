namespace Engeman.Intranet.Extensions
{
  public static class HttpRequestExtensions
  {
    /// <summary>
    /// Método usado para verificar se uma requisição é AJAX e utiliza o mesmo verbo HTTP passado como parâmetro.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="httpVerb">Verbo HTTP = GET, POST, DELETE e etc.</param>
    /// <returns>TRUE = caso seja uma requisição AJAX e utlize o verbo HTTP passado como parâmetro
    /// <br />FALSE = caso não seja uma requisição AJAX e/ou não utlize o verbo HTTP passado como parâmetro</returns>
    /// <exception cref="ArgumentNullException">Excessão de objeto nulo.</exception>
    public static bool IsAjax(this HttpRequest request, string httpVerb = "")
    {
      if (request == null) throw new ArgumentNullException ("O objeto de requisição é nulo.");

      if (!string.IsNullOrEmpty(httpVerb))
      {
        if (request.Method != httpVerb) return false;
      }

      if (request.Headers != null) return request.Headers["X-Requested-With"] == "XMLHttpRequest";

      return false;
    }
  }
}
