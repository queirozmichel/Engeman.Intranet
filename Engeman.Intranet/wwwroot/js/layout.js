$(document).ready(function () {
  "use strict";

  App.init(); // Init layout and core plugins
  Plugins.init(); // Init all plugins
  //FormComponents.init(); // Init all form-specific plugins
  toastrConfig();
  setInterval(exclamation, 1000);

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


$("#new-post-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: url = "/posts/newpost",
    dataType: "html",
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

function exclamation() {
  var icon;
  icon = $(".blinked-exclamation");
  setTimeout(function () {
    icon.css("opacity", "0");
  }, 0);
  setTimeout(function () {
    icon.css("opacity", "1");
  }, 500);
}
