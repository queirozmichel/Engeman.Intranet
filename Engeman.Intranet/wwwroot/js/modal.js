var _title
var _body

function showDangerModal(title, body) {
  _title = title;
  _body = body;
  $(".danger-modal").modal("show");
}

function showConfirmModal(title, body) {
  _title = title;
  _body = body;
  $(".confirm-modal").modal("show");
}

function hideConfirmModal() {
  $(".confirm-modal").modal("hide");
}

$(".danger-modal, .confirm-modal").on('show.bs.modal', function (event) {
  var modal = $(this);
  modal.find('.modal-title').text(_title);
  modal.find('.modal-body').text(_body);
});