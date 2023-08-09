using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface INotificationRepository
  {
    /// <summary>
    /// Adiciona uma nova notificação.
    /// </summary>
    /// <param name="notification"></param>
    public void Add(Notification notification);

    /// <summary>
    /// Obtém todas as notificações pelo TO_USER_ID, ou seja, pelo id de usuário na qual a notificação é direcionada.
    /// </summary>
    /// <param name="toUserId">Id de usuário na qual a notificação é direcionada</param>
    /// <returns></returns>
    public List<NotificationViewModel> GetByToUserId(int toUserId);

    /// <summary>
    /// Define todas as notificações como "visualizadas" pelo RegistryId
    /// </summary>
    /// <param name="registryId">Id do registro(postagem, comentário ou etc.)</param>
    public void MakeViewedByRegistryId(int registryId);

    /// <summary>
    /// Define todas as notificações como "visualizadas" pelo ToUserId
    /// </summary>
    /// <param name="toUserId">Id de usuário na qual a notificação é direcionada</param>
    public void MakeViewedByToUserId(int toUserId);

    /// <summary>
    /// Define todas as notificações como "revisadas" pelo RegistryId
    /// </summary>
    /// <param name="registryId">Id do registro(postagem, comentário ou etc.)</param>
    public void MakeRevisedByRegistryId(int registryId);
  }
}
