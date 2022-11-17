$(document).ready(function () {
  "use strict";

  App.init(); // Init layout and core plugins
  Plugins.init(); // Init all plugins
  //FormComponents.init(); // Init all form-specific plugins
  toastrConfig();

})

function toastrConfig() {
  toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-bottom-center",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
  };
}


$("#new-question-btn, #new-document-btn, #new-manual-btn").on("click", function (event) {
  event.preventDefault();
  if (this.id == "new-question-btn") {
    var url = "/posts/newquestion";
  } else if (this.id == "new-document-btn") {
    var url = "/posts/newdocument";
  } else {
    var url = "/posts/newmanual";
  }
  $.ajax({
    type: "GET",
    url: url,
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      if (response != false) {
        $("#render-body").empty();
        $("#render-body").html(response);
        window.history.pushState({}, '', url);
      } else {
        showAlertModal("Operação não suportada!", "Você não tem permissão para criar uma nova postagem")
      }
    },
    error: function (response) {
      console.log(response);
    },
    complete: function () {
      closeSpinner();
    }
  })
})