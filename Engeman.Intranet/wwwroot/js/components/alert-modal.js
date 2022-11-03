var _title
var _body

function showAlertModal(title, body) {
  _title = title;
  _body = body;

  $(".alert-modal").find('.modal-title').text(_title);
  $(".alert-modal").find('.modal-body').text(_body);
  $(".alert-modal").modal("show");
}

function hideAlertModal() {
  $(".alert-modal").modal("hide");
}