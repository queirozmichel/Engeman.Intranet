var _title
var _body
var _id
var _selector

function showConfirmationModal(title, body, selector, id) {
  _title = title;
  _body = body;
  _id = id;
  _selector = selector;

  $(".confirmation-modal").find('.modal-title').text(_title);
  $(".confirmation-modal").find('.modal-body').text(_body);
  $(".confirmation-modal").find('.btn-yes').attr("data-id", _id);
  $(".confirmation-modal").find('.btn-yes').attr("id", _selector);
  $(".confirmation-modal").modal("show");
}

function hideConfirmationModal() {
  $(".confirmation-modal").modal("hide");
}

$('.confirmation-modal').on('hidden.bs.modal', function (e) {
  $("body").css("overflow", "scroll");
})

$('.confirmation-modal').on('show.bs.modal', function (e) {
  $("body").css("overflow", "hidden");
})