using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers
{
  public class NotificationsController : Controller
  {
    private readonly INotificationRepository _notificationRepository;

    public NotificationsController(INotificationRepository notificationRepository)
    {
      _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Define todas as notificações como "visualizadas" pelo RegistryId.
    /// </summary>
    /// <param name="registryId">Id do registro(postagem, comentário ou etc.)</param>
    public void MakeViewedByRegistryId(int registryId)
    {
      try
      {
        _notificationRepository.MakeViewedByRegistryId(registryId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Define todas as notificações como "visualizadas" pelo ToUserId
    /// </summary>
    /// <param name="toUserId">Id de usuário na qual a notificação é direcionada</param>
    public void MakeViewedByToUserId(int toUserId)
    {
      try
      {
        _notificationRepository.MakeViewedByToUserId(toUserId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Invoca o componente de notificações.
    /// </summary>
    /// <returns></returns>
    public IActionResult NotificationsViewComponent()
    {
      return ViewComponent("Notifications");
    }
  }
}
