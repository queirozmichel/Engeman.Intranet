using Engeman.Intranet.Extensions;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.ViewComponents
{
  public class NotificationsViewComponent : ViewComponent
  {
    private readonly INotificationRepository _notificationRepository;
    private readonly ICommentRepository _commentRepository;

    public NotificationsViewComponent(INotificationRepository notificationRepository, ICommentRepository commentRepository)
    {
      _notificationRepository = notificationRepository;
      _commentRepository = commentRepository;
    }

    public IViewComponentResult Invoke()
    {
      ViewBag.Notifications = _notificationRepository.GetByToUserId(HttpContext.Session.Get<int>("_CurrentUserId"));
      ViewBag.CurrentUserId = HttpContext.Session.Get<int>("_CurrentUserId");

      return View("Default");
    }
  }
}
