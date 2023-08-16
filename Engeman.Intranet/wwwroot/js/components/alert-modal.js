function showAlertModal(headerText, bodyText) {

  document.getElementById('alert-modal').querySelector(".modal-header").innerHTML = "<h5>" + headerText + "</h5>"
  document.getElementById('alert-modal').querySelector(".modal-body").innerHTML = "<p>" + bodyText + "</p>"

  $('#alert-modal').modal('show');
}