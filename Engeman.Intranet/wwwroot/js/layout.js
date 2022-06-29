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