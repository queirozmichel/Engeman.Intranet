$(document).ready(function () {
})

$(".notification").on("click", function () {
  let registryCode = $(this).data("registry-code");
  let registryId = $(this).data("registry-id");

  if (registryCode == "NOCO") {
    postDetailsByCommentId(registryId);
  }

  fetch(`/notifications/makeviewedbyregistryid?registryId=${registryId}`)
    .then(response => {
      if (!response.ok) {
        throw Error();
      } else {
        RenderNotificationsComponent();
      }
    })
})

//Define todas as notificações como "visualizadas".
function MakeAllNotificationsViewed(toUserId) {
  fetch(`/notifications/makeviewedbytouserid?toUserId=${toUserId}`)
    .then(response => {
      if (!response.ok) {
        throw Error();
      } else {
        RenderNotificationsComponent();
      }
    })
}

//Renderiza na tela o componente de notificações.
function RenderNotificationsComponent() {
  fetch(`/notifications/notificationsviewcomponent`)
    .then(response => {
      if (!response.ok) {
        throw Error();
      }
      return response.text();
    }).then(html => {
      renderHTML(html, "notifications-component");
    })
}