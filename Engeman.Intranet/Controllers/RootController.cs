using Engeman.Intranet.Extensions;
using Engeman.Intranet.Helpers;
using Engeman.Intranet.Models;
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

    /// <summary>
    /// Cria um objeto do tipo Notification
    /// </summary>
    /// <param name="registryId">Id do registro(Postagem, comentário e etc.)</param>
    /// <param name="toUserId">Id do usuário na qual a notificação vai ser associada.</param>
    /// <param name="notificationTypeId">Id do tipo de notificação.</param>
    /// <param name="revised">Se a notificação(postagem ou comentário) foi ou não revisada.</param>
    /// <returns></returns>
    public Notification CreateNotification(int registryId, int toUserId, int notificationTypeId, bool revised = true)
    {
      var notification = new Notification()
      {
        RegistryId = registryId,
        ByUserId = HttpContext.Session.Get<int>("_CurrentUserId"),
        ToUserId = toUserId,
        NotifcationTypeId = notificationTypeId,
        Revised = revised
      };
      return notification;
    }
  }
}
