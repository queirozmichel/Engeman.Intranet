using Engeman.Intranet.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers
{
  public class RootController : Controller
  {
    /// <summary>
    /// Propriedade para acessar a classe "Service Configuration" e seus respectivos métodos.
    /// </summary>
    protected ServiceConfiguration ServiceConfiguration
    {
      get => HttpContext.RequestServices.GetService<ServiceConfiguration>();
    }

    /// <summary>
    /// Endpoint para obter a chave secreta presente no arquivo "app.settings.json", por meio da classe de serviços "Service Configuration".
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetCryptoSecretKey()
    {
      return ServiceConfiguration.GetCryptoSecretKey();
    }
  }
}
