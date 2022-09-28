$(document).ready(function () {
  "use strict";

  App.init(); // Init layout and core plugins
  Plugins.init(); // Init all plugins
  FormComponents.init(); // Init all form-specific plugins
  toastrConfig();  

})

function toastrConfig () {
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


$("#new-question-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/posts/newquestion",
    success: function (response) {
      if (response != false) {
        $(".body-content").empty();
        $(".body-content").html(response);
        window.history.pushState({}, '', '/posts/newquestion');
      } else {
        showDangerModal("Operação não suportada!","Você não tem permissão para criar uma nova postagem")
      }
    },
    error: function (response) {
      console.log(response);
    },
  })
})