using Engeman.Intranet.Helpers;
using Engeman.Intranet.Library;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public class NotificationRepository : INotificationRepository
  {
    public void Add(Notification notification)
    {
      var query = $"INSERT INTO NOTIFICATION(REGISTRY_ID, TO_USER_ID, BY_USER_ID, NOTIFICATION_TYPE_ID, DESCRIPTION, REVISED) " +
                  $"VALUES({notification.RegistryId},{notification.ToUserId}, {notification.ByUserId}, {notification.NotifcationTypeId}, " +
                  $"'{notification.Description}', '{notification.Revised}')";

      using StaticQuery sq = new();

      sq.ExecuteCommand(query);
    }

    public List<NotificationViewModel> GetByToUserId(int toUserId)
    {
      var query = $"SELECT N.REGISTRY_ID, NT.CODE AS REGISTRY_CODE, UA.NAME as USERNAME, UA.PHOTO as USER_PHOTO, NT.DESCRIPTION AS REGISTRY_DESCRIPTION, N.CHANGE_DATE " +
                  $"FROM NOTIFICATION AS N " +
                  $"INNER JOIN NOTIFICATIONTYPE AS NT ON NT.ID = N.NOTIFICATION_TYPE_ID " +
                  $"INNER JOIN USERACCOUNT AS UA ON UA.ID = N.BY_USER_ID " +
                  $"WHERE N.TO_USER_ID = {toUserId} AND N.VIEWED = 0 AND N.REVISED = 1";

      using StaticQuery sq = new();
      var result = sq.GetDataSet(query).Tables[0];
      var notifications = new List<NotificationViewModel>();

      for (int i = 0; i < result.Rows.Count; i++)
      {
        var notification = new NotificationViewModel
        {
          RegistryId = Convert.ToInt32(result.Rows[i]["registry_id"]),
          Username = result.Rows[i]["username"].ToString(),
          UserPhoto = (byte[])result.Rows[i]["user_photo"],
          RegistryCode = result.Rows[i]["registry_code"].ToString(),
          RegistryDescription = result.Rows[i]["registry_description"].ToString(),
          PastDays = GlobalFunctions.DaysUntilToday((DateTime)result.Rows[i]["change_date"]),
        };
        notifications.Add(notification);
      }
      return notifications;
    }

    public void MakeViewedByRegistryId(int registryId)
    {
      var query = $"UPDATE NOTIFICATION SET VIEWED = 1 WHERE REGISTRY_ID = {registryId}";

      StaticQuery sq = new();
      sq.ExecuteCommand(query);
    }

    public void MakeViewedByToUserId(int toUserId)
    {
      var query = $"UPDATE NOTIFICATION SET VIEWED = 1 WHERE TO_USER_ID = {toUserId}";

      StaticQuery sq = new();
      sq.ExecuteCommand(query);
    }

    public void MakeRevisedByRegistryId(int registryId)
    {
      var query = $"UPDATE NOTIFICATION SET REVISED = 1 WHERE REGISTRY_ID = {registryId}";

      StaticQuery sq = new();
      sq.ExecuteCommand(query);
    }
  }
}
